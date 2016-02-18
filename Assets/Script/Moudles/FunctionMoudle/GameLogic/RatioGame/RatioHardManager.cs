using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RatioHardManager {

    public const int gameId = 10;
	public static int GetBallCount()
    {
        var list = new List<Config.MiniGameHardConfig>();

        var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty("BallCount", gameId);
        if (null == res)
        {
            Debuger.LogError("can't load correct difficulty config");
            return ConfigManager.Instance.GetRatioGameConfig().BallCount[0].Count;
        }

        var compareList = ConfigManager.Instance.GetRatioGameConfig().BallCount;
        for (int i = 0; i < compareList.Count; ++i)
        {
            var elem = compareList[i];
            if (elem.Hard >= res.MinDiff && elem.Hard <= res.MaxDiff)
            {
                list.Add(elem);
            }
        }
        if (list.Count <= 0)
        {
            Debuger.LogError("can't load correct difficulty config");
            return compareList[0].Count;
        }

        int index = Random.Range(0, list.Count);
        return list[index].Count;
    }

    public static int GetBallColor()
    {
        var list = new List<Config.MiniGameHardConfig>();

        var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty("BallColor", gameId);
        if (null == res)
        {
            Debuger.LogError("can't load correct difficulty config");
            return ConfigManager.Instance.GetRatioGameConfig().BallColor[0].Count;
        }

        var compareList = ConfigManager.Instance.GetRatioGameConfig().BallColor;
        for (int i = 0; i < compareList.Count; ++i)
        {
            var elem = compareList[i];
            if (elem.Hard >= res.MinDiff && elem.Hard <= res.MaxDiff)
            {
                list.Add(elem);
            }
        }
        if (list.Count <= 0)
        {
            Debuger.LogError("can't load correct difficulty config");
            return compareList[0].Count;
        }

        int index = Random.Range(0, list.Count);
        return list[index].Count;
    }

    public static int GetBallMaterial()
    {
        var list = new List<Config.MiniGameHardConfig>();

        var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty("BallMaterial", gameId);
        if (null == res)
        {
            Debuger.LogError("can't load correct difficulty config");
            return ConfigManager.Instance.GetRatioGameConfig().BallMaterial[0].Count;
        }

        var compareList = ConfigManager.Instance.GetRatioGameConfig().BallMaterial;
        for (int i = 0; i < compareList.Count; ++i)
        {
            var elem = compareList[i];
            if (elem.Hard >= res.MinDiff && elem.Hard <= res.MaxDiff)
            {
                list.Add(elem);
            }
        }
        if (list.Count <= 0)
        {
            Debuger.LogError("can't load correct difficulty config");
            return compareList[0].Count;
        }

        int index = Random.Range(0, list.Count);
        return list[index].Count;
    }

    public static int GetBallSpeed()
    {
        var list = new List<Config.MiniGameHardConfig>();

        var res = AdaptiveDifficultyManager.Instance.GetGameDifficulty("BallSpeed", gameId);
        if (null == res)
        {
            Debuger.LogError("can't load correct difficulty config");
            return ConfigManager.Instance.GetRatioGameConfig().BallSpeed[0].Count;
        }

        var compareList = ConfigManager.Instance.GetRatioGameConfig().BallSpeed;
        for (int i = 0; i < compareList.Count; ++i)
        {
            var elem = compareList[i];
            if (elem.Hard >= res.MinDiff && elem.Hard <= res.MaxDiff)
            {
                list.Add(elem);
            }
        }
        if (list.Count <= 0)
        {
            Debuger.LogError("can't load correct difficulty config");
            return compareList[0].Count;
        }

        int index = Random.Range(0, list.Count);
        return list[index].Count;
    }
}
