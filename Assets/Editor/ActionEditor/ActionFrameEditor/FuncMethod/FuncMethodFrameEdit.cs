//========================================================================
// Copyright(C): CYTX
//
// FileName : FuncMethodFrameEdit
// 
// Created by : LeoLi at 2016/1/26 16:56:09
//
// Purpose : 
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;
public class FuncMethodFrameEdit : AbstractFrameEdit
{
    static public FuncMethodFrameEdit Instance
    {
        get
        {
            if (null == m_Instance)
            {
                CreateWindow();
            }

            return m_Instance;
        }
    }

    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static FuncMethodFrameEdit m_Instance;

    private FuncMethodFrameConfig m_Config;

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("功能函数ID:", GUILayout.Width(80f));
            m_Config.FuncID = EditorGUILayout.IntField(m_Config.FuncID);
        }
        EditorGUILayout.EndHorizontal();
        //GUILayout.Space(5f);
        //EditorGUILayout.BeginHorizontal();
        //{
           
        //}
        //EditorGUILayout.EndHorizontal();
    }
    void OnDestroy()
    {

    }
    #endregion

    #region Public Interface
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
        Repaint();
    }
    public static void CloseWindow()
    {
        if (null == m_Instance)
        {
            return;
        }
        m_Instance.Close();
        m_Instance = null;
    }
    #endregion

    #region System Event
    protected void Init()
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.FuncMethodFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new FuncMethodFrameConfig();
        }
    }
    protected override void OnSave()
    {
        m_ActionFrameData.FuncMethodFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
      
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<FuncMethodFrameEdit>(false, "执行功能函数", true);
    }
    #endregion
}

