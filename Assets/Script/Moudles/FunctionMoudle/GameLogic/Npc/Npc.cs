using System;
using BehaviourTree;
using Config;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Npc : IStateMachineBehaviour, ITransformBehaviour, IAIBehaviour
{
    protected CharTransformData m_CharTransformData;
    protected StateMachine m_StateMachine;
    protected AIAgent m_AIAgent;
    protected NpcConfig m_NpcBaseInfo;
    protected int m_iId;
    private List<Vector3> m_lstMovePath;
    private List<float> m_lstMoveSpeed;
    private int currentMovePoint;
    // anim
    private int currentAnimPoint;
    private List<string> m_lstAnimName;
    private bool m_bInitGroup;

    public bool IsPlayerControlled
    {
        set;
        get;
    }
    public bool IsInGroup
    {
        set
        {
            m_NpcBaseInfo.IsInGroup = value;
        }
        get
        {
            return m_NpcBaseInfo.IsInGroup;
        }
    }
    public int GetInstanceId()
    {
        return m_iId;
    }
    public AIAgent GetAIAgent()
    {
        return m_AIAgent;
    }
    public TransformDataBase GetTransformData()
    {
        return m_CharTransformData;
    }
    public StateMachine GetStateController()
    {
        return m_StateMachine;
    }
    public void Initialize(int id)
    {
        m_iId = id;
        m_NpcBaseInfo = ConfigManager.Instance.GetNpcConfig(id);
        if (null == m_NpcBaseInfo)
        {
            Debuger.LogWarning("can't load target npc: " + id);
            return;
        }
        m_bInitGroup = m_NpcBaseInfo.IsInGroup;
        m_StateMachine = new StateMachine(0, 0, this);
        m_CharTransformData = new CharTransformData();
        m_CharTransformData.Initialize(this, m_NpcBaseInfo.ModelResource, AssetType.Char);
        //m_CharTransformData.AddNavmeshObs();
        m_AIAgent = new AIAgent(m_NpcBaseInfo.AiId);

        LifeTickTask.Instance.RegisterToUpdateList(Update);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);

        LifeManager.RegisterLife(m_iId, this);
    }
    public void Distructor()
    {
        m_CharTransformData.Distructor();
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        LifeManager.UnRegisterLife(m_iId);
    }
    public void SetAIStatus(bool status)
    {
        if (m_AIAgent != null)
        {
            m_AIAgent.Active(status, this);
        }
    }
    private void Update()
    {
        try
        {
            if (m_AIAgent != null)
            {
                m_AIAgent.OnTick();
            }
            m_CharTransformData.Update();
        }
        catch (Exception e)
        {
            Debuger.LogWarning("npc throw error : " + e.Message);
            throw;
        }

    }
    public void MoveTo(Vector3 Position, bool isObjMove = false, Action callback = null)
    {
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        if (callback != null)
        {
            m_CharTransformData.MoveTo(Position, 50.0f, 0.5f, callback, isObjMove);
        }
        else
        {
            m_CharTransformData.MoveTo(Position, 50.0f, 0.5f, OnFinishMove, isObjMove);
        }
    }
    public void MoveTo(Vector3 Position, float Speed, bool isObjMove = false)
    {
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        m_CharTransformData.MoveTo(Position, Speed, 0.5f, OnFinishMove, isObjMove);
    }
    public void MovePath(List<Vector3> Path)
    {
        currentMovePoint = 0;
        m_lstMovePath = Path;
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        m_CharTransformData.MoveTo(m_lstMovePath[currentMovePoint], 5.0f, 0.5f, OnNextMove);
    }
    public void MovePath(List<CharMovement> Path)
    {
        currentMovePoint = 0;
        //m_lstMovePath = new List<Vector3>(Path.Keys);
        //m_StateMachine.TryEnterState(ELifeState.Move, false);
        //m_CharTransformData.MoveTo(m_lstMovePath[currentMovePoint], Path[m_lstMovePath[currentMovePoint]], 0.5f, OnNextMove);
        m_lstMovePath = new List<Vector3>();
        m_lstMoveSpeed = new List<float>();
        for (int i = 0; i < Path.Count; i++)
        {
            m_lstMovePath.Add(Path[i].Target);
            m_lstMoveSpeed.Add(Path[i].Speed);
        }

        m_CharTransformData.MoveTo(m_lstMovePath[currentMovePoint], m_lstMoveSpeed[currentMovePoint], 0.5f, OnNextMove);
    }
    public void DirectPlayAnimation(string anim)
    {
        m_CharTransformData.DirectPlayAnimation(anim);
    }
    public void DirectPlayAnimation(List<string> lstAnim)
    {
        currentAnimPoint = 0;
        m_lstAnimName = lstAnim;

        m_CharTransformData.DirectPlayAnimation(m_lstAnimName[currentAnimPoint], OnNextAnim);
    }
    public void StopMove()
    {
        m_CharTransformData.StopMove();
    }
    public void Rotate(float rotate)
    {
        m_CharTransformData.CharRotate(rotate);
    }
    public void ResetGroup ()
    {
        IsInGroup = m_bInitGroup;
    }

    // Private
    private void OnFinishMove()
    {
        m_StateMachine.TryEnterState(ELifeState.Idle, false);
    }
    private void OnNextMove()
    {
        currentMovePoint++;
        if (currentMovePoint < m_lstMovePath.Count)
        {
            float speed = m_lstMoveSpeed == null ? 5f : m_lstMoveSpeed[currentMovePoint];
            m_CharTransformData.MoveTo(m_lstMovePath[currentMovePoint], speed, 0.5f, OnNextMove);
        }
        else
        {
            currentMovePoint = 0;
            m_lstMovePath = null;
            m_lstMoveSpeed = null;
            OnFinishMove();
        }
    }
    private void OnHitLife(MessageObject msg)
    {
        var transform = ((Transform)(msg.msgValue));

        if (transform == m_CharTransformData.GetGameObject().transform)
        {
            // set high light
            //m_CharTransformData.SetSelectedStatus(true);
            //trigger npc hit function
            FuncMethods.HandleFuncExec(HandleTarget.GetHandleTarget(this), m_NpcBaseInfo.ClickFuncId, null);
        }
        else
        {
            //IsPlayerControlled = false;
            m_CharTransformData.SetSelectedStatus(false);
        }
    }
    private void OnHitTerrain(MessageObject msg)
    {
        //IsPlayerControlled = false;
        m_CharTransformData.SetSelectedStatus(false);
    }
    private void OnNextAnim()
    {
        currentAnimPoint++;
        if (currentAnimPoint < m_lstAnimName.Count)
        {
            m_CharTransformData.DirectPlayAnimation(m_lstAnimName[currentAnimPoint], OnNextAnim);
        }
        else
        {
            currentMovePoint = 0;
            m_lstAnimName = null;
        }
    }
}
