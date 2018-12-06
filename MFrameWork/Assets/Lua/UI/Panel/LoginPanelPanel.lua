--this file is gen by script
--you can edit this file in custom part


--lua model
module("UI.LoginPanelPanel", package.seeall)
--lua model end

--lua functions
function Bind(ctrl)
	
	--dont override this function
	local l_panel = {}
	l_panel.PanelRef = ctrl.uObj:GetComponent("MLuaUIPanel")
	l_panel.BtnLogin = l_panel.PanelRef.ComRefs[0]
	l_panel.BtnExit = l_panel.PanelRef.ComRefs[1]
	return l_panel
	
end --func end
--next--
--lua functions end

--lua custom scripts

--lua custom scripts end
