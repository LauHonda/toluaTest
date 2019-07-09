using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class Setting : HttpTransformBLL
{

    protected override void Init()
    {
        base.Init();
        Data.AddData("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        Data.AddData("password", transformData.GetInputField("password"));
        Data.AddData("new_password", transformData.GetInputField("new_password"), null, "con_password", CompareTools.CompareEqual);
        Data.AddData("con_password", transformData.GetInputField("con_password"));

        Button btn_find = transformData.GetButton("btn_sure");
        btn_find.onClick.AddListener(Send);
    }

    protected override string InitUrl()
    {
        return "ajax_pass.php";
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        if (EventData.Code == HttpCode.SUCCESS)
        {
            WindowsManager.GetWindowsManager.CloseWindow(transformData.GetTransform("win_修改密码"));
        }
        MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
    }

}
