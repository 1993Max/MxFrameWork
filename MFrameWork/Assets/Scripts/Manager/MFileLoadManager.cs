// **************************************
//
// 文件名(MFileLoadManager.cs.cs):
// 功能描述("文件读取管理器"):
// 作者(Max1993):
// 日期(2019/5/2  19:21):
//
// **************************************
//
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;

namespace MFrameWork
{
    public sealed class MFileLoadManager : MSingleton<MFileLoadManager>
    {
        private const int SHARED_STREAM_SIZE = 512 * 1024;
        private const int FILE_BYTES_SIZE = 64 * 1024;

        private MemoryStream m_sharedStream = new MemoryStream(SHARED_STREAM_SIZE);
        private byte[] m_fileBytes = new byte[FILE_BYTES_SIZE];
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
            if (!ReadBytes(location, suffix, m_sharedStream))
            {
                if (errNonexist)
                {
                    MDebug.singleton.AddErrorLog($"read stream error location={location}{suffix}");
                }
                return string.Empty;
            }
            return Encoding.UTF8.GetString(m_sharedStream.GetBuffer(), 0, (int)m_sharedStream.Length);
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
                while ((readSize = fs.Read(m_fileBytes, 0, m_fileBytes.Length)) > 0)
                {
                    stream.Write(m_fileBytes, 0, readSize);
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
