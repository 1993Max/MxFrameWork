using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MFrameWork
{
    public static class MLuaCommonHelper
    {
        public static System.Type GetType(string classname)
        {
            Assembly assb = Assembly.GetExecutingAssembly();
            return assb.GetType(classname);
        }

        /*
        public static Component GetComp(this GameObject go, string className)
        {
            var result = go.GetComponent(className);
            if (result == null)
            {
                var typeMap = MInterfaceMgr.singleton.GetInterface<ITypeMap>("TypeMapOfMoonClient");
                var type = typeMap.GetTypeWithName(className);
                if (type == null)
                {
                    MDebug.singleton.AddErrorLog("Not Exist Component Named " + className);
                    return null;
                }
                result = go.GetComponent(type);
            }
            return result;
        }

        public static Component GetComp(this Component comp, string className)
        {
            return GetComp(comp.gameObject, className);
        }
        */

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static ulong ULong(object o)
        {
            return Convert.ToUInt64(o);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static long MaxLong(long x, long y)
        {
            return Math.Max(x, y);
        }

        public static long MinLong(long x, long y)
        {
            return Math.Min(x, y);
        }

        public static int Long2Int(long value)
        {
            return (int)value;
        }

        public static long AbsLong(long value)
        {
            return Math.Abs(value);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static string GetBinaryString(int num)
        {
            return Convert.ToString(num, 2);
        }

        public static void Log(string str)
        {
            MDebug.singleton.AddLog(str);
        }

        public static void LogGreen(string str)
        {
            MDebug.singleton.AddGreenLog(str);
        }

        public static void LogYellow(string str)
        {
            MDebug.singleton.AddYellowLog(str);
        }

        public static void LogRed(string str)
        {
            MDebug.singleton.AddRedLog(str);
        }

        public static void LogWarning(string str)
        {
            MDebug.singleton.AddWarningLog(str);
        }

        public static void LogError(string str)
        {
            MDebug.singleton.AddErrorLog(str);
        }

        #region GameObject

        public static void SetActiveEx(this GameObject go, bool isActive)
        {
            if (go.activeSelf == isActive) return;
            go.SetActive(isActive);
        }
        #endregion

        #region SetPos

        public static void SetPos(this GameObject go, float x, float y, float z)
        {
            if (go)
            {
                go.transform.position = new Vector3(x, y, z);
            }
        }
        public static void SetPos(this GameObject go, Vector3 pos)
        {
            if (go)
            {
                go.transform.position = pos;
            }
        }
        public static void SetPosX(this GameObject go, float x)
        {
            if (go)
            {
                go.SetPos(x, go.transform.position.y, go.transform.position.z);
            }
        }
        public static void SetPosY(this GameObject go, float y)
        {
            if (go)
            {
                go.SetPos(go.transform.position.x, y, go.transform.position.z);
            }
        }
        public static void SetPosZ(this GameObject go, float z)
        {
            if (go)
            {
                go.SetPos(go.transform.position.x, go.transform.position.y, z);
            }
        }
        public static void SetPosZero(this GameObject go)
        {
            if (go)
            {
                go.transform.position = Vector3.zero;
            }
        }
        public static void SetPosToOther(this GameObject go, GameObject other)
        {
            if (go)
            {
                go.transform.position = other.transform.position;
            }
        }
        public static void SetPosToOther(this GameObject go, Transform other)
        {
            if (go)
            {
                go.transform.position = other.position;
            }
        }

        public static void SetLocalPos(this GameObject go, float x, float y, float z)
        {
            if (go)
            {
                go.transform.localPosition = new Vector3(x, y, z);
            }
        }
        public static void SetLocalPos(this GameObject go, Vector3 pos)
        {
            if (go)
            {
                go.transform.localPosition = pos;
            }
        }
        public static void SetLocalPosX(this GameObject go, float x)
        {
            if (go)
            {
                go.SetLocalPos(x, go.transform.localPosition.y, go.transform.localPosition.z);
            }
        }
        public static void SetLocalPosY(this GameObject go, float y)
        {
            if (go)
            {
                go.SetLocalPos(go.transform.localPosition.x, y, go.transform.localPosition.z);
            }
        }
        public static void SetLocalPosZ(this GameObject go, float z)
        {
            if (go)
            {
                go.SetLocalPos(go.transform.localPosition.x, go.transform.localPosition.y, z);
            }
        }
        public static void SetLocalPosZero(this GameObject go)
        {
            if (go)
            {
                go.transform.localPosition = Vector3.zero;
            }
        }
        public static void SetLocalPosToOther(this GameObject go, GameObject other)
        {
            if (go)
            {
                go.transform.localPosition = other.transform.localPosition;
            }
        }
        public static void SetLocalPosToOther(this GameObject go, Transform other)
        {
            if (go)
            {
                go.transform.localPosition = other.localPosition;
            }
        }

        public static void SetPos(this Transform tran, float x, float y, float z)
        {
            if (tran)
            {
                tran.position = new Vector3(x, y, z);
            }
        }
        public static void SetPos(this Transform tran, Vector3 pos)
        {
            if (tran)
            {
                tran.transform.position = pos;
            }
        }
        public static void SetPosX(this Transform tran, float x)
        {
            if (tran)
            {
                tran.SetPos(x, tran.position.y, tran.position.z);
            }
        }
        public static void SetPosY(this Transform tran, float y)
        {
            if (tran)
            {
                tran.SetPos(tran.position.x, y, tran.position.z);
            }
        }
        public static void SetPosZ(this Transform tran, float z)
        {
            if (tran)
            {
                tran.SetPos(tran.position.x, tran.position.y, z);
            }
        }
        public static void SetPosZero(this Transform tran)
        {
            if (tran)
            {
                tran.position = Vector3.zero;
            }
        }
        public static void SetPosToOther(this Transform tran, GameObject other)
        {
            if (tran)
            {
                tran.position = other.transform.position;
            }
        }
        public static void SetPosToOther(this Transform tran, Transform other)
        {
            if (tran)
            {
                tran.position = other.position;
            }
        }

        public static void SetLocalPos(this Transform tran, float x, float y, float z)
        {
            if (tran)
            {
                tran.localPosition = new Vector3(x, y, z);
            }
        }
        public static void SetLocalPos(this Transform tran, Vector3 pos)
        {
            if (tran)
            {
                tran.transform.localPosition = pos;
            }
        }
        public static void SetLocalPosX(this Transform tran, float x)
        {
            if (tran)
            {
                tran.SetLocalPos(x, tran.localPosition.y, tran.localPosition.z);
            }
        }
        public static void SetLocalPosY(this Transform tran, float y)
        {
            if (tran)
            {
                tran.SetLocalPos(tran.localPosition.x, y, tran.localPosition.z);
            }
        }
        public static void SetLocalPosZ(this Transform tran, float z)
        {
            if (tran)
            {
                tran.SetLocalPos(tran.localPosition.x, tran.localPosition.y, z);
            }
        }
        public static void SetLocalPosZero(this Transform tran)
        {
            if (tran)
            {
                tran.localPosition = Vector3.zero;
            }
        }
        public static void SetLocalPosToOther(this Transform tran, GameObject other)
        {
            if (tran)
            {
                tran.localPosition = other.transform.localPosition;
            }
        }
        public static void SetLocalPosToOther(this Transform tran, Transform other)
        {
            if (tran)
            {
                tran.localPosition = other.localPosition;
            }
        }

        #endregion SetPos

        #region SetScale

        public static void SetLocalScale(this GameObject go, float x, float y, float z)
        {
            if (go)
            {
                go.transform.localScale = new Vector3(x, y, z);
            }
        }
        public static void SetLocalScaleX(this GameObject go, float x)
        {
            if (go)
            {
                go.SetLocalScale(x, go.transform.localScale.y, go.transform.localScale.z);
            }
        }
        public static void SetLocalScaleY(this GameObject go, float y)
        {
            if (go)
            {
                go.SetLocalScale(go.transform.localScale.x, y, go.transform.localScale.z);
            }
        }
        public static void SetLocalScaleZ(this GameObject go, float z)
        {
            if (go)
            {
                go.SetLocalScale(go.transform.localScale.x, go.transform.localScale.y, z);
            }
        }
        public static void SetLocalScaleOne(this GameObject go)
        {
            if (go)
            {
                go.transform.localScale = Vector3.one;
            }
        }
        public static void SetLocalScaleToOther(this GameObject go, GameObject other)
        {
            if (go)
            {
                go.transform.localScale = other.transform.localScale;
            }
        }
        public static void SetLocalScaleToOther(this GameObject go, Transform other)
        {
            if (go)
            {
                go.transform.localScale = other.localScale;
            }
        }


        public static void SetLocalScale(this Transform tran, float x, float y, float z)
        {
            if (tran)
            {
                tran.localScale = new Vector3(x, y, z);
            }
        }
        public static void SetLocalScaleX(this Transform tran, float x)
        {
            if (tran)
            {
                tran.SetLocalScale(x, tran.localScale.y, tran.localScale.z);
            }
        }
        public static void SetLocalScaleY(this Transform tran, float y)
        {
            if (tran)
            {
                tran.SetLocalScale(tran.localScale.x, y, tran.localScale.z);
            }
        }
        public static void SetLocalScaleZ(this Transform tran, float z)
        {
            if (tran)
            {
                tran.SetLocalScale(tran.localScale.x, tran.localScale.y, z);
            }
        }
        public static void SetLocalScaleOne(this Transform tran)
        {
            if (tran)
            {
                tran.localScale = Vector3.one;
            }
        }
        public static void SetLocalScaleDouble(this Transform tran, float times)
        {
            if (tran)
            {
                tran.localScale = tran.localScale * times;
            }
        }
        public static void SetLocalScaleToOther(this Transform tran, GameObject other)
        {
            if (tran)
            {
                tran.localScale = other.transform.localScale;
            }
        }
        public static void SetLocalScaleToOther(this Transform tran, Transform other)
        {
            if (tran)
            {
                tran.localScale = other.localScale;
            }
        }

        #endregion SetScale

        public static void SetRot(this GameObject go, float x, float y, float z, float w)
        {
            if (go)
            {
                go.transform.rotation = new Quaternion(x, y, z, w);
            }
        }

        public static void SetLocalRot(this GameObject go, float x, float y, float z, float w)
        {
            if (go)
            {
                go.transform.localRotation = new Quaternion(x, y, z, w);
            }
        }

        public static void SetRot(this Transform tran, float x, float y, float z, float w)
        {
            if (tran)
            {
                tran.rotation = new Quaternion(x, y, z, w);
            }
        }

        public static void SetLocalRot(this Transform tran, float x, float y, float z, float w)
        {
            if (tran)
            {
                tran.localRotation = new Quaternion(x, y, z, w);
            }
        }

        #region SetRotEuler

        public static void SetRotEuler(this GameObject go, float x, float y, float z)
        {
            if (go)
            {
                go.transform.rotation = Quaternion.Euler(x, y, z);
            }
        }
        public static void SetRotEulerX(this GameObject go, float x)
        {
            if (go)
            {
                go.SetRotEuler(x, go.transform.eulerAngles.y, go.transform.eulerAngles.z);
            }
        }
        public static void SetRotEulerY(this GameObject go, float y)
        {
            if (go)
            {
                go.SetRotEuler(go.transform.eulerAngles.x, y, go.transform.eulerAngles.z);
            }
        }
        public static void SetRotEulerZ(this GameObject go, float z)
        {
            if (go)
            {
                go.SetRotEuler(go.transform.eulerAngles.x, go.transform.eulerAngles.y, z);
            }
        }
        public static void SetRotEulerZero(this GameObject go)
        {
            if (go)
            {
                go.transform.eulerAngles = Vector3.zero;
            }
        }
        public static void SetRotEulerToOther(this GameObject go, GameObject other)
        {
            if (go)
            {
                go.transform.eulerAngles = other.transform.eulerAngles;
            }
        }
        public static void SetRotEulerToOther(this GameObject go, Transform other)
        {
            if (go)
            {
                go.transform.eulerAngles = other.eulerAngles;
            }
        }

        public static void SetLocalRotEuler(this GameObject go, float x, float y, float z)
        {
            if (go)
            {
                go.transform.localRotation = Quaternion.Euler(x, y, z);
            }
        }
        public static void SetLocalRotEulerX(this GameObject go, float x)
        {
            if (go)
            {
                go.SetLocalRotEuler(x, go.transform.localRotation.y, go.transform.localRotation.z);
            }
        }
        public static void SetLocalRotEulerY(this GameObject go, float y)
        {
            if (go)
            {
                go.SetLocalRotEuler(go.transform.localRotation.x, y, go.transform.localRotation.z);
            }
        }
        public static void SetLocalRotEulerZ(this GameObject go, float z)
        {
            if (go)
            {
                go.SetLocalRotEuler(go.transform.localRotation.x, go.transform.localRotation.y, z);
            }
        }
        public static void SetLocalRotEulerZero(this GameObject go)
        {
            if (go)
            {
                go.transform.localEulerAngles = Vector3.zero;
            }
        }
        public static void SetLocalRotEulerToOther(this GameObject go, GameObject other)
        {
            if (go)
            {
                go.transform.localEulerAngles = other.transform.localEulerAngles;
            }
        }
        public static void SetLocalRotEulerToOther(this GameObject go, Transform other)
        {
            if (go)
            {
                go.transform.localEulerAngles = other.localEulerAngles;
            }
        }

        public static void SetRotEuler(this Transform tran, float x, float y, float z)
        {
            if (tran)
            {
                tran.rotation = Quaternion.Euler(x, y, z);
            }
        }
        public static void SetRotEulerX(this Transform tran, float x)
        {
            if (tran)
            {
                tran.SetRotEuler(x, tran.eulerAngles.y, tran.eulerAngles.z);
            }
        }
        public static void SetRotEulerY(this Transform tran, float y)
        {
            if (tran)
            {
                tran.SetRotEuler(tran.eulerAngles.x, y, tran.eulerAngles.z);
            }
        }
        public static void SetRotEulerZ(this Transform tran, float z)
        {
            if (tran)
            {
                tran.SetRotEuler(tran.eulerAngles.x, tran.eulerAngles.y, z);
            }
        }
        public static void SetRotEulerZero(this Transform tran)
        {
            if (tran)
            {
                tran.eulerAngles = Vector3.zero;
            }
        }
        public static void SetRotEulerToOther(this Transform tran, GameObject other)
        {
            if (tran)
            {
                tran.eulerAngles = other.transform.eulerAngles;
            }
        }
        public static void SetRotEulerToOther(this Transform tran, Transform other)
        {
            if (tran)
            {
                tran.eulerAngles = other.eulerAngles;
            }
        }

        public static void SetLocalRotEuler(this Transform tran, float x, float y, float z)
        {
            if (tran)
            {
                tran.localRotation = Quaternion.Euler(x, y, z);
            }
        }
        public static void SetLocalRotEulerX(this Transform tran, float x)
        {
            if (tran)
            {
                tran.SetLocalRotEuler(x, tran.localRotation.y, tran.localRotation.z);
            }
        }
        public static void SetLocalRotEulerY(this Transform tran, float y)
        {
            if (tran)
            {
                tran.SetLocalRotEuler(tran.localRotation.x, y, tran.localRotation.z);
            }
        }
        public static void SetLocalRotEulerZ(this Transform tran, float z)
        {
            if (tran)
            {
                tran.SetLocalRotEuler(tran.localRotation.x, tran.localRotation.y, z);
            }
        }
        public static void SetLocalRotEulerZero(this Transform tran)
        {
            if (tran)
            {
                tran.localEulerAngles = Vector3.zero;
            }
        }
        public static void SetLocalRotEulerToOther(this Transform tran, GameObject other)
        {
            if (tran)
            {
                tran.localEulerAngles = other.transform.localEulerAngles;
            }
        }
        public static void SetLocalRotEulerToOther(this Transform tran, Transform other)
        {
            if (tran)
            {
                tran.localEulerAngles = other.localEulerAngles;
            }
        }

        /// <summary>
        /// 子对象坐标转换到Canvas的局部坐标
        /// </summary>
        /// <param name="current"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Vector2 TransformToCanvasLocalPosition(this Transform current, Canvas canvas)
        {
            var screenPos = canvas.worldCamera.WorldToScreenPoint(current.transform.position);
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos,
                canvas.worldCamera, out localPos);
            return localPos;
        }

        #endregion

        public static void SetRectTransformPos(this GameObject go, float x, float y)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.anchoredPosition = new Vector2(x, y);
                }
            }
        }

        public static void SetRectTransformPosX(this GameObject go, float x)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    Vector2 v = rectTrans.anchoredPosition;
                    rectTrans.anchoredPosition = new Vector2(x, v.y);
                }
            }
        }

        public static void SetRectTransformSize(this GameObject go, float width, float height)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.sizeDelta = new Vector2(width, height);
                }
            }
        }

        public static void SetRectTransformWidth(this GameObject go, float width)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.sizeDelta = new Vector2(width, rectTrans.sizeDelta.y);
                }
            }
        }

        public static void SetRectTransformHeight(this GameObject go, float height)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
                }
            }
        }

        public static void SetRectTransformPosY(this GameObject go, float y)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    Vector2 v = rectTrans.anchoredPosition;
                    rectTrans.anchoredPosition = new Vector2(v.x, y);
                }
            }
        }

        public static void SetRectTransformOffset(this GameObject go, float xMin, float xMax, float yMin, float yMax)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.offsetMax = new Vector2(xMax, yMax);
                    rectTrans.offsetMin = new Vector2(xMin, yMin);
                }
            }
        }

        public static void SetRectTransformPivot(this GameObject go, float pivotX, float pivotY)
        {
            if (go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                if (rectTrans)
                {
                    rectTrans.pivot = new Vector2(pivotX, pivotY);
                }
            }
        }

        public static float GetDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static float GetDistance2D(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        public static float GetAngle2D(Vector2 a, Vector2 b)
        {
            return Vector2.Angle(a, b);
        }

        public static void SetParent(GameObject go, GameObject parent)
        {
            if (go)
            {
                var trans = parent == null ? null : parent.transform;
                go.transform.SetParent(trans);
            }
        }

        //得到物体是所有显示的物体的第几个
        public static int GetTransformActiveIndex(Transform transform)
        {
            Transform parent = transform.parent;
            if (parent == null)
            {
                return 0;
            }
            int activeIndex = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child == transform)
                {
                    return activeIndex;
                }
                if (child.gameObject.activeSelf)
                {
                    activeIndex++;
                }
            }
            return 0;
        }

        public static bool IsNull(object objectA)
        {
            if (ReferenceEquals(objectA, null))
            {
                return true;
            }
            return objectA.Equals(null);
        }

        public static bool IsStringContain(string data,string value)
        {
            return data.Contains(value);
        }

        public static void ForceUpdateContentSizeFitter(GameObject target, bool horizontal = true)
        {
            if (target)
            {
                ContentSizeFitter fitter = target.GetComponent<ContentSizeFitter>();
                if (fitter)
                {
                    if (horizontal)
                    {
                        fitter.SetLayoutHorizontal();
                    }
                    else
                    {
                        fitter.SetLayoutVertical();
                    }
                }
                RectTransform trans = target.transform as RectTransform;
                if (trans)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
                }
            }
        }
    }
}