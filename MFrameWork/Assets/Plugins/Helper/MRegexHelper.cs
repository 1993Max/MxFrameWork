using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MFrameWork
{
    public static class MRegexHelper
    {
        #region 用实例方法实现Regex中的静态方法
        //经测试正则表达式静态方法不如实例方法效率高
        public static bool IsMatch(string data, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(data);
        }
        public static Match Match(string data, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.Match(data);
        }
        public static MatchCollection Matches(string data, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.Matches(data);
        }
        public static string Replace(string data, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(data, replacement);
        }
        #endregion

        // 检测字符串中是否包含符合正则的子集
        public static bool IsContains(string data, string pattern)
        {
            return Match(data, pattern).Success;
        }

        public static int GetMatchStartIndex(string data, string pattern)
        {
            Match match = Match(data, pattern);
            if (match.Index == 0)
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    return MCommonHelper.IsFalse;
                }
            }
            return match.Index;
        }

        public static int GetMatchEndIndex(string data, string pattern)
        {
            Match match = Match(data, pattern);
            if (match.Index==0)
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    return MCommonHelper.IsFalse;
                }
            }
            int index = match.Index + match.Length;
            return index;
        }

        public static string Remove(string data, string pattern)
        {
            Match match = Match(data, pattern);
            string newData= data.Remove(match.Index, match.Length);
            return newData;
        }

        // 从指定字符串中得到第一个符合正则匹配的子集
        public static string GetFirstString(string data, string pattern)
        {
            return Match(data, pattern).Groups[0].Value;
        }

        //从指定字符串中得到所有符合正则匹配的子集
        public static List<string> GetString(string data, string pattern)
        {
            var matches = Matches(data, pattern);
            List<string> list = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                list.Add(matches[i].Value);
            }
            return list;
        }

        public static bool IsInt(string data)
        {
            return IsMatch(data, @"^-?\d+$");
        }

        public static bool IsFloat(string data)
        {
            return IsMatch(data, @"^-?\d+.\d+$");
        }

        /// 从指定字符串中取出第一个数字
        public static string GetFirstNumber(string data)
        {
            return Match(data, @"\d+").Groups[0].Value;
        }

        /// <summary>
        /// 从指定字符串中过滤出最后一个数字
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串的最后一个数字</returns>
        public static string GetLastNumber(string data)
        {
            var reg = Matches(data, @"\d+");
            return reg[reg.Count - 1].Value;
        }

        /// <summary>
        /// 从指定字符串中过滤出所有数字
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串的所有数字</returns>
        public static List<string> GetAllNumber(string data)
        {
            var reg = Matches(data, @"\d+");
            List<string> list = new List<string>();
            foreach (Match item in reg)
            {
                list.Add(item.Value);
            }

            return list;
        }

        /// <summary>
        /// 检车源字符串中是否包含数字
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>true:源字符串包含数字;false:源字符串不包含数字</returns>
        public static bool IsContainNumber(string data)
        {
            return Match(data, @"\d").Success;
        }

        /// <summary>
        /// 判断字符串是否全部是数字且长度等于指定长度
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <param name="length">指定长度</param>
        /// <returns>返回值</returns>
        public static bool IsMatchNumberAndLength(string data, int length)
        {
            return IsMatch(data, @"^\d{" + length + "}$");
        }

        /// <summary>
        /// 截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <param name="startString">开始字符串</param>
        /// <param name="endString">结束字符串</param>
        /// <returns>中间字符串</returns>
        public static string Substring(string data, string startString, string endString)
        {
            Regex rg = new Regex("(?<=(" + startString + "))[.\\s\\S]*?(?=(" + endString + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(data).Value;
        }

        /// <summary>
        /// 匹配邮箱是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>匹配结果true是邮箱反之不是邮箱</returns>
        public static bool IsEmail(string data)
        {
            Regex rg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", RegexOptions.IgnoreCase);
            return rg.IsMatch(data);
        }

        /// <summary>
        /// 匹配URL是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>匹配结果true是URL反之不是URL</returns>
        public static bool IsURL(string data)
        {
            Regex rg = new Regex(@"^(https?|s?ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$", RegexOptions.IgnoreCase);
            return rg.IsMatch(data);
        }

        /// <summary>
        /// 匹配日期是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>匹配结果true是日期反之不是日期</returns>
        public static bool IsDateTime(string data)
        {
            string pattern =@"^(\d{4}[\/\-](0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31))|((0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31)[\/\-]\d{4})$";
            return IsMatch(data, pattern);
        }

        /// <summary>
        /// 从字符串中获取第一个日期
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串中的第一个日期</returns>
        public static string GetFirstDateTime(string data)
        {
            string pattern =@"(\d{4}[\/\-](0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31))|((0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31)[\/\-]\d{4})";
            return Match(data, pattern).Groups[0].Value;
        }

        /// <summary>
        /// 从字符串中获取所有的日期
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串中的所有日期</returns>
        public static List<string> GetAllDateTime(string data)
        {
            string pattern =@"(\d{4}[\/\-](0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31))|((0?[1-9]|1[0-2])[\/\-]((0?[1-9])|((1|2)[0-9])|30|31)[\/\-]\d{4})";
            var all = Matches(data, pattern);
            List<string> list = new List<string>();
            foreach (Match item in all)
            {
                list.Add(item.Value);
            }
            return list;
        }

        /// <summary>
        /// 检测密码复杂度是否达标：密码中必须包含字母、数字、特称字符，至少8个字符，最多16个字符。
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>密码复杂度是否达标true是达标反之不达标</returns>
        public static bool IsPassword(string data)
        {
            return IsMatch(data, @"^(?=.*\d)(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{8,16}$");
        }

        /// <summary>
        /// 匹配邮编是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>邮编合法返回true,反之不合法</returns>
        public static bool IsPostcode(string data)
        {
            return IsMatch(data, @"^\d{6}$");
        }

        /// <summary>
        /// 匹配电话号码是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>电话号码合法返回true,反之不合法</returns>
        public static bool IsTelephone(string data)
        {
            return IsMatch(data, @"^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$");
        }

        /// <summary>
        /// 从字符串中获取电话号码
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串中电话号码</returns>
        public static string GetTelephone(string data)
        {
            return Match(data, @"(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}").Groups[0].Value;
        }

        /// <summary>
        /// 匹配手机号码是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>手机号码合法返回true,反之不合法</returns>
        public static bool IsMobilephone(string data)
        {
            return IsMatch(data, @"^[1]+[3,5,7,8]+\d{9}$");
        }

        /// <summary>
        /// 从字符串中获取手机号码
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串中手机号码</returns>
        public static string GetMobilephone(string data)
        {
            return Match(data, @"[1]+[3,5,7,8]+\d{9}").Groups[0].Value;
        }

        /// <summary>
        /// 匹配身份证号码是否合法
        /// </summary>
        /// <param name="data">待匹配字符串</param>
        /// <returns>身份证号码合法返回true,反之不合法</returns>
        public static bool IsIdentityCard(string data)
        {
            return IsMatch(data, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 从字符串中获取身份证号码
        /// </summary>
        /// <param name="data">源字符串</param>
        /// <returns>源字符串中身份证号码</returns>
        public static string GetIdentityCard(string data)
        {
            return Match(data, @"(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))").Groups[0].Value;
        }
    }

}


