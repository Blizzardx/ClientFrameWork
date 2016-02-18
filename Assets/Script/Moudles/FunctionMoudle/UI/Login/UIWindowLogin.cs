//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowLogin
//
// Created by : Baoxue at 2015/11/24 11:02:44
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;

public class UIWindowLogin:WindowBase
{
    private UIInput m_InputUserName;
    private UIInput m_InputPassword;

    public override void OnInit()
    {
        base.OnInit();

        m_InputUserName = FindChildComponent<UIInput>("Input_UserName");
        m_InputPassword = FindChildComponent<UIInput>("Input_Password");

        AddChildElementClickEvent(OnClickLogin, "UIButton_Login");
        AddChildElementClickEvent(OnClickRegister, "UIButton_Register");
    }
    private void OnClickRegister(GameObject go)
    {
        Hide();
        WindowManager.Instance.OpenWindow(WindowID.Register);
    }
    private void OnClickLogin(GameObject go)
    {
        LoginLogic.Instance.DoLogin(m_InputUserName.value, m_InputPassword.value);
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);

        string defaultusername = string.Empty;
        string defaultpassword = string.Empty;
        LoginLogic.Instance.GetDefaultLoginRequest(ref defaultusername, ref defaultpassword);

        m_InputPassword.value = defaultpassword;
        m_InputUserName.value = defaultusername;
        
    }
}

