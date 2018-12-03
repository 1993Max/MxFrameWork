using UnityEngine;

namespace MFrameWork
{
    public static class MPhysicsHelper
    {
        public static T GetComponentByRay<T>(Ray ray, LayerMask layer) where T : Component
        {
            Transform transform = GetTransformByRay(ray, layer);

            if (transform)
            {
                return transform.GetComponent<T>();
            }

            return null;
        }

        public static Transform GetTransformByRay(Ray ray, LayerMask layer)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layer))
            {
                return hit.transform;
            }
            return null;
        }
    }
}


