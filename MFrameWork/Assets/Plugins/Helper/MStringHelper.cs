using System.Collections.Generic;

namespace MFrameWork
{
    public static class MStringHelper
    {
        public static void StringBuilderAppend(System.Text.StringBuilder source, params string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                source.Append(data[i]);
            }
        }
        public static bool StartsWith(string data, string value)
        {
            if (data.Length< value.Length)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (data[i]!= value[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EndsWith(string data, string value)
        {
            if (data.Length < value.Length)
            {
                return false;
            }

            int dataMaxIndex = data.Length - 1;
            int valueMaxIndex = value.Length - 1;

            for (int i = 0; i < value.Length; i++)
            {
                if (data[dataMaxIndex-i] != value[valueMaxIndex-i])
                {
                    return false;
                }
            }
            return true;
        }

        

        //得到字符串中某个字符的个数
        public static int GetCharCountInString(string data, char containee)
        {
            int h = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == containee)
                {
                    h++;
                }
            }
            return h;
        }

        #region 安全字符串操作，不会报错

        //data是总字符串，startIndex开始序号，values要判断的所有字符串
        //返回找到的values中第一个匹配的索引
        public static int SafeGetIndex(string data, int startIndex, params string[] values)
        {
            int index = MCommonHelper.IsFalse;
            for (int i = 0; i < values.Length; i++)
            {
                index = SafeGetIndex(data, startIndex, values[i]);
                if (index != MCommonHelper.IsFalse)
                {
                    break;
                }
            }
            return index;
        }
        public static int SafeGetIndex(string data, params string[] values)
        {
            return SafeGetIndex(data, 0, values);
        }
        //data是总字符串，startIndex开始序号，value要判断的字符串
        public static int SafeGetIndex(string data, int startIndex, string value)
        {
            int index;
            if (startIndex<0|| startIndex> data.Length)
            {
                return MCommonHelper.IsFalse;
            }
            index = data.IndexOf(value, startIndex, System.StringComparison.Ordinal);
            if (index < 0)
            {
                index = MCommonHelper.IsFalse;
            }
            return index;
        }

        public static int SafeGetIndex(string data, string value)
        {
            return SafeGetIndex(data, 0, value);
        }

        public static int SafeGetIndex(string data, int startIndex, char value)
        {
            int index;
            if (startIndex < 0 || startIndex > data.Length)
            {
                return MCommonHelper.IsFalse;
            }
            index = data.IndexOf(value, startIndex);
            if (index < 0)
            {
                index = MCommonHelper.IsFalse;
            }
            return index;
        }

        public static int SafeGetIndex(string data, char value)
        {
            return SafeGetIndex(data, 0, value);
        }

        //得到data中从startIndex到endIndex所有value的索引
        //包含startIndex和endIndex
        public static List<int> SafeGetIndexs(string data, int startIndex,int endIndex, string value)
        {
            List<int> indexs = new List<int>();
            int index= startIndex;
            for (;;)
            {
                index = SafeGetIndex(data, index, value);
                if (index == MCommonHelper.IsFalse || index > endIndex)
                {
                    break;
                }
                indexs.Add(index);
                index++;
            }
            return indexs;
        }

        //安全插入，startIndex超出界限不插入
        public static string SafeInsert(string data, int startIndex, string value)
        {
            if (startIndex < 0 || startIndex > data.Length)
            {
                //DebugHelper.LogRed("startIndex超出界限，插入失败");
                return data;
            }
            return data.Insert(startIndex, value);
        }

        #endregion

        //取一对字符的索引
        //一对字符即形式为"pair1****pair2"样式的字符串，****为任意字符串
        //此索引为一对字符中第二个字符的索引
        public static int GetIndexWithCharPair(string data, int startIndex, char pair1, char pair2)
        {
            //如果两个字符相同即取此字符的索引
            if (pair1 == pair2)
            {
                return SafeGetIndex(data, startIndex, pair1);
            }
            int pair1Count = 0;
            int pair2Count = 0;
            for (int i = startIndex; i < data.Length; i++)
            {
                //取到的第一个字符的个数
                if (data[i] == pair1)
                {
                    pair1Count++;
                }
                else if (data[i] == pair2)
                {
                    //只有取到第一个字符才会开始取第二个字符
                    if (pair1Count != 0)
                    {
                        pair2Count++;
                    }
                }
                if (pair1Count == 0 && pair2Count == 0)
                {
                    continue;
                }
                //当取到的两个字符个数相同时，说明已经匹配到此字符对
                if (pair1Count == pair2Count)
                {
                    return i;
                }
            }
            return MCommonHelper.IsFalse;
        }

        public static int GetIndex(string data,char value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i]==value)
                {
                    return i;
                }
            }
            return MCommonHelper.IsFalse;
        }

        //给一个很长的字符串添加换行符
        //maxPerLine一行字符数
        //truncateString换行需要检索的字符串
        public static string StringLinefeed(string data, int maxPerLine, params string[] truncateString)
        {
            int lineCount = data.Length / maxPerLine;
            for (int i = 0; i < lineCount; i++)
            {
                int index = SafeGetIndex(data, (i + 1) * maxPerLine, truncateString);
                //DebugHelper.Log("index:" + index);
                if (index == MCommonHelper.IsFalse)
                {
                    index = (i + 1) * maxPerLine;
                }

                index += 1;
                if (index < data.Length)
                {
                    data = data.Insert(index, "\n");
                }
            }
            return data;
        }

    }
}

