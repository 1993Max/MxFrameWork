﻿// **************************************
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
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFrameWork
{
    public partial class MResourceManager : MSingleton<MResourceManager>
    {
        //是否从Ab加载资源
        private bool m_isLoadFormAssetBundle = false;

//--------------------------------同步资源加载数据-------------------------------------
        //缓存加载过的资源 
        private Dictionary<uint, MResourceItem> m_resourcesItemDic;
        //缓存引用计数为0的资源列表 作用主要是 加载过资源卸载后管理起来 方便后续继续使用
        protected CMapList<MResourceItem> m_resourcesMapItemList;
//--------------------------------同步资源加载数据-------------------------------------


        public Dictionary<uint, MResourceItem> ResourcesItemDic
        {
            get
            {
                return m_resourcesItemDic;
            }

            set
            {
                m_resourcesItemDic = value;
            }
        }

        public override bool Init()
        {
            m_resourcesMapItemList = new CMapList<MResourceItem>();
            m_resourcesItemDic = new Dictionary<uint, MResourceItem>();
            //异步加载初始化
            InitAsyncManager();
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

        /// <summary>
        /// 同步资源加载 外部直接调用 仅用于加载不需要实例化的资源 例如Texture 音频等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resPath">资源路径</param>
        /// <returns></returns>
        public T LoadResource<T>(string resPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(resPath))
                return null;
            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceItem mResourceItem = GetCacheResourceItem(crc);
            if (mResourceItem != null)
            {
                return mResourceItem.m_object as T;
            }

            T obj = null;
#if UNITY_EDITOR
            //编辑器模式下 直接从本地拿到资源
            if (!m_isLoadFormAssetBundle)
            {
                mResourceItem = MAssetBundleManager.singleton.FindResourceItem(crc);
                if (mResourceItem.m_object != null)
                {
                    if (mResourceItem.m_object != null)
                    {
                        obj = mResourceItem.m_object as T;
                    }
                    else
                    {
                        obj = LoadAssetFormEditor<T>(resPath);
                    }
                }
            }
#endif
            if (obj == null)
            {
                mResourceItem = MAssetBundleManager.singleton.LoadResourcesAssetBundle(crc);
                if (mResourceItem != null && mResourceItem.m_assetBundle != null)
                {
                    if (mResourceItem.m_object != null)
                    {
                        obj = mResourceItem.m_object as T;
                    }
                    else
                    {
                        obj = mResourceItem.m_assetBundle.LoadAsset<T>(mResourceItem.m_assetName);
                    }
                }
            }

            CacheResource(resPath, ref mResourceItem, crc, obj);
            return obj;
        }

        /// <summary>
        /// 不需要实例化的资源的卸载
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destoryCompletly">是否彻底删除 false 则加入资源缓存列表中管理 true 彻底删除</param>
        /// <returns></returns>
        public bool ReleaseResource(Object obj, bool destoryCompletly = false)
        {
            if (obj == null)
                return false;
            MResourceItem mResourceItem = null;
            foreach (var item in m_resourcesItemDic.Values)
            {
                if (item.m_guid == obj.GetInstanceID())
                    mResourceItem = item;
            }
            if (mResourceItem == null)
            {
                MDebug.singleton.AddErrorLog(" m_resourcesItemDic 不存在这个资源 resName : " + obj.name);
                return false;
            }

            mResourceItem.RefCount--;
            DestoryResourceItem(mResourceItem,destoryCompletly);
            return true;
        }

        /// <summary>
        /// 对已经加载过的资源 做资源缓存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mResourceItem"></param>
        /// <param name="crc"></param>
        /// <param name="obj"></param>
        /// <param name="addRefCount"></param>
        protected void CacheResource(string path, ref MResourceItem mResourceItem, uint crc, Object obj, int addRefCount = 1)
        {
            //缓存太多 清楚最早没有使用的资源
            ClearEarlierUnUsedResItem();

            if (mResourceItem == null)
            {
                MDebug.singleton.AddErrorLog(" MResourceItem == null Path : "+ path);
                return;
            }

            if (obj == null)
            {
                MDebug.singleton.AddErrorLog(" Object == null Path : "+ path);
                return;
            }

            mResourceItem.m_object = obj;
            mResourceItem.m_guid = obj.GetInstanceID();
            mResourceItem.m_lastUseTime = Time.realtimeSinceStartup;
            mResourceItem.RefCount = addRefCount;

            MResourceItem oldResourceItem = null;
            if (m_resourcesItemDic.TryGetValue(crc, out oldResourceItem))
            {
                m_resourcesItemDic[mResourceItem.m_crc] = mResourceItem;
            }
            else
            {
                m_resourcesItemDic.Add(mResourceItem.m_crc, mResourceItem);
            }
        }

        /// <summary>
        /// 卸载资源管理的列表缓存太多 清除最早的资源
        /// </summary>
        protected void ClearEarlierUnUsedResItem()
        {
            /*
             * 对内存做判断 内存占用太多 执行清除操作
            if (m_resourcesMapItemList.Size() <= 0)
                return;
            MResourceItem tailItem = m_resourcesMapItemList.GetTail();
            DestoryResourceItem(tailItem, true);
            m_resourcesMapItemList.PopTail();
            */
        }

        /// <summary>
        /// 回收一个资源
        /// </summary>
        /// <param name="mResourceItem"></param>
        /// <param name="destoryCatchResItem">是否彻底删除 false 则加入资源缓存列表中管理 true 彻底删除</param>
        protected void DestoryResourceItem(MResourceItem mResourceItem , bool destoryCompletly = false)
        {
            if (mResourceItem == null || mResourceItem.RefCount > 0)
                return;

            //从已经加载的资源缓存里面删除
            if (!m_resourcesItemDic.Remove(mResourceItem.m_crc))
                return;

            //False 则加入没有引用的资源缓存列表中管理
            if (!destoryCompletly)
            {
                m_resourcesMapItemList.InsertToHead(mResourceItem);
                return;
            }

            //释放在AssetBundle里面的引用
            MAssetBundleManager.singleton.ReleaseAsset(mResourceItem);

            if (mResourceItem.m_object != null)
            {
                mResourceItem.m_object = null;
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
            }
        }

#if UNITY_EDITOR
        //编辑器模式下 直接从本地读取资源
        protected T LoadAssetFormEditor<T>(string path) where T : UnityEngine.Object
        {
            var resData = AssetDatabase.LoadAssetAtPath<T>(path);
            if (resData == null)
            {
                MDebug.singleton.AddErrorLog("本地读取资源为空 Path :" + path);
            }
            return resData;
        }
#endif

        protected MResourceItem GetCacheResourceItem(uint crc,int addRefCount = 1)
        {
            MResourceItem mResourceItem = null;
            if (m_resourcesItemDic.TryGetValue(crc, out mResourceItem) && mResourceItem != null)
            {
                mResourceItem.RefCount += addRefCount;
                mResourceItem.m_lastUseTime = Time.realtimeSinceStartup;

                //容错处理 理论上不会进来这个判定
                if (mResourceItem.RefCount <= 1)
                {
                    m_resourcesMapItemList.Remove(mResourceItem);
                }
            }
            return mResourceItem;
        }
    }
}
