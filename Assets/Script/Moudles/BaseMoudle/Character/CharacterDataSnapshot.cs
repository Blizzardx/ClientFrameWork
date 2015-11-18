using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Moudles.BaseMoudle.Character
{
    public class CharacterDataSnapshot
    {
        public long Version { get; set; }

        public List<TBase> DataList { get; set; }
    }
}
