// **************************************
//
// �ļ���(MUIBase.cs):
// ��������("UI����"):
// ����(Max1993):
// ����(2019/5/19  21:26):
//
// **************************************
//
using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    //GameObject Loaded CallBack ������ػص�
    public delegate void GameObjectLoadedCallBack(GameObject obj);

    /// <summary>
    /// UI�㼶
    /// </summary>
    public enum MUILayerType
    {
        Top,
        Upper,
        Normal,
        Hud
    }

    /// <summary>
    /// ����UI�ķ�ʽ
    /// </summary>
    public enum MUILoadType 
    {
        SyncLoad,
        AsyncLoad, 
    }

    public abstract class MUIBase
    {
        /// <summary>
        /// �Ƿ������ɵı�־λ
        /// </summary>
        protected bool m_isInited;

        /// <summary>
        /// UI����
        /// </summary>
        protected string m_uiName;

        /// <summary>
        /// �ڹرյ�ʱ���Ƿ񻺴�UI Ĭ�ϲ�����
        /// </summary>
        protected bool m_isCatchUI=false;

        /// <summary>
        /// UI��ʵ����GamObejct
        /// </summary>
        protected GameObject m_uiGameObject;

        /// <summary>
        /// ����UI�ɼ���״̬
        /// </summary>
        protected bool m_active = false;

        /// <summary>
        /// ������ɵĻص�
        /// </summary>
        protected GameObjectLoadedCallBack m_callBack;

        /// <summary>
        /// UI����Դȫ·��
        /// </summary>
        protected string m_uiFullPath = "";

        //UILayerType UI������
        protected MUILayerType m_uiLayerType;

        //UI�ļ��ط�ʽ
        protected MUILoadType m_uiLoadType;

        public string MUiName
        {
            get { return m_uiName;  }
            set 
            { 
                m_uiName = value;
                m_uiFullPath = MPathUtils.UI_MAINPATH + "/" + m_uiName;
            }
        }

        public bool MIsCatchUI 
        {
            get { return m_isCatchUI; }
            set
            {
                m_isCatchUI = value;
            }
        }

        public GameObject MUIGameObject
        {
            get { return m_uiGameObject; }
            set { m_uiGameObject = value; }
        }

        public bool MActive
        {
            get { return m_active; }
            set 
            {
                m_active = value; 
                if(m_uiGameObject!=null)
                {
                    m_uiGameObject.SetActive(value);
                    if(m_uiGameObject.activeSelf)
                    {
                        OnActive();   
                    }
                    else
                    {
                        OnDeActive();
                    }
                }
            }
        }

        public bool MIsInited { get { return m_isInited; } }

        protected MUIBase(string uiName,MUILayerType layerType,MUILoadType loadType = MUILoadType.SyncLoad)
        {
            MUiName = uiName;
            m_uiLayerType = layerType;
            m_uiLoadType = loadType;
        }

        public virtual void Init()
        {
            if(m_uiLoadType == MUILoadType.SyncLoad) 
            {
                GameObject go = MObjectManager.singleton.InstantiateGameObeject(m_uiFullPath);
                OnGameObjectLoaded(go);
            }
            else 
            {
                MObjectManager.singleton.InstantiateGameObejectAsync(m_uiFullPath,delegate (string resPath, MResourceObjectItem mResourceObjectItem,object[] parms)
                {
                    GameObject go = mResourceObjectItem.m_cloneObeject;
                    OnGameObjectLoaded(go);
                },LoadResPriority.RES_LOAD_LEVEL_HEIGHT);
            }
        }

        private void OnGameObjectLoaded(GameObject uiObj) 
        {
            if(uiObj == null)
            {
                MDebug.singleton.AddErrorLog("UI����ʧ��������~����·�� ResPath: " + m_uiFullPath);
                return;
            }
            m_uiGameObject = uiObj;
            m_isInited = true;
            SetPanetByLayerType(m_uiLayerType);
            m_uiGameObject.transform.localPosition = Vector3.zero;
            m_uiGameObject.transform.localScale = Vector3.one;
        }

        public virtual void Uninit()
        {
            m_isInited = false;
            m_active = false;
            if (m_isCatchUI) 
            {
                //��Դ�����뵽��Դ��
                MObjectManager.singleton.ReleaseObject(m_uiGameObject);
            }
            else 
            {
                //�������Object��Դ
                MObjectManager.singleton.ReleaseObject(m_uiGameObject, 0, true);
            }
        }

        protected abstract void OnActive();

        protected abstract void OnDeActive();

        public virtual void Update(float deltaTime)
        {
            
        }

        public virtual void LateUpdate(float deltaTime)
        {
            
        }

        public virtual void OnLogOut()
        {
            
        }

        protected void SetPanetByLayerType(MUILayerType layerType)
        {
            switch(m_uiLayerType)
            {
                case MUILayerType.Top:
                    m_uiGameObject.transform.SetParent(MUIManager.singleton.MTransTop);
                    break;
                case MUILayerType.Upper:
                    m_uiGameObject.transform.SetParent(MUIManager.singleton.MTransUpper);
                    break;
                case MUILayerType.Normal:
                    m_uiGameObject.transform.SetParent(MUIManager.singleton.MTransNormal);
                    break;
                case MUILayerType.Hud:
                    m_uiGameObject.transform.SetParent(MUIManager.singleton.MTransHUD);
                    break;
            }
        }
    }
}

