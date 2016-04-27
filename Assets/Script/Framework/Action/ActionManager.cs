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

public class ActionParam
{
    private int _id;
    private object _object;
    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            this._id = value;
        }
    }
    public object Object
    {
        get
        {
            return _object;
        }
        set
        {
            this._object = value;
        }
    }
}
public class ActionManager : Singleton<ActionManager>
{
    private List<int> m_PlayedActionList = new List<int>();
    private List<ActionPlayer> m_lstAction = new List<ActionPlayer>();
    private const int m_iPlayedActionListMacCount = 1024;

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
    public int InsertAction(int iActionId, ActionFileData data, ActionParam param)
    {
        ActionPlayer action = new ActionPlayer(iActionId, data, param, null);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    }
    public int InsertAction(int iActionId, ActionFileData data, ActionParam param, List<GameObject> affectedObjects)
    {
        ActionPlayer action = new ActionPlayer(iActionId, data, param, affectedObjects);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    }
    public int InsertAction(int iActionId, ActionFileData data, ActionParam param, params GameObject[] affectedObjects)
    {
        List<GameObject> affectedOjectList = new List<GameObject>();
        affectedOjectList.AddRange(affectedObjects);
        ActionPlayer action = new ActionPlayer(iActionId, data, param, affectedOjectList);
        m_lstAction.Add(action);
        return action.GetInstanceID();
    }
    public int PlayAction(int iActionId, ActionParam param)
    {
        ActionFileData data = ConfigManager.Instance.GetActionFileData(iActionId);
        ActionPlayer action = new ActionPlayer(iActionId, data, param, null);
        m_lstAction.Add(action);
        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_ACTION_START, param));
        return action.GetInstanceID();
    }
    public int PlayAction(int iActionId, ActionParam param, List<GameObject> affectedObjects)
    {
        ActionFileData data = ConfigManager.Instance.GetActionFileData(iActionId);
        ActionPlayer action = new ActionPlayer(iActionId, data, param, affectedObjects);
        m_lstAction.Add(action);
        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_ACTION_START, param));
        return action.GetInstanceID();
    }
    public int PlayAction(int iActionId, ActionParam param, params GameObject[] affectedObjects)
    {
        ActionFileData data = ConfigManager.Instance.GetActionFileData(iActionId);
        List<GameObject> affectedOjectList = new List<GameObject>();
        affectedOjectList.AddRange(affectedObjects);
        ActionPlayer action = new ActionPlayer(iActionId, data, param, affectedOjectList);
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
                m_lstAction[i].Destory();
                m_lstAction.RemoveAt(i);
                return;
            }
        }
    }
    public void ClearAction()
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
            m_lstAction[i].Destory();
        }
        m_lstAction.Clear();
    }
    public void Update()
    {
        if (null == m_lstAction || m_lstAction.Count <= 0)
        {
            return;
        }


        int nCount = m_lstAction.Count;

        for (int i = nCount - 1; i >= 0; i--)
        {
            ActionPlayer action = m_lstAction[i];

            if (null == action)
            {
                m_lstAction.RemoveAt(i);
                AddToEndPlayList(action.GetActionId());
                continue;
            }

            action.Update();

            ActionPlayer.EActionState eState = action.GetActionState();
            if (eState == ActionPlayer.EActionState.Stop)
            {
                action.Destory();
                m_lstAction.RemoveAt(i);
                AddToEndPlayList(action.GetActionId());
                continue;
            }
            if (action.IsFinish())
            {
                action.Destory();
                m_lstAction.RemoveAt(i);
                AddToEndPlayList(action.GetActionId());
                continue;
            }
        }
    }
    public void Initialize()
    {
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_PLAY_ACTION, OnTriggerPlayAction);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_CHANGE_SCENE, OnChangeScene);
    }
    public void Distructor()
    {
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_PLAY_ACTION, OnTriggerPlayAction);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_CHANGE_SCENE, OnChangeScene);
    }
    public bool CheckActionIsPlayed(int id)
    {
        for (int i = 0; i < m_PlayedActionList.Count; ++i)
        {
            if (m_PlayedActionList[i] == id)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region System Functions
    private void AddToEndPlayList(int id)
    {
        for (int i = 0; i < m_PlayedActionList.Count; ++i)
        {
            if (m_PlayedActionList[i] == id)
            {
                return;
            }
        }
        m_PlayedActionList.Add(id);
        while (m_PlayedActionList.Count > m_iPlayedActionListMacCount)
        {
            m_PlayedActionList.RemoveAt(0);
        }
    }
    private void OnTriggerPlayAction(MessageObject obj)
    {
        // trigger to play action
        if (!(obj.msgValue is int))
        {
            return;
        }

        //get id
        int id = (int)(obj.msgValue);

        //get current scene gameobject
        var objList = LifeManager.GetSceneObjList();
        ActionParam param = new ActionParam();
        param.Id = id;
        //try play  action
        PlayAction(id, param, objList);
    }
    private void OnChangeScene(MessageObject obj)
    {
        ClearAction();
    }
    #endregion
}

