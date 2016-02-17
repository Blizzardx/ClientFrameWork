//========================================================================
// Copyright(C): CYTX
//
// FileName : ConflictSolveWindow
// 
// Created by : LeoLi at 2016/1/15 12:31:29
//
// Purpose : 
//========================================================================
using Assets.Scripts.Core.Utils;
using Communication;
using ActionEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class ConflictSolveWindow : EditorWindow
{
    static public ConflictSolveWindow Instance
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

    // State
    private readonly float WINDOW_MIN_WIDTH = 1000;
    private readonly float WINDOW_MIN_HIEGHT = 200;
    private static ConflictSolveWindow m_Instance;
    private GUIStyle titleStyle;
    private Vector2 m_EventScorllPos;
    // Data
    private Dictionary<int, ActionFileData> m_mapLocalData;
    private Dictionary<int, ActionFileData> m_mapRemoteData;
    private Dictionary<int, bool> m_mapCheckInfo;

    public void OpenWindow(Dictionary<int, ActionFileData> conflictData)
    {
        // State
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;
        titleStyle.normal.textColor = Color.white;
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        // Remote Data
        m_mapRemoteData = conflictData;
        m_mapCheckInfo = new Dictionary<int, bool>();
        foreach (int key in m_mapRemoteData.Keys)
        {
            m_mapCheckInfo.Add(key, false);
        }
        // Local Data
        m_mapLocalData = new Dictionary<int, ActionFileData>();
        ActionFileDataArray filedatalist = ActionHelper.GetActionEditFileList();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            foreach (ActionFileData value in filedatalist.DataList)
            {
                if (!m_mapLocalData.ContainsKey(value.ID) && m_mapRemoteData.ContainsKey(value.ID))
                {
                    m_mapLocalData.Add(value.ID, value);
                }
            }
        }
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
    static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<ConflictSolveWindow>(false, "解决冲突", true);
    }

    #region MonoBehavior
    private void OnGUI()
    {
        GUILayout.Space(10f);
        EditorGUILayout.LabelField("冲突文件:", titleStyle);

        if (null == m_mapLocalData)
        {
            return;
        }
        if (null == m_mapRemoteData || m_mapRemoteData.Count <= 0)
        {
            return;
        }

        GUILayout.Space(20f);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
        m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos);
        {
            foreach (int key in m_mapRemoteData.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("ID: " + m_mapRemoteData[key].ID, GUILayout.Width(100f));

                    EditorGUILayout.LabelField("名称: " + m_mapRemoteData[key].FileName);

                    if (m_mapLocalData.ContainsKey(key))
                    {
                        string localTime = TimeManager.Instance.CheckTime(m_mapLocalData[key].TimeStamp);
                        EditorGUILayout.LabelField("本地修改时间: " + localTime);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("本地文件不存在！");
                    }

                    string remoteTime = TimeManager.Instance.CheckTime(m_mapRemoteData[key].TimeStamp);
                    EditorGUILayout.LabelField("远程修改时间: " + remoteTime);

                    EditorGUILayout.LabelField("是否保留:", GUILayout.Width(60f));
                    m_mapCheckInfo[key] = EditorGUILayout.Toggle(m_mapCheckInfo[key], GUILayout.Width(20f));
                    GUILayout.Space(0f);

                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.Space(20f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {
                OnSave();
            }
            if (GUILayout.Button("取消", GUILayout.Width(100f)))
            {
                ClearData();
                m_Instance.Close();
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20f);
    }
    #endregion

    #region System Functions
    private void ClearData()
    {
        m_mapLocalData = null;
        m_mapCheckInfo = null;
        m_mapRemoteData = null;
    }
    private void OnSave()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "未勾选方案将被远程覆盖", "确定", "取消");
        if (option)
        {
            foreach (int key in m_mapCheckInfo.Keys)
            {
               if (m_mapCheckInfo[key])
               {
                   m_mapRemoteData.Remove(key);
               }
            }
            ActionHelper.MergeActionEditFileList(ActionHelper.GetActionEditFileList(), m_mapRemoteData);
            ClearData();
            m_Instance.Close();
        }
    }

    #endregion
}

