//========================================================================
// Copyright(C): CYTX
//
// FileName : ADE_Helper
// 
// Created by : LeoLi at 2015/12/14 14:27:29
//
// Purpose : 
//========================================================================
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using TerrainEditor;
using AdaptiveDifficulty;
using Communication;
using Assets.Scripts.Core.Utils;

public static class ADE_Helper
{
    static public readonly string ADE_RESOURCE_PATH = "Assets/EditorCommon/EditorResources/";
    static public readonly string ADE_DEFAULTUSERTALENT_PATH = "/mmAdv/1.0/defaultTalentConfig_txtpkg.bytes";
    static public readonly string ADE_DIFFICULTYCONTROL_PATH = "/mmAdv/1.0/difficultyConfig_txtpkg.bytes";
    static public readonly string ADE_EVENTCONTROL_PATH = "/mmAdv/1.0/talentConfig_txtpkg.bytes";

    //Data Path
    public static string GetDefaultUserTalentDataPath()
    {
        return ADE_RESOURCE_PATH + ADE_DEFAULTUSERTALENT_PATH;
    }
    public static string GetDifficultyControlDataPath()
    {
        return ADE_RESOURCE_PATH + ADE_DIFFICULTYCONTROL_PATH;
    }
    public static string GetEventControDataPath()
    {
        return ADE_RESOURCE_PATH + ADE_EVENTCONTROL_PATH;
    }

    //Load Data
    public static DefaultUserTalent GetDefaultUserTalentMap()
    {
        DefaultUserTalent fileDataMap = new DefaultUserTalent();
        ResourceManager.DecodePersonalDataTemplate(GetDefaultUserTalentDataPath(), ref fileDataMap);
        return fileDataMap;
    }
    public static DifficultyControlDataMap GetDifficultyControlDataMap()
    {
        DifficultyControlDataMap fileDataMap = new DifficultyControlDataMap();
        ResourceManager.DecodePersonalDataTemplate(GetDifficultyControlDataPath(), ref fileDataMap);
        return fileDataMap;
    }
    public static EventControlDataMap GetEventControlDataMap()
    {
        EventControlDataMap fileDataMap = new EventControlDataMap();
        ResourceManager.DecodePersonalDataTemplate(GetEventControDataPath(), ref fileDataMap);
        return fileDataMap;
    }

