//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_MoveEffectFrameEdit
//
// Created by : Baoxue at 2015/11/24 20:14:59
//
//
//========================================================================

using ActionEditor;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_MoveEffectFrameEdit : AbstractFrameEdit
{
    private string m_strEffectInstanceId;
    
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;
    private static Runtime_MoveEffectFrameEdit m_Instance;
    private Runtime_MoveEffectFrameConfig m_Config;

    static public Runtime_MoveEffectFrameEdit Instance
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
            EditorGUILayout.LabelField("特效文件instance ID:", GUILayout.Width(150f));
            m_strEffectInstanceId = EditorGUILayout.TextField(m_strEffectInstanceId);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            float high = (float) (m_Config.High);
            float time = (float) (m_Config.Time);
            GUILayout.Label("高度", GUILayout.Width(50f));

            high = EditorGUILayout.FloatField(high);
            GUILayout.Label("移动持续时间", GUILayout.Width(50f));
            time = EditorGUILayout.FloatField(time);

            m_Config.High = high;
            m_Config.Time = time;
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
        m_Instance = EditorWindow.GetWindow<Runtime_MoveEffectFrameEdit>(false, "运行时移动特效编辑", true);

    }
    protected override void OnPlay()
    {
        GameObject obj = EffectContainer.EffectFactory("");
    }
    protected override void OnSave()
    {
        m_Config.InstanceId = m_strEffectInstanceId;
        //Save Data
        m_ActionFrameData.Runtime_MoveEffect = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.Runtime_MoveEffect;
            m_strEffectInstanceId = m_Config.InstanceId;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new Runtime_MoveEffectFrameConfig();
            m_strEffectInstanceId = string.Empty;
        }
    }
}

