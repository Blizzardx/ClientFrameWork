//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// NameSpace : Assets.Script.Framework.EditorRuntimeScript.ActionEditor
// FileName : ActionEditorMain
//
// Created by : LeoLi (742412055@qq.com) at 2015/10/29 16:49:30
//
//
//========================================================================
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters;
using System.Collections;

public class ActionEditorMain : SingletonTemplateMon<ActionEditorMain>
{
    #region Property
    public float m_fSpeed = 0.1f;
    public float m_fRotate = 0.1f;
    #endregion

    #region Field
    private GameObject m_ObjSceneCamera;
    private Vector3 m_vDelta;
    private Vector3 m_vTmp;
    private Vector3 m_vInitPos;
    private Vector3 m_vCamInitRotate;
    private Action m_ClearEditorWindowCallBack;
    private Action m_CloseEditorWindowCallBack;
    private Action<Vector3> m_RaycastCallBack;
    private Camera m_SceneCamera;
    #endregion

    #region MonoBehavior
    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Public Interface

    #endregion

    #region System Function

    #endregion
}
