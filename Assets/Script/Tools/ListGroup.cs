using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ListGroup
{
    public GameObject BaseItem;
    public Transform Parent;

    List<GameObject> PoolData = new List<GameObject>();

    public List<GameObject> HottingObj = new List<GameObject>();

    public ListGroup(GameObject item, Transform parent)
    {
        this.Parent = parent;
        item.SetActive(false);
        this.BaseItem = item;
        PoolData.Add(item);
        BaseItem.AddComponent<TransformDataBase>().Init(BaseItem.transform);
    }


    public GameObject Instantiate()
    {
    
        GameObject CurrentObj = null;
        if (PoolData.Count > 0)
        {
            CurrentObj = PoolData[0];
            PoolData.RemoveAt(0);
        }
        else
        {
            CurrentObj = GameObject.Instantiate(BaseItem);
        }

        CurrentObj.transform.SetParent(Parent);
        CurrentObj.transform.localScale = Vector3.one;
        CurrentObj.transform.localPosition = Vector3.zero;
        CurrentObj.SetActive(true);
        HottingObj.Add(CurrentObj);

        return CurrentObj;
    }

    public void Destory(GameObject Obj, Action action = null)
    {
        if (action != null)
            action();
        Obj.SetActive(false);
        Obj.transform.SetParent(null);
        Button[] btns = Obj.GetComponentsInChildren<Button>();
        foreach (Button child in btns)
            child.onClick.RemoveAllListeners();
        PoolData.Add(Obj);
        HottingObj.Remove(Obj);
    }

    public TransformDataBase InstantiateTransform()
    {
        GameObject CurrentObj = Instantiate();
        return CurrentObj.GetComponent<TransformDataBase>();
    }

    public void Clear()
    {
        GameObject[] HotList = HottingObj.ToArray();
        foreach (GameObject child in HotList)
        {
            Destory(child.gameObject);
        }
    }
}

