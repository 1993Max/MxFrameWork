// **************************************
//
// 文件名(AssetBundleBuildTools.cs):
// 功能描述("用于打包AssetBundle的脚本文件"):
// 作者(Max1993):
// 日期(2019/5/1  13:46):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MFrameWork;
using System;

public class AssetBundleBuildTools
{
    //public static string ASSETBUNDLE_PATH = Application.streamingAssetsPath;
    //public static string ASSETBUNDLE_CONFIG_PATH = "Assets/Resources/GameData/AssetBundleData/AssetBundleConfig.asset";
    //public static string ASSETBUNDLE_XML_PATH = Application.dataPath + "/Resources/GameData/AssetBundleData/MAssetBundleConfig.xml";
    //public static string ASSETBUNDLE_BYTES_PATH = Application.dataPath + "/Resources/GameData/AssetBundleData/MAssetBundleConfig.bytes";

    //保存一对多的Dic 即1个Ab保存一个路径下的所有文件 Key是AbName Value是ab所在的路径
    public static Dictionary<string, string> m_allFileDirDic = new Dictionary<string, string>();

    //这个列表保存所有添加过得路径的列表 用于后续重复性判断
    public static List<string> m_AddedPathList = new List<string>();

    //保存单个AB和它的所有依赖文件Path
    public static Dictionary<string, List<string>> m_abWithDepenceDic = new Dictionary<string, List<string>>();

    //有效路径存储 这里的有效路径指的打包的资源文件夹路径和所有单个资源路径 方便后续引用资源剔除
    public static List<string> m_EffectiveFile = new List<string>();

    //存储所有打包的资源文件的对应信息 Key是资源的全路径 Valeu是AB的名字 这个存储比较重要 后续生成XML的时候有用到
    public static Dictionary<string, string> m_AllAssetBundleDic = new Dictionary<string, string>();

    [MenuItem("MSimpleTools/BuildAssetBundle(打包AssetBundle)")]
    public static void BuildAssetBundle()
    {
        ClearData();

        //1 建立Ab名字和Path的关系
        BuildReleation();

        //2 设置BundleName
        SetAllAssetBundleName();

        //3 根据现有的ABName设置在打包之前删除无用的AB文件
        DelectUnUsedAB();

        //4 数据导出成2进制序列化文件 导出的文件也到打成包 方便在非Editor下读取
        SetBuildAbDicInfo();
        WriteToData(m_AllAssetBundleDic);

        //5 打包AB
        BuildAllAssetBundle();

        //6 为了防止Meta文件的变更 打包结束之后 清除之前设置的BundleName
        RemoveAllABName();

        //5 打包结束 刷新
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    //根据现有配置 建立Ab名字和Path的关系
    public static void BuildReleation()
    {
        AssetBundleBuildConfig assetBundleBuildConfig =
            AssetDatabase.LoadAssetAtPath<AssetBundleBuildConfig>(MPathUtils.ASSETBUNDLE_CONFIG_PATH);

        //下面这个Foreach 直接添加配置的Config信息到Dic里面
        foreach (var item in assetBundleBuildConfig.m_allFileDirAb)
        {
            if (m_allFileDirDic.ContainsKey(item.abName))
            {
                Debug.LogError("当前m_allFileDirDic里面已经有这个Ab名了");
            }
            else
            {
                m_allFileDirDic.Add(item.abName, item.abDir);
                m_AddedPathList.Add(item.abDir);
                m_EffectiveFile.Add(item.abDir);
            }
        }

        //1 首先找到配置文件夹下的所有文件
        //2 一次找到每个文件的依赖关系
        //3 去已添加的依赖
        string[] allFileGuids = AssetDatabase.FindAssets("t:prefab", assetBundleBuildConfig.m_allFilePath.ToArray());
        for (int i = 0; i < allFileGuids.Length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(allFileGuids[i]);
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            EditorUtility.DisplayCancelableProgressBar("Calculate Ab Releation", "ResPath :" + filePath, i / allFileGuids.Length);
            //找到这个文件的所有依赖项 找到的依赖项包括本身
            string[] dependence = AssetDatabase.GetDependencies(filePath);
            List<string> fileAllDependeceList = new List<string>();
            for (int z = 0; z < dependence.Length; z++)
            {
                if (!dependence[z].EndsWith(".cs") && !CheckPathIsAdded(dependence[z]))
                {
                    fileAllDependeceList.Add(dependence[z]);
                    m_AddedPathList.Add(dependence[z]);
                }
            }
            m_abWithDepenceDic.Add(gameObject.name, fileAllDependeceList);
            m_EffectiveFile.Add(filePath);
        }
    }

    public static void SetAllAssetBundleName()
    {
        foreach (var item in m_allFileDirDic)
        {
            SetAbName(item.Key, item.Value);
        }
        foreach (var item in m_abWithDepenceDic)
        {
            SetAbName(item.Key, item.Value);
        }

    }

    //移除所有设置的Ab名字
    public static void RemoveAllABName()
    {
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
            EditorUtility.DisplayCancelableProgressBar("Clear AbNames", "Clear Ab Names : " + abNames[i], i / abNames.Length);
        }
    }

