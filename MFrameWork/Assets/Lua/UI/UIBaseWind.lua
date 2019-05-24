-- @FileName UIBaseWind
-- @Create by mx
-- @Create time 2019/05/24 22:08:38
-- @FileInfo 继承UIbase 实现普通的UI类型的基类逻辑

require("UI/UIBase")

module("UI",package.seeall)

--本地存储GetUIBase
local l_superBase = UI.UIBase
--全局申明UIBase基类
DeclareGlobal("UIBaseWind",class("UIBaseWind",l_superBase))

--UI的层级
UILayer = 
{
    Top,
    Up,
    Normal,
    Hud 
}

--UI的类型
UIActiveType = 
{
    Normal,
    Exclusive,
    StandAlone  
}



return UIBaseWind
