using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MFrameWork
{
    public class MResPool
    {
#if DEBUG
        private StringBuilder _mDebugInfo = new StringBuilder();
#endif
        //AssetBundle PoolDict
        private Dictionary<uint, MAbInfo> _mBundlePoolDict = new Dictionary<uint, MAbInfo>();
        //Asset pool except GameObject and Sprite
        private Dictionary<uint, UnityEngine.Object> _mAssetPoolDict = new Dictionary<uint, UnityEngine.Object>();
        //hash => refCount
        private Dictionary<uint, int> _mAssetRefDict = new Dictionary<uint, int>();
        //Unshared object pool
        private Dictionary<int, Queue<UnityEngine.Object>> _mObjectPoolDict = new Dictionary<int, Queue<UnityEngine.Object>>();
        //InstanceId => hash
        private Dictionary<int, uint> _mReserseAssetDict = new Dictionary<int, uint>();
        //InstanceId => parentInstId
        private Dictionary<int, int> _mReverseObjDict = new Dictionary<int, int>();
        //PoolObject TransForm
        private Transform _mPoolRootTrans;

        //Init ResPool 初始化ResPool
        public void Init()
        {
            GameObject poolRoot = new GameObject("MResPool");
            GameObject.DontDestroyOnLoad(poolRoot);
            _mPoolRootTrans = poolRoot.transform;
            _mPoolRootTrans.position = MResManager.FAR_FAR_AWAY;
        }

        //Uninit ResPool 卸载ResPool
        public void Uninit()
        {
            if (_mPoolRootTrans)
            {
                GameObject.Destroy(_mPoolRootTrans.gameObject);
                _mPoolRootTrans = null;
            }
        }

        public void ClearObjPools()
        {

        }

        public void ClearAll()
        {

        }

        // GetHash By Unity Object InstanceId 根据UnityEngine.Object的InstanceID反向获取hash
        public uint GetHash(UnityEngine.Object obj)
        {
            return GetHashByInstID(obj.GetInstanceID());
        }

        /// 根据UnityEngine.Object的InstanceID反向获取hash
        public uint GetHashByInstID(int instID)
        {
            return 1;
        }

        #region Asset Pool
        /// <summary>
        ///Get Asset in AssetPool if none return null 
        ///获取AssetPool里的Asset 没有则返回null
        /// </summary>
        public UnityEngine.Object GetAssetInPool(uint hash)
        {
            UnityEngine.Object obj = null;
            _mAssetPoolDict.TryGetValue(hash, out obj);
            return obj;
        }

        /// <summary>
        /// 添加Asset资源到AssetPool（不会重复添加）
        /// Add Asset to AssetPool
        /// </summary>
        public void AddAssetToPool(uint hash, UnityEngine.Object asset, MAbInfo abInfo = null)
        {
            if (!asset)
            {
                return;
            }
            UnityEngine.Object obj = null;
            if (!_mAssetPoolDict.TryGetValue(hash, out obj))
            {
                obj = asset;
                _mAssetPoolDict.Add(hash, obj);
                if (abInfo != null && !_mBundlePoolDict.ContainsKey(hash))
                {
                    abInfo.Retain();
                    _mBundlePoolDict.Add(hash, abInfo);
                }

                _mReserseAssetDict[obj.GetInstanceID()] = hash;
            }
        }

        /// <summary>
        /// 从AssetPool里删除Asset
        /// Remove Asset form AssetPool
        /// </summary>
        public void RemoveAssetFromPool(uint hash)
        {
            UnityEngine.Object obj = null;
            MAbInfo abInfo = null;

            if (_mAssetPoolDict.TryGetValue(hash, out obj))
            {
                if (_mBundlePoolDict.TryGetValue(hash, out abInfo))
                {
                    abInfo.Release();
                    _mBundlePoolDict.Remove(hash);
                }
                else if (!(obj is GameObject))
                {
                    Resources.UnloadAsset(obj);
                }
                _mAssetPoolDict.Remove(hash);
                _mReserseAssetDict.Remove(obj.GetInstanceID());
            }
        }

        /// <summary>
        /// 添加AssetPool对应Asset的引用
        /// </summary>
        public void AssetRefRetain(uint hash)
        {
            int refCount = 0;
            _mAssetRefDict.TryGetValue(hash, out refCount);
            refCount++;
            _mAssetRefDict[hash] = refCount;
        }

        /// <summary>
        /// 释放AssetPool对应Asset的引用, 返回是否移除asset
        /// </summary>
        public void AssetRefRelease(uint hash)
        {
            int refCount = 0;
            if (_mAssetRefDict.TryGetValue(hash, out refCount))
            {
                refCount--;
                if (refCount <= 0)
                {
                    RemoveAssetFromPool(hash);
                    _mAssetRefDict.Remove(hash);
                }
                else
                {
                    _mAssetRefDict[hash] = refCount;
                }
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentInstId"></param>
        /// <param name="obj"></param>
        private void addObjectToPool(int parentInstId, UnityEngine.Object obj)
        {
            Queue<UnityEngine.Object> queue = null;
            if (!_mObjectPoolDict.TryGetValue(parentInstId, out queue))
            {
                queue = MQueuePool<UnityEngine.Object>.Get();
                _mObjectPoolDict.Add(parentInstId, queue);
            }

            GameObject go = obj as GameObject;
            if (go)
            {
                Canvas canvas = go.GetComponent<Canvas>();
                if (canvas)
                {
                    canvas.enabled = false;
                }
                else
                {
                    Transform t = go.transform;
                    t.SetParent(_mPoolRootTrans);
                    t.localPosition = Vector3.zero;
                }
            }

            queue.Enqueue(obj);
        }

        #endregion
    }
}
