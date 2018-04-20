using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MFrameWork
{
    /// <summary>
    /// 资源池
    /// ResPool
    /// Info: everyObject has there own instanceId but the same object has the only hashId
    /// </summary>
    public class MResPool
    {
        //PoolObject TransForm
        private Transform _mPoolRootTrans;
        //AssetPool Dict
        private Dictionary<int, UnityEngine.Object> _mAssetPoolDict = new Dictionary<int, UnityEngine.Object>();

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
            _mAssetPoolDict.Clear();
        }

        public int GetObjInstanceId(UnityEngine.Object asset)
        {
            return asset.GetInstanceID();
        }

        /// <summary>
        /// 实例化对象
        /// </summary>
        public UnityEngine.Object CreateObj(UnityEngine.Object obj, bool usePool)
        {
            return obj;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public void DestroyObj(UnityEngine.Object obj, bool returnPool = true)
        {

        }
    }
}
