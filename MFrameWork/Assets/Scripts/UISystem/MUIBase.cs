using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    //GameObject Loaded CallBack ������ػص�
    public delegate void GameObjectLoadedCallBack(GameObject obj);

    //UILayer Type UI�������ö��
    public enum UILayerType
    {
        Top,
        Upper,
        Normal,
        Hud
    }

    //
    public abstract class MUIBase
    {
        //Is Init �Ƿ�������
        protected bool   _mInited;

        //UIName  UI��
        protected string _mUIName;

        //UIGameObject UI����
        protected GameObject _mUIGameObject;

        //IsActive �Ƿ�ɼ�
        protected bool _mActive;

        //LoadedCallBack ���ػص�
        protected GameObjectLoadedCallBack _mCallBack;

        //UILayerType UI������
        protected UILayerType _mUILayerType;

        public string MUIName
        {
            get { return _mUIName;  }
            set { _mUIName = value; }
        }

        public GameObject MUIGameObject
        {
            get { return _mUIGameObject; }
            set { _mUIGameObject = value; }
        }

        public bool MActive
        {
            get { return _mActive; }
            set 
            {
                _mActive = value; 
                if(_mUIGameObject!=null)
                {
                    _mUIGameObject.SetActive(value);
                    if(_mUIGameObject.activeSelf)
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

        public bool MIsInited { get { return _mInited; } }

        public MUIBase(string uiName,UILayerType layerType)
        {
            _mUIName = uiName;
            _mUILayerType = layerType;
        }

        public virtual void Init()
        {
            _mUIGameObject = Object.Instantiate(Resources.Load("UI/Prefabs/" + _mUIName)) as GameObject;
            _mCallBack += OnLoaded;
            _mInited = true;
            OnLoaded(_mUIGameObject);
        }

        public virtual void Uninit()
        {
            _mInited = false;
            //Release GameObject
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

        protected void OnLoaded(GameObject go)
        {
            SetPanetByLayerType(_mUILayerType);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            if (rectTrans)
            {
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
            }

            go.transform.SetAsLastSibling();
            _mActive = true;
        }

        protected void SetPanetByLayerType(UILayerType layerType)
        {
            switch(_mUILayerType)
            {
                case UILayerType.Top:
                    _mUIGameObject.transform.SetParent(MUIManager.singleton.MTransTop);
                    break;
                case UILayerType.Upper:
                    _mUIGameObject.transform.SetParent(MUIManager.singleton.MTransUpper);
                    break;
                case UILayerType.Normal:
                    _mUIGameObject.transform.SetParent(MUIManager.singleton.MTransNormal);
                    break;
                case UILayerType.Hud:
                    _mUIGameObject.transform.SetParent(MUIManager.singleton.MTransHUD);
                    break;
            }
        }
    }
}

