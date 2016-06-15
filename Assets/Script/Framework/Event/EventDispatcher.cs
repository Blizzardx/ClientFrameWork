using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Component;
using Common.Tool;
using Framework.Common;
using Framework.Message;
using Framework.Queue;
using UnityEngine;

namespace Framework.Event
{
    public class EventDispatcher:Singleton<EventDispatcher>
    {
        private RegisterDictionaryTemplate<EventElement>    m_MsgCallList;
        private TemplateQueue<EventElement>                 m_EventQueue;
         
        public EventDispatcher()
        {
            m_MsgCallList = new RegisterDictionaryTemplate<EventElement>();
            m_EventQueue = new TemplateQueue<EventElement>();
            m_EventQueue.Initialize();
        }
        public void Broadcast(int id, object param)
        {
            Broadcast(new EventElement(id, param));
        }
        public void Broadcast(EventElement eventBody)
        {
            m_MsgCallList.BeginUpdate();
            int errorId = 0;
            try
            {
                if (null != eventBody)
                {
                    errorId = eventBody.eventId;
                    m_MsgCallList.Update(eventBody.eventId, eventBody);
                }
            }
            catch (Exception e)
            {
                //log error
                Debug.LogError("Wrong msg callback" + errorId + "error log: " + e.Message);
            }
            m_MsgCallList.EndUpdate();
        }
        public void BroadcastAsync(int id, object param)
        {
            BroadcastAsync(new EventElement(id, param));
        }
        public void BroadcastAsync(EventElement eventBody)
        {
            if (!m_MsgCallList.IsContainsKey(eventBody.eventId))
            {
                return;
            }
            m_EventQueue.Enqueue(eventBody);
        }
        public void RegistEvent(int msgId, Action<EventElement> eventCallback)
        {
            m_MsgCallList.RegistEvent(msgId, eventCallback);
        }
        public void UnregistEvent(int msgId, Action<EventElement> eventCallback)
        {
            m_MsgCallList.UnregistEvent(msgId, eventCallback);
        }
        public void UnregistEventAll(int msgId)
        {
            m_MsgCallList.UnregistAllEvent(msgId);
        }
        public void Update()
        {
            m_MsgCallList.BeginUpdate();
            int errorId = 0;
            try
            {
                //process msglist message
                for (int i = 0; i < 16; ++i)
                {
                    var elem = m_EventQueue.Dequeue();
                    if (null == elem)
                    {
                        break;
                    }
                    errorId = elem.eventId;
                    m_MsgCallList.Update(elem.eventId, elem);
                }
            }
            catch (Exception e)
            {
                //log error
                Debug.LogError("Wrong msg callback" + errorId + "error log: " + e.Message);
            }
            m_MsgCallList.EndUpdate();
        }
    }
}
