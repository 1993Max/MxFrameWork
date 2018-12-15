using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public class MGameManager : MonoBehaviour 
    {
        private List<MBaseSingleton> _mManagerList = null;

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

		public void SetManagerList()
        {
            if (_mManagerList == null)
            {
                _mManagerList = new List<MBaseSingleton>();
            }
            _mManagerList.Add(MUIManager.singleton);
            _mManagerList.Add(MResManager.singleton);
            _mManagerList.Add(MluaManager.singleton);
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
    }

}
