// **************************************
//
// 文件名(MDoubleLinkedList.cs):
// 功能描述("双向链表 为什么使用双向链表管理？因为资源的使用频率 高频使用放在头部 低频底部"):
// 作者(Max1993):
// 日期(2019/5/4  12:21):
//
// **************************************
//
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    //双向列表的节点数据结构
    public class MDoubleLinkedListNode<T> where T : class,new()
    {
        //该节点指向的前一个节点
        public MDoubleLinkedListNode<T> m_prev = null;
        //该节点指向的后一个节点
        public MDoubleLinkedListNode<T> m_next= null;
        //当前节点
        public T m_t = null;
    }

    //双向列表
    public class MDoubleLinkedList<T> where T : class, new() 
    {
        //当前双项列表的头部节点
        public MDoubleLinkedListNode<T> m_head = null;
        //当前双项列表的尾部节点
        public MDoubleLinkedListNode<T> m_tail = null;
        //当前双项链表节后的类对象池
        protected MClassObjectPool<MDoubleLinkedListNode<T>> mDoubleLinkedListNodePool = 
            new MClassObjectPool<MDoubleLinkedListNode<T>>(500);

        //双项列表的大小
        private int m_count = 0;

        //大小属性 对外可访问
        public int Count 
        {
            get { return m_count; } 
        }

        //添加一个节点到头部
        public MDoubleLinkedListNode<T> AddToHead(T t) 
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = mDoubleLinkedListNodePool.Spawn(true);
            mDoubleLinkedListNode.m_prev = null;
            mDoubleLinkedListNode.m_next = null;
            mDoubleLinkedListNode.m_t = t;
            return AddToHead(mDoubleLinkedListNode);
        }

        public MDoubleLinkedListNode<T> AddToHead(MDoubleLinkedListNode<T> mDoubleLinkedListNode)
        {
            if (mDoubleLinkedListNode == null)
                return null;

            //添加到头 前一个肯定为空
            mDoubleLinkedListNode.m_prev = null;

            if (m_head == null)
            {
                m_head = m_tail = mDoubleLinkedListNode;
            }
            else
            {
                mDoubleLinkedListNode.m_next = m_head;
                m_head.m_prev = mDoubleLinkedListNode;
                m_head = mDoubleLinkedListNode;
            }
            m_count++;
            return m_head;
        }

        //添加一个节点到尾部
        public MDoubleLinkedListNode<T> AddToTail(T t)
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = mDoubleLinkedListNodePool.Spawn(true);
            mDoubleLinkedListNode.m_prev = null;
            mDoubleLinkedListNode.m_next = null;
            mDoubleLinkedListNode.m_t = t;
            return AddToTail(mDoubleLinkedListNode);
        }

        public MDoubleLinkedListNode<T> AddToTail(MDoubleLinkedListNode<T> mDoubleLinkedListNode)
        {
            if (mDoubleLinkedListNode == null)
                return null;

            //添加到尾 前一个肯定为空
            mDoubleLinkedListNode.m_next = null;

            if (m_head == null)
            {
                m_tail = m_head = mDoubleLinkedListNode;
            }
            else
            {
                mDoubleLinkedListNode.m_prev = m_tail;
                m_head.m_next = mDoubleLinkedListNode;
                m_tail = mDoubleLinkedListNode;
            }
            m_count++;
            return m_tail;
        }

        //移除一个节点
        public void RemoveNode(MDoubleLinkedListNode<T> mDoubleLinkedListNode) 
        {
            if (mDoubleLinkedListNode == null)
                return;

            if (mDoubleLinkedListNode == m_head)
                m_head = mDoubleLinkedListNode.m_next;

            if (mDoubleLinkedListNode == m_tail)
                m_tail = mDoubleLinkedListNode.m_prev;

            if (mDoubleLinkedListNode.m_prev != null)
                mDoubleLinkedListNode.m_prev.m_next = mDoubleLinkedListNode.m_next;

            if (mDoubleLinkedListNode.m_next != null)
                mDoubleLinkedListNode.m_next.m_prev = mDoubleLinkedListNode.m_prev;

            mDoubleLinkedListNode.m_next = mDoubleLinkedListNode.m_prev = null;
            mDoubleLinkedListNode.m_t = null;
            mDoubleLinkedListNodePool.Recycle(mDoubleLinkedListNode);
            m_count--;
        }

        //把某一个Node移动到头部
        public void MoveNodeToHead(MDoubleLinkedListNode<T> mDoubleLinkedListNode)
        {
            if (mDoubleLinkedListNode == null || mDoubleLinkedListNode == m_head)
                return;

            if (mDoubleLinkedListNode.m_prev == null &&
                mDoubleLinkedListNode.m_next == null)
                return;

            if (mDoubleLinkedListNode == m_tail)
                m_tail = mDoubleLinkedListNode.m_prev;

            if (mDoubleLinkedListNode.m_prev != null)
                mDoubleLinkedListNode.m_prev.m_next = mDoubleLinkedListNode.m_next;

            if (mDoubleLinkedListNode.m_next != null)
                mDoubleLinkedListNode.m_next.m_prev = mDoubleLinkedListNode.m_prev;

            mDoubleLinkedListNode.m_prev = null;
            mDoubleLinkedListNode.m_next = m_head;
            m_head.m_prev = mDoubleLinkedListNode;
            m_head = mDoubleLinkedListNode;

            if (m_tail == null)
                m_tail = m_head;
        }
    }

    //封装双项链表
    public class CMapList<T> where T : class, new()
    {
        MDoubleLinkedList<T> mDoubleLinkedList = new MDoubleLinkedList<T>();

        Dictionary<T, MDoubleLinkedListNode<T>> m_findMap = new Dictionary<T, MDoubleLinkedListNode<T>>();

        ~CMapList() {
            Clear();
        }

        //插入到表头
        public void InsertToHead(T t) 
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = null;
            if(m_findMap.TryGetValue(t,out mDoubleLinkedListNode) && mDoubleLinkedListNode!= null) 
            {
                mDoubleLinkedList.AddToHead(mDoubleLinkedListNode);
                return;
            }
            mDoubleLinkedList.AddToHead(mDoubleLinkedListNode);
            if (!m_findMap.ContainsKey(t)) 
            { 
                m_findMap.Add(t, mDoubleLinkedList.m_head);
            }
        }

        //从表尾部 弹出一个节点
        public void PopTail() 
        {
            if (mDoubleLinkedList.m_tail != null) 
            {
                Remove(mDoubleLinkedList.m_tail.m_t);
            }
        }

        //删除某一个节点
        public void Remove(T t) 
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = null;
            if (!m_findMap.TryGetValue(t, out mDoubleLinkedListNode) || mDoubleLinkedListNode != null)
            {
                return;
            }
            mDoubleLinkedList.RemoveNode(mDoubleLinkedListNode);
        }

        //获取尾部节点
        public T GetTail() 
        {
            return mDoubleLinkedList.m_tail == null ? null : mDoubleLinkedList.m_tail.m_t;
        }

        //返回大小
        public int Size() 
        {
            return m_findMap.Count; 
        }

        //是否存在某一个元素
        public bool IsContain(T t)
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = null;
            if (!m_findMap.TryGetValue(t, out mDoubleLinkedListNode) || mDoubleLinkedListNode == null)
            {
                return false;
            }
            return mDoubleLinkedListNode!=null;
        }

        //移动某个元素到头部
        public bool MoveToHead(T t) 
        {
            MDoubleLinkedListNode<T> mDoubleLinkedListNode = null;
            if (!m_findMap.TryGetValue(t, out mDoubleLinkedListNode) || mDoubleLinkedListNode == null)
            {
                return mDoubleLinkedListNode != null;
            }
            mDoubleLinkedList.MoveNodeToHead(mDoubleLinkedListNode);
            return true;
        }

        //清空双项列表
        public void Clear() 
        {
            while(mDoubleLinkedList.m_tail != null) 
            {
                Remove(mDoubleLinkedList.m_tail.m_t);
            }
        }
    }
}