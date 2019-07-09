using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnNum : MonoBehaviour {


    [SerializeField]
    Transform group;
    List<Vector3> posgroup = new List<Vector3>();
    void Start()
    {
        foreach (Transform child in group)
            posgroup.Add(child.position);
    }

    public void ChangeNumUp()
    {
        int count = group.childCount;
        group.GetChild(0).SetSiblingIndex(count - 1);
        for (int i = 0; i < count; i++)
        {
            if (i < 3)
            {
                group.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                group.GetChild(i).gameObject.SetActive(false);
            }
            group.GetChild(i).position = posgroup[i];
        }
    }
    public void ChangeNumDown()
    {
        int count = group.childCount;
        group.GetChild(count - 1).SetSiblingIndex(0);
        for (int i = 0; i < count; i++)
        {
            if (i < 3)
            {
                group.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                group.GetChild(i).gameObject.SetActive(false);
            }
            group.GetChild(i).position = posgroup[i];
        }
    }
}
