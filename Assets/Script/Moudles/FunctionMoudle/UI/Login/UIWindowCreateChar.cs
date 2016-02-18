//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowCreateChar
//
// Created by : Baoxue at 2015/11/27 14:02:42
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIWindowCreateChar : WindowBase
{
    private UIInput m_InputName;

    public override void OnInit()
    {
        base.OnInit();

        m_InputName = FindChildComponent<UIInput>("Input_NickName");

        AddChildElementClickEvent(OnClickCreate, "UIButton_Create");
    }
    private void OnClickCreate(GameObject go)
    {
        LoginLogic.Instance.DoCreateChar(5, m_InputName.value, 0);
    }
}


