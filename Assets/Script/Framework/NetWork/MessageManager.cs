using System;
using UnityEngine;
using System.Collections.Generic;

public class MessageObject
{
    public object   msgValue;
    public int      msgId;

    public MessageObject()
    {
    }
    public MessageObject(int msgId, object mv)
    {
        this.msgValue = mv;
        this.msgId = msgId;
    }
}
public class MessageManager : Singleton<MessageManager>
{
    private Dictionary<int, List<Action<MessageObject>>>        m_MsgCallbackStore;
    private List<MessageObject>                                 m_MsgList;
    private List<MessageObject>                                 m_DelayMsgList;
    private List<KeyValuePair<int, Action<MessageObject>>>      m_UnRegisterList; 
    private bool                                                m_bIsProcessingMsgList;

    public void Initialize()
    {
        m_MsgCallbackStore      = new Dictionary<int, List<Action<MessageObject>>>();
        m_MsgList               = new List<MessageObject>();
        m_DelayMsgList          = new List<MessageObject>();
        m_UnRegisterList        = new List<KeyValuePair<int,Action<MessageObject>>>();
        m_bIsProcessingMsgList  = false;

        //register message
        MessageDefine.Instance.RegisterMessage();
    }
    public void Update()
    {
        lock (this)
        {
            if (m_MsgList.Count == 0)
            {
                foreach (MessageObject eb in m_DelayMsgList)
                {
                    m_MsgList.Add(eb);
                }
                m_DelayMsgList.Clear();
                return;
            }
            m_bIsProcessingMsgList = true;

            //process msglist message
            foreach (MessageObject elem in m_MsgList)
            {
                try
                {
                    if (m_MsgCallbackStore.ContainsKey(elem.msgId))
                    {
                        foreach (Action<MessageObject> fun in m_MsgCallbackStore[elem.msgId])
                        {
                            if (null != fun)
                            {
                                fun(elem);
                            }
                            else
                            {
                                //log error                        
                                Debuger.LogError("null of call back fun" + elem.msgId.ToString());
                            }
                        }
                    }
                    else
                    {
                        //empty msg list                    
                        Debuger.LogError("empty msg list  " + elem.msgId.ToString());
                    }
                }
                catch (Exception e)
                {
                    //log error
                    Debuger.LogError("Wrong msg callback" + elem.msgId.ToString() + "error log: " + e.Message);
                }
            }
            m_MsgList.Clear();
            m_bIsProcessingMsgList = false;
            DoUnregister();
        }
    }
    public void AddToMessageQueue(MessageObject msgBody)
    {
        lock (this)
        {
            //process msg
            try
            {
                if (!m_MsgCallbackStore.ContainsKey(msgBody.msgId))
                {
                    return;
                }
                if (m_bIsProcessingMsgList)
                {
                    m_DelayMsgList.Add(msgBody);
                }
                else
                {
                    m_MsgList.Add(msgBody);
                }
            }
            catch
            {
                Debuger.LogError("Don't exit msg id " + msgBody.msgId.ToString());
            }
        }
    }
    public void RegistMessage(int msgId, Action<MessageObject> msgCallback)
    {
        if (null == msgCallback)
        {
            Debuger.LogError("msg call back can't be null !!!" + msgId.ToString());
        }
        if (!m_MsgCallbackStore.ContainsKey(msgId))
        {
            m_MsgCallbackStore.Add(msgId, new List<Action<MessageObject>>());
            m_MsgCallbackStore[msgId].Add(msgCallback);
        }
        else
        {
            for (int i = 0; i < m_MsgCallbackStore[msgId].Count; ++i)
            {
                if (m_MsgCallbackStore[msgId][i] == msgCallback)
                {
                    return;
                }
            }
            m_MsgCallbackStore[msgId].Add(msgCallback);
        }
    }
    public void UnregistMessage(int msgId, Action<MessageObject> msgCallback)
    {
        if (m_MsgCallbackStore.ContainsKey(msgId))
        {
            if (!m_bIsProcessingMsgList)
            {
                m_MsgCallbackStore[msgId].Remove(msgCallback);
            }
            else
            {
                for (int i = 0; i < m_UnRegisterList.Count; ++i)
                {
                    if (m_UnRegisterList[i].Key == msgId && m_UnRegisterList[i].Value == msgCallback)
                    {
                        return;
                    }
                }
                m_UnRegisterList.Add(new KeyValuePair<int, Action<MessageObject>>(msgId, msgCallback));
            }
        }
    }
    public void UnregistMessageAll(int msgId)
    {
        if (m_MsgCallbackStore.ContainsKey(msgId))
        {
            m_MsgCallbackStore[msgId].Clear();
        }
    }
    private void DoUnregister()
    {
        if (m_UnRegisterList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < m_UnRegisterList.Count; ++i)
        {
            List<Action<MessageObject>> callback = null;
            if (m_MsgCallbackStore.TryGetValue(m_UnRegisterList[i].Key,out callback))
            {
                callback.Remove(m_UnRegisterList[i].Value);
            }
        }
    }
}
