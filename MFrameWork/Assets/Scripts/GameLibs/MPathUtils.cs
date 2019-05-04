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
}