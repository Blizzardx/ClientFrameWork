//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrameEdit
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 16:46:19
//
// Purpose : 摄像机移动 编辑窗口
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;

public class MoveCameraFrameEdit : FrameEdit
{
    #region Property
    static public MoveCameraFrameEdit Instance
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
    #endregion

    #region Field
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static MoveCameraFrameEdit m_Instance;

    private MoveCameraFrameConfig m_Config;
    private MoveCameraFrame.EMoveCameraState m_eMoveCameraState;
    private float m_fTickTime;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        m_Config.Distance = EditorGUILayout.FloatField("Distance", (float)m_Config.Distance);
        GUILayout.Space(5f);
        m_Config.Height = EditorGUILayout.FloatField("Height", (float)m_Config.Height);
        GUILayout.Space(5f);
        m_Config.OffseHeight = EditorGUILayout.FloatField("OffsetHigh", (float)m_Config.OffseHeight);
        GUILayout.Space(5f);
        m_Config.MoveToTime = EditorGUILayout.FloatField("MoveToTime", (float)m_Config.MoveToTime);
        GUILayout.Space(5f);
        m_Config.StayTime = EditorGUILayout.FloatField("StayTime", (float)m_Config.StayTime);
        GUILayout.Space(5f);
        m_Config.MoveBackTime = EditorGUILayout.FloatField("MoveBackTime", (float)m_Config.MoveBackTime);
    }
    void Update()
    {
        float fCurrtime = TimeManager.Instance.GetTime();
        switch (m_eMoveCameraState)
        {
            case MoveCameraFrame.EMoveCameraState.MoveTo:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = fCurrtime + (float)m_Config.StayTime;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.Stay;
                }
                break;
            case MoveCameraFrame.EMoveCameraState.Stay:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = fCurrtime + (float)m_Config.MoveBackTime;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveBack;

                    GlobalScripts.Instance.mGameCamera.ResetCam();
                }
                break;
            case MoveCameraFrame.EMoveCameraState.MoveBack:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = 0f;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.None;
                }
                break;
        }
    }
    void OnDestroy()
    {
        GlobalScripts.Instance.mGameCamera.ResetCam();
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
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.MoveCameraFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new MoveCameraFrameConfig();
            m_Config.Distance = GlobalScripts.Instance.mGameCamera.m_fDistance;
            m_Config.Height = GlobalScripts.Instance.mGameCamera.m_fHeight;
            m_Config.OffseHeight = GlobalScripts.Instance.mGameCamera.m_fOffsetHeight;
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        GlobalScripts.Instance.mGameCamera.ResetCam();
    }
    protected override void OnSave()
    {
        m_ActionFrameData.Type = (int)m_eFrameType;
        m_ActionFrameData.Time = m_fTime;
        m_ActionFrameData.MoveCameraFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
        GlobalScripts.Instance.mGameCamera.ResetCam();

        float fDetalDistance = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fDistance - (float)m_Config.Distance);
        float fDistanceDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalDistance / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fDistance = (float)m_Config.Distance;
        GlobalScripts.Instance.mGameCamera.m_fDistanceDamping = fDistanceDamping;

        float fDetalOffsetHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fOffsetHeight - (float)m_Config.OffseHeight);
        float fOffsetHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalOffsetHeight / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fOffsetHeight = (float)m_Config.OffseHeight;
        GlobalScripts.Instance.mGameCamera.m_fOffsetHeightDamping = fOffsetHeightDamping;

        float fDetalHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fHeight - (float)m_Config.Height);
        float fHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalHeight / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fHeight = (float)m_Config.Height;
        GlobalScripts.Instance.mGameCamera.m_fHeightDamping = fHeightDamping;


        m_fTickTime = TimeManager.Instance.GetTime() + (float)m_Config.MoveToTime;
        m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveTo;
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<MoveCameraFrameEdit>(false, "移动摄像机", true);
    }
    #endregion
}

