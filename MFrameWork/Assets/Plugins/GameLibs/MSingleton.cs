using System;

namespace MFrameWork
{
    public abstract class MBaseSingleton
    {
        public abstract bool Init();
        public abstract void Uninit();
        public abstract void OnLogout();
    }

    public abstract class MSingleton<T> : MBaseSingleton where T : new()
    {
        protected MSingleton()
        {
             /* Here we added restriction in case a totally new Object of T be created outside.
             * It is not very delicate but works.
             * Also we can use reflection to get ctor. of T in a usual way, But
             * it's may cost more performance
             */
            if (null != _instance)
            {
                throw new MDoubleNewException(_instance.ToString() + @" can not be created again.");
            }
        }

        private static readonly T _instance = new T();

        public static T singleton
        {
            get
            {
                return _instance;
            }
        }

        public override bool Init() { return true; }
        public override void Uninit() { }
        public override void OnLogout() { }
    }
}