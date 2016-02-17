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
    ShakeCamera,
    MoveCamera,
    PlayAudio,
    AddNpc,
    MoveChar,
    AnimChar,
    MoveObject,
    EnableObject,
    EnableMeshRender,
    ChangeColor,
    EntityPlayAnimation,
    Runtime_CreateEffect,
    Runtime_MoveEffect,
    Runtime_RemoveEffect,
    Runtime_AttackExec,
    AddStateEffect,
    Runtime_AddUI,
    Runtime_RemoveUI,
    RotateChar,
    RotateCamera,
    ObjTransform,
    FuncMethod,
    StopAudio,
    Max,
}

public class KeyframeData
{
    public List<ActionFrameData> framedatalist;
    public bool isSelected = false;
}
#if UNITY_EDITOR
public static class ActionHelper
{
    static public readonly string ACTION_DATA_PATH = "/mmAdv/1.0/actionConfig_txtpkg.bytes";
    static public readonly string ACTION_RESOURCE_PATH = "Assets/EditorCommon/EditorResources/";

    #region Get Path
    public static string GetActionFileDataPath()
    {
        //return Application.persistentDataPath + ACTION_DATA_PATH;
        return ACTION_RESOURCE_PATH + ACTION_DATA_PATH;
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
            Debug.LogError("Camera Object Not Found");
        }
        return camobj.transform;
    }
    public static Camera GetCamera(string cameraName)
    {
        GameObject camobj = ResourceManager.Instance.LoadBuildInResource<GameObject>(cameraName, AssetType.EditorRes);
        Camera cam = camobj.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera Component Not Found");
        }
        return cam;
    }
    public static ActionFileDataArray GetActionEditFileList()
    {
        ActionFileDataArray m_FileDataList = new ActionFileDataArray();
        ResourceManager.DecodePersonalDataTemplate(GetActionFileDataPath(), ref m_FileDataList);
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
    public static Texture LoadEditorKeyframeTexRed()
    {
        //return ResourceManager.Instance.LoadBuildInResource<Texture>("KeyFrameTex", AssetType.EditorRes);
        //return (Texture)AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "Director_EventItem_Dark.png", typeof(Texture));
        return AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "KeyFrameTex.png", typeof(Texture)) as Texture;
        //return null;
    }
    public static Texture LoadEditorKeyframeTexBlue()
    {
        //return ResourceManager.Instance.LoadBuildInResource<Texture>("KeyFrameTex", AssetType.EditorRes);
        //return (Texture)AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "Director_EventItem_Dark.png", typeof(Texture));
        return AssetDatabase.LoadAssetAtPath(ACTION_RESOURCE_PATH + "KeyFrameTex_Blue.png", typeof(Texture)) as Texture;
        //return null;
    }

    #endregion

    #region Modify ActionData
    public static bool AddFrame(ActionFileData fileData, ActionFrameData frameData)
    {
        if (frameData == null || fileData == null)
        {
            Debug.LogError("frameData or fileData is null");
            return false;
        }
        if (fileData.FrameDatalist == null)
        {
            fileData.FrameDatalist = new List<ActionFrameData>();
        }
        if (fileData.FrameDatalist.Contains(frameData))
        {
            fileData.FrameDatalist.Sort(SortFrameByTime);
            return false;
        }
        fileData.FrameDatalist.Add(frameData);
        fileData.FrameDatalist.Sort(SortFrameByTime);
        return true;
    }
    public static bool AddFrameList(ActionFileData fileData, List<ActionFrameData> frameDataList)
    {
        if (frameDataList == null || fileData == null)
        {
            Debug.LogError("frameData or fileData is null");
            return false;
        }
        if (fileData.FrameDatalist == null)
        {
            fileData.FrameDatalist = new List<ActionFrameData>();
        }
        if (frameDataList.Count <= 0)
        {
            return false;
        }
        int count = 0;
        foreach (ActionFrameData frameData in frameDataList)
        {
            if (!fileData.FrameDatalist.Contains(frameData))
            {
                fileData.FrameDatalist.Add(frameData);
                count++;
            }
        }
        fileData.FrameDatalist.Sort(SortFrameByTime);
        if (count == 0)
        {
            return false;
        }
        return true;
    }
    public static void DelFrame(ActionFileData fileData, ActionFrameData frameData)
    {
        if (frameData == null || fileData == null)
        {
            Debug.LogError("frameData or fileData is null");
            return;
        }
        if (fileData.FrameDatalist == null)
        {
            Debug.LogError("frameDatalist is null");
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
            Debug.LogError("fileData is null");
            return;
        }
        if (fileData.FrameDatalist == null)
        {
            Debug.LogError("frameDatalist is null");
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

        fileData.TimeStamp = GetTimeStamp();
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
    public static void SyncActionEditFileList(ActionFileDataArray filedatalist)
    {
        // Get Local Data
        Dictionary<int, ActionFileData> localDataMap = new Dictionary<int, ActionFileData>();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            foreach (ActionFileData value in filedatalist.DataList)
            {
                if (!localDataMap.ContainsKey(value.ID))
                {
                    localDataMap.Add(value.ID, value);
                }
                else
                {
                    Debug.LogWarning("本地Action ID 有冲突！");
                }
            }
        }
        // Get Remote Data
        ActionFileDataArray downloadData = ConfigManager.Instance.GetActionFileDataArray();
        if (downloadData != null && downloadData.DataList != null)
        {
            foreach (ActionFileData value in downloadData.DataList)
            {
                if (!localDataMap.ContainsKey(value.ID))
                {
                    localDataMap.Add(value.ID, value);
                }
                else
                {
                    if (CheckConflcit(localDataMap[value.ID], value))
                    {
                        localDataMap.Remove(value.ID);
                        localDataMap.Add(value.ID, value);
                        Debug.LogWarning("远程的ActionFile与本地有冲突，本地文件已被覆盖！");
                    }
                }
            }
        }
        // Sycn Data to Local
        if (localDataMap.Count > 0)
        {
            filedatalist = new ActionFileDataArray();
            filedatalist.DataList = new List<ActionFileData>(localDataMap.Values);
            filedatalist.DataList.Sort(SortFileByID);
            byte[] data = ThriftSerialize.Serialize(filedatalist);
            FileUtils.WriteByteFile(GetActionFileDataPath(), data);
        }
    }
    public static bool CombineActionEditFileList(ActionFileDataArray filedatalist)
    {
        Dictionary<int, ActionFileData> tmp;
        CombineActionEditFileList(filedatalist, out tmp);
        return false;
    }
    // whether have conflict
    public static bool CombineActionEditFileList(ActionFileDataArray filedatalist, out Dictionary<int, ActionFileData> conflictDataMap)
    {
        // Get Local Data
        Dictionary<int, ActionFileData> localDataMap = new Dictionary<int, ActionFileData>();
        // Conflict Value
        conflictDataMap = new Dictionary<int, ActionFileData>();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            foreach (ActionFileData value in filedatalist.DataList)
            {
                if (!localDataMap.ContainsKey(value.ID))
                {
                    localDataMap.Add(value.ID, value);
                }
                else
                {
                    Debug.LogWarning("本地Action ID 有冲突！");
                }
            }
        }
        // Get Remote Data
        ActionFileDataArray downloadData = ConfigManager.Instance.GetActionFileDataArray();
        if (downloadData != null && downloadData.DataList != null)
        {
            foreach (ActionFileData value in downloadData.DataList)
            {
                if (!localDataMap.ContainsKey(value.ID))
                {
                    localDataMap.Add(value.ID, value);
                }
                else
                {
                    if (CheckConflcit(localDataMap[value.ID], value))
                    {
                        conflictDataMap.Add(value.ID, value);
                    }
                }
            }
        }
        // Sycn Data to Local
        if (localDataMap.Count > 0)
        {
            filedatalist = new ActionFileDataArray();
            filedatalist.DataList = new List<ActionFileData>(localDataMap.Values);
            filedatalist.DataList.Sort(SortFileByID);
            byte[] data = ThriftSerialize.Serialize(filedatalist);
            FileUtils.WriteByteFile(GetActionFileDataPath(), data);
        }
        // return
        if (conflictDataMap.Count > 0)
        {
            return true;
        }
        return false;
    }
    public static void MergeActionEditFileList(ActionFileDataArray filedatalist, Dictionary<int, ActionFileData> conflictDataMap)
    {
        // Get Local Data
        Dictionary<int, ActionFileData> localDataMap = new Dictionary<int, ActionFileData>();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            foreach (ActionFileData value in filedatalist.DataList)
            {
                if (!localDataMap.ContainsKey(value.ID))
                {
                    localDataMap.Add(value.ID, value);
                }
                else
                {
                    Debug.LogWarning("本地Action ID 有冲突！");
                }
            }
        }
        // Merge Data
        if (conflictDataMap != null)
        {
            foreach (int key in conflictDataMap.Keys)
            {
                if (localDataMap.ContainsKey(key))
                {
                    localDataMap.Remove(key);
                    localDataMap.Add(key, conflictDataMap[key]);
                    Debug.LogWarning("冲突解决，本地文件 : " + key.ToString() + " 已被覆盖！");
                }
            }
        }
        // Sycn Data to Local
        if (localDataMap.Count > 0)
        {
            filedatalist = new ActionFileDataArray();
            filedatalist.DataList = new List<ActionFileData>(localDataMap.Values);
            filedatalist.DataList.Sort(SortFileByID);
            byte[] data = ThriftSerialize.Serialize(filedatalist);
            FileUtils.WriteByteFile(GetActionFileDataPath(), data);
        }
    }
    public static void DeleteActionEditFile(ActionFileDataArray filedatalist, ActionFileData fileData)
    {
        if (null == filedatalist)
        {
            filedatalist = new ActionFileDataArray();
        }

        if (null == filedatalist.DataList)
        {
            filedatalist.DataList = new List<ActionFileData>();
        }

        for (int i = 0; i < filedatalist.DataList.Count; ++i)
        {
            if (filedatalist.DataList[i].ID == fileData.ID)
            {
                filedatalist.DataList[i] = fileData;
                filedatalist.DataList.RemoveAt(i);
                break;
            }
        }

        byte[] data = ThriftSerialize.Serialize(filedatalist);
        FileUtils.WriteByteFile(GetActionFileDataPath(), data);
    }
    public static void ReplaceEditFileList(ActionFileDataArray filedatalist)
    {
        filedatalist = ConfigManager.Instance.GetActionFileDataArray();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            Debug.LogWarning("本地所有文件被掩盖");
            filedatalist.DataList.Sort(SortFileByID);
            byte[] data = ThriftSerialize.Serialize(filedatalist);
            FileUtils.WriteByteFile(GetActionFileDataPath(), data);
        }
    }
    public static void BackupEditFileList(string path)
    {
        ActionFileDataArray filedatalist = ConfigManager.Instance.GetActionFileDataArray();
        if (filedatalist != null && filedatalist.DataList != null)
        {
            filedatalist.DataList.Sort(SortFileByID);
            byte[] data = ThriftSerialize.Serialize(filedatalist);
            FileUtils.WriteByteFile(path + "actionConfig_txtpkg.bytes", data);
        }
    }
    #endregion

    // Methods
    public static int SortFrameByTime(ActionFrameData item1, ActionFrameData item2)
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
    public static int SortFileByID(ActionFileData item1, ActionFileData item2)
    {
        if (item1.ID < item2.ID)
        {
            return -1;
        }
        else if (item1.ID > item2.ID)
        {
            return 1;
        }

        return 0;
    }
    public static long GetTimeStamp()
    {
        long instanceId = TimeUtil.Now();
        return instanceId;
    }
    public static bool CheckConflcit(ActionFileData item1, ActionFileData item2)
    {
        if (item1.TimeStamp == item2.TimeStamp)
        {
            return false;
        }
        return true;
    }
}
#endif

