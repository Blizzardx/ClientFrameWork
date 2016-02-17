//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : PlayAudioFrameEdit
//
// Created by : Baoxue at 2015/11/12 18:00:11
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

class PlayAudioFrameEdit : AbstractFrameEdit
{
    static public PlayAudioFrameEdit Instance
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
    

    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static PlayAudioFrameEdit m_Instance;
    private PlayAudioFrameConfig m_Config;
    private string[] m_TargetTypePopList = new[] { "摄像机", "npc", "玩家", };


    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("声音文件:", GUILayout.Width(100f));
        m_Config.AudioSource = EditorGUILayout.TextField(m_Config.AudioSource);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            m_Config.IsLoop = EditorGUILayout.Toggle("Is loop", m_Config.IsLoop);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            m_Config.IsAttach = EditorGUILayout.Toggle("Is attach", m_Config.IsAttach);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        {
            m_Config.IsCareGender = EditorGUILayout.Toggle("Is care gender", m_Config.IsCareGender);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5f);

        if(m_Config.IsCareGender)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("备选声音文件:", GUILayout.Width(100f));
            m_Config.ParamAudioSource = EditorGUILayout.TextField(m_Config.ParamAudioSource);
            EditorGUILayout.EndHorizontal();
        }

        if (!m_Config.IsAttach)
        {
            EditorGUILayout.BeginHorizontal();
            Vector3 pos = m_Config.PlayPosition.GetVector3();
            EditorGUILayout.LabelField("声音位置: " + pos.ToString(), GUILayout.Width(100f));

            EditorGUILayout.LabelField("x: ");
            pos.x = EditorGUILayout.FloatField(pos.x);
            EditorGUILayout.LabelField("y: ");
            pos.y = EditorGUILayout.FloatField(pos.y);
            EditorGUILayout.LabelField("z: ");
            pos.z = EditorGUILayout.FloatField(pos.z);
            m_Config.PlayPosition.SetVector3(pos);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            m_Config.EntityType =
                (EntityType)
                    (EditorGUILayout.Popup((int) m_Config.EntityType, m_TargetTypePopList, GUILayout.Width(100f)));

            if (m_Config.EntityType == EntityType.Npc)
            {
                EditorGUILayout.LabelField("npc id: ");
                m_Config.AttachNpcId = EditorGUILayout.IntField(m_Config.AttachNpcId);
            }
            EditorGUILayout.EndHorizontal();
        }

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
        m_Instance = EditorWindow.GetWindow<PlayAudioFrameEdit>(false, "播放声音编辑", true);
    }
    protected override void OnPlay()
    {
    }
    protected override void OnSave()
    {
        //Save Data

        m_ActionFrameData.PlayAudioFrame = m_Config;
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
            m_Config = m_ActionFrameData.PlayAudioFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new PlayAudioFrameConfig();
            m_Config.AudioSource = string.Empty;
            m_Config.ParamAudioSource = string.Empty;
            m_Config.PlayPosition = new ThriftVector3();
        }
    }
}

