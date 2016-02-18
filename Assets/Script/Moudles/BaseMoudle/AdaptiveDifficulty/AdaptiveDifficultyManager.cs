//========================================================================
// Copyright(C): CYTX
//
// FileName : AdaptiveDifficultyManager
// 
// Created by : LeoLi at 2015/12/10 17:49:38
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdaptiveDifficulty;
using System;

public class AdaptiveDifficultyManager : Singleton<AdaptiveDifficultyManager>
{
    public int CurrentGameID
    {
        get
        {
            return m_nCurrentGameID;
        }
        set
        {
            this.m_nCurrentGameID = value;
        }
    }

    // Data
    private Dictionary<string, int> m_CharTalentMap;
    private DifficultyControlDataMap m_DifficultyControlDataMap;
    private EventControlDataMap m_EventControlDataMap;
    private DefaultUserTalent m_DefaultUserTalentData;
    private DefaultUserTalent m_InitUserTalentData;
    // State
    private int m_nCurrentGameID = 1;

    #region Public Static
    public static float ConvertFloat(int input)
    {
        return (float)input / 10000f;
    }
    public static int ConvertInt(float input)
    {
        return Convert.ToInt32(input * 10000f);
    }
    #endregion

    #region Public Interface
    public void Initialize()
    {
        //ResetUserTalent();
        m_DefaultUserTalentData = ConfigManager.Instance.GetDefaultUserTalent();
        m_DifficultyControlDataMap = ConfigManager.Instance.GetDifficultyControlDataMap();
        m_EventControlDataMap = ConfigManager.Instance.GetEventControlDataMap();
        // Init User Talent
        m_InitUserTalentData = new DefaultUserTalent();
        m_InitUserTalentData.MapTalent = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> value in m_DefaultUserTalentData.MapTalent)
        {
            m_InitUserTalentData.MapTalent.Add(value.Key,value.Value);
        }
    }
    // Difficulty Control
    public GameDifficulty GetGameDifficulty(string difficultyName)
    {
        if (m_DifficultyControlDataMap == null || m_DifficultyControlDataMap.MapFileData == null)
        {
            Debuger.LogError("m_DifficultyControlDataMap not Initialized !");
            return null;
        }
        if (!m_DifficultyControlDataMap.MapFileData.ContainsKey(m_nCurrentGameID))
        {
            Debuger.LogError("current game id is not found " + m_nCurrentGameID);
            return null;
        }

        DifficultyControlData currentDifficultyControlData = m_DifficultyControlDataMap.MapFileData[m_nCurrentGameID];
        float minDiff = 0f;
        float maxDiff = 1f;

        CalculateGameDifficulty(difficultyName, currentDifficultyControlData, ref minDiff, ref maxDiff);

        GameDifficulty result = new GameDifficulty(minDiff, maxDiff);

        Debuger.Log("current diff min : " + minDiff + " max : " + maxDiff);

        return result;
    }
    public GameDifficulty GetGameDifficulty(string difficultyName, int gameID)
    {
        if (m_DifficultyControlDataMap == null || m_DifficultyControlDataMap.MapFileData == null)
        {
            Debuger.LogError("m_DifficultyControlDataMap not Initialized !");
            return null;
        }
        if (!m_DifficultyControlDataMap.MapFileData.ContainsKey(gameID))
        {
            Debuger.LogError("current game id is not found " + gameID);
            return null;
        }
        m_nCurrentGameID = gameID;
       

        DifficultyControlData currentDifficultyControlData = m_DifficultyControlDataMap.MapFileData[gameID];
        float minDiff = 0f;
        float maxDiff = 1f;

        CalculateGameDifficulty(difficultyName, currentDifficultyControlData, ref minDiff, ref maxDiff);

        GameDifficulty result = new GameDifficulty(minDiff, maxDiff);

        Debuger.Log("current diff:" + difficultyName + " min : " + minDiff + " max : " + maxDiff);

        return result;
    }
    // Talent Control
    public void SetUserTalent(string eventName)
    {
        Debuger.Log("report event : " + eventName);

        if (m_EventControlDataMap == null || m_EventControlDataMap.MapFileData == null)
        {
            Debuger.LogError("m_EventControlDataMap not Initialized !");
        }
        if (m_EventControlDataMap.MapFileData.ContainsKey(m_nCurrentGameID))
        {
            Debuger.LogError("game id is not found " + m_nCurrentGameID);
            return;
        }

        if (!m_EventControlDataMap.MapFileData.ContainsKey(m_nCurrentGameID))
        {
            Debuger.LogError("current game id is not found " + m_nCurrentGameID);
            return;
        }
        EventControlData currentEventControlData = m_EventControlDataMap.MapFileData[m_nCurrentGameID];

        CalculateUserTalent(eventName, currentEventControlData);
    }
    public void SetUserTalent(string eventName, int gameID)
    {
        Debuger.Log("report event : " + eventName);

        if (m_EventControlDataMap == null || m_EventControlDataMap.MapFileData == null)
        {
            Debuger.LogError("m_EventControlDataMap not Initialized !");
        }
        if (!m_EventControlDataMap.MapFileData.ContainsKey(gameID))
        {
            Debuger.LogError("current game id is not found " + gameID);
            return;
        }

        m_nCurrentGameID = gameID;
        EventControlData currentEventControlData = m_EventControlDataMap.MapFileData[gameID];

        CalculateUserTalent(eventName, currentEventControlData);
    }
    public void ResetUserTalentToDefault()
    {
        m_CharTalentMap = m_InitUserTalentData.MapTalent;
        UpdateUserTalent();
    }
    #endregion

    #region System Functions
    // Difficulty Control
    private void CalculateGameDifficulty(string difficultyName, DifficultyControlData currentDifficultyControlData, ref float minDiff, ref float maxDiff)
    {
        if (m_CharTalentMap == null)
        {
            ResetUserTalent();
        }
        if (currentDifficultyControlData.DifficultyConfig.ContainsKey(difficultyName))
        {
            DifficultyConfig difficultyConfig = currentDifficultyControlData.DifficultyConfig[difficultyName];
            float diff = ConvertFloat(difficultyConfig.DifficultyOffset);
            foreach (string talentName in difficultyConfig.TalentEffect.Keys)
            {
                if (m_CharTalentMap.ContainsKey(talentName))
                {
                    float score = ConvertFloat(m_CharTalentMap[talentName]);
                    float effect = ConvertFloat(difficultyConfig.TalentEffect[talentName]);
                    diff += score * effect;
                }
                else
                {
                    Debuger.LogWarning("Wrong Talent Name in DifficultyConfig : " + talentName + ", it does not Exist in UserTalent");
                }
            }
            minDiff = diff - ConvertFloat(difficultyConfig.DifficultyRange);
            maxDiff = diff + ConvertFloat(difficultyConfig.DifficultyRange);

            //check
            if (minDiff < 0.0f)
            {
                minDiff = 0.0f;
            }
            if (maxDiff > 1.0f)
            {
                maxDiff = 1.0f;
            }
            if (minDiff > maxDiff)
            {
                minDiff = maxDiff;
            }
        }
        else
        {
            Debuger.LogWarning("Wrong Difficulty Name : " + difficultyName + ", it does not Exist in DifficultyControlData");
        }
    }
    // Talent Control
    private void CalculateUserTalent(string eventName, EventControlData currentEventControlData)
    {
        if (m_CharTalentMap == null)
        {
            ResetUserTalent();
        }
        //foreach (string talentName in currentEventControlData.EventConfig.Keys)
        //{
        //    if (m_CharTalentMap.ContainsKey(talentName))
        //    {
        //        EventConfig talentConfig = currentEventControlData.EventConfig[talentName];
        //        if (talentConfig.TalentEffect.ContainsKey(eventName))
        //        {
        //            float effect = ConvertFloat(talentConfig.TalentEffect[eventName]);
        //            float score = ConvertFloat(m_CharTalentMap[talentName]);
        //            float result = Mathf.Clamp01(score + effect);
        //            m_CharTalentMap[talentName] = ConvertInt(result);

        //            UpdateUserTalent();
        //        }
        //        else
        //        {
        //            Debuger.LogWarning("Wrong Event Name : " + eventName + ", it does not Exist in Current EventControlData");
        //        }
        //    }
        //    else
        //    {
        //        Debuger.LogWarning("Wrong Talent Name : " + talentName + ", it does not Exist in UserTalent");
        //    }
        //}
        if (currentEventControlData.EventConfig.ContainsKey(eventName))
        {
            EventConfig eventConfig = currentEventControlData.EventConfig[eventName];
            foreach (string talentName in eventConfig.TalentEffect.Keys)
            {
                if (m_CharTalentMap.ContainsKey(talentName))
                {
                    float score = ConvertFloat(m_CharTalentMap[talentName]);
                    float effect = ConvertFloat(eventConfig.TalentEffect[talentName]);
                    float result = Mathf.Clamp01(score + effect);
                    m_CharTalentMap[talentName] = ConvertInt(result);

                    UpdateUserTalent();
                }
                else
                {
                    Debuger.LogWarning("Wrong Talent Name : " + talentName + ", it does not Exist in UserTalent");
                }
            }
        }
        else
        {
            Debuger.LogWarning("Wrong Event Name : " + eventName + ", it does not Exist in Current EventControlData");
        }
    }
    private void ResetUserTalent()
    {
        m_CharTalentMap = PlayerManager.Instance.GetCharBaseData().CharTalentMap;
        if (m_CharTalentMap == null)
        {
            //m_CharTalentMap = new Dictionary<string, int>();
            m_CharTalentMap = m_DefaultUserTalentData.MapTalent;
        }
    }
    private void UpdateUserTalent()
    {
        PlayerManager.Instance.GetCharBaseData().CharTalentMap = m_CharTalentMap;
    }
    #endregion
}

