﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TemplateQueue<T> where T : class, new()
{
    private int maxSize = int.MaxValue;
    private Queue<T> m_Queue;
    private bool m_bIsLock;

    public void Initialize(bool isLock = true)
    {
        m_bIsLock = isLock;
        m_Queue = new Queue<T>(1024);
    }
    public bool Offer(T message)
    {
        if (m_bIsLock)
        {
            lock (m_Queue)
            {
                if (m_Queue.Count >= maxSize)
                {
                    return false;
                }
                m_Queue.Enqueue(message);
                return true;
            }
        }
        else
        {
            if (m_Queue.Count >= maxSize)
            {
                return false;
            }
            m_Queue.Enqueue(message);
            return true;
        }
    }

    public T Poll()
    {
        if (m_bIsLock)
        {
            lock (m_Queue)
            {
                if (m_Queue.Count == 0)
                {
                    return null;
                }
                return m_Queue.Dequeue();
            }
        }
        else
        {
            if (m_Queue.Count == 0)
            {
                return null;
            }
            return m_Queue.Dequeue();
        }
    }
}
