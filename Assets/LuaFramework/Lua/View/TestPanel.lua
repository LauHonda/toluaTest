local transform;
local gameObject;

TestPanel = {};
local this = TestPanel;

--启动事件--
function TestPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
	logWarn("Awake lua--->>"..gameObject.name);
end

--初始化面板--
function TestPanel.InitPanel()
	this.EventBtn = transform:Find('Button').gameObject;
	this.Worning = transform:Find('worning');
	this.box1 = transform:Find('box1');
	this.box2 = transform:Find('box2');
	this.box3 = transform:Find('box3');
	this.tel = transform:Find('Input_Tel');
	this.password = transform:Find('Input_PassWord');
	this.yzm = transform:Find('Input_Yzm');
end

--单击事件--
function TestPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end