-- @FileName Main
-- @Create by mx
-- @Create time 2019/05/25 22:08:38
-- @FileInfo 所有UI的基类

--全局申明UIBase基类
DeclareGlobal("UIBase",class("UIBase"))

function UIBase:ctor( ... )
    --该UI是否已经初始化完成
    self.m_isInited = false
    --实例化的Prefab
    self.m_uiObj = nil
    --是否处于激活状态
    self.m_isActive = false
    --UIName
    self.m_uiName = nil
    --UIPath
    self.m_uiFullPath = nil
    --是否在加载中
    self.m_isLoading = false
end

function UIBase:IsActive( ... )
    return self.isActive
end

function UIBase:IsInited( ... )
    return self.IsInited
end

function UIBase:Load( callBack )
    if self.m_isLoading then return end
    if self.m_uiObj then LogError("Already Loaded UIName :"..self.m_uiName) return end
    if self.m_uiFullPath == nil then self:SetUIFullPath() end
    self.m_uiObj = MObjectManager:InstantiateGameObeject(self.m_uiFullPath)
end

function UIBase:SetUIFullPath( ... )
    if self.m_uiFullPath ~= nil then return end
    if self.m_uiName == nil then
        LogError("UIName is nil ~ Please Check it")
    end
    self.m_uiFullPath = MPathUtils.UI_MAINPATH.."/"..self.m_uiName..MPathUtils.UI_PREFAB_SUFFIX
end

UIBase.m_uiName = "LoginPanel"
UIBase.Load(UIBase)
