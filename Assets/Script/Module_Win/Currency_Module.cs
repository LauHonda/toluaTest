using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using LuaInterface;
using LuaFramework;
using Tools;

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
        if (EventData.Code == HttpCode.SUCCESS )
        {
            if(isSuccessWorningShow)
                MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
            Util.CallMethod(LuaFail, "GetJson_Suc_CallBack", JsonMapper.ToJson(EventData.Data));
        }

        if (EventData.Code == HttpCode.FAILED)
        {
            if(isFailedWorningShow)
                MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
            Util.CallMethod(LuaFail, "GetJson_Fail_CallBack", JsonMapper.ToJson(EventData.Data));
        }

        if (EventData.Code == HttpCode.ERROR)
        {
            if(isEorroWorningShow)
                MessageManager.GetMessageManager.WindowShowMessage(EventData.ErroMsg);
            Util.CallMethod(LuaFail, "GetJson_Eorr_CallBack");
        }

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


    public void SendDate(string obj, bool isSuc = true, bool isFail = true, bool isEorr = true)
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

    Http newhttp;

    public void Creat_Request(string url)
    {
        newhttp = HttpCreatTools.CreatHttp(url);
        //newhttp.Clear();
    }
    public void Creat_Request_AddSendValue(string key, string value)
    {
        newhttp.AddData(key, value);
    }

    public void Creat_Request_SendDate(string obj,string CallBack, bool isSuc = true, bool isFail = true, bool isEorr = true)
    {
        LuaFail = obj;
        
        newhttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
        
            if (msg.Code == HttpCode.SUCCESS)
            {
                if(isSuc)
                    MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                Util.CallMethod(LuaFail, CallBack+ "_Suc_CallBack", JsonMapper.ToJson(msg.Data));
            }

            if (msg.Code == HttpCode.FAILED)
            {
                if(isFail)
                    MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                Util.CallMethod(LuaFail, CallBack + "_Fail_CallBack", JsonMapper.ToJson(msg.Data));
            }

            if (msg.Code == HttpCode.ERROR)
            {
                if(isEorr)
                    MessageManager.GetMessageManager.WindowShowMessage(msg.ErroMsg);
                Util.CallMethod(LuaFail, CallBack + "_Eorr_CallBack");
            }
            
        });
        newhttp.Send(true);
    }
}
