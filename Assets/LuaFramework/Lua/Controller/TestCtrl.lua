require "Common/define"

require "3rd/pblua/login_pb"
require "3rd/pbc/protobuf"

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local print_r = require "3rd/sproto/print_r"

TestCtrl = {};
local this = TestCtrl;

local test;
local httpFuntion;
local transform;
local gameObject;

--构建函数--
function TestCtrl.New()
    logWarn("TestCtrl.New--->>");
    return this;
end

function TestCtrl.Awake()
    logWarn("TestCtrl.Awake--->>");
    panelMgr:CreatePanel('Test', this.OnCreate);
end

--启动事件--
function TestCtrl.OnCreate(obj)
    gameObject = obj;
    transform = obj.transform;

    test = transform:GetComponent('LuaBehaviour');
    httpFuntion = transform:GetComponent('Currency_Module');
    logWarn("Start lua--->>"..gameObject.name);

    test:AddClick(TestPanel.EventBtn, this.testOnClick);
end





--点击事件--
function TestCtrl.testOnClick(go)
    --httpFuntion:test("213");
    httpFuntion:SetUrl("ajax_login.php");
    httpFuntion:AddSendValue("tel",TestPanel.tel:GetComponent('InputField').text);
    httpFuntion:AddSendValue("password",TestPanel.password:GetComponent('InputField').text);
    httpFuntion:AddSendValue("yzm",TestPanel.yzm:GetComponent('InputField').text);
    httpFuntion:SendDate("TestCtrl");
end

--获取接口返回值--
function  TestCtrl.GetJson(json)
    logWarn(json);
     --引入Cjson
    local cjson=require "cjson"
     --*** 进行加密 *** --

     --*** 解析Cjson*** --
     --将一个json字符串转换为对象
     local JsonGroup=cjson.decode(json)
     TestPanel.Worning:GetComponent('Text').text = JsonGroup.msg;
     logWarn("JsonGroup.msg =" ..JsonGroup.msg)
end


--关闭事件--
function TestCtrl.Close()
    panelMgr:ClosePanel(CtrlNames.Test);
end