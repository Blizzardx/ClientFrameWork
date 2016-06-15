using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Framework.Event;
using Framework.Tick;

public class ModelBase
{
    protected EventDispatchTool m_EventHandler;

    public ModelBase()
    {
        m_EventHandler = new EventDispatchTool();
    }
    public void RegisterEvent(int eventId, Action<EventElement> callBack)
    {
        m_EventHandler.RegistEvent(eventId,callBack);
        CustomTickTask.Instance.RegisterToUpdateList(m_EventHandler.Update);
    }
    public void UnregisterEvent(int eventId, Action<EventElement> callBack)
    {
        m_EventHandler.UnregistEvent(eventId, callBack);
        if (m_EventHandler.GetCallbackListCount() == 0)
        {
            CustomTickTask.Instance.UnRegisterFromUpdateList(m_EventHandler.Update);
        }
    }
    protected void BroadcastEvent(int eventId, object param)
    {
        m_EventHandler.Broadcast(eventId,param);
    }
    protected void BroidcastEventAsync(int eventId, object param)
    {
        m_EventHandler.BroadcastAsync(eventId, param);
    }
    public virtual void OnCreate()
    {
        // do sth
    }
}
