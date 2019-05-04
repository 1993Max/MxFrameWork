// **************************************
//
// 文件名(NewMonoBehaviour.cs):
// 功能描述("用于建立打包类型的管理文件"):
// 作者(Max1993):
// 日期(2019/5/1  13:35):
//
// **************************************
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "AssetBundleConfig",menuName = "MSimpleTools/CreatAbConfig")]
public class AssetBundleBuildConfig : ScriptableObject
{
    //基于所在文件下的所有单个文件进项打包 保证每个文件名字的唯一性
    public List<string> m_allFilePath = new List<string>();
    //基于文件夹打包 该文件夹下面的所有文件都会被打成一个包
    public List<AbDirInfo> m_allFileDirAb = new List<AbDirInfo>();

    [System.Serializable]
    public struct AbDirInfo
    {
        //存储一个路径 该路径下面的所有文件将打包在同意个AB包里面
        public string abDir;
        //保存这个路径下所有文件打包的AB包名
        public string abName;
    }
}
