using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NumFollow : MonoBehaviour {


    [SerializeField]
    Text TEXT;
    [SerializeField]
    InputField inputTEXT;
    public void Change(InputField INPUT)
    {
        TEXT.text = (int.Parse(INPUT.text)*10).ToString();
    }

    public void ChangeInput(InputField INPUT)
    {
        Debug.Log(INPUT.text);
        float bili =float.Parse(CachingRegion.Get("pigbl"));
        TEXT.text = (int.Parse(INPUT.text) / bili).ToString();
    }

}
