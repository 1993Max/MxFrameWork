using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;

namespace MoonCommonLib
{
    public sealed class MResLoader : MSingleton<MResLoader>
    {
        private const int SHARED_STREAM_SIZE = 512 * 1024;
        private const int FILE_BYTES_SIZE = 64 * 1024;

        private MemoryStream _sharedStream = new MemoryStream(SHARED_STREAM_SIZE);
        private byte[] _fileBytes = new byte[FILE_BYTES_SIZE];

        public const string SUFFIX_TXT = ".txt";
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="location"></param>
        /// <param name="suffix"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ReadString(string location, string suffix, bool errNonexist = true)
        {
            if (!ReadBytes(location, suffix, _sharedStream))
            {
                if (errNonexist)
                {
                    MDebug.singleton.AddErrorLog($"read stream error location={location}{suffix}");
                }
                return string.Empty;
            }
            return Encoding.UTF8.GetString(_sharedStream.GetBuffer(), 0, (int)_sharedStream.Length);
        }

        /// <summary>
        /// 同步读取二进制, 并写入到stream里
        /// </summary>
        public bool ReadBytes(string location, string suffix, MemoryStream stream)
        {
            //编辑器下直接读取工程
            string path = string.Format("{0}/Assets/Resources/{1}{2}", MSysEnvHelper.GetEnvParam("MoonClientConfigPath"), location, suffix);
            if (!File.Exists(path))
            {
                return false;
            }

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ReadFromFile(fs, stream);
            }
        }

        public bool ReadFromFile(FileStream fs, MemoryStream stream)
        {
            try
            {
                stream.Position = 0;
                stream.SetLength(0);

                fs.Position = 0;
                int readSize = 0;
                while ((readSize = fs.Read(_fileBytes, 0, _fileBytes.Length)) > 0)
                {
                    stream.Write(_fileBytes, 0, readSize);
                }
                stream.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch (Exception e)
            {
                MDebug.singleton.AddErrorLog(e.StackTrace);
                return false;
            }
        }
    }
}