// **************************************
//
// 文件名(MResourceItem.cs):
// 功能描述("存储资源数据的一个数据结构"):
// 作者(Max1993):
// 日期(2019/5/3  12:07):
//
// **************************************
//
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork 
{
    public class MResourceItem
    {
        //唯一标示Crc码
        public uint m_crc = 0;
        //存储文件全路径 方便加载资源
        public string m_path = string.Empty;
        //Ab名字
        public string m_abName = string.Empty;
        //资源名字
        public string m_assetName = string.Empty;
        //Ab包的依赖 存储的是当前Ab的依赖的Ab文件 后续进行起依赖加载
        public List<string> m_abDependence = null;
        //该资源加载的AB文件
        public AssetBundle m_assetBundle = null;

        //--------------以下下为资源相关

        //资源对象
        public UnityEngine.Object m_object = null;
        //资源最后使用时间
        public float m_lastUseTime = 0.0f;
        //引用计数
        private int m_refCount = 0;

        public int RefCount
        {
            get
            {
                return m_refCount;
            }

            set
            {
                m_refCount = value;
                if (m_refCount < 0) 
                {
                     
                }
            }
        }
    }
}