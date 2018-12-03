using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace MFrameWork
{
    public static class MCopyHelper
    {
        #region 不能用于MonoBehaviour(只要是需要实例化的，都不能用于MonoBehaviour)

        //浅复制
        public static T SuperficialCopy<T>(T data) where T : new()
        {
            T h = new T();
            FieldInfo[] fieldFroms = typeof(T).GetFields();
            for (int i = 0; i < fieldFroms.Length; i++)
            {
                fieldFroms[i].SetValue(h, fieldFroms[i].GetValue(data));
            }
            return h;
        }

        //对两个不同的类中名字相同的字段进行复制
        public static T2 SuperficialCopyWithName<T1, T2>(T1 from) where T2 : new()
        {
            FieldInfo[] fieldFroms = from.GetType().GetFields();
            FieldInfo[] fieldTos = typeof(T2).GetFields();
            T2 to = new T2();
            for (int i = 0; i < fieldFroms.Length; i++)
            {
                for (int j = 0; j < fieldTos.Length; j++)
                {
                    if (fieldTos[j].Name != fieldFroms[i].Name || fieldTos[j].FieldType != fieldFroms[i].FieldType)
                    {
                        continue;
                    }
                    fieldTos[j].SetValue(to, fieldFroms[i].GetValue(from));
                    break;
                }
            }
            return to;
        }

        //深度复制
        //只能复制字段
        //考虑到属性赋值可能会进行一些操作，为了不造成潜在的问题，所以不进行属性的赋值
        //data或data中的数据不能有继承于MonoBehaviour的类
        public static object DeepCopy(object data)
        {
            Type type = data.GetType();

            object instantiation = GetInstantiationWithType(type);

            FieldInfo[] fieldInfos = data.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                object info = fieldInfos[i].GetValue(data);
                if (info != null)
                {
                    if (info.GetType().IsValueType || info is string)
                    {
                        fieldInfos[i].SetValue(instantiation, info);
                    }
                    else
                    {
                        fieldInfos[i].SetValue(instantiation, DeepCopy(info));
                    }
                }
            }
            return instantiation;
        }

        //根据Type得到类的实例，必须有无参构造方法
        public static object GetInstantiationWithType(Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                return null;
            }
            return constructorInfo.Invoke(null);
        }

        #endregion

        #region 可用于MonoBehaviour

        public static void SuperficialCopy<T>(T from, T to)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].SetValue(to, fields[i].GetValue(from));
            }
        }

        public static void SuperficialCopyWithName<T1, T2>(T1 from, T2 to)
        {
            //利用反射获得类成员
            FieldInfo[] fieldFroms = from.GetType().GetFields();
            FieldInfo[] fieldTos = to.GetType().GetFields();
            for (int i = 0; i < fieldFroms.Length; i++)
            {
                for (int j = 0; j < fieldTos.Length; j++)
                {
                    if (fieldTos[j].Name != fieldFroms[i].Name || fieldTos[j].FieldType != fieldFroms[i].FieldType)
                    {
                        continue;
                    }
                    fieldTos[j].SetValue(to, fieldFroms[i].GetValue(from));
                    break;
                }
            }
        }
        #endregion
    }
}

