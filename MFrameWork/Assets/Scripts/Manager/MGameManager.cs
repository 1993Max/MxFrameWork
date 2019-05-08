using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public class MGameManager : MonoBehaviour 
    {
        private List<MBaseSingleton> _mManagerList = null;

        public AudioSource mAudioSource;

        public List<MBaseSingleton> ManagerList
        {
            get
            {
                return _mManagerList;
            }
        }

        private void Awake()
    	{
            SetManagerList();
        }

		private void Start()
		{
            ShowTest();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) {
                ResourceFrameDemo();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, false);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                mAudioSource.Pause();
                MResourceManager.singleton.ReleaseResource(mAudioSource.clip, true);
            }
        }

        public void SetManagerList()
        {
            if (_mManagerList == null)
            {
                _mManagerList = new List<MBaseSingleton>();
            }
            _mManagerList.Add(MUIManager.singleton);
            _mManagerList.Add(MResManager.singleton);
            _mManagerList.Add(MLuaManager.singleton);
            _mManagerList.Add(MClassObjectManager.singleton);
            _mManagerList.Add(MAssetBundleManager.singleton);
            _mManagerList.Add(MResourceManager.singleton);

            ManagerInit();
        }

        private void ManagerInit()
        {
            for (int i = 0; i < ManagerList.Count; i++)
            {
                ManagerList[i].Init();
            }
        }

        private void ManagerUnInit()
        {
            for (int i = 0; i < ManagerList.Count; i++)
            {
                ManagerList[i].UnInit();
            }
        }

        public void ShowTest()
        {
            MUIManager.singleton.ActiveUI("LoginPanel");
        }

        public void ResourceFrameDemo()
        {
            AudioClip audioClip = MResourceManager.singleton.LoadResource<AudioClip>("Assets/Resources/Sound/lemon.mp3");
            mAudioSource.clip = audioClip;
            mAudioSource.Play();
        }
    }

}
