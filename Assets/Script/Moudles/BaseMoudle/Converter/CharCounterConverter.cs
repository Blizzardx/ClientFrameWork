using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Converter
{
    public class CharCounterConverter : ICharDataConverter
    {
        public Thrift.Protocol.TBase Convert(Character.AbstractBusinessObject o)
        {
            if (null == o)
            {
                return null;
            }
            CharCounterData data = o as CharCounterData;
            CharCounterInfo info = new CharCounterInfo();
            info.CharId = data.CharId;
            if (null != data.Bit32CounterList)
            {
                info.Bit32CounterList = new List<int>(data.Bit32CounterList);
            }            
            if(null != data.Bit8CounterList)
            {
                info.Bit8CounterList = new List<sbyte>(data.Bit8CounterList);
            }
            if(null != data.FlagList)
            {
                info.FlagList = new List<bool>(data.FlagList);
            }
            

            return info;
        }

        public Character.AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            if (null == o)
            {
                return null;
            }
            CharCounterInfo info = o as CharCounterInfo;
            CharCounterData data = new CharCounterData(info.CharId);
            if (null != info.Bit8CounterList)
            {
                data.Bit8CounterList = new List<sbyte>(info.Bit8CounterList);
            }
            if (null != info.Bit32CounterList)
            {
                data.Bit32CounterList = new List<int>(info.Bit32CounterList);
            }
            if (null != info.FlagList)
            {
                data.FlagList = new List<bool>(info.FlagList);
            }
            data.Init = false;
            return data;
        }
    }
}