    //Save Data
    public static void SaveDefaultUserTalentMap(DefaultUserTalent fileDataMap)
    {
        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetDefaultUserTalentDataPath(), data);
    }
    public static void SaveDifficultyControlDataMap(DifficultyControlDataMap fileDataMap, int gameID, DifficultyControlData fileData)
    {
        //Init Data Map
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new DifficultyControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, DifficultyControlData>();
        }
        //Init Data
        if (fileData == null || fileData.DifficultyConfig == null)
        {
            fileData = new DifficultyControlData();
            fileData.DifficultyConfig = new Dictionary<string, DifficultyConfig>();
        }
        //Save Data
        if (fileDataMap.MapFileData.ContainsKey(gameID))
        {
            fileDataMap.MapFileData[gameID] = fileData;
        }
        else
        {
            fileDataMap.MapFileData.Add(gameID, fileData);
        }
        //Save File
        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetDifficultyControlDataPath(), data);
    }
    public static void SaveEventControlDataMap(EventControlDataMap fileDataMap, int gameID, EventControlData fileData)
    {
        //Init Data Map
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new EventControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, EventControlData>();
        }
        //Init Data
        if (fileData == null || fileData.EventConfig == null)
        {
            fileData = new EventControlData();
            fileData.EventConfig = new Dictionary<string, EventConfig>();
        }
        //Save Data
        if (fileDataMap.MapFileData.ContainsKey(gameID))
        {
            fileDataMap.MapFileData[gameID] = fileData;
        }
        else
        {
            fileDataMap.MapFileData.Add(gameID, fileData);
        }
        //Save File
        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetEventControDataPath(), data);
    }

    //Sycn Data
    public static void SycnDefaultUserTalentMap(ref DefaultUserTalent fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapTalent == null)
        {
            fileDataMap = new DefaultUserTalent();
            fileDataMap.MapTalent = new Dictionary<string, int>();
        }
        DefaultUserTalent remoteDataMap = ConfigManager.Instance.GetDefaultUserTalent();
        Dictionary<string, int> remoteData = remoteDataMap.MapTalent;
        Dictionary<string, int> localData = fileDataMap.MapTalent;
        foreach (string name in remoteData.Keys)
        {
            if (localData.ContainsKey(name))
            {
                localData[name] = remoteData[name];
            }
            else
            {
                localData.Add(name, remoteData[name]);
            }
        }
        Debug.Log("ssasdasd");
    }
    public static void SycnDifficultyControlDataMap(ref DifficultyControlDataMap fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new DifficultyControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, DifficultyControlData>();
        }
        DifficultyControlDataMap remoteDataMap = ConfigManager.Instance.GetDifficultyControlDataMap();
        Dictionary<int, DifficultyControlData> remoteData = remoteDataMap.MapFileData;
        Dictionary<int, DifficultyControlData> localData = fileDataMap.MapFileData;
        foreach (int ID in remoteData.Keys)
        {
            if (localData.ContainsKey(ID))
            {
                localData[ID] = remoteData[ID];
            }
            else
            {
                localData.Add(ID, remoteData[ID]);
            }
        }

        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetDifficultyControlDataPath(), data);
    }
    public static void SycnEventControlDataMap(ref EventControlDataMap fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new EventControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, EventControlData>();
        }
        EventControlDataMap remoteDataMap = ConfigManager.Instance.GetEventControlDataMap();
        Dictionary<int, EventControlData> remoteData = remoteDataMap.MapFileData;
        Dictionary<int, EventControlData> localData = fileDataMap.MapFileData;
        foreach (int ID in remoteData.Keys)
        {
            if (localData.ContainsKey(ID))
            {
                localData[ID] = remoteData[ID];
            }
            else
            {
                localData.Add(ID, remoteData[ID]);
            }
        }

        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetEventControDataPath(), data);
    }

    //Merge Data
    public static void MergeDefaultUserTalentMap(ref DefaultUserTalent fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapTalent == null)
        {
            fileDataMap = new DefaultUserTalent();
            fileDataMap.MapTalent = new Dictionary<string, int>();
        }
        DefaultUserTalent remoteDataMap = ConfigManager.Instance.GetDefaultUserTalent();
        Dictionary<string, int> remote = remoteDataMap.MapTalent;
        Dictionary<string, int> local = fileDataMap.MapTalent;
        foreach (string name in remote.Keys)
        {
            if (local.ContainsKey(name))
            {

            }
            else
            {
                local.Add(name, remote[name]);
            }
        }
    }
    public static void MergeDifficultyControlDataMap(ref DifficultyControlDataMap fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new DifficultyControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, DifficultyControlData>();
        }
        DifficultyControlDataMap remoteDataMap = ConfigManager.Instance.GetDifficultyControlDataMap();
        Dictionary<int, DifficultyControlData> remoteData = remoteDataMap.MapFileData;
        Dictionary<int, DifficultyControlData> localData = fileDataMap.MapFileData;
        foreach (int ID in remoteData.Keys)
        {
            if (localData.ContainsKey(ID))
            {
               
            }
            else
            {
                localData.Add(ID, remoteData[ID]);
            }
        }

        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetDifficultyControlDataPath(), data);
    }
    public static void MergeEventControlDataMap(ref EventControlDataMap fileDataMap)
    {
        if (fileDataMap == null || fileDataMap.MapFileData == null)
        {
            fileDataMap = new EventControlDataMap();
            fileDataMap.MapFileData = new Dictionary<int, EventControlData>();
        }
        EventControlDataMap remoteDataMap = ConfigManager.Instance.GetEventControlDataMap();
        Dictionary<int, EventControlData> remoteData = remoteDataMap.MapFileData;
        Dictionary<int, EventControlData> localData = fileDataMap.MapFileData;
        foreach (int ID in remoteData.Keys)
        {
            if (localData.ContainsKey(ID))
            {

            }
            else
            {
                localData.Add(ID, remoteData[ID]);
            }
        }

        byte[] data = ThriftSerialize.Serialize(fileDataMap);
        FileUtils.WriteByteFile(GetEventControDataPath(), data);
    }

}

