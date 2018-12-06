using UnityEngine;

namespace MFrameWork
{
    public static class MCoordinateHelper
    {
        //摄像机世界坐标转另一个摄像机坐标
        static public Vector3 CameraToCameraPosition(Camera originCamera, Camera targetCamera, Vector3 originWorldPosition, Transform targetTransform)
        {
            Vector3 viewportPosition = originCamera.WorldToViewportPoint(originWorldPosition);
            return ViewportToCameraPosition(viewportPosition, targetCamera, targetTransform);
        }
        //视窗坐标转摄像机坐标
        public static Vector3 ViewportToCameraPosition(Vector3 viewportPosition, Camera targetCamera, Transform targetTransform)
        {
            Vector3 position = targetCamera.ViewportToWorldPoint(viewportPosition);
            position = WorldPositionToLocalPosition(position, targetTransform);
            return position;
        }
        //屏幕坐标转摄像机坐标
        public static Vector3 ScreenToCameraPosition(Vector3 screenPosition, Camera targetCamera, Transform targetTransform)
        {
            Vector3 position = targetCamera.ScreenToWorldPoint(screenPosition);
            position = WorldPositionToLocalPosition(position, targetTransform);
            return position;
        }
        //世界坐标转局部坐标
        public static Vector3 WorldPositionToLocalPosition(Vector3 worldPosition, Transform targetTransform)
        {
            if (targetTransform == null)
            {
                return worldPosition;
            }
            targetTransform = targetTransform.parent;
            if (targetTransform == null)
            {
                return worldPosition;
            }
            return targetTransform.InverseTransformPoint(worldPosition);
        }
    }
}


