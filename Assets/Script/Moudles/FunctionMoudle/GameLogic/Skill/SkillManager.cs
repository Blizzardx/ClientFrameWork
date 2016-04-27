using System;
using Config;
using Config.Table;
using UnityEngine;
using System.Collections.Generic;

public class SkillManager
{
    public class SkillCd
    {
        public SkillCd(int cd)
        {
            this.cd = cd;
        }
        public SkillCd(int cd,SkillConfig config,Ilife user)
        {
            this.cd = cd;
            this.config = config;
            this.user = user;
        }
        public int cd;
        public SkillConfig config;
        public Ilife user;
    }

    private Dictionary<int, SkillCd>    m_CdStore;
    private Dictionary<int, SkillCd>    m_BeginCdStore;
    private List<int>                   m_RemoveingCdStore;
    private List<int>                   m_RemoveingBeginCdStore;
    private float                       m_fCurrentTime;
    private float                       m_fLastTime;
    private RegisterDictionaryTemplate<int> m_ListenerList;
    private RegisterDictionaryTemplate<int> m_BeginCdListenerList;
    private Dictionary<int, SkillConfig> m_PlayingSkill;
    private Dictionary<int, Ilife> m_LockedTargetList;
 
    public void Initialize()
    {
        m_BeginCdStore = new Dictionary<int, SkillCd>();
        m_CdStore = new Dictionary<int, SkillCd>();
        m_RemoveingCdStore = new List<int>();
        m_RemoveingBeginCdStore = new List<int>();
        SkillCdTickTask.Instance.RegisterToUpdateList(Update);
        m_ListenerList = new RegisterDictionaryTemplate<int>();
        m_BeginCdListenerList = new RegisterDictionaryTemplate<int>();
        m_PlayingSkill = new Dictionary<int, SkillConfig>();
        m_LockedTargetList = new Dictionary<int, Ilife>();

        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinished);
    }
    public void Destructor()
    {
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinished);
        SkillCdTickTask.Instance.UnRegisterFromUpdateList(Update);
        m_ListenerList = null;
    }
    public void TryPlaySkill(int id,Ilife user,Ilife lockedTarget = null)
    {
        SkillConfig config = ConfigManager.Instance.GetSkillConfig(id);
        if (null == config)
        {
            Debuger.Log("can't load skill id: " + id);
            return;
        }

        //check cd
        SkillCd cd = null;
        if (m_CdStore.TryGetValue(id, out cd))
        {
            if (cd.cd > 0)
            {
                // can't play skill 
                return;
            }
        }

        if (m_BeginCdStore.TryGetValue(id, out cd))
        {
            if (cd.cd > 0)
            {
                // can't play skill 
                return;
            }
        }

        if (m_PlayingSkill.ContainsKey(id))
        {
            //can't play
            return;
        }

        //per handle
        HandleTarget target = HandleTarget.GetHandleTarget(user);
        if (LimitMethods.HandleLimitExec(target, config.PerLimitId, null))
        {
            FuncMethods.HandleFuncExec(target, config.PerFuncId, null);

            m_LockedTargetList.Add(id, lockedTarget);

            if (config.BeginCd <= 0)
            {
                TriggerExecSkill(user, config);
            }
            else
            {
                if (m_BeginCdStore.ContainsKey(config.Id))
                {
                    m_BeginCdStore.Add(config.Id, new SkillCd(config.InitCd, config, user));
                }
                else
                {
                    m_BeginCdStore[config.Id] = new SkillCd(config.InitCd, config, user);
                }
            }
        }
    }
    public void RegisterBeginCdListener(int id, Action<int> callBack)
    {
        m_BeginCdListenerList.RegistEvent(id, callBack);
    }
    public void UnRegisterBeginCdListener(int id, Action<int> callBack)
    {
        m_BeginCdListenerList.UnregistEvent(id, callBack);
    }
    public void RegisterCdListener(int id,Action<int> callBack)
    {
        m_ListenerList.RegistEvent(id,callBack);
    }
    public void UnRegisterCdListener(int id, Action<int> callBack)
    {
        m_ListenerList.UnregistEvent(id,callBack);
    }
    private void TriggerExecSkill(Ilife user,SkillConfig config)
    {
        bool isLockedTarget = m_LockedTargetList.ContainsKey(config.Id) && m_LockedTargetList[config.Id] != null;

        //add to playing list
        m_PlayingSkill.Add(config.Id, config);
        
        // play animation
        FuncContext context = FuncContext.Create();
        if (!isLockedTarget)
        {
            context.Put(FuncContext.ContextKey.TargetId, config.TargeteId);
        }
        else
        {
            context.Put(FuncContext.ContextKey.Target, m_LockedTargetList[config.Id]);
        }

        context.Put(FuncContext.ContextKey.User, user);
        context.Put(FuncContext.ContextKey.LimitId, config.LimitId);
        context.Put(FuncContext.ContextKey.FunctionId, config.FuncId);

        ActionParam param = new ActionParam();
        param.Id = config.Id;
        param.Object = context;

        ActionManager.Instance.PlayAction(config.ActionId, param);

        if (m_CdStore.ContainsKey(config.Id))
        {
            m_CdStore.Add(config.Id, new SkillCd(config.InitCd));
        }
        else
        {
            m_CdStore[config.Id] = new SkillCd(config.InitCd);
        }
    }
    private void Update()
    {
        //update time
        UpdateTime();

        //get delta time
        int deltaTime = GetDelteTime();

        //update begin cd
        UpdateBeginCd(deltaTime);

        // update Cd
        UpdateCd(deltaTime);
    }
    private void UpdateBeginCd(int deltaTime)
    {
        if (m_BeginCdStore.Count <= 0)
        {
            return;
        }

        //begin update
        m_BeginCdListenerList.BeginUpdate();

        foreach (var elem in m_BeginCdStore)
        {
            elem.Value.cd -= deltaTime;

            //update listener
            m_BeginCdListenerList.Update(elem.Key, elem.Value.cd);

            if (elem.Value.cd <= 0)
            {
                // trigger play
                TriggerExecSkill(elem.Value.user,elem.Value.config);

                //add to remove store;
                m_RemoveingBeginCdStore.Add(elem.Key);
            }
        }

        //end upate
        m_BeginCdListenerList.EndUpdate();

        if (m_RemoveingBeginCdStore.Count == m_BeginCdStore.Count)
        {
            m_BeginCdStore.Clear();
            m_RemoveingBeginCdStore.Clear();
        }
        else
        {
            for (int i = 0; i < m_RemoveingBeginCdStore.Count; ++i)
            {
                //do remove
                m_BeginCdStore.Remove(m_RemoveingBeginCdStore[i]);
            }
            m_RemoveingBeginCdStore.Clear();
        }
    }
    private void UpdateCd(int deltaTime)
    {
        if (m_CdStore.Count <= 0)
        {
            return;
        }

        //begin update
        m_ListenerList.BeginUpdate();

        foreach (var elem in m_CdStore)
        {
            elem.Value.cd -= deltaTime;

            //update listener
            m_ListenerList.Update(elem.Key, elem.Value.cd);

            if (elem.Value.cd <= 0)
            {
                //add to remove store;
                m_RemoveingCdStore.Add(elem.Key);
            }
        }

        //end upate
        m_ListenerList.EndUpdate();

        if (m_RemoveingCdStore.Count == m_CdStore.Count)
        {
            m_CdStore.Clear();
            m_RemoveingCdStore.Clear();
        }
        else
        {
            for (int i = 0; i < m_RemoveingCdStore.Count; ++i)
            {
                m_CdStore.Remove(m_RemoveingCdStore[i]);
            }
            m_RemoveingCdStore.Clear();
        }

    }
    private void UpdateTime()
    {
        m_fLastTime = m_fCurrentTime;
        m_fCurrentTime = TimeManager.Instance.GetTime();
    }
    private int GetDelteTime()
    {
        return (int)(1000.0f*m_fCurrentTime - m_fLastTime);
    }
    private void OnActionFinished(MessageObject msgObj)
    {
        if (!(msgObj.msgValue is ActionParam))
        {
            return;
        }
        ActionParam ap = msgObj.msgValue as ActionParam;
        if (!(ap.Object is FuncContext))
        {
            return;
        }

        m_PlayingSkill.Remove(ap.Id);
        m_LockedTargetList.Remove(ap.Id);
    }
}