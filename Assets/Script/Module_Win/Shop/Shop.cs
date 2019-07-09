using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
public class Shop : HttpBase
{

    protected override string WinID()
    {
        return "win_商城";
    }

    protected override void Init()
    {
        Transform WinOpen = Win.GetTransform("win_商城");

        Win.GetButton("shangcheng").onClick.AddListener(delegate ()
        {
            WindowsManager.GetWindowsManager.OpenWindow(WinOpen);
            ShopList("1");
        });

        Win.GetButton("btn_pig").onClick.AddListener(delegate ()
        {
            ShopList("1");
        });
        Win.GetButton("btn_dog").onClick.AddListener(delegate ()
        {
            ShopList("2");
        });
        Win.GetButton("btn_prop").onClick.AddListener(delegate ()
        {
            ShopList("3");
        });
        Win.GetButton("btn_object").onClick.AddListener(delegate ()
        {
            ShopList("4");
        });
    }


    //商城列表
    public void ShopList(string id)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_shop_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("type", id);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            ShowShopList(msg.Data["data"], id);
        });
    }

    //显示商品列表
    private void ShowShopList(JsonData jd, string goods_id)
    {

        ListGroup ShopList = ListCreatTools.Creat("ShopPig",
        Win.GetTransform("pig").gameObject,
        Win.GetTransform("Content_GroupList"), true);

        //GameObject[] HotList = ShopList.HottingObj.ToArray();
        //foreach (GameObject child in HotList)
        //{
        //    ShopList.Destory(child, delegate ()
        //    {
        //        TransformData trns = child.GetComponent<TransformData>();
        //        trns.GetText("tag").text = "";
        //        trns.GetText("desc").text = "";
        //    }); 
        //}

        if (jd == null)
            return;
        foreach (JsonData child in jd)
        {
            GameObject item = ShopList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>();
            LoadImage.GetLoadIamge.Load(child["img"].ToString(), new RawImage[] { ItemTransform.Get<RawImage>("icon") });
            ItemTransform.GetText("tag").text = child["title"].ToString();
            ItemTransform.GetText("desc").text = child["content"].ToString();
            Button btn_song = ItemTransform.GetButton("btn_song");
            Button btn_buy = ItemTransform.GetButton("btn_buy");
            if (child["price"].ToString() == "0")
            {
                btn_song.gameObject.SetActive(true);
                btn_buy.gameObject.SetActive(false);
                btn_song.onClick.AddListener(delegate ()
                {
                    Buy(child["id"].ToString(), goods_id, "1");
                });
            }
            else
            {
                btn_song.gameObject.SetActive(false);
                btn_buy.gameObject.SetActive(true);
                ItemTransform.GetText("price").text = child["price"].ToString();
                btn_buy.onClick.AddListener(delegate ()
                {
                    Transform win_buy = Win.GetTransform("win_确定购买");
                    WindowsManager.GetWindowsManager.OpenWindow(win_buy);
                    TransformData win_buy_trans = win_buy.GetComponent<TransformData>();
                    win_buy_trans.GetButton("btn_sure").onClick.AddListener(delegate ()
                    {
                        Buy(child["id"].ToString(), goods_id, win_buy_trans.GetInputField("InputField_num").text,win_buy);
                    });
                });
            }
        }
    }


    //购买商品
    private void Buy(string goods_id, string status, string num, Transform win = null)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_pig_buy.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("goods_id", goods_id);
        http.AddData("status", status);
        http.AddData("sl", num);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                if (win)
                    WindowsManager.GetWindowsManager.CloseWindow(win);
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }
}