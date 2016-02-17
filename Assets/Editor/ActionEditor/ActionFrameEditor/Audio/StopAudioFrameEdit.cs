using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class StopAudioFrameEdit : AbstractFrameEdit
{
    static public StopAudioFrameEdit Instance
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


    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static StopAudioFrameEdit m_Instance;
    private StopAudioFrameConfig m_Config;


    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("声音文件:", GUILayout.Width(100f));
        m_Config.AudioSource = EditorGUILayout.TextField(m_Config.AudioSource);
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
        m_Instance = EditorWindow.GetWindow<StopAudioFrameEdit>(false, "停止声音编辑", true);
    }
    protected override void OnPlay()
    {
    }
    protected override void OnSave()
    {
        //Save Data

        m_ActionFrameData.StopAudioFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.StopAudioFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new StopAudioFrameConfig();
            m_Config.AudioSource = string.Empty;
        }
    }
}