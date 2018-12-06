using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MFrameWork
{
    public static class MFileHelper
    {
        //必定会得到一个文件夹路径
        //如果选中的有文件夹，返回第一个文件夹
        //如果没有选中文件夹但有文件，返回第一个文件所在的文件夹
        //如果什么都没有选中，返回Assets文件夹
        public static string MustGetSelectFolder()
        {
            string folderPath = "Assets";

            Object[] Assets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            if (Assets.Length > 0)
            {
                string path;
                for (int i = 0; i < Assets.Length; i++)
                {
                    path = AssetDatabase.GetAssetPath(Assets[i]);
                    if (Directory.Exists(path))
                    {
                        folderPath = path;
                    }
                }
                if (folderPath == "Assets")
                {
                    path = AssetDatabase.GetAssetPath(Assets[0]);
                    folderPath = Directory.GetParent(path).FullName;
                }
            }

            //DebugHelper.LogRed("选择的文件夹名称:" + folderPath);
            return folderPath;
        }

        //获取选中的脚本
        public static string GetSelectScript()
        {
            Object[] Assets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (Assets.Length > 0)
            {
                string path;
                for (int i = 0; i < Assets.Length; i++)
                {
                    path = AssetDatabase.GetAssetPath(Assets[i]);
                    if (!Directory.Exists(path))
                    {
                        if (MStringHelper.EndsWith(path,"cs"))
                        {
                            return path;
                        }
                    }
                }
            }
            return "";
        }

        //获取path路径下除exceptPaths以外的所有文件
        public static List<string> GetFiles(string path, List<string> exceptPaths)
        {
            List<string> files = GetFiles(path);
            for (int i = 0; i < exceptPaths.Count; i++)
            {
                if (Directory.Exists(exceptPaths[i]))
                {
                    List<string> exceptFiles = GetFiles(exceptPaths[i]);
                    MListHelper.RemoveMany(files, exceptFiles);
                }
                else
                {
                    files.Remove(exceptPaths[i]);
                }
            }
            return files;
        }

        //获取path路径下的所有文件
        public static List<string> GetFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                //DebugHelper.LogError("传递的路径有误");
                return null;
            }

            List<string> files = new List<string>();
            string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                if (Path.GetExtension(allFiles[i]) != ".meta")
                {
                    MListHelper.AddExclusive(files, GetUnityFormatPath(allFiles[i]));
                }
            }
            //for (int i = 0; i < files.Count; i++)
            //{
            //    DebugHelper.LogRed("资源名称:" + files[i]);
            //}
            return files;
        }

        //获取所有选中的文件
        //会取文件夹中的文件，不会取重复的
        public static List<string> GetSelectPath()
        {
            List<string> paths = new List<string>();
            Object[] Assets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            for (int i = 0; i < Assets.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(Assets[i]);
                if (Directory.Exists(path))
                {
                    List<string> h = GetFiles(path);
                    MListHelper.AddExclusive(paths, h);
                }
                else
                {
                    MListHelper.AddExclusive(paths, GetUnityFormatPath(path));
                }
            }
            //for (int i = 0; i < paths.Count; i++)
            //{
            //    DebugHelper.LogRed("选择的资源名称:" + paths[i]);
            //}
            return paths;
        }

        //根据Hierarchy中的GameObject，查找Project中的Prefab路径
        //如果GameObject不是预设返回""，如果GameObject不在Hierarchy中返回""
        //GameObject可以是Prefab或Prefab的子物体
        public static string GetPrefabPathWithGameObject(GameObject go)
        {
            Object parentObject = PrefabUtility.GetPrefabParent(go);
            string path = AssetDatabase.GetAssetPath(parentObject);
            //DebugHelper.LogRed("此物体的Prefab路径：" + path);
            return path;
        }

        //根据名字，查找Unity工程中所有的文件
        //name必须有扩展名
        public static string[] GetPathsWithName(string name)
        {
            string[] paths = Directory.GetFiles("Assets", name, SearchOption.AllDirectories);
            return paths;
        }

        public static string[] GetPathsWithName(Type type)
        {
            return GetPathsWithName(GetScritpNameWithType(type));
        }

        public static string GetScritpNameWithType(Type type)
        {
            return type.Name + ".cs";
        }
        public static string GetUnityFormatPath(string path)
        {
            return path.Replace('\\', '/');
        }

        public static void WriteTextureToFile(Texture2D texture, string path)
        {
            File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.Refresh();
        }
    }
}

