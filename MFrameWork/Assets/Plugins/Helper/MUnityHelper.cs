using UnityEngine;
using System.Collections;
using System;

namespace MFrameWork
{
    public static class MUnityHelper
    {
        public static T MustGetComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component==null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static void SafeSetParent(Transform child,Transform parent)
        {
            if (MNullHelper.IsNull(child) || MNullHelper.IsNull(parent))
            {
                return;
            }

            if (!child.IsChildOf(parent))
            {
                child.SetParent(parent, false);
            }
        }

        #region SetActive

        public static void SafeSetActive(GameObject gameObject, bool isShow)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isShow);
            }
        }
        public static void SafeSetActive(MonoBehaviour component, bool isShow)
        {
            if (component != null)
            {
                SafeSetActive(component.gameObject, isShow);
            }
        }

        /// <summary>
        /// 注意第一个参数
        /// </summary>
        public static void SafeSetActiveAll(bool isShow, params GameObject[] gameObjects)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                SafeSetActive(gameObjects[i], isShow);
            }
        }

        /// <summary>
        /// 注意第一个参数
        /// </summary>
        public static void SafeSetActiveAll(bool isShow, params MonoBehaviour[] gameObjects)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                SafeSetActive(gameObjects[i], isShow);
            }
        }
        #endregion

        #region 创建物体

        //创建一个物体
        public static GameObject CreateGameObject(GameObject prefab, Transform parent)
        {
            GameObject go = UnityEngine.Object.Instantiate(prefab);
            if (parent != null)
            {
                go.transform.SetParent(parent,false);
            }
            go.SetActive(true);
            return go;
        }
        public static T CreateGameObject<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            GameObject go = CreateGameObject(prefab.gameObject, parent);
            return go.GetComponent<T>();
        }

        //如果给定的物体是空的则创建物体
        public static void CreateGameObjectIfNull(ref GameObject data, GameObject prefab, Transform parent, Action finishCreateMethod = null)
        {
            if (MNullHelper.IsNull(data))
            {
                data = CreateGameObject(prefab, parent);
                if (finishCreateMethod != null)
                {
                    finishCreateMethod();
                }
            }
        }
        public static void CreateGameObjectIfNull<T>(ref T data, T prefab, Transform parent, Action finishCreateMethod = null) where T : MonoBehaviour
        {
            if (MNullHelper.IsNull(data))
            {
                data = CreateGameObject(prefab, parent);
                if (finishCreateMethod != null)
                {
                    finishCreateMethod();
                }
            }
        }

        #endregion
    }
}
