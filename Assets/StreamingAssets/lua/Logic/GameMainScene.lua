require "3rd/pblua/login_pb"
require "3rd/pbc/protobuf"

local lpeg = require "lpeg"

local json = require "cjson"
local util = require "3rd/cjson/util"

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local print_r = require "3rd/sproto/print_r"

require "Logic/LuaClass"
require "Logic/CtrlManager"
require "Common/functions"
require "Controller/PromptCtrl"
require "Controller/LoginCtrl"

GameMainScene = {};
local this = GameMainScene;

--初始化完成，发送链接服务器信息--
function GameMainScene.OnInitOK()  

end


--销毁--
function GameMainScene.OnDestroy()
	--logWarn('OnDestroy--->>>');
end
