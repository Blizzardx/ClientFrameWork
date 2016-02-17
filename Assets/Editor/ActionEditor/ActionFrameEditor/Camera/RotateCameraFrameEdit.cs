//========================================================================
// Copyright(C): CYTX
//
// FileName : RotateCameraFrameEdit
// 
// Created by : LeoLi at 2016/1/22 16:34:08
//
// Purpose : 
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;
public class RotateCameraFrameEdit: AbstractFrameEdit
{
    static public RotateCameraFrameEdit Instance
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

    //State
    private static RotateCameraFrameEdit m_Instance;

    //Data
    private RotateCameraFrameConfig m_Config;

    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<RotateCameraFrameEdit>(false, "旋转摄像机", true);
    }

    // Events
    private void Init()
    {
        //Window Setting
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        //Update Info
        if (null != m_ActionFrameData)
        {
            //base
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.RotCameraFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new RotateCameraFrameConfig();
        }
    }
    private void OnGUI()
    {
        DrawBaseInfo();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("旋转角度:", GUILayout.Width(80f));
            m_Config.Rotation = EditorGUILayout.FloatField((float)m_Config.Rotation);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("旋转速度:", GUILayout.Width(80f));
            m_Config.Speed = EditorGUILayout.Slider((float)m_Config.Speed, 0f, 359.9f);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void OnDestroy()
    {
        ClearData();
    }
    protected override void OnSave()
    {
        //Check Info
        if (m_Config == null)
        {
            EditorUtility.DisplayDialog("配置信息为空", "请补全表中信息", "ok");
            return;
        }

        //Save Data
        m_ActionFrameData.RotCameraFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        ClearData();
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
    }

    // Public Interface
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
    
    // System Method
    private void ClearData()
    {
        // clear config
        if (null != m_Config)
        {
            m_Config = new RotateCameraFrameConfig();
        }
    }
}

