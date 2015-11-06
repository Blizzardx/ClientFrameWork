

using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
	/// <summary>
	/// 行为树黑板.
	/// </summary>
	public class BTDatabase
	{
		private Dictionary<EDataBaseKey,object>		m_Data = new Dictionary<EDataBaseKey, object>();

		/// <summary>
		/// 
		/// </summary>
		public T GetData<T>( EDataBaseKey eKey )
		{
			if( m_Data.ContainsKey( eKey ) )
			{
				return (T)m_Data[eKey];
			}

			return default(T);
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetData<T>( EDataBaseKey eKey, T Data )
		{
			if( null == GetData<T>( eKey ) )
			{
				m_Data.Add( eKey, Data );
			}
			else
			{
				m_Data[eKey] = Data;
			}
		}
	}
}