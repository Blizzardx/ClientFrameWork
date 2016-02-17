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
    private Vector2 m_EventScorllPos;
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
        m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos);
        {
            for (int i = 0; i < m_DataList.DataList.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("ID: " + m_DataList.DataList[i].ID, GUILayout.Width(100f));

                    EditorGUILayout.LabelField("名称: " + m_DataList.DataList[i].FileName);

                    string time = TimeManager.Instance.CheckTime(m_DataList.DataList[i].TimeStamp);
                    EditorGUILayout.LabelField("修改时间: " + time);

                    if (GUILayout.Button("选择", GUILayout.Width(100f)))
                    {
                        ChoiseMap(m_DataList.DataList[i]);
                    }
                    if (GUILayout.Button("复制", GUILayout.Width(100f)))
                    {
                        Copy(m_DataList.DataList[i]);
                    }
                    if (GUILayout.Button("删除", GUILayout.Width(100f)))
                    {
                        var option = EditorUtility.DisplayDialog("确定要删除剧情方案吗？",
                                                                                   "确定吗？确定吗？确定吗？确定吗？确定吗？",
                                                                                   "确定", "取消");
                        if (option)
                        {
                            Delete(m_DataList.DataList[i]);
                            break;
                        }

                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
            }
        }
        EditorGUILayout.EndScrollView();
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
        m_Instance = EditorWindow.GetWindow<ActionListWindow>(false, "方案列表", true);
    }
    private void ChoiseMap(ActionFileData data)
    {
        m_Instance.Close();
        m_Instance = null;
        ActionEditorWindow.Instance.OpenAction(data);
    }
    private void Copy(ActionFileData data)
    {
        int max = 0;
        for (int i = 0; i < m_DataList.DataList.Count; ++i)
        {
            if (m_DataList.DataList[i].ID > max)
            {
                max = m_DataList.DataList[i].ID;
            }
        }
        ++max;
        ActionFileData elem = new ActionFileData();
        elem.ID = max;
        elem.FileName = data.FileName;
        elem.MapResName = data.MapResName;
        elem.Duration = data.Duration;
        elem.FrameDatalist = data.FrameDatalist;
        m_DataList = ActionHelper.GetActionEditFileList();
        ActionHelper.SaveActionEditFileList(m_DataList, elem);
        ActionHelper.CombineActionEditFileList(m_DataList);
        Repaint();
    }

    private void Delete(ActionFileData data)
    {
        ActionHelper.DeleteActionEditFile(m_DataList, data);
        Repaint();
    }
    #endregion





}

