using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 选择节点.
	/// </summary>
	public class Selector : BTComposites
    {
        protected EBTState  m_CurrentStatus;
	    private int         m_iCurrentIndex;

	    public override EBTState Tick()
	    {
	        if (!m_Database.GetData<bool>(EDataBaseKey.IsLock))
	        {
	            m_iCurrentIndex = 0;
	        }
	        do
	        {
	            bool isBreak = false;
	            for (; m_iCurrentIndex < m_ChildrenLst.Count; ++m_iCurrentIndex)
	            {
	                m_CurrentStatus = m_ChildrenLst[m_iCurrentIndex].Tick();

                    //mark status
                    SetChildElementStatus(EBTState.UnReach, m_iCurrentIndex);

	                if (m_CurrentStatus != EBTState.False)
	                {
	                    isBreak = true;
                        break;
	                }
	            }
	            if (isBreak)
	            {
	                break;
	            }
	            m_iCurrentIndex = 0;
	            m_CurrentStatus = EBTState.False;
	        } while (false);

            for (int i = m_iCurrentIndex; i < m_ChildrenLst.Count; ++i)
            {
                //mark status
                SetChildElementStatus(EBTState.UnReach, i);
            }
            CurrentStatus = m_CurrentStatus;

	        return m_CurrentStatus;
	    }
	    public override void OnEnd()
	    {
	        foreach (var elem in m_ChildrenLst)
	        {
	            elem.OnEnd();
	        }
	    }
	}
}