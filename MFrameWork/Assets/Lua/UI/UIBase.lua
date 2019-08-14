-- @FileName UIBase
-- @Create by mx
-- @Create time 2019/05/24 22:08:38
-- @FileInfo 所有UI的基类

module("UI",package.seeall)
--全局申明UIBase基类
UIBase = Class("UIBase")
--全局申明 UI资源加载方式
UILoadType = 
{
    Sync,  --同步
    ASync, --异步
}

function UIBase:Ctor( ... )
    --该UI是否已经初始化完成
    self.m_isInited = false
    --实例化的Prefab
    self.m_uiObj = nil
    --实例化的TransForm信息
    self.m_uiTrans = nil
    --是否处于激活状态
    self.m_isActive = false
    --UIName
    self.m_uiName = nil
    --UIPath
    self.m_uiFullPath = nil
    --是否在加载中
    self.m_isLoading = false
    --资源加载方式 默认同步
    self.m_resLoadType = UILoadType.Sync
    --存储UI的数据信息
    self.m_uiPanel = nil
end

------------------生命周期 Start-------------------

--资源加载
function UIBase:Load( callBack )
    if self.m_isLoading then return end
    if self.m_uiObj then LogError("Already Loaded UIName :"..self.m_uiName) return end
    if self.m_uiFullPath == nil then self:SetUIFullPath() end
    local l_uiObj = nil
    if self.m_resLoadType == UILoadType.Sync then
        l_uiObj = MObjectManager:InstantiateGameObeject(self.m_uiFullPath)
        self:OnLoadFinish(l_uiObj,callBack)
    else
        l_uiObj = MObjectManager:InstantiateGameObejectAsync(self.m_uiFullPath,function (resPath,mResObjItem,parms)
            self:OnLoadFinish(l_uiObj,callBack)
        end,LoadResPriority.RES_LOAD_LEVEL_HEIGHT)
    end
end

--资源加载完成 初始化信息的设置
function UIBase:OnLoadFinish(obj,callBack)
    if obj == nil then 
        LogError("资源加载失败~ Name："..self.m_uiName) 
        self.m_uiObj = nil
        return 
    end
    obj.name = self.m_uiName
    self.m_uiObj = obj
    self.m_uiTrans = obj.transform
    self:Init()
    if callBack then callBack(self) end
end

--资源释放
function UIBase:UnLoad( ... )
    --Todo 异步加载对正在加载的异步逻辑做取消判定 
    --资源加载缺少这个接口 需要补上
    if self.m_isLoading then
        
    end

    if self.m_uiObj then
        MObjectManager:ReleaseObject(self.m_uiObj)
        self.m_uiObj = null
        self.m_uiTrans = null
    end
end

--逻辑初始化 在资源加载完成后调用
function UIBase:Init()
    self.m_isInited = true
end

--资源卸载后调用
function UIBase:Uninit()
    self:UnLoad()
    self.m_isInited = false
    self.m_uiPanel = false
end

--打开UI
function UIBase:Active()
end

--关闭UI
function UIBase:Deactive()
end

--UI显示时
function UIBase:OnActive()
end

--UI关闭前
function UIBase:OnDeactive()
end

--事件绑定
function UIBase:BindEvents()
end

--事件解绑
function UIBase:UnBindEvents()
end

--Update事件
function UIBase:Update()
end

--触摸事件
function UIBase:UpdateInPut(touchItem)
end

--断线重连
function UIBase:OnReconnected()
end

--游戏注销
function UIBase:OnLogout()
end

------------------生命周期 End--------------------

function UIBase:IsActive( ... )
    return self.m_isActive
end

function UIBase:IsInited( ... )
    return self.m_isInited
end

--设置UI的全路径
function UIBase:SetUIFullPath( ... )
    if self.m_uiFullPath ~= nil then return end
    if self.m_uiName == nil then
        LogError("UIName is nil ~ Please Check it")
    end
    self.m_uiFullPath = MPathUtils.UI_MAINPATH.."/"..self.m_uiName..MPathUtils.UI_PREFAB_SUFFIX
end

UIBase.m_uiName = "LoginPanel"
UIBase.Load(UIBase)

return UIBase