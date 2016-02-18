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
            info.Fame = data.Fame;
            info.Level = data.Level;
            info.Gender = data.Gender;
            //leo
            info.CharTalentMap = data.CharTalentMap;
            info.Role = data.CharRole;
            info.CharDeail = data.CharDeatail;
            return info;
        }

        public AbstractBusinessObject Convert(Thrift.Protocol.TBase o)
        {
            CharBaseInfo info = o as CharBaseInfo;
            CharBaseData data = new CharBaseData();
            data.CharId = info.CharId;
            data.CharName = info.CharName;
            data.CharAge = info.Age;
            data.Fame = info.Fame;
            data.Level = info.Level;
            data.Gender = info.Gender;
            //leo
            data.CharTalentMap = info.CharTalentMap;
            data.CharRole = info.Role;
            data.CharDeatail = info.CharDeail;
            data.Init = false;
            return data;
        }
    }
}
