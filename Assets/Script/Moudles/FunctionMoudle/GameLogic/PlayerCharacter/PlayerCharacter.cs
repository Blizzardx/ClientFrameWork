using Config;
using UnityEngine;
using System.Collections;
using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using Moudles.BaseMoudle.Converter;
using System.Collections.Generic;

public class PlayerCharacter : IStateMachineBehaviour, ITransformBehaviour
{
    protected CharTransformData m_CharTransformData;
    protected StateMachine m_StateMachine;
    protected CharactorConfig m_CharacterConfig;
    private int m_iInstanceId;
    // move
    private int currentMovePoint;
    private List<Vector3> m_lstMovePath;
    private List<float> m_lstMoveSpeed;
    // anim
    private int currentAnimPoint;
    private List<string> m_lstAnimName;

    private PlayerCharacter()
    {

    }
    public static PlayerCharacter Create(int resId)
    {
        PlayerCharacter character = new PlayerCharacter();
        character.m_iInstanceId = resId;

        character.m_CharacterConfig = ConfigManager.Instance.GetCharactorConfig(resId);

        if (null == character.m_CharacterConfig)
        {
            Debuger.LogWarning("can't load char : " + character.m_iInstanceId);
            return null;
        }
        character.m_StateMachine = new StateMachine(0, 0, character);
        character.m_CharTransformData = new CharTransformData();
        character.m_CharTransformData.Initialize(character, character.m_CharacterConfig.ModelResource, AssetType.Char);
        LifeTickTask.Instance.RegisterToUpdateList(character.Update);
        LifeManager.RegisterLife(character.m_iInstanceId, character);

        return character;
    }
    public int GetInstanceId()
    {
        return m_iInstanceId;
    }
    public TransformDataBase GetTransformData()
    {
        return m_CharTransformData;
    }
    public StateMachine GetStateController()
    {
        return m_StateMachine;
    }
    public void Distructor()
    {
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        LifeManager.UnRegisterLife(m_iInstanceId);
        m_StateMachine.Distructor();
        m_CharTransformData.Distructor();
        //m_CountData.Distructor();
    }
    public GameObject GetFollowPoint(int index)
    {
        GameObject res = null;
        res = ComponentTool.FindChild("FollowPoint" + index, m_CharTransformData.GetGameObject());
        if (null == res)
        {
            res = m_CharTransformData.GetGameObject();
        }
        return res;
    }
    private void Update()
    {
        m_CharTransformData.Update();
    }
    public void MoveTo(Vector3 Position)
    {
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        m_CharTransformData.MoveTo(Position, 50.0f, 0.5f, OnFinishMove);
    }
    public void MoveTo(Vector3 Position, float Speed)
    {
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        m_CharTransformData.MoveTo(Position, Speed, 0.5f, OnFinishMove);
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
    private void OnFinishMove()
    {
        m_StateMachine.TryEnterState(ELifeState.Idle, false);
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
