//***************************

//文件名称(File Name)： MLuaUICom.cs 

//功能描述(Description)： UI控件引用，挂载需要写逻辑的Image，Button，Text等UGUI的控件上

//数据表(Tables)： nothing

//作者(Author)： zzr

//Create Date: 2017.08.10

//修改记录(Revision History)： nothing

//***************************
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

namespace MFrameWork
{

    [ExecuteInEditMode]
    public class MLuaUICom : MonoBehaviour
    {
        /// <summary>
        /// 定义之后的组件名称
        /// </summary>
        [Header("定义之后的组件名称")]
        public string Name;


        [SerializeField]
        [Header("绑定的模块名称")]
        private string _modelName;
        /// <summary>
        /// 绑定数据的模块名
        /// </summary>
        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }

        [SerializeField]
        [Header("绑定的数据名称")]
        private string _dataName;
        /// <summary>
        /// 绑定的数据变量名称
        /// </summary>
        public string DataName
        {
            get { return _dataName ?? $"{Name}Data"; }
            set { _dataName = value; }
        }

        /// <summary>
        /// Panel
        /// </summary>
        [Header("根Panel")]
        public MLuaUIPanel UIPanel;

        /// <summary>
        /// 所绑定的对象
        /// </summary>
        [Header("所绑定的对象")]
        public Component BindCom;

        /// <summary>
        /// 是否要自动生成绑定代码
        /// </summary>
        [Header("是否要自动生成绑定代码")]
        public bool GenBindScript;

