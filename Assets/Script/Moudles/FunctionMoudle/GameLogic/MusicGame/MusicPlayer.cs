//========================================================================
// Copyright(C): CYTX
//
// FileName : MusicPlayer
// 
// Created by : LeoLi at 2015/12/29 14:13:00
//
// Purpose : 
//========================================================================
using Config;
using Config.Table;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MusicGame
{
    public class MusicPlayer
    {
        public enum MusicPlayerState
        {
            Play,
            Pause,
            Stop,
            Guide,
            GuidePause,
        }

        // Player State
        private MusicPlayerState m_ePlayerState = MusicPlayerState.Stop;
        private float m_fStartTime;
        private float m_fRunTime;
        private float m_fMusicSpeed = 1f;
        private int m_nCurrentNoteIndex;
        // Player Data
        private List<MusicGameNoteKey> m_lstNoteKeyList;
        private Action<int> m_PlayKeynoteCallback;
        private Action m_MusicEndCallBack;
        public MusicPlayer(MusicGameNoteKeyConfig noteKeyConfig, Action<int> playKeynoteCallBack, Action musicEndCallBack)
        {
            if (noteKeyConfig.NoteKeyList == null || noteKeyConfig.NoteKeyList.Count == 0)
            {
                Debuger.LogError("Music Keynote can not be loaded");
                return;
            }
            m_lstNoteKeyList = noteKeyConfig.NoteKeyList;
            m_PlayKeynoteCallback = playKeynoteCallBack;
            m_MusicEndCallBack = musicEndCallBack;
        }

        #region Public Interface
        public void SetMusicSpeed(float speed)
        {
            m_fMusicSpeed = speed;
        }
        public void Reset()
        {
            m_ePlayerState = MusicPlayerState.Stop;
            m_fRunTime = 0f;
            m_nCurrentNoteIndex = 0;
            //if (m_lstNoteKeyList != null)
        }
        public void StartGuide()
        {
            m_fStartTime = TimeManager.Instance.GetTime();
            m_ePlayerState = MusicPlayerState.Guide;
            m_PlayKeynoteCallback(1);
        }
        public void ResumeGuide ()
        {
            m_fStartTime = TimeManager.Instance.GetTime() - m_fRunTime;
            m_ePlayerState = MusicPlayerState.Guide;
        }
        public void PauseGuide()
        {
            m_ePlayerState = MusicPlayerState.GuidePause;
        }
        public void Play()
        {
            m_fStartTime = TimeManager.Instance.GetTime() - m_fRunTime / m_fMusicSpeed;
            m_ePlayerState = MusicPlayerState.Play;
        }
        public void Pause()
        {
            m_ePlayerState = MusicPlayerState.Pause;
        }
        public void Update()
        {
            if (m_ePlayerState == MusicPlayerState.Stop)
            {
                return;
            }
            if (m_ePlayerState == MusicPlayerState.Guide)
            {
                GuideProcess();
                if (m_fRunTime > 5f)
                {
                    m_PlayKeynoteCallback(1);
                    m_fStartTime = TimeManager.Instance.GetTime();
                }
                return;
            }
            if(m_ePlayerState == MusicPlayerState.GuidePause)
            {
                return;
            }
            Process();
            if (m_nCurrentNoteIndex > m_lstNoteKeyList.Count - 1)
            {
                m_MusicEndCallBack();
                Reset();
                return;
            }
            // Play Keynote
            if (m_fRunTime > m_lstNoteKeyList[m_nCurrentNoteIndex].Time)
            {
                m_PlayKeynoteCallback(m_lstNoteKeyList[m_nCurrentNoteIndex].Key);
                m_nCurrentNoteIndex++;
            }
        }
        #endregion

        #region System Functions
        private void Process()
        {
            switch (m_ePlayerState)
            {
                case MusicPlayerState.Play:
                    m_fRunTime = (TimeManager.Instance.GetTime() - m_fStartTime) * m_fMusicSpeed;
                    break;
                case MusicPlayerState.Pause:
                    //m_fStartTime = TimeManager.Instance.GetTime() - m_fRunTime;
                    break;
            }
        }
        private void GuideProcess()
        {
            m_fRunTime = TimeManager.Instance.GetTime() - m_fStartTime;
        }
        #endregion
    }
}
