//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : TransformContainer
//
// Created by : Baoxue at 2015/11/17 11:56:31
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;

class TransformContainer : MonoBehaviour
{
    private int     m_iId;
    private Ilife   m_Data;

    public void Initialize(int id,Ilife data)
    {
        m_iId = id;
        m_Data = data;
    }
    public Ilife GetData()
    {
        return m_Data;
    }

    public int GetId()
    {
        return m_iId;
    }
    private void OnDestroy()
    {
        // to do:
    }
}

