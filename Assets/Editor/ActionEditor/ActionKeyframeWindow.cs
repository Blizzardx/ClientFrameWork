//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionKeyframeWindow
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/10 18:14:09
//
// Purpose : 关键帧节点 选择窗口
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;

public class ActionKeyframeWindow : EditorWindow
{
    #region Property
    static public ActionKeyframeWindow Instance
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
    private readonly float WINDOW_MIN_WIDTH = 300;
    private readonly float WINDOW_MIN_HIEGHT = 200;

    private static ActionKeyframeWindow m_Instance;

    private float m_KeyframeTime;
    private KeyframeData m_KeyframeData;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        if (null == m_KeyframeData)
        {
            return;
        }

        if (null == m_KeyframeData.framedatalist)
        {
            return;
        }

        EditorGUILayout.LabelField("时间: " + m_KeyframeTime.ToString("f2"), GUILayout.Width(100f));
        GUILayout.Space(10f);
        EditorGUILayout.LabelField("数量: " + m_KeyframeData.framedatalist.Count.ToString(), GUILayout.Width(100f));
        GUILayout.Space(10f);

        for (int i = 0; i < m_KeyframeData.framedatalist.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                ActionFrameData temp = m_KeyframeData.framedatalist[i];

                EditorGUILayout.LabelField("节点类型: " + ActionEditorWindow.Instance.m_szActionFrameName[temp.Type], GUILayout.Width(100f));

                if (GUILayout.Button("编辑节点", GUILayout.Width(100f)))
                {
                    ActionEditorWindow.Instance.InsertFrame((EActionFrameType)temp.Type, temp);
                    ActionEditorWindow.Instance.Repaint();
                    m_Instance.Close();
                    m_Instance = null;
                    break;
                }
                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    var option = EditorUtility.DisplayDialog("确定要删除节点吗？",
                                                             "确定吗？确定吗？确定吗？确定吗？确定吗？",
                                                             "确定", "取消");
                    if (option)
                    {
                        ActionEditorWindow.Instance.DelFrame(temp);
                        ActionEditorWindow.Instance.Repaint();
                        m_Instance.Close();
                        m_Instance = null;
                        break;
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Public Interface
    public void OpenWindow(float time, KeyframeData key)
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        m_KeyframeTime = time;
        m_KeyframeData = key;
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

    #region System Functions
    static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<ActionKeyframeWindow>(false, "关键帧节点列表", true);
    }
    #endregion
}

