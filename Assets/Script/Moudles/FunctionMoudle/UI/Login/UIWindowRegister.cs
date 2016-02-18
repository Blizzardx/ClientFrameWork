//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowRegister
//
// Created by : Baoxue at 2015/11/24 11:53:38
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIWindowRegister : WindowBase
{
    private UIInput m_InputUserName;
    private UIInput m_InputPassword;
    private UIInput m_InputPassword2;

    public override void OnInit()
    {
        base.OnInit();

        m_InputUserName = FindChildComponent<UIInput>("Input_UserName");
        m_InputPassword = FindChildComponent<UIInput>("Input_Password");
        m_InputPassword2 = FindChildComponent<UIInput>("Input_Password2");

        AddChildElementClickEvent(OnClickRegister, "UIButton_Register");
        AddChildElementClickEvent(OnClickBack, "UIButton_Back");
    }
    private void OnClickRegister(GameObject go)
    {
        LoginLogic.Instance.DoRegister(m_InputUserName.value, m_InputPassword.value, m_InputPassword2.value);
    }
    private void OnClickBack(GameObject go)
    {
        WindowManager.Instance.OpenWindow(WindowID.Login);
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
    }
}

