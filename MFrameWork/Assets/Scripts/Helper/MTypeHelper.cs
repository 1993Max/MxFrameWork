using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MFrameWork
{
    public static class MTypeHelper 
    {
        public static bool IsContainInstanceMethod(Type type, string methodName)
        {
            return IsContainMethod(type, methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static bool IsContainStaticMethod(Type type, string methodName)
        {
            return IsContainMethod(type, methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static bool IsContainMethod(Type type, string methodName, BindingFlags bindFlags)
        {
            MethodInfo methodInfo = type.GetMethod(methodName, bindFlags);
            if (methodInfo == null)
            {
                return false;
            }
            return true;
        }
    }
}


