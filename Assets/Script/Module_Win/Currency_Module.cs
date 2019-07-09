using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using LuaInterface;
using LuaFramework;

public class Currency_Module : HttpTransformBLL
{

    private string Url = "";

    private string LuaFail;

    private bool isSuccessWorningShow;
    private bool isFailedWorningShow;

    protected override void Init()
    {
        base.Init();
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        Util.CallMethod(LuaFail, "GetJson",JsonMapper.ToJson(EventData.Data));
        if (EventData.Code == HttpCode.SUCCESS)
            MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
        if(EventData.Code == HttpCode.FAILED)
            MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
    }

    protected override string InitUrl()
    {
        return Url;
    }

    public void AddSendValue(string key, string value)
    {
        Data.AddData(key, value);
    }

    public void SetUrl(string url)
    {
        Data.SetUrl(url);
    }

    public void SendDate(string obj)
    {
        LuaFail = obj;
        Send();
    }

    public void ToLuaFile()
    {


    }

    public void test(string value)
    {
        MessageManager.GetMessageManager.WindowShowMessage(value);
        SetUrl("ajax_login.php");
        AddSendValue("tel","123456");
        AddSendValue("password", "123456");
        AddSendValue("yzm", "123456");
        SendDate("TestCtrl");
    }
}
