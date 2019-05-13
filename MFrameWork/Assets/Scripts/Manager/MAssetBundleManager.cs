// **************************************
//
// 文件名(MAssetBundleManager.cs):
// 功能描述("MAssetBundle管理类 在初始化的时候加载Ab的打包文件 "):
// 作者(Max1993):
// 日期(2019/5/2  21:33):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MFrameWork
{
    public class MAssetBundleManager : MSingleton<MAssetBundleManager>
    {
        //存储之前打包Ab的时候 记录好的Ab信息文件 Key是资源的全路径的Crc
        protected Dictionary<uint, MResourceItem> m_resourcesItemDic = null;

        //存储已经加载到内存中的Ab的信息和每一个Ab的引用计数 Key是AssetBunleName的Crc
        protected Dictionary<uint, MAssetBundleItem> m_assetBundleItemDic = null;

        //AssetBundleItem类对象池
        protected MClassObjectPool<MAssetBundleItem> m_assetBundleItemPool = null;

        public override bool Init()
        {
            m_assetBundleItemDic = new Dictionary<uint, MAssetBundleItem>();
            m_assetBundleItemPool = new MClassObjectPool<MAssetBundleItem>(500);
            bool loadConfig = LoadAssetBundleConfig();

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

        //从AB文件中加载AB的所有信息文件
        public bool LoadAssetBundleConfig()
        {
            if (m_resourcesItemDic == null)
                m_resourcesItemDic = new Dictionary<uint, MResourceItem>();

            m_resourcesItemDic.Clear();
            string abConfigPath = MPathUtils.ASSETBUNDLE_PATH + "/" + MPathUtils.ASSETBUNDLE_AB_DATA_NAME;
            AssetBundle abData = AssetBundle.LoadFromFile(abConfigPath);
            if (abData != null)
            {
                TextAsset textAsset = abData.LoadAsset<TextAsset>(MPathUtils.ASSETBUNDLE_AB_BYTES_NAME);
                if (textAsset)
                {
                    MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MAssetBundleConfig mAssetBundleConfig = (MAssetBundleConfig)binaryFormatter.Deserialize(memoryStream);
                    memoryStream.Close();

                    for (int i = 0; i < mAssetBundleConfig.AssetBundleList.Count; i++)
                    {
                        MAssetBundleBase mAssetBundleBase = mAssetBundleConfig.AssetBundleList[i];
                        MResourceItem mResource = new MResourceItem();
                        mResource.m_crc = mAssetBundleBase.Crc;
                        mResource.m_abName = mAssetBundleBase.AbName;
                        mResource.m_assetName = mAssetBundleBase.AssetName;
                        mResource.m_path = mAssetBundleBase.Path;
                        mResource.m_abDependence = mAssetBundleBase.AbDependence;
                        if (m_resourcesItemDic.ContainsKey(mResource.m_crc))
                        {
                            MDebug.singleton.AddErrorLog("已经添加了这个数据信息 ：AbName " + mResource.m_abName + "资源名 " + mResource.m_assetName);
                        }
                        else
                        {
                            m_resourcesItemDic.Add(mResource.m_crc, mResource);
                        }

                    }
                    return true;
                }
                else
                {
                    MDebug.singleton.AddErrorLog("这个AB文件中找不到这个资源 ResName " + MPathUtils.ASSETBUNDLE_AB_BYTES_NAME);
                    return false;
                }
            }
            else
            {
                MDebug.singleton.AddErrorLog("请检查AB文件找不到这个Ab Path " + abConfigPath);
                return false;
            }
        }

        //根据文件Crc加载一个ResourcesItem出来 也会加载这个文件的所有依赖Item
        public MResourceItem LoadResourcesAssetBundle(uint crc) 
        {
            MResourceItem mResourceItem = null;
            if (!m_resourcesItemDic.TryGetValue(crc, out mResourceItem) || mResourceItem == null)
            {
                MDebug.singleton.AddErrorLog("没有找到Crc " + crc.ToString());
                return mResourceItem;
            }

            if (mResourceItem.m_assetBundle == null)
            {
                mResourceItem.m_assetBundle = LoadAssetBundle(mResourceItem.m_abName);
                if (mResourceItem.m_abDependence != null)
                {
                    for (int i = 0; i < mResourceItem.m_abDependence.Count; i++)
                    {
                        //Dependence存储的是依赖的ABName
                        LoadAssetBundle(mResourceItem.m_abDependence[i]);
                    }
                }
                return mResourceItem;
            }
            return mResourceItem;
        }

        //根据单个AbName 加载AB
        private AssetBundle LoadAssetBundle(string assetBundleName) 
        {
            MAssetBundleItem assetBundleItem = null;
            uint crc = MCrcHelper.GetCRC32(assetBundleName);

            if(!m_assetBundleItemDic.TryGetValue(crc,out assetBundleItem))
            {
                AssetBundle assetBundle = null;
                string fullPath = MPathUtils.ASSETBUNDLE_PATH + "/" + assetBundleName;

                if (File.Exists(fullPath))
                {
                    assetBundle = AssetBundle.LoadFromFile(fullPath);
                }

                if (assetBundle == null)
                {
                    MDebug.singleton.AddErrorLog("没有找到这个AssetBundle " + assetBundleName);
                }
                assetBundleItem = m_assetBundleItemPool.Spawn(true);
                assetBundleItem.assetBundle = assetBundle;
                assetBundleItem.refCount++;
                m_assetBundleItemDic.Add(crc, assetBundleItem);
            }
            else 
            {
                assetBundleItem.refCount++;
            }

            return assetBundleItem.assetBundle;
        }

        //根据ResourceItem 做资源的释放
        public void ReleaseAsset(MResourceItem mResourceItem) 
        {
            if (mResourceItem == null)
                return;

            if (mResourceItem.m_abDependence != null && mResourceItem.m_abDependence.Count > 0)
            {
                for (int i = 0; i < mResourceItem.m_abDependence.Count; i++)
                {
                    //卸载依赖项
                    UnLoadAssetBundle(mResourceItem.m_abDependence[i]);
                }
            }
            //卸载自己
            UnLoadAssetBundle(mResourceItem.m_abName);    
        }

        //释放单个Ab资源
        private void UnLoadAssetBundle(string assetBundleName) 
        {
            MAssetBundleItem mAssetBundleItem = null;
            uint crc = MCrcHelper.GetCRC32(assetBundleName); 
            if(m_assetBundleItemDic.TryGetValue(crc,out mAssetBundleItem)) 
            {
                mAssetBundleItem.refCount--;
                if(mAssetBundleItem.refCount <= 0 && mAssetBundleItem.assetBundle != null)
                {
                    mAssetBundleItem.assetBundle.Unload(true);
                    mAssetBundleItem.Reset();
                    m_assetBundleItemPool.Recycle(mAssetBundleItem);
                    m_assetBundleItemDic.Remove(crc);
                }
            }
        }

        //根据Crc返回ResourceItem 这里Crc是资源的全路径
        public MResourceItem FindResourceItem(uint crc)
        {
            MResourceItem mResourceItem = null;
            if(m_resourcesItemDic.TryGetValue(crc,out mResourceItem)) 
            {
                return mResourceItem;
            }
            return null;
        }

        //根据资源全路径返回ResourceItem
        public MResourceItem FindResourceItem(string assetPath)
        {
            uint crc = MCrcHelper.GetCRC32(assetPath);
            MResourceItem mResourceItem = null;
            if (m_resourcesItemDic.TryGetValue(crc, out mResourceItem))
            {
                return mResourceItem;
            }
            return null;
        }
    }
}
