using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UltimateDH;

public abstract class HttpBaseBLL : HttpBase
{
    private Http data;

    public Http Data { get { return data; } }

    protected override void Init()
    {
        data = new Http(InitUrl());
        data.CurrentData.AddChangeListener(OnResponesEvent);
        data.CurrentJsonData.AddChangeListener(OnResponesJsonDataEvent);
    }
    /// <summary>
    /// 更新单个数值，值类型为string
    /// </summary>
    public void UpdateData(string key, string Value)
    {
        data.UpdateData(key, Value);
    }

    /// <summary>
    /// 更新单个数值，值类型为jsondata
    /// </summary>
    public void UpdateData(string key, JsonData Value)
    {
        data.UpdateData(key, Value);
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    public void UpdateData(JsonData Value)
    {
        data.UpdateData(Value);
    }

    /// <summary>
    /// 初始化请求地址
    /// </summary>
    protected abstract string InitUrl();

    /// <summary>
    /// 接受服务器返回数据，建议做业务判断，处理消息层数据使用
    /// </summary>
    protected abstract void OnResponesEvent(HttpCallBackMessage EventData);

    /// <summary>
    /// 接受服务器返回Data数据，此方法在HTTPCODE=SUCCESS或者数据刷新时候被调用,建议更新数值在此刷新
    /// </summary>
    protected virtual void OnResponesJsonDataEvent(JsonData jsonData) { }


    /// <summary>
    /// 尝试请求服务器
    /// </summary>
    public virtual void Send()
    {
        data.Send();
    }



    ///// <summary>
    ///// 记录访问自己还是朋友
    ///// </summary>
    //private void FriendOrSelf()
    //{
    //    if (Static.Instance.IsFriend)
    //    {
    //        CurrentModel.Data.AddData("friend_id", Static.Instance.GetValue("friend_id"));
    //    }
    //    else
    //    {
    //        CurrentModel.Data.RemoveData("friend_id");
    //    }
    //}
}


