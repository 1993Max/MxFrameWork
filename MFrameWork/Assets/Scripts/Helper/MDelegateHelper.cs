using System;

namespace MFrameWork
{
    public static class MDelegateHelper
    {
        //判断container是否包含containee
        public static bool IsContain(MulticastDelegate container, Delegate containee)
        {
            if (container == null || containee == null)
            {
                return false;
            }
            Delegate[] delegates = container.GetInvocationList();
            for (int i = 0; i < delegates.Length; i ++)
            {
                if (delegates[i] == containee)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

