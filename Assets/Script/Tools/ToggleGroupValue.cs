using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupValue : MonoBehaviour {

    Toggle[] toggle;
    
	// Use this for initialization
	void Start ()
    {
        toggle= GetComponentsInChildren<Toggle>();
    }

    public string GetValue()
    {
        string valuestr = null;
        for (int i=0;i<toggle.Length;i++)
        {
            if (toggle[i].isOn)
            {
                valuestr = i.ToString();
                break;
            }
        }

        return valuestr;
    }
}
