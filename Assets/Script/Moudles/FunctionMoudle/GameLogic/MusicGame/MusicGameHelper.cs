//========================================================================
// Copyright(C): CYTX
//
// FileName : MusicGameDifficultyManager
// 
// Created by : LeoLi at 2015/12/29 14:35:54
//
// Purpose : 
//========================================================================
using Config;
using Config.Table;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MusicGame
{
    public class MusicGameHelper
    {
        public enum MusicGameEventType
        {
            Win,
            Lose,
            Correct,
            Correct3,
            Correct5,
            Wrong,
            Wrong2,
            Wrong3,
        }

        public enum MusicGameDifficultyType
        {
            TargetRange,
            MusicSpeed,
            ErrorCount,
        }

        public static readonly int m_iGameId = 60;

        public static void ReportEvent(MusicGameEventType eventId)
        {
            AdaptiveDifficultyManager.Instance.SetUserTalent(eventId.ToString(), m_iGameId);
        }
        public static float GetRangeDifficulty()
        {
            List<MusicGameRangeConfig> list = new List<MusicGameRangeConfig>();
            var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty(MusicGameDifficultyType.TargetRange.ToString(), m_iGameId);
            if (null == res)
            {
                Debuger.LogError("can't load correct difficulty config");
                return (float)ConfigManager.Instance.GetMusicGameConfig().MusicRangeConfigMap[0].Range;
            }
            var config = ConfigManager.Instance.GetMusicGameConfig();
            for (int i = 0; i < config.MusicRangeConfigMap.Count; ++i)
            {
                MusicGameRangeConfig elem = config.MusicRangeConfigMap[i];
                if (elem.Difficultyid >= res.MinDiff && elem.Difficultyid <= res.MaxDiff)
                {
                    list.Add(elem);
                }
            }
            if (list.Count <= 0)
            {
                Debuger.LogError("can't load correct difficulty config");
                return (float)config.MusicRangeConfigMap[0].Range;
            }

            int index = Random.Range(0, list.Count);
            return (float)list[index].Range;
        }
        public static float GetSpeedDifficulty()
        {
            List<MusicGameSpeedConfig> list = new List<MusicGameSpeedConfig>();
            var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty(MusicGameDifficultyType.MusicSpeed.ToString(), m_iGameId);
            if (null == res)
            {
                Debuger.LogError("can't load correct difficulty config");
                return (float)ConfigManager.Instance.GetMusicGameConfig().MusicSpeedConfigMap[0].Speed;
            }
            var config = ConfigManager.Instance.GetMusicGameConfig();
            for (int i = 0; i < config.MusicSpeedConfigMap.Count; ++i)
            {
                MusicGameSpeedConfig elem = config.MusicSpeedConfigMap[i];
                if (elem.Difficultyid >= res.MinDiff && elem.Difficultyid <= res.MaxDiff)
                {
                    list.Add(elem);
                }
            }
            if (list.Count <= 0)
            {
                Debuger.LogError("can't load correct difficulty config");
                return (float)config.MusicSpeedConfigMap[0].Speed;
            }

            int index = Random.Range(0, list.Count);
            return (float)list[index].Speed;
        }
        public static int GetErrorDifficulty()
        {
            List<MusicGameErrorConfig> list = new List<MusicGameErrorConfig>();
            var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty(MusicGameDifficultyType.ErrorCount.ToString(), m_iGameId);
            if (null == res)
            {
                Debuger.LogError("can't load correct difficulty config");
                return ConfigManager.Instance.GetMusicGameConfig().MusicErrorConfigMap[0].ErrorCount;
            }
            var config = ConfigManager.Instance.GetMusicGameConfig();
            for (int i = 0; i < config.MusicErrorConfigMap.Count; ++i)
            {
                MusicGameErrorConfig elem = config.MusicErrorConfigMap[i];
                if (elem.Difficultyid >= res.MinDiff && elem.Difficultyid <= res.MaxDiff)
                {
                    list.Add(elem);
                }
            }
            if (list.Count <= 0)
            {
                Debuger.LogError("can't load correct difficulty config");
                return config.MusicErrorConfigMap[0].ErrorCount;
            }

            int index = Random.Range(0, list.Count);
            return list[index].ErrorCount;
        }
    }
}

