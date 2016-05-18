using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Assets.Scripts.Core.Utils;
using LogManger;
using Object = UnityEngine.Object;

public class LogManager:Singleton<LogManager>,ILogHandler
{
    private List<string> m_LogStore;
    private bool m_bIsEnalbeRecord;
    private long m_lWriteRate;
    private string m_strSavePath;
    private long m_lLastTriggerTime;
    private int m_MaxLogFileSize;
    private int m_iLogStoreCount;

    private LogTaskManage m_TaskManager;

    #region public interface
    public void Initialize(bool isEnalbeLogToConsole = true,bool isEnableRecord = true, float writeRate = 30.0f,int logcacheCount = 1024)
    {
        if (isEnalbeLogToConsole)
        {
            Application.logMessageReceived += HandleLog;
        }
        else
        {
            Debug.logger.logHandler = this;
        }
        m_iLogStoreCount = logcacheCount;
        m_TaskManager = new LogTaskManage();
        m_TaskManager.CheckInit();
        m_LogStore = new List<string>();
        m_strSavePath = Application.persistentDataPath + "/log.txt";
        m_bIsEnalbeRecord = isEnableRecord;
        m_lLastTriggerTime = TimeManager.Instance.Now;
        m_lWriteRate = (long)(writeRate * 1000.0f);
        m_MaxLogFileSize = 3 * 1024 * 1024;// 3m

        //check size
        if (CheckSize())
        {
            m_TaskManager.StartTask(TaskType.DelFile, m_strSavePath, null);
            //FileUtils.DeleteFile(m_strSavePath);
        }

        //check time
        m_LogStore.Add(TimeManager.Instance.GetCurrentTime());
        SaveToFileSystem();
    }
    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (!m_bIsEnalbeRecord)
        {
            return;
        }
        StringBuilder res = new StringBuilder();
        // time
        res.Append(TimeManager.Instance.GetCurrentTime());
        // type
        res.Append(" " + type.ToString());
        // content
        res.Append(" " + condition);
        // stacktrace
        if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
        {
            res.Append(" " + stackTrace);
        }
        m_LogStore.Add(res.ToString());

        if (TimeManager.Instance.Now - m_lLastTriggerTime > m_lWriteRate)
        {
            SaveToFileSystem();
            m_lLastTriggerTime = TimeManager.Instance.Now;
        }
        else if (m_LogStore.Count >= m_iLogStoreCount)
        {
            SaveToFileSystem();
            m_lLastTriggerTime = TimeManager.Instance.Now;
        }
    }
    public void OnQuit()
    {
        SaveToFileSystem();
        m_TaskManager.QuickFinishedAllTask();
    }
    public void Tick()
    {
        m_TaskManager.Tick();
    }
    #endregion

    #region sytem function 
    private void SaveToFileSystem()
    {
        if (m_LogStore.Count == 0)
        {
            // do noting
            return;
        }
        FileOperationElem elem = new FileOperationElem();
        elem.m_strFilePath = m_strSavePath;
        elem.m_strParam = m_LogStore;
        m_TaskManager.StartTask(TaskType.WriteFile, elem, null);
        // do not use m_LogStore.Clear();
        m_LogStore = new List<string>();
    }
    private bool CheckSize()
    {
        return FileUtils.GetFileLength(m_strSavePath) > m_MaxLogFileSize;
    }
    public void LogFormat(LogType logType, Object context, string format, params object[] args)
    {
        HandleLog(string.Format(format, args), "", logType);
    }
    public void LogException(Exception exception, Object context)
    {
        HandleLog(context != null ?context.ToString():string.Empty, exception.StackTrace, LogType.Exception);
    }
    #endregion
}
