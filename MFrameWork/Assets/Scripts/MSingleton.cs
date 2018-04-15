using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public abstract class MBaseSingleton
    {
        public abstract void Init();
        public abstract void UnInit();
        public abstract void OnLogOut();
    }

    public abstract class MSingleton<T> : MBaseSingleton where T : new()
    {
        private static readonly T _instatce = new T();

        protected MSingleton()
        {
            if (_instatce == null)
            {
                // Here is method in order to prevent the same instance create again
                throw new System.Exception(_instatce.ToString()+"can not create again");
            }
        }

        public static T singleton
        {
            get
            {
                return _instatce;
            }
        }

        public override void Init()
        {
            //This funcation is called when instance is used the first time
            //Put all the initializations you need here , as you would do in awake  
        }

        public override void UnInit()
        {
            
        }

        public override void OnLogOut()
        {
            
        }

    }

}
