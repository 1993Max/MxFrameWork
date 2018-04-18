using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MFrameWork
{
    public sealed class MResManager : MSingleton<MResManager>
    {
        public static readonly Vector3 FAR_FAR_AWAY = new Vector3(0, -1000f, 0); // for GameObject pool
    }
}
