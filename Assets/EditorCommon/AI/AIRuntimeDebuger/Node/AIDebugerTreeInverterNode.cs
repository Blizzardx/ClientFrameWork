//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : AIDebugerTreeInverterNode
//
// Created by : Baoxue at 2015/12/7 17:13:45
//
//
//========================================================================
using UnityEngine;
using System;
public class AIDebugerTreeInverterNode : AIDebugerTreeNode
{

    public int m_iInverter;
    public AIDebugerTreeInverterNode(string name, GameObject template, int inverter)
        : base(name, template)
    {
        m_iInverter = inverter; 

    }
}