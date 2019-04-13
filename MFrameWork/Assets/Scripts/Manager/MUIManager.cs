using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MFrameWork
{
    /// <summary>
    /// This is a Manager of all UI
    /// </summary>
    public class MUIManager : MSingleton<MUIManager>
    {
        public const string TEST_CONTROLLER = "TestController";
        public const string LOGON_CONTROLLER = "LoginPanel";

        private Dictionary<string, MUIBase> _mUiDict;
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

		public override bool Init()
		{
            _mUiDict = new Dictionary<string, MUIBase>();
            _mUIRoot = Object.Instantiate(Resources.Load("UI/Prefabs/UIRoot") as GameObject);
            _mUIRoot.name = "UIRoot";
            _mUIRoot.SetActive(true);
            Object.DontDestroyOnLoad(_mUIRoot);
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

            UIRegister();
            return base.Init();
        }

        //UI Register
        private void UIRegister()
        {
            _mUiDict.Add(TEST_CONTROLLER, new TestController());
            _mUiDict.Add(LOGON_CONTROLLER, new LogonController());
        }

		public override void UnInit()
		{
            if (_mUIRoot)
            {
                /*
                MResLoader.singleton.DestroyObj(_uiRoot, false);
                _uiRoot = null;
                _transNormal = null;
                _transUpper = null;
                _transTop = null;
                _transHUD = null;
                _uiCamera = null;
                */
            }
            base.UnInit();
		}

		public override void OnLogOut()
		{
            base.OnLogOut();
		}

        public MUIBase ActiveUI(string uiName)
        {
            MUIBase ui = GetUI(uiName);
            if (ui == null)
            {
                Debug.LogError("unregister ui name= "+uiName);
                return null;
            }

            if (!ui.MIsInited)
            {
                ui.Init();
            }

            ui.MActive = true;
            return ui;
        }

        public void DeActiveUI(string uiName)
        {
            MUIBase ui = GetUI(uiName);
            if (ui == null)
            {
                Debug.LogError("Name= " + uiName +"is null");
                return;
            }

            if(ui.MIsInited)
            {
                if (ui.MActive)
                {
                    ui.MActive = false;
                    ui.Uninit();
                }
            }

        }

        public MUIBase GetUI(string uiName)
        {
            MUIBase result = null;
            _mUiDict.TryGetValue(uiName, out result);
            return result;
        }

        public T GetUI<T>(string uiName) where T : MUIBase
        {
            MUIBase result = null;
            if (_mUiDict.TryGetValue(uiName, out result))
            {
                if (result is T)
                {
                    return (T)result;
                }
            }
            return null;
        }

        public void DeActiveAll()
        {
            foreach (KeyValuePair<string, MUIBase> pair in _mUiDict)
            {
                DeActiveUI(pair.Key);
            }
        }

        public void Update(float delta)
        {
            foreach (KeyValuePair<string, MUIBase> pair in _mUiDict)
            {
                pair.Value.Update(delta);
            }
        }

        public void LateUpdate(float delta)
        {
            foreach (KeyValuePair<string, MUIBase> pair in _mUiDict)
            {
                pair.Value.LateUpdate(delta);
            }
        }

        public void OnLogout()
        {
            foreach (KeyValuePair<string, MUIBase> pair in _mUiDict)
            {
                pair.Value.OnLogOut();
            }
            if(_mUICamera)
            {
                _mUICamera.enabled = false;
            }
        }

        public bool IsWorldPosInScreen(ref Vector3 worldPos)
        {
            /*
            Vector3 pos = MScene.singleton.GameCamera.UCam.WorldToScreenPoint(worldPos);
            if (pos.z < 0) return false;
            Rect rect = Screen.safeArea;
            return (pos.x >= rect.xMin && pos.x <= rect.xMax
                    && pos.y >= rect.yMin && pos.y <= rect.yMax);
                    */
            return true;
        }

	}
}