using System;
using System.IO;
using Assets.Scripts.Core.Utils;
using UnityEngine;
using System.Collections.Generic;

public class LogManager :Singleton<LogManager>
{

    private List<string>        m_LogStore;
    private bool                m_bIsEnalbeRecord;
    private long                m_lWriteRate;
    private string              m_strSavePath;
    private string              m_strSaveCsvPath;
    private long                m_lLastTriggerTime;

	#region public interface
    public void Initialize(bool isEnalbeLog=true,bool isEnableRecord=true,float writeRate = 30.0f)
    {
        m_LogStore          = new List<string>();
		m_strSavePath       = Application.persistentDataPath + "/log.txt";
		m_strSaveCsvPath    = Application.persistentDataPath + "/log.csv";
		m_bIsEnalbeRecord   = isEnableRecord;
	    m_lLastTriggerTime  = TimeManager.Instance.Now;
	    m_lWriteRate        = (long)(writeRate*1000.0f);
        Debuger.Initialize(OnLogTrigger, isEnalbeLog);
	}
    public void OnQuit()
    {
        SaveToFileSystem();
    }
    #endregion

    #region sytem function 
	private void SaveToFileSystem()
	{
        FileUtils.WriteStringFile(m_strSavePath, m_LogStore);
        m_LogStore.Clear();
    }
    private void OnLogTrigger(string message)
    {
        if (!m_bIsEnalbeRecord)
        {
            return;
        }
        m_LogStore.Add(message);
        if (TimeManager.Instance.Now - m_lLastTriggerTime > m_lWriteRate)
        {
            SaveToFileSystem();
            m_lLastTriggerTime = TimeManager.Instance.Now;
        }
    }
    #endregion
}
