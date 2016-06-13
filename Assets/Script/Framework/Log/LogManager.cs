﻿using Common.Tool;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace Framework.Log
{
    public class LogManager : Singleton<LogManager>, ILogHandler
    {
        private List<string> m_LogStore;
        private bool m_bIsEnalbeRecord;
        private string m_strCurrentLogSavePath;
        private string m_strLogCashePath;
        private int m_MaxLogFileSize;
        private int m_iLogStoreCount;
        private int m_iLogHistoryCount;
        private LogTaskManage m_TaskManager;
        private Queue<string> m_LogCasheList;
        private const string m_strLogSnapshotHeader = "snapshot_log_";

        #region public interface

        public void Initialize
            (bool isEnalbeLogToConsole = true, 
            bool isEnableRecord = true,
            string logCashePath = "",
            int logcacheCount = 1024,
            int logHistoryCount = 1,
            int maxLogFileSize = 3 * 1024 * 1024)// 3m
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
            if (string.IsNullOrEmpty(logCashePath))
            {
                m_strLogCashePath = Application.persistentDataPath+"/Logcashe";
            }
            else
            {
                m_strCurrentLogSavePath = logCashePath;
            }
            m_strCurrentLogSavePath = m_strLogCashePath + "/log.txt";
            m_bIsEnalbeRecord = isEnableRecord;
            m_MaxLogFileSize = maxLogFileSize; 
            m_iLogHistoryCount = logHistoryCount;
            
            // init log history cashe
            InitLogHistory();
            
            //check size
            if (CheckSize())
            {
                ClearCashe();
            }

            //check time
            m_LogStore.Add(TimeControl.Instance.GetCurrentTime());
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
            res.Append(TimeControl.Instance.GetCurrentTime());
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
            
            if (m_LogStore.Count >= m_iLogStoreCount)
            {
                SaveToFileSystem();
            }
        }
        public void Distructor()
        {
            SaveToFileSystem();
            m_TaskManager.QuickFinishedAllTask();
        }
        public void Update()
        {
            m_TaskManager.Tick();
        }
        public void UploadLog()
        {
            List<string> fileList = new List<string>();
            int count = m_LogCasheList.Count;
            int index = 0;
            while (index < count)
            {
                var elem = m_LogCasheList.Dequeue();
                fileList.Add(elem);
                m_LogCasheList.Enqueue(elem);
                ++ index;
            }
            fileList.Add(m_strCurrentLogSavePath);

            // send to task list to upload
            string url = "";
            for (int i = 0; i < fileList.Count; ++i)
            {
                string realName = fileList[i];
                if (!realName.StartsWith(m_strLogSnapshotHeader))
                {
                    realName = GenLogFileName("CurrentLog");
                }
                else
                {
                    string preFix = m_strLogCashePath + "/";
                    realName = realName.Substring(realName.LastIndexOfAny(preFix.ToCharArray()));
                }
                m_TaskManager.StartTask(TaskType.UploadFile, new object[] { url, fileList[i] , realName }, null);
            }
        }
        #endregion

        #region sytem function 
        private void InitLogHistory()
        {
            List<FileInfo> tmpList = new List<FileInfo>();
            m_LogCasheList = new Queue<string>();
            DirectoryInfo info = Directory.CreateDirectory(m_strLogCashePath);
            var file = info.GetFiles();
            for (int i = 0; i < file.Length; ++i)
            {
                string elem = file[i].Name;
                if (elem.StartsWith(m_strLogSnapshotHeader))
                {
                    //add to tmp list for sort
                    tmpList.Add(file[i]);
                }
            }
            tmpList.Sort(SortFilesByCreateTime);
            // sort files by time
            for (int i = 0; i < tmpList.Count; ++i)
            {
                string elem = m_strLogCashePath + "/" + tmpList[i].Name;
                // add to queue
                m_LogCasheList.Enqueue(elem);
            }
        }
        private int SortFilesByCreateTime(FileInfo x, FileInfo y)
        {
            if (x.LastWriteTime > y.LastWriteTime)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        private void ClearCashe()
        {
            // get new file name & save
            string newName = m_strLogCashePath + "/" + GenLogFileName();
            m_TaskManager.StartTask(TaskType.RenameFile, new object[] {m_strCurrentLogSavePath, newName}, null);
            if (m_LogCasheList.Count + 1 > m_iLogHistoryCount)
            {
                string fileName = m_LogCasheList.Dequeue();
                m_TaskManager.StartTask(TaskType.DelFile, fileName, null);
            }
        }
        private void SaveToFileSystem()
        {
            if (m_LogStore.Count == 0)
            {
                // do noting
                return;
            }
            FileOperationElem elem = new FileOperationElem();
            elem.m_strFilePath = m_strCurrentLogSavePath;
            elem.m_strParam = m_LogStore;
            m_TaskManager.StartTask(TaskType.WriteFile, elem, null);
            // do not use m_LogStore.Clear();
            m_LogStore = new List<string>();
        }
        private bool CheckSize()
        {
            return FileUtils.GetFileLength(m_strCurrentLogSavePath) > m_MaxLogFileSize;
        }
        private string GenLogFileName(string prefix = "")
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string res = m_strLogSnapshotHeader + "_" + TimeControl.Instance.GetCurrentTime() + deviceId + prefix + ".txt";
            res = res.Replace(' ', '_');
            res = res.Replace(':', '_');
            return res;
        }
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            HandleLog(string.Format(format, args), "", logType);
        }
        public void LogException(Exception exception, Object context)
        {
            HandleLog(context != null ? context.ToString() : string.Empty, exception.StackTrace, LogType.Exception);
        }
        #endregion
    }

}