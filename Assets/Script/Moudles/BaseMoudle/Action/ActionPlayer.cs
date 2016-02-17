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
    private static int AllocInstanceId()
    {
        return ++g_ActionInstanceID;
    }
    private static int g_ActionInstanceID = 0;
    private int m_ActionId;
    private ActionFileData m_ActionFileData;
    private List<AbstractActionFrame> m_lstActionFrames = new List<AbstractActionFrame>();
    //private List<GameObject> m_lstAffectedObjects = new List<GameObject>();
    //private List<GameObject> m_lstGeneratedObjects = new List<GameObject>();
    private Dictionary<int, GameObject> m_mapTargetObjects = new Dictionary<int, GameObject>();
    private ActionParam m_ActionParam;
    //Init Info
    private bool m_InitLock;
    private bool m_InitOverObstacle;
    private Transform m_InitTarget;
    private float m_InitDistance = 15f;
    private float m_InitHeight = 9f;
    private float m_InitOffsetHeight = 0.5f;
    private float m_InitRotation = 0f;
    private float m_InitPositonDamping = 2f;
    private float m_InitRotationDamping = 2f;
    private Vector3 m_InitPos = new Vector3(0, 0, 0);
    private Vector3 m_InitRot = new Vector3(0, 0, 0);
    #endregion
    private List<GameObject> m_AffectedObject;

    public ActionPlayer(int iActionId, ActionFileData data, ActionParam param, List<GameObject> affectedObjects = null)
    {
        //Check
        if (null == data)
        {
            return;
        }
        m_AffectedObject = affectedObjects;

        //Save Init Info
        SaveInitInfo();
        //Set Data
        m_ActionId = iActionId;
        m_ActionFileData = data;
        m_nInstanceID = AllocInstanceId();
        m_ActionParam = param;
        if (affectedObjects != null && affectedObjects.Count > 0)
        {
            //Add Target
            foreach (GameObject obj in affectedObjects)
            {
                if (obj == null)
                {
                    Debuger.LogWarning("None(GameObject) in TargetObjectList");
                    continue;
                }
                CharTransformContainer objContainer = obj.GetComponent<CharTransformContainer>();
                if (objContainer == null)
                {
                    Debuger.LogWarning("Instance Container Not Found in " + obj.name);
                    continue;
                }
                else
                {
                    AddAffectedObject(obj, objContainer);
                }
                //disable rigidbody
                SetPhysicStatus(obj, false);
            }
        }
        Reset();
    }
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
    public int GetActionId()
    {
        return m_ActionId;
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
    public ActionParam GetActionParam()
    {
        return m_ActionParam;
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
    public void InsertGeneratedObjects(Dictionary<int, GameObject> generatedObjects)
    {
        foreach (int index in generatedObjects.Keys)
        {
            CharTransformContainer objContainer = generatedObjects[index].GetComponent<CharTransformContainer>();
            if (objContainer == null)
            {
                Debuger.LogWarning("Generated Container Not Found in " + generatedObjects[index].name + " ,ID: " + index.ToString());
                continue;
            }
            else
            {
                AddGeneratedObject(generatedObjects[index], index);
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
                ActionFrameData inputData = m_ActionFileData.FrameDatalist[iIndex];
                AbstractActionFrame skillFrame = ActionFrameFactory.CreateActionFrame(this, inputData);
                m_lstActionFrames.Add(skillFrame);
            }
        }

        // Stop Char Move
        StopCharMove();
        //DisableAI();
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

                frame.DestoryBase();
            }
            ResetInitInfo();
            m_lstActionFrames.Clear();
            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_ACTION_FININSH, m_ActionParam));
            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, GameLogicSceneType.ActionEnd));
            //EnableAI();
            if(null != m_AffectedObject)
            {
                foreach(var obj in m_AffectedObject)
                {
                    if(null == obj)
                    {
                        continue;
                    }
                    //enable 
                    SetPhysicStatus(obj, true);
                }
            }
        }
    }
    public void Pause()
    {
        m_eActionState = EActionState.Pause;
    }
    public void Play()
    {
        m_eActionState = EActionState.Play;
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
                skillFrame.ExecuteBase();
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
            case EActionState.Pause:
                m_fStartTime = TimeManager.Instance.GetTime();
                break;
        }
    }
    private void AddAffectedObject(GameObject obj, CharTransformContainer objContainer)
    {
        if (m_mapTargetObjects.ContainsKey(objContainer.GetId()))
        {
            Debuger.LogWarning("Instance: '" + obj.name + "' ID: " + objContainer.GetId().ToString() + " Already Exist");
        }
        else
        {
            m_mapTargetObjects.Add(objContainer.GetId(), obj);
            Debuger.Log("AffectedObject Add: " + objContainer.GetId().ToString());
        }
    }
    private void AddGeneratedObject(GameObject obj, int iD)
    {
        if (m_mapTargetObjects.ContainsKey(iD))
        {
            Debuger.LogWarning("Instance: '" + obj.name + "' ID: " + iD.ToString() + " Already Exist");
        }
        else
        {
            m_mapTargetObjects.Add(iD, obj);
            Debuger.Log("GeneratedObject Add: " + iD.ToString());
        }
    }
    private void SaveInitInfo()
    {
        GlobalScripts.Instance.Reset();
        m_InitLock = GlobalScripts.Instance.mGameCamera.LockCam;
        m_InitOverObstacle = GlobalScripts.Instance.mGameCamera.IsOverObstacle;
        m_InitPositonDamping = GlobalScripts.Instance.mGameCamera.PositonDamping;
        m_InitRotationDamping = GlobalScripts.Instance.mGameCamera.RotationDamping;
        m_InitDistance = GlobalScripts.Instance.mGameCamera.Distance;
        m_InitHeight = GlobalScripts.Instance.mGameCamera.Height;
        m_InitOffsetHeight = GlobalScripts.Instance.mGameCamera.OffsetHeight;
        m_InitRotation = GlobalScripts.Instance.mGameCamera.Rotation;
        if (m_InitLock && null != GlobalScripts.Instance.mGameCamera.LookTarget)
        {
            m_InitTarget = GlobalScripts.Instance.mGameCamera.LookTarget;
        }
        else
        {
            m_InitPos = GlobalScripts.Instance.mGameCamera.GetCurrentPosition();
            m_InitRot = GlobalScripts.Instance.mGameCamera.GetCurrentEuler();
        }
    }
    private void ResetInitInfo()
    {
        GlobalScripts.Instance.mGameCamera.LockCam = m_InitLock;
        GlobalScripts.Instance.mGameCamera.PositonDamping = m_InitPositonDamping;
        GlobalScripts.Instance.mGameCamera.RotationDamping = m_InitRotationDamping;
        GlobalScripts.Instance.mGameCamera.Distance = m_InitDistance;
        GlobalScripts.Instance.mGameCamera.Height = m_InitHeight;
        GlobalScripts.Instance.mGameCamera.OffsetHeight = m_InitOffsetHeight;
        if (!GlobalScripts.Instance.mGameCamera.CheckRoating())
            GlobalScripts.Instance.mGameCamera.Rotation = m_InitRotation;
        GlobalScripts.Instance.mGameCamera.IsOverObstacle = m_InitOverObstacle;
        if (m_InitLock && null != m_InitTarget)
        {
            GlobalScripts.Instance.mGameCamera.LookTarget = m_InitTarget;
        }
        else
        {
            GlobalScripts.Instance.mGameCamera.SetCameraPos(m_InitPos, true);
            GlobalScripts.Instance.mGameCamera.SetCameraRot(m_InitRot, true);
        }
    }
    private void StopCharMove()
    {
        // stop player
        PlayerCharacter player = PlayerManager.Instance.GetPlayerInstance();
        if (player != null)
        {
            player.StopMove();
        }
        // stop npcs
        //if (m_mapTargetObjects == null || m_mapTargetObjects.Count <= 0)
        //    return;
        //foreach (GameObject obj in m_mapTargetObjects.Values)
        //{
        //    CharTransformContainer container = obj.GetComponent<CharTransformContainer>();
        //    if (container != null)
        //    {
        //        if (container.GetData() is Npc)
        //        {
        //            Npc npc = (Npc)container.GetData();
        //            npc.StopMove();
        //        }
        //    }
        //}
    }
    private void DisableAI()
    {
        if (m_mapTargetObjects == null || m_mapTargetObjects.Count <= 0)
        {
            return;
        }
        foreach (GameObject charObject in m_mapTargetObjects.Values)
        {
            CharTransformContainer container = charObject.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                Debuger.LogError("No Container in " + charObject.ToString());
                return;
            }
            Npc npc = null;
            if (container.GetData() is Npc)
            {
                npc = (Npc)container.GetData();
            }
            if (null != npc)
            {
                npc.SetAIStatus(false);
            }

        }
    }
    private void EnableAI()
    {
        if (m_mapTargetObjects == null || m_mapTargetObjects.Count <= 0)
        {
            return;
        }
        foreach (GameObject charObject in m_mapTargetObjects.Values)
        {
            CharTransformContainer container = charObject.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                Debuger.LogError("No Container in " + charObject.ToString());
                return;
            }
            Npc npc = null;
            if (container.GetData() is Npc)
            {
                npc = (Npc)container.GetData();
            }
            if (null != npc)
            {
                npc.SetAIStatus(true);
            }
        }
    }
    private void SetPhysicStatus(GameObject obj,bool status)
    {
        //disable rigidbody
        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
        if (null != rigidbody)
        {
            rigidbody.isKinematic = !status;
        }
    }
    #endregion
}

