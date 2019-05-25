-- @FileName UIBaseWind
-- @Create by mx
-- @Create time 2019/05/24 22:08:38
-- @FileInfo 继承UIbase 实现普通的UI类型的基类逻辑

require("UI/UIBase")

module("UI",package.seeall)

--本地存储GetUIBase
local l_superBase = UI.UIBase
--申明UIBaseWind基类
UIBaseWind = Class("UIBaseWind",l_superBase)

--UI的层级
UISortLayer = 
{
    Top,
    Up,
    Normal,
    Hud 
}

--UI的激活类型
UIActiveType = 
{
    Normal,
    Exclusive,
    StandAlone  
}

function UIBaseWind:Ctor( windName,windSortLayer,windActiveType )
    --父类Ctor
    l_superBase.Ctor()
    --UIName
    self.m_uiName = windName
    --UI层级
    self.m_sortLayer = windSortLayer or UISortLayer.Normal
    --UI激活类型
    self.m_activeType = windActiveType or UIActiveType.Normal
end

function UIBaseWind:Active(callBack)
    if self.m_uiObj and self.m_isInited then
        self.m_uiTrans:SetAsLastSibling()
        self.m_uiObj:SetActiveEx(true)
        if type(callBack) == "function" then
            callBack(self)
            return
        end
    end
    
    self:load(function(uiWind)
        
    end)
end


return UIBaseWind
