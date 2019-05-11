// **************************************
//
// 文件名(MSyncObjectManager.cs):
// 功能描述("该类是Mobject的拆分类 主要负责需要实例化的Object的同步加载"):
// 作者(Max1993):
// 日期(2019/5/11  16:06):
//
// **************************************
//
using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    public partial class MObjectManager
    {
        //Object对象池的TranForm节点
        private Transform m_recycleObjectPoolTrans = null;

        public Transform RecycleObjectPoolTrans
        {
            get
            {
                if (m_recycleObjectPoolTrans == null)
                {
                    return GameObject.Find(MPathUtils.RECYCLE_POOL_TRANSFORM).transform;
                }
                return m_recycleObjectPoolTrans;
            }
        }

        public bool InitSyncObjectManager()
        {
            return true;
        }
    }
}
