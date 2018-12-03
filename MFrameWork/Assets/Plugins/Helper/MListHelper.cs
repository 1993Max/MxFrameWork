using System.Collections.Generic;
using System;

namespace MFrameWork
{
    public static class MListHelper
    {

        //如果不包含则添加
        public static void AddExclusive<T>(List<T> list,T data)
        {
            if (!list.Contains(data))
            {
                list.Add(data);
            }
        }
        public static void AddExclusive<T>(List<T> list, IList<T> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                AddExclusive(list, data[i]);
            }
        }

        //对列表中的相应的数据进行替换
        public static bool Replace<T>(IList<T> list, T oldData, T newData)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (MNullHelper.IsEqual(list[i], oldData))
                {
                    list[i] = newData;
                    return true;
                }
            }
            return false;
        }

        //对列表中的相应的数据进行替换,如果没有可替换的就添加这条数据
        public static void AddOrReplace<T>(List<T> list, T oldData, T newData)
        {
            if (!Replace(list, oldData, newData))
            {
                list.Add(newData);
            }
        }

        //移除list中与data相同的所有数据
        public static void RemoveMany<T>(List<T> list, IList<T> data)
        {
            if (MNullHelper.IsNullOrEmpty(list)|| MNullHelper.IsNullOrEmpty(data))
            {
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (MNullHelper.IsEqual(list[j], data[i]))
                    {
                        list.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        //有序插入
        //默认list中的数据已经是有序的
        public static void OrderlyInserted<T>(List<T> list, T data, bool isSequence = true) where T : IComparable<T>
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (isSequence ? list[i].CompareTo(data) > 0 : list[i].CompareTo(data) < 0)
                {
                    list.Insert(i,data);
                    return;
                }
            }
            list.Add(data);
        }

        //快速移除数据，会打乱list中数据的顺序
        public static void QuickRemove<T>(List<T> list, T data)
        {
            int index = list.IndexOf(data);
            QuickRemoveAt(list, index);
        }

        public static void QuickRemoveAt<T>(List<T> list, int index)
        {
            if (index < list.Count && index >= 0)
            {
                list[index] = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }
        }

        public static T SafeGetData<T>(IList<T> list, int index)
        {
            int count = list.Count;
            if (index < 0 || index >= count)
            {
                return default(T);
            }
            return list[index];
        }

        public static void SafeSetData<T>(IList<T>list,int index,T data)
        {
            int count = list.Count;
            if (index<0|| index>=count)
            {
                return;
            }
            list[index] = data;
        }
    }
}

