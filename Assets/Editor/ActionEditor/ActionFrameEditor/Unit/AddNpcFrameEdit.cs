//========================================================================
// Copyright(C): CYTX
//
// FileName : AddNpcFrameEdit
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/19 16:26:36
//
// Purpose : 
//========================================================================

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using TerrainEditor;
using System;
using Config;
using Config.Table;
using Common.Auto;

public class AddNpcFrameEdit : AbstractFrameEdit
{
    public enum EAddNpcType
    {
        ByID = 0,
        ByObject = 1,
    }
    static public AddNpcFrameEdit Instance
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
    private readonly string[] NPCTYPENAME = new string[] { "添加ID", "拖入物体" };
    private static AddNpcFrameEdit m_Instance;
    //State
    private EAddNpcType m_eAddNpcType;
    private int m_iSelectedNpcId;
    private GameObject m_ObjNpcRoot;
    private string[] m_NpcTypeList;
    private Vector3 m_LastTimeRot;
    private Vector3 m_LastTimePos;
    //Data
    private AddNpcFrameConfig m_Config;
    private int m_InputNpcTempID;
    private GameObject m_CreatedNpcObject;      //ByID
    private GameObject m_InputNpcObject;        //ByObject 

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("添加方式 :", GUILayout.Width(80f));
            m_eAddNpcType = (EAddNpcType)EditorGUILayout.Popup((int)m_eAddNpcType, NPCTYPENAME, GUILayout.Width(80f));
        }
        EditorGUILayout.EndHorizontal();
        switch (m_eAddNpcType)
        {
            case EAddNpcType.ByID:
                DrawNpcByID();
                break;
            case EAddNpcType.ByObject:
                DrawNpcByObject();
                break;
        }
    }
    void DrawNpcByID()
    {
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("NPC配置"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                m_iSelectedNpcId = EditorGUILayout.Popup(m_iSelectedNpcId, m_NpcTypeList, GUILayout.Width(100f));
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("生成NPC临时ID: ", GUILayout.Width(90f));
                m_InputNpcTempID = EditorGUILayout.IntField(m_InputNpcTempID, GUILayout.Width(50f));
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                if (GUILayout.Button("生成NPC", GUILayout.Width(100f)))
                {
                    ClearData();
                    AddNpc(int.Parse(m_NpcTypeList[m_iSelectedNpcId]), Vector3.zero, Vector3.zero, Vector3.one);
                }
                if (m_CreatedNpcObject != null && m_Config != null)
                {
                    if (GUILayout.Button("调整NPC位置", GUILayout.Width(120f)))
                    {
                        ActionEditorRuntime.Instance.SetRaycastCallBack(SetNpcPos);
                    }
                    if (GUILayout.Button("X", GUILayout.Width(20f)))
                    {
                        ClearData();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (m_CreatedNpcObject != null && m_Config != null)
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpPos = m_CreatedNpcObject.transform.position;
                    EditorGUILayout.LabelField("NPC位置:", GUILayout.Width(80f));

                    GUILayout.Label("x", GUILayout.Width(20f));
                    tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                    GUILayout.Label("z", GUILayout.Width(20f));
                    tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                    if (m_LastTimePos != tmpPos)
                    {
                        m_CreatedNpcObject.transform.position = tmpPos;
                        m_Config.Pos.SetVector3(m_CreatedNpcObject.transform.position);
                    }

                    m_LastTimePos = tmpPos;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpRot = m_CreatedNpcObject.transform.eulerAngles;
                    EditorGUILayout.LabelField("NPC朝向:", GUILayout.Width(80f));

                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpRot.y = EditorGUILayout.Slider(tmpRot.y, 0f, 359.9f);


                    if (m_LastTimeRot != tmpRot)
                    {
                        m_CreatedNpcObject.transform.eulerAngles = tmpRot;
                        m_Config.Rot.SetVector3(m_CreatedNpcObject.transform.eulerAngles);
                    }

                    m_LastTimeRot = tmpRot;

                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    void DrawNpcByObject()
    {
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

        //SetUp Info
        m_ObjNpcRoot = GameObject.Find("NpcRoot");
        NpcConfigTable npcConfigTable = ConfigManager.Instance.GetNpcTable();
        if (null == npcConfigTable)
        {
            EditorUtility.DisplayDialog("", "npc配置文件读取失败", "ok");
            CloseWindow();
            return;
        }
        m_NpcTypeList = new string[npcConfigTable.NpcCofigMap.Count];
        int tmpIndex = 0;
        foreach (var elem in npcConfigTable.NpcCofigMap)
        {
            m_NpcTypeList[tmpIndex++] = elem.Key.ToString();
        }
        m_iSelectedNpcId = 0;

        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.AddNpcFrame;
            m_InputNpcTempID = m_ActionFrameData.AddNpcFrame.TempId;
            AddNpc(m_Config.Id, m_Config.Pos.GetVector3(), m_Config.Rot.GetVector3(), m_Config.Scale.GetVector3());
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new AddNpcFrameConfig();
            m_Config.Pos = new ThriftVector3();
            m_Config.Rot = new ThriftVector3();
            m_Config.Scale = new ThriftVector3();
        }
    }
    protected override void OnSave()
    {
        //Check Info
        if (m_Config == null || m_Config.Id <=0 || m_Config.Pos == null || m_Config.Rot == null || m_Config.Scale == null)
        {
            EditorUtility.DisplayDialog("配置信息为空", "请补全表中信息", "ok");
            return;
        }
        if (m_InputNpcTempID <= 0 || m_InputNpcTempID > 100000)
        {
            EditorUtility.DisplayDialog("NPC临时ID错误", "NPC临时ID范围：1至100000", "ok");
            return;
        }

        //Save Data
        m_ActionFrameData.AddNpcFrame = m_Config;
        m_ActionFrameData.AddNpcFrame.TempId = m_InputNpcTempID;
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
        m_Instance = EditorWindow.GetWindow<AddNpcFrameEdit>(false, "创建NPC", true);
    }
    void ClearData()
    {
        //clear npc
        if (null != m_CreatedNpcObject)
        {
            GameObject.Destroy(m_CreatedNpcObject);
        }
        if (null != m_Config)
        {
            m_Config = new AddNpcFrameConfig();
            m_Config.Pos = new ThriftVector3();
            m_Config.Rot = new ThriftVector3();
            m_Config.Scale = new ThriftVector3();
        }

    }
    private void AddNpc(int id, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        NpcConfig tmpConfig = ConfigManager.Instance.GetNpcConfig(id);
        if (null == tmpConfig)
        {
            EditorUtility.DisplayDialog("id 错误", "请检查表中npc id ，错误 id= " + id, "ok");
            return;
        }

        GameObject sourceObj = ResourceManager.Instance.LoadBuildInResource<GameObject>(tmpConfig.ModelResource,
            AssetType.Char);
        if (null == sourceObj)
        {
            EditorUtility.DisplayDialog("模型 id 错误", "请检查表中npc id ，错误 id= " + tmpConfig.ModelResource, "ok");
            return;
        }
        GameObject instance = GameObject.Instantiate(sourceObj);
        ComponentTool.Attach(m_ObjNpcRoot.transform, instance.transform);


        m_Config.Id = id;
        m_Config.Pos.SetVector3(position);
        m_Config.Rot.SetVector3(rotation);
        m_Config.Scale.SetVector3(scale);

        m_CreatedNpcObject = instance;
        m_CreatedNpcObject.transform.position = position;
        m_CreatedNpcObject.transform.eulerAngles = rotation;
        m_CreatedNpcObject.transform.localScale = scale;

        //disable npc anim
        Animator anim = m_CreatedNpcObject.GetComponent<Animator>();
        if (null != anim)
        {
            anim.applyRootMotion = false;
        }
    }
    private void SetNpcPos(Vector3 positin)
    {
        m_Config.Pos.SetVector3(positin);
        m_CreatedNpcObject.transform.position = positin;
    }
    #endregion
}

