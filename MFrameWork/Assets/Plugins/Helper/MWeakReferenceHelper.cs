using System;

namespace MFrameWork
{
    public static class MWeakReferenceHelper
    {
        //引用是否失效
        public static bool IsInvalid(WeakReference reference)
        {
            if (reference.IsAlive == false)
            {
                return true;
            }
            if (MNullHelper.IsNull(reference.Target))
            {
                return true;
            }
            return false;
        }
    }
}


