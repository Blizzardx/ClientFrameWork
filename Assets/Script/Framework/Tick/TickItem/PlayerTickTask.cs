using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerTickTask : AbstractTickTask
{
    private List<Action>    m_UpdateList;
    private List<Action>    m_UnregisterUpdateListStore;
    private bool            m_bIsUpdateListBusy;

    public PlayerTickTask()
    {
        m_UpdateList = new List<Action>();
        m_UnregisterUpdateListStore = new List<Action>();
        m_bIsUpdateListBusy = false;
        m_Instance = this;
    }
    private static PlayerTickTask m_Instance;
    public static PlayerTickTask Instance
    {
        get
        {
            return m_Instance;
        }
    }
    public void RegisterToUpdateList(Action element)
    {
        for (int i = 0; i < m_UpdateList.Count; ++i)
        {
            if (element == m_UpdateList[i])
            {
                return;
            }
        }
        m_UpdateList.Add(element);
    }
    public void UnRegisterFromUpdateList(Action element)
    {
        if (!m_bIsUpdateListBusy)
        {
            m_UpdateList.Remove(element);
        }
        else
        {
            for (int i = 0; i < m_UnregisterUpdateListStore.Count; ++i)
            {
                if (element == m_UnregisterUpdateListStore[i])
                {
                    return;
                }
            }
            m_UnregisterUpdateListStore.Add(element);
        }
    }
    protected override bool FirstRunExecute()
    {
       
        return true;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_PLAYER;
    }
    protected override void Beat()
    {
        ExcutionUpdateList();
    }
    private void ExcutionUpdateList()
    {
        m_bIsUpdateListBusy = true;
        foreach (Action elem in m_UpdateList)
        {
            elem();
        }
        m_bIsUpdateListBusy = false;
        ExcutionUnregister();
    }
    private void ExcutionUnregister()
    {
        if (m_UnregisterUpdateListStore.Count == 0)
        {
            return;
        }
        for (int i = 0; i < m_UnregisterUpdateListStore.Count; ++i)
        {
            m_UpdateList.Remove(m_UnregisterUpdateListStore[i]);
        }
        m_UnregisterUpdateListStore.Clear();
    }
}
