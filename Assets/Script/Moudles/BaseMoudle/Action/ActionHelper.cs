//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionHelper
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/5 17:13:39
//
// Purpose : Action相关静态方法
//========================================================================
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using Communication;
using Assets.Scripts.Core.Utils;
public static class ActionHelper
{
    #region Get ActionData
    public static string GetActionDataPath()
    {
        return Application.persistentDataPath + "/EditorData/ActionData.bytes";
    }
    public static ActionFileDataArray GetActionEditFileList()
    {
        ActionFileDataArray m_FileDataList = new ActionFileDataArray();
        ResourceManager.Instance.DecodePersonalDataTemplate(GetActionDataPath(), ref m_FileDataList);
        return m_FileDataList;
    }
    public static List<ActionFrameData> ConvertActionFrameData(Dictionary<float, ActionFrameData> keyFrameData)
    {
        return new List<ActionFrameData>(keyFrameData.Values);
    }
    public static Dictionary<float, ActionFrameData> ConvertKeyFrameData(List<ActionFrameData> actionFrameData)
    {
        Dictionary<float, ActionFrameData> result = new Dictionary<float, ActionFrameData>();
        if (actionFrameData != null)
        {
            foreach (ActionFrameData value in actionFrameData)
            {
                if (!result.ContainsValue(value))
                {
                    result.Add((float)value.Time, value);
                }
            }
        }
        return result;
    }
    #endregion

    #region Modify ActionData
    public static void AddFrame(ActionFileData fileData, ActionFrameData frameData)
    {
        if (frameData == null || fileData == null)
        {
            Debuger.LogError("frameData or fileData is null");
            return;
        }
        if (fileData.FrameDatalist == null)
        {
            fileData.FrameDatalist = new List<ActionFrameData>();
        }
        if (fileData.FrameDatalist.Contains(frameData))
        {
            return;
        }
        fileData.FrameDatalist.Add(frameData);
        fileData.FrameDatalist.Sort(SortByTime);
    }
    public static void DelFrame(ActionFileData fileData, ActionFrameData frameData)
    {
        if (frameData == null || fileData == null)
        {
            Debuger.LogError("frameData or fileData is null");
            return;
        }
        if (fileData.FrameDatalist == null)
        {
            Debuger.LogError("frameDatalist is null");
            return;
        }
        if (fileData.FrameDatalist.Contains(frameData))
        {
            fileData.FrameDatalist.Remove(frameData);
        }
    }
    public static void DelFrame(ActionFileData fileData, int nIndex)
    {
        if (fileData == null)
        {
            Debuger.LogError("fileData is null");
            return;
        }
        if (fileData.FrameDatalist == null)
        {
            Debuger.LogError("frameDatalist is null");
            return;
        }
        if (nIndex < 0 || nIndex >= fileData.FrameDatalist.Count)
        {
            return;
        }
        fileData.FrameDatalist.RemoveAt(nIndex);
    }
    #endregion

    #region Save ActionData
    public static void SaveActionEditFileList(ActionFileDataArray filedatalist, ActionFileData fileData)
    {
        if (null == filedatalist)
        {
            filedatalist = new ActionFileDataArray();
            filedatalist.DataList = new List<ActionFileData>();
        }

        bool bIsNeedAddNew = true;
        for (int i = 0; i < filedatalist.DataList.Count; ++i)
        {
            if (filedatalist.DataList[i].ID == fileData.ID)
            {
                filedatalist.DataList[i] = fileData;
                bIsNeedAddNew = false;
                break;
            }
        }
        if (bIsNeedAddNew)
        {
            filedatalist.DataList.Add(fileData);
        }

        byte[] data = ThriftSerialize.Serialize(filedatalist);
        FileUtils.WriteByteFile(GetActionDataPath(), data);

    }
    #endregion

    #region Other
    public static int SortByTime(ActionFrameData item1, ActionFrameData item2)
    {
        if (item1.Time < item2.Time)
        {
            return -1;
        }
        else if (item1.Time > item2.Time)
        {
            return 1;
        }

        return 0;
    }
    #endregion
}
