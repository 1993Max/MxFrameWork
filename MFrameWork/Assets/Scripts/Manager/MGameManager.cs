using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public class MGameManager : MonoBehaviour 
    {
        private MUIManager  _mUIManager = null;
        private MResManager _mResManager = null;

    	private void Awake()
    	{
            //GameObject.DestroyObject(this.gameObject);
             _mUIManager = MUIManager.singleton;
            _mResManager = MResManager.singleton;
    	}

		private void Start()
		{
            ManagerInit();
		}

		public void ManagerInit()
        {
            _mUIManager.Init();
            _mResManager.Init();

            ShowTest();
        }

        public void ShowTest()
        {
            _mUIManager.ActiveUI("LoginPanel");
        }
    }

}
