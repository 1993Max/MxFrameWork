using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MFrameWork
{
    public class LogonController : MUIBase
    {
        public GameObject BtnStart;
        public GameObject BtnExit;

        public LogonController() : base("LoginPanel", UILayerType.Normal)
        {

        }

		public override void Init()
		{
            base.Init();
            BtnStart = _mUIGameObject.transform.Find("BtnLogin").gameObject;
            BtnExit  = _mUIGameObject.transform.Find("BtnExit").gameObject;

            BtnStart.GetComponent<Button>().onClick.AddListener(delegate
            {
                Debug.Log("Start");
            });

            BtnExit.GetComponent<Button>().onClick.AddListener(delegate
            {
                Debug.Log("Exit");
            });
		}

		protected override void OnActive()
        {
            Debug.Log("Active");
        }

        protected override void OnDeActive()
        {
            Debug.Log("OnDeActive");
        }

    }
}
