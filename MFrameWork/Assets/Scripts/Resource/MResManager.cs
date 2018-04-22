using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFrameWork
{
    public sealed class MResManager : MSingleton<MResManager>
    {
        public static readonly Vector3 FAR_FAR_AWAY = new Vector3(0, -1000f, 0); //for GameObject pool
        // the all asset dic 所有资源的字典
        private Dictionary<string, MAssetInfo> _mAssetDic = new Dictionary<string, MAssetInfo>();
        // the loading list of assetInfo  正在加载中的资源列表
        private List<MRequestInfo> _mLoadList = new List<MRequestInfo>();
        // the waiting list for loading 等待加载的列表
        private Queue<MRequestInfo> _mWaitQueue = new Queue<MRequestInfo>();

        public override void Init()
        {
            InitGameRes();
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        /// <summary>
        /// Init GameRes 初始化游戏资源
        /// </summary>
        public void InitGameRes()
        {

        }

        #region Resource Load 资源加载
        public void Load(string assetPath,MIResLoadListener listener,bool isAsyc = true,bool isKeepInMemory = false, Type assettype = null)
        {
            if (_mAssetDic.ContainsKey(assetPath))
            {
                listener.Finished(_mAssetDic[assetPath]);
                return;
            }
            if (isAsyc)
            {
                LoadAsync(assetPath, listener, assettype, isKeepInMemory);
            }
        }

        public void LoadAsync(string assetPath, MIResLoadListener listener, Type assetType, bool isKetInMemory)
        {
            //判断是否在正在加载的加载列表中
            for (int i = 0; i < _mLoadList.Count; i++)
            {
                if (_mLoadList[i].MAssetPath == assetPath)
                {
                    _mLoadList[i].AddListener(listener);
                    return;
                }
            }
            //判断是否在等待加载的家在列表中
            foreach (MRequestInfo info in _mWaitQueue)
            {
                if (info.MAssetPath == assetPath)
                {
                    info.AddListener(listener);
                    return;
                }
            }

            MRequestInfo requestInfo = new MRequestInfo();
            requestInfo.MAssetPath = assetPath;
            requestInfo.AddListener(listener);
            requestInfo.MIsKeepInMemory = isKetInMemory;
            requestInfo.MAssetType = assetType == null ? typeof(GameObject) : assetType;
            _mWaitQueue.Enqueue(requestInfo);
        }
        #endregion

        #region 资源处理

        #endregion
    }
}
