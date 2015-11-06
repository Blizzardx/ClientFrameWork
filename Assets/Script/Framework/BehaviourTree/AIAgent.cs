using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
	public class AIAgent
	{
		private BTRoot m_Root;

		public AIAgent( int iId )
		{
			m_Root = BehaviourTreeParser.Instance.CreateBehaviourTree( iId );
		}

		//
		public void Active( bool bActive, Ilife Owner )
		{
			if( null == m_Root )
			{
				return;
			}

			if( bActive )
			{
				m_Root.Active( new BTDatabase() );
                m_Root.Database.SetData<Ilife>(EDataBaseKey.Owner, Owner);
			}
			else
			{
				m_Root.active = false;
			}
		}

		//
		public bool IsActive()
		{
			if( null != m_Root )
			{
				return m_Root.active;
			}
			return false;
		}

		//
		public void OnTick()
		{
			if( null == m_Root || !m_Root.active )
			{
				return;
			}

			m_Root.Tick();
		}

		public void OnResertAI()
		{
			if( null == m_Root || !m_Root.active )
			{
				return;
			}
			
			m_Root.OnResertAI();
		}
	}
}