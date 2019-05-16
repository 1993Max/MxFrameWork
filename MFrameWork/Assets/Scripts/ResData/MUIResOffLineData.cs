// **************************************
//
// 文件名(MUIResOffLineData.cs):
// 功能描述("处理UI资源的离线数据类):
// 作者(Max1993):
// 日期(2019/5/16  16:09):
//
// **************************************
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MFrameWork
{
    public class MUIResOffLineData : MResOffLineDataBase
    {
        public RectTransform m_rectTransform;

        [ContextMenu("BindBasicData")]
        public override void BindBasicData()
        {
            base.BindBasicData();
        }

        public override void ResetBasicData()
        {
            base.ResetBasicData();
        }
    }
}
