//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/2 19:17:12
//
// Purpose : 
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using ActionEditor;

public class ActionPlayer
{
    public enum EActionState
    {
        Play,
        Pause,
        Stop,
        Finish,
    }

    #region Field
    //Action State
    private int m_nInstanceID;
    private EActionState m_eActionState = EActionState.Play;
    private float m_fStartTime;
    private float m_fRunTime;
    //Action Data
    private int m_ActionId;
    private ActionFileData m_ActionFileData;
    private List<AbstractActionFrame> m_lstActionFrames = new List<AbstractActionFrame>();
    private List<GameObject> m_lstAffectedObjects = new List<GameObject>();
    private List<GameObject> m_lstGeneratedObjects = new List<GameObject>();
    private Dictionary<int, GameObject> m_mapTargetObjects = new Dictionary<int, GameObject>();
    #endregion

    public ActionPlayer(int iActionId, ActionFileData data, List<GameObject> affectedObjects = null)
    {
        if (null == data)
        {
            return;
        }
        m_ActionId = iActionId;
        m_ActionFileData = data;
        m_nInstanceID = AllocInstanceId();
        m_lstAffectedObjects = affectedObjects;
        foreach (GameObject obj in m_lstAffectedObjects)
        {
            TransformContainer objContainer = obj.GetComponent<TransformContainer>();
            if (objContainer == null)
            {
                Debuger.LogWarning("Instance Container Not Found in " + obj.name);
                continue;
            }
            else
            {
                InsertTargetObjects(obj, objContainer);
            }
        }

        Reset();
    }
    private static int AllocInstanceId()
    {
        return ++g_ActionInstanceID;
    }

    private static int g_ActionInstanceID = 0;

    #region Get
    public bool IsFinish()
    {
        if (GetActionRunTime() >= GetActionTotalTime())
        {
            return true;
        }

        return false;
    }
    public int GetInstanceID()
    {
        return m_nInstanceID;
    }
    public EActionState GetActionState()
    {
        return m_eActionState;
    }
    public float GetActionTotalTime()
    {
        if (null == m_ActionFileData)
        {
            return 0f;
        }
        return (float)m_ActionFileData.Duration;
    }
    public float GetActionRunTime()
    {
        return m_fRunTime;
    }

    //public List<GameObject> GetAffectedObjects()
    //{
    //    return m_lstAffectedObjects;
    //}
    //public List<GameObject> GetGeneratedObjects()
    //{
    //    return m_lstGeneratedObjects;
    //}
    public Dictionary<int, GameObject> GetTargetObjects()
    {
        return m_mapTargetObjects;
    }
    #endregion

    #region Set
    public void SetActionRunTime(float time)
    {
        m_fRunTime = time;
        m_fStartTime -= time;
    }
    public void InsertGeneratedObjects(List<GameObject> generatedObjects)
    {
        m_lstGeneratedObjects.AddRange(generatedObjects);

        foreach (GameObject obj in generatedObjects)
        {
            TransformContainer objContainer = obj.GetComponent<TransformContainer>();
            if (objContainer == null)
            {
                Debuger.LogWarning("Generated Container Not Found in " + obj.name);
                continue;
            }
            else
            {
                InsertTargetObjects(obj, objContainer);
            }
        }
    }
    public void InsertGeneratedObjects(params GameObject[] generatedObjects)
    {
        m_lstGeneratedObjects.AddRange(generatedObjects);

        foreach (GameObject obj in generatedObjects)
        {
            TransformContainer objContainer = obj.GetComponent<TransformContainer>();
            if (objContainer == null)
            {
                Debuger.LogWarning("Generated Container Not Found in " + obj.name);
                continue;
            }
            else
            {
                InsertTargetObjects(obj, objContainer);
            }
        }
    }
    #endregion

    #region Public Interface
    public void Reset()
    {
        m_fStartTime = TimeManager.Instance.GetTime();
        m_fRunTime = 0f;
        m_eActionState = EActionState.Play;

        m_lstActionFrames.Clear();
        if (m_ActionFileData != null && m_ActionFileData.FrameDatalist != null)
        {
            for (int iIndex = m_ActionFileData.FrameDatalist.Count - 1; iIndex >= 0; --iIndex)
            {
                AbstractActionFrame skillFrame = ActionFrameFactory.CreateActionFrame(this, m_ActionFileData.FrameDatalist[iIndex]);
                m_lstActionFrames.Add(skillFrame);
            }
        }
    }
    public void Destory()
    {
        if (null != m_lstActionFrames)
        {
            foreach (AbstractActionFrame frame in m_lstActionFrames)
            {
                if (null == frame)
                {
                    continue;
                }

                frame.Destory();
            }

            m_lstActionFrames.Clear();
        }
    }
    public void Pause()
    {

    }
    public void Stop()
    {
        if (null != m_lstActionFrames)
        {
            foreach (AbstractActionFrame frame in m_lstActionFrames)
            {
                if (null == frame)
                {
                    continue;
                }

                frame.Stop();
            }
        }

        m_eActionState = EActionState.Stop;
        //Destory();
    }
    public void Update()
    {
        Process();
        if (null == m_lstActionFrames || m_lstActionFrames.Count <= 0)
        {
            return;
        }

        float fRealTime = m_fRunTime;

        int nCount = m_lstActionFrames.Count;


        for (int i = nCount - 1; i >= 0; i--)
        {
            AbstractActionFrame skillFrame = m_lstActionFrames[i];

            if (null == skillFrame)
            {
                m_lstActionFrames.RemoveAt(i);
                continue;
            }

            skillFrame.Update(fRealTime);

            if (skillFrame.IsTrigger(fRealTime) && skillFrame.GetFrameState() == AbstractActionFrame.EActionFrameState.None)
            {
                skillFrame.SetFrameState(AbstractActionFrame.EActionFrameState.Excute);
                skillFrame.Execute();
            }
            else if (skillFrame.IsFinish(fRealTime) && skillFrame.GetFrameState() == AbstractActionFrame.EActionFrameState.Excute)
            {
                skillFrame.SetFrameState(AbstractActionFrame.EActionFrameState.Finish);
            }
        }
    }
    #endregion

    #region System Functions
    private void Process()
    {
        switch (m_eActionState)
        {
            case EActionState.Play:
                if (m_fRunTime > GetActionTotalTime())
                {
                    m_fRunTime = GetActionTotalTime();
                }
                else
                {
                    m_fRunTime = TimeManager.Instance.GetTime() - m_fStartTime;
                }
                break;
        }
    }
    private void InsertTargetObjects(GameObject obj, TransformContainer objContainer)
    {
        if (m_mapTargetObjects.ContainsKey(objContainer.GetId()))
        {
            Debuger.LogWarning("Instance: '" + obj.name + "' ID: " + objContainer.GetId().ToString() + " Already Exist");
        }
        else
        {
            m_mapTargetObjects.Add(objContainer.GetId(), obj);
        }
    }
    #endregion
}

