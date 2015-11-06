using System;
using System.Resources;
using Assets.Scripts.Core.Utils;
using Communication;
using Config;
using Config.Table;
using TerrainEditor;
using Thrift.Protocol;
using UnityEngine;
using System.Collections.Generic;

public class ConfigManager :Singleton<ConfigManager>
{
    #region system function
    private Dictionary<string, TBase> m_ConfigPool = new Dictionary<string, TBase>();
    
    public T TryGetConfig<T>(string path) where T : class, TBase, new()
    {
        TBase config = null;
        if (m_ConfigPool.TryGetValue(path, out config))
        {
            return config as T;
        }
        T configInstance = default(T);
        if (ResourceManager.Instance.DecodePersonalDataTemplate<T>(GetConfigPath() + path, ref configInstance))
        {
            m_ConfigPool.Add(path, configInstance);
        }
        return configInstance;
    }
    public void TrySetConfig(string path,TBase value) 
    {
        if (m_ConfigPool.ContainsKey(path))
        {
            m_ConfigPool[path] = value;
        }
        else
        {
            m_ConfigPool.Add(path, value);
        }
        //save to file
        FileUtils.EnsureFolder(GetConfigPath() + path);
        byte[] buffer = ThriftSerialize.Serialize(value);
        FileUtils.WriteByteFile(GetConfigPath() + path, buffer);
    }
    #endregion

    #region config path define

    public const string Config_LangageDefine            = "zh_CN";
    public const string ConfigPath_VersionConfig        = "version_txtpkg.bytes";
    public const string ConfigPath_StateConflictConfig  = "";
    public const string ConfigPath_FunctionGroupConfig  = "";
    public const string ConfigPath_TerrainConfig        = "";
    public const string ConfigPath_CharactorConfig      = "Config/charactorConfig_txtpkg.bytes";
    public const string ConfigPath_NpcConfig            = "Config/npcConfig_txtpkg.bytes";
    public const string ConfigPath_MessageConfig        = "Config/messageConfig_txtpkg.bytes";
    #endregion

    #region config handler

    public void SaveLocalVersionConfig(VersionConfig versionConfig)
    {
        TrySetConfig(ConfigPath_VersionConfig,versionConfig);
    }
    public string GetConfigPath()
    {
        return Application.persistentDataPath + "/Download/";
    }
    public VersionConfig GetLocalVersionConfig()
    {
        VersionConfig config = TryGetConfig<VersionConfig>(ConfigPath_VersionConfig) ;
        return config;
    }
    public List<StateConflictConfigElement> GetStateConflicList(int uid,ELifeState state)
    {
        //get table
        StateConflictTable config =
            TryGetConfig<StateConflictTable>(ConfigPath_StateConflictConfig) ;

        if (null != config)
        {
            //match uid to sate conflict table
            StateConflictConfig res = null;
            if (config.StateConflictConfigMap.TryGetValue(uid, out res))
            {
                //get state
                List<StateConflictConfigElement> result = null;
                if (res.StateConflictMap.TryGetValue((int) (state), out result))
                {
                    return result;
                }
            }
        }
        return null;
    }
    public StateConflictConfig GetStateConflicMap(int uid)
    {
        //get table
        StateConflictTable config =
            TryGetConfig<StateConflictTable>(ConfigPath_StateConflictConfig) ;

        if (null != config)
        {
            //match uid to sate conflict table
            StateConflictConfig res = null;
            if (config.StateConflictConfigMap.TryGetValue(uid, out res))
            {
                return res;
            }
        }
        return null;
    }
    public FuncGroup GetFuncGroup(int iFuncGroupId)
    {
        FuncConfigTable config = TryGetConfig<FuncConfigTable>(ConfigPath_FunctionGroupConfig);
        FuncGroup result = null;
        config.FuncMap.TryGetValue(iFuncGroupId, out result);
        return result;
    }
    public LimitGroup GetLimitGroup(int iLimitGroupId)
    {
        LimitConfgTable config = TryGetConfig<LimitConfgTable>(ConfigPath_FunctionGroupConfig) ;
        LimitGroup result = null;
        config.LimitMap.TryGetValue(iLimitGroupId, out result);
        return result;
    }
    public TargetGroup GetTargetGroup(int iTargetGroupId)
    {
        TargetConfigTable config = TryGetConfig<TargetConfigTable>(ConfigPath_FunctionGroupConfig) ;
        TargetGroup result = null;
        config.TargetMap.TryGetValue(iTargetGroupId, out result);
        return result;
    }
    public TerrainEditorData GetTerrainEditorData(int terrainId)
    {
        TerrainEditorDataArray config =
            TryGetConfig<TerrainEditorDataArray>(ConfigPath_TerrainConfig) ;

        for (int i = 0; i < config.DataList.Count; ++i)
        {
            if (terrainId == config.DataList[i].ID)
            {
                return config.DataList[i];
            }
        }
        return null;
    }
    public CharactorConfig GetCharactorConfig(int id)
    {
        CharactorConfigTable config = TryGetConfig<CharactorConfigTable>(ConfigPath_CharactorConfig) ;
        CharactorConfig res = null;
        config.CharactorCofigMap.TryGetValue(id, out res);
        return res;
    }
    public NpcConfig GetNpcConfig(int id)
    {
        NpcConfigTable config = TryGetConfig<NpcConfigTable>(ConfigPath_NpcConfig);
        NpcConfig res = null;
        config.NpcCofigMap.TryGetValue(id, out res);
        return res;
    }
    public string GetMessage(int id)
    {
        MessageConfigTable config = TryGetConfig<MessageConfigTable>(ConfigPath_MessageConfig);
        Dictionary<int, string> resMap = null;
        string res = null;
        if (config.MessageMap.TryGetValue(Config_LangageDefine, out resMap))
        {
            resMap.TryGetValue(id, out res);
        }
        return res;
    }

    #endregion

}
