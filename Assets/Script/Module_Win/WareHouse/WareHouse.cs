using LitJson;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.UI;
public class WareHouse : HttpBase
{

    protected override string WinID()
    {
        return "win_我的";
    }

    protected override void Init()
    {
        Win.GetButton("wode").onClick.AddListener(delegate ()
        {
            GoToWareHouse();
        });

        ButtonChangeGroup btngroup = Win.Get<ButtonChangeGroup>("btnGroup");

        btngroup.GetButton(0).onClick.AddListener(delegate ()
        {
            PigList();
        });

        btngroup.GetButton(1).onClick.AddListener(delegate ()
        {
            Money();
        });

        btngroup.GetButton(2).onClick.AddListener(delegate ()
        {
            Log();
        });

        btngroup.GetButton(3).onClick.AddListener(delegate ()
        {
            message();
        });

        Win.GetButton("btn_dingdan").onClick.AddListener(delegate ()
        {
            Order();
        });

        Win.GetButton("btn_jiaoyi").onClick.AddListener(delegate ()
        {
            Deal();
        });
    }


    //跳转仓库
    public void GoToWareHouse()
    {
        ButtonChangeGroup btngroup = Win.Get<ButtonChangeGroup>("btnGroup");
        WindowsManager.GetWindowsManager.OpenWindow(Win.GetTransform("win_我的"));
        btngroup.SetState(0);
        PigList();
    }

    //消息跳转
    public void ShowMessageWin()
    {
        ButtonChangeGroup btngroup = Win.Get<ButtonChangeGroup>("btnGroup");
        OpenWin(Win.GetTransform("win_我的"));
        btngroup.SetState(3, true);
    }

