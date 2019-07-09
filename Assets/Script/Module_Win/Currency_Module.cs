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
    private bool isEorroWorningShow;

    protected override void Init()
    {
        base.Init();
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        Util.CallMethod(LuaFail, "GetJson",JsonMapper.ToJson(EventData.Data));
        if (EventData.Code == HttpCode.SUCCESS && isSuccessWorningShow)
            MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
        if(EventData.Code == HttpCode.FAILED && isFailedWorningShow)
            MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
        if (EventData.Code == HttpCode.ERROR && isEorroWorningShow)
            MessageManager.GetMessageManager.WindowShowMessage(EventData.ErroMsg);
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

    public void SendDate(string obj,bool isSuc = true,bool isFail = true,bool isEorr = true)
    {
        isSuccessWorningShow = isSuc;
        isFailedWorningShow = isFail;
        isEorroWorningShow = isEorr;
        LuaFail = obj;
        Send();
    }

    public void SendVerCode(string url,GameObject btn_VerCode,string tel_key,string tel_value)
    {

        HttpVerCode VerCodeMole = new HttpVerCode(url, btn_VerCode.GetComponent<Button>(), tel_key,tel_value);
    
    }
}
