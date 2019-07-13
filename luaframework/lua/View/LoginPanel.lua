local transform;
local gameObject;

LoginPanel = {};
local this = LoginPanel;

--启动事件--
function LoginPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
	logWarn("Awake lua--->>"..gameObject.name);
end

--初始化面板--
function LoginPanel.InitPanel()
	this.EventBtn = transform:Find('Button').gameObject;
	this.box1 = transform:Find('box1');
	this.box2 = transform:Find('box2');
	this.box3 = transform:Find('box3');
	this.tel = transform:Find('Input_Tel');
	this.password = transform:Find('Input_PassWord');
	this.yzm = transform:Find('Input_Yzm');
	this.yzmbtn = transform:Find('VerCode').gameObject;
	this.yzmtext = transform:Find('VerCode/Text');
	this.Rem_Account = transform:Find('Rem_Account'):GetComponent('Toggle');
	this.Rem_Password = transform:Find('Rem_Password'):GetComponent('Toggle');
end

--单击事件--
function LoginPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end