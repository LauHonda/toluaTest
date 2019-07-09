using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UltimateDH;
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
        PagePig();
        MessageInfo();
        kefu();
        RunLight();

        transformData.GetButton("Home").onClick.AddListener(delegate ()
        {
            BackMySelf();
        });

        //绑定状态机，限制拖拽，只有在自己农场才可以操作
        EntityHttpModel.Get<Drag>().TryerDrag.AddTryerBase(IsSelf);
        TrySteal.AddTryer(IsFriend);
        TrySteal.AddListener(Steal);

        InvokeRepeating("word", 0, 10);

        UpdateHeadImg();

    }

    //头像上传
    private void UpdateHeadImg()
    {
        TransformData TRNS = transformData.Get<TransformData>("win_个人信息").Init();
        TRNS.GetButton("Headimg").onClick.AddListener(AndroidPhoto.GetPhoto.OpenPhoto);

        AndroidPhoto.GetPhoto.headImg.AddChangeListener(delegate (Texture2D img)
        {
            transformData.Get<RawImage>("img_head").texture = img;
            TRNS.Get<RawImage>("Headimg").texture = img;
            LoadImage.GetLoadIamge.SendImage(img);
        });

    }

    void word()
    {
        TransformData pig01 = transformData.GetTransform("Pig_Level01").GetComponent<TransformData>();
        TransformData pig02 = transformData.GetTransform("Pig_Level02").GetComponent<TransformData>();
        TransformData pig03 = transformData.GetTransform("Pig_Level03").GetComponent<TransformData>();

        pig01.GetTransform("Back").gameObject.SetActive(false);
        pig02.GetTransform("Back").gameObject.SetActive(false);
        pig03.GetTransform("Back").gameObject.SetActive(false);

        int num = UnityEngine.Random.Range(0, 5);

        if (num == 0)
        {
            pig01.GetTransform("Back").gameObject.SetActive(true);
        }
        else if (num == 1)
        {
            pig02.GetTransform("Back").gameObject.SetActive(true);
        }
        else if (num == 2)
        {
            pig03.GetTransform("Back").gameObject.SetActive(true);

        }
    }

    //跑马灯
    private void RunLight()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_runhorse_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                transformData.GetText("msg").text = msg.Data["data"].ToString();
            }
        });
    }


    public Attempt TrySteal = new Attempt();

    //偷取好友饲料
    private void Steal()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_fodder_stole.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("id", GameManagerhttp.GetGameManager.FriendID);
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }


    //主页喂养
    public void MainSceenFeed(Vector3 Point)
    {



        Transform pig01 = transformData.GetTransform("Pig_Level01");
        Transform pig02 = transformData.GetTransform("Pig_Level02");
        Transform pig03 = transformData.GetTransform("Pig_Level03");
        int num = -1;
        float dis01 = Vector3.Distance(pig01.position, Point);
        if (dis01 <= 3)
        {
            num = 0;
        }
        float dis02 = Vector3.Distance(pig02.position, Point);
        if (dis02 <= 3)
        {
            num = 1;
        }
        float dis03 = Vector3.Distance(pig03.position, Point);
        if (dis03 <= 3)
        {
            num = 2;
        }

        Http httpIndex = EntityHttp.Get("ajax_index.php");
        JsonData jd = httpIndex.CurrentJsonData.Get();
        JsonData pigdata = jd["pig"];
        if (num == -1 || pigdata.Count <= num)
            return;

        WareHouse wareHouse = EntityHttpModel.Get<WareHouse>();
        wareHouse.WeiYang(pigdata[num]["id"].ToString(), false);
    }

    private bool IsSelf()
    {
        return string.IsNullOrEmpty(GameManagerhttp.GetGameManager.FriendID);
    }

    private bool IsFriend()
    {
        return !string.IsNullOrEmpty(GameManagerhttp.GetGameManager.FriendID);
    }


    //跳转好友农场
    public void GoToFriend(string ID)
    {
        transformData.GetButton("siliao").onClick.AddListener(delegate ()
        {
            TrySteal.Try();
        });

        GameManagerhttp.GetGameManager.FriendID = ID;
        transformData.GetTransform("butonGroup").gameObject.SetActive(false);
        transformData.GetTransform("Mask").gameObject.SetActive(true);
        transformData.GetTransform("Home").gameObject.SetActive(true);
        this.Send();
    }

    //返回我的农场
    private void BackMySelf()
    {
        transformData.GetButton("siliao").onClick.RemoveAllListeners();
        GameManagerhttp.GetGameManager.FriendID = null;
        transformData.GetTransform("butonGroup").gameObject.SetActive(true);
        transformData.GetTransform("Mask").gameObject.SetActive(false);
        transformData.GetTransform("Home").gameObject.SetActive(false);
        this.Send();
    }


    //客服
    private void kefu()
    {
        TransformData Win_kefu = GetWinTransrormData("win_客服对话框").Init();

        ListGroup kefuCharList = ListCreatTools.Creat("kefuChar",
          Win_kefu.GetTransform("item").gameObject,
          Win_kefu.GetTransform("Content_kefuCharList"));

        transformData.GetButton("kefu").onClick.AddListener(delegate ()
        {
            WindowsManager.GetWindowsManager.OpenWindow(Win_kefu.transform);
            Win_kefu.GetButton("btn_send").onClick.RemoveAllListeners();
            Win_kefu.GetButton("btn_send").onClick.AddListener(delegate ()
            {
                UpdateFriendChar(Win_kefu.GetInputField("kefu_char").text, kefuCharList, true);
                Http FrienSendCharHttp = HttpCreatTools.CreatHttp("ajax_kfword_get.php");
                FrienSendCharHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
                FrienSendCharHttp.AddData("content", Win_kefu.GetInputField("kefu_char"));
                FrienSendCharHttp.Send(true);
                FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
                {
                    if (msg.Code == HttpCode.SUCCESS)
                    {
                        UpdateFriendChar(msg.Data["data"]["word"].ToString(), kefuCharList);
                    }
                });
            });
        });
    }

    //插入客服聊天内容
    private void UpdateFriendChar(string charlistdata, ListGroup KeFuCharList, bool Self = false)
    {
        if (charlistdata == null)
            return;
        GameObject item = KeFuCharList.Instantiate();

        TransformData ItemTransform;
        Transform other = item.transform.GetChild(0);
        Transform self = item.transform.GetChild(1);
        foreach (Transform childother in other)
        {
            childother.gameObject.SetActive(false);
        }
        foreach (Transform childself in self)
        {
            childself.gameObject.SetActive(false);
        }
        if (!Self)
        {
            foreach (Transform childother in other)
            {
                childother.gameObject.SetActive(true);
            }
            ItemTransform = other.GetComponent<TransformData>();
        }
        else
        {
            foreach (Transform childself in self)
            {
                childself.gameObject.SetActive(true);
            }
            ItemTransform = self.GetComponent<TransformData>();
        }

        // LoadImage.GetLoadIamge.Load(charlistdata[i]["img"].ToString(), new RawImage[] { ItemTransform.Get<RawImage>("head") });
        //  ItemTransform.GetText("name").text = charlistdata[i]["name"].ToString();
        ItemTransform.GetText("char").text = charlistdata;
    }


    //消息
    private void MessageInfo()
    {
        transformData.GetButton("message").onClick.AddListener(delegate ()
        {
            WareHouse warehouse = EntityHttpModel.Get<WareHouse>();
            warehouse.ShowMessageWin();
        });
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
    public void PagePig()
    {
        Button pig01 = transformData.GetButton("Pig_Level01");
        Button pig02 = transformData.GetButton("Pig_Level02");
        Button pig03 = transformData.GetButton("Pig_Level03");

        pig01.onClick.AddListener(delegate ()
        {
            ReSetPig(0);
            GameObject tools = pig01.transform.GetChild(1).gameObject;

            ReSetTools(tools.transform, 0);
        });

        pig02.onClick.AddListener(delegate ()
        {
            ReSetPig(1);
            GameObject tools = pig02.transform.GetChild(1).gameObject;

            ReSetTools(tools.transform, 1);
        });

        pig03.onClick.AddListener(delegate ()
        {
            ReSetPig(2);
            GameObject tools = pig03.transform.GetChild(1).gameObject;

            ReSetTools(tools.transform, 2);
        });
    }

    Animator CurrentAnimator = null;
    int CurrentNum = -1;
    private void ReSetPig(int num)
    {

        Animator ani01 = transformData.GetTransform("Pig_Level01").GetChild(1).GetComponent<Animator>();
        Animator ani02 = transformData.GetTransform("Pig_Level02").GetChild(1).GetComponent<Animator>();
        Animator ani03 = transformData.GetTransform("Pig_Level03").GetChild(1).GetComponent<Animator>();

        if (CurrentAnimator != null)
        {
            foreach (Transform child in CurrentAnimator.transform)
            {
                child.GetComponent<Image>().raycastTarget = false;
            }
            CurrentAnimator.SetBool("Back", true);
            if (num == CurrentNum)
            {
                num = -1;
                CurrentAnimator = null;
                return;
            }

        }

        if (num == 0)
        {
            CurrentAnimator = ani01;
        }
        else if (num == 1)
        {
            CurrentAnimator = ani02;
        }
        else if (num == 2)
        {
            CurrentAnimator = ani03;
        }

        CurrentNum = num;

        foreach (Transform child in CurrentAnimator.transform)
        {
            child.GetComponent<Image>().raycastTarget = true;
        }
        CurrentAnimator.SetBool("Go", true);

    }

    private void ReSetTools(Transform Tools, int num)
    {
        Button btn_A = Tools.GetChild(0).GetComponent<Button>();
        Button btn_B = Tools.GetChild(1).GetComponent<Button>();
        Button btn_C = Tools.GetChild(2).GetComponent<Button>();
        Button btn_D = Tools.GetChild(3).GetComponent<Button>();

        btn_A.onClick.RemoveAllListeners();
        btn_A.onClick.AddListener(delegate ()
        {
            if (num == CurrentNum)
            {
                Shop shop = EntityHttpModel.Get<Shop>();
                shop.GoToShop();
            }
        });
        btn_B.onClick.RemoveAllListeners();
        btn_B.onClick.AddListener(delegate ()
        {
            if (num == CurrentNum)
            {
                WareHouse wareHouse = EntityHttpModel.Get<WareHouse>();
                wareHouse.GoToWareHouse();
            }
        });
        btn_C.onClick.RemoveAllListeners();
        btn_C.onClick.AddListener(delegate ()
        {
            if (num == CurrentNum)
            {
                FriendList friendList = EntityHttpModel.Get<FriendList>();
                friendList.GoToFriend();
            }
        });
        btn_D.onClick.RemoveAllListeners();
        btn_D.onClick.AddListener(delegate ()
        {
            if (num == CurrentNum)
            {
                WareHouse wareHouse = EntityHttpModel.Get<WareHouse>();
                wareHouse.GoToWareHouse();
            }
        });

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


    // 充值
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
                    double fl_num = Math.Round((num / float.Parse(CachingRegion.Get("pigbl"))), 3);
                    Debug.Log(fl_num);
                    http.AddData("pigcoin", fl_num.ToString());
                    http.AddData("sl", num.ToString());
                    http.Send();
                    http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
                    {
                        if (msg.Code == HttpCode.SUCCESS)
                        {
                            OpenWin(GetWinTransrorm("win_猪币充值B"));
                            TransformData win_transformDataB = GetWinTransrormData("win_猪币充值B");
                            win_transformDataB.GetInputField("InputField_Address").text = CachingRegion.Get("tjs_url");
                            win_transformDataB.GetButton("close").onClick.AddListener(delegate ()
                            {
                                OpenWin(GetWinTransrorm("win_猪币充值B"));
                                CloseWin(GetWinTransrorm("win_猪币充值A"));
                                CloseWin(GetWinTransrorm("win_充值"));
                            });
                            win_transformDataB.GetButton("Copy").onClick.AddListener(delegate ()
                            {
                                UniClipboard.SetText(CachingRegion.Get("tjs_url"));
                            });
                            win_transformDataB.GetButton("HavePay").onClick.AddListener(delegate ()
                            {
                                Http httpHavepay = HttpCreatTools.CreatHttp("ajax_pigcoin_sure.php");
                                http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
                                http.Send();
                                http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage Havemsg)
                                {
                                    if (msg.Code != HttpCode.ERROR)
                                    {
                                        MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                                    }

                                    CloseWin(GetWinTransrorm("win_猪币充值B"));
                                    CloseWin(GetWinTransrorm("win_猪币充值A"));
                                    CloseWin(GetWinTransrorm("win_充值"));
                                });
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
        transformData.DoLayout(jsonData);
        UpdateMainSceen(jsonData);
        UpdateUserInfo(jsonData);
    }


    [SerializeField]
    Sprite[] BackGround;
    [SerializeField]
    Color NightColor;
    //主页信息配置
    private void UpdateMainSceen(JsonData jsonData)
    {
        //头像
        TransformData TRNS = transformData.Get<TransformData>("win_个人信息").Init();
        TRNS.GetButton("Headimg").onClick.AddListener(AndroidPhoto.GetPhoto.OpenPhoto);

        LoadImage.GetLoadIamge.Load(jsonData["headimg"].ToString(), new RawImage[]
        {
            transformData.Get<RawImage>("img_head"),
            TRNS.Get<RawImage>("Headimg")
        });

        transformData.GetUIComponentInChildren<Text>("siliao").text = string.Format("X{0}", jsonData["fodder"].ToString());

        dog(jsonData);
        pig(jsonData);
        background(jsonData);
    }

    //狗配置
    private void dog(JsonData jsonData)
    {
        string dog_have = jsonData["dog_have"].ToString();
        string wake_status = jsonData["wake_status"].ToString();
        Button dog01 = transformData.GetButton("Dog_HSQ", true);
        Button dog02 = transformData.GetButton("Dog_CQ", true);

        dog01.gameObject.SetActive(false);
        dog02.gameObject.SetActive(false);

        transformData.GetTransform("t_b").gameObject.SetActive(false);

        if (dog_have == "1")
        {
            dog01.gameObject.SetActive(true);
            dog01.transform.GetChild(1).gameObject.SetActive(false);
            dog01.transform.GetChild(0).gameObject.SetActive(false);

            dog01.onClick.AddListener(delegate ()
            {
                WakeDog();
            });

            if (wake_status == "0")
            {
                dog01.transform.GetChild(1).gameObject.SetActive(true);
                if (jsonData["dog_time"].ToString() != "")
                {
                    transformData.GetTransform("t_b").gameObject.SetActive(true);
                    transformData.GetUIComponentInChildren<Text>("t_b").text = jsonData["dog_time"].ToString();
                }
            }
            else
            {
                dog01.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (dog_have == "2")
        {
            dog02.gameObject.SetActive(true);
            dog02.transform.GetChild(1).gameObject.SetActive(false);
            dog02.transform.GetChild(0).gameObject.SetActive(false);

            dog02.onClick.AddListener(delegate ()
            {
                WakeDog();
            });



            if (wake_status == "0")
            {
                dog02.transform.GetChild(1).gameObject.SetActive(true);

                if (jsonData["dog_time"].ToString() != "")
                {
                    transformData.GetTransform("t_b").gameObject.SetActive(true);
                    transformData.GetUIComponentInChildren<Text>("t_b").text = jsonData["dog_time"].ToString();
                }
            }
            else
            {
                dog02.transform.GetChild(0).gameObject.SetActive(true);
            }
        }


    }


    //猪配置
    private void pig(JsonData jsonData)
    {
        Transform pig01 = transformData.GetTransform("Pig_Level01");
        Transform pig02 = transformData.GetTransform("Pig_Level02");
        Transform pig03 = transformData.GetTransform("Pig_Level03");

        pig01.gameObject.SetActive(false);
        pig02.gameObject.SetActive(false);
        pig03.gameObject.SetActive(false);

        foreach (JsonData child in jsonData["pig"])
        {
            if (child["pig_name"].ToString() == "悍虎")
            {
                transformData.GetTransform("Pig_Level01").gameObject.SetActive(true);
            }
            else if (child["pig_name"].ToString() == "豆豆")
            {
                transformData.GetTransform("Pig_Level02").gameObject.SetActive(true);
            }
            else if (child["pig_name"].ToString() == "豆豆")
            {
                transformData.GetTransform("Pig_Level03").gameObject.SetActive(true);
            }
        }
        int num = 0;
        if (jsonData["clothes"].ToString() == "裸体猪")
        {
            num = 0;
        }
        else if (jsonData["clothes"].ToString() == "休闲猪")
        {
            num = 1;
        }
        else if (jsonData["clothes"].ToString() == "厨师猪")
        {
            num = 2;
        }
        else if (jsonData["clothes"].ToString() == "魔术猪")
        {
            num = 3;
        }

        foreach (Transform child in pig01.GetChild(2))
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in pig02.GetChild(2))
        {
            child.gameObject.SetActive(false);
        }


        foreach (Transform child in pig03.GetChild(2))
        {
            child.gameObject.SetActive(false);
        }

      

        Transform p01 = pig01.GetChild(2).GetChild(num);
        Transform p02 = pig02.GetChild(2).GetChild(num);
        Transform p03 = pig03.GetChild(2).GetChild(num);
        p01.gameObject.SetActive(true);
        p02.gameObject.SetActive(true);
        p03.gameObject.SetActive(true);

        Transform dog01 = transformData.GetTransform("Dog_HSQ");
        Transform dog02 = transformData.GetTransform("Dog_CQ");

        string background = jsonData["background_clothes"].ToString();
        //夜晚需要降低贴图颜色
        if (background == "2")
        {
            p01.GetComponent<Image>().color = NightColor;
            p02.GetComponent<Image>().color = NightColor;
            p03.GetComponent<Image>().color = NightColor;
            foreach (Transform child in dog01)
            {
                child.GetComponent<Image>().color= NightColor;
            }
            foreach (Transform child in dog02)
            {
                child.GetComponent<Image>().color = NightColor;
            }
        }
        else
        {
            p01.GetComponent<Image>().color = Color.white;
            p02.GetComponent<Image>().color = Color.white;
            p03.GetComponent<Image>().color = Color.white;
            foreach (Transform child in dog01)
            {
                child.GetComponent<Image>().color = Color.white;
            }
            foreach (Transform child in dog02)
            {
                child.GetComponent<Image>().color = Color.white;
            }
        }

        pig01.GetChild(0).GetComponentInChildren<Text>().text = jsonData["pig_words"].ToString();
        pig02.GetChild(0).GetComponentInChildren<Text>().text = jsonData["pig_words"].ToString();
        pig03.GetChild(0).GetComponentInChildren<Text>().text = jsonData["pig_words"].ToString();
    }


    //背景配置
    private void background(JsonData jsonData)
    {
        Image backgroundimg = transformData.Get<Image>("win_主页面");
        Transform Night = transformData.GetTransform("Night");
        string background = jsonData["background_clothes"].ToString();
        if (background == "0")
        {
            backgroundimg.sprite = BackGround[0];
        }
        else if (background == "1")
        {
            backgroundimg.sprite = BackGround[1];
        }
        else if (background == "2")
        {
            backgroundimg.sprite = BackGround[2];
        }
        else if (background == "3")
        {
            backgroundimg.sprite = BackGround[3];
        }
    }



    //唤醒狗
    private void WakeDog()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_dog_wakeup.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
            if (msg.Code == HttpCode.SUCCESS)
            {
                this.Send();
            }
        });
    }


    //个人信息配置
    private void UpdateUserInfo(JsonData jsonData)
    {
        Toggle man = transformData.Get<Toggle>("man");
        Toggle woman = transformData.Get<Toggle>("woman");
        string sex = jsonData["sex"].ToString();
        man.isOn = sex == "0" ? false : true;
        woman.isOn = sex == "0" ? true : false;
        transformData.GetText("NameText").text = jsonData["name"].ToString();
        transformData.GetText("Coin").text = jsonData["money"].ToString();


        CachingRegion.Add("pigbl", jsonData["pigbl"].ToString());
        CachingRegion.Add("moneybl", jsonData["moneybl"].ToString());
        CachingRegion.Add("name", jsonData["name"].ToString());
        CachingRegion.Add("tjs_url", jsonData["tjs_url"].ToString());
    }
}
