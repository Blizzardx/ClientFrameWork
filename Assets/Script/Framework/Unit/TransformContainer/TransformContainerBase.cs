//========================================================================
// Copyright(C): CYTX
//
// FileName : TransformContainerBase
// 
// Created by : LeoLi at 2015/11/25 17:40:05
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformContainerBase : MonoBehaviour
{
    [SerializeField]
    protected int m_iId;
    [SerializeField]
    protected Ilife m_Data;

    #region MonoBehavior
    private void Start()
    {
        LifeManager.AddToSceneObjList(gameObject);
    }
    private void OnDestroy()
    {
        LifeManager.RemoveFromSceneObjList(gameObject);
    }
    #endregion

    #region Public Interface
    public void Initialize(int id, Ilife data)
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
    #endregion

}

