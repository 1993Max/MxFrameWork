using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using MFrameWork;
using XUPorterJSON;

//对使用TexturePacker打出的图集进行相关图片数据的设置
public static class AtlasDataProcess
{
    //对选中的图集进行设置
    //选中文件夹、选中图片和选中相应的数据文件都可以设置数据
    //图片和数据文件必须在一个文件夹中
    public static void SetAtlasDatas()
    {
        Object[] objects = Selection.objects;
        List<string> paths = getTextPaths(objects);
        disposeTexture(paths);
    }

    //获取路径，主要是获取图集数据文件路径
    static List<string> getTextPaths(Object[] objects)
    {
        List<string> paths = new List<string>();

        for (int i = 0; i < objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            if (Directory.Exists(path))
            {
                List<string> h = MFileHelper.GetFiles(path);
                MListHelper.AddExclusive(paths, h);
                continue;
            }
            if (objects[i] is Texture2D)
            {
                string directoryPath = Path.GetDirectoryName(path);
                string textureName = Path.GetFileNameWithoutExtension(path);
                string textPath = directoryPath + "/" + textureName + ".txt";
                path = textPath;
            }
            MListHelper.AddExclusive(paths, MFileHelper.GetUnityFormatPath(path));
        }
        return paths;
    }

    
    public static void disposeTexture(List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            ProcessToSprite(GetAtlasData(paths[i]));
        }
    }

    //处理图片
    static void ProcessToSprite(TextAsset text)
    {
        if (text==null)
        {
            return;
        }

        string directoryPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(text));
        TexturePacker.MetaData meta = TexturePacker.GetMetaData(text.text);
        
        string targetTexturePath = directoryPath + "/" + meta.image;

        ProcessToSprite(text, targetTexturePath, targetTexturePath);
    }

    public static void ProcessToSprite(TextAsset text, string sourceTexturePath, string targetTexturePath)
    {
        ProcessToSprite(targetTexturePath, GetSpriteSheetDatas(text, sourceTexturePath, targetTexturePath));
    }
    public static void ProcessToSprite(string targetTexturePath, SpriteMetaData[] spriteSheetDatas)
    {
        if (spriteSheetDatas==null)
        {
            return;
        }

        TextureImporter textureImporter = AssetImporter.GetAtPath(targetTexturePath) as TextureImporter;

        textureImporter.spritesheet = spriteSheetDatas;

        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;

        AssetDatabase.ImportAsset(targetTexturePath, ImportAssetOptions.ForceUpdate);
    }

    //取图集数据
    public static SpriteMetaData[] GetSpriteSheetDatas(TextAsset text,string sourceTexturePath, string targetTexturePath)
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
            sheets[i].border = getSpriteMetaDataBorder(sprites[i].name, sourceTexturePath, targetTexturePath);
        }
        
        return sheets;
    }

    //取图片中的border数据，先取目标图片的数据，取不到再取源图片的数据
    static Vector4 getSpriteMetaDataBorder(string spriteName, string sourceTexturePath, string targetTexturePath)
    {
        Vector4 border = getSpriteMetaDataBorder(spriteName, targetTexturePath);
        if (border==Vector4.zero)
        {
            border = getSpriteMetaDataBorder(spriteName, sourceTexturePath);
        }
        return border;
    }
    static Vector4 getSpriteMetaDataBorder(string spriteName, string texturePath)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        for (int i = 0; i < textureImporter.spritesheet.Length; i++)
        {
            if (textureImporter.spritesheet[i].name == spriteName)
            {
                return textureImporter.spritesheet[i].border;
            }
        }
        return Vector4.zero;
    }

    public static TextAsset GetAtlasData(string path)
    {
        TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (isTexturePackerData(text))
        {
            return text;
        }
        return null;
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
    
}