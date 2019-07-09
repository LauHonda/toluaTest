using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateDH;
using System;
using LitJson;
using Dh_json;

public class Http : NewMessageInfo
{
    public DicBase<Http> EntityHttp;

    public void SetUrl(string CurrentUrl)
    {
        this.url = CurrentUrl;
        CurrentModel.name = this.url.ToString();
        EntityHttp = MessageManager.GetMessageManager.Http;
        EntityHttp.Add(CurrentUrl, this);
    }

    public Http(string url = null)
    {
        this.DataName = "data";
        Init(url);
    }

    /// <summary>
    /// 创建http请求对象
    /// </summary>
    /// <param name="url">接口名称</param>
    /// <param name="dataName">数据header</param>
    public Http(string url, string dataName = "data")
    {
        this.DataName = dataName;
        Init(url);
    }

    protected void Init(string URL = "NewHttpModel")
    {
        GameObject httpModel = new GameObject();
        CurrentModel = httpModel.AddComponent<HttpModel>();
        CurrentModel.Data = this;
        //绑定http消息附属关系
        HttpActivity.AddTryer(CurrentModel.CanSend);
        HttpActivity.AddListener(CurrentModel.Get);
        SetUrl(URL);

        CurrentData.Set(new HttpCallBackMessage() { Data = null });
        CurrentJsonData.Set(null);

    }

    /// <summary>
    /// 储存http返回数据
    /// </summary>
    public Value<HttpCallBackMessage> CurrentData = new Value<HttpCallBackMessage>(null);


    /// <summary>
    /// 存储http Json数据，每次执行成功或者数据更改，此方法会自动刷新,注意事项,数据包的key为：“data”;
    /// </summary>
    public Value<JsonData> CurrentJsonData = new Value<JsonData>(null);


    /// <summary>
    /// 尝试发送请求
    /// </summary>
    public Attempt HttpActivity = new Attempt();


    public HttpModel CurrentModel;

    /// <summary>
    /// 临时事件，每次请求完会被清除
    /// </summary>
    public void HttpSuccessCallBack(Action<HttpCallBackMessage> action)
    {
        CurrentModel.HttpSuccessCallBack.AddListener(action);
    }

    /// <summary>
    /// 尝试请求
    /// </summary>
    /// <param name="Clear">指定每次请求之后是否清除事件</param>
    public void Send(bool Clear = false, bool NoShow = false)
    {
        string F_ID = GameManagerhttp.GetGameManager.FriendID;
        if (!string.IsNullOrEmpty(F_ID))
        {
            AddData("friend_id", F_ID);
        }
        else
        {
            RemoveData("friend_id");
        }
        this.NoShow = NoShow;
        if (HttpActivity.Try())
        {
            if (Clear)
            {
                CurrentData.ClearChangeListener();
            }
            CurrentModel.HttpSuccessCallBack.AddListener(CurrentData.SetAndForceUpdate);
            CurrentModel.HttpSuccessCallBack.AddListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {
                    JsonData jd = msg.Data;
                    if (jd.Keys.Contains(DataName))
                        CurrentJsonData.SetAndForceUpdate(jd[DataName]);
                }
            });
        }
    }

    /// <summary>
    /// 更新单个数值，值类型为string
    /// </summary>
    public void UpdateData(string key, string Value)
    {
        JsonData jd = CurrentJsonData.Get();
        jd[key] = Value;
        CurrentJsonData.SetAndForceUpdate(jd);
    }
    /// <summary>
    /// 更新单个数值，值类型为jsondata
    /// </summary>
    public void UpdateData(string key, JsonData Value)
    {
        JsonData jd = CurrentJsonData.Get();
        jd[key] = (JsonData)Value;
        CurrentJsonData.SetAndForceUpdate(jd);
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    public void UpdateData(JsonData Value)
    {
        CurrentJsonData.SetAndForceUpdate(Value);
    }
}
