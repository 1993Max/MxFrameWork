﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public class MGameManager : MonoBehaviour 
    {
        //全部保存所有Singleton的List
        private List<MBaseSingleton> m_managerList = null;

        //全部保存Mono的一个引用
        public static MonoBehaviour m_monoBehavior;

        public List<MBaseSingleton> ManagerList
        {
            get
            {
                return m_managerList;
            }
        }

        private void Awake()
    	{
            m_monoBehavior = this;
            SetManagerList();
        }

		private void Start()
		{

        }

        public void OnGUI()
        {
            ResourceTest();
        }

        private void Update()
        {

        }

        public void SetManagerList()
        {
            if (m_managerList == null)
            {
                m_managerList = new List<MBaseSingleton>();
            }
            m_managerList.Add(MObjectManager.singleton);
            m_managerList.Add(MAssetBundleManager.singleton);
            m_managerList.Add(MFileLoadManager.singleton);
            m_managerList.Add(MResourceManager.singleton);
            m_managerList.Add(MLuaManager.singleton);
            m_managerList.Add(MUIManager.singleton);
            m_managerList.Add(MSceneManager.singleton);
            ManagerInit();
        }

        private void ManagerInit()
        {
            for (int i = 0; i < ManagerList.Count; i++)
            {
                if (!ManagerList[i].Init()) 
                {
                    MDebug.singleton.AddErrorLog("小老弟~ 这个Manager初始化出问出问题了 快去看看 Name ： " + ManagerList[i].GetType().Name); 
                }
            }
        }

        private void ManagerUnInit()
        {
            for (int i = 0; i < ManagerList.Count; i++)
            {
                ManagerList[i].UnInit();
            }
        }

        #region 逻辑的测试代码
        //------以下为测试逻辑--------
        public AudioSource mAudioSource;
        public void ResourceTest() 
        {
            if (GUI.Button(new Rect(10, 10, 200, 50), "同步加载"))
            {
                SyncResourceTest();
            }
            if (GUI.Button(new Rect(10, 70, 200, 50), "异步加载"))
            {
                AsyncResourceTest();
            }
            if (GUI.Button(new Rect(10, 130, 200, 50), "资源彻底卸载"))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, true);
                mAudioSource.clip = null;
            }
            if (GUI.Button(new Rect(10, 200, 200, 50), "资源进入缓存"))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, false);
                mAudioSource.clip = null;
            }
            if (GUI.Button(new Rect(10, 260, 200, 50), "资源预加载"))
            {
                PreResourceLoad();
            }

            if (GUI.Button(new Rect(10, 320, 200, 50), "实例化资源加载"))
            {
                ObjectResload();
            }

            if (GUI.Button(new Rect(10, 380, 200, 50), "实例化资源异步加载"))
            {
                AsyncObjectResload();
            }

            if (GUI.Button(new Rect(10, 440, 200, 50), "实例化资源释放ToPool"))
            {
                ObjectResRelease();
            }

            if (GUI.Button(new Rect(10, 500, 200, 50), "实例化资源彻底释放"))
            {
                ObjectResReleaseCompletely();
            }

            if (GUI.Button(new Rect(10, 560, 200, 50), "实例化资源预加载"))
            {
                MObjectManager.singleton.PreLoadGameObject("Assets/Resources/UI/Prefabs/LoginPanel.prefab", 10);
            }

            if (GUI.Button(new Rect(10, 620, 200, 50), "UIManager测试"))
            {
                MUIManager.singleton.ActiveUI("LoginPanel.prefab");
            }
        }

        //同步资源测试
        public void SyncResourceTest()
        {
            float begin = Time.realtimeSinceStartup;
            AudioClip audioClip = MResourceManager.singleton.LoadResource<AudioClip>("Assets/Resources/Sound/lemon.mp3");
            mAudioSource.clip = audioClip;
            mAudioSource.Play();
            MDebug.singleton.AddErrorLog("同步加载时间 Time = " + (Time.realtimeSinceStartup - begin));
        }

        //异步资源加载测试
        public void AsyncResourceTest() 
        {
            MResourceManager.singleton.AsyncLoadResource("Assets/Resources/Sound/lemon.mp3",delegate(string resPath, UnityEngine.Object loadedObj, object[] parms)
            {
                AudioClip audioClip = loadedObj as AudioClip;
                mAudioSource.clip = audioClip;
                mAudioSource.Play();

            },LoadResPriority.RES_LOAD_LEVEL_HEIGHT,null);
        }

        //资源预加载测试 资源预加载之后 主动加载的时间明显变少 
        public void PreResourceLoad()
        {
            MResourceManager.singleton.PreLoadRes("Assets/Resources/Sound/lemon.mp3");
        }

        //测试需要实例化的Object资源加载
        public GameObject testObjectResload;
        public void ObjectResload()
        {
            testObjectResload = MObjectManager.singleton.InstantiateGameObeject("Assets/Resources/UI/Prefabs/LoginPanel.prefab", true);
        }

        //异步资源加载
        public void AsyncObjectResload()
        {
            MObjectManager.singleton.InstantiateGameObejectAsync("Assets/Resources/UI/Prefabs/LoginPanel.prefab", delegate (string resPath, MResourceObjectItem mResourceObjectItem,object[] parms)
             {
                 testObjectResload = mResourceObjectItem.m_cloneObeject;
             },LoadResPriority.RES_LOAD_LEVEL_HEIGHT,true);
        }

        //清除资源并加入到资源池
        public void ObjectResRelease()
        {
            MObjectManager.singleton.ReleaseObject(testObjectResload);
        }

        //彻底清除Object资源
        public void ObjectResReleaseCompletely()
        {
            MObjectManager.singleton.ReleaseObject(testObjectResload,0,true);
        }
        #endregion
    }
}
