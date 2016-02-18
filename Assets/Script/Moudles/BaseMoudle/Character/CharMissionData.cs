//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : CharMissionData
//
// Created by : Baoxue at 2015/12/17 11:36:27
//
//
//========================================================================

using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Character
{
    public class CharMissionData : AbstractBusinessObject
    {
        private int m_iCharId;
        public int CharId
        {
            get
            {
                return m_iCharId;
            }
            set
            {
                m_iCharId = value;
                SetModify();
            }
        }
        public CharMissionData(int charId)
        {
            m_iCharId = charId;
        }

        private List<MissionInfo> m_MissionList;

        public List<MissionInfo> MissionList
        {
            get
            {
                return m_MissionList;
            }
            set
            {
                m_MissionList = value;
                SetModify();
            }
        }
        public override void CheckValid()
        {

        }
    }

}