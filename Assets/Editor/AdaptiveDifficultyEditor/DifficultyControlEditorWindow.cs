//========================================================================
// Copyright(C): CYTX
//
// FileName : AdaptiveDifficultyEditorWindow
// 
// Created by : LeoLi at 2015/12/11 17:53:55
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

public class DifficultyInfo
{
    public string Name;
    public Dictionary<string, float> TalentEffects;
    public float Offset;
    public float Range;
}

public class DifficultyControlEditorWindow : EditorWindow
{
    static public DifficultyControlEditorWindow Instance
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
    private readonly float WINDOW_MIN_WIDTH = 1400f;
    private readonly float WINDOW_MIN_HIEGHT = 500f;
    //Editor Info
    private static DifficultyControlEditorWindow m_MainWnd;
    private float m_fNodeSpace = 200f;
    private Vector2 m_EventScorllPos;
    //Editor Data
    private int m_GameID = 1;
    private int m_GameID_InputField = 1;
    private int m_DifficultyLastCount;
    private List<DifficultyInfo> m_lstDifficultyInfo;
    //Action Data
    private DifficultyControlDataMap m_DifficultyControlDataMap;
    private DifficultyControlData m_DifficultyControlData;
    private int m_DifficultyCount;

    #region MonoBehavior
    [MenuItem("Editors/AdaptiveDifficulty/难度控制器")]
    private static void CreateWindow()
    {
        m_MainWnd = EditorWindow.GetWindow<DifficultyControlEditorWindow>(false, "难度控制器", true);
        m_MainWnd.Init();
    }
    private void Init()
    {
        // SetUp Window
        m_MainWnd.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        LoadFile();
        Repaint();
    }
    private void OnGUI()
    {
        #region Setting
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("重置", GUILayout.Width(60f)))
            {
                LoadFile();
                Repaint();
            }
            if (GUILayout.Button(m_lstDifficultyInfo == null ? "生成" : "保存", GUILayout.Width(60f)))
            {
                SaveFile();
                Repaint();
            }
            if (GUILayout.Button("同步", GUILayout.Width(60f)))
            {
                SysnFile();
                LoadFile();
                Repaint();
            }
            if (GUILayout.Button("合并", GUILayout.Width(60f)))
            {
                MergeFile();
                LoadFile();
                Repaint();
            }

        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Game ID
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("游戏ID : ", GUILayout.Width(60f));
            m_GameID_InputField = EditorGUILayout.IntField(m_GameID_InputField, GUILayout.Width(60f));
            if (GUILayout.Button("加载", GUILayout.Width(60f)))
            {
                m_GameID = m_GameID_InputField;
                //GUI.SetNextControlName("Input");
                //EditorUtility.DisplayDialog("加载成功", "加载成功", "确定");
                //GUI.FocusControl("Input");
                Refresh();
                this.Focus();
            }

            GUILayout.Space(15f);
            EditorGUILayout.LabelField("显示宽度 : ", GUILayout.Width(60f));
            m_fNodeSpace = EditorGUILayout.Slider(m_fNodeSpace, 100f, 300f, GUILayout.Width(200f));
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        if (m_lstDifficultyInfo == null)
            return;

