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
using UnityEngine;

namespace MFrameWork
{
    public partial class MObjectManager : MSingleton<MObjectManager> 
    {
        //全局存储类对象池的Dic Key是类的类型 Value是这个类的类对象池 最终会转换为Pool
        protected Dictionary<Type,object> m_classPoolDic = null;

        public override bool Init()
        {
            bool bObject = InitObjectManager();
            bool bSyncObject = InitSyncObjectManager();
            bool bAsyncObject = InitASyncObjectManager();
            return bObject && bSyncObject;
        }

        public override void OnLogOut()
        {
            base.OnLogOut();
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        public bool InitObjectManager() 
        {
            m_classPoolDic = new Dictionary<Type, object>();
            return true;
         }
        /// <summary>
        /// 创建类对象池
        /// </summary>
        /// <returns>某一个类的对象池</returns>
        /// <param name="Count">类对象池的大小</param>
        public MClassObjectPool<T> GetClassPool<T>(int Count) where T : class, new()
        {
            Type type = typeof(T);
            object outObj = null;
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