#region ColorDefine
public class ColorDefine
{
    public static Color Cheery
    {
        get { return new Color(227f / 255, 71f / 255f, 108f / 255f); }
    }

    public static Color Pink
    {
        get { return new Color(234f / 255, 83f / 255f, 85f / 255f); }
    }

    public static Color LightGray
    {
        get { return new Color(0.921f, 0.921f, 0.921f); }
    }

    public static Color Gray
    {
        get { return new Color(0.698f, 0.698f, 0.698f); }
    }

    public static Color DarkGray
    {
        get { return new Color(0.353f, 0.353f, 0.353f); }
    }

    public static Color Yellow
    {
        get { return new Color(255f / 255f, 255f / 255f, 0f / 255f); }
    }

    public static Color Orange
    {
        get { return new Color(245f / 255, 166f / 255f, 35f / 255f); }
    }
    public static Color Blue
    {
        get { return new Color(109f / 255, 171f / 255f, 250f / 255f); }
    }
    public static Color LightBlue
    {
        get { return new Color(0f / 255, 255f / 255f, 255f / 255f); }
    }
    public static Color Purple
    {
        get { return new Color(150f / 255, 0f / 255f, 255f / 255f); }
    }
    public static Color Red
    {
        get { return new Color(227f / 255, 71f / 255f, 108f / 255f); }
    }

    public static Color Green
    {
        get { return new Color(78f / 255, 216f / 255f, 122f / 255f); }
    }

    public static Color Black
    {
        get { return new Color(0, 0, 0); }
    }
}
#endregion
