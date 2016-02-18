using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
	/// <summary>
	/// 行为树节点基类.
	/// </summary>
	public abstract class BTNode 
	{
		protected BTDatabase	m_Database;
		public BTDatabase		Database{ get{ return m_Database; } }
        protected List<BTNode> m_ChildrenLst = new List<BTNode>();

        public abstract EBTState Tick();
        public abstract void OnEnd();
		public virtual void Resert()
		{
			//m_eActionState = EBTActionState.Ready;
			foreach(BTNode node in m_ChildrenLst)
			{
				node.Resert();
			}
		}
		public virtual void Active( BTDatabase database )
		{
			m_Database = database;
			foreach( BTNode node in m_ChildrenLst )
			{
				node.Active( database );
			}
		}
		public virtual void AddChild( BTNode node )
		{
			if( null == node )
			{
				return;
			}

			m_ChildrenLst.Add( node );
		}
		public virtual void RemoveChild( BTNode node )
		{
			if( null == node )
			{
				return;
			}

			m_ChildrenLst.Remove( node );
        }

        #region debuger
	    public List<BTNode> GetChildList()
	    {
	        return m_ChildrenLst;
	    }
        public EBTState CurrentStatus;
	    public void ResetStatus()
	    {
	        CurrentStatus = EBTState.UnReach;
	        foreach (var elem in m_ChildrenLst)
	        {
	            elem.ResetStatus();
	        }
	    }
        #endregion
    }
}