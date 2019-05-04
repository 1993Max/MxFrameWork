// **************************************
//
// 文件名(MClassObjectManager.cs):
// 功能描述("类对象池管理类"):
// 作者(Max1993):
// 日期(2019/5/2  21:33):
//
// **************************************
//

using System.Collections;
using System.Collections.Generic;
using System;

namespace MFrameWork
{
    public class MClassObjectManager : MSingleton<MClassObjectManager> 
    {
        //全局存储类对象池的Dic Key是类的类型 Value是这个类的类对象池 最终会转换为Pool
        protected Dictionary<Type, Object> m_classPoolDic = null;

        public override bool Init()
        {
            m_classPoolDic = new Dictionary<Type, object>();
            return base.Init();
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        /// <summary>
        /// 创建类对象池
        /// </summary>
        /// <returns>某一个类的对象池</returns>
        /// <param name="Count">类对象池的大小</param>
        public MClassObjectPool<T> GetClassPool<T>(int Count) where T : class, new()
        {
            Type type = typeof(T);
            Object outObj = null;
            if (!m_classPoolDic.TryGetValue(type, out outObj) || outObj == null)
            {
                MClassObjectPool<T> newPool = new MClassObjectPool<T>(Count);
                m_classPoolDic.Add(type, newPool);
                return newPool;
            }
            return outObj as MClassObjectPool<T>;
        }
    }
}