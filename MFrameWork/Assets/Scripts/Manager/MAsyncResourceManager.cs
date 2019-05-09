// **************************************
//
// 文件名(MResourceManager.cs.cs):
// 功能描述("资源管理类 异步加载"):
// 作者(Max1993):
// 日期(2019/5/9  10:24):
//
// **************************************
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MFrameWork
{
    //加载优先级枚举
    public enum LoadResPriority
    {
        RES_LOAD_LEVEL_HEIGHT = 0,   //最高优先级
        RES_LOAD_LEVEL_MEDIAL,       //中优先级
        RES_LOAD_LEVEL_NORMAL,       //普通优先级
        RES_LOAD_LEVEL_LOW,          //低优先级
        RES_LOAD_LEVEL_COUNT         //总数量
    }

    //异步加载的中间类
    public class AsyncLoadResParam
    {
        public uint m_crc;
        //保存资源路径
        public string m_resPath;
        //保存优先级
        public LoadResPriority m_loadResPriority = LoadResPriority.RES_LOAD_LEVEL_LOW;
        //假设有多个地方同时请求一个资源 这个时候产生了不同的回调内容 这里这个列表存储回调列表
        public List<AsyncCallBack> m_asyncCallBacks = new List<AsyncCallBack>();
        //存储是否是图片
        public bool m_isSprite = false;
        //数据重置
        public void Reset()
        {
            m_crc = 0;
            m_resPath = "";
            m_loadResPriority = LoadResPriority.RES_LOAD_LEVEL_LOW;
            m_isSprite = false;
            m_asyncCallBacks.Clear();
        }
    }

    public class AsyncCallBack
    {
        //加载完成的回掉
        public OnAsyncLoadFinished m_onAsyncLoadFinished = null;
        //回掉参数
        public object[] m_parms = null;
        //数据重置
        public void Reset()
        {
            m_onAsyncLoadFinished = null;
            m_parms = null;
        }
    }

    /// <summary>
    /// 异步加载完成的回调委托
    /// </summary>
    /// <param name="resPath">资源路径</param>
    /// <param name="loadedObj">加载完成的Obj</param>
    /// <param name="parms">可变参数</param>
    public delegate void OnAsyncLoadFinished(string resPath,object loadedObj,object[] parms = null);

    public partial class MResourceManager
    {
        //异步加载的初始化Mono脚本
        private MonoBehaviour m_startMono;

        //异步加载的资源列表
        protected List<AsyncLoadResParam>[] m_asyncAssetLoadingList = null;

        //存储正在异步加载中的队列
        protected Dictionary<uint, AsyncLoadResParam> m_asyncLoadingAssetDic = null;

        //异步加载中间类的类对象池
        protected MClassObjectPool<AsyncLoadResParam> m_asyncLoadResParamPool = null;

        //异步加载回掉类的类对象池
        protected MClassObjectPool<AsyncCallBack> m_asyncCallBackPool = null;

        //最长等待时间 毫秒
        public const long MAX_WAITTIME = 100000;

        /// <summary>
        /// 初始化异步管理器
        /// </summary>
        /// <returns></returns>
        public bool InitAsyncManager()
        {
            //初始化异步加载资源列表
            m_asyncAssetLoadingList = new List<AsyncLoadResParam>[(int)LoadResPriority.RES_LOAD_LEVEL_COUNT];

            m_asyncLoadingAssetDic = new Dictionary<uint, AsyncLoadResParam>();

            m_asyncLoadResParamPool = new MClassObjectPool<AsyncLoadResParam>(50);

            m_asyncCallBackPool = new MClassObjectPool<AsyncCallBack>(50);

            //初始化MonoBehavior脚本
            if (m_startMono == null)
            {
                //todo
                //m_startMono = xxx;
                return false;
            }

            for (int i = 0; i < (int)LoadResPriority.RES_LOAD_LEVEL_COUNT; i++)
            {
                m_asyncAssetLoadingList[i] = new List<AsyncLoadResParam>();
            }

            //游戏初始化 需要开启这个异步加载的协程
            m_startMono.StartCoroutine(AsyncLoader());
            return false;
        }

        /// <summary>
        /// 异步加载资源 仅加载不需要实例化的资源 音频 图片等等
        /// </summary>
        public void AsyncLoadResource(string resPath,OnAsyncLoadFinished onAsyncLoadFinished,LoadResPriority loadResPriority,Object[] parms) 
        {
            if (string.IsNullOrEmpty(resPath))
                return;

            uint crc = MCrcHelper.GetCRC32(resPath);
            MResourceItem mResourceItem = GetCacheResourceItem(crc);

            if (mResourceItem != null && mResourceItem.m_object != null)
            {
                if (onAsyncLoadFinished != null)
                {
                    onAsyncLoadFinished(resPath, mResourceItem.m_object, parms);
                }
                return;
            }

            //判断下对象是不是在加载中
            AsyncLoadResParam asyncLoadResParam = null;
            if (!m_asyncLoadingAssetDic.TryGetValue(crc, out asyncLoadResParam) || asyncLoadResParam == null)
            {
                asyncLoadResParam = m_asyncLoadResParamPool.Spawn(true);
                asyncLoadResParam.m_crc = crc;
                asyncLoadResParam.m_resPath = resPath;
                asyncLoadResParam.m_loadResPriority = loadResPriority;
                //结果保存
                m_asyncAssetLoadingList[(int)loadResPriority].Add(asyncLoadResParam);
                m_asyncLoadingAssetDic.Add(crc, asyncLoadResParam);
            }

            //添加回调
            AsyncCallBack m_asyncCallBack = m_asyncCallBackPool.Spawn(true);
            m_asyncCallBack.m_onAsyncLoadFinished = onAsyncLoadFinished;
            m_asyncCallBack.m_parms = parms;
            asyncLoadResParam.m_asyncCallBacks.Add(m_asyncCallBack);
        }

        /// <summary>
        /// 异步加载的携程
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoader()
        {
            List<AsyncCallBack> callBackList;
            //用于记录上次的加载时间
            long lastReturnTime = System.DateTime.Now.Ticks;
            while (true)
            {
                //标志位 用于判读在For循环中是否已经return过了
                bool isYieldReturn = false;

                for (int i = 0; i < (int)LoadResPriority.RES_LOAD_LEVEL_COUNT; i++)
                {
                    List<AsyncLoadResParam> cAsyncLoadResList = m_asyncAssetLoadingList[i];
                    if (m_asyncAssetLoadingList[i] != null && cAsyncLoadResList.Count > 0)
                        continue;

                    AsyncLoadResParam asyncLoadResParam = cAsyncLoadResList[0];
                    cAsyncLoadResList.RemoveAt(0);
                    callBackList = asyncLoadResParam.m_asyncCallBacks;

                    Object obj = null;
                    MResourceItem mResourceItem = null;
#if UNITY_EDITOR
                    if (!m_isLoadFormAssetBundle)
                    {
                        obj = LoadAssetFormEditor<Object>(asyncLoadResParam.m_resPath);
                        //模拟异步
                        yield return new WaitForSeconds(0.2f);

                        mResourceItem = MAssetBundleManager.singleton.FindResourceItem(asyncLoadResParam.m_crc);
                    }
#endif
                    if (obj == null)
                    {
                        mResourceItem = MAssetBundleManager.singleton.LoadResourcesAssetBundle(asyncLoadResParam.m_crc);
                        if (mResourceItem != null && mResourceItem.m_assetBundle != null)
                        {
                            AssetBundleRequest assetBundleRequest = null;
                            if (asyncLoadResParam.m_isSprite)
                            {
                                assetBundleRequest = mResourceItem.m_assetBundle.LoadAssetAsync<Sprite>(mResourceItem.m_assetName);
                            }
                            else
                            {
                                assetBundleRequest = mResourceItem.m_assetBundle.LoadAssetAsync(mResourceItem.m_assetName);
                            }
                            yield return assetBundleRequest;
                            if (assetBundleRequest.isDone)
                            {
                                obj = assetBundleRequest.asset;
                            }
                            lastReturnTime = System.DateTime.Now.Ticks;
                        }
                    }
                    //资源缓存
                    CacheResource(asyncLoadResParam.m_resPath, ref mResourceItem, asyncLoadResParam.m_crc, obj, callBackList.Count);
                    //处理加载完成的回调
                    for (int z = 0; z < callBackList.Count; z++)
                    {
                        AsyncCallBack callBack = callBackList[z];
                        if (callBack != null && callBack.m_onAsyncLoadFinished != null)
                        {
                            callBack.m_onAsyncLoadFinished(asyncLoadResParam.m_resPath, obj, callBack.m_parms);
                            callBack.m_onAsyncLoadFinished = null;
                        }
                        //异步加载CallBack对象回收
                        callBack.Reset();
                        m_asyncCallBackPool.Recycle(callBack);
                    }

                    obj = null;
                    callBackList.Clear();
                    //从正在异步加载的Dic里面移除
                    m_asyncLoadingAssetDic.Remove(mResourceItem.m_crc);

                    //异步加载中间对象回收
                    asyncLoadResParam.Reset();
                    m_asyncLoadResParamPool.Recycle(asyncLoadResParam);

                    //上下写了两个同样的Return逻辑 是因为如果加载比较大的资源 可能在这个For循环内就需要多帧 所以这里也做了一个判定
                    if (!isYieldReturn || System.DateTime.Now.Ticks - lastReturnTime > MAX_WAITTIME)
                    {
                        lastReturnTime = System.DateTime.Now.Ticks;
                        yield return null;
                        isYieldReturn = true;
                    }
                }

                if (!isYieldReturn || System.DateTime.Now.Ticks - lastReturnTime > MAX_WAITTIME)
                {
                    lastReturnTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
            }
        }
    }
}
