//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionManager
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/2 19:17:12
//
// Purpose : Action管理器
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using ActionEditor;

public class ActionManager : Singleton<ActionManager>
{
    #region Field
    private List<ActionPlayer> m_lstAction = new List<ActionPlayer>();
    #endregion

    #region Public Interface
    public ActionPlayer GetAction(int instanceID)
    {
        if (null == m_lstAction)
        {
            return null;
        }

        foreach (ActionPlayer action in m_lstAction)
        {
            if (null == action)
            {
                continue;
            }

            if (action.GetInstanceID() == instanceID)
            {
                return action;
            }
        }
        return null;
    }
    public int InsertAction(int iActionId, ActionFileData data)
    {
        ActionPlayer action = new ActionPlayer(iActionId, data, null);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    } 
    public int InsertAction(int iActionId, ActionFileData data, List<GameObject> affectedObjects)
    {
        ActionPlayer action = new ActionPlayer(iActionId, data, affectedObjects);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    } 
    public int InsertAction(int iActionId, ActionFileData data, params GameObject[] affectedObjects)
    {
        List<GameObject> affectedOjectList = new List<GameObject>();
        affectedOjectList.AddRange(affectedObjects);
        ActionPlayer action = new ActionPlayer(iActionId, data, affectedOjectList);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    }
    public void RemoveAction(int instanceID)
    {
        if (null == m_lstAction)
        {
            return;
        }

        for (int i = 0; i < m_lstAction.Count; i++)
        {
            if (null == m_lstAction[i])
            {
                continue;
            }

            if (m_lstAction[i].GetInstanceID() == instanceID)
            {
                m_lstAction.RemoveAt(i);
                return;
            }
        }
    }
    public void ClearAction()
    {
        m_lstAction.Clear();
    }
    public void Update()
    { 
        if (null == m_lstAction || m_lstAction.Count <= 0)
		{
			return ;
		}
		
		int nCount = m_lstAction.Count;

        for (int i = nCount - 1; i >= 0; i--)
        {
            ActionPlayer action = m_lstAction[i];

            if (null == action)
            {
                m_lstAction.RemoveAt(i);
                continue;
            }

            action.Update();

            ActionPlayer.EActionState eState = action.GetActionState();
            if (eState == ActionPlayer.EActionState.Stop)
            {
                action.Destory();
                m_lstAction.RemoveAt(i);
                continue;
            }
            if (action.IsFinish())
            {
                action.Destory();
                m_lstAction.RemoveAt(i);
                continue;
            }
        }
    }
    #endregion

    #region System Functions

    #endregion
}

