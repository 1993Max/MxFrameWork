using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace MFrameWork
{
    public class MSysEnvHelper
    {
        private const string ENV_CONF_NAME = "sys_env.json";
        private static readonly Dictionary<string, string> _varDict = new Dictionary<string, string>();

        public static string GetEnvParam(string name)
        {
            if (Application.isMobilePlatform)
            {
                return string.Empty;
            }
            if (!_varDict.ContainsKey(name))
            {
                string param;
                if (!getEnvParamByLocalConf(name, out param))
                {
                    param = Environment.GetEnvironmentVariable(name);
                }
                if (param != null)
                {
                    _varDict.Add(name, param);
                }
                return param ?? string.Empty;
            }

            return _varDict[name];
        }

#if !PACKAGE_MODE && DEBUG
        public static string GetEnvParamMoonClientProjPath()
        {
            return PathEx.Normalize(GetEnvParam("MoonClientProjPath"));
        }
        public static string GetEnvParamMoonClientConfigPath()
        {
            return PathEx.Normalize(GetEnvParam("MoonClientConfigPath"));
        }
        public static string GetEnvParamMoonResPath()
        {
            return PathEx.Normalize(GetEnvParam("MoonResPath"));
        }
#endif

        private static bool getEnvParamByLocalConf(string name, out string param)
        {
            var jsonFile = MFileEx.FindFileInParents(Directory.GetCurrentDirectory(), ENV_CONF_NAME);
            param = string.Empty;
            if (jsonFile == null)
            {
                return false;
            }
            var txt = jsonFile.ReadText();
            var jo = JsonMapper.ToObject(txt);
            var p = jo[name];
            if (p != null)
            {
                param = (string)p;
                return true;
            }
            return false;
        }
    }
}