//========================================================================
// Copyright(C): CYTX
//
// FileName : AnimCharFrameEdit
// 
// Created by : LeoLi at 2015/12/18 15:48:28
//
// Purpose : 
//========================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using TerrainEditor;
using System;
using Config;
using Config.Table;
using Common.Auto;

public class AnimCharFrameEdit : AbstractFrameEdit
{
    static public AnimCharFrameEdit Instance
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

    //readonly
    private readonly float WINDOW_MIN_WIDTH = 650f;
    private readonly float WINDOW_MIN_HIEGHT = 300f;
    private readonly string[] CHARTYPENAME = new string[] { "玩家", "NPC" };
    private static AnimCharFrameEdit m_Instance;
    //State
    private ECharType m_eCharType = ECharType.Npc;
    private int m_nAnimNum;
    private int m_nLastAnimNum = 0;
    //Data
    private AnimCharFrameConfig m_Config;
    private List<string> m_lstAnimName;

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("单位类型:", GUILayout.Width(80f));
            m_eCharType = (ECharType)EditorGUILayout.Popup((int)m_eCharType, CHARTYPENAME, GUILayout.Width(80f));
        }
        EditorGUILayout.EndHorizontal();
       
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("动画"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("动画数量:", GUILayout.Width(80f));
                GUILayout.Space(5f);
                m_nAnimNum = EditorGUILayout.IntField(m_nAnimNum, GUILayout.Width(50f));
                if (m_nAnimNum != m_nLastAnimNum)
                {
                    m_lstAnimName = new List<string>();
                    for (int i = 0; i < m_nAnimNum; i++)
                    {
                        m_lstAnimName.Add("");
                    }
                }
                EditorGUILayout.BeginVertical();
                {
                    if (m_lstAnimName != null && m_lstAnimName.Count > 0)
                    {
                        for (int i = 0; i < m_lstAnimName.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("动画" + (i + 1).ToString() + " : ", GUILayout.Width(80f));
                                m_lstAnimName[i] = EditorGUILayout.TextField(m_lstAnimName[i]);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        m_nLastAnimNum = m_nAnimNum;
    }
    private void OnDestroy()
    {
        ClearData();
    }
    #endregion

    #region Public Interface
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
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

    #region System Event
    protected void Init()
    {
        //Window Setting
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        //Update Info
        if (null != m_ActionFrameData)
        {
            //base
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.AnimcharFrame;
            //m_Config
            m_eCharType = m_Config.CharType;
            m_lstAnimName = m_Config.LstAnimName;
            //Anim Num
            m_nAnimNum = m_lstAnimName.Count;
            m_nLastAnimNum = m_nAnimNum;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new AnimCharFrameConfig();
            m_Config.LstAnimName = new List<string>();
        }

    }
    protected override void OnSave()
    {
        //Check Info
        if (m_Config == null )
        {
            EditorUtility.DisplayDialog("配置信息为空", "请补全表中信息", "ok");
            return;
        }

        //Save Data
        m_Config.CharType = m_eCharType;
        m_Config.LstAnimName = m_lstAnimName;
        m_ActionFrameData.AnimcharFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        ClearData();
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
        PlayerCharacter player = PlayerManager.Instance.GetPlayerInstance();
        if (player == null)
        {
            Debuger.LogWarning("No Exist Player !");
            return;
        }
        player.DirectPlayAnimation(m_Config.LstAnimName);
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<AnimCharFrameEdit>(false, "单位动画", true);
    }
    private void ClearData()
    {
        // clear config
        if (null != m_Config)
        {
            m_Config = new AnimCharFrameConfig();
            m_Config.LstAnimName = new List<string>();
        }
    }

    #endregion
}

