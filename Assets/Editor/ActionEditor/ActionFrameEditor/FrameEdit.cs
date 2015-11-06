//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrameEdit
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 15:51:32
//
// Purpose : 剧情编辑窗口 基类
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;

public class FrameEdit : EditorWindow
{
    #region Property

    #endregion

    #region Field
    protected float m_fTime;
    protected float m_fTotalTime;
    protected EActionFrameType m_eFrameType;
    protected ActionFrameData m_ActionFrameData;
    #endregion

    #region Public Interface

    #endregion

    #region System Functions
    protected void SetBaseInfo(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_fTime = fTime;
        m_fTotalTime = fTotalTime;
        m_eFrameType = eType;
        m_ActionFrameData = data;
    }

    protected virtual void DrawBaseInfo()
    {
        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            m_fTime = EditorGUILayout.Slider(m_fTime, 0, m_fTotalTime, GUILayout.Width(600f));
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

        GUILayout.Space(5f);

        EditorGUILayout.LabelField("时间" + m_fTime.ToString("f3"), GUILayout.Width(100f));
    }


    protected virtual void OnPlay()
    {
    }

    protected virtual void OnSave()
    {
    }
    #endregion
}

