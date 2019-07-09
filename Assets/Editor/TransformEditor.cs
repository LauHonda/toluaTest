//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System;

//[CustomEditor(typeof(RectTransform))]
//public class MyTest : DecoratorEditor
//{
//    RectTransform trans;
//    bool IsWind = false;

//    void Init()
//    {
//        trans = (RectTransform)target;
//        if (trans.localScale.x == 1 || trans.localScale.y == 1 || trans.localScale.y == 1)
//        {
//            IsOpen = true;
//        }
//        else
//        {
//            IsOpen = false;
//        }

//        ButtonTag = IsOpen ? "CloseWindow" : "OpenWindow";
//        AllName.Clear();
//        foreach (var item in Enum.GetValues(typeof(SizeType)))
//        {
//            AllName.Add(item.ToString());
//        }
//    }

//    private void OnEnable()
//    {
//        Init();
//    }


//    List<string> AllName = new List<string>();
//    private bool IsOpen = false;
//    private string ButtonTag = "OpneWindow";
//    private string Win_Desc = "As a window";
//    private int CurrentWinType = 0;

//    public MyTest() : base("RectTransformEditor") { }
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        GUILayout.BeginHorizontal();
//        {
//            IsWind = GUILayout.Toggle(IsWind, Win_Desc);
//            // EditorGUI.BeginDisabledGroup(!IsWind);
//            CurrentWinType = EditorGUILayout.Popup(CurrentWinType, AllName.ToArray());
//            if (GUILayout.Button(ButtonTag))
//            {
//                IsOpen = !IsOpen;
//                ButtonTag = IsOpen ? "CloseWindow" : "OpenWindow";
//                trans.localScale = IsOpen ? Vector3.one : Vector3.zero;
//            }
//            //  EditorGUI.EndDisabledGroup();
//        }
//        GUILayout.EndHorizontal();
//    }
//}