require "Common/define"

require "3rd/pblua/login_pb"
require "3rd/pbc/protobuf"

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local print_r = require "3rd/sproto/print_r"

LoginCtrl = {};
local this = LoginCtrl;

local LuaBehaviour;
local transform;
local gameObject;


local Account;
local PassWord;
--构建函数--
function LoginCtrl.New()
    logWarn("LoginCtrl.New--->>");
    return this;
end

function LoginCtrl.Awake()
    logWarn("LoginCtrl.Awake--->>");
    panelMgr:CreatePanel('Login', this.OnCreate);
end

--启动事件--
function LoginCtrl.OnCreate(obj)
    gameObject = obj;
    transform = obj.transform;

    LuaBehaviour = transform:GetComponent('LuaBehaviour');
    
    Account = AccountPersistence(LoginPanel.Rem_Account,LoginPanel.tel:GetComponent('InputField'),"account");
    PassWord = AccountPersistence(LoginPanel.Rem_Password,LoginPanel.password:GetComponent('InputField'),"password");
    
    Account:Read();
    PassWord:Read();

    LoginPanel.tel:GetComponent('InputField').onEndEdit:AddListener(this.VerCode);
    LuaBehaviour:AddClick(LoginPanel.EventBtn, this.testOnClick);
    LuaBehaviour:AddClick(LoginPanel.yzmbtn,this.VerCode);
end

function LoginCtrl.VerCode()
    local http = Http("ajax_login_yzm.php");
    http:AddData("tel",LoginPanel.tel:GetComponent('InputField').text);
    http.CurrentData:AddChangeListener(this.VerCodeCallBack);
    http:Send(false,true);
end

function LoginCtrl.VerCodeCallBack(go)
    local  cjson = require "cjson"
    if(go.Code == HttpCode.SUCCESS)
    then
        local JsonGroup=cjson.decode(LitJson.JsonMapper.ToJson(go.Data));
        LoginPanel.yzmtext:GetComponent('Text').text = JsonGroup.data;
    elseif(go.Code == HttpCode.FAILED)
    then
    local JsonGroup=cjson.decode(LitJson.JsonMapper.ToJson(go.Data));
        MessageManager.GetMessageManager:WindowShowMessage(JsonGroup.msg);
    elseif(go.Code == HttpCode.ERROR)
    then
        MessageManager.GetMessageManager:WindowShowMessage("请输入账号");
    end
end


--点击事件--
function LoginCtrl.testOnClick(go)
    local http = Http("ajax_login.php");
    http:AddData("tel",LoginPanel.tel:GetComponent('InputField').text);
    http:AddData("password",LoginPanel.password:GetComponent('InputField').text);
    http:AddData("yzm",LoginPanel.yzm:GetComponent('InputField').text);
    http.CurrentData:AddChangeListener(this.Loading_CallBack);
    http:Send();
end

--成功回调--
function  LoginCtrl.Loading_CallBack(go)
    logWarn(LitJson.JsonMapper.ToJson(go.Data));
     --引入Cjson
    local cjson=require "cjson"
     --*** 进行加密 *** --
    if(go.Code == HttpCode.SUCCESS)
    then
        Account:Write();
        PassWord:Write();
    elseif(go.Code == HttpCode.FAILED)
    then
    local JsonGroup=cjson.decode(LitJson.JsonMapper.ToJson(go.Data));
        MessageManager.GetMessageManager:WindowShowMessage(JsonGroup.msg);
    elseif(go.Code == HttpCode.ERROR)
    then
        logWarn("错误回调123");
    end
     --引入Cjson
    --local cjson=require "cjson"
     --*** 进行加密 *** --

     --*** 解析Cjson*** --
     --将一个json字符串转换为对象
     --local JsonGroup=cjson.decode(json) 
     --TestPanel.Worning:GetComponent('Text').text = JsonGroup.msg;
     --LuaFramework.Util.LoadScene("menu");
     --GameMenu:OnInitOK();
end
--关闭事件--
function LoginCtrl.Close()
    panelMgr:ClosePanel(CtrlNames.Test);
end