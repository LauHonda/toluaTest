using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
public class Register : HttpTransformBLL
{

    protected override void Init()
    {
        base.Init();
        Button btn_Reg = transformData.GetButton("btn_register");
        btn_Reg.onClick.AddListener(DoAction);

        //发送验证码
        Button btn_VerCode = transformData.GetButton("btn_verification code");
        HttpVerCode VerCodeMole = new HttpVerCode("ajax_yzm.php", btn_VerCode, transformData.GetInputField("tel"));
    }


    private void DoAction()
    {     
        Data.AddData("tel", transformData.GetInputField("tel"));
        Data.AddData("name", transformData.GetInputField("nickname"));
        Data.AddData("farm_name", transformData.GetInputField("farmname"));
        Data.AddData("tel_yzm", transformData.GetInputField("verification code"));
        Data.AddData("password", transformData.GetInputField("password"), null, "con_password", CompareTools.CompareEqual);
        Data.AddData("con_password", transformData.GetInputField("con_password"));
        Data.AddData("tj_tel", transformData.GetInputField("Invitation code"));

        Data.AddData("sex", transformData.Get<ToggleGroup>("sex"));

        InputField birthday_year = transformData.GetInputField("birthday_year");
        InputField birthday_month = transformData.GetInputField("birthday_month");
        InputField birthday_day = transformData.GetInputField("birthday_day");

        Data.AddData("birthday", birthday_year.text + "-" + birthday_month.text + "-" + birthday_year.text);
        Data.AddData("address", transformData.GetInputField("shipping address"));
        Data.AddData("money_url", transformData.GetInputField("Monetary address"));

        Send();

    }

    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
    }

    protected override string InitUrl()
    {
        return "ajax_reg.php";
    }
}
