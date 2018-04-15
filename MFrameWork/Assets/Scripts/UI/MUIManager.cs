using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
/* Create By Mx
 *
 */
namespace MFrameWork
{
    public class MUIManager : MSingleton<MUIManager>
    {

        private Dictionary<string,MUIBase> _uiDict = new Dictionary<string, MUIBase>();
        private Camera _mUICamera;
        private GameObject _mUIRoot;

        private CanvasScaler _mUiRootCanvasScaler;
        private Transform _mTransNormal;
        private RectTransform _mRectTransNormal;
        private Transform _mTtransTop;
        private RectTransform _mRectTransTop;
        private Transform _mTransUpper;
        private RectTransform _mRectTransUpper;
        private Transform _mTransHUD;
        private RectTransform _mRectTransHUD;

        public Camera MUICamera { get { return _mUICamera; } }
        public GameObject MUIRoot { get { return _mUIRoot; } }
        public Transform MTransNormal { get { return _mTransNormal; } }
        public RectTransform MRectTransNormal { get { return _mRectTransNormal; } }
        public Transform MTransUpper { get { return _mTransUpper; } }
        public RectTransform MRectTransUpper { get { return _mRectTransUpper; } }
        public Transform MTransTop { get { return _mTtransTop; } }
        public RectTransform MRectTransTop { get { return _mRectTransTop; } }
        public Transform MTransHUD { get { return _mTransHUD; } }
        public RectTransform MRectTransHUD { get { return _mRectTransHUD; } }

        public Transform MTransRoleHUD { get; private set; }
        public Transform MTransMonsterHUD { get; private set; }
        public Transform MTransBubbleHUD { get; private set; }
        public Transform MTransSkillHUD { get; private set; }

		public override void Init()
		{
            base.Init();
            _mUIRoot = Resources.Load("UI/Prefabs/UIRoot") as GameObject;
            _mUIRoot.name = "UIRoot";
            GameObject.DontDestroyOnLoad(_mUIRoot);
            _mUiRootCanvasScaler = _mUIRoot.GetComponent<CanvasScaler>();

            _mTransNormal = _mUIRoot.transform.Find("NormalLayer");
            _mRectTransNormal = _mTransNormal.gameObject.GetComponent<RectTransform>();
            _mTtransTop = _mUIRoot.transform.Find("TopLayer");
            _mRectTransTop = _mTtransTop.gameObject.GetComponent<RectTransform>();
            _mTransUpper = _mUIRoot.transform.Find("UpperLayer");
            _mRectTransUpper = _mTransUpper.gameObject.GetComponent<RectTransform>();
            _mTransHUD = _mUIRoot.transform.Find("HudLayer");
            _mRectTransHUD = _mTransHUD.gameObject.GetComponent<RectTransform>();

            for (int i = 0; i < _mTransHUD.childCount; i++)
            {
                Transform t = _mTransHUD.GetChild(i);
                switch (t.name)
                {
                    case "RoleHUDList":
                        MTransRoleHUD = t;
                        break;
                    case "MonsterHUDList":
                        MTransMonsterHUD = t;
                        break;
                    case "BubbleHUDList":
                        MTransBubbleHUD = t;
                        break;
                    case "SkillHUDList":
                        MTransSkillHUD = t;
                        break;
                }
            }
            _mUICamera = _mUIRoot.transform.Find("Camera").GetComponent<Camera>();
		}

		public override void UnInit()
		{
            base.UnInit();
		}

		public override void OnLogOut()
		{
            base.OnLogOut();
		}

        public void ActiveUI(string uiName)
        {
            
        }

        public void DeActiveUI(string uiName)
        {
            
        }

        public MUIBase GetUI(string uiName)
        {
            MUIBase result = null;
            _uiDict.TryGetValue(uiName, out result);
            return result;
        }

        public void DeActiveAll()
        {
            foreach (KeyValuePair<string, MUIBase> pair in _uiDict)
            {
                DeActiveUI(pair.Key);
            }
        }
	}
}