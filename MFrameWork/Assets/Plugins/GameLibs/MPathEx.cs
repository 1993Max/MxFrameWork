using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MFrameWork
{
    public class PathEx
    {
        /// <summary>
        /// 工程根目录
        /// </summary>
        public static string projectPath
        {
            get
            {
                DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
                return MakePathStandard(directory.Parent.FullName);
            }
        }

        /// <summary>
        /// 使目录存在,Path可以是目录名必须是文件名
        /// </summary>
        /// <param name="path"></param>
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }

        /// <summary>
        /// 使目录存在
        /// </summary>
        /// <param name="path"></param>
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 如果目录存在，清空文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectorySafely(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

        }

        public static void DeleteFileSafely(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 结合目录
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            string result = "";
            foreach (string path in paths)
            {
                result = Path.Combine(result, path);
            }
            result = MakePathStandard(result);
            return result;
        }

        /// <summary>
        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathParentFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 将绝对路径转换为相对于Asset的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertAbstractToAssetPath(string path)
        {
            path = MakePathStandard(path);
            return MakePathStandard(path.Replace(projectPath + "/", ""));
        }

        /// <summary>
        /// 将绝对路径转换为相对于Asset的路径且去除后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertAbstractToAssetPathWithoutExtention(string path)
        {
            return MFileEx.GetFilePathWithoutExtention(ConvertAbstractToAssetPath(path));
        }

        /// <summary>
        /// 将相对于Asset的路径转换为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertAssetPathToAbstractPath(string path)
        {
            path = MakePathStandard(path);
            return Combine(projectPath, path);
        }

        /// <summary>
        /// 将相对于Asset的路径转换为绝对路径且去除后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertAssetPathToAbstractPathWithoutExtention(string path)
        {
            return MFileEx.GetFilePathWithoutExtention(ConvertAssetPathToAbstractPath(path));
        }

        /// <summary>
        /// 使路径标准化，去除空格并将所有'\'转换为'/'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }

        /// <summary>
        /// 去除‘..’用路径替换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Normalize(string path)
        {
            var normalized = path;
            normalized = Regex.Replace(normalized, @"/\./", "/");
            if (normalized.Contains(".."))
            {
                var list = new List<string>();
                var paths = normalized.Split('/');
                foreach (var name in paths)
                {
                    // 首位是".."无法处理的
                    if (name.Equals("..") && list.Count > 0)
                        list.RemoveAt(list.Count - 1);
                    else
                        list.Add(name);
                }
                normalized = "";// list.Join("/");
            }
            if (path.Contains("\\"))
            {
                normalized = normalized.Replace("\\", "/");
            }
            return normalized;
        }

        public static string GetShortName(string path)
        {
            var standardPath = MakePathStandard(path);
            int lastIndex = standardPath.LastIndexOf("/");
            if (lastIndex >= 0)
            {
                return standardPath.Substring(lastIndex + 1);
            }
            else
            {
                return standardPath;
            }
        }

        // <summary>
        /// StreamingAsset文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="forWWW"></param>
        /// <returns></returns>
        public static string GetStreamingAssetsFile(string path, bool forWWW = true)
        {
            string filePath = null;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    if (forWWW)
                        filePath = $"jar:file://{Application.dataPath}!/assets/{path}";
                    else
                        filePath = $"{Application.dataPath}!assets/{path}";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    if (forWWW)
                        filePath = $"file://{Application.dataPath}/Raw/{path}";
                    else
                        filePath = $"{Application.dataPath}/Raw/{path}";
                    break;
                default:
                    if (forWWW)
                        filePath = $"file://{Application.dataPath}/StreamingAssets/{path}";
                    else
                        filePath = $"{Application.dataPath}/StreamingAssets/{path}";
                    break;
            }
            return filePath;
        }

        public static string GetResourcesAssetsPath()
        {
            return Path.Combine(Application.dataPath, "Resources/");
        }

        public static string GetFileNamePathWithoutExtension(string path)
        {
            int len = path.LastIndexOf('.');
            if (-1 == len)
            {
                return path;
            }
            else
            {
                return path.Substring(0, len);
            }
        }

        // <summary>
        /// StreamingAsset文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="forWWW"></param>
        /// <returns></returns>
        public static string GetStreamingAssetsPath(bool forWWW = false)
        {
            string filePath = null;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    if (forWWW)
                        filePath = $"jar:file://{Application.dataPath}!/assets/";
                    else
                        filePath = $"{Application.dataPath}!assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    if (forWWW)
                        filePath = $"file://{Application.dataPath}/Raw/";
                    else
                        filePath = $"{Application.dataPath}/Raw/";
                    break;
                default:
                    if (forWWW)
                        filePath = $"file://{Application.dataPath}/StreamingAssets/";
                    else
                        filePath = $"{Application.dataPath}/StreamingAssets/";
                    break;
            }
            return filePath;
        }
    }

}