    //每次打包都做一下操作删除之前打包的无用的AB
    public static void DelectUnUsedAB()
    {
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo directory = new DirectoryInfo(MPathUtils.ASSETBUNDLE_PATH);
        FileInfo[] allFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            if (ContainAbName(allFiles[i].Name, abNames) || allFiles[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            else
            {
                if(File.Exists(allFiles[i].FullName))
                {
                    File.Delete(allFiles[i].FullName);
                }
            }
        }
    }

    public static bool ContainAbName(string fileName,string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (fileName == strs[i])
                return true;
        }
        return false;
    }

    //在打包之前建立资源路径和AB名字的关系
    public static void SetBuildAbDicInfo()
    {
        string[] allBundleName = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < allBundleName.Length; i++)
        {
            string[] allAssetPath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundleName[i]);
            for (int j = 0; j < allAssetPath.Length; j++)
            {
                if (allAssetPath[j].EndsWith(".cs"))
                    continue;

                //是否是有效路径判断
                if (IsEffectivePath(allAssetPath[j]))
                    m_AllAssetBundleDic.Add(allAssetPath[j], allBundleName[i]);
            }
        }
    }

    //数据导出
    public static void WriteToData(Dictionary<string, string> resPathDic)
    {
        MAssetBundleConfig mAssetBundleConfig = new MAssetBundleConfig();
        mAssetBundleConfig.AssetBundleList = new List<MAssetBundleBase>();
        foreach (var item in resPathDic)
        {
            string nowPath = item.Key;
            MAssetBundleBase mAssetBundleBase = new MAssetBundleBase();
            mAssetBundleBase.Path = nowPath;
            mAssetBundleBase.Crc = MCrcHelper.GetCRC32(nowPath);
            mAssetBundleBase.AbName = item.Value;
            mAssetBundleBase.AssetName = item.Key.Remove(0, nowPath.LastIndexOf("/") + 1);
            mAssetBundleBase.AbDependence = new List<string>();
            //先获取到这个路径的所有依赖项的路径 然后遍历依赖项
            string[] resDependences = AssetDatabase.GetDependencies(nowPath);
            for (int i = 0; i < resDependences.Length; i++)
            {
                string tempPath = resDependences[i];
                //如果当前遍历路径==查询路径 或者 是脚本文件
                if (tempPath == nowPath || tempPath.EndsWith(".cs"))
                    continue;

                //然后根据上文传进来的Dic 判定每一个依赖路径所对应的AB名字 进行添加
                string abName;
                if (resPathDic.TryGetValue(tempPath, out abName))
                {
                    if (abName == resPathDic[nowPath])
                        continue;
                    if (!mAssetBundleBase.AbDependence.Contains(abName))
                    {
                        mAssetBundleBase.AbDependence.Add(abName);
                    }
                }
            }
            //数据插入
            mAssetBundleConfig.AssetBundleList.Add(mAssetBundleBase);
        }

        //写入XML
        if (File.Exists(MPathUtils.ASSETBUNDLE_XML_PATH))
            File.Delete(MPathUtils.ASSETBUNDLE_XML_PATH);
        FileStream fileStream = new FileStream(MPathUtils.ASSETBUNDLE_XML_PATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xmlSerializer = new XmlSerializer(mAssetBundleConfig.GetType());
        xmlSerializer.Serialize(streamWriter, mAssetBundleConfig);
        streamWriter.Close();
        fileStream.Close();

        //写入Bytes
        if (File.Exists(MPathUtils.ASSETBUNDLE_BYTES_PATH))
            File.Delete(MPathUtils.ASSETBUNDLE_BYTES_PATH);
        FileStream bytesStresm = new FileStream(MPathUtils.ASSETBUNDLE_BYTES_PATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        //写入二进制 不写入Path 优化Bytes大小
        for (int i = 0; i < mAssetBundleConfig.AssetBundleList.Count; i++)
        {
            mAssetBundleConfig.AssetBundleList[i].Path = null;
        }
        binaryFormatter.Serialize(bytesStresm, mAssetBundleConfig);
        bytesStresm.Close();
    }

    //打包Ab文件
    public static void BuildAllAssetBundle()
    {
        BuildPipeline.BuildAssetBundles(MPathUtils.ASSETBUNDLE_PATH,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        EditorUserBuildSettings.activeBuildTarget);
    }

    //清除Dic
    public static void ClearData()
    {
        m_allFileDirDic.Clear();
        m_abWithDepenceDic.Clear();
        m_AddedPathList.Clear();
        m_EffectiveFile.Clear();
    }

    //检测是否添加的接口函数
    public static bool CheckPathIsAdded(string path)
    {
        for (int i = 0; i < m_AddedPathList.Count; i++)
        {
            if (path == m_AddedPathList[i] || path.Contains(m_AddedPathList[i]) && (path.Replace(m_AddedPathList[i],"")[0] == '/'))
                return true;
        }
        return false;
    }

    //设置AB名的函数
    public static void SetAbName(string abName, string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
        {
            Debug.LogError("此路径不存在文件 Path ：" + path);
        }
        else
        {
            assetImporter.assetBundleName = abName;
        }
    }

    //设置AB名
    public static void SetAbName(string abName, List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetAbName(abName, paths[i]);
        }
    }

    //是否是有效路径判断
    public static bool IsEffectivePath(string path)
    {
        for (int i = 0; i < m_EffectiveFile.Count; i++)
        {
            if(path.Contains(m_EffectiveFile[i]))
            {
                return true;
            }
        }
        return false;
    }
}
