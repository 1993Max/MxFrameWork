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
        private Dictionary<string, Object> _mAssetDic = new Dictionary<string, Object>();

        public override bool Init()
        {
            InitGameRes();
            return base.Init();
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
        /// <summary>
        /// object loader 非常的资源加载并管理
        /// </summary>
        public T ResLoad<T>(string path, bool isCatch) where T : UnityEngine.Object
        {
            if (_mAssetDic.ContainsKey(path))
            {
                return _mAssetDic[path] as T;
            }

            T asset = Resources.Load<T>(path);
            if (asset != null)
            {
                if (isCatch)
                {
                    _mAssetDic.Add(path, asset);
                }
            }
            else
            {
                Debug.Log("---------------------->>> Can't find in res Path :" + path);
            }
            return asset;
        }

        #endregion

        #region 资源处理
        public void CreateGameObject(GameObject resObj, Transform targetParent, Vector3 pos, Vector3 scale, Vector3 rotate)
        {
            GameObject cObj = GameObject.Instantiate(resObj) as GameObject;
            cObj.transform.SetParent(targetParent);
            cObj.transform.localPosition = pos;
            cObj.transform.localScale = scale;
            cObj.transform.localRotation = Quaternion.Euler(rotate);
        }

        public void ClearAll()
        {
            _mAssetDic.Clear();
        }
        #endregion
    }
}
