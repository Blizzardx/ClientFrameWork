using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Character
{
    public class CharCounterData : AbstractBusinessObject
    {
        private int charId;
        public int CharId
        {
            get { return charId; }
        }

        private List<sbyte> bit8CounterList;

        public List<sbyte> Bit8CounterList
        {
            get { return bit8CounterList; }
            set
            {
                bit8CounterList = value;
                SetModify();
            }
        }
        private List<int> bit32CounterList;

        public List<int> Bit32CounterList
        {
            get { return bit32CounterList; }
            set
            {
                bit32CounterList = value;
                SetModify();
            }
        }

        private List<bool> flagList;

        public List<bool> FlagList
        {
            get { return flagList; }
            set
            {
                flagList = value;
                SetModify();
            }
        }


        public CharCounterData(int charId)
        {
            this.charId = charId;
        }

        public override void CheckValid()
        {
            if (bit8CounterList == null)
            {
                bit8CounterList = new List<sbyte>();
            }
            if (bit32CounterList == null)
            {
                bit32CounterList = new List<int>();
            }
            if (flagList == null)
            {
                flagList = new List<bool>();
            }
        }

        public void SetFlag(int flagId, bool flag)
        {
            if (flagList.Count - 1 < flagId)
            {
                int num = flagId - (flagList.Count - 1);
                for (int i = 0; i < num; i++)
                {
                    flagList.Add(false);
                }
            }
            bool source = flagList[flagId];
            if (source != flag)
            {
                flagList[flagId] = flag;
                SetModify();
            }
        }

        public bool GetFlag(int flagId)
        {
            if (flagList.Count - 1 < flagId)
            {
                return false;
            }
            return flagList[flagId];
        }

        public void SetBit8Count(int counterId, sbyte count)
        {
            sbyte source = GetBit8Count(counterId);
            if (source != count)
            {
                bit8CounterList[counterId] = count;
                SetModify();
            }
        }

        public sbyte GetBit8Count(int counterId)
        {
            int num = counterId - (bit8CounterList.Count - 1);
            if (bit8CounterList.Count - 1 < counterId)
            {
                for (int i = 0; i < num; i++)
                {
                    bit8CounterList.Add((sbyte)0);
                }
            }
            return bit8CounterList[counterId];
        }

        public void SetBit32Count(int counterId, int count)
        {
            int source = GetBit32Count(counterId);
            if (source != count)
            {
                bit32CounterList[counterId] = count;
                SetModify();
            }
        }

        public int GetBit32Count(int counterId)
        {
            if (bit32CounterList.Count - 1 < counterId)
            {
                int num = counterId - (bit32CounterList.Count - 1);
                for (int i = 0; i < num; i++)
                {
                    bit32CounterList.Add(0);
                }
            }
            return bit32CounterList[counterId];
        }
    }
}
