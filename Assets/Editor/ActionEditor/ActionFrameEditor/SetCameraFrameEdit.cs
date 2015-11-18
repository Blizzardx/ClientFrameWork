//========================================================================
// Copyright(C): CYTX
//
// FileName : SetCameraFrameEdit
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

public class SetCameraFrameEdit : FrameEdit
{
    #region Property
    static public SetCameraFrameEdit Instance
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
    private static SetCameraFrameEdit m_Instance;

    private SetCameraFrameConfig m_Config;
    private ESetCameraType m_eSetCameraType;
    private float m_fTickTime;
    private GameObject m_InputCameraObj;
    private string m_CameraName;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("摄像机选择:", GUILayout.Width(50f));
            m_InputCameraObj = (GameObject)EditorGUILayout.ObjectField(m_InputCameraObj, typeof(GameObject));
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("选取当前摄像机", GUILayout.Width(100f)))
            {
                m_InputCameraObj = GlobalScripts.Instance.mGameCamera.gameObject;
                m_CameraName = m_InputCameraObj.name +"_00";
            }
            EditorGUILayout.LabelField("剧情名称:", GUILayout.Width(50f));
            m_CameraName = EditorGUILayout.TextField(m_CameraName);
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
            m_Config = m_ActionFrameData.SetCameraFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new SetCameraFrameConfig();
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        GlobalScripts.Instance.mGameCamera.ResetCam();
    }
    protected override void OnSave()
    {
        CreateCameraPrefab();
        //Set Data
        m_Config.CamName = m_CameraName;
        m_Config.CamType = ESetCameraType.Permanent; // Temp
        //Save Data
        m_ActionFrameData.Type = (int)m_eFrameType;
        m_ActionFrameData.Time = m_fTime;
        m_ActionFrameData.SetCameraFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
        GlobalScripts.Instance.mGameCamera.ResetCam();
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<SetCameraFrameEdit>(false, "定义摄像机", true);
    }
    private void CreateCameraPrefab()
    {
        foreach (MonoBehaviour script in m_InputCameraObj.GetComponents<MonoBehaviour>())
        {
            DestroyImmediate(script);
        }
        ActionHelper.SaveCameraPrefab(m_CameraName, m_InputCameraObj);
    }
    #endregion
}

