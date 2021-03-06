﻿// **************************************
//
// 文件名(MAsyncObjectManager.cs):
// 功能描述("该类是Mobject的拆分类 主要负责需要实例化的Object的异步加载"):
// 作者(Max1993):
// 日期(2019/5/13  23:26):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MFrameWork
{
    public partial class MObjectManager
    {
        //根据异步的Guid存储MResourceObjectItem  用于记录异步加载中的列表 也可以取消正在进行中的异步加载
        public Dictionary<long, MResourceObjectItem> m_asyncResourcesObjectsDic = null;

        public bool InitASyncObjectManager()
        {
            m_asyncResourcesObjectsDic = new Dictionary<long, MResourceObjectItem>();
            return true;
        }

        /// <summary>
        /// 取消异步加载
        /// </summary>
        /// <param name="asyncGuid">异步加载GUID</param>
        public void CancleAsyncResObjectLoad(long asyncGuid)
        {
            MResourceObjectItem mResourceObjectItem = null;
            if (m_asyncResourcesObjectsDic.TryGetValue(asyncGuid, out mResourceObjectItem))
            {
                m_asyncResourcesObjectsDic.Remove(asyncGuid);
                mResourceObjectItem.Reset();
                m_resourceObjectClssPool.Recycle(mResourceObjectItem);
            }
        }

        /// <summary>
        /// 是否在异步加载中
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsInAsyncLoad(long guid)
        {
            return m_asyncResourcesObjectsDic[guid] != null;
        }

        /// <summary>
        /// 是否由对象池创建的
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsObjectManagerCreate(GameObject obj)
        {
            MResourceObjectItem mResourceObjectItem = m_resourceObjectDic[obj.GetInstanceID()];
            return mResourceObjectItem != null;
        }

        /// <summary>
        /// 异步加载需要实力化的GameObject的函数
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="onAsyncLoadFinished"></param>
        /// <param name="loadResPriority"></param>
        /// <param name="isSetToDefault"></param>
        /// <param name="parms"></param>
        /// <param name="isChangeSceneClear"></param>
        public long InstantiateGameObejectAsync(string resPath,OnAsyncLoadObjectFinished onAsyncLoadFinished,LoadResPriority loadResPriority,
                                                bool isSetToDefault = false, object[] parms = null,bool isChangeSceneClear = true)
        {
            if (string.IsNullOrEmpty(resPath))
                return 0;

            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceObjectItem mResourceObjectItem = GetObjectFromPool(crc);
            if (mResourceObjectItem != null && mResourceObjectItem.m_cloneObeject!= null)
            {
                if (isSetToDefault)
                {
                    mResourceObjectItem.m_cloneObeject.transform.SetParent(DefaultObjectTrans);
                }

                if (onAsyncLoadFinished != null)
                {
                    onAsyncLoadFinished(resPath, mResourceObjectItem, parms);
                }
                return mResourceObjectItem.m_asyncGuid;
            }
            long m_asyncGuid = MResourceManager.singleton.GetGUID();
            mResourceObjectItem = m_resourceObjectClssPool.Spawn(true);
            mResourceObjectItem.m_crc = crc;
            mResourceObjectItem.m_isSetToDefault = isSetToDefault;
            mResourceObjectItem.m_isClear = isChangeSceneClear;
            mResourceObjectItem.m_onAsyncLoadObjectFinished = onAsyncLoadFinished;
            mResourceObjectItem.m_parms = parms;
            mResourceObjectItem.m_asyncGuid = m_asyncGuid;

            //添加到异步加载管理列表里
            m_asyncResourcesObjectsDic.Add(m_asyncGuid, mResourceObjectItem);
            //调用MResourceManager为Object准备的的异步加载函数
            MResourceManager.singleton.AsyncLoadResource(resPath, mResourceObjectItem, OnAsyncLoadObjectFinish, loadResPriority, parms);
            return m_asyncGuid;
        }

        /// <summary>
        /// 资源加载完成回调
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="mResourceObjectItem">中间类</param>
        /// <param name="parms">附加参数</param>
        void OnAsyncLoadObjectFinish(string resPath, MResourceObjectItem mResourceObjectItem, object[] parms = null)
        {
            if (mResourceObjectItem == null)
                return;
            if (mResourceObjectItem.m_resItem.m_object == null)
            {
#if UNITY_EDITOR
                MDebug.singleton.AddErrorLog("异步资源加载为空 : " + resPath);
#endif
            }
            else
            {
                mResourceObjectItem.m_cloneObeject = GameObject.Instantiate(mResourceObjectItem.m_resItem.m_object) as GameObject;
                mResourceObjectItem.m_resOffLineData = mResourceObjectItem.m_cloneObeject.GetComponent<MResOffLineDataBase>();
            }

            //加载完成从正在加载的异步队列中移除
            if (m_asyncResourcesObjectsDic.ContainsKey(mResourceObjectItem.m_asyncGuid))
            {
                m_asyncResourcesObjectsDic.Remove(mResourceObjectItem.m_asyncGuid);
            }

            if (mResourceObjectItem.m_cloneObeject != null && mResourceObjectItem.m_isSetToDefault)
            {
                mResourceObjectItem.m_cloneObeject.transform.SetParent(DefaultObjectTrans);
            }

            if (mResourceObjectItem.m_onAsyncLoadObjectFinished != null)
            {
                int instanceId = mResourceObjectItem.m_cloneObeject.GetInstanceID();
                if (!m_resourceObjectDic.ContainsKey(instanceId))
                {
                    m_resourceObjectDic.Add(instanceId, mResourceObjectItem);
                }
                mResourceObjectItem.m_onAsyncLoadObjectFinished(resPath, mResourceObjectItem, parms);
            }
        }
    }
}
