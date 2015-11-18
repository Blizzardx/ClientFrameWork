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

        //hp
        private int hp;

        public int Hp
        {
            get { return hp; }
            set
            {
                hp = value;
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

        //attack
        private int attack;

        public int Attack
        {
            get { return attack; }
            set
            {
                attack = value;
                SetModify();
            }
        }

        //money
        private int gold;

        public int Gold
        {
            get { return gold; }
            set
            {
                gold = value;
                SetModify();
            }
        }
        public override void CheckValid()
        {

        }
    }
}
