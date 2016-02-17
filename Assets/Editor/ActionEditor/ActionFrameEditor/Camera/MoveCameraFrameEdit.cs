//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrameEdit
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 16:46:19
//
// Purpose : 摄像机移动 编辑窗口
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using System;

public class MoveCameraFrameEdit : AbstractFrameEdit
{
    static public MoveCameraFrameEdit Instance
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
    private readonly string[] MOVETYPE = new string[] { "锁定目标移动", "非锁定目标移动" };
    private readonly string[] CHARTYPENAME = new string[] { "NPC", "玩家" };

    private static MoveCameraFrameEdit m_Instance;

    private MoveCameraFrameConfig m_Config;
    //private float m_fTickTime;
    private Vector3 m_CamPos = new Vector3(0, 0, 0);
    private Vector3 m_CamEuler = new Vector3(0, 0, 0);
    private Vector3 m_LastTimePos;
    private Vector3 m_LastTimeEuler;

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("移动方式 :", GUILayout.Width(80f));
            m_Config.MoveType = (EMoveCameraType)EditorGUILayout.Popup((int)m_Config.MoveType, MOVETYPE, GUILayout.Width(150f));
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            m_Config.IsImmediate = EditorGUILayout.Toggle("瞬间移动", m_Config.IsImmediate);
            GUILayout.Space(5f);
            m_Config.IsOverObstacle = EditorGUILayout.Toggle("摄像机躲避障碍", m_Config.IsOverObstacle);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(15f);
        if (m_Config.MoveType == EMoveCameraType.Lock)
        {
            DrawLock();
        }
        else if (m_Config.MoveType == EMoveCameraType.UnLock)
        {
            DrawUnlock();
        }
        GUILayout.Space(5f);
        m_Config.PosDamping = EditorGUILayout.FloatField("位移速度:", (float)m_Config.PosDamping);
        GUILayout.Space(5f);
        m_Config.RotDamping = EditorGUILayout.FloatField("旋转速度:", (float)m_Config.RotDamping);
    }

    private void DrawLock()
    {
        GUILayout.Space(5f);
        m_Config.FollowType = (ECameraFollowType)EditorGUILayout.Popup((int)m_Config.FollowType, CHARTYPENAME, GUILayout.Width(80f));
        GUILayout.Space(5f);
        m_Config.Distance = EditorGUILayout.FloatField("相机距离:", (float)m_Config.Distance);
        GUILayout.Space(5f);
        m_Config.Height = EditorGUILayout.FloatField("相机高度:", (float)m_Config.Height);
        GUILayout.Space(5f);
        m_Config.OffseHeight = EditorGUILayout.FloatField("焦点高度:", (float)m_Config.OffseHeight);
        GUILayout.Space(5f);
        m_Config.Rotation = EditorGUILayout.FloatField("旋转角度:", (float)m_Config.Rotation);
    }
    private void DrawUnlock()
    {
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("相机位置:", GUILayout.Width(80f));

            GUILayout.Label("x", GUILayout.Width(20f));
            m_CamPos.x = EditorGUILayout.FloatField(m_CamPos.x);
            GUILayout.Label("y", GUILayout.Width(20f));
            m_CamPos.y = EditorGUILayout.FloatField(m_CamPos.y);
            GUILayout.Label("z", GUILayout.Width(20f));
            m_CamPos.z = EditorGUILayout.FloatField(m_CamPos.z);

            if (GUILayout.Button("获取当前相机位置", GUILayout.Width(140f)))
            {
                GameObject tmpObj = GameObject.Find("MainCamera");
                if (null != tmpObj)
                {
                    m_CamPos = tmpObj.transform.position;
                }
            }

            if (m_LastTimePos != m_CamPos)
            {
                m_Config.EndPos.SetVector3(m_CamPos);
            }

            m_LastTimePos = m_CamPos;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField("相机方向:", GUILayout.Width(80f));

            GUILayout.Label("x", GUILayout.Width(20f));
            m_CamEuler.x = EditorGUILayout.FloatField(m_CamEuler.x);
            GUILayout.Label("y", GUILayout.Width(20f));
            m_CamEuler.y = EditorGUILayout.FloatField(m_CamEuler.y);
            GUILayout.Label("z", GUILayout.Width(20f));
            m_CamEuler.z = EditorGUILayout.FloatField(m_CamEuler.z);

            if (GUILayout.Button("获取当前相机方向", GUILayout.Width(140f)))
            {
                GameObject tmpObj = GameObject.Find("MainCamera");
                if (null != tmpObj)
                {
                    m_CamEuler = tmpObj.transform.eulerAngles;
                }
            }

            if (m_LastTimeEuler != m_CamEuler)
            {
                m_Config.EndRot.SetVector3(m_CamEuler);
            }

            m_LastTimeEuler = m_CamEuler;
        }
        EditorGUILayout.EndHorizontal();
    }
    void Update()
    {
        //float fCurrtime = TimeManager.Instance.GetTime();
        //switch (m_eMoveCameraState)
        //{
        //    case MoveCameraFrame.EMoveCameraState.MoveTo:
        //        if (fCurrtime >= m_fTickTime)
        //        {
        //            m_fTickTime = fCurrtime + (float)m_Config.StayTime;
        //            m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.Stay;
        //        }
        //        break;
        //    case MoveCameraFrame.EMoveCameraState.Stay:
        //        if (fCurrtime >= m_fTickTime)
        //        {
        //            m_fTickTime = fCurrtime + (float)m_Config.MoveBackTime;
        //            m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveBack;

        //            GlobalScripts.Instance.mGameCamera.ResetCam();
        //        }
        //        break;
        //    case MoveCameraFrame.EMoveCameraState.MoveBack:
        //        if (fCurrtime >= m_fTickTime)
        //        {
        //            m_fTickTime = 0f;
        //            m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.None;
        //        }
        //        break;
        //}
    }
    void OnDestroy()
    {

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
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.MoveCameraFrame;
            if (null == m_Config.EndPos)
            {
                m_Config.EndPos = new Common.Auto.ThriftVector3();
            }
            if (null == m_Config.EndRot)
            {
                m_Config.EndRot = new Common.Auto.ThriftVector3();
            }
            m_CamPos = m_Config.EndPos.GetVector3();
            m_CamEuler = m_Config.EndRot.GetVector3();
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new MoveCameraFrameConfig();

            m_Config.MoveType = EMoveCameraType.Lock;
            m_Config.IsImmediate = false;
            m_Config = new MoveCameraFrameConfig();
            m_Config.Distance = GlobalScripts.Instance.mGameCamera.Distance;
            m_Config.Height = GlobalScripts.Instance.mGameCamera.Height;
            m_Config.OffseHeight = GlobalScripts.Instance.mGameCamera.OffsetHeight;
            m_Config.Rotation = GlobalScripts.Instance.mGameCamera.Rotation;
            m_Config.PosDamping = GlobalScripts.Instance.mGameCamera.PositonDamping;
            m_Config.RotDamping = GlobalScripts.Instance.mGameCamera.RotationDamping;
            m_Config.EndPos = new Common.Auto.ThriftVector3();
            m_Config.EndRot = new Common.Auto.ThriftVector3();
            m_Config.IsOverObstacle = GlobalScripts.Instance.mGameCamera.IsOverObstacle;
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
    }
    protected override void OnSave()
    {
        m_ActionFrameData.MoveCameraFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
        //GlobalScripts.Instance.mGameCamera.ResetCam();

        //float fDetalDistance = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fDistance - (float)m_Config.Distance);
        //float fDistanceDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalDistance / (float)m_Config.MoveToTime;
        //GlobalScripts.Instance.mGameCamera.m_fDistance = (float)m_Config.Distance;
        //GlobalScripts.Instance.mGameCamera.m_fDistanceDamping = fDistanceDamping;

        //float fDetalOffsetHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fOffsetHeight - (float)m_Config.OffseHeight);
        //float fOffsetHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalOffsetHeight / (float)m_Config.MoveToTime;
        //GlobalScripts.Instance.mGameCamera.m_fOffsetHeight = (float)m_Config.OffseHeight;
        //GlobalScripts.Instance.mGameCamera.m_fOffsetHeightDamping = fOffsetHeightDamping;

        //float fDetalHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fHeight - (float)m_Config.Height);
        //float fHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalHeight / (float)m_Config.MoveToTime;
        //GlobalScripts.Instance.mGameCamera.m_fHeight = (float)m_Config.Height;
        //GlobalScripts.Instance.mGameCamera.m_fHeightDamping = fHeightDamping;

        //m_fTickTime = TimeManager.Instance.GetTime() + (float)m_Config.MoveToTime;
        //m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveTo;

        GameObject tmpObj = GameObject.Find("MainCamera");
        if (null != tmpObj)
        {
            tmpObj.transform.position = m_CamPos;
            tmpObj.transform.eulerAngles = m_CamEuler;
        }
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<MoveCameraFrameEdit>(false, "摄像机移动", true);
    }
    #endregion
}

