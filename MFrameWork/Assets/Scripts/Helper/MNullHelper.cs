using System.Collections.Generic;

namespace MFrameWork
{
    //与Null有关的处理
    public static class MNullHelper
    {
        //判断是否是null或是空
        public static bool IsNullOrEmpty<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return true;
            }
            return false;
        }
        public static bool IsNullOrEmpty<T1, T2>(Dictionary<T1, T2> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return true;
            }
            return false;
        }

        //判断是否相等,此方法兼容了一个或两个是null的情况和一个或两个是Unity GameObject "null"的情况
        public static bool IsEqual<T>(T objectA, T objectB)
        {
            if (ReferenceEquals(objectA, null))
            {
                if (ReferenceEquals(objectB, null))
                {
                    return true;
                }
                return objectB.Equals(null);
            }
            return objectA.Equals(objectB);
        }

        //判断是否为空,此方法兼容了null和Unity GameObject "null"
        public static bool IsNull<T>(T objectA)
        {
            if (ReferenceEquals(objectA, null))
            {
                return true;
            }
            return objectA.Equals(null);
        }

        //判断是否包含null
        //当unity的gameobject被销毁时，类有引用时并不会销毁类而是把名字改成了"null"
        public static bool IsContainNullOrUnityNull<T>(IList<T> list) where T : class
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (IsNull(list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        //移除所有的null和Unity "null"
        public static void RemoveAllNull<T>(List<T> list) where T : class
        {
            if (IsNullOrEmpty(list))
            {
                return;
            }
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (IsNull(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }
        public static T[] RemoveAllNull<T>(T[] array) where T : class
        {
            if (IsNullOrEmpty(array))
            {
                return array;
            }
            List<T> list = new List<T>(array);
            RemoveAllNull(list);
            return list.ToArray();
        }

    }
}

