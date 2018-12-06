using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    public static class ComponentEx
    {
        /// <summary>
        /// 获取或者创建组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetOrCreateComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }

        /// <summary>
        /// 在父节点中找到Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetComponentInParent<T>(this GameObject go) where T : Component
        {
            if (go.transform.parent == null)
                return null;

            var parent = go.transform.parent.gameObject;
            var res = parent.GetComponent<T>();
            if (res == null)
            {
                return GetComponentInParent<T>(parent);
            }
            else
            {
                return res;
            }
        }

        public static T CopyComponent<T>(this GameObject org, GameObject des) where T : Component
        {
            T orgComp = org.GetComponent<T>();
            if(!orgComp)
            {
                return null;
            }
            T desComp= des.GetComponent<T>();
            if(!desComp)
            {
                desComp = des.AddComponent<T>();
            }
            System.Type type = orgComp.GetType();
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(desComp, field.GetValue(orgComp));
            }
            return desComp;
        }
    }

}
