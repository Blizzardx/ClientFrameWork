using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	public class BTRoot : BTNode
	{
		private int		m_iID;
		public  bool	active{ get; set; }

		public BTRoot( int iId ) : base()
		{
			m_iID = iId;
		}

		public override void Active (BTDatabase database)
		{
			base.Active( database );
			active = true;
		}

		//
		public override EBTState Tick ()
		{
			if( m_ChildrenLst.Count < 1 )
			{
				return EBTState.False;
			}
			return m_ChildrenLst[0].Tick();
		}
		
		//
		public override void OnEnd ()
		{
			
		}

		public void OnResertAI()
		{
			if( m_ChildrenLst.Count < 1 )
			{
				return ;
			}
			m_ChildrenLst[0].Resert();
		}
		//
		public int ID
		{
			get{ return m_iID; }
		}
	}
}