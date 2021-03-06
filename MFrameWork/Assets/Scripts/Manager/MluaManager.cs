﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaInterface;

namespace MFrameWork
{
    public class MLuaManager : MSingleton<MLuaManager>
    {
        private LuaState _mLuaState = null;

        public LuaState MLuaState
        {
            get
            {
                return _mLuaState;
            }

            set
            {
                _mLuaState = value;
            }
        }

        public override bool Init()
        {
            //创建Lua虚拟机
            MLuaState = new LuaState();
            MLuaState.LuaSetTop(0);
            MLuaState.Start();

            //Wrap绑定
            LuaBinder.Bind(MLuaState);
            //委托
            DelegateFactory.Init();
            //协程
            LuaCoroutine.Register(MLuaState,MGameManager.m_monoBehavior);

            MLuaState.DoFile("Main");
            return base.Init();
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        public override void UnInit()
        {
            MLuaState.CheckTop();
            MLuaState.Dispose();
            MLuaState = null;
            base.UnInit();
        }

        public void LuaGC()
        {
            MLuaState.Collect();
        }
    }
}
