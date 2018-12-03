using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextureSolver : EditorWindow {

    [MenuItem("Assets/Texture Solver #%b")]
    public static void Init()
    {
        TextureSolver me;
        me = GetWindow(typeof(TextureSolver)) as TextureSolver;
        me.titleContent = new GUIContent("图片处理工具");
        LoadResources();
    }

    static List<Object> textures = new List<Object>();
    public string rgbPostfix = "_rgb";
    public string alphaPostfix = "_alpha";

    static void LoadResources()
    {
        textures.Clear();
        textures.AddRange(Selection.GetFiltered<Texture>(SelectionMode.DeepAssets));
    }

    Vector2 uv;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("重新获取", GUILayout.Height(60)))
        {
            LoadResources();
        }
        if (GUILayout.Button("全部压缩", GUILayout.Height(60)))
        {
            int i = 0;
            foreach (var t in textures)
            {
                if (t && t is Texture2D)
                {
                    EditorUtility.DisplayProgressBar("压缩中", string.Format("正在压缩 {0}", t.name), (float)(++i) / textures.Count);
                    ConvertToDither16(t);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button("补充Alpha", GUILayout.Height(60)))
        {
            ReAlpha(textures);
        }

        if (GUILayout.Button("全部拆分", GUILayout.Height(60)))
        {
            int i = 0;
            foreach (var t in textures)
            {
                if (t && t is Texture2D)
                {
                    EditorUtility.DisplayProgressBar("拆分中", string.Format("正在拆分 {0}", t.name), (float)(++i) / textures.Count);
                    ExtractAlpha(t as Texture2D, rgbPostfix, alphaPostfix);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        EditorGUILayout.EndHorizontal();

        rgbPostfix = EditorGUILayout.TextField("分离RGB后缀", rgbPostfix);
        alphaPostfix = EditorGUILayout.TextField("分离Alpha后缀", alphaPostfix);

        uv = EditorGUILayout.BeginScrollView(uv);
        {
            foreach (var t in textures)
            {
                if (t && t is Texture2D)
                {
                    EditorGUILayout.ObjectField(t.name, t, typeof(Texture2D), false);
                }              
            }
        }
        EditorGUILayout.EndScrollView();
    }

    public static void ReAlpha(List<Object> selections)
    {
        if (EditorUtility.DisplayDialog("处理确认", "确认选中的文件为带_A的文件", "确定", "取消"))
        {
            for (int i = 0; i < selections.Count; i++)
            {
                if (selections[i] is Texture2D)
                {
                    var path = AssetDatabase.GetAssetPath(selections[i]);
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }
                    EditorUtility.DisplayProgressBar("补上Alpha", string.Format("正在修复 {0}", selections[i].name), (float)(++i) / selections.Count);
                    var ai = AssetImporter.GetAtPath(path) as TextureImporter;
                    ai.isReadable = true;
                    var pfw = ai.GetPlatformTextureSettings("Standalone");
                    var pfa = ai.GetPlatformTextureSettings("Android");
                    var pfi = ai.GetPlatformTextureSettings("iPhone");
                    pfw.format = TextureImporterFormat.RGBA32;
                    pfa.format = TextureImporterFormat.RGBA32;
                    pfi.format = TextureImporterFormat.RGBA32;
                    ai.SetPlatformTextureSettings(pfw);
                    ai.SetPlatformTextureSettings(pfa);
                    ai.SetPlatformTextureSettings(pfi);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                    var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                    var pixels = t.GetPixels();

                    for (int j = 0; j < pixels.Length; j++)
                    {
                        pixels[j] = new Color(pixels[j].r, pixels[j].r, pixels[j].r, pixels[j].r);
                    }

                    var nt = new Texture2D(t.width, t.height, TextureFormat.RGBA32, false);
                    nt.SetPixels(pixels);
                    var bytes = nt.EncodeToPNG();
                    ai.textureType = TextureImporterType.SingleChannel;
                    pfw.format = TextureImporterFormat.Alpha8;
                    pfa.format = TextureImporterFormat.Alpha8;
                    pfi.format = TextureImporterFormat.Alpha8;
                    ai.SetPlatformTextureSettings(pfw);
                    ai.SetPlatformTextureSettings(pfa);
                    ai.SetPlatformTextureSettings(pfi);
                    ai.isReadable = false;
                    ai.mipmapEnabled = false;
                    ai.SaveAndReimport();
                    System.IO.File.WriteAllBytes(path, bytes);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("完毕确认", "完成补充", "确定");
        }
    }

    public static void ConvertToDither16(Object t)
    {
        if (t is Texture2D)
        {
            var texture = t as Texture2D;
            var path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti)
            {
                if (!ti.isReadable)
                {
                    ti.isReadable = true;
                    ti.SaveAndReimport();
                }

                System.IO.File.WriteAllBytes(path, OnPostprocessTexture(texture));

                AssetDatabase.Refresh();
                ti = AssetImporter.GetAtPath(path) as TextureImporter;
                var platform = ti.GetPlatformTextureSettings("Standalone");
                platform.format = TextureImporterFormat.RGB16;
                ti.SetPlatformTextureSettings(platform);

                platform = ti.GetPlatformTextureSettings("iPhone");
                platform.format = TextureImporterFormat.RGB16;
                ti.SetPlatformTextureSettings(platform);

                platform = ti.GetPlatformTextureSettings("Android");
                platform.format = TextureImporterFormat.RGB16;
                ti.SetPlatformTextureSettings(platform);

                ti.isReadable = false;
                ti.SaveAndReimport();
            }
        }
    }

    public static void ExtractAlpha(Texture2D t, string rgbPostfix = "_rgb", string alphaPostfix = "_alpha")
    {
        var asset = AssetDatabase.GetAssetPath(t);
        if (string.IsNullOrEmpty(asset))
            return;

        var ip = AssetImporter.GetAtPath(asset) as TextureImporter;
        if (!ip.DoesSourceTextureHaveAlpha())
            return;

        if (ip.alphaSource != TextureImporterAlphaSource.FromInput)
            return;

        bool update = false;
        TextureImporterFormat format = TextureImporterFormat.Automatic;
        var pf = ip.GetDefaultPlatformTextureSettings();

        if (!ip.isReadable)
        {
            ip.isReadable = true;
            update = true;
        }

        if (pf.format != TextureImporterFormat.RGBA32 || pf.format != TextureImporterFormat.ARGB32 || pf.format != TextureImporterFormat.RGB24)
        {
            update = true;
            format = pf.format;
            pf.format = TextureImporterFormat.RGBA32;
            ip.SetPlatformTextureSettings(pf);
        }

        if (update)
        {
            AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
        }

        if (ip.isReadable)
        {
            var colors = t.GetPixels();
            Texture2D rgb = new Texture2D(t.width, t.height, TextureFormat.RGB24, false);
            Texture2D alpha = new Texture2D(t.width, t.height, TextureFormat.RGBA32, false);

            rgb.SetPixels(colors);

            for (int i = 0; i < colors.Length; i++)
            {
                var a = colors[i].a;
                colors[i] = new Color(a, a, a, a);
            }

            alpha.SetPixels(colors);

            WriteFile(rgb, alpha, asset.Substring(0, asset.LastIndexOf("/")), t.name, rgbPostfix, alphaPostfix, ip.mipmapEnabled);

            ip.isReadable = false;
            AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
        }
    }

    static HashSet<Material> sceneMaterials = new HashSet<Material>();

    public static void OneKeyChangeTex(Texture2D[] texes, bool split = true)
    {
        if (split)
        {
            for (int i = 0; i < texes.Length; i++)
            {
                if (texes[i])
                {
                    var asset = AssetDatabase.GetAssetPath(texes[i]);

                    if (string.IsNullOrEmpty(asset))
                        continue;


                }
            }
        }
    }

    static void WriteFile(Texture2D rgb, Texture2D alpha, string folder, string sourceName, string rgbPostfix, string alphaPostfix, bool mipmap)
    {
        var bytesRGB = rgb.EncodeToPNG();
        var bytesAlpha = alpha.EncodeToPNG();
        var rgbFile = string.Format("{0}/{1}{2}.png", folder, sourceName, rgbPostfix);
        var alphaFile = string.Format("{0}/{1}{2}.png", folder, sourceName, alphaPostfix);

        if (!System.IO.Directory.Exists(folder))
        {
            System.IO.Directory.CreateDirectory(folder);
        }

        if (System.IO.File.Exists(rgbFile) || System.IO.File.Exists(alphaFile))
        {
            Debug.Log("存在该名称文件, 不予写入");
            return;
        }

        System.IO.File.WriteAllBytes(rgbFile, bytesRGB);
        System.IO.File.WriteAllBytes(alphaFile, bytesAlpha);
        AssetDatabase.ImportAsset(rgbFile, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(alphaFile, ImportAssetOptions.ForceUpdate);
        var iprgb = AssetImporter.GetAtPath(rgbFile) as TextureImporter;
        var ipalpha = AssetImporter.GetAtPath(alphaFile) as TextureImporter;
        iprgb.mipmapEnabled = mipmap;
        ipalpha.mipmapEnabled = mipmap;
        AssetDatabase.ImportAsset(rgbFile, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(alphaFile, ImportAssetOptions.ForceUpdate);
    }

    static byte[] OnPostprocessTexture(Texture2D texture)
    {
        var texw = texture.width;
        var texh = texture.height;

        var pixels = texture.GetPixels();
        var offs = 0;

        var k1Per15 = 1.0f / 15.0f;
        var k1Per16 = 1.0f / 16.0f;
        var k3Per16 = 3.0f / 16.0f;
        var k5Per16 = 5.0f / 16.0f;
        var k7Per16 = 7.0f / 16.0f;

        for (var y = 0; y < texh; y++)
        {
            for (var x = 0; x < texw; x++)
            {
                float a = pixels[offs].a;
                float r = pixels[offs].r;
                float g = pixels[offs].g;
                float b = pixels[offs].b;

                //裁剪颜色值从8bit到4bit
                //设r=0.7843(200, 1100 1000)
                //Mathf.Floor(a * 16) = 12即相当于r(1100 1000) >> 4 = 1100 = 0.8
                //12 / 15 = 0.8 归一化
                var a2 = Mathf.Clamp01(Mathf.Floor(a * 16) * k1Per15);
                var r2 = Mathf.Clamp01(Mathf.Floor(r * 16) * k1Per15);
                var g2 = Mathf.Clamp01(Mathf.Floor(g * 16) * k1Per15);
                var b2 = Mathf.Clamp01(Mathf.Floor(b * 16) * k1Per15);

                //得到差值-0.0157
                var ae = a - a2;
                var re = r - r2;
                var ge = g - g2;
                var be = b - b2;

                //将当前点的颜色值改为裁剪后的
                pixels[offs].a = a2;
                pixels[offs].r = r2;
                pixels[offs].g = g2;
                pixels[offs].b = b2;

                //由于pixels是一维数组, 所以+texw即往下移一行
                //n1/2/3/4 分别为该点的左侧/左下/下方/右下的索引
                var n1 = offs + 1;
                var n2 = offs + texw - 1;
                var n3 = offs + texw;
                var n4 = offs + texw + 1;

                //如果该点不是该行最后一个点，则左侧的点存在，乘以7/16修正之。
                if (x < texw - 1)
                {
                    pixels[n1].a += ae * k7Per16;
                    pixels[n1].r += re * k7Per16;
                    pixels[n1].g += ge * k7Per16;
                    pixels[n1].b += be * k7Per16;
                }

                //如果该点不是该列最后一个点，则下一行存在，下方的点存在，乘以5/16修正之。
                if (y < texh - 1)
                {
                    pixels[n3].a += ae * k5Per16;
                    pixels[n3].r += re * k5Per16;
                    pixels[n3].g += ge * k5Per16;
                    pixels[n3].b += be * k5Per16;

                    //如果该点不是该行第一个点，则左下的点存在，乘以3/16修正之。
                    if (x > 0)
                    {
                        pixels[n2].a += ae * k3Per16;
                        pixels[n2].r += re * k3Per16;
                        pixels[n2].g += ge * k3Per16;
                        pixels[n2].b += be * k3Per16;
                    }

                    //如果该点不是该行最后一个点，则右下的点存在，乘以1/16修正之。
                    if (x < texw - 1)
                    {
                        pixels[n4].a += ae * k1Per16;
                        pixels[n4].r += re * k1Per16;
                        pixels[n4].g += ge * k1Per16;
                        pixels[n4].b += be * k1Per16;
                    }
                }

                //读取下一个点
                offs++;
            }
        }
        Texture2D et = new Texture2D(texture.width, texture.height, TextureFormat.RGB565, false);
        et.SetPixels(pixels);
        et.Apply();
        return et.EncodeToPNG();
        //EditorUtility.CompressTexture(texture, TextureFormat.RGB565, TextureCompressionQuality.Best);
    }

}
