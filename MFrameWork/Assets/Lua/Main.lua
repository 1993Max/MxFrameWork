 -- @FileName Main
 -- @Create by mx
 -- @Create time 2019/02/23 22:06:38
 -- @FileInfo 这是一个Lua逻辑初始化的入口文件 C#初始化Lua虚拟机 初始化函数

-- lua垃圾回收设置 避免内存泄露 设置内存回收参数
collectgarbage("setpause", 100)
collectgarbage("setstepmul", 5000)

require("Common/Define")
require("Common/Class")
require("Common/Log")
--需要全局访问的Lua文件在者之前Require
require("Common/RestrictGlobal")

local Main = {}

--预加载一些数据
local PreLoadDate = function ()
    require "ModuleMgr/MgrMgr"
    DeclareGlobal("MgrMgr",MoudleMgr.MgrMgr.New())
end

--主入口函数。从这里开始lua逻辑
function Main.OnStart()
    Log("lua logic start")
    require "Game"
    --require "UI/UIBase"
    Game:Init()
end

function Main.OnClose()
	Game:Uninit()
end

function Main:InitHotUpdate(params)
    
end

Main.OnStart()