using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Utils;
using NetWork;
using UnityEngine;

namespace Assets.Scripts.Framework.Network
{
    public class AssetDownloadCallBack : HttpManager.DownloadCallback
    {
        private Action<byte[], object, long, long>  m_OnProcess;
        private Action<object>                      m_OnComplate;
        private Action<Exception, object>           m_OnError;
        private FileStream                          m_CurrentFileStream;
        private long                                m_lCurrentLength;
        private long m_lLength;
        private string                              m_strFileTmpName;
        private string m_strDataPath;
        private static long m_MarkIndex = 0;
        private bool m_bIsStoped;

        public void Initialize(string dataPath,Action<byte[], object,long,long> onProcess,Action<object> OnComplate,Action<Exception,object> OnError)
        {
            m_bIsStoped = false;
            m_strDataPath = dataPath;
            m_OnProcess = onProcess;
            m_OnComplate = OnComplate;
            m_OnError = OnError;
            m_lCurrentLength = 0L;
            m_lLength = 0L; 
        }
        public void OnPrepare(long length, FileStream continueFileStream, object param)
        {
            if (m_bIsStoped)
            {
                return;
            }
            AssetFile fileElement = param as AssetFile;
            if (!fileElement.IsSaveToFile)
            {
                return;
            }
            m_lLength = length;
            // create tmp file
            m_strFileTmpName = m_strDataPath + DateTime.Now.Ticks + (m_MarkIndex++).ToString();
            FileUtils.EnsureFolder(m_strFileTmpName);
            m_CurrentFileStream = continueFileStream == null ? new FileStream(m_strFileTmpName, FileMode.Create) : continueFileStream;
        }
        public void OnProcess(byte[] buffer, object param)
        {
            if (m_bIsStoped)
            {
                return;
            }
            Debug.Log("onprocess file ");
            AssetFile fileElement = param as AssetFile;
            if (fileElement.IsSaveToFile)
            {
                m_CurrentFileStream.Write(buffer, 0, buffer.Length);
            }
            m_lCurrentLength += buffer.Length;
            //call back process and save to file
            m_OnProcess(buffer, param,m_lCurrentLength,m_lLength);
        }
        public void OnComplate(object param)
        {
            if (m_bIsStoped)
            {
                return;
            }
            Debug.Log("OnComplate file ");
            AssetFile fileElement = param as AssetFile;
            if (fileElement.IsSaveToFile)
            {
                m_CurrentFileStream.Close();
                FileUtils.DeleteFile(fileElement.LocalPath);
                FileUtils.EnsureFolder(fileElement.LocalPath);
                File.Copy(m_strFileTmpName, fileElement.LocalPath);
                Debuger.Log("complete download file : " + fileElement.LocalPath);
                //File.Move(m_strFileTmpName, fileElement.LocalPath);
            }
            m_OnComplate(param);
        }
        public void OnError(object param, Exception e)
        {
            if (m_bIsStoped)
            {
                return;
            }
            Debug.Log("OnError file ");
            AssetFile fileElement = param as AssetFile;
            if (fileElement.IsSaveToFile && m_CurrentFileStream != null)
            {
                m_CurrentFileStream.Close();
            }

            //error call back
            m_OnError(e, param);
        }
        public void StopCallback()
        {
            m_bIsStoped = true;
        }
    }
    public class AssetsDownloader_Async:Singleton<AssetsDownloader_Async>
    {
        //成功完成回调，如果资源为存储型，则byte[]为空
        private Action<byte[], AssetFile>       m_OnSucceedCompleteCallBack;
        //错误回调
        private Action<Exception, AssetFile>    m_OnErrorCallBack;
        //进度回调，分别为当前进度和当前正在下载的文件名
        private Action<float, AssetFile>        m_OnProcessCallBack;
        private Action                          m_OnAllCompleteCallBack;
        private int                             m_iCurrentIndex;
        private List<AssetFile>                 m_CurrentDownloadList;
        private AssetDownloadCallBack           m_CallBack;
        private bool                            m_bIsBusy;
        private float                           m_fCurrentProcess;
        private float                           m_fLastProcess;
        private bool                            m_bHaveError;
        private Exception                       m_ErrorException;
        private bool                            m_bIsOneComplete;
        private List<byte>                      m_CurrentDownloadBuffer;
        private string m_strTmpCache;

        //time out controller
        private const int m_iTimeout = 10000;
        private int m_iCurrentDuringTime = 0;

