using Moudles.BaseMoudle.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Moudles.BaseMoudle.Converter
{
    public interface ICharDataConverter
    {
        TBase Convert(AbstractBusinessObject o);

        AbstractBusinessObject Convert(TBase o);
    }
}
