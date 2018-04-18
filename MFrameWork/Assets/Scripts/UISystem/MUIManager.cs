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

            //UI Register
            _uiDict.Add(TEST_CONTROLLER, new TestController());
		}

		public override void UnInit()
		{
            /*
            if (_uiRoot)
            {
                MResLoader.singleton.DestroyObj(_uiRoot, false);
                _uiRoot = null;
                _transNormal = null;
                _transUpper = null;
                _transTop = null;
                _transHUD = null;
                _uiCamera = null;
            }*/
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
            _uiDict.TryGetValue(uiName, out result);
            return result;
        }

        public T GetUI<T>(string uiName) where T : MUIBase
        {
            MUIBase result = null;
            if (_uiDict.TryGetValue(uiName, out result))
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
            foreach (KeyValuePair<string, MUIBase> pair in _uiDict)
            {
                DeActiveUI(pair.Key);
            }
        }

        public void Update(float delta)
        {
            foreach (KeyValuePair<string, MUIBase> pair in _uiDict)
            {
                pair.Value.Update(delta);
            }
        }

        public void LateUpdate(float delta)
        {
            foreach (KeyValuePair<string, MUIBase> pair in _uiDict)
            {
                pair.Value.LateUpdate(delta);
            }
        }

        public void OnLogout()
        {
            foreach (KeyValuePair<string, MUIBase> pair in _uiDict)
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
