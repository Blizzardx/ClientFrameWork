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
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PlayAudioFrameEdit : FrameEdit
{
    #region Property
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
        Repaint();
    }
    #endregion

    #region Field

    private static PlayAudioFrameEdit m_Instance;
    private string m_strResourceName;
    private string m_strX;
    private string m_strY;
    private string m_strZ;
    private Vector3 m_PlayPosition;
    private bool    m_bIsLoop;

    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("声音文件:", GUILayout.Width(50f));
            m_strResourceName = GUILayout.TextArea(m_strResourceName);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("声音位置:", GUILayout.Width(50f));
            EditorGUILayout.LabelField("x: " + m_PlayPosition.x, GUILayout.Width(50f));
            m_strX = GUILayout.TextArea(m_strX);
            EditorGUILayout.LabelField("y: " + m_PlayPosition.y, GUILayout.Width(50f));
            m_strY = GUILayout.TextArea(m_strY);
            EditorGUILayout.LabelField("z: " + m_PlayPosition.z, GUILayout.Width(50f));
            m_strZ = GUILayout.TextArea(m_strZ);

            float.TryParse(m_strX, out m_PlayPosition.x);
            float.TryParse(m_strY, out m_PlayPosition.y);
            float.TryParse(m_strZ, out m_PlayPosition.z);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        {
            m_bIsLoop = EditorGUILayout.Toggle("Is loop", m_bIsLoop);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("播放", GUILayout.Width(100f)))
            {
                OnPlay();
            }

            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {
                OnSave();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Public Interface

    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<PlayAudioFrameEdit>(false, "播放声音编辑", true);
        m_Instance.Init();
    }
    protected override void OnPlay()
    {
        AudioManager.Instance.PlayAudio(m_strResourceName, m_PlayPosition, m_bIsLoop);
    }
    protected override void OnSave()
    {
        
    }
    private void Init()
    {
        m_strResourceName = string.Empty;
        m_strX = string.Empty;
        m_strY = string.Empty;
        m_strZ = string.Empty;
    }

    #endregion
}

