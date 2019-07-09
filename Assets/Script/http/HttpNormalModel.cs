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

    public HttpVerCode(string URL, Button Btn, string tel_key,string tel_value)
    {
        InitString(URL, Btn, tel_key,tel_value);
    }

    private void InitString(string URL, Button Bnt, string tel)
    {
        Bnt.onClick.AddListener(delegate ()
        {
            if (VerCodehtttp == null)
                VerCodehtttp = new Http(URL);

            VerCodehtttp.AddData("tel", tel);
            VerCodehtttp.Send();
            VerCodehtttp.HttpSuccessCallBack(delegate (HttpCallBackMessage msg)
            {
                Bnt.interactable = false;
                Utils.Timer.Instance.AddDeltaTimer(1, 60, 0, delegate (int count)
                   {
                       if (count > 1)
                           Bnt.GetComponentInChildren<Text>().text = "发送成功" + count.ToString();
                       else
                       {
                           Bnt.GetComponentInChildren<Text>().text = "发送";
                           Bnt.interactable = true;
                       }
                   });
            });
        });
    }

    private void InitString(string URL, Button Bnt, string tel_key,string tel_value)
    {
        Bnt.onClick.AddListener(delegate ()
        {
            if (VerCodehtttp == null)
                VerCodehtttp = new Http(URL);

            VerCodehtttp.AddData(tel_key, tel_value);
            VerCodehtttp.Send();
            VerCodehtttp.HttpSuccessCallBack(delegate (HttpCallBackMessage msg)
            {
                Bnt.interactable = false;
                Utils.Timer.Instance.AddDeltaTimer(1, 60, 0, delegate (int count)
                {
                    if (count > 1)
                        Bnt.GetComponentInChildren<Text>().text = "发送成功" + count.ToString();
                    else
                    {
                        Bnt.GetComponentInChildren<Text>().text = "发送";
                        Bnt.interactable = true;
                    }
                });
            });
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
            VerCodehtttp.HttpSuccessCallBack(delegate (HttpCallBackMessage msg)
            {
                Bnt.interactable = false;
                Utils.Timer.Instance.AddDeltaTimer(1, 60, 0, delegate (int count)
                {
                    if (count > 1)
                        Bnt.GetComponentInChildren<Text>().text = "发送成功" + count.ToString();
                    else
                    {
                        Bnt.GetComponentInChildren<Text>().text = "发送";
                        Bnt.interactable = true;
                    }
                });
            });
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
