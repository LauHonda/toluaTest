using LitJson;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : HttpBaseBLL
{
    TransformData transformData;
    Button btn_ChangeFarmNickName;
    Http ChangedFramNickHttp;
    InputField InputField_NickName;

    protected override void Init()
    {
        base.Init();
        transformData = GetWinTransrormData("win_主页面");
        Data.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        Send();
        ChangeNickName();
        OpenInfoWin();
        EnterGroup();
        ChongZhi();
    }


    //打开个人信息窗口
    private void OpenInfoWin()
    {
        Button btn_info = transformData.Get<Button>("btn_info");
        btn_info.onClick.AddListener(delegate ()
        {
            Transform Win_Info = transformData.GetTransform("win_个人信息");
            WindowsManager.GetWindowsManager.OpenWindow(Win_Info);
        });
    }

    //初始化页面的猪
    public void PagePig(JsonData jd)
    {

    }

    //修改农场昵称
    public void ChangeNickName()
    {
        Button btn_OpenChangeWin = transformData.Get<Button>("ChangeNickName");
        btn_OpenChangeWin.onClick.AddListener(delegate ()
        {
            ChangeFarmNameAction();
        });

        ChangedFramNickHttp = new Http("ajax_farm_name_put.php");

        InputField_NickName = transformData.Get<InputField>("InputField_newname");
        btn_ChangeFarmNickName = transformData.Get<Button>("btn_sure");

        btn_ChangeFarmNickName.onClick.AddListener(delegate ()
        {
            ChangedFramNickHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
            ChangedFramNickHttp.AddData("farm_name", InputField_NickName);
            ChangedFramNickHttp.HttpSuccessCallBack(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {
                    Transform Win_NickName = transformData.GetTransform("win_修改农场昵称");
                    WindowsManager.GetWindowsManager.CloseWindow(Win_NickName);
                    this.UpdateData("farm_name", InputField_NickName.text);
                }
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            });
            ChangedFramNickHttp.Send();
        });
    }

    public void ChangeFarmNameAction()
    {
        Transform Win_NickName = transformData.GetTransform("win_修改农场昵称");
        WindowsManager.GetWindowsManager.OpenWindow(Win_NickName);
    }


    //显示二维码
    private void EnterGroup()
    {
        transformData.GetButton("qun").onClick.AddListener(delegate ()
        {
            WindowsManager.GetWindowsManager.OpenWindow(GetWinTransrorm("win_二维码"));
            Http http = HttpCreatTools.CreatHttp("ajax_wx_pic.php");
            http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
            http.Send(true);
            http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {
                    LoadImage.GetLoadIamge.Load(msg.Data["data"]["img"].ToString(), new RawImage[] { GetWinTransrormData("win_二维码").Get<RawImage>("RawImage") });
                }
            });
        });
    }

    /// 充值
    private void ChongZhi()
    {
        transformData.GetButton("btn_icon").onClick.AddListener(delegate ()
        {
            OpenWin(GetWinTransrorm("win_充值"));
            TransformData transformDataC = GetWinTransrormData("win_充值");

            transformDataC.GetText("renminbi_tag").text = string.Format("1人民币={0}粮票", CachingRegion.Get("pigbl"));
            transformDataC.GetText("zhubi_tag").text = string.Format("1猪币={0}粮票", CachingRegion.Get("moneybl"));

            transformDataC.GetButton("btn_cz").onClick.AddListener(delegate ()
            {
                //打开微信
            });

            transformDataC.GetButton("btn_zbcz").onClick.AddListener(delegate ()
            {
                OpenWin(GetWinTransrorm("win_猪币充值A"));
                TransformData win_transformDataA = GetWinTransrormData("win_猪币充值A");
                win_transformDataA.GetButton("close").onClick.AddListener(delegate ()
                {
                    CloseWin(GetWinTransrorm("win_猪币充值A"));
                    CloseWin(GetWinTransrorm("win_充值"));
                });

                win_transformDataA.GetInputField("InputField_num").text = transformDataC.GetInputField("InputField_num").text;

                win_transformDataA.GetButton("btn_sure").onClick.AddListener(delegate ()
                {
                    Http http = HttpCreatTools.CreatHttp("ajax_pigcoin_put.php");
                    http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
                    int num = int.Parse(win_transformDataA.GetInputField("InputField_num").text);
                    http.AddData("pigcoin", num);
                    http.AddData("sl", num * int.Parse(CachingRegion.Get("pigbl")));
                    http.Send();
                    http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
                    {
                        if (msg.Code == HttpCode.SUCCESS)
                        {
                            OpenWin(GetWinTransrorm("win_猪币充值B"));
                            TransformData win_transformDataB = GetWinTransrormData("win_猪币充值B");
                            win_transformDataB.GetButton("close").onClick.AddListener(delegate ()
                            {
                                OpenWin(GetWinTransrorm("win_猪币充值B"));
                                CloseWin(GetWinTransrorm("win_猪币充值A"));
                                CloseWin(GetWinTransrorm("win_充值"));
                            });
                        }
                    });

                });
            });

        });
    }

    protected override string InitUrl()
    {
        return "ajax_index.php";
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {

    }

    protected override void OnResponesJsonDataEvent(JsonData jsonData)
    {
        Toggle man = transformData.Get<Toggle>("man");
        Toggle woman = transformData.Get<Toggle>("woman");
        string sex = jsonData["sex"].ToString();
        man.isOn = sex == "0" ? false : true;
        woman.isOn = sex == "0" ? true : false;
        CachingRegion.Add("pigbl", jsonData["pigbl"].ToString());
        CachingRegion.Add("moneybl", jsonData["moneybl"].ToString());
        transformData.DoLayout(jsonData);
    }

}
