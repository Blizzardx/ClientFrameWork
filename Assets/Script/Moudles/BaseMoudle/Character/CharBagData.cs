//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : CharBagData
//
// Created by : Baoxue at 2015/11/30 18:05:00
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

namespace Moudles.BaseMoudle.Character
{
    public class CharBagData : AbstractBusinessObject
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

        private List<ItemInfo> m_ItemInfoList;
        public List<ItemInfo> ItemInfoList
        {
            get
            {
                return m_ItemInfoList;
            }
            set
            {
                m_ItemInfoList = value;
                SetModify();
            }
        }
        
        public CharBagData(int charid, List<ItemInfo> itemInfoList)
        {
            CharId = charid;
            m_ItemInfoList = new List<ItemInfo>();
            for (int i = 0; itemInfoList != null && i < itemInfoList.Count; ++i)
            {
                m_ItemInfoList.Add(itemInfoList[i]);
            }
        }
        public override void CheckValid()
        {

        }
    }

}