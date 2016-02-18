using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Character
{
    public class CharBaseData : AbstractBusinessObject
    {
        //char id
        private int charId;

        public int CharId
        {
            get { return charId; }
            set { this.charId = value; }
        }

        //char name
        private string charName;

        public string CharName
        {
            get { return charName; }
            set
            {
                this.charName = value;
                SetModify();
            }
        }

        //char sex
        private sbyte gender;

        public sbyte Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                SetModify();
            }
        }

        //char age
        private sbyte charAge;

        public sbyte CharAge
        {
            get { return charAge; }
            set
            {
                charAge = value;
                SetModify();
            }
        }

        //Fame
        private int friendly;

        public int Fame
        {
            get { return friendly; }
            set
            {
                friendly = value;
                SetModify();
            }
        }

       
        //level
        private short level;

        public short Level
        {
            get { return level; }
            set
            {
                level = value;
                SetModify();
            }
        }

        //talent map
        private Dictionary<string, int> charTalentMap;

        public Dictionary<string, int> CharTalentMap
        {
            get { return charTalentMap; }
            set
            {
                charTalentMap = value;
                SetModify();
            }
        }

        private sbyte charRole;

        public sbyte CharRole
        {
            get { return charRole;}
            set
            {
                charRole = value;
                SetModify();
            }
        }

        private byte[] charDetail;
        public byte[] CharDeatail
        {
            get { return charDetail; }
            set
            {
                charDetail = value;
                SetModify();
            }
        }
        public override void CheckValid()
        {

        }
    }
}
