using UnityEngine;
using System.Collections;

namespace MFrameWork
{
    //GameObject Loaded CallBack 物体加载回掉
    public delegate void GameObjectLoadedCallBack(GameObject obj);

    //UILayer Type UI层的类型枚举
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
        //Is Init 是否加载完成
        protected bool   _mInited;

        //UIName  UI名
        protected string _mUIName;

        //UIGameObject UI物体
        protected GameObject _mUIGameObject;

        //IsActive 是否可见
        protected bool _mActive;

        //LoadedCallBack 加载回掉
        protected GameObjectLoadedCallBack _mCallBack;

        //UILayerType UI层类型
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

