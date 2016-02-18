//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : CharBagConverter
//
// Created by : Baoxue at 2015/11/30 18:03:55
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
    public class CharBagConverter : ICharDataConverter
    {
        public Thrift.Protocol.TBase Convert(Character.AbstractBusinessObject o)
        {
            if (null == o)
            {
                return null;
            }
            CharBagData data = o as CharBagData;
            CharBagInfo info = new CharBagInfo();
            info.CharId = data.CharId;
            info.ItemInfoList = data.ItemInfoList;
            return info;
        }

        public Character.AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            if (null == o)
            {
                return null;
            }
            CharBagInfo info = o as CharBagInfo;
            CharBagData data = new CharBagData(info.CharId,info.ItemInfoList);

            data.Init = false;
            return data;
        }
    }
}
