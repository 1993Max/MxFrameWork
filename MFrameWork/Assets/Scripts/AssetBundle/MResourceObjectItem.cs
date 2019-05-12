// **************************************
//
// 文件名(MObjectItem.cs):
// 功能描述("Autor Please Edit"):
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
        public GameObject m_gameObeject;
        //是否切场景清除
        public bool m_isClear = true;
        //实例化Gameobject的Guid
        public int m_guid = 0;
        //是否已经被释放过了
        public bool m_isAlreadyRelease = false;

        public void Reset() 
        {
            m_crc = 0;
            m_gameObeject = null;
            m_isClear = true;
            m_guid = 0;
            m_isAlreadyRelease = false;
        }
    }
}

