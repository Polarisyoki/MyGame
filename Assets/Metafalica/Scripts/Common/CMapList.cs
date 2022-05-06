using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class CMapList<T> where T : class, new()
    {
        private DoubleLinkedList<T> m_DLink = new DoubleLinkedList<T>();
        private Dictionary<T, DoubleLinkedListNode<T>> m_FindMap = new Dictionary<T, DoubleLinkedListNode<T>>();

        ~CMapList()
        {
            Clear();
        }
        public void Clear()
        {
            while (m_DLink.Tail != null)
            {
                Remove(m_DLink.Tail.t);
            }
        }
        
        public void InsertToHead(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (m_FindMap.TryGetValue(t, out node) && node != null)
            {
                m_DLink.AddToHeader(node);
                return;
            }

            m_DLink.AddToHeader(t);
            m_FindMap.Add(t, m_DLink.Head);
        }

        /// <summary>
        /// 从表尾弹出一个节点
        /// </summary>
        public void Pop()
        {
            if (m_DLink.Tail != null)
            {
                Remove(m_DLink.Tail.t);
            }
        }


        public void Remove(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (!m_FindMap.TryGetValue(t, out node) || node == null)
            {
                return;
            }

            m_DLink.RemoveNode(node);
            m_FindMap.Remove(t);
        }

        public T Back()
        {
            return m_DLink.Tail == null ? null : m_DLink.Tail.t;
        }

        /// <summary>
        /// 返回节点个数
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return m_FindMap.Count;
        }

        public bool Find(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (!m_FindMap.TryGetValue(t, out node) || node == null)
                return false;
            return true;
        }

        /// <summary>
        /// 刷新链表,把节点添加到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Refresh(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (!m_FindMap.TryGetValue(t, out node) || node == null)
                return false;
            m_DLink.MoveToHead(node);
            return true;
        }
    }

    //双向链表结构节点
    public class DoubleLinkedListNode<T> where T : class, new()
    {
        //前一个节点
        public DoubleLinkedListNode<T> prev = null;

        //后一个节点
        public DoubleLinkedListNode<T> next = null;

        //当前节点
        public T t = null;
    }

    //双向链表结构
    public class DoubleLinkedList<T> where T : class, new()
    {
        //链表头
        public DoubleLinkedListNode<T> Head = null;

        //链表尾
        public DoubleLinkedListNode<T> Tail = null;

        //双向链表结构类对象池
        protected ClassObjectPool<DoubleLinkedListNode<T>> m_DoubleLinkedNodePool =
            ObjectManager.Instance.GetOrCreateClassPool<DoubleLinkedListNode<T>>(500);

        protected int m_Count = 0;
        public int Count => m_Count;

        public DoubleLinkedListNode<T> AddToHeader(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkedNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToHeader(pList);
        }

        public DoubleLinkedListNode<T> AddToHeader(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return null;
            pNode.prev = null;
            if (Head == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.next = Head;
                Head.prev = pNode;
                Head = pNode;
            }

            m_Count++;
            return Head;
        }

        public DoubleLinkedListNode<T> AddToTail(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkedNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToHeader(pList);
        }

        public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return null;
            pNode.next = null;
            if (Tail == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.prev = Tail;
                Tail.next = pNode;
                Tail = pNode;
            }

            m_Count++;
            return Tail;
        }

        public void RemoveNode(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return;
            if (pNode == Head)
                Head = pNode.next;
            if (pNode == Tail)
                Tail = pNode.prev;
            if (pNode.prev != null)
                pNode.prev.next = pNode.next;
            if (pNode.next != null)
                pNode.next.prev = pNode.prev;
            pNode.next = pNode.prev = null;
            pNode.t = null;
            m_DoubleLinkedNodePool.Recycle(pNode);
            m_Count--;
        }

        public void MoveToHead(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null || pNode == Head)
                return;
            if (pNode.prev == null && pNode.next == null)
                return;
            if (pNode == Tail)
                Tail = pNode.prev;
            if (pNode.prev != null)
                pNode.prev.next = pNode.next;
            if (pNode.next != null)
                pNode.next.prev = pNode.prev;

            pNode.prev = null;
            pNode.next = Head;
            Head.prev = pNode;
            Head = pNode;
            if (Tail == null)
            {
                Tail = Head;
            }
        }
    }
}