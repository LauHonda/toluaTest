using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDataBase : MonoBehaviour
{

    public void Init(Transform Parent)
    {
        foreach (Transform child in Parent)
        {
            Add(child.name, child);
        }
    }

    private Dictionary<string, Transform> BodyDic = new Dictionary<string, Transform>();


    public Transform GetTransform(string KEY)
    {
        Transform Obj = null;
        BodyDic.TryGetValue(KEY, out Obj);
        return Obj;
    }

    public U Get<U>(string KEY) where U : UnityEngine.Object
    {
        Transform Obj = null;
        bool isget = BodyDic.TryGetValue(KEY, out Obj);
        if (isget)
            return Obj.GetComponent<U>();
        return null;
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
}