        private string _bindComType;
        /// <summary>
        /// 绑定组件的类型
        /// </summary>
        public string BindComType
        {
            get
            {
                if (_bindComType == null)
                {
                    _bindComType = BindCom.GetType().Name;
                }
                return _bindComType;
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [Header("默认值")]
        public string DefaultValue;

        public bool IsArray;

        private Transform _transform;
        private RectTransform _rectTransform;
        private Button _btn;
        private Text _lab;
        private Image _img;
        private RawImage _rawImg;
        private Toggle _tog;
        private InputField _input;
        private Slider _slider;
        private ScrollRect _scrollRect;
        private MLuaUIListener _listener;
        private GridLayoutGroup _grid;
        private LayoutElement _layoutEle;
        private CanvasGroup _canvasGroup;
        private ContentSizeFitter _fitter;

        public GameObject MGameObject
        {
            get
            {
                return gameObject;
            }
        }

        public Transform Transform
        {
            get
            {
                if (!_transform)
                {
                    _transform = transform;
                    if (!_transform) MDebug.singleton.AddErrorLogF("nonexist Transform on this gameObject, name={0}", gameObject.name);
                }
                return _transform;
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                {
                    _rectTransform = Transform as RectTransform;
                    if (!_rectTransform) MDebug.singleton.AddErrorLogF("nonexist RectTransform on this gameObject, name={0}", gameObject.name);
                }
                return _rectTransform;
            }
        }

        public bool IsGray { get { return Img.color == Color.black; } }

        public void SetActiveEx(bool isShow)
        {
            MGameObject.SetActiveEx(isShow);
        }

        #region UGUI components
        public Button Btn
        {
            get
            {
                if (!_btn)
                {
                    _btn = gameObject.GetComponent<Button>();
                    if (!_btn) MDebug.singleton.AddErrorLogF("noexist Button on this gameObject, name={0}", gameObject.name);
                }
                return _btn;
            }
        }

        public Text Lab
        {
            get
            {
                if (!_lab)
                {
                    _lab = gameObject.GetComponent<Text>();
                    if (!_lab) MDebug.singleton.AddErrorLogF("nonexist Text on this gameObject, name={0}", gameObject.name);
                }
                return _lab;
            }
        }

        public Image Img
        {
            get
            {
                if (!_img)
                {
                    _img = gameObject.GetComponent<Image>();
                    if (!_img)
                    {
                        MDebug.singleton.AddErrorLogF("nonexist _img on this gameObject, name={0}", gameObject.name);
                    }
                }
                return _img;
            }
        }
        public RawImage RawImg
        {
            get
            {
                if (!_rawImg)
                {
                    _rawImg = gameObject.GetComponent<RawImage>();
                    if (!_rawImg)
                    {
                        MDebug.singleton.AddErrorLogF("nonexist RawImage on this gameObject, name={0}", gameObject.name);
                    }
                }
                return _rawImg;
            }
        }
        public Toggle Tog
        {
            get
            {
                if (!_tog)
                {
                    _tog = gameObject.GetComponent<Toggle>();
                    if (!_tog) MDebug.singleton.AddErrorLogF("nonexist Toggle on this gameObject, name={0}", gameObject.name);
                }
                return _tog;
            }
        }

        public InputField Input
        {
            get
            {
                if (!_input)
                {
                    _input = gameObject.GetComponent<InputField>();
                    if (!_input) MDebug.singleton.AddErrorLogF("nonexist InputField on this gameObject, name={0}", gameObject.name);
                }
                return _input;
            }
        }

        public Slider Slider
        {
            get
            {
                if (!_slider)
                {
                    _slider = gameObject.GetComponent<Slider>();
                    if (!_slider) MDebug.singleton.AddErrorLogF("nonexist Slider on this gameObject, name={0}", gameObject.name);
                    else _slider.enabled = true;
                }
                return _slider;
            }
        }

        public ScrollRect Scroll
        {
            get
            {
                if (!_scrollRect)
                {
                    _scrollRect = gameObject.GetComponent<ScrollRect>();
                    if (!_scrollRect) MDebug.singleton.AddErrorLogF("nonexist ScrollRect on this gameObject, name={0}", gameObject.name);
                }
                return _scrollRect;
            }
        }

        public MLuaUIListener Listener
        {
            get
            {
                if (!_listener)
                {
                    _listener = gameObject.GetComponent<MLuaUIListener>();
                    if (!_listener) MDebug.singleton.AddErrorLogF("nonexist Listener on this gameObject, name={0}", gameObject.name);
                }
                return _listener;
            }
        }

        public GridLayoutGroup Grid
        {
            get
            {
                if (!_grid)
                {
                    _grid = gameObject.GetComponent<GridLayoutGroup>();
                    if (!_grid)
                        MDebug.singleton.AddErrorLogF("nonexist RectTransform on this gameObject, name={0}", gameObject.name);
                }
                return _grid;
            }
        }

        public LayoutElement LayoutEle
        {
            get
            {
                if (!_layoutEle)
                {
                    _layoutEle = gameObject.GetComponent<LayoutElement>();
                    if (!_layoutEle) MDebug.singleton.AddErrorLogF("nonexist LayoutElement on this gameObject, name={0}", gameObject.name);
                }
                return _layoutEle;
            }
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (!_canvasGroup)
                {
                    _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                    if (!_canvasGroup) MDebug.singleton.AddErrorLogF("nonexist LayoutElement on this gameObject, name={0}", gameObject.name);
                }
                return _canvasGroup;
            }
        }
        #endregion

        #region button functions
        /// <summary>
        /// 添加按钮事件
        /// </summary>
        public void AddClick(LuaFunction luaFunc, bool clearListener = true)
        {
            if (Btn)
            {
                if (luaFunc == null)
                {
                    Btn.onClick.RemoveAllListeners();
                    return;
                }
                if (clearListener)
                {
                    Btn.onClick.RemoveAllListeners();
                }
                Btn.onClick.AddListener(() =>
                {
                    if (luaFunc != null)
                    {
                        luaFunc.Call();
                    }
                });
            }
        }
        #endregion

        #region InputField
        public void OnInputFieldChange(LuaFunction luaFunc, bool clearListener = true)
        {
            if (Input)
            {
                if (luaFunc == null)
                {
                    Btn.onClick.RemoveAllListeners();
                    return;
                }
                if (clearListener)
                {
                    Input.onValueChanged.RemoveAllListeners();
                }
                Input.onValueChanged.AddListener((value) =>
                {
                    if (luaFunc != null)
                    {
                        luaFunc.Call(value);
                    }
                });
            }
        }
        #endregion

        #region Slider
        public void OnSliderChange(LuaFunction luaFunc, bool clearListener = true)
        {
            if (Slider)
            {
                if (luaFunc == null)
                {
                    Btn.onClick.RemoveAllListeners();
                    return;
                }
                if (clearListener)
                {
                    Slider.onValueChanged.RemoveAllListeners();
                }
                Slider.onValueChanged.AddListener((value) =>
                {
                    if (luaFunc != null)
                    {
                        luaFunc.Call(value);
                    }
                });
            }
        }
        #endregion

        #region toggle functions
        public void OnToggleChanged(LuaFunction luaFunc, bool clearListener = true)
        {
            if (Tog)
            {
                if (clearListener)
                {
                    Tog.onValueChanged.RemoveAllListeners();
                }
                Tog.onValueChanged.AddListener((bool value) =>
                {
                    if (luaFunc != null)
                    {
                        luaFunc.Call(value);
                    }
                });
            }
        }
        #endregion

        #region Image functions
        /// <summary>
        /// 设置去色
        /// </summary>
        public void SetGray(bool gray)
        {
            if (Img)
            {
                Img.color = gray ? Color.black : Color.white;
            }
        }

        /*
        /// <summary>
        /// 同步设置图素（一般只用在该UI拥有该图集的情况）
        /// </summary>
        public void SetSprite(string atlasName, string spriteName)
        {
            if (Img && _imgSetter != null)
            {
                _imgSetter.SetSprite(atlasName, spriteName);
            }
        }

        /// <summary>
        /// 异步设置图集
        /// </summary>
        public void SetSpriteAsync(string atlasName, string spriteName, Action callback = null)
        {
            if (Img && _imgSetter != null)
            {
                _imgSetter.SetSpriteAsync(atlasName, spriteName, callback);
            }
        }

        /// <summary>
        /// 重置图素
        /// </summary>
        public void ResetSprite()
        {
            if (Img && _imgSetter != null)
            {
                _imgSetter.ResetSprite();
            }
        }
        */
        /// <summary>
        /// 设置图片的enable
        /// </summary>
        /// <param name="enable"></param>
        public void SetImgEnable(bool enable)
        {
            if (Img && enable != Img.enabled)
            {
                Img.enabled = enable;
            }
        }

        #endregion

        #region RawImage functions

        public void SetRawImgEnable(bool enable)
        {
            if (RawImg && RawImg.enabled != enable)
            {
                RawImg.enabled = enable;
            }
        }

        public int GetRawImgWidth()
        {
            if (RawImg)
            {
                return _rawImg.texture.width;
            }
            return 0;
        }

        public int GetRawImgHeight()
        {
            if (RawImg)
            {
                return _rawImg.texture.height;
            }
            return 0;
        }
        #endregion

        #region ScrollRect functions
        public void OnScrollRectChange(LuaFunction luaFunc, bool clearListener = true)
        {
            if (Scroll)
            {
                if (luaFunc == null)
                {
                    Btn.onClick.RemoveAllListeners();
                    return;
                }
                if (clearListener)
                {
                    Scroll.onValueChanged.RemoveAllListeners();
                }
                Scroll.onValueChanged.AddListener((value) =>
                {
                    if (luaFunc != null)
                    {
                        luaFunc.Call(value);
                    }
                });
            }
        }
        #endregion

        public ContentSizeFitter Fitter
        {
            get
            {
                if (!_fitter)
                {
                    _fitter = gameObject.GetComponent<ContentSizeFitter>();
                    if (!_fitter) MDebug.singleton.AddErrorLogF("nonexist ContentSizeFitter on this gameObject, name={0}", gameObject.name);
                }
                return _fitter;
            }
        }

        public void SetHeight(float height)
        {
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
        }

        public void SetWidth(float width)
        {
            RectTransform.sizeDelta = new Vector2(width, RectTransform.sizeDelta.y);
        }

        #region 生命周期
        void Awake()
        {
            if (!string.IsNullOrEmpty(DefaultValue))
            {
                if (BindCom is Text)
                {
                    Lab.text = DefaultValue;
                }
                else if (BindCom is InputField)
                {
                    Input.text = DefaultValue;
                }
                else if (BindCom is Slider)
                {
                    float result;
                    if (float.TryParse(DefaultValue, out result))
                    {
                        Slider.value = result;
                    }
                }
                else if (BindCom is Toggle)
                {
                    bool result;
                    if (bool.TryParse(DefaultValue, out result))
                    {
                        Tog.isOn = result;
                    }
                }
            }
        }

        void OnTransformParentChanged()
        {

        }

        void OnDestroy()
        {

        }

        void Reset()
        {
            UIPanel = MFindHelper.GetParentComponent<MLuaUIPanel>(transform);
            var group = MFindHelper.GetParentComponent<MLuaUIGroup>(transform);
            if (group != null)
            {
                setComRefs(ref group.ComRefs);
            }
            else
            {
                if (UIPanel != null)
                {
                    setComRefs(ref UIPanel.ComRefs);
                }
            }

            Name = gameObject.name;
            GenBindScript = false;
        }

        void setComRefs(ref MLuaUICom[] components)
        {
            if (!MArrayHelper.IsArrayContain(components, this))
            {
                components = MArrayHelper.Add(components, this);
            }
        }

        #endregion 生命周期

        public void Clear()
        {

        }

#if WINDOWS_DEBUG
        private bool e_isInited = false;
        void Update()
        {
            if (e_isInited) return;

            if (string.IsNullOrEmpty(Name))
            {
                Name = gameObject.name;
            }
            e_isInited = true;
        }
#endif
    }

}
