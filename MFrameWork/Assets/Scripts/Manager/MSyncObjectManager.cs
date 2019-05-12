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
                    GameObject obj = GameObject.Find(MPathUtils.RECYCLE_POOL_TRANSFORM);
                    Object.DontDestroyOnLoad(obj);
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
            return true;
        }

        /// <summary>
        /// 同步加载GameObject的方法
        /// </summary>
        /// <returns>The game obeject.</returns>
        /// <param name="resPath">资源路径</param>
        /// <param name="isSetToDefault">是否实例化到默认节点</param>
        /// <param name="isClear">在切换场景的时候是否清楚资源的缓存</param>
        public GameObject InstantiateGameObeject(string resPath,bool isSetToDefault = false ,bool isClear=true) {
            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceObjectItem mResourceObjectItem = GetObjectFromPool(crc);

            if(mResourceObjectItem == null) 
            {
                mResourceObjectItem = m_resourceObjectClssPool.Spawn(true);
                mResourceObjectItem.m_crc = crc;
                mResourceObjectItem.m_isClear = isClear;
                mResourceObjectItem = MResourceManager.singleton.LoadToResourceObject(resPath, mResourceObjectItem);

                if (mResourceObjectItem.m_resItem.m_object != null) 
                {
                    mResourceObjectItem.m_gameObeject = (GameObject)Object.Instantiate(mResourceObjectItem.m_resItem.m_object);
                    mResourceObjectItem.m_guid = mResourceObjectItem.m_gameObeject.GetInstanceID();
                }
            }

            if (isSetToDefault) 
            {
                mResourceObjectItem.m_gameObeject.transform.SetParent(DefaultObjectTrans);
            }

            if (!m_resourceObjectDic.ContainsKey(mResourceObjectItem.m_guid)) 
            {
                m_resourceObjectDic.Add(mResourceObjectItem.m_guid, mResourceObjectItem);
            }
            return mResourceObjectItem.m_gameObeject;
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
                MResourceObjectItem resObj = mResourceObjectItemList[0];
                mResourceObjectItemList.RemoveAt(0);
                GameObject gameObject = resObj.m_gameObeject;
                if (!System.Object.ReferenceEquals(gameObject, null))
                {
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
        /// ObjectManager的释放 对创建出来的Gameobject的释放
        /// </summary>
        /// <param name="gameObject">释放对象</param>
        /// <param name="maxCatchCount">最大缓存个数</param>
        /// <param name="destoryCompletly">是否彻底删除</param>
        /// <param name="isToRecycleParent">是非设置默认父节点</param>
        public void ReleaseObject(GameObject gameObject,int maxCatchCount = -1,bool destoryCompletly = false,bool isToRecycleParent = false) 
        {
            if (gameObject == null)
                return;
            MResourceObjectItem mResourceObjectItem = null;
            if(!m_resourceObjectDic.TryGetValue(gameObject.GetInstanceID(),out mResourceObjectItem))
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
            if (maxCatchCount == 0) 
            {
                m_resourceObjectDic.Remove(gameObject.GetInstanceID());
                MResourceManager.singleton.ReleaseResource(mResourceObjectItem,destoryCompletly);
                mResourceObjectItem.Reset();
                m_resourceObjectClssPool.Recycle(mResourceObjectItem);
            }
#endif
            //todo
        }

    }
}
