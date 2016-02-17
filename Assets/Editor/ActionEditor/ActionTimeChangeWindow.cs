//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionTimeChangeWindow
// 
// Created by : LeoLi at 2015/12/21 11:20:26
//
// Purpose : 
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;

public class ActionTimeChangeWindow : EditorWindow
{
    static public ActionTimeChangeWindow Instance
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
    
    // Editor State
    private readonly float WINDOW_MIN_WIDTH = 300;
    private readonly float WINDOW_MIN_HIEGHT = 100;
    private static ActionTimeChangeWindow m_Instance;
    // Editor Data
    private float m_fTimeShift = 0f;
    private float m_fMinShift = -1f;
    private float m_fMaxShift = 1f;
    private float m_fDuration = 1f;
    // Action Data
    private List<ActionFrameData> m_lstSelectedFrameData = null;

    #region MonoBehavior
    private void OnGUI()
    {
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("平移时间: ", GUILayout.Width(60f));
            m_fTimeShift = EditorGUILayout.Slider(m_fTimeShift, m_fMinShift, m_fMaxShift);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5f);
        if (GUILayout.Button("保存", GUILayout.Width(100f)))
        {
            foreach (ActionFrameData data in m_lstSelectedFrameData)
            {
                data.Time += m_fTimeShift;
                if (data.Time < 0)
                    data.Time = 0;
                else if (data.Time > m_fDuration)
                    data.Time = m_fDuration;
            }
            ActionEditorWindow.Instance.Repaint();
            m_Instance.Close();
            m_Instance = null;
        }
        if (GUILayout.Button("取消", GUILayout.Width(100f)))
        {
            m_Instance.Close();
            m_Instance = null;
        }
    }
    #endregion

    #region Public Interface
    public void OpenWindow(List<ActionFrameData> selectedFrameData , float duration)
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        Repaint();
        this.Focus();

        m_lstSelectedFrameData = selectedFrameData;
        m_fDuration = duration;
        float tmpMin = (float) m_lstSelectedFrameData[0].Time;
        float tmpMax = (float)m_lstSelectedFrameData[0].Time;
        foreach (ActionFrameData data in m_lstSelectedFrameData)
        {
            if (data.Time < tmpMin)
            {
                tmpMin = (float)data.Time;
            }
            if (data.Time > tmpMax)
            {
                tmpMax = (float)data.Time;
            }
        }
        m_fMaxShift = duration - tmpMax;
        m_fMinShift = -tmpMin;
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

    #region System Functions
    static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<ActionTimeChangeWindow>(false, "平移选择的节点", true);
    }
    #endregion
}

