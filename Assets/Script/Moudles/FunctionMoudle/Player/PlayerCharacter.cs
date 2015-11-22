using Config;
using UnityEngine;
using System.Collections;
using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using Moudles.BaseMoudle.Converter;

public class PlayerCharacter : IStateMachineBehaviour,ITransformBehaviour
{
    protected CharTransformData     m_CharTransformData;
    protected StateMachine      m_StateMachine;
    protected CharactorConfig   m_CharacterConfig;


    private CharBaseData m_CharBaseData;
    private CharCounterData m_CharCounterData;
    private static PlayerCharacter m_PlayerInstance;

    private PlayerCharacter()
    {

    }
    public static PlayerCharacter Create(CharBaseInfo charBaseInfo, CharCounterInfo charCountInfo)
    {
        PlayerCharacter character = new PlayerCharacter();
        character.m_CharBaseData = (ConverterManager.Instance.FindConverter(typeof(CharBaseData)) as CharBaseConverter).Convert(charBaseInfo) as CharBaseData;
        character.m_CharBaseData.CheckValid();
        character.m_CharCounterData = (ConverterManager.Instance.FindConverter(typeof(CharCounterData)) as CharCounterConverter).Convert(charCountInfo) as CharCounterData;
        character.m_CharCounterData.CheckValid();

        character.m_CharCounterData.SetBit8Count(10, (sbyte)10);

        character.m_CharacterConfig = ConfigManager.Instance.GetCharactorConfig(character.m_CharBaseData.CharId);
        if (null == character.m_CharacterConfig)
        {
            Debuger.LogWarning("can't load char : " + charBaseInfo.CharId);
            return null;
        }
        character.m_StateMachine = new StateMachine(0, 0, character);
        character.m_CharTransformData = new CharTransformData();
        character.m_CharTransformData.Initialize(character, character.m_CharacterConfig.ModelResource, AssetType.Char);
        LifeTickTask.Instance.RegisterToUpdateList(character.Update);
        LifeManager.RegisterLife(character.m_CharBaseData.CharId, character);

        m_PlayerInstance = character;
        return character;
    }
    public static PlayerCharacter GetPlayerInstance()
    {
        return m_PlayerInstance;
    }
    public int GetInstanceId()
    {
        return m_CharBaseData.CharId;
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
        LifeManager.UnRegisterLife(m_CharBaseData.CharId);
        m_StateMachine.Distructor();
        m_CharTransformData.Distructor();
        //m_CountData.Distructor();
    }
    public void MoveTo(Vector3 Position)
    {
        m_StateMachine.TryEnterState(ELifeState.Move, false);
        m_CharTransformData.MoveTo(Position,2.0f,0.0f,OnStopMove);
    }
    private void OnStopMove()
    {
        m_StateMachine.TryEnterState(ELifeState.Idle, false);
    }
    private void Update()
    {
        m_CharTransformData.Update();
    }
    public CharBaseData GetCharBaseData()
    {
        return m_CharBaseData;
    }
    public CharCounterData GetCharCounterData()
    {
        return m_CharCounterData;
    }
}
