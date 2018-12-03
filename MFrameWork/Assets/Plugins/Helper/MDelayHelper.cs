using UnityEngine;
using System.Collections;
using System;

namespace MFrameWork
{
    public static class MDelayHelper
    {
        // 按帧数延时 
        public static Coroutine DelayAction(MonoBehaviour mono, int frame, Action method)
        {
            if (mono)
            {
                return mono.StartCoroutine(DoAction(mono, frame, method));
            }
            return null;
        }
        private static IEnumerator DoAction(MonoBehaviour mono, int frame, Action method)
        {
            while (frame > 0 && mono != null)
            {
                yield return null;
                frame--;

            }
            if (mono)
            {
                method();
            }
            yield return null;
        }

        //按秒数延时
        public static Coroutine DelayAction(MonoBehaviour mono, YieldInstruction delay, Action method)
        {
            if (mono)
            {
                return mono.StartCoroutine(DoAction(mono, delay, method));
            }
            return null;
        }

        private static IEnumerator DoAction(MonoBehaviour mono, YieldInstruction delay, Action method)
        {
            yield return delay;
            if (mono)
            {
                method();
            }
            yield return null;
        }

        //按条件延时
        public static Coroutine DelayAction(MonoBehaviour mono, Func<bool> detectionMethod, Action method)
        {
            if (mono)
            {
                return mono.StartCoroutine(DoAction(mono, detectionMethod, null, method));
            }
            return null;
        }
        public static Coroutine DelayAction(MonoBehaviour mono, Func<bool> detectionMethod, Action inTimeMethod, Action endMethod)
        {
            if (mono)
            {
                return mono.StartCoroutine(DoAction(mono, detectionMethod, inTimeMethod, endMethod));
            }
            return null;
        }
        private static IEnumerator DoAction(MonoBehaviour mono, Func<bool> detectionMethod, Action inTimeMethod, Action endMethod)
        {
            while (mono != null && !detectionMethod())
            {
                if (inTimeMethod != null)
                    inTimeMethod();
                yield return null;
            }
            if (mono != null)
            {
                endMethod();
            }
            yield return null;
        }

        // 在延时中每帧执行inTimeMethod，延时结束执行endMethod
        public static Coroutine DelayAction(MonoBehaviour mono, YieldInstruction delay, Action inTimeMethod, Action endMethod)
        {
            if (mono)
            {
                return mono.StartCoroutine(DoAction(mono, delay, inTimeMethod, endMethod));
            }
            return null;
        }

        private static IEnumerator DoAction(MonoBehaviour mono, YieldInstruction delay, Action inTimeMethod, Action endMethod)
        {
            bool b = false;
            DelayAction(mono, delay, () => b = true);
            while (!b)
            {
                if (inTimeMethod != null)
                {
                    inTimeMethod();
                }
                yield return null;
            }
            if (endMethod != null && mono)
            {
                endMethod();
            }
        }

        public static void StopCoroutine(MonoBehaviour mono, Coroutine coroutine)
        {
            if (mono==null|| coroutine==null)
            {
                return;
            }
            mono.StopCoroutine(coroutine);
        }
    }
}

