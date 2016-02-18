//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Regularity2DGameLogic
//
// Created by : Baoxue at 2015/12/23 15:52:29
//
//
//========================================================================

using Config;
using Config.Table;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegularityGame
{
    public class Regularity2DGameLogic : SingletonTemplateMon<Regularity2DGameLogic>
    {
        private RegularityGameDifficultyManager m_DiffMgr;
        private RegularityGameSettingTable      m_GameSettingConfig;
        private float                           m_fLeftTime;
        private int                             m_iLimitCount;
        private UIWindowRegularity2D m_UIWindow;
        
        private void Awake()
        {
            _instance = this;
        }
        public void Initialize()
        {
            m_GameSettingConfig = ConfigManager.Instance.GetRegularityGameSetting();
            m_iLimitCount = m_GameSettingConfig.PlayCountLimit;
            m_fLeftTime = m_GameSettingConfig.PlayTime;
            m_DiffMgr = new RegularityGameDifficultyManager();
            RegularityGameConfig config = m_DiffMgr.GetDifficulty();
            WindowManager.Instance.OpenWindow(WindowID.Regularity2D);
            m_UIWindow = (UIWindowRegularity2D) WindowManager.Instance.GetWindow(WindowID.Regularity2D);

            Regularity2DWindowParam param = new Regularity2DWindowParam();
            param.m_ResultCallBack = OnResultCallBack;
            param.m_PilesList = config.OptionList;
            param.m_OptionList = config.AnswerList;

            m_UIWindow.ResetWindow(param);
        }
        private void OnResultCallBack(bool res)
        {
            
        }
    }
}


