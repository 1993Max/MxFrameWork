using System.Collections.Generic;

namespace MFrameWork
{
    public static class MDictionaryHelper
    {
        //得到字典中相应key的值，如果没有这个key，就返回value相应的默认值
        public static T2 DictionaryGetValue<T1, T2>(Dictionary<T1, T2> dictionary, T1 key)
        {
            T2 value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        //得到字典中相应key的值，如果没有这个key，就把这个key、value添加进字典
        public static T2 DictionaryGetValue<T1, T2>(Dictionary<T1, T2> dictionary, T1 key, T2 defaultValue)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, defaultValue);
                return defaultValue;
            }
            return dictionary[key];
        }
        //得到这个值并从字典中删除这条数据
        public static T2 DictionaryGetAndRemoveValue<T1, T2>(Dictionary<T1, T2> dictionary, T1 key)
        {
            if (dictionary.ContainsKey(key))
            {
                T2 data = dictionary[key];
                dictionary.Remove(key);
                return data;
            }
            return default(T2);
        }
        public static void SetDataIfContainKey<T1, T2>(Dictionary<T1, T2> dictionary,T2 value, T1 key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }
        public static void SetDataIfContainKeys<T1, T2>(Dictionary<T1, T2> dictionary, T2 value,params T1[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                SetDataIfContainKey(dictionary, value, keys[i]);
            }
        }

        public static void AddRange<T1, T2>(Dictionary<T1, T2> dictionary, Dictionary<T1, T2> datas)
        {
            foreach (var item in datas)
            {
                dictionary[item.Key] = item.Value;
            }
        }
    }
}

