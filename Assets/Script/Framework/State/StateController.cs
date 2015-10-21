using Common.Config;
using UnityEngine;
using System.Collections.Generic;

public class StateController
{
    private IState m_CurrentState;

    public bool TryEnterState(IState newState, bool force)
    {
        ELifeState newStateID = newState.GetState();

        if (!force)
        {
            List<StateConflictConfigElement> stateClash = ConfigManager.Instance.GetStateConflicList(newStateID);
            foreach (StateConflictConfigElement s in stateClash)
            {
                //与当前状态冲突
                if (s.StateId == (int)newStateID )
                {
                    if (s.IsConflict)
                    {
                        Debuger.LogWarning(string.Format("current state:{0} can't enter new state:{1}",
                            m_CurrentState.GetState(), newStateID));
                        return false;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!m_CurrentState.CanExit())
            {
                return false;
            }

            if (!newState.CanEnter())
            {
                return false;
            }
        }

        m_CurrentState.DoExit();

        //reset state
        m_CurrentState = newState;
        m_CurrentState.DoEnter();
        return true;
    }
}
