using MFrameWork;
using UnityEngine;

public class MPathUtils
{
    #region AssetBundle相关路径
    public static string ASSETBUNDLE_PATH = Application.streamingAssetsPath;
    public static string ASSETBUNDLE_CONFIG_PATH = "Assets/Resources/GameData/AssetBundleData/AssetBundleConfig.asset";
    public static string ASSETBUNDLE_XML_PATH = Application.dataPath + "/Resources/GameData/AssetBundleData/MAssetBundleConfig.xml";
    public static string ASSETBUNDLE_BYTES_PATH = Application.dataPath + "/Resources/GameData/AssetBundleData/MAssetBundleConfig.bytes";
    public static string ASSETBUNDLE_AB_DATA_NAME = "assetbundledata";
    public static string ASSETBUNDLE_AB_BYTES_NAME = "MAssetBundleConfig.bytes";
    #endregion

    #region Object对象池相关
    public const string RECYCLE_POOL_TRANSFORM = "RecycleObjectPool"; //对象池回收节点
    public const string DEFAULT_OBJECT_TRANSFORM = "DefaultObjectTrans"; //默认创建节点
    #endregion

    #region UI相关的路径
    public const string UI_MAINPATH = "Assets/Resources/UI/Prefabs";
    public const string UI_ROOTPATH = "Assets/Resources/UI/Prefabs/UIRoot.prefab";
    public const string UI_PREFAB_SUFFIX  = ".prefab";
    #endregion
}