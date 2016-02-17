//========================================================================
// Copyright(C): CYTX
//
// FileName : EventControlEditorWindow
// 
// Created by : LeoLi at 2015/12/11 19:01:00
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

public class EventEffectInfo
{
    public string EventName;
    public Dictionary<string, float> TalentEffects;
}

public class EventControlEditorWindow : EditorWindow
{
    static public EventControlEditorWindow Instance
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
    private static EventControlEditorWindow m_MainWnd;
    private float m_fNodeSpace = 200f;
    private Vector2 m_EventScorllPos;
    //Editor Data
    private int m_GameID = 1;
    private int m_GameID_InputField = 1;
    private int m_EventLastCount;
    private List<EventEffectInfo> m_lstEventEffectInfo;
    //Action Data
    private EventControlDataMap m_EventControlDataMap;
    private EventControlData m_EventControlData;
    private int m_EventCount;

    #region MonoBehavior
    [MenuItem("Editors/AdaptiveDifficulty/成长控制器")]
    private static void CreateWindow()
    {
        m_MainWnd = EditorWindow.GetWindow<EventControlEditorWindow>(false, "成长控制器", true);
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
            if (GUILayout.Button(m_lstEventEffectInfo == null ? "生成" : "保存", GUILayout.Width(60f)))
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

        if (m_lstEventEffectInfo == null)
            return;

        #region Event Count
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("事件数量:", GUILayout.Width(60f));
            m_EventCount = EditorGUILayout.IntField(m_EventCount, GUILayout.Width(60f));
            GUILayout.Space(5f);
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(30f)))
            {
                m_EventCount++;
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30f)))
            {
                if (m_EventCount > 0)
                    m_EventCount--;
            }
            //Init Data
            if (m_EventCount < m_EventLastCount && m_EventCount >= 0)
            {
                int removeCount = m_EventLastCount - m_EventCount;
                for (int i = 0; i < removeCount; i++)
                {
                    m_lstEventEffectInfo.RemoveAt(m_lstEventEffectInfo.Count - 1);
                }
            }
            else if (m_EventCount > m_EventLastCount)
            {
                int extraCount = m_EventCount - m_EventLastCount;
                for (int i = 0; i < extraCount; i++)
                {
                    EventEffectInfo tmpData = new EventEffectInfo();
                    tmpData.EventName = "";
                    tmpData.TalentEffects = new Dictionary<string, float>();
                    DefaultUserTalent defaultUserTalent = ConfigManager.Instance.GetDefaultUserTalent();
                    foreach (string talentName in defaultUserTalent.MapTalent.Keys)
                    {
                        tmpData.TalentEffects.Add(talentName, 0f);
                    }
                    m_lstEventEffectInfo.Add(tmpData);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Event Data
        m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos);
        {
            // Data Name
            GUILayout.Space(15f);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("事件 \\ 天赋", GUILayout.Width(m_fNodeSpace));
                //GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width(1) });
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
            if (m_lstEventEffectInfo != null && m_lstEventEffectInfo.Count > 0)
            {
                for (int i = 0; i < m_lstEventEffectInfo.Count; i++)
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    {
                        m_lstEventEffectInfo[i].EventName = EditorGUILayout.TextField(m_lstEventEffectInfo[i].EventName, GUILayout.Width(m_fNodeSpace - 5f));
                        GUILayout.Space(5f);
                        // tmp list
                        List<UserTalentInfo> effectList = new List<UserTalentInfo>();
                        foreach (string talentName in m_lstEventEffectInfo[i].TalentEffects.Keys)
                        {
                            UserTalentInfo effect = new UserTalentInfo();
                            effect.TalentName = talentName;
                            effect.TalentValue = m_lstEventEffectInfo[i].TalentEffects[talentName];
                            effectList.Add(effect);
                        }
                        // update data
                        for (int num = 0; num < effectList.Count; num++)
                        {
                            effectList[num].TalentValue = EditorGUILayout.Slider(effectList[num].TalentValue, -1f, 1f, GUILayout.Width(m_fNodeSpace - 5f));
                            GUILayout.Space(5f);
                            m_lstEventEffectInfo[i].TalentEffects[effectList[num].TalentName] = effectList[num].TalentValue;
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

        m_EventLastCount = m_EventCount;
    }
    #endregion

    #region System Functions
    private void LoadFile()
    {
        //ADE data
        m_EventControlDataMap = ADE_Helper.GetEventControlDataMap();
        Refresh();

    }
    private void SaveFile()
    {
        //Convert Data
        m_EventControlData = new EventControlData();
        m_EventControlData.EventConfig = new Dictionary<string, EventConfig>();
        if (m_lstEventEffectInfo == null)
        {
            m_lstEventEffectInfo = new List<EventEffectInfo>();
        }
        for (int i = 0; i < m_lstEventEffectInfo.Count; i++)
        {
            if (m_lstEventEffectInfo[i].EventName == null || m_lstEventEffectInfo[i].EventName == "")
            {
                EditorUtility.DisplayDialog("保存失败", "事件名称不能为空", "确定");
                return;
            }
            EventConfig eventConfig = new EventConfig();
            eventConfig.TalentEffect = new Dictionary<string, int>();
            foreach (string talent in m_lstEventEffectInfo[i].TalentEffects.Keys)
            {
                eventConfig.TalentEffect.Add(talent, AdaptiveDifficultyManager.ConvertInt(m_lstEventEffectInfo[i].TalentEffects[talent]));
            }
            m_EventControlData.EventConfig.Add(m_lstEventEffectInfo[i].EventName, eventConfig);
        }
        //Save File
        EditorUtility.DisplayDialog("保存成功", "保存成功", "确定");
        ADE_Helper.SaveEventControlDataMap(m_EventControlDataMap, m_GameID, m_EventControlData);
    }
    private void SysnFile()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "同步方案将覆盖本地数据", "确定", "取消");
        if (option)
        {
            ADE_Helper.SycnEventControlDataMap(ref m_EventControlDataMap);
            Refresh();
        }
    }
    private void MergeFile()
    {
        ADE_Helper.MergeEventControlDataMap(ref m_EventControlDataMap);
        Refresh();
        EditorUtility.DisplayDialog("合并成功", "合并成功", "确定");
    }
    private void Refresh()
    {
        m_lstEventEffectInfo = null;
        // Check File
        if (m_EventControlDataMap == null || m_EventControlDataMap.MapFileData == null)
        {
            return;
        }
        if (m_EventControlDataMap.MapFileData.Count <= 0)
        {
            return;
        }
        // Game Talent
        if (!m_EventControlDataMap.MapFileData.ContainsKey(m_GameID))
        {
            m_EventCount = 0;
            m_EventLastCount = 0;
            return;
        }
        m_EventControlData = m_EventControlDataMap.MapFileData[m_GameID];
        // Event Count
        m_EventCount = m_EventControlData.EventConfig.Count;
        m_EventLastCount = m_EventCount;
        // Editor Data
        m_lstEventEffectInfo = new List<EventEffectInfo>();
        foreach (string eventName in m_EventControlData.EventConfig.Keys)
        {
            EventEffectInfo temp = new EventEffectInfo();
            temp.EventName = eventName;
            Dictionary<string, float> effect = new Dictionary<string, float>();
            DefaultUserTalent defaultUserTalent = ConfigManager.Instance.GetDefaultUserTalent();
            foreach (string talentName in defaultUserTalent.MapTalent.Keys)
            {
                if (m_EventControlData.EventConfig[eventName].TalentEffect.ContainsKey(talentName))
                {
                    float effectValue = AdaptiveDifficultyManager.ConvertFloat(m_EventControlData.EventConfig[eventName].TalentEffect[talentName]);
                    effect.Add(talentName, effectValue);
                }
                else
                {
                    effect.Add(talentName, 0f);
                }
            }
            temp.TalentEffects = effect;
            // add to list
            m_lstEventEffectInfo.Add(temp);
        }

    }
    #endregion

    #region Public Interface

    #endregion
}

