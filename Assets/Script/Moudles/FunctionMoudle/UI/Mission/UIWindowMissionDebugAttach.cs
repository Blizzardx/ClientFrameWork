//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowMissionDebugAttach
//
// Created by : Baoxue at 2015/12/17 13:52:03
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIWindowMissionDebugAttach : WindowBase
{
    private List<KeyValuePair<int, int>> m_MissionCounterList;
    private List<KeyValuePair<int, int>> m_MissionStepCounterList;
    private UIPopupList m_MissionPoplist;
    private UIPopupList m_MissionStepPoplist;

    public override void OnInit()
    {
        base.OnInit();
        m_MissionPoplist = FindChildComponent<UIPopupList>("PopupList_Mission");
        m_MissionStepPoplist = FindChildComponent<UIPopupList>("PopupList_MissionStep");
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_MISSION_COUNTER_CHANGE, OnMissionCounterChange);
        RefreshStatus();
    }
    public override void OnClose()
    {
        base.OnClose(); MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_MISSION_COUNTER_CHANGE, OnMissionCounterChange);
    }
    private void OnMissionCounterChange(MessageObject obj)
    {
        RefreshStatus();
    }
    private void RefreshStatus()
    {
        if (null == MissionManager.Instance)
        {
            return;
        }

        //refreah panel
        MissionManager.Instance.GetMissionCounterReporter(ref m_MissionCounterList);
        MissionManager.Instance.GetMissionStepCounterReporter(ref m_MissionStepCounterList);

        List<string> m_missionDesc = new List<string>();
        foreach (var elem in m_MissionCounterList)
        {
            m_missionDesc.Add("Mission ID: " + elem.Key + " counter: " + elem.Value);
        }
        m_MissionPoplist.items = m_missionDesc;
        m_MissionPoplist.value = m_MissionPoplist.items[0];

        List<string> m_missionStepDesc = new List<string>();
        foreach (var elem in m_MissionStepCounterList)
        {
            m_missionStepDesc.Add("Step ID: " + elem.Key + " counter: " + elem.Value);
        }
        m_MissionStepPoplist.items = m_missionStepDesc;
        m_MissionStepPoplist.value = m_MissionStepPoplist.items[0];
    }
}

