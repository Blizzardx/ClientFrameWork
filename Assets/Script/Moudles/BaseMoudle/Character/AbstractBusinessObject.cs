using UnityEngine;
using System.Collections;

namespace Moudles.BaseMoudle.Character
{
    public abstract class AbstractBusinessObject
    {
        private bool modify = false;
        public bool Init = true;

        public abstract void CheckValid();

        public void ResetModify()
        {
            this.modify = false;
        }
        public bool IsModify()
        {
            return this.modify;
        }

        protected void SetModify()
        {
            if (Init)
            {
                return;
            }
            if (this.modify)
            {
                return;
            }
            this.modify = true;
            UpdateManager.Instance.Offer(this);
        }
    }
}