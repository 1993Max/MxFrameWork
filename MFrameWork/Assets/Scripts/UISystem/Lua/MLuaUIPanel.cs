using System;
using UnityEngine;

namespace MFrameWork
{
    public class MLuaUIPanel : MonoBehaviour
    {
        public bool IsHandler = false;
        public MLuaUICom[] ComRefs = null;
        public MLuaUIGroup[] Groups;
        public MLuaUICom HandlerRef = null;
    }
}
