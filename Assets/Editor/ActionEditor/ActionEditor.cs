//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// NameSpace : Assets.Script.Framework.EditorRuntimeScript.ActionEditor
// FileName : ActionEditorWindow
//
// Created by : LeoLi (742412055@qq.com) at 2015/10/29 16:49:30
//
//
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ActionEditorWindow : EditorWindow
{
    #region Property
    static public ActionEditorWindow Instance
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
    #endregion

    #region Field
    private static ActionEditorWindow m_MainWnd;

    private bool m_bPlay = false;
    private float m_fAniTimeValue = 0f;
    private float m_fAniTimeLastValue = 0f;

    //readonly
    private readonly float ANIM_BAR_LENGTH = 1000f;
    private readonly float ANIM_PERFRAME_LENGTH =  0.1f;
    #endregion

    #region MonoBehavior
    [MenuItem("Editors/Action")]
    static void CreateWindow()
    {
        if (!CheckScene())
        {
            return;
        }
        m_MainWnd = EditorWindow.GetWindow<ActionEditorWindow>(false, "动画编辑器", true);
        m_MainWnd.Init();
    }


    #endregion

    public void OnGUI()
    {
        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("选择动画", GUILayout.Width(100f)))
            {

            }
            if (GUILayout.Button("创建动画", GUILayout.Width(100f)))
            {

            }

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("播放", GUILayout.Width(100f)))
            {

            }

            if (GUILayout.Button("暂停", GUILayout.Width(100f)))
            {

            }
            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {

            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15f);
        EditorGUILayout.LabelField("时间轴:");

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            float fTotalTime = GetSelectAnimLength();
            m_fAniTimeValue = EditorGUILayout.Slider(m_fAniTimeValue, 0, fTotalTime, GUILayout.Width(ANIM_BAR_LENGTH));
            if (!m_bPlay && m_fAniTimeValue != m_fAniTimeLastValue)
            {
                OnChangeAniTimeSlider(m_fAniTimeValue);
            }

            if (GUILayout.Button("<", GUILayout.Width(20f)))
            {
                m_fAniTimeValue -= ANIM_PERFRAME_LENGTH;
                if (m_fAniTimeValue < 0f)
                {
                    m_fAniTimeValue = 0f;
                }
                OnChangeAniTimeSlider(m_fAniTimeValue);
            }

            if (GUILayout.Button(">", GUILayout.Width(20f)))
            {
                m_fAniTimeValue += ANIM_PERFRAME_LENGTH;
                if (m_fAniTimeValue > fTotalTime)
                {
                    m_fAniTimeValue = fTotalTime;
                }
                OnChangeAniTimeSlider(m_fAniTimeValue);
            }
        }
        EditorGUILayout.EndHorizontal();

        m_fAniTimeLastValue = m_fAniTimeValue;
    }



    #region Public Interface

    #endregion

    #region System Function

    void Init()
    {
        //GameObject RootObj = GameObject.Find("TerrainEditorRoot");
        //m_ObjSceneRoot = GameObject.Find("SceneRoot");
        //m_ObjTriggerRoot = GameObject.Find("TriggerRoot");
        //var TriggerTemplateRoot = GameObject.Find("TriggerTemplateRoot");

        //if (null == RootObj || null == m_ObjSceneRoot || null == m_ObjTriggerRoot || null == TriggerTemplateRoot)
        //{
        //    Debug.LogError("wrong scene");
        //}

        m_MainWnd.minSize = new Vector2(1100,600);
        m_MainWnd.maxSize = new Vector2(1200,1200);
    }

    private static bool CheckScene()
    {
        var RootObj = GameObject.Find("TerrainEditorRoot");
        var a = GameObject.Find("SceneRoot");
        var b = GameObject.Find("TriggerRoot");
        var TriggerTemplateRoot = GameObject.Find("TriggerTemplateRoot");

        if (null == RootObj || null == a || null == b || null == TriggerTemplateRoot)
        {
            return false;
        }
        if (TerrainEditorMain.Instance == null)
        {
            return false;
        }
        return true;

    }

    private float GetSelectAnimLength()
    {
        return 20f;
    }

    private void OnChangeAniTimeSlider(float fValue)
    {
        //Debug.Log(fValue.ToString());
    }

    #endregion


}
