using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    public delegate void GameObjectLoadedCallBack(GameObject obj);

    public enum UILayerType
    {
        Top,
        Upper,
        Normal,
        Hud
    }

    public abstract class MUIBase
    {
        protected bool   _mInited;
        protected string _mUIName;
        protected GameObject _mUIGameObject;
        protected bool _mActive;
        protected GameObjectLoadedCallBack _mCallBack;
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
            _mUIGameObject = Resources.Load(_mUIName) as GameObject;
            _mCallBack += OnLoaded;
            _mInited = true;
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
            //go.transform.SetParent();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            if (rectTrans)
            {
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
            }

            SetPanetByLayerType(_mUILayerType);
            go.transform.SetAsLastSibling();
            _mActive = true;
        }

        protected void SetPanetByLayerType(UILayerType layerType)
        {
            switch(_mUILayerType)
            {
                case UILayerType.Top:
                    break;
                case UILayerType.Upper:
                    break;
                case UILayerType.Normal:
                    break;
                case UILayerType.Hud:
                    break;
            }
        }
    }
}

