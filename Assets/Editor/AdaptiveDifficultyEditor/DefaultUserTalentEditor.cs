//========================================================================
// Copyright(C): CYTX
//
// FileName : DefaultUserTalentEditor
// 
// Created by : LeoLi at 2015/12/14 10:43:36
//
// Purpose : 
//========================================================================
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using TerrainEditor;
using AdaptiveDifficulty;

public class UserTalentInfo
{
    public string TalentName;
    public float TalentValue;
}

public class DefaultUserTalentEditor : EditorWindow
{
    static public DefaultUserTalentEditor Instance
    {
        get
        {
            if (null == m_MainWnd)
            {
                CreateWindow();
            }

            return m_MainWnd;
        }
    }

    //Read Only
    private readonly float WINDOW_MIN_WIDTH = 500f;
    private readonly float WINDOW_MIN_HIEGHT = 500f;
    //Editor Data
    private static DefaultUserTalentEditor m_MainWnd;
    private int m_UserTalentLastCount;
    private List<UserTalentInfo> m_lstUserTalents;
    //Action Data
    private DefaultUserTalent m_DefaultUserTalentData;
    private int m_UserTalentCount;

    #region MonoBehavior
    [MenuItem("Editors/AdaptiveDifficulty/玩家默认天赋")]
    private static void CreateWindow()
    {
        m_MainWnd = EditorWindow.GetWindow<DefaultUserTalentEditor>(false, "玩家默认天赋", true);
        m_MainWnd.Init();
    }
    private void Init()
    {
        m_MainWnd.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        LoadFile();
        Repaint();
    }
    private void OnGUI()
    {
        // Setting
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("重置", GUILayout.Width(60f)))
            {
                LoadFile();
                Repaint();
            }
            if (GUILayout.Button("保存", GUILayout.Width(60f)))
            {
                SaveFile();
                Repaint();
            }
            if (GUILayout.Button("同步", GUILayout.Width(60f)))
            {
                SysnFile();
                //LoadFile();
                Repaint();
            }
            if (GUILayout.Button("合并", GUILayout.Width(60f)))
            {
                MergeFile();
                //LoadFile();
                Repaint();
            }

        }
        EditorGUILayout.EndHorizontal();
        // Talent Count
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("天赋数量:", GUILayout.Width(60f));
            m_UserTalentCount = EditorGUILayout.IntField(m_UserTalentCount, GUILayout.Width(60f));
            GUILayout.Space(5f);
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(30f)))
            {
                m_UserTalentCount++;
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30f)))
            {
                if (m_UserTalentCount > 0)
                    m_UserTalentCount--;
            }
            //Init Data
            if (m_UserTalentCount < m_UserTalentLastCount && m_UserTalentCount >= 0)
            {
                int removeCount = m_UserTalentLastCount - m_UserTalentCount;
                for (int i = 0; i < removeCount; i++)
                {
                    m_lstUserTalents.RemoveAt(m_lstUserTalents.Count - 1);
                }
            }
            else if (m_UserTalentCount > m_UserTalentLastCount)
            {
                int extraCount = m_UserTalentCount - m_UserTalentLastCount;
                for (int i = 0; i < extraCount; i++)
                {
                    UserTalentInfo tmpData = new UserTalentInfo();
                    tmpData.TalentName = "";
                    tmpData.TalentValue = 0f;
                    m_lstUserTalents.Add(tmpData);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        // Talent Data
        GUILayout.Space(15f);
        if (m_lstUserTalents != null && m_lstUserTalents.Count > 0)
        {
            for (int i = 0; i < m_lstUserTalents.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("天赋名称： ", GUILayout.Width(60f));
                    m_lstUserTalents[i].TalentName = EditorGUILayout.TextField(m_lstUserTalents[i].TalentName, GUILayout.Width(60f));
                    EditorGUILayout.LabelField("默认值： ", GUILayout.Width(60f));
                    m_lstUserTalents[i].TalentValue = EditorGUILayout.Slider(m_lstUserTalents[i].TalentValue, 0f, 1f);
                    if (GUILayout.Button("X", GUILayout.Width(20f)))
                    {
                        var option = EditorUtility.DisplayDialog("确定要删除吗？",
                                                                 "确定吗？确定吗？确定吗？确定吗？确定吗？",
                                                                 "确定", "取消");
                        if (option)
                        {
                            m_lstUserTalents.RemoveAt(i);
                            m_UserTalentCount--;
                            Repaint();
                            break;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        m_UserTalentLastCount = m_UserTalentCount;
    }
    #endregion

    #region System Functions
    private void LoadFile()
    {
        //ADE data
        m_DefaultUserTalentData = ADE_Helper.GetDefaultUserTalentMap();
        if (m_DefaultUserTalentData == null || m_DefaultUserTalentData.MapTalent == null)
        {
            return;
        }
        Refresh();

    }
    private void SaveFile()
    {
        //Convert Data
        m_DefaultUserTalentData = new DefaultUserTalent();
        m_DefaultUserTalentData.MapTalent = new Dictionary<string, int>();
        for (int i = 0; i < m_UserTalentCount; i++)
        {
            string name = m_lstUserTalents[i].TalentName;
            int value = AdaptiveDifficultyManager.ConvertInt(m_lstUserTalents[i].TalentValue);
            m_DefaultUserTalentData.MapTalent.Add(name, value);
        }
        //Save File
        EditorUtility.DisplayDialog("保存成功", "保存成功", "确定");
        ADE_Helper.SaveDefaultUserTalentMap(m_DefaultUserTalentData);
    }
    private void SysnFile()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "同步方案将覆盖本地数据", "确定", "取消");
        if (option)
        {
            ADE_Helper.SycnDefaultUserTalentMap(ref m_DefaultUserTalentData);
            Refresh();
        }
    }
    private void MergeFile()
    {
        ADE_Helper.MergeDefaultUserTalentMap(ref m_DefaultUserTalentData);
        Refresh();
        EditorUtility.DisplayDialog("合并成功", "合并成功", "确定");
    }
    private void Refresh()
    {
        //Talent Count
        m_UserTalentCount = m_DefaultUserTalentData.MapTalent.Count;
        m_UserTalentLastCount = m_UserTalentCount;
        //Editor Data
        m_lstUserTalents = new List<UserTalentInfo>();
        foreach (string talentName in m_DefaultUserTalentData.MapTalent.Keys)
        {
            UserTalentInfo temp = new UserTalentInfo();
            temp.TalentName = talentName;
            temp.TalentValue = AdaptiveDifficultyManager.ConvertFloat(m_DefaultUserTalentData.MapTalent[talentName]);
            m_lstUserTalents.Add(temp);
        }
    }
    #endregion

    #region Public Interface

    #endregion

}


