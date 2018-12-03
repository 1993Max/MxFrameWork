using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using MoonCommonLib;

public class SplitTextureAlpha : EditorWindow     
{
    UnityEngine.Object[] selections;
    Vector2 scroll = Vector2.zero;

    [MenuItem("Assets/ROTools/图集处理/Texture Alpha Split #Q")]
    [MenuItem("ROTools/图集处理/Texture Alpha Split #Q")]
    public static void ShowWindow()
    {
        SplitTextureAlpha st = (SplitTextureAlpha)EditorWindow.GetWindow(typeof(SplitTextureAlpha));
        st.titleContent = new GUIContent("批量分离");
        st.LoadSelection();
    }

    public void LoadSelection()
    {
        if (Selection.objects != null || Selection.objects.Length > 0)
        {
            selections = Selection.objects;
        }
    }

    private void OnGUI()
    {

        if (GUILayout.Button("一键分离"))
        {
            SplitSelectTextures(0, 0, selections);
        }

        if (selections != null)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                for (int i = 0; i < selections.Length; i++)
                {
                    EditorGUILayout.LabelField("", selections[i].name, "helpbox");
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    public static void SplitSelectTextures(int texSize, int alphaSize,  params UnityEngine.Object[] selections)
    {
        Texture2D source;
        Color[] colors;
        Color[] alphas;

        string path;
        string sourcefilePath;
        for (int i = 0; i < selections.Length; i++)
        {
            string atlasPath = MPathUtils.UI_PUBLISH_PATH + "Atlas";
            string alphaPath = MPathUtils.UI_PUBLISH_PATH + "Atlas/Alpha";
            if (selections[i] is Texture2D)
            {
                path = AssetDatabase.GetAssetPath(selections[i]);
                atlasPath = atlasPath + "/" + Path.GetFileNameWithoutExtension(path) + ".png";
                alphaPath = alphaPath + "/" + Path.GetFileNameWithoutExtension(path) + "_A.png";

                EditorUtility.DisplayCancelableProgressBar("图片分离中~", string.Format("正在分离 {0}", path+ (i + 1) +"/"+ (selections.Length)),(float)((i+1)/(selections.Length)));

                if (IsTextureFile(path) && !IsTextureConverted(path))
                {
                    try
                    {
                        //对源数据Border缓存
                        TextureImporter cacheTextureImporter = GetFileData(atlasPath);
                        sourcefilePath = path.Split('.')[0];
                        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

                        importer.isReadable = true;
                        importer.SaveAndReimport();
                        source = AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
                        if (source && importer.DoesSourceTextureHaveAlpha())
                        {
                            colors = source.GetPixels();
                            WriteToFile(source.width, source.height, colors, TextureFormat.RGB24, path,atlasPath, texSize, cacheTextureImporter,true);
                            alphas = GetAlphaPixels(source);
                            //alphas = new Color[colors.Length];
                            //for (int c = 0; c < colors.Length; c++)
                            //{
                            //    alphas[c].r = colors[c].a;
                            //    alphas[c].g = colors[c].a;
                            //    alphas[c].b = colors[c].a;
                            //    alphas[c].a = colors[c].a;
                            //}
                            WriteToFile(alphas, source.width / 2, source.height / 2, alphaPath);
                        }
                        importer.isReadable = false;
                        importer.SaveAndReimport();
                        //创建材质
                        CreateMaterial(atlasPath, alphaPath);
                    }
                    catch(Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("分离图集", "完成", "OK");
    }

    [MenuItem("Assets/ROTools/图集处理/Texture Alpha Convert")]
    public static void BatchConvertToAlpha4()
    {
        var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        for (int i = 0; i < textures.Length; i++)
        {
            var ti = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(textures[i])) as TextureImporter;
            EditorUtility.DisplayProgressBar("批量转换", ti.assetPath, (float)(i + 1) / textures.Length);

            var pf = ti.GetPlatformTextureSettings("Standalone");
            pf.overridden = false;
            ti.SetPlatformTextureSettings(pf);

            pf = ti.GetPlatformTextureSettings("Android");
            pf.overridden = false;
            ti.SetPlatformTextureSettings(pf);

            pf = ti.GetPlatformTextureSettings("iPhone");
            pf.overridden = false;
            ti.SetPlatformTextureSettings(pf);

            pf = ti.GetDefaultPlatformTextureSettings();
            pf.textureCompression = TextureImporterCompression.Uncompressed;
            pf.format = TextureImporterFormat.RGBA32;
            ti.SetPlatformTextureSettings(pf);

            ti.textureType = TextureImporterType.Default;
            ti.isReadable = true;

            AssetDatabase.ImportAsset(ti.assetPath, ImportAssetOptions.ForceUpdate);

            Color[] alphas = GetAlphaPixels(textures[i]);

            WriteToFile(alphas, textures[i].width / 2, textures[i].height / 2, ti.assetPath);
        }

        EditorUtility.ClearProgressBar();
    }

    static void WriteToFile(Color[] colors, int width, int height, string path)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        tex.SetPixels(colors);
        tex.Apply();
        var bytes = tex.EncodeToPNG();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllBytes(path, bytes);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        ti.textureType = TextureImporterType.Default;
        ti.isReadable = false;

        var pf = ti.GetPlatformTextureSettings("Standalone");
        pf.overridden = false;
        pf.format = TextureImporterFormat.RGBA32;
        ti.SetPlatformTextureSettings(pf);

        pf = ti.GetPlatformTextureSettings("Android");
        pf.overridden = false;
        pf.format = TextureImporterFormat.RGBA32;
        ti.SetPlatformTextureSettings(pf);

        pf = ti.GetPlatformTextureSettings("iPhone");
        pf.overridden = false;
        pf.format = TextureImporterFormat.RGBA32;
        ti.SetPlatformTextureSettings(pf);

        ti.filterMode = FilterMode.Bilinear;
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.mipmapEnabled = false;
        ti.anisoLevel = 0;

        ti.SaveAndReimport();
    }

    static Color[] GetAlphaPixels(Texture2D source)
    {
        int halfWidth = source.width / 2, halfHeight = source.height / 2;
        Color[] colors = new Color[halfWidth * halfHeight];
        //r = 0 g = 1, b =  2 a = 3
        int ixy = 0;
        for (int y = 0; y < source.height; y++)
        {
            int yr = y - halfHeight;
            int yl = y;

            for (int x = 0; x < source.width; x++)
            {
                int xr = x - halfWidth;
                int xl = x;
                Color col = Color.clear;
                if (yr >= 0 && xr >= 0)
                {
                    ixy = yr * halfWidth + xr;
                    col = colors[ixy];
                    col.a = source.GetPixel(x, y).a;
                }
                else if (yr >= 0)
                {
                    ixy = yr * halfWidth + xl;
                    col = colors[ixy];
                    col.b = source.GetPixel(x, y).a;
                }
                else if (xr >= 0)
                {
                    ixy = yl * halfWidth + xr;
                    col = colors[ixy];
                    col.g = source.GetPixel(x, y).a;
                }
                else
                {
                    ixy = yl * halfWidth + xl;
                    col = colors[ixy];
                    col.r = source.GetPixel(x, y).a;
                }

                colors[ixy] = col;
            }
        }

        return colors;
    }

    static TextureImporter getTextureInfo(TextureImporter sourceInfo,TextureImporter targetInfo)
    {
        var cTextureImporter = targetInfo;
        var cacheSpriteSheetDatas  = sourceInfo.spritesheet;
        var targetSpriteSheetDatas = cTextureImporter.spritesheet;
        for (int i = 0; i < targetInfo.spritesheet.Length; i++)
        {
            for (int j = 0; j < cacheSpriteSheetDatas.Length; j++)
            {
                if (targetSpriteSheetDatas[i].name == cacheSpriteSheetDatas[j].name)
                {
                    targetSpriteSheetDatas[i].border = cacheSpriteSheetDatas[j].border;
                    break;
                }
            }
        }
        return cTextureImporter;
    }

    static TextureImporter GetFileData(string targetPath)
    {
        bool isTargetTextureExist = File.Exists(targetPath);

        TextureImporter cacheTextureImporter = null;
        //对图片数据进行缓存
        if (isTargetTextureExist)
        {
            cacheTextureImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
        }

        return cacheTextureImporter;
    }

    static void WriteToFile(int width, int height, Color[] colors, TextureFormat format, string sourcePath,string targetPath, int maxSize,TextureImporter sourceInporter,bool isRGB)
    {
        Texture2D texture = null;
        Texture2D exist = (Texture2D)AssetDatabase.LoadAssetAtPath(targetPath, typeof(Texture2D));
        if (exist)
        {
            File.Delete(targetPath);
        }

        //------对源图集的数据图集txt数据进行读取 写入目标图集---
        TextAsset atlasData = getAtlasDatas(sourcePath);
        var targetSpriteSheetDatas = GetSpriteSheetDatas(atlasData, sourcePath);
        if (sourceInporter != null)
        {
            for (int z = 0; z < targetSpriteSheetDatas.Length; z++)
            {
                for (int j = 0; j < sourceInporter.spritesheet.Length; j++)
                {
                    if (targetSpriteSheetDatas[z].name == sourceInporter.spritesheet[j].name)
                    {
                        targetSpriteSheetDatas[z].border = sourceInporter.spritesheet[j].border;
                    }
                }
            }
        }

        texture = new Texture2D(width, height, format, false);
        texture.SetPixels(colors);
        byte[] stream = texture.EncodeToPNG();
        File.WriteAllBytes(targetPath, stream);
        AssetDatabase.ImportAsset(targetPath);
        texture.Apply();

        //对RGB图边缘化处理
        if (isRGB)
        {
            texture = TextureEdgeProcessing(texture);
            texture.Apply();
        }

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(targetPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
        importer.maxTextureSize = maxSize == 0 ? 2048 : maxSize;
        importer.isReadable = true;

        if (isRGB)
        {
            importer.spritesheet = targetSpriteSheetDatas;
            SetTexImportSettings(targetPath, TextureImporterFormat.RGB16, TextureImporterFormat.RGB16, TextureImporterFormat.RGB16, texture);
            TextureSolver.ConvertToDither16(texture);
        }
        else
        {
            SetTexImportSettings(targetPath, TextureImporterFormat.RGBA32, TextureImporterFormat.RGBA32, TextureImporterFormat.RGBA32,texture);
        }
        AssetDatabase.ImportAsset(targetPath);
    }

    static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static bool IsTextureConverted(string _path)
    {
        return _path.Contains("_rgb.") || _path.Contains("_alpha.");
    }

    static string[] mainTextureMatNames = new string[] { "HUD"};
    static bool IsHasMainTexture(string fileName)
    {
        foreach(var val in mainTextureMatNames)
        {
            if( fileName.IndexOf(val, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }
        }
        return false;
    }
    
    static string[] noClipMatNames = new string[] { "Fonts", "HUD", "HUD_Gulid" };
    static bool IsHasNoClip(string fileName)
    {
        fileName = fileName.ToLower().Trim();
        foreach (var val in noClipMatNames)
        {
            if (fileName == val.ToLower().Trim())
            {
                return true;
            }
        }
        return false;
    }

    static void CreateMaterial(string rgbTexturePath, string alphaTexturePath)
    {
        string materialPath = MPathUtils.UI_PUBLISH_PATH + "Atlas/Materials";

        Texture2D rgbTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(rgbTexturePath);
        Texture2D alphaTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(alphaTexturePath);

        var matPath = materialPath + "/" + rgbTexture.name + ".mat";
        var isMatExist = File.Exists(matPath);
        Shader shader;

        if (IsHasNoClip(rgbTexture.name))
        {
            shader = Shader.Find("RO/UI/HUD");
        }
        else
        {
            shader = Shader.Find("RO/UI/ETC1");
        }

        Material mat = null;
        if (isMatExist)
        {
            mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            mat.shader = shader;
        }
        else
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, matPath);
        }

        //mat.SetTexture("_MainTex", (IsHasMainTexture(rgbTexture.name)? rgbTexture : null));
        mat.SetTexture("_MainTex", rgbTexture);
        if (IsHasNoClip(rgbTexture.name))
        {
            mat.DisableKeyword("UNITY_UI_CLIP_RECT");
        }
        else
        {
            mat.EnableKeyword("UNITY_UI_CLIP_RECT");
        }
        mat.SetTexture("_AlphaTex", alphaTexture);
    }

    static void SetTexImportSettings(string path, TextureImporterFormat pc, TextureImporterFormat android, TextureImporterFormat ios,Texture2D cTex)
    {
        List<TextureImporterPlatformSettings> platformSettings = new List<TextureImporterPlatformSettings>();

        TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings();
        platformSetting.format = pc;
        platformSetting.name = "Standalone";
        platformSettings.Add(platformSetting);

        platformSetting = new TextureImporterPlatformSettings();
        platformSetting.format = android;
        platformSetting.name = "Android";
        platformSettings.Add(platformSetting);

        platformSetting = new TextureImporterPlatformSettings();
        platformSetting.format = ios;
        platformSetting.name = "iPhone";
        platformSettings.Add(platformSetting);

        Texture2D texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

        for (int i = 0; i < platformSettings.Count; i++)
        {
            platformSettings[i].overridden = true;
            platformSettings[i].maxTextureSize = Mathf.Max(cTex.width, cTex.height);
            textureImporter.SetPlatformTextureSettings(platformSettings[i]);
        }
        textureImporter.isReadable = false;
    }

    //对图片进行边缘处理
    public static Texture2D TextureEdgeProcessing(Texture2D texture2D)
    {
        if (texture2D == null)
        {
            return null;
        }

        int width = texture2D.width;
        int height = texture2D.height;
        Color[] sourcePixels = texture2D.GetPixels();
        Color[] targetPixels = texture2D.GetPixels();
        var offset = 0;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (IsColorZero(targetPixels[offset]))
                {
                    targetPixels[offset] = TextureProcess.TextureSmoothEdge(sourcePixels, width, height, offset);
                }

                offset++;
            }
        }

        Texture2D result = new Texture2D(width, height, texture2D.format, false);
        result.SetPixels(targetPixels);
        result.Apply();
        return result;
    }

    static TextAsset getAtlasDatas(string sourceTexturePath)
    {
        string textPath = Path.GetDirectoryName(sourceTexturePath) + "/" + Path.GetFileNameWithoutExtension(sourceTexturePath) + ".txt";
        return AtlasDataProcess.GetAtlasData(textPath);
    }

    //取图集数据
    public static SpriteMetaData[] GetSpriteSheetDatas(TextAsset text, string sourceTexturePath)
    {
        if (!isTexturePackerData(text))
        {
            return null;
        }

        List<SpriteMetaData> sprites = TexturePacker.ProcessToSprites(text.text);
        SpriteMetaData[] sheets = new SpriteMetaData[sprites.Count];

        for (int i = 0; i < sheets.Length; i++)
        {
            sheets[i] = sprites[i];
        }

        return sheets;
    }

    //判断数据文件是否是图集数据
    static bool isTexturePackerData(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            return false;
        }

        return (textAsset.text.hashtableFromJson()).IsTexturePackerTable();
    }

    public static bool IsColorZero(Color color)
    {
        if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0)
        {
            return true;
        }
        return false;
    }

    //图片边缘平滑
    public static Color TextureSmoothEdge(IList<Color> sourcePixels, int width, int height, int offset)
    {
        Color currentColor = ListHelper.SafeGetData(sourcePixels, offset);
        if (!IsColorZero(currentColor))
        {
            return currentColor;
        }

        //此像素的上下左右像素位置
        List<int> arounds = new List<int>();
        arounds.Add(offset + width);
        arounds.Add(offset - width);
        arounds.Add(offset - 1);
        arounds.Add(offset + 1);

        Color result = currentColor;
        int count = 0;
        for (int i = 0; i < arounds.Count; i++)
        {
            Color color = ListHelper.SafeGetData(sourcePixels, arounds[i]);
            if (!IsColorZero(color))
            {
                result += color;
                count++;
            }
        }

        if (count > 0)
        {
            result = result / count;
        }

        return result;
    }
}
