// **************************************
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
        private bool m_isLoadFormAssetBundle = true;

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
            //同步加载初始化
            bool bSync = InitSyncManager();
            //异步加载初始化
            bool bAsync = InitAsyncManager();

            return bSync && bAsync;
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
        /// 同步加载相基础数据初始化
        /// </summary>
        public bool InitSyncManager() 
        {
            m_resourcesMapItemList = new CMapList<MResourceItem>();
            m_resourcesItemDic = new Dictionary<uint, MResourceItem>();
            return true;
        }

        /// <summary>
        /// 清空双项列表 在场景切换的时候需要做的操作
        /// </summary>
        public void Clear() 
        {
            //todo
            //这个函数目前有问题
            /*
            while (m_resourcesMapItemList.Size() > 0) 
            {
                MResourceItem mResourceItem = m_resourcesMapItemList.GetTail();
                DestoryResourceItem(mResourceItem,true);
                m_resourcesMapItemList.PopTail();
            } 
            */
        }

        /// <summary>
        /// 资源的预加载 这里和同步加载微小区别 就是不需要引用计数 设置该预先加载的资源 在卸载的时候不卸载
        /// 就是直接加载 然后卸载
        /// </summary>
        /// <param name="resPath">Res path.</param>
        public void PreLoadRes(string resPath) 
        {
            if (string.IsNullOrEmpty(resPath))
                return;
            uint crc = MCrcHelper.GetCRC32(resPath);
            //预加载到内存中 不需要进行应用计数
            MResourceItem mResourceItem = GetCacheResourceItem(crc,0);
            if (mResourceItem != null)
            {
                return;
            }

            Object obj = null;
#if UNITY_EDITOR
            //编辑器模式下 直接从本地拿到资源
            if (!m_isLoadFormAssetBundle)
            {
                mResourceItem = MAssetBundleManager.singleton.FindResourceItem(crc);
                if (mResourceItem.m_object != null)
                {
                    if (mResourceItem.m_object != null)
                    {
                        obj = mResourceItem.m_object;
                    }
                    else
                    {
                        obj = LoadAssetFormEditor<Object>(resPath);
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
                        obj = mResourceItem.m_object;
                    }
                    else
                    {
                        obj = mResourceItem.m_assetBundle.LoadAsset<Object>(mResourceItem.m_assetName);
                    }
                }
            }

            CacheResource(resPath, ref mResourceItem, crc, obj);
            mResourceItem.m_clear = false;
            //切换场景不清空缓存
            ReleaseResource(obj, false);
        }

        /// <summary>
        /// 同步加载资源 针对给ObjectManager
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="mResourceObjectItem">ObjecyResItem</param>
        public MResourceObjectItem LoadToResourceObject(string resPath, MResourceObjectItem mResourceObjectItem) 
        {
            if (mResourceObjectItem == null)
                return null;

            uint crc = mResourceObjectItem.m_crc == 0 ? MCrcHelper.GetCRC32(resPath) : mResourceObjectItem.m_crc;

            MResourceItem mResourceItem = GetCacheResourceItem(crc);
            if(mResourceItem != null && mResourceItem.m_object != null) 
            {
                mResourceObjectItem.m_resItem = mResourceItem;
                return mResourceObjectItem;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (!m_isLoadFormAssetBundle)
            {
                mResourceItem = MAssetBundleManager.singleton.FindResourceItem(crc);
                if (mResourceItem.m_object != null) 
                {
                    obj = mResourceItem.m_object as Object;
                }
                else 
                {
                    obj = LoadAssetFormEditor<Object>(mResourceItem.m_path);
                }
            }
#endif
            if(obj == null) 
            {
                mResourceItem = MAssetBundleManager.singleton.LoadResourcesAssetBundle(crc);
                if (mResourceItem.m_object != null)
                {
                    obj = mResourceItem.m_object as Object;
                }
                else
                {
                    obj = mResourceItem.m_assetBundle.LoadAsset<Object>(mResourceItem.m_path);
                }
            }

            CacheResource(resPath,ref mResourceItem, crc, obj);
            mResourceItem.m_clear = mResourceObjectItem.m_isClear;
            mResourceObjectItem.m_resItem = mResourceItem;
            return mResourceObjectItem;
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
        /// 给ObjectManager的释放对象的接口 依据MResourceObjectItem对对象做释放
        /// </summary>
        /// <param name="mResourceObjectItem">ObjectManager 对象</param>
        /// <param name="destoryCompletly">是否彻底删除</param>
        public bool ReleaseResource(MResourceObjectItem mResourceObjectItem, bool destoryCompletly = false) 
        {
            if (mResourceObjectItem == null || mResourceObjectItem.m_resItem == null || mResourceObjectItem.m_resItem.m_object == null)
                return false;

            MResourceItem mResourceItem = null;
            if (!m_resourcesItemDic.TryGetValue(mResourceObjectItem.m_crc, out mResourceItem) && mResourceItem != null)
            {
                MDebug.singleton.AddErrorLog(" m_resourcesItemDic 不存在这个资源 resPath : " + mResourceItem.m_path);
                return false;
            }
            Object.Destroy(mResourceObjectItem.m_gameObeject);
            mResourceItem.RefCount--;
            DestoryResourceItem(mResourceItem, destoryCompletly);
            return true;
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
        /// 依据文件路径 不需要实例化的资源的卸载
        /// </summary>
        /// <param name="resPath">Res path.</param>
        /// <param name="destoryCompletly">If set to <c>true</c> destory completly.</param>
        public bool ReleaseResource(string resPath, bool destoryCompletly = false)
        {
            if (string.IsNullOrEmpty(resPath))
                return false;
            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceItem mResourceItem = null;
            if (!m_resourcesItemDic.TryGetValue(crc, out mResourceItem) && mResourceItem != null)
            {
                MDebug.singleton.AddErrorLog(" m_resourcesItemDic 不存在这个资源 resPath : " + resPath);
                return false;
            }
            mResourceItem.RefCount--;
            DestoryResourceItem(mResourceItem, destoryCompletly);
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

        /// <summary>
        /// 依据MResourceObjectItem增加Crc
        /// </summary>
        /// <returns></returns>
        public int InCreaseResourceRef(MResourceObjectItem mResourceObjectItem, int count = 1)
        {
            if (mResourceObjectItem == null)
                return 0;
            return InCreaseResourceRef(mResourceObjectItem.m_crc, count);
        }

        /// <summary>
        /// 提供一个操作引用计数的方法
        /// </summary>
        /// <param name="crc">crc</param>
        /// <param name="Count">计数</param>
        /// <returns></returns>
        public int InCreaseResourceRef(uint crc, int count = 1)
        {
            MResourceItem mResourceItem = null;
            if (!m_resourcesItemDic.TryGetValue(crc, out mResourceItem) && mResourceItem != null)
            {
                return 0;
            }
            mResourceItem.RefCount += count;
            mResourceItem.m_lastUseTime = Time.realtimeSinceStartup;

            return mResourceItem.RefCount;
        }


        /// <summary>
        /// 依据MResourceObjectItem减少Crc
        /// </summary>
        /// <returns></returns>
        public int DeCreaseResourceRef(MResourceObjectItem mResourceObjectItem, int count = 1)
        {
            if (mResourceObjectItem == null)
                return 0;
            return DeCreaseResourceRef(mResourceObjectItem.m_crc, count);
        }

        /// <summary>
        /// 提供一个操作引用计数的方法
        /// </summary>
        /// <param name="crc">crc</param>
        /// <param name="Count">计数</param>
        /// <returns></returns>
        public int DeCreaseResourceRef(uint crc, int count = 1)
        {
            MResourceItem mResourceItem = null;
            if (!m_resourcesItemDic.TryGetValue(crc, out mResourceItem) && mResourceItem != null)
            {
                return 0;
            }
            mResourceItem.RefCount -= count;

            return mResourceItem.RefCount;
        }
    }
}
