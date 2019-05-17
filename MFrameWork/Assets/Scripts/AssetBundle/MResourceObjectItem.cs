// **************************************
//
// 文件名(MResourceObjectItem.cs):
// 功能描述("需要实例化的资源加载的中间类"):
// 作者(Max1993):
// 日期(2019/5/12  13:36):
//
// **************************************
//
using UnityEngine;
using System.Collections;

namespace MFrameWork 
{
    public class MResourceObjectItem
    {
        //资源路径Crc
        public uint m_crc = 0;
        //存储MresourceItem这个对象数据
        public MResourceItem m_resItem;
        //实例化出来的gameobject
        public GameObject m_cloneObeject;
        //是否切场景清除
        public bool m_isClear = true;
        //实例化Gameobject的实例化Id
        public int m_instanceId = 0;
        //是否已经被释放过了
        public bool m_isAlreadyRelease = false;
        //离线数据
        public MResOffLineDataBase m_resOffLineData = null;
        //----------------------------以下是一些异步的参数
        
        //是否默认创建到默认节点下
        public bool m_isSetToDefault = false;
        //存储实例化资源资源加载完成的回掉
        public OnAsyncLoadObjectFinished m_onAsyncLoadObjectFinished = null;
        //异步回调参数
        public object[] m_parms = null;
        //记录每一个异步加载的Id
        public long m_asyncGuid = 0;

        public void Reset() 
        {
            m_crc = 0;
            m_cloneObeject = null;
            m_isClear = true;
            m_instanceId = 0;
            m_isAlreadyRelease = false;
            m_isSetToDefault = false;
            m_onAsyncLoadObjectFinished = null;
            m_parms = null;
            m_asyncGuid = 0;
            m_resOffLineData = null;
        }
    }
}

