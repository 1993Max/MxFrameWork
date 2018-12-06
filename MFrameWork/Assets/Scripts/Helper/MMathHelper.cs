using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public static class MMathHelper
    {
        //是否为偶数，是返回true，否返回false
        public static bool IsEven(int number)
        {
            return (number & 1) == 0;
        }

        //2的n次方
        public static int PowByTwo(int n)
        {
            return (int)Mathf.Pow(2, n);
        }

        //container是否包含containee中的所有位
        public static bool IsIntContainInt(int container, int containee)
        {
            return (container & containee) == containee;
        }

        //随机，不包含min和max
        public static float RandomExclusive(float min, float max)
        {
            float value = Random.Range(min, max);
            if (value == min)
            {
                value += float.Epsilon;
            }
            else if (value == max)
            {
                value -= float.Epsilon;
            }
            return value;
        }

        //随机0到1之间的值，不包含0和1
        public static float RandomValueExclusive()
        {
            return RandomExclusive(0f, 1f);
        }

        //根据传入的概率数组中的概率，随机出一个数值，返回相对应数值的索引值
        public static int RandomGetIndexInPercent(IList<float> percent)
        {
            if (MNullHelper.IsNullOrEmpty(percent))
            {
                return MCommonHelper.IsFalse;
            }

            float totalPercent = 0f;
            for (int i = 0; i < percent.Count; i++)
            {
                totalPercent += percent[i];
            }

            if (totalPercent <= 0f)
            {
                return MCommonHelper.IsFalse;
            }

            float random = RandomExclusive(0, totalPercent);

            float proportion = 0f;
            for (int i = 0; i < percent.Count; i++)
            {
                proportion += percent[i];
                if (random <= proportion)
                {
                    return i;
                }
            }
            return MCommonHelper.IsFalse;
        }

        public static bool IsApproximately(float valueA, float valueB, float distanceValue)
        {
            return Mathf.Abs(valueB - valueA) < distanceValue;
        }
        public static bool IsApproximately(Vector2 valueA, Vector2 valueB, float distanceValue)
        {
            return IsApproximately(valueA.x, valueB.x, distanceValue) && IsApproximately(valueA.y, valueB.y, distanceValue);
        }
    }
}