        public void BeginDownload
            (List<AssetFile>                downloadList,           //下载列表
            Action<byte[], AssetFile>       onOneCompleteCallBack,  //单个完成进度回调
            Action<Exception, AssetFile>    onOneErrorCallBack,     //单个错误回调
            Action<float, AssetFile>        onProcessCallBack,      //进度回调 当前进度 和 当前下载文件
            Action                          onCompleteCallBack)      
        {
            if (m_bIsBusy || 
                downloadList == null || 
                downloadList.Count == 0 || 
                onOneCompleteCallBack == null ||
                onOneErrorCallBack == null ||
                onProcessCallBack == null ||
                onCompleteCallBack == null 
                )
            {
                Debug.LogError("return");
                return;
            }

            m_strTmpCache = Application.persistentDataPath + "/tmp/";

            m_CurrentDownloadList = downloadList;
            m_OnSucceedCompleteCallBack = onOneCompleteCallBack;
            m_OnErrorCallBack = onOneErrorCallBack;
            m_OnProcessCallBack = onProcessCallBack;
            m_OnAllCompleteCallBack = onCompleteCallBack;

            m_fLastProcess = 0.0f;
            m_fCurrentProcess = 0.0f;
            if (null != m_CurrentDownloadBuffer)
            {
                m_CurrentDownloadBuffer.Clear();
            }
            m_iCurrentIndex = 0;
            m_bIsOneComplete = false;
            m_bHaveError = false;

            //triger download
            BeginDownload();

            m_bIsBusy = true;
            DownloadTickTask.Instance.RegisterToUpdateList(MainThreadUpdate);
        }
        private void OnProcess(byte[] buffer, object param,long curLength,long length)
        {
            AssetFile downloadElement = param as AssetFile;
            //call back process and save to file
            if (!downloadElement.IsSaveToFile)
            {
                m_CurrentDownloadBuffer.InsertRange(m_CurrentDownloadBuffer.Count, buffer);
            }
            UpdateProcess(length, curLength);
        }
        private void OnComplate(object param)
        {
            m_bIsOneComplete = true;
        }
        private void OnError( Exception e,object param)
        {
            m_ErrorException = e;
            m_bHaveError = true;
        }
        private void BeginDownload()
        {
            m_bIsOneComplete = false;
            m_CallBack = new AssetDownloadCallBack();
            m_CallBack.Initialize(m_strTmpCache, OnProcess, OnComplate, OnError);

            AssetFile downloadElement = m_CurrentDownloadList[m_iCurrentIndex];
            if (!downloadElement.IsSaveToFile)
            {
                m_CurrentDownloadBuffer = new List<byte>(1024);
            }
            HttpManager.Instance.BeginDownload(downloadElement.Url, m_CallBack, downloadElement);
        }
        private void Clear()
        {
            RemarkTime();
            m_fLastProcess = 0.0f;
            m_fCurrentProcess = 0.0f;
            m_CurrentDownloadList.Clear();
            if (null != m_CurrentDownloadBuffer)
            {
                m_CurrentDownloadBuffer.Clear();
            }
            m_iCurrentIndex = 0;
            m_bIsBusy = false;
            m_bIsOneComplete = false;
            m_bHaveError = false;
            DownloadTickTask.Instance.UnRegisterFromUpdateList(MainThreadUpdate);
        }
        private void UpdateProcess(long length,long currentLength)
        {
            //process = (index + buffersize/length)/downloadlist.cout;
            m_fCurrentProcess = (float)(((double)(m_iCurrentIndex) + (double)(currentLength) / (double)(length)) /
                                (double)(m_CurrentDownloadList.Count));
        }
        private void MainThreadUpdate()
        {
            if (!m_bIsBusy)
            {
                return;
            }
            UpdateTime();
            if (m_fCurrentProcess > m_fLastProcess)
            {
                RemarkTime();
                m_fLastProcess = m_fCurrentProcess;
                //process call back
                m_OnProcessCallBack(m_fCurrentProcess, m_CurrentDownloadList[m_iCurrentIndex]);
            }
            if (m_bHaveError)
            {
                RemarkTime();
                m_OnErrorCallBack(m_ErrorException, m_CurrentDownloadList[m_iCurrentIndex]);
                Clear();
            }
            if (m_bIsOneComplete)
            {
                RemarkTime();

                byte[] buffer = null;
                if (!m_CurrentDownloadList[m_iCurrentIndex].IsSaveToFile)
                {
                    buffer = m_CurrentDownloadBuffer.ToArray();
                }
                //on one complete
                m_OnSucceedCompleteCallBack(buffer, m_CurrentDownloadList[m_iCurrentIndex]);

                ++ m_iCurrentIndex;
                if (m_iCurrentIndex >= m_CurrentDownloadList.Count)
                {
                    Clear();
                    m_OnAllCompleteCallBack();
                }
                else
                {
                    BeginDownload();
                }
            }
            if (IsTimeout())
            {
                // time out ,do sth
                m_CallBack.StopCallback();
                m_ErrorException = new Exception("Time out");
                m_OnErrorCallBack(m_ErrorException, m_CurrentDownloadList[m_iCurrentIndex]);
                Clear();
            }
        }
        private bool IsTimeout()
        {
            return m_iCurrentDuringTime > m_iTimeout;
        }
        private void RemarkTime()
        {
            m_iCurrentDuringTime = 0;
        }
        private void UpdateTime()
        {
            m_iCurrentDuringTime += (int) (Time.time*1000.0f);
        }
    }
}