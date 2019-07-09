using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonState : MonoBehaviour
{

    private Image btn;
    [SerializeField]
    private Sprite Normal, Press;
    private bool IsChose = false;

    public bool GetState { get { return IsChose; } }
    // Use this for initialization
    void Start()
    {  
        //btn.onClick.AddListener(delegate() 
        //{
        //    IsChose = !IsChose;
        //    btn.GetComponent<Image>().sprite = IsChose ? Press : Normal;
        //});
    }

    public bool ChangeState()
    {
        btn = GetComponent<Button>().image;
        IsChose = !IsChose;
        btn.GetComponent<Image>().sprite = IsChose ? Press : Normal;
        return IsChose;
    }

    public bool SetState(bool IsDo)
    {
        if (GetState != IsDo)
            ChangeState();
        return GetState;
    }
}
