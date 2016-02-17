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

public class AbstractFrameEdit : EditorWindow
{
    #region Property

    #endregion

    #region Field
    //State
    private int m_nTargetObjectNum = 0;
    private int m_nTargetObjectLastNum = 0;
    //Data
    protected float m_fTime;
    protected float m_fTotalTime;
    protected List<int> m_lstTargetIDs;
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
        if (data == null)
        {
            //m_ActionFrameData = new ActionFrameData();
            return;
        }
        if (data.TargetIDs != null)
        {
            m_lstTargetIDs = data.TargetIDs;
            m_nTargetObjectNum = m_lstTargetIDs.Count;
            m_nTargetObjectLastNum = m_lstTargetIDs.Count;
        }
        this.Focus();
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

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("时间: " + m_fTime.ToString("f2"), GUILayout.Width(70f));
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("对象数量: ", GUILayout.Width(60f));
            m_nTargetObjectNum = EditorGUILayout.IntField(m_nTargetObjectNum, GUILayout.Width(40f));
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("对象ID列表: ", GUILayout.Width(65f));
            if (m_nTargetObjectNum != m_nTargetObjectLastNum)
            {
                m_lstTargetIDs = new List<int>();
                for (int i = 0; i < m_nTargetObjectNum; i++)
                {
                    m_lstTargetIDs.Add(0);
                }
            }
            if (m_lstTargetIDs != null && m_lstTargetIDs.Count > 0)
            {
                for (int i = 0; i < m_lstTargetIDs.Count; i++)
                {
                    m_lstTargetIDs[i] = EditorGUILayout.IntField(m_lstTargetIDs[i], GUILayout.Width(65f));
                }
            }
            m_nTargetObjectLastNum = m_nTargetObjectNum;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            if (GUILayout.Button("预览", GUILayout.Width(100f)))
            {
                OnPlayBase();
            }

            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {
                OnSaveBase();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20f);
    }
    private void OnPlayBase()
    {
        OnPlay();
    }
    private void OnSaveBase()
    {
        if (m_lstTargetIDs == null)
        {
            m_lstTargetIDs = new List<int>();
        }
        foreach (int index in m_lstTargetIDs)
        {
            if (index <= 0)
            {
                EditorUtility.DisplayDialog("错误", "所有目标ID必须大于等于0", "确定");
                return;
            }
        }

        if (m_lstTargetIDs.Contains(10000001) && !m_lstTargetIDs.Contains(10000002))
        {
            m_lstTargetIDs.Add(10000002);
        }
        else if (m_lstTargetIDs.Contains(10000002) && !m_lstTargetIDs.Contains(10000001))
        {
            m_lstTargetIDs.Add(10000001);
        }

        m_ActionFrameData.Type = (int)m_eFrameType;
        m_ActionFrameData.Time = m_fTime;
        //bug fixed
        //if (m_ActionFrameData.TargetIDs != null)
        m_ActionFrameData.TargetIDs = m_lstTargetIDs;
        OnSave();
    }
    protected virtual void OnPlay()
    {
    }
    protected virtual void OnSave()
    {
    }
    #endregion
}

