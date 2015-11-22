using System;
using UnityEngine;
using System.Collections;

public class SkillCdTickTask : AbstractTickTask
{
    private RegisterList m_RegisterList;

    public SkillCdTickTask()
    {
        m_RegisterList = new RegisterList();
        m_Instance = this;
    }
    private static SkillCdTickTask m_Instance;
    public static SkillCdTickTask Instance
    {
        get
        {
            return m_Instance;
        }
    }
    public void RegisterToUpdateList(Action element)
    {
        m_RegisterList.RegisterToUpdateList(element);
    }
    public void UnRegisterFromUpdateList(Action element)
    {
        m_RegisterList.UnRegisterFromUpdateList(element);
    }
    protected override bool FirstRunExecute()
    {
       
        return true;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_SKILLCD;
    }
    protected override void Beat()
    {
        ExcutionUpdateList();
    }
    private void ExcutionUpdateList()
    {
       m_RegisterList.ExcutionUpdateList();
    }
}