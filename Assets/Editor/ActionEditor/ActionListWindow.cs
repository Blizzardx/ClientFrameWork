//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ActionListWindow
//
// Created by : LeoLi (742412055@qq.com) at 2015/11/3 14:35:27
//
//
//========================================================================
using Assets.Scripts.Core.Utils;
using Communication;
using ActionEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class ActionListWindow : EditorWindow
{
    #region Property
    static public ActionListWindow Instance
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
    private static ActionListWindow m_Instance;
    private ActionFileDataArray m_DataList;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        if (null == m_DataList)
        {
            return;
        }

        if (null == m_DataList.DataList)
        {
            return;
        }

        for (int i = 0; i < m_DataList.DataList.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("名称: " + m_DataList.DataList[i].FileName, GUILayout.Width(100f));

                EditorGUILayout.LabelField("ID: " + m_DataList.DataList[i].ID, GUILayout.Width(100f));

                if (GUILayout.Button("选择", GUILayout.Width(100f)))
                {
                    ChoiseMap(m_DataList.DataList[i]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Public Interface
    public void OpenWindow()
    {
        m_DataList = ActionHelper.GetActionEditFileList();
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
        m_Instance = EditorWindow.GetWindow<ActionListWindow>(false, "地形方案列表", true);
    }
    private void ChoiseMap(ActionFileData data)
    {
        m_Instance.Close();
        m_Instance = null;
        ActionEditorWindow.Instance.OpenAction(data);
    }
    #endregion





}

