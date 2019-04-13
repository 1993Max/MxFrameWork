  -- @FileName MgrMgr
  -- @Create by mx
  -- @Create time 2019/02/23 22:06:15
 
module("MoudleMgr", package.seeall)
MgrMgr = class("MgrMgr")

--全局保存所有Mgr的一个Table
local l_mgrTable = {}
--全部保存Mgr的Update方法的一个Table
local l_mgrUpdateFuncTable = {}

--获取Mgr走统一的接口
function MgrMgr:GetMgr(mgrName)
    self:Regist(mgrName)
    return l_mgrTable[mgrName]
end

function MgrMgr:ctor()
    print("MgrMgr Ctor")
end

--生命周期的定义 Regist
function MgrMgr:Regist(mgrName)
    if l_mgrTable[mgrName] == nil then
        require("ModuleMgr/"..mgrName)
        l_mgrTable[mgrName] = ModuleMgr[mgrName]
        if l_mgrTable[mgrName].OnInit ~= nil then
            l_mgrTable[mgrName].OnInit()
        end
        local l_updateFunc = l_mgrTable[mgrName].OnUpdate
        if l_updateFunc ~= nil then
            l_mgrUpdateFuncTable[mgrName] = l_updateFunc
        end
    end
end

--生命周期的定义 Update
function MgrMgr:Update()
    for mgrName, mgrUpdateFunc in pairs(l_mgrUpdateFuncTable) do
        if mgrUpdateFunc ~= nil then
            mgrUpdateFunc()
        end
    end
end

--生命周期的定义 Unint
function MgrMgr:UnInit()
    for mgrName, mgr in pairs(l_mgrTable) do
        if mgr.OnUnInit ~= nil then
            mgr.OnUnInit()
        end
    end
end

--生命周期的定义 LogOut
function MgrMgr:Logout()
    for mgrName, mgr in pairs(l_mgrTable) do
        if mgr.OnLogout ~= nil then
            mgr.OnLogout()
        end
    end
end