        #region Difficulty Count
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("难度维度:", GUILayout.Width(60f));
            m_DifficultyCount = EditorGUILayout.IntField(m_DifficultyCount, GUILayout.Width(60f));
            GUILayout.Space(5f);
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(30f)))
            {
                m_DifficultyCount++;
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30f)))
            {
                if (m_DifficultyCount > 0)
                    m_DifficultyCount--;
            }
            //Init Data
            if (m_DifficultyCount < m_DifficultyLastCount && m_DifficultyCount >= 0)
            {
                int removeCount = m_DifficultyLastCount - m_DifficultyCount;
                for (int i = 0; i < removeCount; i++)
                {
                    m_lstDifficultyInfo.RemoveAt(m_lstDifficultyInfo.Count - 1);
                }
            }
            else if (m_DifficultyCount > m_DifficultyLastCount)
            {
                int extraCount = m_DifficultyCount - m_DifficultyLastCount;
                for (int i = 0; i < extraCount; i++)
                {
                    DifficultyInfo tmpData = new DifficultyInfo();
                    tmpData.Name = "";
                    tmpData.Offset = 0f;
                    tmpData.Range = 0f;
                    tmpData.TalentEffects = new Dictionary<string, float>();
                    DefaultUserTalent defaultUserTalent = ConfigManager.Instance.GetDefaultUserTalent();
                    foreach (string talentName in defaultUserTalent.MapTalent.Keys)
                    {
                        tmpData.TalentEffects.Add(talentName, 0f);
                    }
                    m_lstDifficultyInfo.Add(tmpData);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Difficuly Data
        m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos);
        {
            // Data Name
            GUILayout.Space(15f);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("难度 \\ 天赋", GUILayout.Width(m_fNodeSpace));
                //GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width(1) });
                // Offset & Range
                EditorGUILayout.LabelField("基础值", GUILayout.Width(m_fNodeSpace));
                EditorGUILayout.LabelField("浮动范围", GUILayout.Width(m_fNodeSpace));
                // Talent Effect
                DefaultUserTalent defaultUserTalent = ConfigManager.Instance.GetDefaultUserTalent();
                foreach (string talentName in defaultUserTalent.MapTalent.Keys)
                {
                    EditorGUILayout.LabelField(talentName, GUILayout.Width(m_fNodeSpace));
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw hor line
            // Data List
            if (m_lstDifficultyInfo != null && m_lstDifficultyInfo.Count > 0)
            {
                for (int i = 0; i < m_lstDifficultyInfo.Count; i++)
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    {
                        m_lstDifficultyInfo[i].Name = EditorGUILayout.TextField(m_lstDifficultyInfo[i].Name, GUILayout.Width(m_fNodeSpace - 5f));
                        GUILayout.Space(5f);
                        m_lstDifficultyInfo[i].Offset = EditorGUILayout.Slider(m_lstDifficultyInfo[i].Offset, 0f, 1f, GUILayout.Width(m_fNodeSpace - 5f));
                        GUILayout.Space(5f);
                        m_lstDifficultyInfo[i].Range = EditorGUILayout.Slider(m_lstDifficultyInfo[i].Range, 0f, 1f, GUILayout.Width(m_fNodeSpace - 5f));
                        GUILayout.Space(5f);
                        // tmp list
                        List<UserTalentInfo> effectList = new List<UserTalentInfo>();
                        foreach (string talentName in m_lstDifficultyInfo[i].TalentEffects.Keys)
                        {
                            UserTalentInfo effect = new UserTalentInfo();
                            effect.TalentName = talentName;
                            effect.TalentValue = m_lstDifficultyInfo[i].TalentEffects[talentName];
                            effectList.Add(effect);
                        }
                        // update data
                        for (int num = 0; num < effectList.Count; num++)
                        {
                            effectList[num].TalentValue = EditorGUILayout.Slider(effectList[num].TalentValue, -1f, 1f, GUILayout.Width(m_fNodeSpace - 5f));
                            GUILayout.Space(5f);
                            m_lstDifficultyInfo[i].TalentEffects[effectList[num].TalentName] = effectList[num].TalentValue;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5f);
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw hor line
                }
            }
        }
        EditorGUILayout.EndScrollView();
        #endregion

        m_DifficultyLastCount = m_DifficultyCount;
    }
    #endregion

    #region System Functions
    private void LoadFile()
    {
        //ADE data
        m_DifficultyControlDataMap = ADE_Helper.GetDifficultyControlDataMap();
        Refresh();
    }
    private void SaveFile()
    {
        // Save Data
        m_DifficultyControlData = new DifficultyControlData();
        m_DifficultyControlData.DifficultyConfig = new Dictionary<string, DifficultyConfig>();
        if (m_lstDifficultyInfo == null)
        {
            m_lstDifficultyInfo = new List<DifficultyInfo>();
        }
        for (int i = 0; i < m_lstDifficultyInfo.Count; i++)
        {
            if (m_lstDifficultyInfo[i].Name == null || m_lstDifficultyInfo[i].Name == "")
            {
                EditorUtility.DisplayDialog("保存失败", "难度名称不能为空", "确定");
                return;
            }
            DifficultyConfig diffConfig = new DifficultyConfig();
            diffConfig.DifficultyOffset = AdaptiveDifficultyManager.ConvertInt(m_lstDifficultyInfo[i].Offset);
            diffConfig.DifficultyRange = AdaptiveDifficultyManager.ConvertInt(m_lstDifficultyInfo[i].Range);
            diffConfig.TalentEffect = new Dictionary<string, int>();
            foreach (string talent in m_lstDifficultyInfo[i].TalentEffects.Keys)
            {
                diffConfig.TalentEffect.Add(talent, AdaptiveDifficultyManager.ConvertInt(m_lstDifficultyInfo[i].TalentEffects[talent]));
            }
            m_DifficultyControlData.DifficultyConfig.Add(m_lstDifficultyInfo[i].Name, diffConfig);
        }
        //Save File
        EditorUtility.DisplayDialog("保存成功", "保存成功", "确定");
        ADE_Helper.SaveDifficultyControlDataMap(m_DifficultyControlDataMap, m_GameID, m_DifficultyControlData);
    }
    private void SysnFile()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "同步方案将覆盖本地数据", "确定", "取消");
        if (option)
        {
            ADE_Helper.SycnDifficultyControlDataMap(ref m_DifficultyControlDataMap);
            Refresh();
        }
    }
    private void MergeFile()
    {
        ADE_Helper.MergeDifficultyControlDataMap(ref m_DifficultyControlDataMap);
        Refresh();
        EditorUtility.DisplayDialog("合并成功", "合并成功", "确定");
    }
    private void Refresh()
    {
        m_lstDifficultyInfo = null;
        // Check File
        if (m_DifficultyControlDataMap == null || m_DifficultyControlDataMap.MapFileData == null)
        {
            return;
        }
        if (m_DifficultyControlDataMap.MapFileData.Count <= 0)
        {
            return;
        }
        // Game Difficulty
        if (!m_DifficultyControlDataMap.MapFileData.ContainsKey(m_GameID))
        {
            m_DifficultyCount = 0;
            m_DifficultyLastCount = 0;
            return;
        }
        m_DifficultyControlData = m_DifficultyControlDataMap.MapFileData[m_GameID];
        // Difficulty Count
        m_DifficultyCount = m_DifficultyControlData.DifficultyConfig.Count;
        m_DifficultyLastCount = m_DifficultyCount;
        // Editor Data
        m_lstDifficultyInfo = new List<DifficultyInfo>();
        foreach (string diffName in m_DifficultyControlData.DifficultyConfig.Keys)
        {
            DifficultyInfo temp = new DifficultyInfo();
            temp.Name = diffName;
            temp.Offset = AdaptiveDifficultyManager.ConvertFloat(m_DifficultyControlData.DifficultyConfig[diffName].DifficultyOffset);
            temp.Range = AdaptiveDifficultyManager.ConvertFloat(m_DifficultyControlData.DifficultyConfig[diffName].DifficultyRange);
            Dictionary<string, float> effect = new Dictionary<string, float>();
            DefaultUserTalent defaultUserTalent = ConfigManager.Instance.GetDefaultUserTalent();
            foreach (string talentName in defaultUserTalent.MapTalent.Keys)
            {
                if (m_DifficultyControlData.DifficultyConfig[diffName].TalentEffect.ContainsKey(talentName))
                {
                    float effectValue = AdaptiveDifficultyManager.ConvertFloat(m_DifficultyControlData.DifficultyConfig[diffName].TalentEffect[talentName]);
                    effect.Add(talentName, effectValue);
                }
                else
                {
                    effect.Add(talentName, 0f);
                }
            }
            temp.TalentEffects = effect;
            // add to list
            m_lstDifficultyInfo.Add(temp);
        }
    }
    #endregion

    #region Public Interface

    #endregion

}

