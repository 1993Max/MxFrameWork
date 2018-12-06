using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MFrameWork
{
    public static class TypeEx
    {
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        public static string DefaultValueStringForType(this Type targetType)
        {

            if (targetType.IsClass)
            {
                return "null";
            }
            else
            {
                return $"default({targetType.FullName})";
            }
        }

        /// <summary>
        /// 获取类型的简化名称
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetSimpleTypeName(string typeName)
        {
            var result = typeName;
            if (typeName.Contains("."))
            {
                result = typeName.Substring(typeName.LastIndexOf(".") + 1);
            }
            return result;
        }

        /// <summary>
        /// 获取类型名，可使用在代码中
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type)
        {
            var replace = type.FullName?.Replace('+', '.');
            return replace;
        }
    }

}
