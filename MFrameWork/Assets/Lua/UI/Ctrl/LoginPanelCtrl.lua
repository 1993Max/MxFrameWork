--this file is gen by script
--you can edit this file in custom part


--lua requires
require "UI/UIBaseWind"
require "UI/Panel/LoginPanelPanel"
--lua requires end

--lua model
module("UI", package.seeall)
--lua model end

--lua fields
local super = UI.UIBase
--next--
--lua fields end

--lua Class define
LoginPanelCtrl = Class("LoginPanelCtrl", super)
--lua Class define end

--lua functions
function LoginPanelCtrl:Ctor()
	
	super.Ctor(self, CtrlNames.LoginPanel, UILayer.Upper, nil, ActiveType.Exclusive)
	
end --func end
--next--
function LoginPanelCtrl:Init()
	
	self.panel = UI.LoginPanelPanel.Bind(self)
	
end --func end
--next--
function LoginPanelCtrl:Uninit()
	
	super.Uninit(self)
	self.panel = nil
	
end --func end
--next--
function LoginPanelCtrl:OnActive()
	
	
end --func end
--next--
function LoginPanelCtrl:OnDeActive()
	
	
end --func end
--next--
function LoginPanelCtrl:Update()
	
	
end --func end
--next--
function LoginPanelCtrl:Refresh()
	
	
end --func end
--next--
function LoginPanelCtrl:OnLogout()
	
	
end --func end
--next--
function LoginPanelCtrl:OnReconnected(roleData)
	
	
end --func end
--next--
function LoginPanelCtrl:Show()
	
	super.Show(self)
	
end --func end
--next--
function LoginPanelCtrl:Hide()
	
	super.Hide(self)
	
end --func end
--next--
function LoginPanelCtrl:BindEvents()
	
	
end --func end
--next--
function LoginPanelCtrl:UnBindEvents()
	
	
end --func end
--next--
--lua functions end

--lua custom scripts

--lua custom scripts end
