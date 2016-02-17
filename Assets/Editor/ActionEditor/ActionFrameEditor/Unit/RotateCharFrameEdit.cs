//========================================================================
// Copyright(C): CYTX
//
// FileName : RotateCharFrameEdit
// 
// Created by : LeoLi at 2016/1/16 15:09:58
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
public class RotateCharFrameEdit : AbstractFrameEdit
{
    static public RotateCharFrameEdit Instance
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
    //State
    private static RotateCharFrameEdit m_Instance;
    private ECharType m_eCharType = ECharType.Npc;
    private GameObject m_CreatedNpcObject;
    private GameObject m_ObjNpcRoot;
    private float m_fLastTimeEuler;
    //Data
    private RotateCharFrameConfig m_Config;

    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<RotateCharFrameEdit>(false, "旋转单位", true);
    }

    private void OnGUI()
    {
        DrawBaseInfo();
        // State 
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            if (m_CreatedNpcObject != null && m_Config != null)
            {
                if (GUILayout.Button("调整物体位置", GUILayout.Width(120f)))
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
        // Data
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("单位类型:", GUILayout.Width(80f));
            m_eCharType = (ECharType)EditorGUILayout.Popup((int)m_eCharType, CHARTYPENAME, GUILayout.Width(80f));
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("旋转角度:", GUILayout.Width(80f));
            m_Config.Rotation = EditorGUILayout.Slider((float)m_Config.Rotation, 0f, 359.9f);
            if (m_CreatedNpcObject != null)
            {
                Vector3 euler = m_CreatedNpcObject.transform.eulerAngles;
                euler.y = (float)m_Config.Rotation;
                if (m_fLastTimeEuler != euler.y)
                {
                    m_CreatedNpcObject.transform.eulerAngles = euler;
                }
                m_fLastTimeEuler = euler.y;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    private void OnDestroy()
    {
        ClearData();
    }
    private void Init()
    {
        //Window Setting
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        m_ObjNpcRoot = GameObject.Find("NpcRoot");
        //Update Info
        if (null != m_ActionFrameData)
        {
            //base
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.RotcharFrame;
            //m_Config
            m_eCharType = m_Config.CharType;
            AddNpc(20000001, Vector3.zero, new Vector3(0, (float)m_Config.Rotation, 0), Vector3.one);
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new RotateCharFrameConfig();
            AddNpc(20000001, Vector3.zero, Vector3.zero, Vector3.one);
        }
    }
    protected override void OnSave()
    {
        //Check Info
        if (m_Config == null)
        {
            EditorUtility.DisplayDialog("配置信息为空", "请补全表中信息", "ok");
            return;
        }

        //Save Data
        m_Config.CharType = m_eCharType;
        m_ActionFrameData.RotcharFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        ClearData();
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
    }

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

    #region System Method
    private void ClearData()
    {
        if (null != m_CreatedNpcObject)
        {
            GameObject.Destroy(m_CreatedNpcObject);
        }
        // clear config
        if (null != m_Config)
        {
            m_Config = new RotateCharFrameConfig();
        }
    }
    private void AddNpc(int id, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        NpcConfig tmpConfig = ConfigManager.Instance.GetNpcConfig(id);
        if (null == tmpConfig)
        {
            return;
        }

        GameObject sourceObj = ResourceManager.Instance.LoadBuildInResource<GameObject>(tmpConfig.ModelResource,
            AssetType.Char);
        if (null == sourceObj)
        {
            return;
        }
        GameObject instance = GameObject.Instantiate(sourceObj);
        ComponentTool.Attach(m_ObjNpcRoot.transform, instance.transform);

        m_Config.Rotation = rotation.y;

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
        m_CreatedNpcObject.transform.position = positin;
    }
    #endregion
}

