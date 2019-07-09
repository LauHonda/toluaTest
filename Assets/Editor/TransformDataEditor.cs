using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using LitJson;

[CustomEditor(typeof(TransformData))]
public class TransformDataEditor : Editor
{
    private ReorderableList TransformList;
    private ReorderableList SelfTransformList;

    protected SerializedObject SelfserializedObject;

    public string JosnString;

    TransformData myModel;


    private void OnEnable()
    {
        SelfserializedObject = new SerializedObject(this);

        myModel = (TransformData)target;

        SelfTransformList = new ReorderableList(serializedObject,
        serializedObject.FindProperty("SelfBody"),
        false, true, false, false);

        TransformList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("body"),
            true, true, false, false);

        TransformList.drawElementCallback += DrawMessageList;
        SelfTransformList.drawElementCallback += SelfDrawMessageList;



        SelfTransformList.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "未添加组件");
        };

        SelfTransformList.onSelectCallback += onSelectCallback;

        TransformList.onSelectCallback += onSelectCallbackBase;

        TransformList.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "已添加组件");
        };

        ReSet();
    }

    private void ReSet()
    {
        if (myModel.Win == null)
        {
            // var CanvasObj = FindObjectOfType<Canvas>();

            //if (CanvasObj != null)
            //  {
            // myModel.Win = CanvasObj.transform;
            // }

            myModel.Win = myModel.transform;
        }

        myModel.SelfBody.Clear();

        foreach (Transform child in myModel.Win)
        {
            TransformObj item = new TransformObj();
            item.Obj = child;
            myModel.SelfBody.Add(item);
        }
    }


    private void onSelectCallback(ReorderableList list)
    {
        myModel.Win = myModel.SelfBody[list.index].Obj;
        ReSet();
    }

    private void onSelectCallbackBase(ReorderableList list)
    {
        myModel.Win = myModel.body[list.index].Obj;
        ReSet();
    }

    private void SelfDrawMessageList(Rect rect, int index, bool selected, bool focused)
    {
        if (myModel.SelfBody.Count <= index)
            return;

        if (myModel.SelfBody[index].Obj == null)
        {
            myModel.SelfBody.RemoveAt(index);
            return;
        }

        SerializedProperty element = SelfTransformList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, (rect.width / 5) * 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Obj"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.x + (rect.width / 5) * 2, rect.y, (rect.width / 5), EditorGUIUtility.singleLineHeight), myModel.SelfBody[index].Obj.name);

        TransformObj obj = myModel.SelfBody[index];

        if (!myModel.Contains(obj.Obj))
        {
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(rect.x + (rect.width / 5) * 4, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight), "Add"))
            {
                myModel.AddBody(obj);
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void DrawMessageList(Rect rect, int index, bool selected, bool focused)
    {
        if (myModel.body[index].Obj == null)
        {
            myModel.RemoveBody(myModel.body[index]);
            return;
        }
        SerializedProperty element = TransformList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;


        EditorGUI.PropertyField(new Rect(rect.x, rect.y, (rect.width / 5), EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Obj"), GUIContent.none);

        if (GUI.Button(new Rect(rect.x + (rect.width / 5) + 10, rect.y, 20, EditorGUIUtility.singleLineHeight), "C"))
        {
            TextEditor t = new TextEditor();
            t.content = new GUIContent(myModel.body[index].Obj.name);
            t.OnFocus();
            t.Copy();
        }

        EditorGUI.LabelField(new Rect(rect.x + (rect.width / 5) + 40, rect.y, (rect.width / 5), EditorGUIUtility.singleLineHeight), myModel.body[index].Obj.name);
        if (myModel.JsonString.Count > 0)
        {
            myModel.body[index].Num = EditorGUI.Popup(new Rect(rect.x + (rect.width / 5) * 2 + 40, rect.y, (rect.width / 5), EditorGUIUtility.singleLineHeight), myModel.body[index].Num,
                myModel.JsonString.ToArray());
        }

        GUI.backgroundColor = Color.gray;
        if (GUI.Button(new Rect(rect.x + (rect.width / 5) * 4, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight), "Remove"))
        {
            TransformObj obj = myModel.body[index];
            myModel.RemoveBody(obj);
        }
        GUI.backgroundColor = Color.white;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Win"));

        if (GUILayout.Button("返回上级"))
        {
            if (myModel.Win.parent != null)
                myModel.Win = myModel.Win.parent;
            ReSet();
        }

        if (myModel.SelfBody.Count > 0)
            SelfTransformList.DoLayoutList();
        else
            EditorGUILayout.HelpBox("当前节点下没有任何组件", MessageType.Info);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("移除所有"))
            {
                myModel.body.Clear();
            }
            if (GUILayout.Button("添加所有"))
            {
                foreach (TransformObj child in myModel.SelfBody)
                    if (!myModel.body.Contains(child))
                        myModel.body.Add(child);
            }

        }
        GUILayout.EndHorizontal();
        if (myModel.body.Count > 0)
            TransformList.DoLayoutList();
        else
            EditorGUILayout.HelpBox("没有添加任何组件", MessageType.Info);


        JosnString = EditorGUILayout.TextField("修改文件的名称:", JosnString);

        //if (GUILayout.Button("初始化组件"))
        //{
        //    myModel.Init();
        //}

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("清除字段"))
            {
                myModel.JsonString.Clear();
            }

            if (GUILayout.Button("解析字段"))
            {
                myModel.JsonString = SerializeObjectString(JosnString);
            }
        }
        GUILayout.EndHorizontal();
        Undo.RecordObject(target, "target change");

        serializedObject.ApplyModifiedProperties();
    }


    private List<string> SerializeObjectString(string result)
    {
        List<string> KeyList = new List<string>();

        try
        {
            Dictionary<string, object> newtable = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            KeyList.Add("none");
            foreach (var child in newtable)
            {
                KeyList.Add(child.Key);
            }
        }
        catch
        {
            Debug.Log("解析失败");
        }
        return KeyList;
    }

}