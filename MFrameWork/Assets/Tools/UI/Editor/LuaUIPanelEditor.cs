using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using MFrameWork;

public enum UILayer
{
    Normal = 0, 
    Upper = 1,
    Top = 2
}

public enum ActiveType
{
    Normal = 0,
    Exclusive = 1,
    Standalone = 2
}

[CustomEditor(typeof(MLuaUIPanel), true)]
public class LuaUIPanelEditor : Editor
{
    public GameObject gameObject => (target as MLuaUIPanel)?.gameObject;
    public MLuaUIPanel uiPanel => target as MLuaUIPanel;
    public string LuaPanelPath => Path.Combine(LuaConst.luaDir, @"UI\Panel\" + gameObject.name + "Panel.lua");
    public string LuaCtrlPath => Path.Combine(LuaConst.luaDir, @"UI\Ctrl\" + gameObject.name + "Ctrl.lua");
    public string LuaHandlerPath => Path.Combine(LuaConst.luaDir, @"UI\Handler\" + gameObject.name + "Handler.lua");

    private UILayer uiLayer = UILayer.Upper;
    private ActiveType activeType = ActiveType.Exclusive;

    public override void OnInspectorGUI()
    {
        if(target == null || EditorApplication.isPlaying)
            return;

        base.OnInspectorGUI();
        ShowPanelBaseInfo();
        Standard();
        ShowCreateScriptBtn();
    }

    #region 普通寻找工具
    private void ShowPanelBaseInfo()
    {
        GUILayout.Label("UI层级");
        uiLayer = (UILayer)EditorGUILayout.EnumPopup(uiLayer);
        GUILayout.Label("显示方式");
        activeType = (ActiveType)EditorGUILayout.EnumPopup(activeType);
    } 

    private void Standard()
    {
        if (GUILayout.Button("一键标准化", GUILayout.Height(20)))
        {
            FindAllCom();
            SetRaycastTarget();
        }
    }
    #endregion

    #region 代码生成工具

    private void ShowCreateScriptBtn()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建LuaPanel代码", GUILayout.Height(20)))
        {
            TryGenScript(LuaPanelPath, GetLuaPanelScript);
            UpdateConst();

            LuaUIGroupEditor.CreateTemplateWithPanel(uiPanel.Groups);
        }
        if (GUILayout.Button("创建LuaMgr代码", GUILayout.Height(20)))
        {
            //GetLuaModelScript();
            //UIModelManagerWindow.NeedRefreshLuaDocument = true;
            // UpdateConst();
        }
        if (!uiPanel.IsHandler)
        {
            if (GUILayout.Button("创建LuaCtrl代码", GUILayout.Height(20)))
            {
                TryGenScript(LuaCtrlPath, GetLuaCtrlScript);
                UpdateConst();
            }
        }
        else
        {
            if (GUILayout.Button("创建LuaHandler代码", GUILayout.Height(20)))
            {
                TryGenScript(LuaHandlerPath, GetLuaHandlerScript);
                UpdateConst();
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("创建所有Lua代码", GUILayout.Height(20)))
        {
            TryGenScript(LuaPanelPath, GetLuaPanelScript);
            GetLuaModelScript();
            if (uiPanel.IsHandler)
            {
                TryGenScript(LuaHandlerPath, GetLuaHandlerScript);
            }
            else
            {
                TryGenScript(LuaCtrlPath, GetLuaCtrlScript);
            }
            UpdateConst();
        }
    }

    public static void TryGenScript(string path, Action<LuaDocumentNode> genAction, bool quiet = false)
    {
        LuaDocumentNode luaDocument = new LuaDocumentNode();
        try
        {
            if (File.Exists(path) && !quiet)
            {
                if (!EditorUtility.DisplayDialog("文件已存在", path+"\n文档已存在，要重新生成吗？", "ok", "不要"))
                {
                    return;
                }
            }
            luaDocument.Load(path);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (quiet || !EditorUtility.DisplayDialog("解析错误", "文档解析错误，要重新生成吗？", "ok", "不要"))
            {
                return;
            }
        }
        genAction(luaDocument);
        AssetDatabase.Refresh();
    }

    
    /// <summary>
    /// 生成LuaPanel
    /// </summary>
    /// <param name="document"></param>
    private void GetLuaPanelScript(LuaDocumentNode document)
    {
        var panelName = $"{gameObject.name}Panel";
        var luaUIPanel = gameObject.GetComponent<MLuaUIPanel>();
        if (luaUIPanel == null)
        {
            EditorUtility.DisplayDialog("导出错误", "目标找不到LuaUIPanel", "ok");
            return;
        }

        document.ModelNode = new LuaModelNode($"UI.{panelName}");
        document.RemoveFunction("Bind");

        //Bind函数
        var bindFunction = new LuaFunctionNode("Bind", LuaMemberType.Global, new List<string>() {"ctrl"},
            new List<LuaBaseStatementNode>());
        document.AddFunction(bindFunction);

        bindFunction.statementNodes.Add(new LuaScriptStatementNode("--dont override this function"));
        bindFunction.statementNodes.Add(new LuaScriptStatementNode("local l_panel = {}"));
        bindFunction.statementNodes.Add(new LuaScriptStatementNode("l_panel.PanelRef = ctrl.uObj:GetComponent(\"MLuaUIPanel\")"));
        
        Dictionary<string, string> comRefsCodes = LuaUIGroupEditor.GetComRefsCodes(luaUIPanel.ComRefs);
        foreach (var item in comRefsCodes)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("l_panel.");
            builder.Append(item.Key);
            if (!string.IsNullOrEmpty(item.Value))
            {
                builder.Append(" = l_panel.PanelRef.");
                builder.Append(item.Value);
            }
            var getComRefStatement = new LuaScriptStatementNode(builder.ToString());
            bindFunction.statementNodes.Add(getComRefStatement);
        }
        
        var groupsCodes = LuaUIGroupEditor.GetGroupsCodes(luaUIPanel.Groups, "l_panel.", " = l_panel.PanelRef.");

        foreach (var item in groupsCodes)
        {
            var getComRefStatement = new LuaScriptStatementNode(item.Key+ item.Value);
            bindFunction.statementNodes.Add(getComRefStatement);
        }

        //返回代码
        bindFunction.statementNodes.Add(new LuaScriptStatementNode("return l_panel"));

        MFileEx.SaveText(document.ToString(), LuaPanelPath);
    }

    

    /// <summary>
    /// 生成LuaModel
    /// </summary>
    private void GetLuaModelScript()
    {
        var luaUIPanel = gameObject.GetComponent<MLuaUIPanel>();
        if (luaUIPanel == null)
        {
            EditorUtility.DisplayDialog("导出错误", "目标找不到LuaUIPanel", "欧克");
            return;
        }

        for (int i = 0; i < luaUIPanel.ComRefs.Length; i++)
        {
            var comRef = luaUIPanel.ComRefs[i];
            if (comRef.BindCom == null || string.IsNullOrEmpty(comRef.DataName))
                continue;
            var modelName = comRef.ModelName ?? $"{gameObject.name}Model";
            var modelPath = Path.Combine(LuaConst.luaDir, $"Data\\Model\\{modelName}.lua");
            LuaDocumentNode luaDocument = new LuaDocumentNode();
            GetLuaModelScript(comRef, luaDocument, modelPath);
        }
    }

    private void GetLuaModelScript(MLuaUICom comRef, LuaDocumentNode document, string savePath)
    {
        document.Load(savePath);
        var modelName = comRef.ModelName ?? $"{gameObject.name}Model";

        document.AddRequire(new LuaRequireNode("Data/BaseModel"));
        document.AddRequire(new LuaRequireNode("Event/EventDispacher"));
        document.ModelNode = new LuaModelNode("Data");
        document.ClassName = modelName;
        document.ClassInitStatement = new LuaScriptStatementNode($"class(\"{modelName}\", super)");
        document.AddField(new LuaFieldNode("super", LuaMemberType.Local, new LuaScriptStatementNode("Data.BaseModel")));

        var eventNameFieldName = $"{modelName}.{comRef.DataName.ToUpper()}";

        var property = new LuaPropertyNode(comRef.DataName, LuaMemberType.Local);
        property.setStatementNodes.Add(new LuaScriptStatementNode($"if self._{comRef.DataName} == {comRef.DataName} then return end"));
        property.setStatementNodes.Add(new LuaScriptStatementNode(property.defualtSetStatement));
        property.setStatementNodes.Add(new LuaScriptStatementNode($"{eventNameFieldName}:Dispatch(Data.onDataChange, {comRef.DataName})"));
        document.AddProperty(property);


        var eventNameField = new LuaFieldNode(eventNameFieldName, LuaMemberType.Global, new LuaScriptStatementNode($"EventDispatcher.new()"));
        document.AddField(eventNameField);

        document.AddFunction(new LuaFunctionNode("ctor", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode($"super.ctor(self, CtrlNames.{gameObject.name})")
        }));

        MFileEx.SaveText(document.ToString(), savePath);
    }
    /// <summary>
    /// 生成LuaHandler
    /// </summary>
    /// <param name="document"></param>
    private void GetLuaHandlerScript(LuaDocumentNode document)
    {
        var panelName = $"{gameObject.name}Panel";
        var handlerName = $"{gameObject.name}Handler";
        var defultModelName = $"{gameObject.name}Model";

        var luaUIPanel = gameObject.GetComponent<MLuaUIPanel>();
        if (luaUIPanel == null)
        {
            EditorUtility.DisplayDialog("导出错误", "目标找不到LuaUIPanel", "欧克");
            return;
        }

        //document.RemoveFunction("BindEvents");
        //document.RemoveFunction("UnBindEvents");

        document.AddRequire(new LuaRequireNode("UI/UIBaseHandler"));
        document.AddRequire(new LuaRequireNode($"UI/Panel/{panelName}"));

        document.ModelNode = new LuaModelNode("UI");

        document.ClassName = handlerName;
        document.ClassInitStatement = new LuaScriptStatementNode($"class(\"{handlerName}\", super)");
        document.AddField(new LuaFieldNode("super", LuaMemberType.Local, new LuaScriptStatementNode("UI.UIBaseHandler")));

        var ctorFunc = new LuaFunctionNode("ctor", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode($"super.ctor(self, HandlerNames.{gameObject.name}, 0)")
        });

        var initFunc = new LuaFunctionNode("Init", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        initFunc.statementNodes.Add(new LuaScriptStatementNode($"self.panel = UI.{panelName}.Bind(self)"));

        var unInitFunc = new LuaFunctionNode("Uninit", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        unInitFunc.statementNodes.Add(new LuaScriptStatementNode("super.Uninit(self)"));
        unInitFunc.statementNodes.Add(new LuaScriptStatementNode("self.panel = nil"));

        var onActiveFunc = new LuaFunctionNode("OnActive", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var onDeActiveFunc = new LuaFunctionNode("OnDeActive", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var updateFunc = new LuaFunctionNode("Update", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var refreshFunc = new LuaFunctionNode("Refresh", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var logoutFunc = new LuaFunctionNode("OnLogout", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var showFunc = new LuaFunctionNode("Show", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode("super.Show(self)")
        });

        var hideFunc = new LuaFunctionNode("Hide", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode("super.Hide(self)")
        });

        var bindFunc = new LuaFunctionNode("BindEvents", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        //bindFunc.statementNodes.Add(new LuaScriptStatementNode("--dont override this function"));
        var unbindFunc = new LuaFunctionNode("UnBindEvents", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        //unbindFunc.statementNodes.Add(new LuaScriptStatementNode("--dont override this function"));

        Dictionary<string, int> comCountNumber = new Dictionary<string, int>();
        List<string> unbindEvent = new List<string>();
        for (int i = 0; i < luaUIPanel.ComRefs.Length; i++)
        {
            var comRef = luaUIPanel.ComRefs[i];
            if (comRef.BindCom == null || string.IsNullOrEmpty(comRef.DataName) || !comRef.GenBindScript)
                continue;
            var modelName = comRef.ModelName ?? defultModelName;

            document.AddRequire(new LuaRequireNode($"Data/Model/{modelName}"));

            var eventNameFieldName = $"{modelName}.{comRef.DataName.ToUpper()}";

            if (comRef.IsArray)
            {
                if (!comCountNumber.ContainsKey(comRef.Name))
                {
                    comCountNumber[comRef.Name] = 0;
                }
                comCountNumber[comRef.Name] = comCountNumber[comRef.Name] + 1;
                bindFunc.statementNodes.Add(new LuaScriptStatementNode(
                    $"self:Bind(Data.{eventNameFieldName}, self.panel.{comRef.Name}[{comCountNumber[comRef.Name]}])"));
            }
            else
            {
                bindFunc.statementNodes.Add(new LuaScriptStatementNode($"self:Bind(Data.{eventNameFieldName}, self.panel.{comRef.Name})"));
            }
            if (!unbindEvent.Contains(eventNameFieldName))
            {
                unbindFunc.statementNodes.Add(new LuaScriptStatementNode($"self:UnBind(Data.{eventNameFieldName})"));
                unbindEvent.Add(eventNameFieldName);
            }
        }

        document.AddFunction(ctorFunc);
        document.AddFunction(initFunc);
        document.AddFunction(unInitFunc);
        document.AddFunction(onActiveFunc);
        document.AddFunction(onDeActiveFunc);
        document.AddFunction(updateFunc);
        document.AddFunction(refreshFunc);
        document.AddFunction(logoutFunc);
        document.AddFunction(showFunc);
        document.AddFunction(hideFunc);
        document.AddFunction(bindFunc);
        document.AddFunction(unbindFunc);

        MFileEx.SaveText(document.ToString(), LuaHandlerPath);
    }
    /// <summary>
    /// 生成LuaCtrl
    /// </summary>
    /// <param name="document"></param>
    private void GetLuaCtrlScript(LuaDocumentNode document)
    {
        var panelName = $"{gameObject.name}Panel";
        var ctrlName = $"{gameObject.name}Ctrl";
        var defultModelName = $"{gameObject.name}Model";

        var luaUIPanel = gameObject.GetComponent<MLuaUIPanel>();
        if (luaUIPanel == null)
        {
            EditorUtility.DisplayDialog("导出错误", "目标找不到LuaUIPanel", "欧克");
            return;
        }

        //不再覆盖
        //document.RemoveFunction("BindEvents");
        //document.RemoveFunction("UnBindEvents");

        document.AddRequire(new LuaRequireNode("UI/UIBaseCtrl"));
        document.AddRequire(new LuaRequireNode($"UI/Panel/{panelName}"));

        document.ModelNode = new LuaModelNode("UI");

        document.ClassName = ctrlName;
        document.ClassInitStatement = new LuaScriptStatementNode($"class(\"{ctrlName}\", super)");
        document.AddField(new LuaFieldNode("super", LuaMemberType.Local, new LuaScriptStatementNode("UI.UIBaseCtrl")));

        var ctorFunc = new LuaFunctionNode("ctor", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode($"super.ctor(self, CtrlNames.{gameObject.name}, UILayer.{uiLayer.ToString()}, nil, ActiveType.{activeType.ToString()})")
        });

        var initFunc = new LuaFunctionNode("Init", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        initFunc.statementNodes.Add(new LuaScriptStatementNode($"self.panel = UI.{panelName}.Bind(self)"));

        var unInitFunc = new LuaFunctionNode("Uninit", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        unInitFunc.statementNodes.Add(new LuaScriptStatementNode("super.Uninit(self)"));
        unInitFunc.statementNodes.Add(new LuaScriptStatementNode("self.panel = nil"));

        LuaFunctionNode setupHandlerFunc = null;
        if (luaUIPanel.HandlerRef != null)
        {
            setupHandlerFunc = new LuaFunctionNode("SetupHandlers", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
            {
                new LuaScriptStatementNode("super.SetupHandlers(self)"),
                new LuaScriptStatementNode($"self:SetHandlerToggleTpl(self.panel.{luaUIPanel.HandlerRef.Name})"),
                new LuaScriptStatementNode("--self:EnsureCreateHandler(xxx, xxx)")
            });
        }

        var onActiveFunc = new LuaFunctionNode("OnActive", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var onDeActiveFunc = new LuaFunctionNode("OnDeActive", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var updateFunc = new LuaFunctionNode("Update", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var refreshFunc = new LuaFunctionNode("Refresh", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var logoutFunc = new LuaFunctionNode("OnLogout", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());

        var reconnectedFunc = new LuaFunctionNode("OnReconnected", LuaMemberType.Local, new List<string>(){"roleData"}, new List<LuaBaseStatementNode>());

        var showFunc = new LuaFunctionNode("Show", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode("super.Show(self)")
        });

        var hideFunc = new LuaFunctionNode("Hide", LuaMemberType.Local, null, new List<LuaBaseStatementNode>()
        {
            new LuaScriptStatementNode("super.Hide(self)")
        });

        var bindFunc = new LuaFunctionNode("BindEvents", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        //bindFunc.statementNodes.Add(new LuaScriptStatementNode("--dont override this function"));
        var unbindFunc = new LuaFunctionNode("UnBindEvents", LuaMemberType.Local, null, new List<LuaBaseStatementNode>());
        //unbindFunc.statementNodes.Add(new LuaScriptStatementNode("--dont override this function"));

        Dictionary<string, int> comCountNumber = new Dictionary<string, int>();
        List<string> unbindEvent = new List<string>();
        for (int i = 0; i < luaUIPanel.ComRefs.Length; i++)
        {
            var comRef = luaUIPanel.ComRefs[i];
            if(comRef.BindCom == null || string.IsNullOrEmpty(comRef.DataName) || !comRef.GenBindScript)
                continue;
            var modelName = comRef.ModelName ?? defultModelName;

            document.AddRequire(new LuaRequireNode($"Data/Model/{modelName}"));

            var eventNameFieldName = $"{modelName}.{comRef.DataName.ToUpper()}";

            if (comRef.IsArray)
            {
                if (!comCountNumber.ContainsKey(comRef.Name))
                {
                    comCountNumber[comRef.Name] = 0;
                }
                comCountNumber[comRef.Name] = comCountNumber[comRef.Name] + 1;
                bindFunc.statementNodes.Add(new LuaScriptStatementNode(
                    $"self:Bind(Data.{eventNameFieldName}, self.panel.{comRef.Name}[{comCountNumber[comRef.Name]}])"));
            }
            else
            {
                bindFunc.statementNodes.Add(new LuaScriptStatementNode($"self:Bind(Data.{eventNameFieldName}, self.panel.{comRef.Name})"));
            }
            if (!unbindEvent.Contains(eventNameFieldName))
            {
                unbindFunc.statementNodes.Add(new LuaScriptStatementNode($"self:UnBind(Data.{eventNameFieldName})"));
                unbindEvent.Add(eventNameFieldName);
            }
        }

        document.AddFunction(ctorFunc);
        document.AddFunction(initFunc);
        if (setupHandlerFunc != null)
        {
            document.AddFunction(setupHandlerFunc);
        }
        document.AddFunction(unInitFunc);
        document.AddFunction(onActiveFunc);
        document.AddFunction(onDeActiveFunc);
        document.AddFunction(updateFunc);
        document.AddFunction(refreshFunc);
        document.AddFunction(logoutFunc);
        document.AddFunction(reconnectedFunc);
        document.AddFunction(showFunc);
        document.AddFunction(hideFunc);
        document.AddFunction(bindFunc);
        document.AddFunction(unbindFunc);

        MFileEx.SaveText(document.ToString(), LuaCtrlPath);
    }

    private void UpdateConst()
    {
        var filePath = Path.Combine(LuaConst.luaDir, "UI\\UIConst.lua");

        LuaDocumentNode luaDocument = new LuaDocumentNode();
        luaDocument.Load(filePath);
        luaDocument.ModelNode = new LuaModelNode("UI");
        var ctrlPath = PathEx.GetPathParentFolder(LuaCtrlPath);
        DirectoryInfo ctrlDirectory = new DirectoryInfo(ctrlPath);

        var files = ctrlDirectory.GetFiles();
        var ctrlNames = from file in files
            where file.Name.EndsWith(".lua")
            select MFileEx.GetFileNameWithoutExtention(file.FullName).Replace("Ctrl", string.Empty);

        List<string> ctrlList = new List<string>(ctrlNames);
        StringBuilder initStatementBuilder = new StringBuilder();
        initStatementBuilder.Append("{\n");
        ctrlList.ForEach((index, str) =>
        {
            initStatementBuilder.Append($"\t{str} = \"{str}\"");
            if (index != ctrlList.Count - 1)
                initStatementBuilder.Append(",\n");
        });

        initStatementBuilder.Append("\n}");
        luaDocument.RemoveField("CtrlNames");
        luaDocument.AddField(new LuaFieldNode("CtrlNames", LuaMemberType.Global, new LuaScriptStatementNode(initStatementBuilder.ToString())));

        initStatementBuilder = new StringBuilder();
        var handlerPath = PathEx.GetPathParentFolder(LuaHandlerPath);
        DirectoryInfo handlerDirectory = new DirectoryInfo(handlerPath);

        var handlerFiles = handlerDirectory.GetFiles();
        var handlerNames = from file in handlerFiles
                           where file.Name.EndsWith(".lua")
                        select MFileEx.GetFileNameWithoutExtention(file.FullName).Replace("Handler", string.Empty);

        List<string> handlerList = new List<string>(handlerNames);
        initStatementBuilder.Append("{\n");
        handlerList.ForEach((index, str) =>
        {
            initStatementBuilder.Append($"\t{str} = \"{str}\"");
            if (index != handlerList.Count - 1)
                initStatementBuilder.Append(",\n");
        });

        initStatementBuilder.Append("\n}");
        luaDocument.RemoveField("HandlerNames");
        luaDocument.AddField(new LuaFieldNode("HandlerNames", LuaMemberType.Global, new LuaScriptStatementNode(initStatementBuilder.ToString())));

        MFileEx.SaveText(luaDocument.ToString(), filePath);
    }
    #endregion

    #region 通用工具
    /// <summary>
    /// 找到所有com
    /// </summary>
    void FindAllCom()
    {
        LuaUIGroupEditor.FillComponents(gameObject, ref uiPanel.Groups, ref uiPanel.ComRefs);
    }

    /// <summary>
    /// 合理设置RaycastTarget
    /// </summary>
    void SetRaycastTarget()
    {
        gameObject.transform.DoToAllChildren((tran) =>
        {
            var text = tran.gameObject.GetComponent<Text>();
            var img = tran.gameObject.GetComponent<Image>();
            var hasCom = tran.gameObject.GetComponent<MLuaUICom>() != null;

            if(text)
            {
                var button = tran.gameObject.GetComponent<Button>();
                if (!button)
                {
                    text.raycastTarget = false;
                }
            }
        }, true);
    }
    #endregion
}

