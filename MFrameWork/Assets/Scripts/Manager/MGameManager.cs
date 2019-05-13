using System.Collections;
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
            //练习函数
            Test();
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
            m_managerList.Add(MUIManager.singleton);
            m_managerList.Add(MResManager.singleton);
            m_managerList.Add(MLuaManager.singleton);
            m_managerList.Add(MObjectManager.singleton);
            m_managerList.Add(MAssetBundleManager.singleton);
            m_managerList.Add(MResourceManager.singleton);
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

        //------以下为测试逻辑--------

        public AudioSource mAudioSource;

        public void Test() 
        {
            ShowUIPanelTest();
        }

        public void ShowUIPanelTest()
        {
            MUIManager.singleton.ActiveUI("LoginPanel");
        }

        public void ResourceTest() 
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "同步加载"))
            {
                SyncResourceTest();
            }
            if (GUI.Button(new Rect(10, 70, 200, 40), "异步加载"))
            {
                AsyncResourceTest();
            }
            if (GUI.Button(new Rect(10, 130, 200, 40), "资源彻底卸载"))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, true);
                mAudioSource.clip = null;
            }
            if (GUI.Button(new Rect(10, 200, 200, 40), "资源进入缓存"))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, false);
                mAudioSource.clip = null;
            }
            if (GUI.Button(new Rect(10, 260, 200, 40), "资源预加载"))
            {
                PreResourceLoad();
            }

            if (GUI.Button(new Rect(10, 320, 200, 40), "实例化资源加载"))
            {
                ObjectResload();
            }

            if (GUI.Button(new Rect(10, 380, 200, 40), "实例化资源释放ToPool"))
            {
                ObjectResRelease();
            }

            if (GUI.Button(new Rect(10, 440, 200, 40), "实例化资源彻底释放"))
            {
                ObjectResReleaseCompletely();
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
            MResourceManager.singleton.AsyncLoadResource("Assets/Resources/Sound/lemon.mp3",AsyncLoadFinish,LoadResPriority.RES_LOAD_LEVEL_HEIGHT,null);
        }

        public void AsyncLoadFinish(string resPath, UnityEngine.Object loadedObj, object[] parms = null) 
        {

            AudioClip audioClip = loadedObj as AudioClip;
            mAudioSource.clip = audioClip;
            mAudioSource.Play();
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
            testObjectResload = MObjectManager.singleton.InstantiateGameObeject("Assets/Resources/UI/Prefabs/TestPrefab.prefab", true);
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
    }
}
