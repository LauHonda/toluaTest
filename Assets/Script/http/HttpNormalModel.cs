using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


//手机验证码
public class HttpVerCode
{
    public HttpVerCode(string URL, Button Btn, InputField tel)
    {
        InitString(URL, Btn, tel);
    }

    public HttpVerCode(string URL, Button Btn, string tel)
    {
        InitString(URL, Btn, tel);
    }

    private void InitString(string URL, Button Bnt, string tel)
    {
        Bnt.onClick.AddListener(delegate ()
        {
            if (VerCodehtttp == null)
                VerCodehtttp = new Http(URL);

            VerCodehtttp.AddData("tel", tel);
            VerCodehtttp.Send();
        });
    }

    private void InitString(string URL, Button Bnt, InputField tel)
    {
        Bnt.onClick.AddListener(delegate ()
        {
            if (VerCodehtttp == null)
                VerCodehtttp = new Http(URL);

            VerCodehtttp.AddData("tel", tel);
            VerCodehtttp.Send();
        });
    }

    Http VerCodehtttp;
}


//图片验证码
public class HttpImgVerCode
{
    Http VerCodehtttp;
    CreatImgVerCode m_CreatImgVerCode;

    public HttpImgVerCode(string url, CreatImgVerCode m_CreatImgVerCode)
    {
        if (VerCodehtttp == null)
        {
            VerCodehtttp = new Http(url);
            this.m_CreatImgVerCode = m_CreatImgVerCode;
        }
    }


    public void AddEvent(UnityEvent BtnEvent, InputField tel)
    {
        BtnEvent.AddListener(delegate ()
        {
            Do();
            VerCodehtttp.AddData("tel", tel);
            VerCodehtttp.Send();
        });

    }

    public void AddEvent(UnityEvent<string> BtnEvent)
    {
        BtnEvent.AddListener(delegate (string tel)
        {
            Do();
            VerCodehtttp.AddData("tel", tel);
            VerCodehtttp.Send();
        });
    }


    public void Send(string tel = null)
    {
        if (string.IsNullOrEmpty(tel))
        {
            return;
        }

        Do();

        VerCodehtttp.AddData("tel", tel);
        VerCodehtttp.Send();
    }

    public void Do()
    {
        VerCodehtttp.HttpSuccessCallBack(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                string Code = msg.Data["data"].ToString();
                m_CreatImgVerCode.Creat(Code);
            }
            else
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }

}
