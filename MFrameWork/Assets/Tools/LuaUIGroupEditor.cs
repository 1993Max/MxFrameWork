using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using MFrameWork;

[CustomEditor(typeof(MLuaUIGroup))]
public class LuaUIGroupEditor : Editor
{
    public string LuaTemplatePath => Path.Combine(LuaConst.luaDir, @"UI\Template\" + group.ClassName + ".lua");

    MLuaUIGroup _group;
    MLuaUIGroup group
    {
        get
        {
            if (_group==null)
            {
                _group= target as MLuaUIGroup;
            }
            return _group;
        }
    }
    
    public override void OnInspectorGUI()
    {
        if (target == null || EditorApplication.isPlaying)
            return;

        base.OnInspectorGUI();

        if (string.IsNullOrEmpty(group.Name))
        {
            group.Name = group.gameObject.name;
        }
        if (string.IsNullOrEmpty(group.ClassName))
        {
            group.ClassName = group.gameObject.name;
        }

        MLuaUIPanel panel= MFindHelper.GetParentComponent<MLuaUIPanel>(group.transform);
        MLuaUIGroup parentGroup = MFindHelper.GetParentComponent<MLuaUIGroup>(group.transform);

        if (GUILayout.Button("标准化", GUILayout.Height(20)))
        {
            MLuaUIGroup[] cacheGroups = null;
            MLuaUICom[] cacheComponents = null;
            FillComponents(group.gameObject, ref cacheGroups, ref cacheComponents);
        }

        if (GUILayout.Button("生成Lua模版代码", GUILayout.Height(20)))
        {
            CreateScript();
        }
    }

    //在Panel里点击生成Panel代码时生成相应的Template代码
    public static void CreateTemplateWithPanel(MLuaUIGroup[] groups)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            CreateTemplateWithPanel(groups[i].Groups);
            if (groups[i].IsCreateTemplateWithCreatePanel)
            {
                var groupEditor = (LuaUIGroupEditor)CreateEditor(groups[i]);
                groupEditor.CreateScript();
            }
        }
    }

    public void CreateScript()
    {
        LuaUIPanelEditor.TryGenScript(LuaTemplatePath, GetLuaUITemplateScript);
    }

    //生成Template的lua代码
    private void GetLuaUITemplateScript(LuaDocumentNode document)
    {
        document.ModelNode = new LuaModelNode("UITemplate");
        document.AddRequire(new LuaRequireNode("UI/BaseUITemplate"));
        document.ClassName = group.ClassName;
        document.ClassInitStatement = new LuaScriptStatementNode($"class(\"{group.ClassName}\", super)");
        document.AddField(new LuaFieldNode("super", LuaMemberType.Local, new LuaScriptStatementNode("UITemplate.BaseUITemplate")));

        document.RemoveFunction("ParameterDeclarations");

        var SetDataFunc = new LuaFunctionNode("SetData", LuaMemberType.Local,new List<string>() { "data" });
        var InitFunc = new LuaFunctionNode("Init", LuaMemberType.Local);
        var OnDestroyFunc = new LuaFunctionNode("OnDestroy", LuaMemberType.Local);
        var OnDeActiveFunc = new LuaFunctionNode("OnDeActive", LuaMemberType.Local);

        var ParameterDeclarationsFunction = new LuaFunctionNode("ParameterDeclarations", LuaMemberType.Local, null,new List<LuaBaseStatementNode>());
       
        ParameterDeclarationsFunction.statementNodes.Add(new LuaScriptStatementNode("self.Parameter.LuaUIGroup = self:transform():GetComponent(\"MLuaUIGroup\")"));

        var groupCodes = GetGroupCodes(group, "self.Parameter.", " = self.Parameter.LuaUIGroup.");

        foreach (var item in groupCodes)
        {
            var getComRefStatement = new LuaScriptStatementNode(item.Key + item.Value);
            ParameterDeclarationsFunction.statementNodes.Add(getComRefStatement);
        }

        document.AddFunction(InitFunc);
        document.AddFunction(OnDestroyFunc);
        document.AddFunction(OnDeActiveFunc);
        document.AddFunction(SetDataFunc);
        document.AddFunction(ParameterDeclarationsFunction);

        MFileEx.SaveText(document.ToString(), LuaTemplatePath);
    }

    //得到MLuaUIGroup相应的代码
    //=前面的是keyHead，后面的是valueHead（包含=）
    public static Dictionary<string, string> GetGroupsCodes(IList<MLuaUIGroup> groups,string keyHead,string valueHead)
    {
        Dictionary<string, string> codes = new Dictionary<string, string>();

        for (int i = 0; i < groups.Count; i++)
        {
            string currentKeyHead = keyHead + groups[i].Name;
            string currentValueHead = valueHead + $"Groups[{i}]";
            //当需要在上级生成Group代码
            if (groups[i].IsGenerateCodeInUpper)
            {
                codes[currentKeyHead] = " = {}";
                codes[currentKeyHead + ".LuaUIGroup"] = currentValueHead;

                var groupCodes = GetGroupCodes(groups[i], currentKeyHead + ".", currentValueHead + ".");
                MDictionaryHelper.AddRange(codes, groupCodes);
            }
            else
            {
                //当不需要在上级生成Group代码时只生成一句调用组件的代码
                codes[currentKeyHead] = currentValueHead;
            }
        }

        return codes;
    }

    //得到当前Group的Code
    public static Dictionary<string, string> GetGroupCodes(MLuaUIGroup group, string keyHead, string valueHead)
    {
        Dictionary<string, string> codes = new Dictionary<string, string>();

        Dictionary<string, string> comRefsCodes =GetComRefsCodes(group.ComRefs);

        foreach (var item in comRefsCodes)
        {
            string key = keyHead+ item.Key;
            string value=null;
            if (!string.IsNullOrEmpty(item.Value))
            {
                value = valueHead + item.Value;
            }
            codes[key] = value;
        }

        //当前Group中还有Groups时，得到Group中的Groups的Code
        if (group.Groups.Length>0)
        {
            var groupsCodes = GetGroupsCodes(group.Groups, keyHead, valueHead);
            MDictionaryHelper.AddRange(codes, groupsCodes);
        }
        return codes;
    }

    //得到MLuaUICom相应的代码
    public static Dictionary<string, string> GetComRefsCodes(MLuaUICom[] comRefs)
    {
        Dictionary<string, string> codes = new Dictionary<string, string>();
        Dictionary<string, int> comCountNumber = new Dictionary<string, int>();

        for (int i = 0; i < comRefs.Length; i++)
        {
            var comRef = comRefs[i];
            string key;
            string value;
            StringBuilder builder = new StringBuilder();
            if (comRef.IsArray)
            {
                if (!comCountNumber.ContainsKey(comRef.Name))
                {
                    string keyInitial = comRef.Name + " = {}";
                    comCountNumber.Add(comRef.Name, 0);
                    codes.Add(keyInitial, null);
                }
                comCountNumber[comRef.Name]++;
                key = $"{comRef.Name}[{comCountNumber[comRef.Name]}]";

            }
            else
            {
                key = comRef.Name;
            }
            value = $"ComRefs[{i}]";
            if (codes.ContainsKey(key))
            {
                Debug.LogError("已经存在Key：" + key + " gameobject name is " + comRef.gameObject.name);
                continue;
            }
            codes.Add(key, value);
        }

        return codes;
    }

    //填充Group和Component数据
    public static void FillComponents(GameObject go,ref MLuaUIGroup[] groups,ref MLuaUICom[] comRefs)
    {
        var data = GetGroupsAndComs(go);

        groups = data.Key;
        comRefs = data.Value;
        NameRepetitionDetection(data.Value,data.Key);
    }

    //得到此物体可包含的MLuaUIGroup和MLuaUICom
    public static KeyValuePair<MLuaUIGroup[], MLuaUICom[]> GetGroupsAndComs(GameObject go)
    {
        MLuaUIGroup[] h = go.GetComponentsInChildren<MLuaUIGroup>(true);
        List<MLuaUIGroup> groups = new List<MLuaUIGroup>(h);

        Dictionary<MLuaUIGroup, List<MLuaUICom>> comInGroup = new Dictionary<MLuaUIGroup, List<MLuaUICom>>();
        Dictionary<MLuaUIGroup, List<MLuaUIGroup>> groupInGroup = new Dictionary<MLuaUIGroup, List<MLuaUIGroup>>();
        List<MLuaUIGroup> panelGroup = new List<MLuaUIGroup>();
        for (int i = 0; i < groups.Count; i++)
        {
            comInGroup[groups[i]] = new List<MLuaUICom>();
            groupInGroup[groups[i]] = new List<MLuaUIGroup>();
        }
        for (int i = 0; i < groups.Count; i++)
        {
            MLuaUIGroup parentGroup = MFindHelper.GetParentComponent<MLuaUIGroup>(groups[i].transform, go.transform);
            //如果Group父物体有Group则放入父物体Group中
            if (parentGroup != null && groupInGroup.ContainsKey(parentGroup))
            {
                groupInGroup[parentGroup].Add(groups[i]);
            }
            else
            {
                //没有则放入Panel的Group中
                panelGroup.Add(groups[i]);
            }
        }

        foreach (var item in groupInGroup)
        {
            item.Key.Groups = item.Value.ToArray();
        }

        MLuaUICom[] components = go.GetComponentsInChildren<MLuaUICom>(true);
        List<MLuaUICom> comList = new List<MLuaUICom>();
        for (int i = 0; i < components.Length; i++)
        {
            components[i].UIPanel = go.GetComponent<MLuaUIPanel>();
            MLuaUIGroup parentGroup = components[i].GetComponent<MLuaUIGroup>();
            if (parentGroup == null)
            {
                parentGroup = MFindHelper.GetParentComponent<MLuaUIGroup>(components[i].transform, go.transform);
            }
            //如果Component父物体有Group则放入父物体Group中
            if (parentGroup != null && comInGroup.ContainsKey(parentGroup))
            {
                comInGroup[parentGroup].Add(components[i]);
            }
            else
            {
                //没有则放入Panel的ComRefs中
                comList.Add(components[i]);
            }
        }
        foreach (var item in comInGroup)
        {
            item.Key.ComRefs = item.Value.ToArray();
        }

        MLuaUICom[] coms = comList.ToArray();
        //如果自己身上有MLuaUIGroup，做下处理
        //MLuaUIGroup selfGroup = go.GetComponent<MLuaUIGroup>();
        //if (selfGroup != null)
        //{
        //    panelGroup.Remove(selfGroup);
        //    coms = selfGroup.ComRefs;
        //}

        KeyValuePair<MLuaUIGroup[], MLuaUICom[]> data = new KeyValuePair<MLuaUIGroup[], MLuaUICom[]>(panelGroup.ToArray(), coms);

        return data;
    }

    //对组件的名字进行检测
    //当名字没有相同的时候通过检测
    public static void NameRepetitionDetection(MLuaUICom[] coms,MLuaUIGroup[] groups)
    {
        Dictionary<string, List<int>> cacheName = new Dictionary<string, List<int>>();
        for (int i = 0; i < coms.Length; i++)
        {
            MLuaUICom com = coms[i];
            if (com.IsArray)
            {
                if (!cacheName.ContainsKey(com.Name))
                {
                    cacheName[com.Name] = new List<int>();
                    cacheName[com.Name].Add(i);
                }
            }
            else
            {
                if (!cacheName.ContainsKey(com.Name))
                {
                    cacheName[com.Name] = new List<int>();
                }
                cacheName[com.Name].Add(i);
            }
        }
        for (int i = 0; i < groups.Length; i++)
        {
            MLuaUIGroup group = groups[i];
            if (!cacheName.ContainsKey(group.Name))
            {
                cacheName[group.Name] = new List<int>();
            }
            cacheName[group.Name].Add(i+20000);
            NameRepetitionDetection(group.ComRefs, group.Groups);
        }

        bool isPassDetection = true;
        StringBuilder builder = new StringBuilder();

        foreach (var item in cacheName)
        {
            if (item.Value.Count>1)
            {
                builder.Append(item.Key);
                builder.Append(":\n");
                for (int i = 0; i < item.Value.Count; i++)
                {
                    int index = item.Value[i];
                    if (index >= 20000)
                    {
                        builder.Append("  Groups:");
                        index -= 20000;
                    }
                    else
                    {
                        builder.Append("  ComRefs:");
                    }
                    builder.Append(index);
                    builder.Append("\n");
                }
                isPassDetection = false;
            }
        }

        if (!isPassDetection)
        {
            EditorUtility.DisplayDialog("有名字重复的组件，请解决", builder.ToString(), "确定");
        }
        
    }
    
}
