using System;
using Common.Config;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;

public class StateController
{
    private IState                          m_CurrentState;
    private Dictionary<ELifeState, IState>  m_StateUsingStore;
    private Dictionary<ELifeState, Type>    m_StateFactory;
    private int                             m_nCurrentListenId;

    #region public interface
    public StateController(int registerClientMsgId)
    {
        m_nCurrentListenId = registerClientMsgId;
        MessageManager.Instance.RegistMessage(registerClientMsgId, OnTriggerChangeState);
    }
    public void RegisterState(ELifeState state, Type type)
    {
        if (null == m_StateFactory)
        {
            m_StateFactory = new Dictionary<ELifeState, Type>();
        }
        m_StateFactory.Add(state, type);
    }
    public bool TryEnterState(ELifeState newStateID, bool force)
    {
        IState newState = StateFactory(newStateID);
        if (null == newState)
        {
            return false;
        }

        if (null == m_CurrentState)
        {
            if (!newState.CanEnter())
            {
                return false;
            }
            //reset state
            m_CurrentState = newState;
            m_CurrentState.DoEnter();
            return true;
        }
        if (!force )
        {
            List<StateConflictConfigElement> stateClash = ConfigManager.Instance.GetStateConflicList(m_CurrentState.GetState());
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
    public void Distructor()
    {
        m_StateUsingStore.Clear();
        m_CurrentState = null;
        MessageManager.Instance.UnregistMessage(m_nCurrentListenId,OnTriggerChangeState);
    }
    #endregion

    #region system function
    private void OnTriggerChangeState(MessageObject obj)
    {
        if (!(obj.msgValue is ELifeState))
        {
            return;
        }
        ELifeState newState = (ELifeState)(obj.msgValue);
        TryEnterState(newState, false);
    }
    private IState StateFactory(ELifeState stateId)
    {
        IState result = null;
        if (m_StateUsingStore.TryGetValue(stateId, out result))
        {
            return result;
        }

        Type type = null;
        if (m_StateFactory.TryGetValue(stateId, out type))
        {
            result = Activator.CreateInstance(type) as IState;
            if (null == m_StateUsingStore)
            {
                m_StateUsingStore = new Dictionary<ELifeState, IState>();
            }
            m_StateUsingStore.Add(stateId, result);
        }
        return result;
    }
    #endregion
}
