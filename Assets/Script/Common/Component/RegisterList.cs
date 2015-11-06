using UnityEngine;
using System.Collections.Generic;
using System;

public class RegisterList
{
	private List<Action>    m_UpdateList;
    private List<Action>    m_UnregisterUpdateListStore;
    private bool            m_bIsUpdateListBusy;

    #region public interface
    public RegisterList()
    {
        m_UpdateList = new List<Action>();
        m_UnregisterUpdateListStore = new List<Action>();
        m_bIsUpdateListBusy = false;
    }
    public void RegisterToUpdateList(Action element)
    {
        for (int i = 0; i < m_UpdateList.Count; ++i)
        {
            if (element == m_UpdateList[i])
            {
                CheckRemovingStore(element);
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
    public void ExcutionUpdateList()
    {
        m_bIsUpdateListBusy = true;
        foreach (Action elem in m_UpdateList)
        {
            elem();
        }
        m_bIsUpdateListBusy = false;
        ExcutionUnregister();
    }
    #endregion

    #region system function
    private void CheckRemovingStore(Action element)
    {
        for (int i = 0; i < m_UnregisterUpdateListStore.Count; ++i)
        {
            if (element == m_UnregisterUpdateListStore[i])
            {
                m_UnregisterUpdateListStore.RemoveAt(i);
                return;
            }
        }
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
    #endregion
}
