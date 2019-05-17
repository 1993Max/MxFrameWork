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
        public Vector2[] m_anchorMax;
        public Vector2[] m_anchorMin;
        public Vector2[] m_pivot;
        public Vector2[] m_sizeDelta;
        public Vector3[] m_anchorPos;

        [ContextMenu("BindBasicData")]
        public override void BindBasicData()
        {
            for (int i = 0; i < m_allCount; i++)
            {

            }
        }

        public override void ResetBasicData()
        {
            base.ResetBasicData();
        }
    }
}
