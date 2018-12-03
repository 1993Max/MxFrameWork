using System.Text;
using UnityEngine;
using System.Collections.Generic;
using MFrameWork;

namespace MFrameWork
{
    public static class MFindHelper
    {
        //查找物体，可查找根节点隐藏物体
        //效率超级低
        public static GameObject FindGameObject(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            path = MCommonHelper.GetUnityFormatPath(path);
            var h = path.Split('/');
            string gameObjectName = h[h.Length - 1];
            var allGameObjects= Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < allGameObjects.Length; i++)
            {
                if (allGameObjects[i].name== gameObjectName)
                {
                    string gameObjectPath = GetGameObjectPath(allGameObjects[i].transform);
                    if (gameObjectPath==path)
                    {
                        return allGameObjects[i];
                    }
                }
            }
            return null;
        }

        //查找子物体
        //此方法支持物体名称查找、全部或部分路径查找
        //查找方式为：先序遍历物体树状结构,返回找到的第一个
        public static Transform FindChildrenTransformRecursion(Transform root, string targetName)
        {
            Transform targetTransform;
            targetTransform = root.Find(targetName);
            if (MNullHelper.IsNull(targetTransform))
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    targetTransform = FindChildrenTransformRecursion(child, targetName);
                    if (!MNullHelper.IsNull(targetTransform))
                    {
                        break;
                    }
                }
            }
            return targetTransform;
        }

        //查找所有名字相同的子物体
        //此查找方法比较消耗性能
        //此方法只支持物体名称查找，不支持路径查找
        public static List<Transform> FindChildrenTransformsRecursion(Transform root, string targetName)
        {
            List<Transform> targetTransforms = new List<Transform>();

            if (!MNullHelper.IsNull(root) && !string.IsNullOrEmpty(targetName))
            {
                Transform child;
                for (int i = 0; i < root.childCount; i++)
                {
                    child = root.GetChild(i);
                    if (child.name == targetName)
                    {
                        targetTransforms.Add(child);
                    }
                    targetTransforms.AddRange(FindChildrenTransformsRecursion(child, targetName));
                }
            }

            return targetTransforms;
        }

        //搜索父物体，直到找到包含组件T的GameObject，返回T
        //可查找隐藏的
        static public T GetParentComponent<T>(Transform go, Transform endTransform = null,bool isContainEnd=true) where T : Component
        {
            if (go == null)
            {
                return null;
            }
            Transform currentTransform = go.parent;
            T currentComponent =null;
            for (; ; )
            {
                if (currentTransform==null)
                {
                    break;
                }
                currentComponent = currentTransform.GetComponent<T>();
                if (currentComponent!=null)
                {
                    break;
                }

                Transform parent = currentTransform.parent;
                if (parent == null)
                {
                    break;
                }

                if (isContainEnd)
                {
                    if (currentTransform == endTransform)
                    {
                        break;
                    }
                }
                else
                {
                    if (parent == endTransform)
                    {
                        break;
                    }
                }
                
                currentTransform = parent;
            }
            return currentComponent;
        }

        //得到所有的父节点
        public static Stack<Transform> GetAllParentNode(Transform go,Transform endTransform=null, bool isContainEnd = true)
        {
            Stack<Transform> parentTransforms = new Stack<Transform>();
            if (MNullHelper.IsNull(go))
            {
                return parentTransforms;
            }
            for (; ; )
            {
                go = go.parent;
                if (go==null)
                {
                    return parentTransforms;
                }

                if (go== endTransform)
                {
                    if (isContainEnd)
                    {
                        parentTransforms.Push(go);
                    }
                    return parentTransforms;
                }
                
                parentTransforms.Push(go);
            }
        }

        //得到物体在场景中的路径
        public static string GetGameObjectPath(Transform transform, Transform endTransform = null, bool isContainEnd = true)
        {
            if (MNullHelper.IsNull(transform))
            {
                return "";
            }

            Stack<Transform> paths = GetAllParentNode(transform, endTransform, isContainEnd);

            var path = MSharedStringBuilder.Get();
            int pathsCount = paths.Count;
            for (int i = 0; i < pathsCount; i++)
            {
                Transform go = paths.Pop();

                path.Append(go.name);
                path.Append('/');
            }
            path.Append(transform.name);
            return path.ToString();
        }

        //获取所有子物体数量
        public static int GetAllChildrenCount(Transform transform)
        {
            int count = 0;
            if (transform.childCount > 0)
            {
                count += transform.childCount;
                for (int i = 0; i < transform.childCount; i++)
                {
                    count += GetAllChildrenCount(transform.GetChild(i));
                }
            }
            return count;
        }
    }
}