    //猪列表
    public void PigList()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_my_storehouse.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        // http.AddData("uid", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowCangKu(msg.Data["data"]);
        });
    }


    public void ShowCangKu(JsonData jd)
    {
        Transform WareHouse_con = Win.Get<ScrollRect>("WareHouse").content.transform;
        ListGroup WareHouseList = ListCreatTools.Creat("WareHouse",
        WareHouse_con.GetChild(0).gameObject.gameObject, WareHouse_con
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = WareHouseList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            Button btn_shiyong = ItemTransform.GetButton("btn_shiyong");
            Button btn_weiyang = ItemTransform.GetButton("btn_weiyang");
            Button btn_chushou = ItemTransform.GetButton("btn_chushou");
            btn_shiyong.gameObject.SetActive(false);
            btn_weiyang.gameObject.SetActive(false);
            btn_chushou.gameObject.SetActive(false);
            if (child.Keys.Contains("img"))
                LoadImage.GetLoadIamge.Load(child["img"].ToString(), new RawImage[] { ItemTransform.Get<RawImage>("img") });
            int status = int.Parse(child["type"].ToString());
            if (status == 0)
            {
                ItemTransform.GetText("name").text = child["pig_name"].ToString();
                ItemTransform.GetText("desc").text = string.Format("数量：X {0}", child["num"].ToString());
                btn_shiyong.gameObject.SetActive(true);
                btn_shiyong.onClick.AddListener(delegate ()
                {

                });
                btn_shiyong.gameObject.SetActive(false);
                btn_weiyang.gameObject.SetActive(false);
                btn_chushou.gameObject.SetActive(false);
            }
            else if (status == 1)
            {
                ItemTransform.GetText("name").text = child["pig_name"].ToString();
                ItemTransform.GetText("desc").text = string.Format("体重{0}：", child["weight"].ToString());
                string str = child["feed_time"].ToString();

                if (str != "0")
                    ItemTransform.GetText("time").text = string.Format("喂养倒计时:{0}", child["feed_time"].ToString());

                btn_chushou.gameObject.SetActive(true);
                btn_chushou.onClick.AddListener(delegate ()
                {
                    ChuShow(child["id"].ToString());
                });
                btn_weiyang.gameObject.SetActive(true);
                btn_weiyang.onClick.AddListener(delegate ()
                {
                    WeiYang(child["id"].ToString());
                });
            }
            else if (status == 2)
            {
                btn_shiyong.gameObject.SetActive(false);
                btn_weiyang.gameObject.SetActive(false);
                btn_chushou.gameObject.SetActive(false);
            }
            else if (status == 3)
            {
                btn_shiyong.gameObject.SetActive(false);
                btn_weiyang.gameObject.SetActive(false);
                btn_chushou.gameObject.SetActive(false);
            }
            else if (status == 4)
            {
                ItemTransform.GetText("name").text = child["pig_name"].ToString();
                ItemTransform.GetText("desc").text = string.Format("数量：X {0}", child["num"].ToString());
                btn_shiyong.gameObject.SetActive(true);
                btn_shiyong.onClick.AddListener(delegate ()
                {
                    ChangeName();
                });
            }
            else if (status == 5)
            {
                ItemTransform.GetText("name").text = child["pig_name"].ToString();
                ItemTransform.GetText("desc").text = string.Format("数量：X {0}", child["num"].ToString());
                btn_shiyong.gameObject.SetActive(true);
                btn_shiyong.onClick.AddListener(delegate ()
                {
                    Cloth(child["id"].ToString());
                });
            }
        }
    }


    //使用服装
    private void Cloth(string id)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_clothing_wear.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("id", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                MainScene mainScene = EntityHttpModel.Get<MainScene>();
                mainScene.Send();
                PigList();
            }
        });
    }


    //喂养
    public void WeiYang(string id, bool Update = true)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_feed_pig.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("pig_id", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                MainScene mainScene = EntityHttpModel.Get<MainScene>();
                mainScene.Send();
                if (Update)
                    PigList();
            }
        });
    }


    //出售
    private void ChuShow(string id)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_sale_pig.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("pig_id", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                PigList();
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }


    //使用改名卡
    private void ChangeName()
    {
        MainScene mainScene = EntityHttpModel.Get<MainScene>();
        mainScene.ChangeFarmNameAction();
    }


    //日志
    private void Log()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_diary_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();

        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowLog(msg.Data["data"]);
        });
    }

    private void ShowLog(JsonData jd)
    {
        Transform WareHouse_con = Win.Get<ScrollRect>("Scroll View_Log").content.transform;
        ListGroup WareHouseList = ListCreatTools.Creat("WareHouseLog",
        WareHouse_con.GetChild(0).gameObject.gameObject, WareHouse_con
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = WareHouseList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("time").text = child["sj"].ToString();
            ItemTransform.GetText("type").text = child["content"].ToString();
            ItemTransform.GetText("action").text = child["result"].ToString();
        }
    }

    //消息
    private void message()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_news_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowMsg(msg.Data["data"]);
        });
    }

    private void ShowMsg(JsonData jd)
    {
        Transform WareHouse_con = Win.Get<ScrollRect>("WareHouse_Msg").content.transform;
        ListGroup WareHouseList = ListCreatTools.Creat("WareHouseMsg",
        WareHouse_con.GetChild(0).gameObject.gameObject, WareHouse_con
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = WareHouseList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("name").text = child["sj"].ToString();
            ItemTransform.GetText("desc").text = child["title"].ToString();
            Button btn_ok = ItemTransform.GetButton("btn_ok");
            Button btn_cancle = ItemTransform.GetButton("btn_cancle");

            btn_ok.gameObject.SetActive(false);
            btn_cancle.gameObject.SetActive(false);

            btn_ok.onClick.AddListener(delegate ()
            {
                YesOrNo(child["id"].ToString(), "1");
            });

            btn_cancle.onClick.AddListener(delegate ()
            {
                YesOrNo(child["id"].ToString(), "2");
            });

            if (child["status"].ToString() == "1")
            {
                btn_ok.gameObject.SetActive(true);
                btn_cancle.gameObject.SetActive(true);
            }
        }
    }


    private void YesOrNo(string id, string choose)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_change_status.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("id", id);
        http.AddData("choose", choose);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                message();
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }


    //交易记录
    private void Deal()
    {
        TransformData win_jy = GetWinTransrormData("win_交易记录");
        WindowsManager.GetWindowsManager.OpenWindow(win_jy.transform);
        Http http = HttpCreatTools.CreatHttp("ajax_transaction_record.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            jiaoyi(msg.Data["data"], win_jy);
        });
    }

    //交易
    private void jiaoyi(JsonData jd, TransformData win_jiaoyi)
    {
        ListGroup DealList = ListCreatTools.Creat("DealList",
        win_jiaoyi.GetTransform("Item").gameObject, win_jiaoyi.GetTransform("Content")
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = DealList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("time").text = child["sj"].ToString();
            ItemTransform.GetText("order").text = child["order_str"].ToString();
            ItemTransform.GetText("product").text = child["title"].ToString();
            ItemTransform.GetText("money").text = child["money"].ToString();
        }
    }

    //订单
    private void Order()
    {
        Transform win_order = Win.GetTransform("win_商品订单");
        WindowsManager.GetWindowsManager.OpenWindow(win_order);
        Http http = HttpCreatTools.CreatHttp("ajax_order_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowOrder(msg.Data["data"]);
        });
    }


    private void ShowOrder(JsonData jd)
    {
        TransformData OrderList_con = GetWinTransrormData("win_商品订单");
        ListGroup OrderList = ListCreatTools.Creat("OrderList",
       OrderList_con.GetTransform("Item").gameObject, OrderList_con.GetTransform("Content")
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = OrderList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("order").text = child["order_str"].ToString();
            ItemTransform.GetText("name").text = child["title"].ToString();
            ItemTransform.GetText("time").text = child["sj"].ToString();
            ItemTransform.GetText("num").text = string.Format("{0}:粮票", child["sl"].ToString());
            Button btn_ok = ItemTransform.GetButton("btn_ok");
            if (child["zhuangtai"].ToString() == "3")
            {
                btn_ok.gameObject.SetActive(false);
            }
            else
            {
                btn_ok.gameObject.SetActive(true);
            }

            btn_ok.onClick.AddListener(delegate ()
            {
                SureOrder(child["id"].ToString());
            });
        }
    }


    //确认订单
    private void SureOrder(string id)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_order_sure.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("id", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                Order();
            }
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }

    //资金流水
    private void Money()
    {

        Http http = HttpCreatTools.CreatHttp("ajax_capital_log.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowMoney(msg.Data["data"]);
        });
    }


    private void ShowMoney(JsonData jd)
    {
        Transform MoneyList_con = Win.Get<ScrollRect>("Scroll View_Money").content.transform;
        ListGroup MoneyList = ListCreatTools.Creat("MoneyList",
        MoneyList_con.GetChild(0).gameObject.gameObject, MoneyList_con
        , true);

        if (jd == null)
            return;

        foreach (JsonData child in jd)
        {
            GameObject item = MoneyList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("time").text = child["sj"].ToString();
            ItemTransform.GetText("come").text = child["title"].ToString();
            ItemTransform.GetText("price").text = child["pay"].ToString();
            ItemTransform.GetText("get").text = child["money"].ToString();
        }
    }
}
