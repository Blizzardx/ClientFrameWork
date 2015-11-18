using Moudles.BaseMoudle.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Moudles.BaseMoudle.Converter
{
    public class ConverterManager : Singleton<ConverterManager>
    {
        private Dictionary<System.Type, ICharDataConverter> dic = new Dictionary<System.Type, ICharDataConverter>();
        public void Initialize()
        {
            dic.Add(typeof(CharBaseData), new CharBaseConverter());
            dic.Add(typeof(CharCounterData), new CharCounterConverter());
        }

        public ICharDataConverter FindConverter(System.Type businessObjectType)
        {
            if (!dic.ContainsKey(businessObjectType))
            {
                return null;
            }
            return dic[businessObjectType];
        }

    }
}
