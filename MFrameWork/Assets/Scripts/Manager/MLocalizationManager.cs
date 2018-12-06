using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace MFrameWork
{
    /// <summary>
    /// 对本地文本进行反序列化
    /// </summary>
    public class MLocalizationManager : MSingleton<MLocalizationManager>
    {
        private readonly Dictionary<string, string> _stringDict = new Dictionary<string, string>();

        private static bool _hasInited = false;

        public override bool Init()
        {
            _stringDict.Clear();

            var value = MResLoader.singleton.ReadString("Localization/ChineseTable", MResLoader.SUFFIX_TXT);
            using (TextReader reader = new StringReader(value))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var kv = line.Split('\t');

                    if (kv.Length < 2)
                    {
                        MDebug.singleton.AddErrorLog($"中文表解析错误：{line},不存在tab");
                        continue;
                    }

                    if (_stringDict.ContainsKey(kv[0]))
                    {
                        MDebug.singleton.AddErrorLog($"中文表键值重复：{line}");
                        continue;
                    }

                    var key = kv[0];
                    var val = kv[1];
                    _stringDict.Add(key.Trim(), val);
                }
            }
            _hasInited = true;
            return true;
        }

        /// <summary>
        /// 获取本地化字符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (!_hasInited)
                Init();
            string res;
            if (!_stringDict.TryGetValue(key, out res))
            {
                return key;
            }
            return res;
        }

        /// <summary>
        /// 找到相应的Key
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string FindKey(string value)
        {
            if (!_hasInited)
                Init();
            foreach (var kvp in _stringDict)
            {
                if (kvp.Value == value)
                    return kvp.Key;
            }
            return null;
        }

    }
}
