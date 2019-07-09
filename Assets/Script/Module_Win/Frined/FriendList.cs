using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using UnityEngine.UI;
using System;

public class FriendList : HttpTransformBLL
{

    protected override void Init()
    {
        base.Init();
        OpenFriendWin();
        OpenSearchWin();
        ChoseFriend(CreatGroupAction);
        UpdateGroup();
        UpdateFriendList();
        NewFriendList();
    }

    //初始化好友列表
    private void NewFriendList()
    {
        ListGroup FriendList = ListCreatTools.Creat("Frined",
      transformData.GetTransform("Item_Frined").gameObject,
      transformData.GetTransform("Content_FriendList"));
    }

    //跳转好友
    public void GoToFriend()
    {
        this.Send();
        WindowsManager.GetWindowsManager.OpenWindow(transformData.GetTransform("win_好友"));
    }


    //打开好友窗口
    private void OpenFriendWin()
    {
        Data.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        transformData.GetButton("haoyou").onClick.AddListener(Send);
        transformData.GetButton("haoyou").onClick.AddListener(delegate ()
        {
            WindowsManager.GetWindowsManager.OpenWindow(transformData.GetTransform("win_好友"));
        });
    }


    //打开搜索窗口
    private void OpenSearchWin()
    {
        transformData.GetButton("search").onClick.AddListener(delegate ()
        {
            Transform win_friend = transformData.GetTransform("win_搜索好友");
            WindowsManager.GetWindowsManager.OpenWindow(win_friend);
            TransformData searchTransform = win_friend.GetComponent<TransformData>();
            InputField InputField_SearchFriend = searchTransform.GetInputField("InputField_SearchFriend");

            searchTransform.GetButton("close").onClick.AddListener(delegate ()
            {
                InputField_SearchFriend.text = "";
                searchTransform.GetText("desc").text = "";
            });

            searchTransform.GetButton("btn_add").onClick.RemoveAllListeners();
            searchTransform.GetButton("Search").onClick.RemoveAllListeners();
            searchTransform.GetButton("Search").onClick.AddListener(delegate ()
            {
                SearchFriend(InputField_SearchFriend.text, searchTransform);
            });
        });
    }


