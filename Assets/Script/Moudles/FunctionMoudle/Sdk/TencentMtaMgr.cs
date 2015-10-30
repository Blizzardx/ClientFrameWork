using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TencentMtaMgr : Singleton<TencentMtaMgr>
{
    private bool m_bIsActive;

    public void Initialize()
    {
        m_bIsActive = !(Application.platform == RuntimePlatform.WindowsEditor ||
                       Application.platform == RuntimePlatform.OSXEditor);
        if (!m_bIsActive)
        {
            return;
        }

        // set配置接口
        // 开启debug，发布时请设置为false
        MtaService.SetDebugEnable(AppManager.Instance.m_bIsShowDebugMsg);

        // 设置发布渠道，如果在androidManifest.xml配置，可不需要调用此接口
        MtaService.SetInstallChannel("play");
        // 设置上报策略，默认为APP_LAUNCH
         //MtaService.SetStatSendStrategy(MtaService.MTAStatReportStrategy.APP_LAUNCH);

        // 初始化，andriod可跳过此步骤
        // !!!!! 重要 !!!!!!!
        // MTA的appkey在android和ios系统上不同，请为根据不同平台设置不同appkey，否则统计结果可能会有问题。
        string mta_appkey = null;
#if UNITY_IOS
		mta_appkey = "I2KRDRNW985C";
#elif UNITY_ANDROID
        mta_appkey = "AQ4R2J18YJZD";
#endif
        MtaService.StartStatServiceWithAppKey(mta_appkey);
    }
    public void ReportAppMonitor(long reportSize,long responseSize,long millisecondsConsume,int resultType,int returnCode,int sampling)
    {
        if (!m_bIsActive)
        {
            return;
        }
        // 根据业务实际情况，填充monitor对象的值
        MtaAppMonitor monitor = new MtaAppMonitor("download");
        monitor.RequestSize = reportSize;
        monitor.ResponseSize = responseSize;
        monitor.MillisecondsConsume = millisecondsConsume;
        monitor.ResultType = resultType;
        monitor.ReturnCode = returnCode;
        monitor.Sampling = sampling;
        MtaService.ReportAppMonitorStat(monitor);

        /*// 根据业务实际情况，填充monitor对象的值
        MtaAppMonitor monitor = new MtaAppMonitor("download");
        monitor.RequestSize = 1000;
        monitor.ResponseSize = 304;
        monitor.MillisecondsConsume = 1000;
        monitor.ResultType = MtaAppMonitor.SUCCESS_RESULT_TYPE;
        monitor.ReturnCode = 0;
        monitor.Sampling = 1;*/
        // 上报接口监控数据
    }
    public void ReportUserInfo(string account,string name,string level)
    {
        if (!m_bIsActive)
        {
            return;
        }
        // 上报游戏用户，游戏高级模型需要用到
        MtaGameUser gameUser = new MtaGameUser(account, name, level);
        MtaService.ReportGameUser(gameUser);
    }
    public void EnterScene(string sceneName)
    {
        if (!m_bIsActive)
        {
            return;
        }
        Debuger.Log("xxEnterScene");
        // 进入场景
        MtaService.TrackBeginPage(sceneName);
    }
    public void ExitScene(string sceneName)
    {
        if (!m_bIsActive)
        {
            return;
        }
        Debuger.Log("xxExitScene");
        // 退出场景
        MtaService.TrackEndPage(sceneName);
    }
    public void ReportCustomEvent(string eventid,Dictionary<string, string> value)
    {
        if (!m_bIsActive)
        {
            return;
        }
        /*// 构建自定义事件的key-value*/
        MtaService.TrackCustomKVEvent(eventid, value);
    }
    public void BeginEvent(string key, Dictionary<string, string> value)
    {
        if (!m_bIsActive)
        {
            return;
        }
        /*// 构建自定义事件的key-value
        Dictionary<string, string> beDict = new Dictionary<string, string>();
        beDict.Add("account", "12345");
        beDict.Add("level", "8");
        beDict.Add("name", "model");
        // 通关前
        MtaService.TrackCustomBeginKVEvent("mission", beDict);
        // 通关ing...
        // 通关后
        MtaService.TrackCustomEndKVEvent("mission", beDict);*/
        MtaService.TrackCustomBeginKVEvent(key, value);
    }
    public void EndEvent(string key, Dictionary<string, string> value)
    {
        if (!m_bIsActive)
        {
            return;
        }
        MtaService.TrackCustomEndKVEvent(key, value);
    }
    public void ReportError(string error)
    {
        if (!m_bIsActive)
        {
            return;
        }
        // 上报错误信息
        MtaService.ReportError(error);
    }
    public string GetCustomProperty(string key)
    {
        if (!m_bIsActive)
        {
            return string.Empty;
        }
        // 获取在线配置，key为前台配置的在线配置信息
        return MtaService.GetCustomProperty("key");
    }
    public string GetMID()
    {
        if (!m_bIsActive)
        {
            return string.Empty;
        }
#if UNITY_ANDROID
        return MtaService.GetMid();
#endif
        return string.Empty;
    }
}
