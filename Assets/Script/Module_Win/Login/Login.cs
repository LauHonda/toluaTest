using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class Login : HttpTransformBLL
{
    [SerializeField]
    CreatImgVerCode m_CreatImgVerCode;

    private InputField tel_InputField;
    private InputField password_InputField;
    private InputField verificationCode_InputField;

    AccountPersistence Account;
    AccountPersistence PassWord;

    protected override void Init()
    {
        base.Init();
        tel_InputField = transformData.GetInputField("tel");
        password_InputField = transformData.GetInputField("password");
        verificationCode_InputField = transformData.GetInputField("verification code");

        InitData();
    }

    private void InitData()
    {

        //初始化http数据
        Data.AddData("tel", tel_InputField);
        Data.AddData("password", password_InputField);
        Data.AddData("yzm", verificationCode_InputField);
        transformData.GetButton("btn_login").onClick.AddListener(Send);

        HttpImgVerCode ImgCode = new HttpImgVerCode("ajax_login_yzm.php", m_CreatImgVerCode);
        //手机号改变时候获取图片验证码
        ImgCode.AddEvent(tel_InputField.onEndEdit);
        //点击验证码图片时候获取手机验证码
        ImgCode.AddEvent(transformData.GetButton("btn_SendVerCode").onClick, tel_InputField);

        //读取账号信息
        Account = new AccountPersistence(AccountToggle, tel_InputField, "account");
        PassWord = new AccountPersistence(PasswordToggle, password_InputField, "password");
        Account.Read();
        PassWord.Read();

        //程序启动时自动获取一次验证码
        ImgCode.Send(tel_InputField.text);
    }

    [SerializeField]
    private Toggle AccountToggle, PasswordToggle;


    protected override void OnResponesEvent(HttpCallBackMessage EventData)
    {
        if (EventData.Code == HttpCode.SUCCESS)
        {
            JsonData data = EventData.Data;

            CachingRegion.Add("huiyuan_id", data["data"]["huiyuan_id"].ToString());

            Account.Write();
            PassWord.Write();

            SceneManager.LoadScene("LabbyScene");
        }
        else if (EventData.Code != HttpCode.ERROR)
        {
            MessageManager.GetMessageManager.WindowShowMessage(EventData.Data["msg"].ToString());
        }
    }

    protected override string InitUrl()
    {
        return "ajax_login.php";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
