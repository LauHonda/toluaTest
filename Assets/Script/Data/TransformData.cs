using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformData : MonoBehaviour
{
    public Transform Win;
    private Dictionary<string, Transform> BodyDic = new Dictionary<string, Transform>();

    void Awake()
    {
        Init();
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="U">要获取的组件类型</typeparam>
    /// <param name="KEY">要获取的对象名称</param>
    /// <param name="ChildNode">从对象的子节点层级获取=>层级序号</param>
    /// <returns></returns>
    public U Get<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponent<U>();
        return null;
    }


    public U GetUIComponentInChildren<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponentInChildren<U>();
        return null;
    }

    public IEnumerable GetUIComponentsInChildren<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponentsInChildren<U>();
        return null;
    }


    public U GetUIComponentInParent<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponentInParent<U>();
        return null;
    }

    public IEnumerable GetUIComponentsInParent<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponentsInParent<U>();
        return null;
    }

    public Transform GetTransform(string KEY)
    {     
        Transform Obj = null;
        BodyDic.TryGetValue(KEY, out Obj);
        return Obj;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="KEY">组件名称</param>
    /// <param name="Clear">是否初始化组件</param>
    /// <returns></returns>
    public InputField GetInputField(string KEY, bool Clear = false)
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
        {
            InputField input = Obj.GetComponent<InputField>();
            if (Clear)
                input.text = "";
            return input;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="KEY">组件名称</param>
    /// <param name="Clear">是否初始化组件</param>
    /// <returns></returns>
    public Text GetText(string KEY, bool Clear = false)
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
        {
            Text text = Obj.GetComponent<Text>();
            if (Clear)
                text.text = "";
            return text;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="KEY">组件名称</param>
    /// <param name="Clear">是否初始化组件</param>
    /// <returns></returns>
    public Button GetButton(string KEY, bool Clear = false)
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
        {
            Button btn = Obj.GetComponent<Button>();
            if (Clear)
                btn.onClick.RemoveAllListeners();
            return btn;
        }
        return null;
    }

    public void DoLayout(LitJson.JsonData data)
    {
        if (JsonString.Count <= 0)
            return;
        foreach (TransformObj child in body)
        {
            string j_str = JsonString[child.Num];
            if (j_str == "none")
                continue;

            Text text = child.Obj.GetComponent<Text>();
            if (text != null)
            {
                text.text = data[j_str].ToString();
            }
            else
            {
                InputField Ftext = child.Obj.GetComponent<InputField>();
                if (Ftext != null)
                {
                    Ftext.text = data[j_str].ToString();
                }
            }
        }
    }

    public void Add(string Key, Transform Value)
    {
        if (BodyDic.ContainsKey(Key))
            BodyDic.Remove(Key);
        BodyDic.Add(Key, Value);
    }

    public void Remove(string Key)
    {
        if (BodyDic.ContainsKey(Key))
            BodyDic.Remove(Key);
    }

    public List<TransformObj> body = new List<TransformObj>();

    public void AddBody(TransformObj item)
    {
        body.Add(item);
        //Add(item.Obj.name, item.Obj);
    }

    public void RemoveBody(TransformObj item)
    {
        body.Remove(item);
        // Remove(item.Obj.name);
    }

    public List<TransformObj> SelfBody = new List<TransformObj>();

    public bool Contains(Transform Obj)
    {
        foreach (TransformObj child in body)
        {
            if (child.Obj == Obj)
            {
                return true;
            }
        }
        return false;
    }

    public List<string> JsonString = new List<string>();

    public TransformData Init()
    {
        foreach (TransformObj child in body)
        {
            Add(child.Obj.name, child.Obj);

        }
        return this;
    }
}

[System.Serializable]
public class TransformObj
{
    public Transform Obj;
    public string Name;
    public int Num;
}

