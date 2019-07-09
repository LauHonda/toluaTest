using UnityEngine;
using DataItem;
using System.Collections;
using System.Collections.Generic;
using LitJson;
public class DataManager : MonoBehaviour
{
    public static DataManager GetDataManager;

    private void Awake()
    {
        GetDataManager = this;
    }
    public GameObject OnResponesObj { get { return this.gameObject; } }

    #region Data创建区域

    //列表数据
    public DataDic<GoodsData> Goods = new DataDic<GoodsData>("Goods");
    public DataDic<ShopCar> ShopCar = new DataDic<ShopCar>("ShopCar");
    public DataDic<AddressData> Address = new DataDic<AddressData>("Address");
    //单体数据
    public DataInfo<UserData> user = new DataInfo<UserData>("userdata");

    #endregion //data创建区域

    #region 接收消息区域
    

    //任务列表
    public void Receive_Data(object[] data)
    {
        DataPool.GetInstance().SendDataMessage.Send(data);
    }
		
    #endregion //********************接收消息区域

	public void UpdateSb(string jd)
	{
		user.Data.sb = double.Parse(jd);
		user.SyncData ();
	}
		
}