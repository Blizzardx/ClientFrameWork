using System;
using System.IO;
using Assets.Scripts.Core.Utils;
using Assets.Scripts.Framework.Network;
using Common.Config;
using Communication;
using UnityEngine;
using System.Collections.Generic;

public class AssetUpdate : Singleton<AssetUpdate>
{
    private string          m_strSavePath;
    private VersionConfig   m_RemoteRootVersionConfig;
    private VersionConfig   m_LocalRootVersionConfig;
    private List<string>    m_DownloadFloderList;
    private VersionConfig   m_TmpRemoteFolderVersionConfig;
    private VersionConfig   m_TmpLocalFolderVersionConfig;
    private string          m_strTmpFolderName;
    private Action          m_AllDoneCallBack;

    private const string    m_strRootPathVersionConfigName  = "VersionConfig.byte";
    private const string    m_strRemoteAssetServerUrl       = "http://localhost/";
    public void BeginCheck(Action onFinished)
    {
        m_AllDoneCallBack = onFinished;
        m_strSavePath = Application.persistentDataPath;

        CheckNetwork();

        BeginCheckDownload();
    }

    private void ReTry()
    {
        BeginCheck(m_AllDoneCallBack);
    }
    private void CheckNetwork()
    {
        return;
    }
    private void BeginCheckDownload()
    {
        HttpService.Singleton.enabled = true;

        List<AssetFile> downloadList = new List<AssetFile>();
        AssetFile file = new AssetFile(m_strRootPathVersionConfigName, "", m_strRemoteAssetServerUrl + m_strRootPathVersionConfigName);
        downloadList.Add(file);

        AssetsDownloader.Singleton.StartDownload
            (
                downloadList,
                (isComplete) =>
                {
                    CheckDownloadFolder();
                }
                , (progress) =>
                {
                    
                }
                , (element) =>
                {
                    if (element != null && element.bytes != null)
                    {
                        m_RemoteRootVersionConfig = new VersionConfig();
                        ThriftSerialize.DeSerialize(m_RemoteRootVersionConfig, element.bytes);
                    }

                },
                false
            );
    }
    private void CheckDownloadFolder()
    {
        if (null == m_RemoteRootVersionConfig)
        {
            ShowErrorTip();
            return;
        }

        m_DownloadFloderList = new List<string>();

        // load local version config
        string path = Path.Combine(m_strSavePath, m_strRootPathVersionConfigName);
        bool isExit = ResourceManager.Instance.DecodePersonalDataTemplate(path, ref m_LocalRootVersionConfig);
        if (isExit)
        {
            for (int i = 0; i < m_RemoteRootVersionConfig.VersionList.Count; ++i)
            {
                bool isHasNewFolder = false;
                for (int j = 0; j < m_LocalRootVersionConfig.VersionList.Count; ++j)
                {
                    if (m_LocalRootVersionConfig.VersionList[j].Name == m_RemoteRootVersionConfig.VersionList[i].Name)
                    {
                        isHasNewFolder = true;
                        if (m_LocalRootVersionConfig.VersionList[j].Sign !=
                            m_RemoteRootVersionConfig.VersionList[i].Sign)
                        {
                            m_DownloadFloderList.Add(m_RemoteRootVersionConfig.VersionList[i].Name);
                        }
                    }
                }
                if (isHasNewFolder)
                {
                    m_DownloadFloderList.Add(m_RemoteRootVersionConfig.VersionList[i].Name);
                }
            }
        }
        else
        {
            for (int i = 0; i < m_RemoteRootVersionConfig.VersionList.Count; ++i)
            {
                m_DownloadFloderList.Add(m_RemoteRootVersionConfig.VersionList[i].Name);
            }
        }

        BeginCheckFolder(m_DownloadFloderList[0], OnCheckFinished);
    }
    private void OnCheckFinished()
    {
        m_DownloadFloderList.RemoveAt(0);
        if ( m_DownloadFloderList.Count > 0)
        {
            BeginCheckFolder(m_DownloadFloderList[0], OnCheckFinished);
        }
        else
        {
            string path = Path.Combine(m_strSavePath, m_strRootPathVersionConfigName);
            SaveVersionConfig(path, m_RemoteRootVersionConfig);
            HttpService.Singleton.enabled  = false;
            m_AllDoneCallBack();
        }
    }
    private void BeginCheckFolder(string folderName,Action finishedCallBack)
    {
        List<AssetFile> downloadList = new List<AssetFile>();
        string path = Path.Combine(m_strRemoteAssetServerUrl, folderName);
        path = Path.Combine(path, m_strRootPathVersionConfigName);

        AssetFile file = new AssetFile(m_strRootPathVersionConfigName, "", path);
        downloadList.Add(file);
        m_strTmpFolderName = folderName;

        m_TmpRemoteFolderVersionConfig = null;

        AssetsDownloader.Singleton.StartDownload
            (
                downloadList,
                (isComplete) =>
                {
                    CheckDownloadElement(finishedCallBack);
                }
                , (progress) =>
                {

                }
                , (element) =>
                {
                    if (element != null && element.bytes != null)
                    {
                        m_TmpRemoteFolderVersionConfig = new VersionConfig();
                        ThriftSerialize.DeSerialize(m_TmpRemoteFolderVersionConfig, element.bytes);
                    }

                },
                false
            );
    }
    private void CheckDownloadElement(Action finishedCallBack)
    {
        if (null == m_TmpRemoteFolderVersionConfig)
        {
            ShowErrorTip("");
            return;
        }

        List<AssetFile> downloadList = new List<AssetFile>();

        // load local version config
        string path = Path.Combine(m_strSavePath, m_strTmpFolderName);
        path = Path.Combine(path, m_strRootPathVersionConfigName);

        bool isExit = ResourceManager.Instance.DecodePersonalDataTemplate(path, ref m_TmpLocalFolderVersionConfig);
        if (isExit)
        {
            for (int i = 0; i < m_TmpRemoteFolderVersionConfig.VersionList.Count; ++i)
            {
                bool isHasNewFolder = false;
                for (int j = 0; j < m_TmpLocalFolderVersionConfig.VersionList.Count; ++j)
                {
                    if (m_TmpLocalFolderVersionConfig.VersionList[j].Name == m_TmpRemoteFolderVersionConfig.VersionList[i].Name)
                    {
                        isHasNewFolder = true;
                        if (m_TmpLocalFolderVersionConfig.VersionList[j].Sign !=
                            m_TmpRemoteFolderVersionConfig.VersionList[i].Sign)
                        {

                            downloadList.Add(BuildDownloadElement(m_TmpRemoteFolderVersionConfig.VersionList[i]));
                        }
                    }
                }
                if (isHasNewFolder)
                {
                    downloadList.Add(BuildDownloadElement(m_TmpRemoteFolderVersionConfig.VersionList[i]));
                }
            }
        }
        else
        {
            for (int i = 0; i < m_TmpRemoteFolderVersionConfig.VersionList.Count; ++i)
            {
                downloadList.Add(BuildDownloadElement(m_TmpRemoteFolderVersionConfig.VersionList[i]));
            }
        }

        //trigger download 
        AssetsDownloader.Singleton.StartDownload
            (
                downloadList,
                (isSucceed) =>
                {
                    if (isSucceed)
                    {
                        // save version config
                        SaveVersionConfig(path, m_TmpRemoteFolderVersionConfig);
                        finishedCallBack();
                    }
                    else
                    {
                        ShowErrorTip();
                        return;
                    }
                },
                (progress) =>
                {
                    Debuger.Log("progress : " + progress);
                },
                (data) =>
                {
                    Debuger.Log("one done");
                }
            );
    }
    private void ShowErrorTip(string errorMsg = "")
    {
        if (string.IsNullOrEmpty(errorMsg))
        {
            errorMsg = "error ,retry";
        }
        Debuger.Log(errorMsg);
        ReTry();
    }
    private AssetFile BuildDownloadElement(VersionConfigElement elem)
    {
        string name = elem.Name;
        string localPath = Path.Combine(m_strSavePath, elem.Name);
        string url = Path.Combine(m_strRemoteAssetServerUrl, elem.Name);
        AssetFile downloadElem = new AssetFile(name, localPath, url);
        return downloadElem;
    }
    private void SaveVersionConfig(string path, VersionConfig data)
    {
        byte[] byteData = ThriftSerialize.Serialize(data);
        FileUtils.WriteByteFile(path, byteData);
    }
}
