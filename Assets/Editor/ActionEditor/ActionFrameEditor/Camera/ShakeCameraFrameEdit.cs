//========================================================================
// Copyright(C): CYTX
//
// FileName : ShakeCameraFrameEdit
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 16:49:54
//
// Purpose : 摄像机设定 编辑窗口
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;

public class ShakeCameraFrameEdit : AbstractFrameEdit
{
    #region Property
    static public ShakeCameraFrameEdit Instance
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
    private static ShakeCameraFrameEdit m_Instance;

    private ShakeCameraFrameConfig m_Config;
    private Vector3 m_ShakeAmount = new Vector3 (1,1,1);
    private Vector3 m_LastTimeShakeAmount;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            m_Config.Time = EditorGUILayout.FloatField("震动时长:", (float)m_Config.Time);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("震动幅度:", GUILayout.Width(80f));

            GUILayout.Label("x", GUILayout.Width(20f));
            m_ShakeAmount.x = EditorGUILayout.FloatField(m_ShakeAmount.x);
            GUILayout.Label("y", GUILayout.Width(20f));
            m_ShakeAmount.y = EditorGUILayout.FloatField(m_ShakeAmount.y);
            GUILayout.Label("z", GUILayout.Width(20f));
            m_ShakeAmount.z = EditorGUILayout.FloatField(m_ShakeAmount.z);

            if (m_LastTimeShakeAmount != m_ShakeAmount)
            {
                m_Config.Amount.SetVector3(m_ShakeAmount);
            }

            m_LastTimeShakeAmount = m_ShakeAmount;
        }
        EditorGUILayout.EndHorizontal();
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
            m_Config = m_ActionFrameData.ShakeCameraFrame;
            m_ShakeAmount = m_Config.Amount.GetVector3();
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new ShakeCameraFrameConfig();
            m_Config.Amount = new Common.Auto.ThriftVector3();
            m_Config.Time = 1f;
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

    }
    protected override void OnSave()
    {
        //Save Data
        //m_Config.CamName = m_CameraName;
        //m_Config.CamType = ESetCameraType.Permanent; // Temp
        m_ActionFrameData.ShakeCameraFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
        GameObject tmpObj = GameObject.Find("MainCamera");
        if (null != tmpObj)
        {
            GlobalScripts.Instance.mGameCamera.ShakeCamera((float)m_Config.Time, m_Config.Amount.GetVector3());
        }
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<ShakeCameraFrameEdit>(false, "摄像机震动", true);
    }
    #endregion
}

