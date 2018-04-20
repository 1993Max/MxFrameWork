using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MFrameWork
{
    /// <summary>
    /// Resource Info 资源信息
    /// </summary>
    public class MAssetInfo
    {
        //Resource 资源
        public object MAsset;   
        //Is Keep in memory 是否常驻内存
        public bool MIsKeepInMemory;
        //The Count of using resource heap 资源堆的引用计数
        public int MrefCount;
    }


    /// <summary>
    /// ResourceLoaded Info 资源加载信息
    /// </summary>
    public class MRequestInfo
    {
        //ResourceRequestInfo 资源反馈信息
        public ResourceRequest MRequest;

        //Is Keep in memory   是否常驻于内存
        public bool MIsKeepInMemory;

        //Loaded CallBack     加载完成后的回调
        public List<MIResLoadListener> MListeners;

        //AddListener         添加资源监听者
        public void AddListener(MIResLoadListener listener)
        {
            if (MListeners == null)
            {
                MListeners = new List<MIResLoadListener>() { listener };
            }
            else
            {
                if (!MListeners.Contains(listener))
                {
                    MListeners.Add(listener);
                }
            }
        }

        //assetPath 资源路径(名称)
        public string MAssetPath;

        //assetType 资源类型
        public Type MAssetType;
        
        // isLoad Finish 资源是否加载完成
        public bool MIsDone
        {
            get
            {
                return (MRequest != null && MRequest.isDone);
            }
        }

        //Loaded 加载到的资源
        public object MAsset
        {
            get
            {
                return MRequest != null ? MRequest.asset : null;
            }
        }

        // LoadAsync 异步加载
        public void LoadAsync()
        {
            MRequest = Resources.LoadAsync(MAssetPath, MAssetType);
        }
    }

}
