//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_CreateEffectFrameEdit
//
// Created by : Baoxue at 2015/11/24 20:14:41
//
//
//========================================================================

using System.Diagnostics;
using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_CreateEffectFrameEdit : AbstractFrameEdit
{
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static Runtime_CreateEffectFrameEdit m_Instance;
    private Runtime_CreateEffectFrameConfig m_Config;
    private string m_strResourceName;
    private Vector3 m_vPos;
    private Vector3 m_vRot;
    private string m_instanceId;
    private Vector3 m_vLastPos;
    private Vector3 m_vLastRotation;
    private string[] m_TargetTypePopList;
    private int m_iSelectedTargetTypeId;

    static public Runtime_CreateEffectFrameEdit Instance
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
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
        Repaint();
    }
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("特效文件:", GUILayout.Width(50f));

            m_strResourceName = GUILayout.TextArea(m_strResourceName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("创建特效"))
            {
                CreateEffect();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.TextField("生成实例ID：" + m_instanceId);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            m_Config.TargetType = EditorGUILayout.Popup(m_Config.TargetType, m_TargetTypePopList, GUILayout.Width(100f));
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            Vector3 tmpPos = m_Config.Pos.GetVector3();
            EditorGUILayout.LabelField("特效位置:", GUILayout.Width(80f));

            GUILayout.Label("x", GUILayout.Width(20f));
            tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
            GUILayout.Label("y", GUILayout.Width(20f));
            tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
            GUILayout.Label("z", GUILayout.Width(20f));
            tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

            if (m_vLastPos != tmpPos)
            {
                m_Config.Pos.SetVector3(tmpPos);
            }

            m_vLastPos = tmpPos;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            Vector3 tmpRot = m_Config.Rot.GetVector3();
            EditorGUILayout.LabelField("特效方向:", GUILayout.Width(80f));

            GUILayout.Label("x", GUILayout.Width(20f));
            tmpRot.x = EditorGUILayout.FloatField(tmpRot.x);
            GUILayout.Label("y", GUILayout.Width(20f));
            tmpRot.y = EditorGUILayout.FloatField(tmpRot.y);
            GUILayout.Label("z", GUILayout.Width(20f));
            tmpRot.z = EditorGUILayout.FloatField(tmpRot.z);

            if (m_vLastRotation != tmpRot)
            {
                m_Config.Rot.SetVector3(tmpRot);
            }

            m_vLastRotation = tmpRot;
        }
        EditorGUILayout.EndHorizontal();
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
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<Runtime_CreateEffectFrameEdit>(false, "运行时生成特效编辑", true);
        
    }
    protected override void OnPlay()
    {
    }
    protected override void OnSave()
    {
        if (string.IsNullOrEmpty(m_instanceId))
        {
            EditorUtility.DisplayDialog("", "保存失败，instance id 不能为空", "ok");
            return;
        }
        m_Config.InstanceId = m_instanceId;
        m_Config.EffectName = m_strResourceName;
        
        //Save Data
        m_ActionFrameData.Runtime_CreateEffect = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.Runtime_CreateEffect;
            m_strResourceName = m_Config.EffectName;
            m_instanceId = m_Config.InstanceId;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
            m_strResourceName = string.Empty;
            m_instanceId = string.Empty;
            m_Config = new Runtime_CreateEffectFrameConfig();
            m_Config.EffectName = string.Empty;
            m_Config.Pos = new ThriftVector3();
            m_Config.Rot = new ThriftVector3();
            m_Config.InstanceId = string.Empty;
        }
        m_TargetTypePopList = new[] {"目标","使用者" };
    }
    private void CreateEffect()
    {
        //check name
        var source = ResourceManager.Instance.LoadBuildInResource<GameObject>(m_strResourceName, AssetType.Effect);
        if (null == source)
        {
            // log error
            EditorUtility.DisplayDialog("", "特效文件读取失败", "ok");
        }
        else
        {
            m_instanceId = EffectContainer.CreateInstanceId().ToString();
            Debuger.Log(m_instanceId);
        }
    }
}

