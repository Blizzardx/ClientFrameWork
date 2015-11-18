using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Converter
{
    public class CharBaseConverter : ICharDataConverter
    {
        public Thrift.Protocol.TBase Convert(AbstractBusinessObject o)
        {
            CharBaseData data = o as CharBaseData;
            CharBaseInfo info = new CharBaseInfo();
            info.CharId = data.CharId;
            info.CharName = data.CharName;
            info.Age = data.CharAge;
            info.Atk = data.Attack;
            info.Fame = data.Fame;
            info.Gold = data.Gold;
            info.Hp = data.Hp;
            info.Level = data.Level;
            info.Gender = data.Gender;

            return info;
        }

        public AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            CharBaseInfo info = o as CharBaseInfo;
            CharBaseData data = new CharBaseData();
            data.CharId = info.CharId;
            data.CharName = info.CharName;
            data.CharAge = info.Age;
            data.Attack = info.Atk;
            data.Fame = info.Fame;
            data.Gold = info.Gold;
            data.Hp = info.Hp;
            data.Level = info.Level;
            data.Gender = info.Gender;

            data.Init = false;
            return data;
        }
    }
}
