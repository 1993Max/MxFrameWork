// **************************************
//
// 文件名(MClassObjectPool.cs):
// 功能描述("类对象池"):
// 作者(Max1993):
// 日期(2019/5/2  21:53):
//
// **************************************
//
using System;
using System.Collections.Generic;

namespace MFrameWork
{
    public class MClassObjectPool<T> where T : class,new()
    {
        //池
        protected Stack<T> m_pool;
        //最大创建对象 <=0 不限制创建个数
        protected int m_maxCount = 0;
        //没有回收的对象个数
        protected int m_notRecycleCount = 0;

        public MClassObjectPool(int maxCount)
        {
            m_pool = new Stack<T>();
            m_maxCount = maxCount;
            for (int i = 0; i < m_maxCount; i++)
            {
                m_pool.Push(new T());
            }
        }

        //参数createIfPoolEmpty 当当前池子已经满了 是否强制创建
        public T Spawn(bool createIfPoolEmpty)
        {
            if(m_pool.Count > 0)
            {
                T poolClass = m_pool.Pop();
                if(poolClass == null)
                {
                    if(createIfPoolEmpty)
                    {
                        poolClass = new T();
                    }
                }
                m_notRecycleCount++;
                return poolClass;
            }
            else
            {
                if(createIfPoolEmpty)
                {
                    T poolClass = new T();
                    m_notRecycleCount++;
                    return poolClass;
                }
            }
            return null;
        }

        //回收对象
        public bool Recycle(T classObj)
        {
            if (classObj == null)
                return false;

            m_notRecycleCount--;
            if(m_pool.Count >= m_maxCount && m_maxCount > 0)
            {
                classObj = null;
                return false;
            }

            m_pool.Push(classObj);
            return true;
        }

    }
}
