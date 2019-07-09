using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MessageManager : MonoBehaviour
{

    public static MessageManager GetMessageManager;

    private ShowMessageBar ShowMessage;

    private GameObject ShowLoad;

    private void Awake()
    {
        GetMessageManager = this;
    }


    void Start()
    {
        var obj = Resources.Load("Win/LoadLogo");
        ShowLoad = GameObject.Instantiate(obj) as GameObject;


        var Bar = Resources.Load("Win/MessageBar");
        GameObject barobj = GameObject.Instantiate(Bar) as GameObject;
        ShowMessage = barobj.GetComponent<ShowMessageBar>();

        var win = Resources.Load("Win/WindowMessage");
        Window = GameObject.Instantiate(win) as GameObject;

        var CanvasObj = FindObjectOfType<Canvas>();

        if (CanvasObj != null)
        {
            ShowLoad.transform.SetParent(CanvasObj.transform);
            Window.transform.SetParent(CanvasObj.transform);
            barobj.transform.SetParent(CanvasObj.transform);

            ShowLoad.transform.localPosition = Vector3.zero;
            Window.transform.localPosition = Vector3.zero;
            barobj.transform.localPosition = Vector3.zero;
            ShowLoad.transform.localScale = Vector3.zero;
            Window.transform.localScale = Vector3.zero;
            barobj.transform.localScale = Vector3.zero;
        }
    }

    public void ShowBar(string GetMessage, float time = 0)
    {
        ShowMessage.SetMessage(GetMessage, time);
    }

    private GameObject Window;

    public void WindowShowMessage(string GetText, string title = GlobalData.prompt)
    {
        TransformData transformData = Window.GetComponent<TransformData>().Init();

        transformData.Get<Text>("Message").text = GetText;
        transformData.Get<Text>("titleTxt").text = title;
        Button closeBtn = transformData.Get<Button>("close");
        WindowsManager.GetWindowsManager.OpenWindow(Window.transform);
    }

    public void WindowShowMessage(string GetText, UnityEngine.Events.UnityAction action, string buttonName, string title = GlobalData.prompt, bool isCloseSelf = true)
    {
        TransformData transformData = Window.GetComponent<TransformData>().Init();
        transformData.Get<Text>("Message").text = GetText;
        transformData.Get<Text>("titleTxt").text = title;
        Button submitBtn = transformData.Get<Button>("btn_sure");
        submitBtn.gameObject.SetActive(true);
        submitBtn.GetComponentInChildren<Text>().text = buttonName;

        submitBtn.onClick.RemoveAllListeners();

        if (isCloseSelf)
            submitBtn.onClick.AddListener(() => { WindowsManager.GetWindowsManager.CloseWindow(Window.transform); });

        if (action != null)
            submitBtn.onClick.AddListener(action);

        WindowsManager.GetWindowsManager.OpenWindow(Window.transform);
    }

    public void QuiteGame()
    {
        SceneManager.LoadScene("mainmeun");
    }

    [HideInInspector]
    public int LoadNub = 0;
    [HideInInspector]
    public int StatusNub = 0;

    public void AddLockNub()
    {
        LoadNub++;
    }
    public void DisLockNub()
    {
        LoadNub--;
    }

    private void Update()
    {
        if (LoadNub <= 0)
        {
            ShowLoad.transform.localScale = new Vector3(0, 0, 0);
            StatusNub = 0;
            LoadNub = 0;
        }
        else
        {
            ShowLoad.transform.localScale = new Vector3(1, 1, 1);
            StatusNub++;
        }
    }



    //ÍøÂçÄ£¿é´¢´æ
    public DicBase<Http> Http = new DicBase<Http>();
    //µØÖ·´¢´æÄ£¿é
    public DicBase<HttpBase> HttpModel = new DicBase<HttpBase>();
}
