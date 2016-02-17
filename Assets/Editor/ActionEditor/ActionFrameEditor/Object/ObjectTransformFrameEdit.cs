//========================================================================
// Copyright(C): CYTX
//
// FileName : ObjectTransfromFrameEdit
// 
// Created by : LeoLi at 2016/1/22 18:00:49
//
// Purpose : 
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;
using Config;
using Common.Auto;
public class ObjectTransformFrameEdit : AbstractFrameEdit
{
    static public ObjectTransformFrameEdit Instance
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
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    //State
    private static ObjectTransformFrameEdit m_Instance;
    private GameObject m_ObjNpcRoot;
    //Data
    private ObjectTransformFrameConfig m_Config;
    private Vector3 m_LastTimePos;
    private Vector3 m_LastTimeRot;
    private GameObject m_CreatedNpcObject;

    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<ObjectTransformFrameEdit>(false, "物体瞬移", true);
    }

    // Events
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
            m_Config = m_ActionFrameData.ObjTransformFrame;
            AddNpc(20000001, m_Config.Pos.GetVector3(), m_Config.Rot.GetVector3(), m_Config.Scale.GetVector3());
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new ObjectTransformFrameConfig();
            m_Config.Pos = new ThriftVector3();
            m_Config.Rot = new ThriftVector3();
            m_Config.Scale = new ThriftVector3();
            AddNpc(20000001, Vector3.zero, Vector3.zero, Vector3.one);
        }
    }
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            if (m_CreatedNpcObject != null && m_Config != null)
            {
                if (GUILayout.Button("调整测试物体位置", GUILayout.Width(120f)))
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
        GUILayout.Space(10f);
        if (m_CreatedNpcObject != null && m_Config != null)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                Vector3 tmpPos = m_CreatedNpcObject.transform.position;
                EditorGUILayout.LabelField("物体位置:", GUILayout.Width(80f));

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
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                Vector3 tmpRot = m_CreatedNpcObject.transform.eulerAngles;
                EditorGUILayout.LabelField("物体朝向:", GUILayout.Width(80f));

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
    private void OnDestroy()
    {
        ClearData();
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
        m_ActionFrameData.ObjTransformFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        ClearData();
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
    }

    // Public Interface
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        ClearData();
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

    // System Method
    private void ClearData()
    {
        if (null != m_CreatedNpcObject)
        {
            GameObject.Destroy(m_CreatedNpcObject);
        }
        // clear config
        if (null != m_Config)
        {
            m_Config = new ObjectTransformFrameConfig();
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
}

