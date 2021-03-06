﻿// **************************************
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
using System.Collections.Generic;

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
                    GameObject obj = GameObject.Find(MPathUtils.RECYCLE_POOL_TRANSFORM);
                    Object.DontDestroyOnLoad(obj);
                    return obj.transform;
                }
                return m_recycleObjectPoolTrans;
            }
        }

        private Transform m_defaultObjectTrans = null;

        public Transform DefaultObjectTrans
        {
            get
            {
                if (m_defaultObjectTrans == null)
                {
                    GameObject obj = GameObject.Find(MPathUtils.DEFAULT_OBJECT_TRANSFORM);
                    return obj.transform;
                }
                return m_defaultObjectTrans;
            }
        }

        /// <summary>
        /// 所有实例化MResourceObjectItem的存储 key是ResPath的Crc Value是MResourceObjectItem
        /// </summary>
        protected Dictionary<uint, List<MResourceObjectItem>> m_resourcesItemPoolDic = null;
        /// <summary>
        /// MResourceObjectItem的类对象池
        /// </summary>
        protected MClassObjectPool<MResourceObjectItem> m_resourceObjectClssPool = null;
        /// <summary>
        /// 保存MResourceObjectItem的Dic Key是实例化GameObject的guid 方便查找
        /// </summary>
        protected Dictionary<int, MResourceObjectItem> m_resourceObjectDic = null;

        public bool InitSyncObjectManager()
        {
            m_resourcesItemPoolDic = new Dictionary<uint, List<MResourceObjectItem>>();
            m_resourceObjectClssPool = new MClassObjectPool<MResourceObjectItem>(100);
            m_resourceObjectDic = new Dictionary<int, MResourceObjectItem>();
            Object.DontDestroyOnLoad(DefaultObjectTrans);
            Object.DontDestroyOnLoad(RecycleObjectPoolTrans);
            return true;
        }

        /// <summary>
        /// objectManager 提供一个清除缓存的接口
        /// </summary>
        public void ClearCatch()
        {
            List<uint> tempList = new List<uint>();
            foreach (var key in m_resourcesItemPoolDic.Keys)
            {
                List<MResourceObjectItem> mResourceObjectLsit = m_resourcesItemPoolDic[key];
                for (int i = mResourceObjectLsit.Count-1; i >= 0; i--)
                {
                    MResourceObjectItem mResourceObjectItem = mResourceObjectLsit[i];
                    m_resourceObjectDic.Remove(mResourceObjectItem.m_cloneObeject.GetInstanceID());
                    mResourceObjectItem.Reset();
                    m_resourceObjectClssPool.Recycle(mResourceObjectItem);
                }

                if (mResourceObjectLsit.Count <= 0)
                {
                    tempList.Add(key);
                }
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                uint tempKey = tempList[i];
                if (m_resourcesItemPoolDic.ContainsKey(tempKey))
                {
                    m_resourcesItemPoolDic.Remove(tempKey);
                }
            }

            tempList.Clear();
        }

        /// <summary>
        /// 清除某个对象在对象池中的所有对象
        /// </summary>
        /// <param name="resCrc">资源路径Crc</param>
        public void ClearPoolObject(uint resCrc)
        {
            List<MResourceObjectItem> mResourceObjectLsit = null;
            if (!m_resourcesItemPoolDic.TryGetValue(resCrc, out mResourceObjectLsit) || mResourceObjectLsit == null)
                return;
            for (int i = mResourceObjectLsit.Count - 1; i >= 0; i--)
            {
                MResourceObjectItem mResourceObjectItem = mResourceObjectLsit[i];
                if (mResourceObjectItem.m_isClear)
                {
                    mResourceObjectLsit.Remove(mResourceObjectItem);
                    mResourceObjectItem.Reset();
                    m_resourceObjectDic.Remove(mResourceObjectItem.m_cloneObeject.GetInstanceID());
                    m_resourceObjectClssPool.Recycle(mResourceObjectItem);
                    GameObject.Destroy(mResourceObjectItem.m_cloneObeject);
                }
            }

            if (mResourceObjectLsit.Count <= 0)
            {
                if (m_resourcesItemPoolDic.ContainsKey(resCrc))
                {
                    m_resourcesItemPoolDic.Remove(resCrc);
                }
            }
        }

        /// <summary>
        /// 预加载GamObject
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="addCount">预加载数量</param>
        /// <param name="isChangeSceneClear">且场景是否清除</param>
        public void PreLoadGameObject(string resPath,int addCount = 1,bool isChangeSceneClear = true)
        {
            List<GameObject> tempGameObjects = new List<GameObject>();
            for (int i = 0; i < addCount; i++)
            {
                GameObject obj = InstantiateGameObeject(resPath, false, isChangeSceneClear);
                tempGameObjects.Add(obj);
            }

            for (int i = 0; i < tempGameObjects.Count; i++)
            {
                ReleaseObject(tempGameObjects[i].gameObject);
                tempGameObjects[i] = null;
            }
        }

        /// <summary>
        /// 同步加载GameObject的方法
        /// </summary>
        /// <returns>The game obeject.</returns>
        /// <param name="resPath">资源路径</param>
        /// <param name="isSetToDefault">是否实例化到默认节点</param>
        /// <param name="isChangeSceneClear">在切换场景的时候是否清楚资源的缓存</param>
        public GameObject InstantiateGameObeject(string resPath,bool isSetToDefault = false,bool isChangeSceneClear=true) {
            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceObjectItem mResourceObjectItem = GetObjectFromPool(crc);

            if(mResourceObjectItem == null) 
            {
                mResourceObjectItem = m_resourceObjectClssPool.Spawn(true);
                mResourceObjectItem.m_crc = crc;
                mResourceObjectItem.m_isClear = isChangeSceneClear;
                mResourceObjectItem = MResourceManager.singleton.LoadToResourceObject(resPath, mResourceObjectItem);

                if (mResourceObjectItem.m_resItem.m_object != null) 
                {
                    mResourceObjectItem.m_cloneObeject = (GameObject)Object.Instantiate(mResourceObjectItem.m_resItem.m_object);
                    mResourceObjectItem.m_instanceId = mResourceObjectItem.m_cloneObeject.GetInstanceID();
                    mResourceObjectItem.m_resOffLineData = mResourceObjectItem.m_cloneObeject.GetComponent<MResOffLineDataBase>();
                }
            }

            if (isSetToDefault) 
            {
                mResourceObjectItem.m_cloneObeject.transform.SetParent(DefaultObjectTrans,false);
            }

            if (!m_resourceObjectDic.ContainsKey(mResourceObjectItem.m_instanceId)) 
            {
                m_resourceObjectDic.Add(mResourceObjectItem.m_instanceId, mResourceObjectItem);
            }
            return mResourceObjectItem.m_cloneObeject;
        }

        /// <summary>
        /// 存ResobjecyItem对象池中取出对象
        /// </summary>
        /// <returns>返回 MResourceObjectItem 对象</returns>
        /// <param name="crc">资源的ResPath的Crc</param>
        public MResourceObjectItem GetObjectFromPool(uint crc)
        {
            List<MResourceObjectItem> mResourceObjectItemList = null;
            if (m_resourcesItemPoolDic.TryGetValue(crc, out mResourceObjectItemList) && mResourceObjectItemList.Count > 0)
            {
                MResourceManager.singleton.InCreaseResourceRef(crc);
                MResourceObjectItem resObj = mResourceObjectItemList[0];
                mResourceObjectItemList.RemoveAt(0);
                GameObject gameObject = resObj.m_cloneObeject;
                if (!System.Object.ReferenceEquals(gameObject, null))
                {
                    if (!System.Object.ReferenceEquals(resObj.m_resOffLineData,null))
                    {
                        resObj.m_resOffLineData.ResetBasicData();
                    }
                    resObj.m_isAlreadyRelease = false;
#if UNITY_EDITOR
                    if (gameObject.name.EndsWith("(Recycle)"))
                    {
                        gameObject.name = gameObject.name.Replace("(Recycle)", "");
                    }
#endif
                }
                return resObj;
            }
            return null;
        }

        /// <summary>
        /// 彻底删除某一个Go
        /// </summary>
        /// <param name="gameObject"></param>
        public void ReleaseObjectComopletly(GameObject gameObject)
        {
            ReleaseObject(gameObject, -1, true);
        }

        /// <summary>
        /// ObjectManager的释放 对创建出来的Gameobject的释放
        /// </summary>
        /// <param name="gameObject">释放对象</param>
        /// <param name="maxCatchCount">最大缓存个数</param>
        /// <param name="destoryCompletly">是否彻底删除</param>
        /// <param name="isToRecycleParent">是非设置默认父节点</param>
        public void ReleaseObject(GameObject gameObject,int maxCatchCount = -1,bool destoryCompletly = false,bool isToRecycleParent = true) 
        {
            if (gameObject == null)
                return;

            int tempInstanceId = gameObject.GetInstanceID();
            MResourceObjectItem mResourceObjectItem = null;
            if(!m_resourceObjectDic.TryGetValue(tempInstanceId, out mResourceObjectItem))
            {
                MDebug.singleton.AddErrorLog("这个对象不是用ObjectManager创建的GameObject Name：" + gameObject.name);
                return;
            }
            if(mResourceObjectItem == null) 
            {
                MDebug.singleton.AddErrorLog("本地没有缓存这个GameObject Name：" + gameObject.name);
                return; 
            }
            if (mResourceObjectItem.m_isAlreadyRelease) 
            {
                MDebug.singleton.AddErrorLog("重复释放对象 Name：" + gameObject.name);
                return;
            }

#if UNITY_EDITOR
            gameObject.name += "(Recycle)";
#endif
            List<MResourceObjectItem> listObjectItems = null;
            //maxCatchCount == 0 说明不缓存 对象释放
            if (maxCatchCount == 0)
            {
                m_resourceObjectDic.Remove(tempInstanceId);
                MResourceManager.singleton.ReleaseResource(mResourceObjectItem, destoryCompletly);
                mResourceObjectItem.Reset();
                m_resourceObjectClssPool.Recycle(mResourceObjectItem);
            }
            else
            {
                if (!m_resourcesItemPoolDic.TryGetValue(mResourceObjectItem.m_crc, out listObjectItems) || listObjectItems == null)
                {
                    listObjectItems = new List<MResourceObjectItem>();
                    m_resourcesItemPoolDic.Add(mResourceObjectItem.m_crc, listObjectItems);
                }

                if (mResourceObjectItem.m_cloneObeject != null)
                {
                    if (isToRecycleParent)
                    {
                        mResourceObjectItem.m_cloneObeject.transform.SetParent(RecycleObjectPoolTrans);
                    }
                    else
                    {
                        mResourceObjectItem.m_cloneObeject.SetActiveEx(false);
                    }
                }

                //没有达到最大缓存个数的时候 进行缓存
                if (maxCatchCount < 0 || listObjectItems.Count < maxCatchCount)
                {
                    listObjectItems.Add(mResourceObjectItem);
                    mResourceObjectItem.m_isAlreadyRelease = true;
                    MResourceManager.singleton.DeCreaseResourceRef(mResourceObjectItem.m_crc);
                }
                else
                {
                    //多出来的资源卸载掉
                    m_resourceObjectDic.Remove(tempInstanceId);
                    MResourceManager.singleton.ReleaseResource(mResourceObjectItem, destoryCompletly);
                    mResourceObjectItem.Reset();
                    m_resourceObjectClssPool.Recycle(mResourceObjectItem);
                }
            }
            //todo
        }

        //返回当前Gameobject身上的离线数据信息
        public MResOffLineDataBase GetObjOffLineData(GameObject gameObject) 
        {
            MResOffLineDataBase mResOffLineData = null;
            MResourceObjectItem mResourceObjectItem = null;
            if(m_resourceObjectDic.TryGetValue(gameObject.GetInstanceID(), out mResourceObjectItem)) 
            {
                if (mResourceObjectItem != null) 
                {
                    mResOffLineData = mResourceObjectItem.m_resOffLineData;
                } 
            }
            return mResOffLineData;
        }
    }
}
