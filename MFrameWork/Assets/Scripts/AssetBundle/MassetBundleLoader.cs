// **************************************
//
// 文件名(MassetBundleLoader.cs):
// 功能描述("Autor Please Edit"):
// 作者(Max1993):
// 日期(2019/5/2  15:06):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MFrameWork;

public class MassetBundleLoader : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        //加载ab的
        AssetBundle abData = AssetBundle.LoadFromFile(MPathUtils.ASSETBUNDLE_PATH+"/"+MPathUtils.ASSETBUNDLE_AB_DATA_NAME);
        TextAsset textAsset = abData.LoadAsset<TextAsset>("MAssetBundleConfig.bytes");

        MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MAssetBundleConfig mAssetBundleConfig = (MAssetBundleConfig)binaryFormatter.Deserialize(memoryStream);
        memoryStream.Close();

        string prefabPath = "Assets/Resources/UI/Prefabs/TestPrefab.prefab";
        uint crc = MCrcHelper.GetCRC32(prefabPath);
        MAssetBundleBase mAssetBundleBase = null;
        for (int i = 0; i < mAssetBundleConfig.AssetBundleList.Count; i++)
        {
            if(mAssetBundleConfig.AssetBundleList[i].Crc == crc)
            {
                mAssetBundleBase = mAssetBundleConfig.AssetBundleList[i];
            }
        }

        for (int i = 0; i < mAssetBundleBase.AbDependence.Count; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + mAssetBundleBase.AbDependence[i]);
        }

        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + mAssetBundleBase.AbName);
        GameObject @object = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>(mAssetBundleBase.AssetName));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
