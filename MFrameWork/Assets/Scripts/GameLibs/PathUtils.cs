using MFrameWork;

public class MPathUtils
{
    #region 美术资源路径
    public const string ARTRES_PATH = "Assets/artres/";
    public const string CREATURE_PATH = "Assets/artres/_Creature/";
    public const string GAMERES_PATH = "Assets/artres/_GameRes/";
    public const string UIRES_PATH = "Assets/artres/_UI/";
    public const string SCENERES_PATH = "Assets/artres/_GameRes/Scenes/";
    public const string SCENERES_DEV_PATH = "Assets/artres/_GameRes/Scenes/_Scenes/";
    public const string EFFECTRES_PATH = "Assets/artres/_GameRes/Effects/";
    public const string ITEMRES_PATH = "Assets/artres/_GameRes/Item/";
    public const string PALETTE_PATH = "Assets/artres/_Creature/Palette/";
    public const string JIAO_HU_PULISH_PATH = "Assets/artres/_GameRes/PulishJiaoHu/";
    #endregion

    #region 美术发布资源路径
    public const string RESOURCES_PATH = "Assets/artres/Resources/";
    public const string SCENE_PUBLISH_PATH = "Assets/artres/Resources/Scenes/";
    public const string EFFECT_PUBLISH_PATH = "Assets/artres/Resources/Effects/";
    public const string EQUIP_PUBLISH_PATH = "Assets/artres/Resources/Equipments/";
    public const string MOUNT_PUBLISH_PATH = "Assets/artres/Resources/Equipments/Mount/";
    public const string ANIM_PUBLISH_PATH = "Assets/artres/Resources/Anims/";
    public const string HEADCOLORDATA_PATH = "Assets/artres/Resources/HeadColor/";
    public const string MATERIAL_PUBLISH_PATH = "Assets/artres/Resources/Materials/";
    public const string MODEL_PUBLISH_PATH = "Assets/artres/Resources/Prefabs/";
    public const string UI_PUBLISH_PATH = "Assets/artres/Resources/UI/";
    public const string ENVIRODATA_PATH = "Assets/artres/Resources/SceneEnviroData/";
    public const string SHADER_PATH = "Assets/artres/Resources/Shader/";
    
    #endregion

    #region 美术资源目录
    public const string CREATURE_DIRC = "Assets/artres/_Creature";
    public const string GAMERES_DIRC = "Assets/artres/_GameRes";
    public const string UIRES_DIRC = "Assets/artres/_UI";
    public const string SCENERES_DIRC = "Assets/artres/_GameRes/Scenes";
    public const string SCENERES_DEV_DIRC = "Assets/artres/_GameRes/Scenes/_Scenes";
    public const string EFFECTRES_DIRC = "Assets/artres/_GameRes/Effects";
    public const string ITEMRES_DIRC = "Assets/artres/_GameRes/Item";
    public const string PALETTE_DIRC = "Assets/artres/_Creature/Palette";
    #endregion

    #region 美术发布资源目录
    public const string RESOURCES_DIRC = "Assets/artres/Resources";
    public const string SCENE_PUBLISH_DIRC = "Assets/artres/Resources/Scenes";
    public const string EFFECT_PUBLISH_DIRC = "Assets/artres/Resources/Effects";
    public const string EQUIP_PUBLISH_DIRC = "Assets/artres/Resources/Equipments";
    public const string MOUNT_PUBLISH_DIRC = "Assets/artres/Resources/Equipments/Mount";
    public const string ANIM_PUBLISH_DIRC = "Assets/artres/Resources/Anims";
    public const string HEADCOLORDATA_DIRC = "Assets/artres/Resources/HeadColor";
    public const string MATERIAL_PUBLISH_DIRC = "Assets/artres/Resources/Materials";
    public const string MODEL_PUBLISH_DIRC = "Assets/artres/Resources/Prefabs";
    public const string UI_PUBLISH_DIRC = "Assets/artres/Resources/UI";
    public const string ENVIRODATA_DIRC = "Assets/artres/Resources/SceneEnviroData";
    public const string SHADER_DIRC = "Assets/artres/Resources/Shader";
    #endregion

    #region 场景编辑器
    public static string SCENE_EDITOR_OUT_DIRC
    {
        get
        {
            return string.Format("{0}/Assets/Resources/SceneDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string SCENE_EDITOR_AUTO_SAVE_FILE
    {
        get
        {
            return "Assets/artres/Editor/SceneEditor/AutoSceneEditor.bytes";
        }
    }

    public static string SCENE_EDITOR_TMP_SCENE
    {
        get
        {
            return "Assets/artres/_Editor/Scene/SceneEditor.unity";
        }
    }

    public static string SCENE_EDITOR_NPC_GENERIC_DIRC
    {
        get
        {
            return "Assets/artres/_Creature/NPC/Generic";
        }
    }

    public static string SCENE_EDITOR_NPC_HUMAN_DIRC
    {
        get
        {
            return "Assets/artres/_Creature/NPC/Human";
        }
    }

    public static string SCENE_EDITOR_SCENES_UNITY_DATAS_DIRC
    {
        get
        {
            return string.Format("{0}/Assets/Resources/ScenesUnityDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    #endregion

    #region NPC离线数据
    public static string NPC_INFO_SAVE_FILE
    {
        get
        {
            return string.Format("{0}/Assets/Resources/PathDatas/NpcInfo.bytes", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 生活技能收集物离线数据
    
    public static string GATHER_POS_SAVE_FILE
    {
        get
        {
            return string.Format("{0}/Assets/Resources/PathDatas/GatherPosInfo.bytes", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 生活技能交互位置离线数据
    
    public static string LIFE_PROFESSION_INFO_SAVE_FILE
    {
        get
        {
            return string.Format("{0}/Assets/Resources/PathDatas/LifeProfessionInfo.bytes", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 地面导航
    public static string NAV_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/NavDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 空中导航
    public static string FLY_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/SceneGrid", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string AIR_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/AirSpaceDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 地形属性
    public static string TRN_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/TerrainDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region ProtoBuf转换工具
    public static string PB_TOOL_DIRT
    {
        get
        {
            return string.Format("{0}/AutoGen/protobuf/local", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string PB_TOOL_DLL_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Plugins/GameLibs", MSysEnvHelper.GetEnvParam("MoonClientProjPath"));
        }
    }

    public static string PB_TOOL_XML_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/AutoGen/local/data", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string PB_TOOL_XML_ENUM_DIRT
    {
        get
        {
            return string.Format("{0}/AutoGen/local/enum", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    #endregion

    #region 技能编辑器
    public static string SKILL_EDITOR_DATA_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/SkillData", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
    #endregion

    #region 剧情编辑器
    public static string STORY_EDITOR_OUT_BYTES_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/Resources/CutSceneDatas", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string STORY_EDITOR_OUT_XML_DIRT
    {
        get
        {
            return string.Format("{0}/Assets/artres/Resources/CutSceneDatas", MSysEnvHelper.GetEnvParam("MoonClientProjPath"));
        }
    }
    #endregion

    public static string SERVER_CURVE_OUT_PATH
    {
        get
        {
            return string.Format("{0}/Assets/Editor/EditorResources/Server/Curve", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }

    public static string ANIM_INFO_PATH
    {
        get
        {
            return string.Format("{0}/Assets/Resources/AnimInfo", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"));
        }
    }
}
