using System;
using Config;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;

public class StateMachine
{
    private IState                                  m_CurrentState;
    private Dictionary<ELifeState, IState>          m_StateUsingStore;
    static private Dictionary<ELifeState, Type>     m_StateFactory;
    private int                                     m_nCurrentListenId;
    //private StateConflictConfig                     m_CurrentCharStateConflictMap;
    private Ilife m_lifeInstance;

    #region public interface
    public StateMachine(int uid,int registerClientMsgId,Ilife lifeInstance )
    {
        m_lifeInstance = lifeInstance;
        m_nCurrentListenId = registerClientMsgId;
        MessageManager.Instance.RegistMessage(registerClientMsgId, OnTriggerChangeState);
        //m_CurrentCharStateConflictMap = ConfigManager.Instance.GetStateConflicMap(uid);
    }
    static public void RegisterState(ELifeState state, Type type)
    {
        if (null == m_StateFactory)
        {
            m_StateFactory = new Dictionary<ELifeState, Type>();
        }
        m_StateFactory.Add(state, type);
    }
    public bool TryEnterState(ELifeState newStateID, bool force,object param = null)
    {
        if (null != m_CurrentState && m_CurrentState.GetState() == newStateID)
        {
            //do nothing
            return true;
        }
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
            m_CurrentState.DoEnter(param);
            return true;
        }
        if (!force )
        {
            List<StateConflictConfigElement> stateClash = null;
            /*if (m_CurrentCharStateConflictMap.StateConflictMap.TryGetValue((int)(m_CurrentState.GetState()), out stateClash))
            {
                foreach (StateConflictConfigElement s in stateClash)
                {
                    //与当前状态冲突
                    if (s.StateId == (int)newStateID)
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
            }*/

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
        m_CurrentState.DoEnter(param);
        return true;
    }
    public void Distructor()
    {
        if (null != m_StateUsingStore)
        {
            m_StateUsingStore.Clear();
        }
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
        if (null != m_StateUsingStore && m_StateUsingStore.TryGetValue(stateId, out result))
        {
            return result;
        }
        result = CreateStateInstance(stateId, m_lifeInstance);
        if (null == m_StateUsingStore)
        {
            m_StateUsingStore = new Dictionary<ELifeState, IState>();
        }
        m_StateUsingStore.Add(stateId, result);
        
        return result;
    }
    private static IState CreateStateInstance(ELifeState stateId, Ilife unit)
    {
        if (null == m_StateFactory)
        {
            StateDefine.RegisterState();
        }
        IState result = null;
        Type type = null;
        if (m_StateFactory.TryGetValue(stateId, out type))
        {
            result = Activator.CreateInstance(type, unit,stateId) as IState;
        }
        return result;
    }
    #endregion
}