    //搜索好友
    private void SearchFriend(string str, TransformData searchTrns)
    {
        if (str == "")
            return;
        Http SearchFriendHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_search.php");

        SearchFriendHttp.AddData("search", str);
        SearchFriendHttp.Send();
        SearchFriendHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            Transform img = searchTrns.GetTransform("img_head");
            Button btn_add = searchTrns.GetButton("btn_add");
            if (msg.Code == HttpCode.SUCCESS)
            {
                JsonData jsdata = msg.Data["data"];
                searchTrns.GetText("desc").text = jsdata["name"].ToString();
             
                btn_add.onClick.AddListener(delegate ()
                {
                    AddFriend(jsdata["id"].ToString());
                });

                LoadImage.GetLoadIamge.Load(jsdata["img"].ToString(), new RawImage[] { searchTrns.Get<RawImage>("img_head") });
                img.gameObject.SetActive(true);
                btn_add.gameObject.SetActive(true);
            }
            else if (msg.Code == HttpCode.FAILED)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                img.gameObject.SetActive(false);
                btn_add.gameObject.SetActive(false);
                searchTrns.GetText("desc").text = "";
            }
        });
    }


    //添加好友
    private void AddFriend(string ID)
    {
        Http ADDFriendHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_add.php");
        ADDFriendHttp.AddData("id", ID);
        ADDFriendHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        ADDFriendHttp.Send();
        ADDFriendHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                Send();
            }
        });
    }


    //删除好友
    private void DeleteFriend(string ID, GameObject Currentitem, ListGroup ItemGroup)
    {
        Http ADDFriendHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_del.php");
        ADDFriendHttp.AddData("id", ID);
        ADDFriendHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        ADDFriendHttp.Send();
        ADDFriendHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                ItemGroup.Destory(Currentitem);
            }
        });
    }


    //打开好友聊天
    private void FriendChar(JsonData chardata)
    {

        string ID = chardata["id"].ToString();
        Transform win_char = transformData.GetTransform("win_好友对话框");
        WindowsManager.GetWindowsManager.OpenWindow(win_char);
        TransformData charTransfoem = win_char.GetComponent<TransformData>();
        Transform biaoqing = charTransfoem.GetTransform("Content_biaoqing");

        GetFriendChar(ID, charTransfoem, biaoqing);

        charTransfoem.GetText("Desc").text = string.Format("正在与{0}聊天", chardata["name"].ToString());

        Http FrienSendCharHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_chat_put.php");
        FrienSendCharHttp.AddData("id", ID);
        FrienSendCharHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        charTransfoem.GetButton("btn_send").onClick.RemoveAllListeners();
        charTransfoem.GetButton("btn_send").onClick.AddListener(delegate ()
        {
            FrienSendCharHttp.AddData("content", charTransfoem.GetInputField("InputField_char"));
            FrienSendCharHttp.AddData("type", "1");
            FrienSendCharHttp.Send(true);
            FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {
                    page = 0;
                    GetFriendChar(ID, charTransfoem, biaoqing);
                }
            });
        });
        charTransfoem.GetButton("btn_biaoqing").onClick.RemoveAllListeners();
        charTransfoem.GetButton("btn_biaoqing").onClick.AddListener(delegate ()
        {
            Transform win_bq = charTransfoem.GetTransform("img");
            win_bq.gameObject.SetActive(true);

            foreach (Transform child in biaoqing)
            {
                child.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    FrienSendCharHttp.AddData("type", "2");
                    FrienSendCharHttp.AddData("content", child.GetSiblingIndex().ToString());
                    FrienSendCharHttp.Send(true);
                    FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
                    {
                        if (msg.Code == HttpCode.SUCCESS)
                        {
                            win_bq.gameObject.SetActive(false);
                            page = 0;
                            GetFriendChar(ID, charTransfoem, biaoqing);
                        }
                    });
                });
            }
        });


        charTransfoem.GetButton("close").onClick.AddListener(delegate ()
        {
            if (Friend != null)
                StopCoroutine(Friend);
        });

        if (Friend != null)
            StopCoroutine(Friend);
        Friend = StartCoroutine(UpdateFrined(ID, charTransfoem, biaoqing));
    }

    //刷新好友天记录
    IEnumerator UpdateFrined(string ID, TransformData charTransfoem, Transform BQ)
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            GetFriendChar(ID, charTransfoem, BQ);
        }
    }



    Coroutine Friend = null;
    public int page = 0;
    private string LastID = "";
    //拉取好友聊天记录
    private void GetFriendChar(string ID, TransformData friencharitem, Transform BQ)
    {
        Http FrienCharListHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_chat.php");
        //if (LastID != ID)
        //    page = 0;
        //LastID = ID;
        FrienCharListHttp.AddData("id", ID);
        FrienCharListHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        FrienCharListHttp.AddData("page", "0");
        FrienCharListHttp.Send(true, true);
        FrienCharListHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                page++;
                UpdateFriendChar(msg.Data.Keys.Contains("data") ? msg.Data["data"] : null, friencharitem, BQ);
            }
        });
    }


    //刷新好友消息
    private void UpdateFriendChar(JsonData charlistdata, TransformData charWin, Transform BQ, string GroupTag = "FrinedChar")
    {
        ListGroup FriendCharList = ListCreatTools.Creat(GroupTag,
           charWin.GetTransform("item").gameObject,
           charWin.GetTransform("Content_FriendCharList"), true);

        //GameObject[] HotList = FriendCharList.HottingObj.ToArray();
        //foreach (GameObject child in HotList)
        //{
        //    FriendCharList.Destory(child, delegate ()
        //     {
        //         TransformData trns0 = child.transform.GetChild(0).GetComponent<TransformData>();
        //         TransformData trns1 = child.transform.GetChild(1).GetComponent<TransformData>();
        //         trns0.Get<Image>("img").color = new Color(0, 0, 0, 0);
        //         trns1.Get<Image>("img").color = new Color(0, 0, 0, 0);
        //     });
        //}

        if (charlistdata == null)
            return;

        for (int i = charlistdata.Count - 1; i >= 0; i--)
        {
            GameObject item = FriendCharList.Instantiate();

            string Name = charlistdata[i]["name"].ToString();
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
            if (Name != CachingRegion.Get("name"))
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

            LoadImage.GetLoadIamge.Load(charlistdata[i]["img"].ToString(), new RawImage[] { ItemTransform.Get<RawImage>("head") });

            ItemTransform.GetText("name").text = charlistdata[i]["name"].ToString();
            if (charlistdata[i]["type"].ToString() == "1")
            {
                ItemTransform.GetText("char").text = charlistdata[i]["content"].ToString();
                ItemTransform.GetTransform("back").gameObject.SetActive(true);
                ItemTransform.Get<Image>("img").color = new Color(0, 0, 0, 0);
            }
            else
            {
                ItemTransform.Get<Image>("img").color = Color.white;
                int num = 0;

                bool isGet = int.TryParse(charlistdata[i]["content"].ToString(), out num);

                if (isGet)
                {
                    if (num < BQ.childCount)
                        ItemTransform.Get<Image>("img").sprite = BQ.GetChild(num).GetComponent<Image>().sprite;
                }

                ItemTransform.GetTransform("back").gameObject.SetActive(false);
            }

            RawImage raw = ItemTransform.Get<RawImage>("img");
            LoadImage.GetLoadIamge.Load(charlistdata[i]["img"].ToString(), new RawImage[] { raw });
        }
    }


    //刷新好友列表
    private void UpdateFriendList()
    {
        Button btn_CreatGroup = transformData.GetButton("btn_friend");
        btn_CreatGroup.onClick.AddListener(delegate ()
        {
            Send();
            ReSetList();
        });
    }


    //好友列表
    private void CreatFriendList(JsonData friend_data, bool IsSelf = true)
    {
        CurrentFriendListid.Clear();

        ListGroup FriendList = ListCreatTools.Creat("Frined",
        transformData.GetTransform("Item_Frined").gameObject,
        transformData.GetTransform("Content_FriendList"));

        GameObject[] HotList = FriendList.HottingObj.ToArray();
        foreach (GameObject child in HotList)
        {
            FriendList.Destory(child, delegate ()
            {
                TransformData trns = child.GetComponent<TransformData>();
                trns.GetText("Name").text = "";
            });
        }

        if (friend_data == null)
            return;

        foreach (JsonData child in friend_data)
        {
            GameObject item = FriendList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>();
            RawImage raw = ItemTransform.Get<RawImage>("icon");
            LoadImage.GetLoadIamge.Load(child["img"].ToString(), new RawImage[] { raw });
            Toggle tog = ItemTransform.Get<Toggle>("Chose");
            tog.isOn = false;
            if (!IsSelf)
            {
                tog.gameObject.SetActive(true);
                tog.onValueChanged.RemoveAllListeners();
                tog.onValueChanged.AddListener(delegate (bool IsOn)
                {
                    if (IsOn)
                    {
                        if (!CurrentFriendListid.Contains(child["id"].ToString()))
                            CurrentFriendListid.Add(child["id"].ToString());
                    }
                    else
                    {
                        if (CurrentFriendListid.Contains(child["id"].ToString()))
                            CurrentFriendListid.Remove(child["id"].ToString());
                    }
                    Debug.Log(CurrentFriendListid.Count);
                });
            }
            else
            {
                tog.gameObject.SetActive(false);
            }

            ItemTransform.GetText("Name").text = child["name"].ToString();
            ItemTransform.GetButton("btn_delete").onClick.AddListener(delegate ()
            {
                DeleteFriend(child["id"].ToString(), item, FriendList);
            });
            ItemTransform.GetButton("btn_char").onClick.AddListener(delegate ()
            {
                FriendChar(child);
            });

            ItemTransform.GetButton("btn_eat").onClick.AddListener(delegate ()
            {
                WeiYangSned(child["id"].ToString());
            });

            ItemTransform.GetButton("btn_money").onClick.AddListener(delegate ()
            {
                MoneySend(child["id"].ToString());
            });


            ItemTransform.GetButton("btn_home").onClick.AddListener(delegate ()
            {
                MainScene mainScene = EntityHttpModel.Get<MainScene>();
                mainScene.GoToFriend(child["id"].ToString());
                WindowsManager.GetWindowsManager.CloseWindow(transformData.GetTransform("win_好友"));
            });
        }
    }


    //发红包
    private void MoneySend(string id)
    {
        Transform win_money = transformData.GetTransform("win_发送红包");
        WindowsManager.GetWindowsManager.OpenWindow(win_money);

        TransformData tran_sl = win_money.GetComponent<TransformData>();

        tran_sl.GetButton("btn_sure").onClick.AddListener(delegate ()
        {
            Http http = HttpCreatTools.CreatHttp("ajax_hongbao_send.php");
            http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
            http.AddData("hy_id", id);
            http.AddData("sl", tran_sl.GetInputField("InputField_Num"));
            http.Send(true);
            http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code != HttpCode.ERROR)
                {
                    MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                    WindowsManager.GetWindowsManager.CloseWindow(win_money);
                }
            });
        });
    }


    //喂养请求
    private void WeiYangSned(string id)
    {
        Http http = HttpCreatTools.CreatHttp("ajax_feed_send.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("hy_id", id);
        http.Send(true);
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }

    List<string> CurrentFriendListid = new List<string>();


    private void ReSetList()
    {
        transformData.GetTransform("btn_CreatGroup").gameObject.SetActive(true);
        transformData.GetTransform("btn_CreatGroupOK").gameObject.SetActive(false);
        transformData.GetTransform("btn_CreatGroupCANCLE").gameObject.SetActive(false);
    }


    //建群
    private void CreatGroupAction()
    {
        Http CreatGroupHttp = HttpCreatTools.CreatHttp("ajax_group_create.php");
        CreatGroupHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        string str = "";

        foreach (string child in CurrentFriendListid)
        {
            if (str == "")
                str += child;
            else
                str += "," + child;
        }
        if (str != "")
            CreatGroupHttp.AddData("id_str", str);
        CreatGroupHttp.Send();
        CreatGroupHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                transformData.Get<ButtonChangeGroup>("btnGroup").SetState(1);
                UpdateGroupListAction();
                // ReSetList();
            }
        });
    }


    //添加群友
    private void AddGroupItem(JsonData groupdata)
    {
        Http CreatGroupHttp = HttpCreatTools.CreatHttp("ajax_group_huiyuan_add.php");
        CreatGroupHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        string str = "";

        foreach (string child in CurrentFriendListid)
        {
            if (str == "")
                str += child;
            else
                str += "," + child;
        }
        if (str != "")
            CreatGroupHttp.AddData("id_str", str);

        CreatGroupHttp.AddData("group_id", groupdata["id"].ToString());

        CreatGroupHttp.Send();
        CreatGroupHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }

            transformData.Get<ButtonChangeGroup>("btnGroup").SetState(1);
            UpdateGroupListAction();
            // ReSetList();

        });
    }


    //选择好友
    private void ChoseFriend(Action action = null)
    {
        Button btn_CreatGroup = transformData.GetButton("btn_CreatGroup");
        Button btn_CreatGroupOK = transformData.GetButton("btn_CreatGroupOK");
        Button btn_CreatGroupCANCLE = transformData.GetButton("btn_CreatGroupCANCLE");

        btn_CreatGroup.onClick.RemoveAllListeners();
        btn_CreatGroup.onClick.AddListener(delegate ()
        {
            transformData.Get<ButtonChangeGroup>("btnGroup").SetState(0);
            CreatFriendList(Data.CurrentJsonData.Get(), false);

            btn_CreatGroup.gameObject.SetActive(false);
            btn_CreatGroupOK.gameObject.SetActive(true);
            btn_CreatGroupCANCLE.gameObject.SetActive(true);

            btn_CreatGroupOK.onClick.RemoveAllListeners();
            btn_CreatGroupOK.onClick.AddListener(delegate ()
            {
                if (action != null)
                {
                    action();
                    ReSetList();
                }
            });

            btn_CreatGroupCANCLE.onClick.RemoveAllListeners();
            btn_CreatGroupCANCLE.onClick.AddListener(delegate ()
            {
                btn_CreatGroup.gameObject.SetActive(true);
                btn_CreatGroupOK.gameObject.SetActive(false);
                btn_CreatGroupCANCLE.gameObject.SetActive(false);
            });
        });
    }


    //选择群友
    private void ChoseGroupFriend(JsonData groupdata, Action<JsonData> action = null)
    {
        Button btn_CreatGroup = transformData.GetButton("btn_CreatGroup");
        Button btn_CreatGroupOK = transformData.GetButton("btn_CreatGroupOK");
        Button btn_CreatGroupCANCLE = transformData.GetButton("btn_CreatGroupCANCLE");

        transformData.Get<ButtonChangeGroup>("btnGroup").SetState(0);
        CreatFriendList(Data.CurrentJsonData.Get(), false);

        btn_CreatGroup.gameObject.SetActive(false);
        btn_CreatGroupOK.gameObject.SetActive(true);
        btn_CreatGroupCANCLE.gameObject.SetActive(true);

        btn_CreatGroupOK.onClick.RemoveAllListeners();
        btn_CreatGroupOK.onClick.AddListener(delegate ()
        {
            if (action != null)
            {
                action(groupdata);
                ReSetList();
            }
        });

        btn_CreatGroupCANCLE.onClick.RemoveAllListeners();
        btn_CreatGroupCANCLE.onClick.AddListener(delegate ()
        {
            btn_CreatGroup.gameObject.SetActive(true);
            btn_CreatGroupOK.gameObject.SetActive(false);
            btn_CreatGroupCANCLE.gameObject.SetActive(false);
            transformData.Get<ButtonChangeGroup>("btnGroup").SetState(1);
            UpdateGroupListAction();
        });
    }

    //群列表
    private void UpdateGroup()
    {
        Button btn_CreatGroup = transformData.GetButton("btn_group");
        btn_CreatGroup.onClick.AddListener(delegate ()
        {
            UpdateGroupListAction();
            ReSetList();
        });
    }


    private void UpdateGroupListAction()
    {
        ListGroup GroupList = ListCreatTools.Creat("Group",
          transformData.GetTransform("Item_Group").gameObject,
          transformData.GetTransform("Content_GroupList"));

        Http ListGroupHttp = HttpCreatTools.CreatHttp("ajax_group_list.php");
        ListGroupHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        ListGroupHttp.Send();
        ListGroupHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                CreatGroupList(msg.Data["data"], GroupList);
            }
        });
    }


    //管理群
    private void Management(JsonData Groupdata)
    {
        Transform win_groupMan = transformData.GetTransform("win_群信息");
        WindowsManager.GetWindowsManager.OpenWindow(win_groupMan);
        TransformData transformDataMan = win_groupMan.GetComponent<TransformData>();
        transformDataMan.GetText("tag").text = Groupdata["group_name"].ToString();

        //transformDataMan.GetButton("btn_char").onClick.AddListener(delegate() 
        //{

        //});

        Http GroupUser = HttpCreatTools.CreatHttp("ajax_group_huiyuan_list.php");
        GroupUser.AddData("group_id", Groupdata["group_id"].ToString());
        GroupUser.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        GroupUser.Send(true);


        GroupUser.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                ListGroup GroupList = ListCreatTools.Creat("GroupList",
         transformDataMan.GetTransform("item").gameObject,
         transformDataMan.GetTransform("Content"));

                GameObject[] HotList = GroupList.HottingObj.ToArray();

                foreach (GameObject child in HotList)
                {
                    GroupList.Destory(child, delegate ()
                    {
                        TransformData trns = child.GetComponent<TransformData>();
                        trns.Get<RawImage>("img").texture = null;
                        trns.GetText("Name").text = "";
                    });
                }

                Button btn_Add_Frined = transformDataMan.GetButton("btn_Add_Friend");
                btn_Add_Frined.transform.SetParent(win_groupMan);

                JsonData charlistdata = msg.Data["data"];

                if (charlistdata == null)
                    return;

                // btn_delete.onClick.RemoveAllListeners();
                foreach (JsonData child in charlistdata)
                {
                    GameObject item = GroupList.Instantiate();
                    TransformData ItemTransform = item.GetComponent<TransformData>();
                    ItemTransform.GetText("Name").text = child["name"].ToString();
                    RawImage raw = ItemTransform.Get<RawImage>("img");
                    LoadImage.GetLoadIamge.Load(child["img"].ToString(), new RawImage[] { raw });
                    Button btn_delete_Item = ItemTransform.GetButton("btn_delete");
                    btn_delete_Item.onClick.AddListener(delegate ()
                    {
                        //删除好友
                        Http GroupRemoveUserHttp = HttpCreatTools.CreatHttp("ajax_group_huiyuan_del.php");
                        GroupRemoveUserHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
                        GroupRemoveUserHttp.AddData("id_str", child["id"].ToString());
                        GroupRemoveUserHttp.AddData("group_id", Groupdata["group_id"].ToString());
                        GroupRemoveUserHttp.Send();
                        GroupRemoveUserHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage d_msg)
                        {
                            if (msg.Code == HttpCode.SUCCESS)
                            {
                                MessageManager.GetMessageManager.WindowShowMessage(d_msg.Data["msg"].ToString());
                                WindowsManager.GetWindowsManager.CloseWindow(win_groupMan);
                                UpdateGroupListAction();
                            }
                        });
                    });

                    //btn_delete.onClick.AddListener(delegate ()
                    //{
                    //    btn_delete_Item.gameObject.SetActive(false);
                    //});
                }

                btn_Add_Frined.transform.SetParent(transformDataMan.GetTransform("Content"));
                btn_Add_Frined.gameObject.SetActive(true);


                btn_Add_Frined.onClick.RemoveAllListeners();
                btn_Add_Frined.onClick.AddListener(delegate ()
                {
                    //添加好友
                    WindowsManager.GetWindowsManager.CloseWindow(win_groupMan);
                    ChoseGroupFriend(Groupdata, AddGroupItem);
                });

            }
        });


    }


    //刷新群列表
    private void CreatGroupList(JsonData friend_data, ListGroup GroupList)
    {
        GameObject[] HotList = GroupList.HottingObj.ToArray();
        foreach (GameObject child in HotList)
        {
            GroupList.Destory(child, delegate ()
            {
                TransformData trns = child.GetComponent<TransformData>();
                trns.GetText("Name").text = "";

            });
        }

        if (friend_data == null)
            return;
        foreach (JsonData child in friend_data)
        {
            GameObject item = GroupList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>().Init();
            ItemTransform.GetText("Name").text = child["group_name"].ToString();
            item.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                GroupChar(child);
            });
            Button btn_man = ItemTransform.GetButton("btn_guanli");
            if (child["admin"].ToString() == "1")
            {
                btn_man.gameObject.SetActive(true);
                btn_man.onClick.AddListener(delegate ()
                {
                    Management(child);
                });
            }
            else
            {
                btn_man.gameObject.SetActive(false);
            }

        }
    }

    //打开群聊天
    private void GroupChar(JsonData chardata)
    {
        string ID = chardata["group_id"].ToString();
        Transform win_char = GetWinTransrorm("win_群聊对话框");
        WindowsManager.GetWindowsManager.OpenWindow(win_char);
        TransformData charTransfoem = win_char.GetComponent<TransformData>();
        Transform biaoqing = charTransfoem.GetTransform("Content_biaoqing");

        GetGroupChar(ID, charTransfoem, biaoqing);

        charTransfoem.GetText("Desc").text = string.Format("正在与{0}聊天", chardata["group_name"].ToString());

        Http FrienSendCharHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_group_chat_put.php");
        FrienSendCharHttp.AddData("group_id", ID);
        FrienSendCharHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));

        charTransfoem.GetButton("btn_send").onClick.RemoveAllListeners();
        charTransfoem.GetButton("btn_send").onClick.AddListener(delegate ()
        {
            FrienSendCharHttp.AddData("content", charTransfoem.GetInputField("InputField_char"));
            FrienSendCharHttp.AddData("type", "1");
            FrienSendCharHttp.Send(true);
            FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {
                    pageGroup = 0;
                    GetGroupChar(ID, charTransfoem, biaoqing);
                    Debug.Log("SNED");
                }
            });
        });

        charTransfoem.GetButton("btn_biaoqing").onClick.RemoveAllListeners();
        charTransfoem.GetButton("btn_biaoqing").onClick.AddListener(delegate ()
        {
            Transform win_bq = charTransfoem.GetTransform("img");
            win_bq.gameObject.SetActive(true);

            foreach (Transform child in biaoqing)
            {
                child.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    FrienSendCharHttp.AddData("type", "2");
                    FrienSendCharHttp.AddData("content", child.GetSiblingIndex().ToString());
                    FrienSendCharHttp.Send(true);
                    FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
                    {
                        if (msg.Code == HttpCode.SUCCESS)
                        {
                            win_bq.gameObject.SetActive(false);
                            pageGroup = 0;
                            GetGroupChar(ID, charTransfoem, biaoqing);
                            Debug.Log("SNED");
                        }
                    });
                });
            }
        });

        charTransfoem.GetButton("close").onClick.AddListener(delegate ()
        {
            if (Group != null)
                StopCoroutine(Group);
        });

        if (Group != null)
            StopCoroutine(Group);
        Group = StartCoroutine(UpdateGroup(ID, charTransfoem, biaoqing));
    }

    Coroutine Group = null;
    public int pageGroup = 0;
    private string LastGroupID = "";
    //拉取群聊聊天记录
    private void GetGroupChar(string ID, TransformData friencharitem, Transform BQ)
    {
        Http FrienCharListHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_group_chat.php");
        //if (LastGroupID != ID)
        //    pageGroup = 0;
        //LastGroupID = ID;

        FrienCharListHttp.AddData("group_id", ID);
        FrienCharListHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        FrienCharListHttp.AddData("page", "0");
        FrienCharListHttp.Send(true, true);
        FrienCharListHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                pageGroup++;
                UpdateFriendChar(msg.Data.Keys.Contains("data") ? msg.Data["data"] : null, friencharitem, BQ, "GroupChar");
            }
        });
    }
    //刷新群聊天记录

    IEnumerator UpdateGroup(string ID, TransformData charTransfoem, Transform BQ)
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            GetGroupChar(ID, charTransfoem, BQ);
        }
    }


    protected override string InitUrl()
    {
        return "ajax_huiyuan_list.php";
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        JsonData jsonData = null;
        if (EventData.Code == HttpCode.SUCCESS)
        {
            jsonData = EventData.Data["data"];
        }
        CreatFriendList(jsonData);
    }

    protected override void OnResponesJsonDataEvent(JsonData jsonData)
    {

    }

}
