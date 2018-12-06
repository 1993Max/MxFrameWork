using UnityEngine;

namespace MFrameWork
{
    public class MLuaUIGroup : MonoBehaviour
    {
        public string Name;
        public string ClassName;
        [Header("是否在上级中生成Group相关代码")]
        public bool IsGenerateCodeInUpper = true;
        [Header("是否在生成Panel代码时生成Template代码")]
        public bool IsCreateTemplateWithCreatePanel;
        public MLuaUICom[] ComRefs;
        public MLuaUIGroup[] Groups;
        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                {
                    _rectTransform = gameObject.GetComponent<RectTransform>();
                    if (!_rectTransform) MDebug.singleton.AddErrorLogF("nonexist RectTransform on this gameObject, name={0}", gameObject.name);
                }
                return _rectTransform;
            }
        }
    }
}
