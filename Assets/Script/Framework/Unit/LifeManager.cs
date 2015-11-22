//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : LifeManager
//
// Created by : Baoxue at 2015/11/17 12:04:33
//
//
//========================================================================

using System.Linq.Expressions;
using UnityEngine;
using System;
using System.Collections.Generic;

class LifeManager
{
    private static Dictionary<int, Ilife> m_LifeMap = new Dictionary<int,Ilife>();
  
    public static void RegisterLife(int id, Ilife life)
    {
        if (m_LifeMap.ContainsKey(id))
        {
            m_LifeMap[id] = life;
        }
        else
        {
            m_LifeMap.Add(id, life);
        }
    }
    public static void UnRegisterLife(int id)
    {
        m_LifeMap.Remove(id);
    }
    public static Ilife GetLife(int id)
    {
        Ilife elem = null;
        if (!m_LifeMap.TryGetValue(id, out elem))
        {
            return null;
        }
        return elem;
    }
    public static Dictionary<int, Ilife> GetLifeList()
    {
        return m_LifeMap;
    }
}

