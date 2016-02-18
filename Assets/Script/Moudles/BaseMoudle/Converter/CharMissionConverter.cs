//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : CharMissionConverter
//
// Created by : Baoxue at 2015/12/17 11:40:01
//
//
//========================================================================

using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Converter
{
    public class CharMissionConverter : ICharDataConverter
    {
        public Thrift.Protocol.TBase Convert(Character.AbstractBusinessObject o)
        {
            if (null == o)
            {
                return null;
            }
            CharMissionData data = o as CharMissionData;
            CharMissionInfo info = new CharMissionInfo();
            info.CharId = data.CharId;
            info.CharMissionInfoList = new List<MissionInfo>();
            for (int i = 0; i < data.MissionList.Count; ++i)
            {
                info.CharMissionInfoList.Add(DeepCopy(data.MissionList[i]));
            }
            return info;
        }
        public Character.AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            if (null == o)
            {
                return null;
            }
            CharMissionInfo info = o as CharMissionInfo;
            CharMissionData data = new CharMissionData(info.CharId);
            var missionList = new List<MissionInfo>();

            if (info.CharMissionInfoList != null)
            {
                for (int i = 0; i < info.CharMissionInfoList.Count; ++i)
                {
                    missionList.Add(DeepCopy(info.CharMissionInfoList[i]));
                }
            }
            data.MissionList = missionList;
            data.Init = false;
            return data;
        }
        public static MissionInfo DeepCopy(MissionInfo source)
        {
            MissionInfo elem = new MissionInfo();
            elem.Counter = source.Counter;
            elem.MissionId = source.MissionId;
            elem.MissionStepInfoList = new List<MissionStepInfo>();
            for (int i = 0; i < source.MissionStepInfoList.Count; ++i)
            {
                MissionStepInfo info = DeepCopy(source.MissionStepInfoList[i]);
                elem.MissionStepInfoList.Add(info);
            }
            return elem;
        }
        public static MissionStepInfo DeepCopy(MissionStepInfo source)
        {
            MissionStepInfo elem = new MissionStepInfo();
            elem.Counter = source.Counter;
            elem.StepId = elem.StepId;
            return elem;
        }
    }
}
