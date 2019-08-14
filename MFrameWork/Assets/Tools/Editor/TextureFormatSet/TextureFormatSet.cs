using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TextureFormatSet : Editor
{
    //UI贴图文件夹路径
    private static string TEXTURE_FULL_PATH = UnityEngine.Application.dataPath +"/Resources/Texture";
    //UI贴图文件夹Asset路径
    private static string TEXTURE_ASSET_PATH = "Assets/Resources/Texture";
    //忽略列表配置路径
    private static string IGNORE_SET_TEXTURE_CONFIGPATH = UnityEngine.Application.dataPath + "/Resources/Texture/IgonreConifg/TextureIgnoreConfig.txt";

    private static List<string> IgnoreFormatList = new List<string>();

    [MenuItem("MSimpleTools/TextureFormatSet/AllTextureFormatSet(全部设置)")]
    [MenuItem("Assets/TextureFormatSet/AllTextureFormatSet(全部设置)")]
    public static void TextureReset()
    {
        if (Directory.Exists(TEXTURE_FULL_PATH))
        {
            List<FileInfo> fileInfoList = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(TEXTURE_FULL_PATH);
            FileInfo[] allPng = dir.GetFiles("*.png", SearchOption.AllDirectories);
            FileInfo[] allJpg = dir.GetFiles("*.jpg", SearchOption.AllDirectories);
            fileInfoList.AddRange(allPng.ToList());
            fileInfoList.AddRange(allJpg.ToList());

            if (UnityEditor.EditorUtility.DisplayDialog("贴图预设置", "全部设置前是否需要进行预设值？", "需要", "不需要"))
            {
                for (int i = 0; i < fileInfoList.Count; i++)
                {
                    string assetFilePath = fileInfoList[i].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    EditorUtility.DisplayCancelableProgressBar("贴图预设置中~", "正在设置 Path: " + assetFilePath, ((float)(i + 1)) / fileInfoList.Count);
                    PreSetTexture(assetFilePath);
                }
            }
            else
            {

            }

            for (int i = 0; i < fileInfoList.Count; i++)
            {
                string assetFilePath = fileInfoList[i].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                EditorUtility.DisplayCancelableProgressBar("贴图设置中~", "正在设置 Path: " + assetFilePath, ((float)(i + 1)) / fileInfoList.Count);
                SetTextureByTextureType(assetFilePath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("MSimpleTools/TextureFormatSet/OneTextureFormatSet(设置选择贴图Format)")]
    [MenuItem("Assets/TextureFormatSet/OneTextureFormatSet(设置选择贴图Format)")]
    public static void OneTextureFormatSet()
    {
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i] is Texture2D)
            {
                PreSetTexture(AssetDatabase.GetAssetPath(Selection.objects[i]));
                SetTextureByTextureType(AssetDatabase.GetAssetPath(Selection.objects[i]));
            }
        }
    }

    [MenuItem("MSimpleTools/TextureFormatSet/AddTextureToIgnoreList(添加到忽略列表)")]
    [MenuItem("Assets/TextureFormatSet/AddTextureToIgnoreList(添加到忽略列表)")]
    public static void AddToIgnoreList()
    {
        InitIgnoreList();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i] is Texture2D)
            {
                if (!IgnoreFormatList.Contains(AssetDatabase.GetAssetPath(Selection.objects[i])))
                {
                    IgnoreFormatList.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    Debug.LogError("成功添加到了忽略列表 Path ：" + AssetDatabase.GetAssetPath(Selection.objects[i]));
                }
                else
                {
                    Debug.LogError("已经添加到了忽略列表 Path ：" + AssetDatabase.GetAssetPath(Selection.objects[i]));
                }
            }
        }
        WriteIgnoreToFile();
    }

    [MenuItem("MSimpleTools/TextureFormatSet/RemoveTextureToIgnoreList(从忽略列表移除)")]
    [MenuItem("Assets/TextureFormatSet/RemoveTextureToIgnoreList(从忽略列表移除)")]
    public static void RemoveFromIgnoreList()
    {
        InitIgnoreList();
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i] is Texture2D)
            {
                if (IgnoreFormatList.Contains(AssetDatabase.GetAssetPath(Selection.objects[i])))
                {
                    IgnoreFormatList.Remove(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    Debug.LogError("成功从忽略列表中移除 Path ：" + AssetDatabase.GetAssetPath(Selection.objects[i]));
                }
            }
        }
        WriteIgnoreToFile();
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    public static void WriteIgnoreToFile()
    {
        string writeStr = "";// streamReader.ReadToEnd();
        //清除原有内容
        FileStream fileStream = new FileStream(IGNORE_SET_TEXTURE_CONFIGPATH, FileMode.Create, FileAccess.Write);
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.SetLength(0);
        fileStream.Close();
        //写内容
        StreamWriter streamWriter = new StreamWriter(IGNORE_SET_TEXTURE_CONFIGPATH);
        for (int i = 0; i < IgnoreFormatList.Count; i++)
        {
            writeStr += IgnoreFormatList[i] + "|";
        }
        streamWriter.Write(writeStr);
        streamWriter.Close();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 判断贴图是不是2的N次方贴图
    /// </summary>
    /// <param name="texture2D"></param>
    /// <returns></returns>
    public static bool Is2NSizeTexture(Texture2D texture2D)
    {
        float sqrtNum = Mathf.Sqrt(texture2D.width * texture2D.height);
        return !sqrtNum.ToString().Contains('.');
    }

    /// <summary>
    /// 对贴图进行预设置 主要问题是Non Power of 2 设置完成之后 需要Apply 才能Get到正确的宽高
    /// </summary>
    public static void PreSetTexture(string assetPath)
    {
        //初始化忽略列表
        InitIgnoreList();
        //UI贴图判断
        if (!assetPath.Contains(TEXTURE_ASSET_PATH))
        {
            return;
        }
        //忽略列表判断
        if (IgnoreFormatList.Contains(assetPath))
        {
            return;
        }

        Texture2D cTexture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (cTexture2D == null)
        {
            Debug.LogError("Texture Is Nil Path : " + assetPath);
            return;
        }
        TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        bool is2NSize = Is2NSizeTexture(cTexture2D);
        bool isTransparent = ti.DoesSourceTextureHaveAlpha();

        ti.isReadable = true;//用完需要关掉
        ti.textureType = TextureImporterType.Default;
        ti.textureShape = TextureImporterShape.Texture2D;
        //科普 Ro的设置为Gamma sRGB在Gamma空间下~勾选与否无关。但是在liner空间下，勾选shader会自动将读到的像素作gramma矫正，即x的0.45次方
        ti.sRGBTexture = false;
        ti.alphaSource = TextureImporterAlphaSource.FromInput;
        //透明贴图勾选了后 透明部分RBG信息丢掉？？
        ti.alphaIsTransparency = isTransparent;
        ti.mipmapEnabled = false;
        //科普 TextureWrapMode.Clamp使用强制贴图边界拉伸  TextureWrapMode.Repeat贴图重复平铺 
        ti.wrapMode = TextureWrapMode.Repeat;
        //科普 https://blog.csdn.net/candycat1992/article/details/22794773
        ti.filterMode = FilterMode.Bilinear;
        //是否自动化到2的N次方
        ti.npotScale = TextureImporterNPOTScale.None;
		ti.isReadable = false;//用完需要关掉
        ti.SaveAndReimport();
    }

    /// <summary>
    /// 设置贴图
    /// </summary>
    /// <param name="assetPath"></param>
    public static void SetTextureByTextureType(string assetPath)
    {
        //初始化忽略列表
        InitIgnoreList();
        //UI贴图判断
        if (!assetPath.Contains(TEXTURE_ASSET_PATH))
        {
            return;
        }
        //忽略列表判断
        if (IgnoreFormatList.Contains(assetPath))
        {
            return;
        }

        Texture2D cTexture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (cTexture2D == null)
        {
            Debug.LogError("Texture Is Nil Path : " + assetPath);
            return;
        }
        TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        bool is2NSize = Is2NSizeTexture(cTexture2D);
        bool isTransparent = ti.DoesSourceTextureHaveAlpha();

        //------------------Pc设置
        var pfStandalone = ti.GetPlatformTextureSettings("Standalone");
        pfStandalone.overridden = true;
        pfStandalone.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        if (is2NSize)
        {
            pfStandalone.format = isTransparent ? TextureImporterFormat.DXT5 :TextureImporterFormat.DXT1;
        }
        else
        {
            pfStandalone.format = isTransparent ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB16;
        }
        ti.SetPlatformTextureSettings(pfStandalone);
        pfStandalone.overridden = false;

        //-----------------Ios设置
        var pfIPhone = ti.GetPlatformTextureSettings("iPhone");
        pfIPhone.overridden = true;
        pfIPhone.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        if (is2NSize)
        {
            pfIPhone.format = isTransparent ? TextureImporterFormat.ASTC_RGBA_4x4 : TextureImporterFormat.ASTC_RGB_4x4;
        }
        else
        {
            pfIPhone.format = isTransparent ? TextureImporterFormat.ASTC_RGBA_4x4 : TextureImporterFormat.RGB16;
        }
        ti.SetPlatformTextureSettings(pfIPhone);
        pfIPhone.overridden = false;

        //-----------------Android设置
        var pfAndroid = ti.GetPlatformTextureSettings("Android");
        pfAndroid.overridden = true;
        pfAndroid.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        if (is2NSize)
        {
            pfAndroid.format = isTransparent ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC_RGB4;
        }
        else
        {
            pfAndroid.format = isTransparent ? TextureImporterFormat.RGBA16 : TextureImporterFormat.RGB16;
        }
        pfAndroid.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
        ti.SetPlatformTextureSettings(pfAndroid);
        pfAndroid.overridden = false;
    }

    public static void InitIgnoreList()
    {
        IgnoreFormatList.Clear();
        if (File.Exists(IGNORE_SET_TEXTURE_CONFIGPATH))
        {
            StreamReader stringReader = new StreamReader(IGNORE_SET_TEXTURE_CONFIGPATH);
            string txt = stringReader.ReadToEnd();
            var data = txt.Split('|');
            for (int i = 0; i < data.Length; i++)
            {
                if (!IgnoreFormatList.Contains(data[i]) && data[i]!="")
                {
                    IgnoreFormatList.Add(data[i]);
                }
            }
            stringReader.Close();
        }
    }
}
