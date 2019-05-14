// **************************************
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

namespace MFrameWork
{
    public partial class MObjectManager
    {
        /// <summary>
        /// 异步加载需要实力化的GameObject的函数
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="onAsyncLoadFinished"></param>
        /// <param name="loadResPriority"></param>
        /// <param name="isSetToDefault"></param>
        /// <param name="parms"></param>
        /// <param name="isChangeSceneClear"></param>
        public void InstantiateGameObejectAsync(string resPath,OnAsyncLoadObjectFinished onAsyncLoadFinished,LoadResPriority loadResPriority,
                                                bool isSetToDefault = false, object[] parms = null,bool isChangeSceneClear = true)
        {
            if (string.IsNullOrEmpty(resPath))
                return;

            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceObjectItem mResourceObjectItem = GetObjectFromPool(crc);
            if (mResourceObjectItem != null && mResourceObjectItem.m_gameObeject!= null)
            {
                if (isSetToDefault)
                {
                    mResourceObjectItem.m_gameObeject.transform.SetParent(DefaultObjectTrans);
                }

                if (onAsyncLoadFinished != null)
                {
                    onAsyncLoadFinished(resPath, mResourceObjectItem, parms);
                }
                return;
            }

            mResourceObjectItem = m_resourceObjectClssPool.Spawn(true);
            mResourceObjectItem.m_crc = crc;
            mResourceObjectItem.m_isSetToDefault = isSetToDefault;
            mResourceObjectItem.m_isClear = isChangeSceneClear;
            mResourceObjectItem.m_onAsyncLoadObjectFinished = onAsyncLoadFinished;
            mResourceObjectItem.m_parms = parms;

            //调用MResourceManager为Object准备的的异步加载函数
            MResourceManager.singleton.AsyncLoadResource(resPath, mResourceObjectItem, OnAsyncLoadObjectFinish, loadResPriority, parms);
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
                mResourceObjectItem.m_gameObeject = GameObject.Instantiate(mResourceObjectItem.m_resItem.m_object) as GameObject;
            }

            if (mResourceObjectItem.m_gameObeject != null && mResourceObjectItem.m_isSetToDefault)
            {
                mResourceObjectItem.m_gameObeject.transform.SetParent(DefaultObjectTrans);
            }

            if (mResourceObjectItem.m_onAsyncLoadObjectFinished != null)
            {
                int instanceId = mResourceObjectItem.m_gameObeject.GetInstanceID();
                if (!m_resourceObjectDic.ContainsKey(instanceId))
                {
                    m_resourceObjectDic.Add(instanceId, mResourceObjectItem);
                }
                mResourceObjectItem.m_onAsyncLoadObjectFinished(resPath, mResourceObjectItem, parms);
            }
        }

        //取消异步加载
    }
}
