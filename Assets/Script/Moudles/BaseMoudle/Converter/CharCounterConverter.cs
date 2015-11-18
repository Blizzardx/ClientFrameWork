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
            CharCounterData data = o as CharCounterData;
            CharCounterInfo info = new CharCounterInfo();
            info.CharId = data.CharId;

            return info;
        }

        public Character.AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            CharCounterInfo info = o as CharCounterInfo;
            CharCounterData data = new CharCounterData(info.CharId);

            data.Init = false;
            return data;
        }
    }
}
