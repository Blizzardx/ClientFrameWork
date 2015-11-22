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
        public int cd;
    }
    private Dictionary<int, SkillCd>    m_CdStore;
    private List<int>                   m_RemoveingCdStore;
    private float                       m_fCurrentTime;
    private float                       m_fLastTime;
    private RegisterDictionaryTemplate<int> m_ListenerList;
 
    public void Initialize()
    {
        m_CdStore = new Dictionary<int, SkillCd>();
        m_RemoveingCdStore = new List<int>();
        SkillCdTickTask.Instance.RegisterToUpdateList(Update);
        m_ListenerList = new RegisterDictionaryTemplate<int>();
    }
    public void Destructor()
    {
        SkillCdTickTask.Instance.UnRegisterFromUpdateList(Update);
        m_ListenerList = null;
    }
    public void TryPlaySkill(int id,Ilife user)
    {
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

        SkillConfig config = ConfigManager.Instance.GetSkillConfig(id);
        if (null == config)
        {
            Debuger.Log("can't load skill id: " + id);
            return;
        }


        //per handle
        HandleTarget target = HandleTarget.GetHandleTarget(user);
        if (LimitMethods.HandleLimitExec(target, config.PerLimitId, null))
        {
            FuncMethods.HandleFuncExec(target, config.PerFuncId, null);
        }

        //excu skill
        HandleTarget target1 = TargetMethods.GetTargetList(user, config.TargeteId, null);


        // play animation
        ActionManager.Instance.InsertAction(config.ActionId, null);

        if (m_CdStore.ContainsKey(id))
        {
            m_CdStore.Add(id, new SkillCd(config.Cd));
        }
        else
        {
            m_CdStore[id] = new SkillCd(config.Cd);
        }
    }
    public void RegisterCdListener(int id,Action<int> callBack)
    {
        m_ListenerList.RegistEvent(id,callBack);
    }
    public void UnRegisterCdListener(int id, Action<int> callBack)
    {
        m_ListenerList.UnregistEvent(id,callBack);
    }
    private void OnSkillCompleteExec(SkillConfig config,HandleTarget target)
    {
        if (LimitMethods.HandleLimitExec(target, config.LimitId, null))
        {
            FuncMethods.HandleFuncExec(target, config.FuncId, null);
        }
    }
    private void Update()
    {
        //update time
        UpdateTime();

        //get delta time
        int deltaTime = GetDelteTime();

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
}
