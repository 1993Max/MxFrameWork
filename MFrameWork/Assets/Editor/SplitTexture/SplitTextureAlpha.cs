using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SplitTextureBatch
{
    [MenuItem("GameObject/Split Textures Alpha/Split Normal", false, 7)]
    public static void AutoChangeShaderO()
    {
        AutoChangeShader(Selection.activeGameObject);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:1024, a:512)", false, 7)]
    public static void AutoChangeShaderA()
    {
        AutoChangeShader(Selection.activeGameObject, 1024, 512);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:1024, a:1024)", false, 7)]
    public static void AutoChangeShaderB()
    {
        AutoChangeShader(Selection.activeGameObject, 1024, 1024);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:2048, a:1024)", false, 7)]
    public static void AutoChangeShaderC()
    {
        AutoChangeShader(Selection.activeGameObject, 2048, 1024);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:512, a:512)", false, 7)]
    public static void AutoChangeShaderD()
    {
        AutoChangeShader(Selection.activeGameObject, 512, 512);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:512, a:256)", false, 7)]
    public static void AutoChangeShaderE()
    {
        AutoChangeShader(Selection.activeGameObject, 512, 256);
    }

    [MenuItem("GameObject/Split Textures Alpha/Split (t:256, a:256)", false, 7)]
    public static void AutoChangeShaderF()
    {
        AutoChangeShader(Selection.activeGameObject, 256, 256);
    }

    [MenuItem("GameObject/Split Textures Alpha/Resume For Lightmap", false, 7)]
    public static void AutoChangeShaderResume()
    {
        Resume(Selection.activeGameObject);
    }

    static void Resume(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            var mat = renderers[i].sharedMaterial;
            if (!mat)
                continue;
            if (mat.shader.name.Equals("Mobile/Diffuse-AlphaTest") 
                || mat.shader.name.Equals("Mobile/Unlit-AlphaTest (Lightmap)")
                || mat.shader.name.Equals("Mobile/Diffuse-Alpha-Fade")) 
            {
                var rgb = mat.GetTexture("_MainTex");            
                if (rgb)
                {
                    var path = AssetDatabase.GetAssetPath(rgb).Split('.')[0];
                    if (path.EndsWith("_rgb"))
                    {            
                        var texO = (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(0, path.Length - 4) + ".png", typeof(Texture2D));
                        if (!texO) texO = (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(0, path.Length - 4) + ".tga", typeof(Texture2D));
                        if (texO)
                        {
                            mat.SetTexture("_MainTex", texO);
                        }                 
                    }                
                }
                mat.SetTexture("_AlphaTex", null);
                mat.SetFloat("_UseSplitAlpha", 0);
                mat.DisableKeyword("SPLIT");
            }
        }

        EditorUtility.DisplayDialog("Resume", "Finish", "Close");
    }


    static void AutoChangeShader(GameObject obj, int texSize = 0, int alphaSize = 0)
    {
        string path;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Renderer renderer;
        Material material;
        Texture2D texture;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderer = renderers[i];
            if (renderer.sharedMaterials != null)
            {
                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    material = renderer.sharedMaterials[j];

                    if (!material)
                        continue;

                    if (material.shader.name.Equals("Mobile/Diffuse-AlphaTest")
                        || material.shader.name.Equals("Legacy Shaders/Transparent/Cutout/Diffuse")
                        || material.shader.name.Equals("Mobile/Unlit-AlphaTest (Lightmap)")
                        || material.shader.name.Equals("Mobile/Diffuse-Alpha-Fade")
                        || material.shader.name.Equals("Legacy Shaders/Transparent/Diffuse"))
                    {
                        texture = (Texture2D)material.GetTexture("_MainTex");

                        if (texture)
                        {
                            string sourcefilePath = AssetDatabase.GetAssetPath(texture).Split('.')[0];

                            if (sourcefilePath.EndsWith("_rgb"))
                                continue;

                            if (material.shader.name.Equals("Mobile/Diffuse-AlphaTest")
                                || material.shader.name.Equals("Legacy Shaders/Transparent/Cutout/Diffuse")
                                || material.shader.name.Equals("Mobile/Unlit-AlphaTest (Lightmap)"))
                            {
                                material.shader = Shader.Find("Mobile/Diffuse-AlphaTest");
                            }
                            else if (material.shader.name.Equals("Mobile/Diffuse-Alpha-Fade")                    
                                || material.shader.name.Equals("Legacy Shaders/Transparent/Diffuse"))
                            {
                                material.shader = Shader.Find("Mobile/Diffuse-Alpha-Fade");
                            }

                            material.SetFloat("_UseSplitAlpha", 1);
                            material.EnableKeyword("SPLIT");
                            var rgb = (Texture2D)AssetDatabase.LoadAssetAtPath(sourcefilePath + "_rgb.png", typeof(Texture2D));
                            if (rgb)
                                material.SetTexture("_MainTex", rgb);
                            else
                            {
                                SplitTextureAlpha.SplitSelectTextures(texSize, alphaSize, texture);
                                rgb = (Texture2D)AssetDatabase.LoadAssetAtPath(sourcefilePath + "_rgb.png", typeof(Texture2D));
                                if (rgb)
                                    material.SetTexture("_MainTex", rgb);
                            }
                            var a = (Texture2D)AssetDatabase.LoadAssetAtPath(sourcefilePath + "_alpha.png", typeof(Texture2D));
                            if (a)
                                material.SetTexture("_AlphaTex", a);
                            EditorUtility.SetDirty(material);
                        }
                    }
                }
            }
        }

        EditorUtility.DisplayDialog("Split", "Finish", "Close");
    }
}

public class SplitTextureAlpha : EditorWindow     
{
    UnityEngine.Object[] selections;
    Vector2 scroll = Vector2.zero;

    [MenuItem("Assets/Texture Alpha Split")]
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
            if (selections[i] is Texture2D)
            {
                path = AssetDatabase.GetAssetPath(selections[i]);
                
                if (IsTextureFile(path) && !IsTextureConverted(path))
                {
                    try
                    {
                        sourcefilePath = path.Split('.')[0];
                        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                        importer.isReadable = true;
                        importer.SaveAndReimport();
                        source = AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
                        if (source && importer.DoesSourceTextureHaveAlpha())
                        {
                            colors = source.GetPixels();
                            WriteToFile(source.width, source.height, colors, TextureFormat.RGB24, sourcefilePath + "_rgb.png", texSize);

                            alphas = new Color[colors.Length];
                            for (int c = 0; c < colors.Length; c++)
                            {
                                alphas[c].r = colors[c].a;
                                alphas[c].g = 0;
                                alphas[c].b = 0;
                                alphas[c].a = 0;
                            }
                            WriteToFile(source.width, source.height, alphas, TextureFormat.RGB24, sourcefilePath + "_alpha.png", alphaSize);
                        }
                        importer.isReadable = false;
                        importer.SaveAndReimport();
                    }
                    catch(Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
                AssetDatabase.Refresh();
            }
        }
    }

    static void WriteToFile(int width, int height, Color[] colors, TextureFormat format, string path, int maxSize)
    {
        Texture2D exist = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        if (exist) return;

        Texture2D texture = new Texture2D(width, height, format, false);
        texture.SetPixels(colors);
        texture.Apply();
        byte[] stream = texture.EncodeToPNG();
        File.WriteAllBytes(path, stream);
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = maxSize == 0 ? 2048 : maxSize;
        AssetDatabase.ImportAsset(path);
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
}
