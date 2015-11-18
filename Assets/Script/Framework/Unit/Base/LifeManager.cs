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
    private static Dictionary<int, List<Ilife>> m_LifeMap = new Dictionary<int, List<Ilife>>();

    public static void RegisterLife(int id, Ilife life)
    {
        List<Ilife> elem = null;
        if (!m_LifeMap.TryGetValue(id, out elem))
        {
            elem = new List<Ilife>();
            m_LifeMap.Add(id, elem);
        }
        for (int i = 0; i < elem.Count; ++i)
        {
            if (elem[i] == life)
            {
                return;
            }
        }
        elem.Add(life);
    }
    public static void UnRegisterLife(int id, Ilife life)
    {
        List<Ilife> elem = null;
        if (!m_LifeMap.TryGetValue(id, out elem))
        {
            return;
        }
        for (int i = 0; i < elem.Count; ++i)
        {
            if (elem[i] == life)
            {
                elem.RemoveAt(i);
                return;
            }
        }
    }
    public static List<Ilife> GetLife(int id)
    {
        List<Ilife> elem = null;
        if (!m_LifeMap.TryGetValue(id, out elem))
        {
            return null;
        }
        return elem;
    }
    public static Dictionary<int, List<Ilife>> GetLifeList()
    {
        return m_LifeMap;
    }
}

