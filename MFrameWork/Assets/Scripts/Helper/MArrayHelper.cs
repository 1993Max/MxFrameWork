using System.Collections.Generic;

namespace MFrameWork
{
    public static class MArrayHelper
    {
        //移除数组中的一个元素
        public static T[] ArrayRemove<T>(T[] array, T data)
        {
            List<T> list = new List<T>(array);
            list.Remove(data);
            return list.ToArray();
        }
        //移除数组中指定位置的一个元素
        public static T[] ArrayRemoveAt<T>(T[] array, int index)
        {
            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            return list.ToArray();
        }

        public static bool IsArrayContain<T>(T[] array, T data)
        {
            if (array==null)
            {
                return false;
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (MNullHelper.IsEqual(array[i], data))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsArrayEquel<T>(T[] array, IList<T> list)
        {
            if (array.Length != list.Count)
            {
                return false;
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (!MNullHelper.IsEqual(array[i], list[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static T[] Add<T>(T[] array, T data)
        {
            List<T> list;
            if (array == null)
            {
                list = new List<T>();
            }
            else
            {
                list = new List<T>(array);
            }
            list.Add(data);
            return list.ToArray();
        }

    }
}

