// **************************************
//
// 文件名(LoadingController.cs):
// 功能描述("进度"):
// 作者(Max1993):
// 日期(2019/6/30  17:48):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MFrameWork
{
    public class LoadingController : MUIBase
    {
        public GameObject BtnStart;
        public GameObject BtnExit;

        public LoadingController() : base("LoadingPanel", MUILayerType.Normal)
        {

        }

        public override void Init()
        {
            base.Init();
        }

        protected override void OnActive()
        {
            Debug.Log("Active");
        }

        protected override void OnDeActive()
        {
            Debug.Log("OnDeActive");
        }
    }
}