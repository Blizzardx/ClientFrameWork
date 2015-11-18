using Communication;
using Moudles.BaseMoudle.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Cache
{
    public class CharacterDataSnapshotTranscoder : ITranscoder
    {
        private const byte FLAG_END = 0;
        private const byte FLAG_VERSION = 1;
        private const byte FLAG_DATA_LIST = 2;
        public void Encode(NetWork.ByteBuffer buffer, object value)
        {
            CharacterDataSnapshot data = value as CharacterDataSnapshot;
            buffer.WriteByte(FLAG_VERSION);
            buffer.WriteLong(data.Version);
            buffer.WriteByte(FLAG_DATA_LIST);

            int size = data.DataList != null ? data.DataList.Count : 0;
            buffer.WriteShort((short)size);
            if (size > 0)
            {
                foreach (TBase tbase in data.DataList)
                {
                    buffer.WriteString(tbase.GetType().FullName);
                    byte[] tbaseBytes = ThriftSerialize.Serialize(tbase);
                    buffer.WriteInt(tbaseBytes.Length);
                    buffer.WriteBytes(tbaseBytes);
                }
            }

            buffer.WriteByte(FLAG_END);
        }

        public object Decode(NetWork.ByteBuffer buffer)
        {
            CharacterDataSnapshot data = new CharacterDataSnapshot();

            byte flag = FLAG_END;
            do{
                flag = buffer.ReadByte();

                switch (flag)
                {
                    case FLAG_VERSION:
                        data.Version = buffer.ReadLong();
                        break;
                    case FLAG_DATA_LIST:
                        {
                            int size = buffer.ReadShort();
                            if (size == 0)
                            {
                                continue;
                            }
                            List<TBase> list = new List<TBase>(size);
                            data.DataList = list;
                            for (int i = 0; i < size; i++)
                            {
                                string className = buffer.ReadString();
                                byte[] tbaseBytes = new byte[buffer.ReadInt()];
                                buffer.ReadBytes(tbaseBytes, 0, tbaseBytes.Length);
                                TBase tbase = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className, false) as TBase;
                                ThriftSerialize.DeSerialize(tbase, tbaseBytes);
                                list.Add(tbase);
                            }
                            break;
                        }
                }

            }while(flag != FLAG_END);

            return data;
        }
    }
}
