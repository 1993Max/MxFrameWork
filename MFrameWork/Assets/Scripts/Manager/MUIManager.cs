// **************************************
//
// �ļ���(MUIManager.cs):
// ��������("UI���Ĺ�����"):
// ����(Max1993):
// ����(2019/5/19  21:26):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MFrameWork
{
    public class MUIManager : MSingleton<MUIManager>
    {
        /// <summary>
        /// ���ĵĹ�������UI��Dictionary
        /// </summary>
        private Dictionary<string, MUIBase> m_uiDict;

        /// <summary>
        /// �����UICamera
        /// </summary>
        private Camera m_uiCamera;

        private GameObject m_uiRoot;
        private Transform m_transNormal;
        private RectTransform m_rectTransNormal;
        private Transform m_transTop;
        private RectTransform m_rectTransTop;
        private Transform m_transUpper;
        private RectTransform m_rectTransUpper;
        private Transform m_transHUD;
        private RectTransform m_rectTransHUD;

        //������һЩ�����Ĳ㼶λ����Ϣ
        public GameObject MUIRoot { get { return m_uiRoot; } }
        public Transform MTransNormal { get { return m_transNormal; } }
        public RectTransform MRectTransNormal { get { return m_rectTransNormal; } }
        public Transform MTransUpper { get { return m_transUpper; } }
        public RectTransform MRectTransUpper { get { return m_rectTransUpper; } }
        public Transform MTransTop { get { return m_transTop; } }
        public RectTransform MRectTransTop { get { return m_rectTransTop; } }
        public Transform MTransHUD { get { return m_transHUD; } }
        public RectTransform MRectTransHUD { get { return m_rectTransHUD; } }

        public Camera MUICamera
        {
            get
            {
                return m_uiCamera;
            }
        }

        public override bool Init()
		{
            return InitUIInfo() && UIRegister();
        }

        /// <summary>
        /// ��ʼ��һЩ���õ�UI��Ϣ
        /// </summary>
        /// <returns></returns>
        public bool InitUIInfo()
        {
            m_uiDict = new Dictionary<string, MUIBase>();
            m_uiRoot = MObjectManager.singleton.InstantiateGameObeject(MPathUtils.UI_ROOTPATH);
            if (m_uiRoot == null)
            {
                MDebug.singleton.AddErrorLog("��ʼ��UIManager ʧ����~");
                return false;
            }
            m_uiRoot.name = "UIRoot";
            m_uiRoot.SetActive(true);
            m_transNormal = m_uiRoot.transform.Find("NormalLayer");
            m_rectTransNormal = m_transNormal.gameObject.GetComponent<RectTransform>();
            m_transTop = m_uiRoot.transform.Find("TopLayer");
            m_rectTransTop = m_transTop.gameObject.GetComponent<RectTransform>();
            m_transUpper = m_uiRoot.transform.Find("UpperLayer");
            m_rectTransUpper = m_transUpper.gameObject.GetComponent<RectTransform>();
            m_transHUD = m_uiRoot.transform.Find("HudLayer");
            m_rectTransHUD = m_transHUD.gameObject.GetComponent<RectTransform>();
            m_uiCamera = m_uiRoot.transform.Find("Camera").GetComponent<Camera>();
            GameObject.DontDestroyOnLoad(m_uiRoot);
            return true;
        }

        public const string LOGON_CONTROLLER = "LoginPanel.prefab";
        public const string LOADING_CONTROLLER = "LoadingPanel.prefab";
        /// <summary>
        /// ��C#��ʵ���߼���UI����ע��ע�� 
        /// </summary>
        /// <returns></returns>
        private bool UIRegister()
        {
            m_uiDict.Add(LOGON_CONTROLLER, new LogonController());
            m_uiDict.Add(LOADING_CONTROLLER, new LoadingController());
            return true;
        }

        public override void UnInit()
		{
            if (m_uiRoot)
            {
                MObjectManager.singleton.ReleaseObjectComopletly(m_uiRoot);
                m_uiRoot = null;
                m_transNormal = null;
                m_rectTransNormal = null;
                m_transTop = null;
                m_rectTransTop = null;
                m_transUpper = null;
                m_rectTransUpper = null;
                m_transHUD = null;
                m_rectTransHUD = null;
                m_uiCamera = null;
            }
		}

		public override void OnLogOut()
		{
            base.OnLogOut();
		}

        /// <summary>
        /// ��һ��UI�Ľӿ�
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public MUIBase ActiveUI(string uiName)
        {
            MUIBase mUIBase = GetUI(uiName);
            if (mUIBase == null)
            {
                Debug.LogError("UIDic����û�����UI��Ϣ UIName��"+ uiName);
                return null;
            }

            if (!mUIBase.IsInited)
            {
                mUIBase.Init();
            }

            return mUIBase;
        }

        /// <summary>
        /// �ر�һ��UI�Ľӿ�
        /// </summary>
        /// <param name="uiName"></param>
        public void DeActiveUI(string uiName)
        {
            MUIBase mUIBase = GetUI(uiName);
            if (mUIBase == null)
            {
                Debug.LogError("UIDic����û�����UI��Ϣ UIName��" + uiName);
                return;
            }

            if(mUIBase.IsInited)
            {
                if (mUIBase.Active)
                {
                    mUIBase.Active = false;
                }
                mUIBase.Uninit();
            }

        }

        /// <summary>
        /// ��ȡһ��UI�Ľӿ�
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public MUIBase GetUI(string uiName)
        {
            MUIBase mUIBase = null;
            m_uiDict.TryGetValue(uiName, out mUIBase);
            return mUIBase;
        }

        public T GetUI<T>(string uiName) where T : MUIBase
        {
            MUIBase mUIBase = null;
            if (m_uiDict.TryGetValue(uiName, out mUIBase))
            {
                if (mUIBase is T)
                {
                    return (T)mUIBase;
                }
            }
            return null;
        }

        /// <summary>
        /// �ر�����UI�Ľӿ�
        /// </summary>
        public void DeActiveAll()
        {
            foreach (KeyValuePair<string, MUIBase> pair in m_uiDict)
            {
                DeActiveUI(pair.Key);
            }
        }
        
        /// <summary>
        /// Update����
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            foreach (var mUIBase in m_uiDict.Values)
            {
                mUIBase.Update(delta);
            }
        }

        /// <summary>
        /// LateUpdate����
        /// </summary>
        /// <param name="delta"></param>
        public void LateUpdate(float delta)
        {
            foreach (var mUIBase in m_uiDict.Values)
            {
                mUIBase.LateUpdate(delta);
            }
        }

        /// <summary>
        /// ע������
        /// </summary>
        public void OnLogout()
        {
            foreach (var mUIBase in m_uiDict.Values)
            {
                mUIBase.OnLogOut();
            }
            if (m_uiCamera)
            {
                m_uiCamera.enabled = false;
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
