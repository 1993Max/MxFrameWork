namespace MFrameWork
{
    public static class MCommonHelper
    {
        //有些方法返回整形，但会有不满足条件的情况，用IsFalse表示不满足条件时的返回值
        public const int IsFalse = -987654321;
        //用IsTrue表示满足条件时的返回值
        public const int IsTrue = 987654321;

        //可变参数的委托
        public delegate void ActionParams(params object[] datas);

        public static string GetUnityFormatPath(string path)
        {
            return path.Replace('\\', '/');
        }

        //得到当前编辑器所在的平台
        public static eEditorPlatform GetEditorPlatform()
        {
#if   UNITY_STANDALONE
            return eEditorPlatform.Standalone;
#elif UNITY_ANDROID
            return eEditorPlatform.Android;
#elif UNITY_IOS
            return eEditorPlatform.iPhone;
#else
            return eEditorPlatform.Other;
#endif
        }

        public enum eEditorPlatform
        {
            Other,
            Standalone,
            Android,
            iPhone,
        }
    }
}


