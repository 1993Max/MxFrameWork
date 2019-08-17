using UnityEngine;

//功能配置
public static class Configure
{        
    //基础组件路径
    public static string PrefabWinBasicComPath = Application.dataPath + "/ToolsReleate/PrefabPreview/BasicComponent";
    //基础面板路径
    public static string PrefabWinCommonPanelPath = Application.dataPath + "/ToolsReleate/PrefabPreview/CommonPanel";
    //预览图存储全路径
    public static string ResAssetsPath = Application.dataPath + "/ToolsReleate/PrefabPreview/PrefabViewPath";
    //常用颜色配置
    public static string ColorDataPath = Application.dataPath + "/ToolsReleate/PrefabPreview/ConfigPath/UIColor.txt";
    //命名基本组件配置
    public static string UIWidgetPath = Application.dataPath + "/ToolsReleate/PrefabPreview/ConfigPath/UIWidgetName.txt";
    //命名信息面板基本配置
    public static string UIInfoPath = Application.dataPath + "/ToolsReleate/PrefabPreview/ConfigPath/UIInfoName.txt";
    //命名基本状态文本配置
    public static string UIStatePath = Application.dataPath + "/ToolsReleate/PrefabPreview/ConfigPath/UIStateName.txt";
    //效果图路径
    public static string UIEffectSpritePath = "Assets/ToolsReleate/PrefabPreview/TexPreview";

    static string projectUUID = string.Empty;
    public static string ProjectUUID
    {
        get
        {
#if UNITY_EDITOR
            if (projectUUID == string.Empty)
            {
                projectUUID = UIEditorHelper.GenMD5String(Application.dataPath);
            }
#endif
            return projectUUID;
        }
    }
}
