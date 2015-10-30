using Config;
using Config.Table;
using Thrift.Protocol;
using UnityEngine;
using System.Collections.Generic;

public class ConfigManager :Singleton<ConfigManager>
{
    #region system function
    private Dictionary<string, TBase> m_ConfigPool = new Dictionary<string, TBase>();
    
    public TBase TryGetConfig<T>(string path) where T : TBase, new()
    {
        TBase config = null;
        if (m_ConfigPool.TryGetValue(path, out config))
        {
            return config;
        }
        T configInstance = default(T);
        if (ResourceManager.Instance.DecodePersonalDataTemplate<T>(path, ref configInstance))
        {
            m_ConfigPool.Add(path, configInstance);
        }
        return configInstance;
    }
    #endregion

    #region config path define
    public const string ConfigPath_StateConflictConfig = "";
    public const string ConfigPath_FunctionGroupConfig = "";
    #endregion

    #region config handler
    public List<StateConflictConfigElement> GetStateConflicList(int uid,ELifeState state)
    {
        //get table
        StateConflictTable config =
            TryGetConfig<StateConflictTable>(ConfigPath_StateConflictConfig) as StateConflictTable;

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
            TryGetConfig<StateConflictTable>(ConfigPath_StateConflictConfig) as StateConflictTable;

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
        FuncConfigTable config = TryGetConfig<FuncConfigTable>(ConfigPath_FunctionGroupConfig) as FuncConfigTable;
        FuncGroup result = null;
        config.FuncMap.TryGetValue(iFuncGroupId, out result);
        return result;
    }
    public LimitGroup GetLimitGroup(int iLimitGroupId)
    {
        LimitConfgTable config = TryGetConfig<LimitConfgTable>(ConfigPath_FunctionGroupConfig) as LimitConfgTable;
        LimitGroup result = null;
        config.LimitMap.TryGetValue(iLimitGroupId, out result);
        return result;
    }
    public TargetGroup GetTargetGroup(int iTargetGroupId)
    {
        TargetConfigTable config = TryGetConfig<TargetConfigTable>(ConfigPath_FunctionGroupConfig) as TargetConfigTable;
        TargetGroup result = null;
        config.TargetMap.TryGetValue(iTargetGroupId, out result);
        return result;
    }
    #endregion
}
