using System.Collections.Generic;
using Cache;
using Moudles.BaseMoudle.Character;
using Moudles.BaseMoudle.Converter;
using NetWork.Auto;
using UnityEngine;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager>
{
    //template data
    private NetWork.Auto.CharacterInfo m_CharInfo;

    //local data
    private CharBaseData    m_CharBaseData;
    private CharCounterData m_CharCounterData;
    private CharBagData     m_CharBagData;
    private CharMissionData m_CharMissionData;
    private PlayerCharacter m_PlayerInstance;

    public void Initialize(NetWork.Auto.CharacterInfo info)
    {
        m_CharInfo = info;
        //merge first
        //MergeData();
        //init
        InitData();
    }
    public PlayerCharacter GetPlayerInstance()
    {
        if (null == m_PlayerInstance)
        {
            if (m_CharInfo == null)
            {
                return null;
            }
        }
        return m_PlayerInstance;
    }
    public void ClearPlayer()
    {
        if (null != m_PlayerInstance)
        {
            m_PlayerInstance.Distructor();
            m_PlayerInstance = null;
        }
    }
    public void SetPlayerInstance(PlayerCharacter player)
    {
        if (null == player)
        {
            return;
        }
        m_PlayerInstance = player;
    }
    public void CreatePlayerChar()
    {
        m_PlayerInstance = PlayerCharacter.Create(10000001);
    }
    private void InitData()
    {
        //char base data
        m_CharBaseData = (ConverterManager.Instance.FindConverter(typeof(CharBaseData)) as CharBaseConverter).Convert(m_CharInfo.CharBaseInfo) as CharBaseData;
        m_CharBaseData.CheckValid();
        

        //char counter data
        m_CharCounterData = (ConverterManager.Instance.FindConverter(typeof(CharCounterData)) as CharCounterConverter).Convert(m_CharInfo.CharCounterInfo) as CharCounterData;
        if (null == m_CharCounterData)
        {
            m_CharCounterData = new CharCounterData(m_CharBaseData.CharId);
            m_CharCounterData.Init = false;
        }
        m_CharCounterData.CheckValid();
        
        //char bag data
        m_CharBagData =
            (ConverterManager.Instance.FindConverter(typeof(CharBagData)) as CharBagConverter).Convert(m_CharInfo.CharBagInfo) as CharBagData;

        if (null == m_CharBagData)
        {
            m_CharBagData = new CharBagData(m_CharBaseData.CharId,new List<ItemInfo>());
            m_CharBagData.Init = false;
        }
        m_CharBagData.CheckValid();
        ItemManager.Instance.Initialize();


        //char mission data
       
        m_CharMissionData =
            (ConverterManager.Instance.FindConverter(typeof(CharMissionData)) as CharMissionConverter).Convert(m_CharInfo.CharMissionInfo) as CharMissionData;
        if (null == m_CharMissionData)
        {
            m_CharMissionData = new CharMissionData(m_CharBaseData.CharId);
            m_CharMissionData.MissionList = new List<MissionInfo>();
            m_CharMissionData.Init = false;
        }
        m_CharMissionData.CheckValid();
    }
    public CharBaseData GetCharBaseData()
    {
        return m_CharBaseData;
    }
    public CharCounterData GetCharCounterData()
    {
        return m_CharCounterData;
    }
    public CharBagData GetCharBagData()
    {
        return m_CharBagData;
    }
    public CharMissionData GetMissionData()
    {
        return m_CharMissionData;
    }

    #region merge
    private void MergeData()
    {
        CacheKeyInfo keyInfo = CacheKeyContants.CHAR_DATA_SNAPSHOT_KEY.BuildCacheInfo(PlayerManager.Instance.GetCharBaseData().CharId);

        CharacterDataSnapshot data = CacheManager.GetInsance().Get(keyInfo) as CharacterDataSnapshot;

        if (null != data)
        {
            foreach (var elem in data.DataList)
            {
                if (elem is CharBaseInfo)
                {
                    m_CharInfo.CharBaseInfo = (CharBaseInfo)elem;
                }
                else if (elem is CharCounterInfo)
                {
                    m_CharInfo.CharCounterInfo = (CharCounterInfo)elem;
                }
                else if (elem is CharBagInfo)
                {
                    m_CharInfo.CharBagInfo = (CharBagInfo)elem;

                }
                else if (elem is CharMissionInfo)
                {
                    m_CharInfo.CharMissionInfo = (CharMissionInfo)elem;
                }
            }
        }
    }
    #endregion
}
