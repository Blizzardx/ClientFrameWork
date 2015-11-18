using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moudles.BaseMoudle.Character
{
    /// <summary>
    /// 线程不安全的更新器，只能在主线程中使用
    /// </summary>
    public class UpdateManager : Singleton<UpdateManager>
    {
        private LinkedList<AbstractBusinessObject> list = new LinkedList<AbstractBusinessObject>();
        private Dictionary<System.Type, AbstractBusinessObject> dic = new Dictionary<Type, AbstractBusinessObject>();

        public void Offer(AbstractBusinessObject obj)
        {
            if (dic.ContainsKey(obj.GetType()))
            {
                return;
            }
            dic.Add(obj.GetType(), obj);
            list.AddLast(obj);
        }

        public AbstractBusinessObject Poll()
        {
            if (list.Count == 0)
            {
                return null;
            }
            AbstractBusinessObject obj = list.First.Value;
            list.RemoveFirst();
            dic.Remove(obj.GetType());
            return obj;
        }
    }
}
