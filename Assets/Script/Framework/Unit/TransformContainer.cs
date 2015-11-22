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
    [SerializeField]
    private int                     m_iId;
    [SerializeField]
    private Ilife                   m_Data;
    [SerializeField]
    private static List<GameObject> m_SceneObjList = new List<GameObject>();

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
        m_SceneObjList.Remove(this.gameObject);
    }
    private void Start()
    {
        m_SceneObjList.Add(this.gameObject);
    }
    public static List<GameObject> GetSceneObjList()
    {
        return m_SceneObjList;
    }
}

