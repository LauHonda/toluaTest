using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class FindPassword : HttpTransformBLL
{

    protected override void Init()
    {
        base.Init();
        Data.AddData("tel", transformData.GetInputField("tel"));
        Data.AddData("password", transformData.GetInputField("password"));
        Data.AddData("con_password", transformData.GetInputField("con_password"));
        Data.AddData("yzm", transformData.GetInputField("verification code"));
        Button btn_find = transformData.GetButton("btn_find");
        btn_find.onClick.AddListener(Send);


        //发送验证码
        Button btn_VerCode = transformData.GetButton("btn_verification code");
        HttpVerCode VerCodeMole = new HttpVerCode("ajax_yzm.php", btn_VerCode, transformData.GetInputField("tel"));
    }

    protected override string InitUrl()
    {
        return "ajax_zhmm.php";
    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
    }

}
