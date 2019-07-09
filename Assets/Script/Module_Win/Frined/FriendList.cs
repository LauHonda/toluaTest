using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using UnityEngine.UI;

public class FriendList : HttpTransformBLL
{
    protected override void Init()
    {
        base.Init();
        OpenFriendWin();
        OpenSearchWin();
        CreatGroup();
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
            InputField_SearchFriend.onEndEdit.RemoveAllListeners();
            InputField_SearchFriend.onEndEdit.AddListener(delegate (string str)
            {
                SearchFriend(str, searchTransform);
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
            if (msg.Code == HttpCode.SUCCESS)
            {
                JsonData jsdata = msg.Data["data"];
                searchTrns.GetText("desc").text = jsdata["name"].ToString();
                searchTrns.GetButton("btn_add").onClick.AddListener(delegate ()
                {
                    AddFriend(jsdata["id"].ToString());
                });
            }
            else if (msg.Code == HttpCode.FAILED)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
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

        GetFriendChar(ID, charTransfoem);

        charTransfoem.GetText("Desc").text = string.Format("正在与{0}聊天", chardata["name"].ToString());

        Http FrienSendCharHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_chat_put.php");
        FrienSendCharHttp.AddData("id", ID);
        FrienSendCharHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        FrienSendCharHttp.AddData("content", charTransfoem.GetInputField("InputField_char"));
        charTransfoem.GetButton("btn_send").onClick.AddListener(delegate ()
        {
            FrienSendCharHttp.AddData("type", "1");
            FrienSendCharHttp.Send();
            FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {

                }
            });
        });

        charTransfoem.GetButton("btn_biaoqing").onClick.AddListener(delegate ()
        {
            FrienSendCharHttp.AddData("type", "2");
            FrienSendCharHttp.Send();
            FrienSendCharHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Code == HttpCode.SUCCESS)
                {

                }
            });
        });
    }


    public int page = 0;
    //拉取好友聊天记录
    private void GetFriendChar(string ID, TransformData friencharitem)
    {
        Http FrienCharListHttp = HttpCreatTools.CreatHttp("ajax_huiyuan_chat.php");
        FrienCharListHttp.AddData("id", ID);
        FrienCharListHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        FrienCharListHttp.AddData("page", page);
        FrienCharListHttp.Send(true);
        FrienCharListHttp.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                page++;
                UpdateFriendChar(msg.Data["data"], friencharitem);
            }
        });
    }


    //刷新好友消息
    private void UpdateFriendChar(JsonData charlistdata, TransformData charWin)
    {
        ListGroup FriendCharList = ListCreatTools.Creat("FrinedChar",
           charWin.GetTransform("item").gameObject,
           charWin.GetTransform("Content_FriendCharList"));

        GameObject[] HotList = FriendCharList.HottingObj.ToArray();
        foreach (GameObject child in HotList)
        {
            FriendCharList.Destory(child, delegate ()
             {
                 TransformData trns = child.GetComponent<TransformData>();
                 trns.GetText("name").text = "";
                 trns.GetText("char").text = "";
             });
        }

        if (charlistdata == null)
            return;
        foreach (JsonData child in charlistdata)
        {
            GameObject item = FriendCharList.Instantiate();
            TransformData ItemTransform = item.GetComponent<TransformData>();
            //ItemTransform.Get<RawImage>("icon");
            ItemTransform.GetText("name").text = child["name"].ToString();
            ItemTransform.GetText("char").text = child["content"].ToString();
            // item.name = child["id"].ToString();
        }
    }

    //刷新好友列表`
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
            //ItemTransform.Get<RawImage>("icon");
            Toggle tog = ItemTransform.Get<Toggle>("Chose");
            tog.isOn = false;
            if (!IsSelf)
            {
                tog.gameObject.SetActive(true);
                tog.onValueChanged.RemoveAllListeners();
                tog.onValueChanged.AddListener(delegate (bool IsOn)
                {
                    Debug.Log(IsOn);
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
    private void CreatGroup()
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
                Http CreatGroupHttp = HttpCreatTools.CreatHttp("ajax_group_create.php");
                CreatGroupHttp.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
                string str = "";
                Debug.Log(CurrentFriendListid.Count);
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
                        btn_CreatGroupOK.gameObject.SetActive(false);
                        btn_CreatGroupCANCLE.gameObject.SetActive(false);
                        btn_CreatGroup.gameObject.SetActive(true);
                    }
                });
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


        Http GroupUser = HttpCreatTools.CreatHttp("ajax_group_huiyuan_list.php");
        GroupUser.AddData("group_id", Groupdata["group_id"].ToString());
        GroupUser.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        GroupUser.Send(true);


        //Button btn_delete = transformDataMan.GetButton("btn_Delete");
        //btn_delete.onClick.AddListener(delegate ()
        //{

        //});
        Button btn_Add_Frined = transformDataMan.GetButton("btn_Add_Friend");
        btn_Add_Frined.onClick.AddListener(delegate ()
        {
            //添加好友
        });

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
                    //  ItemTransform.GetText("img").text 
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
            Button btn_man = ItemTransform.GetButton("btn_guanli");
            btn_man.onClick.AddListener(delegate ()
            {
                Management(child);
            });
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
