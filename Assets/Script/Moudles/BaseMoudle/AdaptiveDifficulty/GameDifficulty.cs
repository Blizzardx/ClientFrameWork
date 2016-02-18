//========================================================================
// Copyright(C): CYTX
//
// FileName : GameDifficulty
// 
// Created by : LeoLi at 2015/12/10 18:43:09
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdaptiveDifficulty;
public class GameDifficulty
{
    public float MinDiff;
    public float MaxDiff;

    public GameDifficulty(float min , float max)
    {
        if (min > max)
        {
            var temp = min;
            min = max;
            max = temp;
        }

        MinDiff = min;
        MaxDiff = max;
    }
}

