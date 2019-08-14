 -- @FileName UIManager
 -- @Create by mx
 -- @Create time 2019/05/26 10:27:37
 -- @FileInfo: UI管理类

 module("UI",package.seeall)

 --定义UIManager
 UIManager = class("UIManager")

 --UI的层级
UISortLayer = 
{
    Top,
    Up,
    Normal,
    Hud 
}

function UIManager:Ctor()
    --存储一些节点信息
    self.m_uiRootTrans = nil
    self.m_topLayerTrans = nil
    self.m_upLayerTrans = nil
    self.m_normalLayerTrans = nil
    self.m_hudLayerTrans = nil

    --存储所有已经初始化过得UI的集合
    self.m_uiWindTb = {} 
    --存储当前展示的UI集合
    self.m_uiActiveWindTb = {}
    --存储当前UI的堆栈
    self.m_uiStack = {}
end

function UIManager:Init()
    --关键节点赋值
    self.m_uiRootTrans = GameObject.Find("UIRoot")
    self.m_topLayerTrans = self.m_uiRootTrans.transform:Find("TopLayer")
    self.m_upLayerTrans = self.m_uiRootTrans.transform:Find("UpperLayer")
    self.m_normalLayerTrans = self.m_uiRootTrans.transform:Find("NormalLayer")
    self.m_hudLayerTrans = self.m_uiRootTrans.transform:Find("HudLayer")
    --预加载
    self.PreLoadUI()
end

--需要预加载的UI
function UIManager:PreLoadUI()

end


--初始化UI
function UIManager:InitUIWind(uiName)
    if uiName == nil then
        LogError("UIName is Nil Check u Code")
        return
    end
    --初始化UI逻辑
    local l_uiFullName = GetUIWindFullName(uiName)
    if self.m_uiWindTb[uiName] == nil then
        require("UI/Wind/"..l_uiFullName)
        if UI[l_uiFullName] ~= nil then
            self.m_uiWindTb[l_uiFullName] = UI[l_uiFullName].New()
        end
    end
    return self.m_uiWindTb[uiName]
end

function UIManager:GetUIWindFullName(windName)
    local l_suffix = MPathUtils.UI_LOGIC_WIND_SUFFIX
    return windName..l_suffix
end

--UI管理器的UnInit
function UIManager:UnInit()
    for key, value in pairs(self.m_uiWindTb) do
        if value then
            if value:IsInited() then
                value:UnInit()
            end
        end
    end
    --数据Reset
    self.m_uiWindTb = nil
    self.m_uiActiveWindTb = nil
    self.m_uiStack = nil
    --节点信息重置
    self.m_uiRootTrans = nil
    self.m_topLayerTrans = nil
    self.m_upLayerTrans = nil
    self.m_normalLayerTrans = nil
    self.m_hudLayerTrans = nil
end

function UIManager:ActiveUI(uiName,callBack)
    local l_currentWind = self:InitUIWind(uiName)
    if l_currentWind == nil then
        LogError("UIManager InitUIWind Failed UIName : "..uiName)
        return nil
    end

end

return UIManager


