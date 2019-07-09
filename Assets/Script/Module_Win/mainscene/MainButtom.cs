using LitJson;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class MainButtom : HttpBase
{

    TransformData transformData;

    protected override void Init()
    {
        transformData = GetComponent<TransformData>();

        Button btn_qiandao = transformData.GetButton("qiandao");
        Button btn_yaoqing = transformData.GetButton("yaoqing");
        Button btn_dati = transformData.GetButton("dati");
        Button btn_tongjiaosuo = transformData.GetButton("tongjiaosuo");

        btn_qiandao.onClick.AddListener(delegate ()
        {
            Qiandao();
        });

        btn_yaoqing.onClick.AddListener(delegate ()
        {
            YaoQing();
        });

        btn_dati.onClick.AddListener(delegate ()
        {
            TiKu();
        });

        btn_tongjiaosuo.onClick.AddListener(delegate ()
        {
            TongJiaoSuo();
        });
    }


    //签到
    public void Qiandao()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_qiandao.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send(true);
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code != HttpCode.ERROR)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }


    //邀请
    private void YaoQing()
    {
        Transform win_Share = transformData.GetTransform("win_分享二维码");
        WindowsManager.GetWindowsManager.OpenWindow(win_Share);
    }

    //获取题库
    private void TiKu()
    {
        Http http = HttpCreatTools.CreatHttp("ajax_question_get.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.Send();
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                Transform win_dati = transformData.GetTransform("win_答题");
                WindowsManager.GetWindowsManager.OpenWindow(win_dati);
                TransformData AnswerData = win_dati.GetComponent<TransformData>();
                JsonData jd = msg.Data["data"];
                AnswerData.GetText("desc").text = jd["nr"].ToString();

                DoLayout(jd, AnswerData);
            }
        });
    }


    //显示题目
    private void DoLayout(JsonData jsondata, TransformData AnswerWin)
    {
        ListGroup AnswerList = ListCreatTools.Creat("AnswerList",
          AnswerWin.GetTransform("item_chose").gameObject,
          AnswerWin.GetTransform("Content_AnswerList"));

        GameObject[] HotList = AnswerList.HottingObj.ToArray();
        foreach (GameObject child in HotList)
        {
            AnswerList.Destory(child, delegate ()
            {
                TransformData trns = child.GetComponent<TransformData>();
                //trns.GetText("Lable").text = "";
            });
        }
        //if (jsondata == null)
        //    return;

        // foreach (JsonData Answerchild in jsondata)
        //{



        GameObject itemA = AnswerList.Instantiate();
        itemA.GetComponent<TransformData>().GetText("Label").text = jsondata["A"].ToString();

        GameObject itemB = AnswerList.Instantiate();
        itemB.GetComponent<TransformData>().GetText("Label").text = jsondata["B"].ToString();


        itemA.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
        {
            if (isOn)
                Answer(jsondata, "1", AnswerWin);
        });
        itemB.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
        {
            if (isOn)
                Answer(jsondata, "2", AnswerWin);
        });
        itemA.GetComponent<Toggle>().isOn = true;
    }

    //答题
    private void Answer(JsonData jsondata, string ID, TransformData win_ans)
    {
        Button btn_send = win_ans.GetButton("btn_send");

        btn_send.onClick.RemoveAllListeners();
        btn_send.onClick.AddListener(delegate ()
        {
            Http http = HttpCreatTools.CreatHttp("ajax_question_put.php");
            http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
            http.AddData("question_id", jsondata["id"].ToString());
            http.AddData("answer", ID);
            http.Send();
            http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
            {
                if (msg.Data["data"].ToString() == "1")
                {
                    WindowsManager.GetWindowsManager.OpenWindow(transformData.GetTransform("win_答题YES"));
                }
                else if (msg.Data["data"].ToString() == "2")
                {
                    WindowsManager.GetWindowsManager.OpenWindow(transformData.GetTransform("win_答题NO"));
                }
                else
                {
                    MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
                }
            });
        });
    }


    //通交所
    private void TongJiaoSuo()
    {
        // Transform win_TJS = transformData.GetTransform("");
        Http http = HttpCreatTools.CreatHttp("ajax_tjs_bind.php");
        http.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        http.AddData("name", "干死你不偿命");
        http.Send(true);
        http.CurrentData.AddChangeListener(delegate (HttpCallBackMessage msg)
        {
            if (msg.Code == HttpCode.SUCCESS)
            {
                MessageManager.GetMessageManager.WindowShowMessage(msg.Data["msg"].ToString());
            }
        });
    }

}
