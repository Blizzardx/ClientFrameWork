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

public enum EActionFrameType
{
    None = -1,
    SetCamera = 0,
    MoveCamera = 1,
    PlayAudio = 2,
    AddNpc = 3,
    EditNpc = 4,

    Max,
}

public class KeyframeData
{
    public List<ActionFrameData> framedatalist;
}

public static class ActionHelper
{
    static public readonly string ACTION_DATA_PATH = "/EditorData/actionConfig_txtpkg.bytes";
    static public readonly string ACTION_RESOURCE_PATH = "Assets/EditorCommon/EditorResources/";

    #region Get Path
    public static string GetActionFileDataPath()
    {
        return Application.persistentDataPath + ACTION_DATA_PATH;
    }
    #endregion

    #region Get ActionData
    public static GameObject GetSceneMap(string mapResName)
    {
        return ResourceManager.Instance.LoadBuildInResource<GameObject>(mapResName, AssetType.Map);
    }
    public static Transform GetCameraTransform(string cameraName)
    {
        GameObject camobj = ResourceManager.Instance.LoadBuildInResource<GameObject>(cameraName, AssetType.EditorRes);
        if (camobj == null)
        {
            Debuger.LogError("Camera Object Not Found");
        }
        return camobj.transform;
    }
    public static Camera GetCamera(string cameraName)
    {
        GameObject camobj = ResourceManager.Instance.LoadBuildInResource<GameObject>(cameraName, AssetType.EditorRes);
        Camera cam = camobj.GetComponent<Camera>();
        if (cam == null)
        {
            Debuger.LogError("Camera Component Not Found");
        }
        return cam;
    }
    public static ActionFileDataArray GetActionEditFileList()
    {
        ActionFileDataArray m_FileDataList = new ActionFileDataArray();
        ResourceManager.Instance.DecodePersonalDataTemplate(GetActionFileDataPath(), ref m_FileDataList);
        return m_FileDataList;
    }
    public static List<ActionFrameData> ConvertActionFrameData(Dictionary<float, KeyframeData> keyFrameDataDict)
    {
        List<KeyframeData> keyFrameDataList = new List<KeyframeData>(keyFrameDataDict.Values);
        List<ActionFrameData> result = new List<ActionFrameData>();
        foreach (KeyframeData key in keyFrameDataList)
        {
            if (key.framedatalist != null)
            {
                result.AddRange(key.framedatalist);
            }
        }
        //return new List<ActionFrameData>(keyFrameData);
        return result;
    }
    public static Dictionary<float, KeyframeData> ConvertKeyFrameData(List<ActionFrameData> actionFrameData)
    {
        Dictionary<float, KeyframeData> result = new Dictionary<float, KeyframeData>();

        if (actionFrameData != null)
        {
            foreach (ActionFrameData value in actionFrameData)
            {
                if (!result.ContainsKey((float)value.Time))
                {
                    KeyframeData key = new KeyframeData();
                    key.framedatalist = new List<ActionFrameData>();
                    key.framedatalist.Add(value);
                    result.Add((float)value.Time, key);
                }
                else
                { 
                    KeyframeData key = result[(float)value.Time];
                    key.framedatalist.Add(value);
                }
            }
        }
        return result;
    }
    public static Texture LoadEditorKeyframeTex()
    {
        //return ResourceManager.Instance.LoadBuildInResource<Texture>("KeyFrameTex", AssetType.EditorRes);
        //return (Texture)AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "Director_EventItem_Dark.png", typeof(Texture));
        return AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "KeyFrameTex.png", typeof(Texture)) as Texture;
        //return null;
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
            fileData.FrameDatalist.Sort(SortByTime);
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
        }

        if (null == filedatalist.DataList)
        {
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
        FileUtils.WriteByteFile(GetActionFileDataPath(), data);
    }
#if UNITY_EDITOR
    public static void SaveCameraPrefab(string prefabName, GameObject prefab)
    {
        PrefabUtility.CreatePrefab(ACTION_RESOURCE_PATH + prefabName + ".prefab", prefab);
    }
#endif
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
