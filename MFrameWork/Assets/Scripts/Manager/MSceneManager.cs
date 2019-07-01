// **************************************
//
// 文件名(MSceneManager.cs):
// 功能描述("场景的管理类"):
// 作者(Max1993):
// 日期(2019/6/30  13:31):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace MFrameWork 
{
    public class MSceneManager : MSingleton<MSceneManager>
    {
        //保存当前的场景名字
        public string m_currentSceneName { get; set; }
        //保存当前的场景ID
        public string m_currentSceneId { get; set; }
        //场景加载是否完成
        public bool m_isAlreadyLoadScene { get; set; }
        //保存加载进度
        public int m_loadingProgress = 0;
        //异步加载的初始化Mono脚本
        private MonoBehaviour m_sceneMono;

        public Action m_onLoadSceneFinish = null;

        public Action m_onLoadSceneEnter = null;

        public override bool Init()
        {
            //初始化MonoBehavior脚本
            if (m_sceneMono == null)
            {
                m_sceneMono = MGameManager.m_monoBehavior;
            }

            return base.Init();
        }

        public void LoadScene(int ScneeID,Action onLoadEnter=null,Action onLoadFinish=null) 
        {

        }

        public void LoadScene(string sceneName,Action onLoadEnter = null, Action onLoadFinish = null) 
        {
            m_loadingProgress = 0;
            m_onLoadSceneEnter = onLoadEnter;
            m_onLoadSceneFinish = onLoadFinish;
            LoadSceneAsync(sceneName); 
        }

        IEnumerator LoadSceneAsync(string sceneName) 
        {
            ClearCatch();
            m_isAlreadyLoadScene = false;
            if (m_onLoadSceneEnter != null) 
            {
                m_onLoadSceneEnter(); 
            }
            AsyncOperation unLoadScene = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single);
            while(unLoadScene!=null && !unLoadScene.isDone) 
            {
                yield return new WaitForEndOfFrame(); 
            }
            m_loadingProgress = 0;
            int targetProgress = 0;
            AsyncOperation targetScene = SceneManager.LoadSceneAsync(sceneName);
            if(targetScene!=null && !targetScene.isDone) 
            {
                targetScene.allowSceneActivation = false;
                while(targetScene.progress < 0.9f) 
                {
                    targetProgress = (int)targetScene.progress * 100;
                    yield return new WaitForEndOfFrame();
                    //平滑过渡
                    while (m_loadingProgress < targetProgress) 
                    {
                        ++m_loadingProgress;
                        yield return new WaitForEndOfFrame();
                    }
                }
                //自行加载剩余10%
                targetProgress = 100;
                if(m_loadingProgress < targetProgress - 2) 
                {
                    ++m_loadingProgress;
                    yield return new WaitForEndOfFrame();
                }
                m_loadingProgress = 100;
                targetScene.allowSceneActivation = true;
                m_isAlreadyLoadScene = true;
                m_currentSceneName = sceneName;
                SettingScene(sceneName);
                if (m_onLoadSceneFinish != null)
                {
                    m_onLoadSceneFinish();
                }
            }
            yield return null; 
        }

        /// <summary>
        /// 切场景需要清除缓存
        /// </summary>
        public void ClearCatch() 
        {
            MObjectManager.singleton.ClearCatch();
            MResourceManager.singleton.Clear(); 
        }

        //场景加载完成做一些场景设置 根据配置来做
        public void SettingScene(string sceneName) 
        {
             
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        public override void UnInit()
        {
            base.UnInit();
        }
    }
}
