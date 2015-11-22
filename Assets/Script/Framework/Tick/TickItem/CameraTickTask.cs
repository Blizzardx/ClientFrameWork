﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CameraTickTask : AbstractTickTask
{
   
    private RegisterList m_RegisterList;

    public CameraTickTask()
    {
        m_RegisterList = new RegisterList();
        m_Instance = this;
    }
    private static CameraTickTask m_Instance;
    public static CameraTickTask Instance
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
        return TickTaskConstant.TICK_CAMERA;
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

