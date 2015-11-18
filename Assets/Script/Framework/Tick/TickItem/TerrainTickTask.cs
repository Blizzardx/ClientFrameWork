using UnityEngine;
using System.Collections.Generic;
using System;

public class TerrainTickTask : AbstractTickTask
{
    private RegisterList    m_RegisterList;
    private bool            m_bIsActive;

    public TerrainTickTask()
    {
        m_RegisterList = new RegisterList();
        m_Instance = this;
        m_bIsActive = false;
    }
    private static TerrainTickTask m_Instance;
    public static TerrainTickTask Instance
    {
        get
        {
            return m_Instance;
        }
    }
    public void SetStatus(bool status)
    {
        m_bIsActive = status;
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
        return TickTaskConstant.TICK_TERRAINTRIGGER;
    }
    protected override void Beat()
    {
        if (!m_bIsActive)
        {
            return;
        }
        ExcutionUpdateList();
    }
    private void ExcutionUpdateList()
    {
        m_RegisterList.ExcutionUpdateList();
    }
}
