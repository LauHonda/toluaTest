using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UltimateDH;

public class HttpModel : MonoBehaviour
{
    public TypeGo DataType;

    public NewMessageInfo Data;
    public List<GetMessageModel> MsgList = new List<GetMessageModel>();
    public UnityEvent Suc=new UnityEvent(), Fal=new UnityEvent();

    //临时事件
    public Message<HttpCallBackMessage> HttpSuccessCallBack = new Message<HttpCallBackMessage>();

    public UnityEvent DoAction;
    public bool NoShow = false;
    public bool HideMessage = false;
    public bool OnlyError;
    public bool IsAdd;
    [TextArea(2, 5)]
    public string ShowAddMessage;
    private string message = null;
    public float WaitTime = 0;

    public bool isLoacl;
    public string LocalHttp;
    public bool IsShell;

    public bool NoldIcon;
    public bool IsWait;
    public bool NoSus;
    public bool GetTWO;

    public bool CanSend()
    {
        return !isLocksend;
    }

    public bool isLocksend = false;

    //创建网络返回消息
    HttpCallBackMessage msg = new HttpCallBackMessage();

    public void Get()
    {
        if (isLocksend)
            return;
        msg.Code = HttpCode.FAILED;
        StartLoad();
        Data.ErrorCode = "-1";
        Data.BackData.Clear();
        message = null;
        if (!isLoacl)
            message += "?";

        message += EncryptDecipherTool.UserMd5();

        //URL请求拼接
        string url = null;
        if (isLoacl)
            url = Static.Instance.LocalURL + LocalHttp;
        else
            url = Static.Instance.URL + Data.URL;

        if (Data.SendData.Count > 0)
        {
            foreach (DataValue child in Data.SendData)
            {
                string str = child.GetString();
                if (str == "Error")
                {
                    EndLoad();
                    return;
                }
                if (str == "Gone")
                    continue;

                message += "&" + child.Name + "=" + str;
            }
        }

        url = url + message;
        StartCoroutine(GetMessage(url));
    }

    IEnumerator GetMessage(string url)
    {
        if (!Data.NoShow)
            Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www.text);
        EndLoad();
        if (www.text != "")
        {
            if (IsWait)
                yield return new WaitForSeconds(0.1f);

            if (www.error != null)
            {
                if (!NoShow)
                    MessageManager.GetMessageManager.ShowBar("网络连接失败,请求超时！");
                Data.ShowMessage = www.error;
                ErrorAction();
            }
            else
            {
                Data.ShowMessage = www.text;

                // string jsondata = System.Text.Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
                string jsondata = System.Text.Encoding.UTF8.GetString(www.bytes);
                jsondata = jsondata.Remove(0, Data.CutCount);

                foreach (HttpModel child in Data.ShareModel)
                {
                    child.DebugAction(jsondata);
                }

                DebugAction(jsondata);
            }
        }
        else
        {
            ErrorAction();
        }
    }

    private void ErrorAction()
    {
        HttpCallBackMessage aa = new HttpCallBackMessage();

        aa.Code = HttpCode.ERROR;
        aa.ErroMsg = "ERROR";
        HttpSuccessCallBack.Send(aa);
        HttpSuccessCallBack.ClearAllEevnt();
    }


    public void DebugAction(string DebugData)
    {
        string jsondata = DebugData;

        Static.Instance.DeleteFile(Application.persistentDataPath, "json.txt");
        Static.Instance.CreateFile(Application.persistentDataPath, "json.txt", jsondata);
        ArrayList infoall = Static.Instance.LoadFile(Application.persistentDataPath, "json.txt");
        String sr = null;
        foreach (string str in infoall)
        {
            sr += str;
        }
        JsonData jd = JsonMapper.ToObject(sr);
        msg.Data = jd;

        Data.ErrorMsg = jd.Keys.Contains("msg") ? jd["msg"].ToString() : "";
        Debug.Log(jd.Keys.Contains("code").ToString());
        Data.ErrorCode = jd.Keys.Contains("code") ? jd["code"].ToString() : "-1";

        if (IsShell)
        {
            if (Data.ErrorCode == "1")
            {
                if (jd.Keys.Contains(Data.HeaderName))
                    jd = jd[Data.HeaderName];
                else
                {
                    Debug.Log("没有获得headName");
                    return;
                }
            }
        }

        if (Data.ErrorCode == "1")
        {
            foreach (GetMessageModel child in MsgList)
            {
                child.SetValue(jd);
            }
            msg.Code = HttpCode.SUCCESS;
            List<string> Savename = new List<string>();
            Dictionary<string, string> SaveMessage = new Dictionary<string, string>();
            switch (DataType)
            {
                case TypeGo.None:

                    break;
                case TypeGo.GetTypeA:
                    break;
                case TypeGo.GetTypeB:
                    if (jd.Keys.Contains(Data.DataName))
                    {
                        foreach (Transform child in Data.MyListMessage.FatherObj)
                        {
                            ObjectPool.GetInstance().RecycleObj(child.gameObject);
                        }
                        Data.MyListMessage.SetVaule(jd[Data.DataName]);
                    }
                    break;

                case TypeGo.GetTypeC:
                    if (jd.Keys.Contains(Data.DataName))
                        Data.MyListMessage.SetValueSingle(jd[Data.DataName]);
                    break;

                case TypeGo.GetTypeD:
                    Data.MyListMessage.SendData(jd);
                    break;

                case TypeGo.GetTypeE:

                    if (jd.Keys.Contains(Data.DataName))
                    {
                        string name = Data.NeedReplayName ? Data.ReplayName : Data.DataName;
                        bool HaveKey = Data.AddTag == string.Empty ? false : true;
                        object[] data = new object[] { jd[Data.DataName], Data.Receivemodel, Data.AddTag, name, HaveKey };
                        DataManager.GetDataManager.OnResponesObj.SendMessage("Receive_Data", data);
                    }
                    break;

            }

            if (Data.Action)
            {
                Data.GetData(SaveMessage);
            }
        }

        HttpSuccessCallBack.Send(msg);
        HttpSuccessCallBack.ClearAllEevnt();

        if (Data.ErrorCode == "1")
        {
            Suc.Invoke();
        }
        else
        {
            Fal.Invoke();
        }

        //if (!HideMessage)
        //{
        //    ShowMessageWait();
        //}

    }


    public void ShowMessageWait()
    {
        if (OnlyError)
        {
            if (Data.ErrorCode != "1")
                MessageManager.GetMessageManager.WindowShowMessage(Data.ErrorMsg);
            return;
        }

        if (Data.ErrorMsg == string.Empty && IsAdd)
            Data.ErrorMsg = ShowAddMessage;
        MessageManager.GetMessageManager.WindowShowMessage(Data.ErrorMsg);
    }

    void QuiteGo()
    {
        SceneManager.LoadScene("mainmeun");
    }


    private void StartLoad()
    {
        if (Data.NoShow)
            return;
        MessageManager.GetMessageManager.AddLockNub();
        isLocksend = true;
    }

    private void EndLoad()
    {
        if (Data.NoShow)
            return;
        MessageManager.GetMessageManager.DisLockNub();
        isLocksend = false;
    }
}
