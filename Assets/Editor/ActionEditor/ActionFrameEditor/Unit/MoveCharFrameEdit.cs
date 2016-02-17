//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCharFrameEdit
// 
// Created by : LeoLi at 2015/11/27 19:37:24
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

public class MoveCharFrameEdit : AbstractFrameEdit
{
    static public MoveCharFrameEdit Instance
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
    private static MoveCharFrameEdit m_Instance;
    //State
    private ECharType m_eCharType = ECharType.Npc;
    private int m_nTargetNum = 0;
    private int m_nTargetLastNum = 0;
    private int m_nCurrentSettingPosTriggerIndex;
    private List<Vector3> m_lstLastTimePos;
    private GameObject m_HintPointSource;
    private List<GameObject> m_lstHintPointList;
    //Data
    private MoveCharFrameConfig m_Config;
    private List<CharSpeedMove> m_lstSpeedMoveList;

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
        DrawCharMove();
    }
    private void DrawCharMove()
    {
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("路径节点"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("节点数量:", GUILayout.Width(80f));
                GUILayout.Space(5f);
                m_nTargetNum = EditorGUILayout.IntField(m_nTargetNum, GUILayout.Width(50f));
                GUILayout.Space(15f);
                if (m_nTargetNum != m_nTargetLastNum)
                {
                    m_lstSpeedMoveList = new List<CharSpeedMove>();
                    m_lstLastTimePos = new List<Vector3>();
                    ClearHintPoint();
                    m_lstHintPointList = new List<GameObject>();
                    for (int i = 0; i < m_nTargetNum; i++)
                    {
                        // data
                        ThriftVector3 pos = new ThriftVector3();
                        pos.SetVector3(new Vector3(0, 0, 0));
                        CharSpeedMove point = new CharSpeedMove();
                        point.Target = pos;
                        point.Speed = 5;
                        m_lstSpeedMoveList.Add(point);
                        // state
                        m_lstLastTimePos.Add(new Vector3(0, 0, 0));
                        GameObject instance = GameObject.Instantiate(m_HintPointSource);
                        GameObject root = GameObject.Find("Canvas");
                        if (root)
                        {
                            ComponentTool.Attach(root.transform, instance.transform);
                            Text txtComp = instance.GetComponentInChildren<Text>();
                            if (txtComp)
                            {
                                txtComp.text = (i+1).ToString();
                            }
                        }
                        m_lstHintPointList.Add(instance);
                    }
                }
                EditorGUILayout.BeginVertical();
                {
                    if (m_lstSpeedMoveList != null && m_lstSpeedMoveList.Count > 0)
                    {
                        for (int i = 0; i < m_lstSpeedMoveList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("节点 " + (i + 1).ToString() + " : ", GUILayout.Width(80f));
                                Vector3 tmpPos = new Vector3(0, 0, 0);
                                if (m_lstSpeedMoveList[i].Target != null)
                                {
                                    tmpPos = m_lstSpeedMoveList[i].Target.GetVector3();
                                }

                                GUILayout.Label("x", GUILayout.Width(20f));
                                tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                                GUILayout.Label("y", GUILayout.Width(20f));
                                tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                                GUILayout.Label("z", GUILayout.Width(20f));
                                tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                                if (m_lstLastTimePos[i] != tmpPos)
                                {
                                    m_lstSpeedMoveList[i].Target.SetVector3(tmpPos);
                                    //m_Config.LstTargets = m_lstTargetList;
                                }
                                GUILayout.Space(5f);
                                if (GUILayout.Button("调整位置", GUILayout.Width(100f)))
                                {
                                    m_nCurrentSettingPosTriggerIndex = i;
                                    ActionEditorRuntime.Instance.SetRaycastCallBack(SetTargetPos);
                                }
                                m_lstLastTimePos[i] = tmpPos;
                                GUILayout.Space(5f);
                                m_lstSpeedMoveList[i].Speed = EditorGUILayout.FloatField("速度:", (float)m_lstSpeedMoveList[i].Speed);

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        m_nTargetLastNum = m_nTargetNum;
    }
    private void OnDestroy()
    {
        ClearData();
    }
    #endregion

    #region Public Interface
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        ClearHintPoint();
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
            m_Config = m_ActionFrameData.MovecharFrame;
            //m_Config
            m_eCharType = m_Config.CharType;
            m_lstSpeedMoveList = m_Config.LstSpeedMove;
            //TargetNum
            m_nTargetNum = m_lstSpeedMoveList.Count;
            m_nTargetLastNum = m_nTargetNum;
            //LastTimePos
            m_lstLastTimePos = new List<Vector3>();
            for (int i = 0; i < m_nTargetNum; i++)
            {
                if (m_lstSpeedMoveList[i].Target != null)
                {
                    m_lstLastTimePos.Add(m_lstSpeedMoveList[i].Target.GetVector3());
                }
                else
                {
                    m_lstLastTimePos.Add(new Vector3(0, 0, 0));
                }
            }
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new MoveCharFrameConfig();
            m_Config.LstSpeedMove = new List<CharSpeedMove>();
        }
        // Hint Point Source
        var TriggerTemplateRoot = GameObject.Find("TriggerTemplateRoot");
        m_HintPointSource = TriggerTemplateRoot.transform.GetChild(0).gameObject;
        if (m_HintPointSource == null)
        {
            Debuger.LogError("m_HintPointSource not found");
        }
        // Add Hint Points
        m_lstHintPointList = new List<GameObject>();
        if (m_lstLastTimePos != null && m_lstLastTimePos.Count > 0)
        {
            for (int i = 0; i < m_lstLastTimePos.Count; ++i)
            {
                AddHintPoint(m_lstLastTimePos[i],i);
            }
        }

    }
    protected override void OnSave()
    {
        //Check Info
        if (m_Config == null || m_lstSpeedMoveList == null || m_lstSpeedMoveList.Count <= 0)
        {
            EditorUtility.DisplayDialog("配置信息为空", "请补全表中信息", "ok");
            return;
        }

        //Save Data
        m_Config.CharType = m_eCharType;
        m_Config.LstSpeedMove = m_lstSpeedMoveList;
        m_ActionFrameData.MovecharFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        ClearData();
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<MoveCharFrameEdit>(false, "移动单位", true);
    }
    private void ClearData()
    {
        // clear config
        if (null != m_Config)
        {
            m_Config = new MoveCharFrameConfig();
            m_Config.LstSpeedMove = new List<CharSpeedMove>();
        }
        // clear TargetList
        if (null != m_lstSpeedMoveList)
        {
            m_lstSpeedMoveList = new List<CharSpeedMove>();
        }
        ClearHintPoint();
    }

    private void ClearHintPoint()
    {
        if (m_lstHintPointList == null || m_lstHintPointList.Count <= 0)
            return;
        // clear hint points
        foreach (GameObject obj in m_lstHintPointList)
        {
            Destroy(obj);
        }
        m_lstHintPointList = null;
    }
    private void SetTargetPos(Vector3 positin)
    {
        if (m_nCurrentSettingPosTriggerIndex < 0 || m_nCurrentSettingPosTriggerIndex >= m_lstSpeedMoveList.Count)
        {
            return;
        }
        m_lstSpeedMoveList[m_nCurrentSettingPosTriggerIndex].Target.SetVector3(positin);
        // update hint point
        if (m_lstHintPointList != null && m_lstHintPointList.Count >= m_nCurrentSettingPosTriggerIndex)
        {
            m_lstHintPointList[m_nCurrentSettingPosTriggerIndex].gameObject.transform.position = positin;
        }
    }
    private void AddHintPoint(Vector3 position,int num)
    {
        if (m_HintPointSource == null)
            return;

        GameObject instance = GameObject.Instantiate(m_HintPointSource);
        GameObject root = GameObject.Find("Canvas");
        if (root)
        {
            ComponentTool.Attach(root.transform, instance.transform);
            Text txtComp = instance.GetComponentInChildren<Text>();
            if (txtComp)
            {
                txtComp.text = (num+1).ToString();
            }
        }
        instance.transform.position = position;
        m_lstHintPointList.Add(instance);
    }
    #endregion
}

