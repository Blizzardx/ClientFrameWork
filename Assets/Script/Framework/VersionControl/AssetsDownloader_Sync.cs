using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Utils;
using Framework.Async;
using NetWork;

namespace Assets.Scripts.Framework.Network
{

    public class DownloadAsyncHelper : IAsyncTask
    {
        private Action<byte[], AssetFile>           m_CompleteCallBack;
        private Action<Exception,AssetFile>         m_ErrorCallBack;
        private Action                              m_AllCompleteCallBack;
        private int                                 m_iCurrentIndex;
        private List<AssetFile>                     m_CurrentDownloadList;
        private string                              m_strTmpCache;
        private bool                                m_bIsInit = false;

        // tmp Data
        private bool        m_bIsSucceed;
        private Exception   m_Exception;
        private List<byte>  m_tmpByteBuffer;

        private static int m_iMarkIndex = 0;
        public void Initialize
            (
            List<AssetFile> downloadFiles,
            Action<byte[], AssetFile> completeCallBack, 
            Action<Exception, AssetFile> errorCallBack,
            Action allCompleteCallBack
            )
        {
            m_bIsInit = true;
            m_strTmpCache = Application.persistentDataPath + "/tmp/";
            m_iCurrentIndex = 0;
            m_CurrentDownloadList = downloadFiles;
            m_CompleteCallBack  = completeCallBack;
            m_ErrorCallBack     = errorCallBack;
            m_AllCompleteCallBack = allCompleteCallBack;
        }

        public void Clear()
        {
            m_CompleteCallBack = null;
            m_ErrorCallBack = null;
            m_bIsInit = false;
        }
        public AsyncState BeforeAsyncTask()
        {
            if (!m_bIsInit)
            {
                return AsyncState.Done;
            }
            return AsyncState.DoAsync;
        }
        public AsyncState DoAsyncTask()
        {
            if (!m_bIsInit)
            {
                return AsyncState.Done;
            }
            try
            {
                AssetFile file = m_CurrentDownloadList[m_iCurrentIndex];
                FileStream fileStream = null;
                string fileName = m_strTmpCache + DateTime.Now.Ticks + (m_iMarkIndex++).ToString();
                if (file.IsSaveToFile)
                {
                    // create tmp file

                    FileUtils.EnsureFolder(fileName);
                    fileStream = new FileStream(fileName, FileMode.Create);
                }

                if (HttpManager.Instance.Download(file.Url, out m_Exception, out m_tmpByteBuffer, fileStream))
                {
                    AssetFile fileElement = m_CurrentDownloadList[m_iCurrentIndex];
                    // copy file to dir
                    if (fileElement.IsSaveToFile)
                    {
                        FileUtils.DeleteFile(fileElement.LocalPath);
                        FileUtils.EnsureFolder(fileElement.LocalPath);
                        File.Copy(fileName, fileElement.LocalPath);
                        Debug.Log("complete download file : " + fileElement.LocalPath);
                    }
                    // succeed
                    m_bIsSucceed = true;
                }
                else
                {
                    // complete with error
                    m_bIsSucceed = false;
                }
            }
            catch (Exception e)
            {
                m_Exception = e;
                m_bIsSucceed = false;
            }
            
            return AsyncState.AfterAsync;
        }
        public AsyncState AfterAsyncTask()
        {
            if (!m_bIsInit)
            {
                return AsyncState.Done;
            }
            if (m_bIsSucceed)
            {
                // succeed call back
                m_CompleteCallBack(m_tmpByteBuffer.ToArray(), m_CurrentDownloadList[m_iCurrentIndex]);
                ++m_iCurrentIndex;
                if (m_iCurrentIndex >= m_CurrentDownloadList.Count)
                {
                    m_AllCompleteCallBack();
                    return AsyncState.Done;
                }
                else
                {
                    return AsyncState.DoAsync;
                }
            }
            else
            {
                m_ErrorCallBack(m_Exception, m_CurrentDownloadList[m_iCurrentIndex]);
                return AsyncState.Done;
            }
        }
    }

    public class AssetsDownloader_Sync : Singleton<AssetsDownloader_Sync>
    {
        //成功完成回调，如果资源为存储型，则byte[]为空
        private Action<byte[], AssetFile>               m_OnSucceedCompleteCallBack;
        //错误回调
        private Action<Exception, AssetFile>            m_OnErrorCallBack;
        //进度回调，分别为当前进度和当前正在下载的文件名
        private Action<float, AssetFile>                m_OnProcessCallBack;
        private Action                                  m_OnAllCompleteCallBack;
        private List<AssetFile>                         m_CurrentDownloadList;
        private bool                                    m_bIsBusy;
        private List<byte>                              m_CurrentDownloadBuffer;
        private DownloadAsyncHelper                     m_AsyncHelper;
        private int                                     m_iTmpIndex;

        public void BeginDownload
            (
                List<AssetFile> downloadList,                       //下载列表
                Action<byte[], AssetFile> onOneCompleteCallBack,    //单个完成进度回调
                Action<Exception, AssetFile> onOneErrorCallBack,    //单个错误回调
                Action<float, AssetFile> onProcessCallBack,         //进度回调 当前进度 和 当前下载文件
                Action onCompleteCallBack                           //完成回调
            )
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


            m_CurrentDownloadList = downloadList;
            m_OnSucceedCompleteCallBack = onOneCompleteCallBack;
            m_OnErrorCallBack = onOneErrorCallBack;
            m_OnProcessCallBack = onProcessCallBack;
            m_OnAllCompleteCallBack = onCompleteCallBack;
            m_iTmpIndex = 0;

            if (null != m_CurrentDownloadBuffer)
            {
                m_CurrentDownloadBuffer.Clear();
            }

            m_AsyncHelper = new DownloadAsyncHelper();
            m_bIsBusy = true;

            BeginDownload();
        }

        private void Clear()
        {
            m_iTmpIndex = 0;
            if (null != m_AsyncHelper)
            {
                m_AsyncHelper.Clear();
            }
            m_CurrentDownloadList.Clear();
            if (null != m_CurrentDownloadBuffer)
            {
                m_CurrentDownloadBuffer.Clear();
            }
            m_bIsBusy = false;
        }
        private void BeginDownload()
        {
            Debug.Log("begin download");
            m_AsyncHelper.Initialize(m_CurrentDownloadList, OnOneCompleted, OnOneError, AllComplete);
            AsyncManager.Instance.ExecuteAsyncTask(m_AsyncHelper);
        }
        private void OnOneCompleted(byte[] data, AssetFile file)
        {
            Debug.Log("OnOneCompleted download " + file.Name);
            ++m_iTmpIndex;
            // update process
            m_OnProcessCallBack(UpdateProcess(), file);
            
            m_OnSucceedCompleteCallBack(data, file);
        }
        private void OnOneError(Exception e, AssetFile file)
        {
            Debug.LogException(e);
            Debug.Log("OnOneError download " + file.Name);
            Clear();
            m_OnErrorCallBack(e, file);
        }
        private void AllComplete()
        {
            Debug.Log("AllComplete download " );
            Clear();
            m_OnAllCompleteCallBack();
        }
        private float UpdateProcess()
        {
            return (float) (m_iTmpIndex)/(float) (m_CurrentDownloadList.Count);
        }
    }
}