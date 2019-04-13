 -- @FileName Main
 -- @Create by mx
 -- @Create time 2019/02/23 22:06:38

-- lua垃圾回收设置 避免内存泄露 设置内存回收参数
collectgarbage("setpause", 100)
collectgarbage("setstepmul", 5000)

require("Common/Class")
require("Common/RestrictGlobal")

--预加载一些数据
local PreLoadDate = function ()
    require "ModuleMgr/MgrMgr"
    declareGlobal("MgrMgr",MoudleMgr.MgrMgr.new())
end

--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")
end

--Lua 逻辑入口
local Start = function()
    require "Game"
    declareGlobal("Game", Game.new())
    Game:Init()
end

function Close()
	Game:Uninit()
end

--场景切换内存回收
function OnLevelWasLoaded(level)
	collectgarbage("collect")
end

PreLoadDate()
Start()