// **************************************
//
// 文件名(MResourceManager.cs.cs):
// 功能描述("资源管理类"):
// 作者(Max1993):
// 日期(2019/5/2  21:35):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MFrameWork
{
    public class MResourceManager : MSingleton<MResourceManager>
    {
        public override bool Init()
        {
            return base.Init();
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        public override void UnInit()
        {
            base.UnInit();
        }
    }
}
