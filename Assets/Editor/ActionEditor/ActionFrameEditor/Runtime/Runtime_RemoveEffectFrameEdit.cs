//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_RemoveEffectFrameEdit
//
// Created by : Baoxue at 2015/11/27 16:15:24
//
//
//========================================================================

using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_RemoveEffectFrameEdit : AbstractFrameEdit
{
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static Runtime_RemoveEffectFrameEdit m_Instance;
    private Runtime_RemoveEffectFrameConfig m_Config;
    private string m_instanceId;

    static public Runtime_RemoveEffectFrameEdit Instance
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
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
        Repaint();
    }
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("特效实例ID:", GUILayout.Width(50f));

            m_instanceId = EditorGUILayout.TextField(m_instanceId);
        }
        EditorGUILayout.EndHorizontal();
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
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<Runtime_RemoveEffectFrameEdit>(false, "运行时删除特效编辑", true);

    }
    protected override void OnPlay()
    {
    }
    protected override void OnSave()
    {
        m_Config.InstanceId = m_instanceId;

        //Save Data
        m_ActionFrameData.Runtime_RemoveEffect = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.Runtime_RemoveEffect;
            m_instanceId = m_Config.InstanceId;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
           m_Config = new Runtime_RemoveEffectFrameConfig();
            m_Config.InstanceId = string.Empty;
        }
    }
}


