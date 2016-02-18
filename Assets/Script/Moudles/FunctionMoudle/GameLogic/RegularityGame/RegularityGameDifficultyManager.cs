//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : RegularityGameDifficultyManager
//
// Created by : Baoxue at 2015/12/18 18:09:25
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace RegularityGame
{
    public class RegularityGameDifficultyManager
    {
        public enum RegularityEventType
        {
            Win,
            Lose,
            Correct,
            Correct2,
            Correct3,
            Wrong,
            Wrong2,
            Wrong3,
        }

        public enum DifficultyType
        {
            RegularityDiff,
        }

        public static readonly int m_iGameId = 20;

        public void ReportEvent(RegularityEventType eventId)
        {
            Debuger.Log("event report : " + eventId.ToString());
            AdaptiveDifficultyManager.Instance.SetUserTalent(eventId.ToString(), m_iGameId);
        }
        public RegularityGameConfig GetDifficulty()
        {
           List<RegularityGameConfig> list = new List<RegularityGameConfig>();
            var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty(DifficultyType.RegularityDiff.ToString(), m_iGameId);
            if (null == res)
            {
                Debuger.LogError("can't load correct difficulty config");
                return ConfigManager.Instance.GetRegularityGameConfig().RegularityConfigMap[0];
            }
            var config = ConfigManager.Instance.GetRegularityGameConfig();
            Debuger.Log("diff min: " + res.MinDiff + " max : " + res.MaxDiff);

            for (int i = 0; i < config.RegularityConfigMap.Count; ++i)
            {
                RegularityGameConfig elem = config.RegularityConfigMap[i];
                if (elem.Difficultyid >= res.MinDiff && elem.Difficultyid <= res.MaxDiff)
                {
                    Debuger.Log("diff id: " + elem.Difficultyid);
                    list.Add(elem);
                }
            }
            if (list.Count <= 0)
            {
                Debuger.LogError("can't load correct difficulty config");
                return config.RegularityConfigMap[0];
            }

            int index = Random.Range(0, list.Count);
            return list[index];
            //return ConfigManager.Instance.GetRegularityGameConfig().RegularityConfigMap[0];
        }
    }
}