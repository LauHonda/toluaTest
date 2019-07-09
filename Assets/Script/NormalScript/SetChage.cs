using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetChage : MonoBehaviour {


    [SerializeField]
    GameObject OK,LOST;

    private void Start()
    {
        OK.SetActive(true);
        LOST.SetActive(false);
    }

    public void CAHNGE(Toggle TG)
    {
        OK.SetActive(TG.isOn);
        LOST.SetActive(!TG.isOn);
    }

}
