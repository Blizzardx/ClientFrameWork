//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowAssetUdpate
//
// Created by : Baoxue at 2015/11/24 12:16:42
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIWindowAssetUdpate : WindowBase
{
    private UILabel m_LabelProcess;
    private UILabel m_LabelCurrentFile;
    private UISlider m_SliderProcess;

    public override void OnInit()
    {
        base.OnInit();
        m_LabelProcess = FindChildComponent<UILabel>("UILabel_Process");
        m_LabelCurrentFile = FindChildComponent<UILabel>("UILabel_CurrentDownloadFile");
        m_SliderProcess = FindChildComponent<UISlider>("Slider");
    }

    public override void OnOpen(object param)
    {
        base.OnOpen(param);
    }

    public void OnProcess(string fileName, float process)
    {
        m_LabelProcess.text = (process*100.0f).ToString();
        m_LabelCurrentFile.text = fileName;
        m_SliderProcess.value = process;
    }
}

