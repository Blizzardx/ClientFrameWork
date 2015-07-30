using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Core.Utils;

public class Debuger 
{
	public static bool 			    m_bIsEnableLog;
	public static bool 			    m_bIsEnalbeRecord;
	public static long 	            m_lWriteRate;
	public static List<string> 	    m_LogStore;
	public static string 		    m_strSavePath;
    private static long             m_lLastTriggerTime;

    #region public interface
    public static void Initialize(bool isEnalbeLog=true,bool isEnableRecord=true,float writeRate = 30.0f)
	{
		m_LogStore          = new List<string> ();
		m_strSavePath       = Application.persistentDataPath + "/log.txt";
		m_bIsEnableLog      = isEnalbeLog;
		m_bIsEnalbeRecord   = isEnableRecord;
	    m_lLastTriggerTime  = TimeManager.Instance.Now;
	    m_lWriteRate        = (long)(writeRate*1000.0f);
        Application.RegisterLogCallback(HandleLog);
	}
    public static void Log(object message)
	{
		if (m_bIsEnableLog) 
		{
			Debug.Log(message);
		}
		RecordLog (message);
	}
	public static void Log(object message,Object context)
	{
		if (m_bIsEnableLog) 
		{
			Debug.Log(message,context);
		}
		RecordLog (message);
	}
	public static void LogWarning(object message)
	{
		if (m_bIsEnableLog) 
		{
			Debug.LogWarning(message);
		}
		RecordLog (message);
	}
	public static void LogWarning(object message,Object context)
	{
		if (m_bIsEnableLog) 
		{
			Debug.LogWarning(message,context);
		}
		RecordLog (message);
	}
	public static void LogError(object message)
	{
		if (m_bIsEnableLog) 
		{
			Debug.LogError(message);
		}
		RecordLog (message);
	}
	public static void LogError(object message,Object context)
	{
		if (m_bIsEnableLog) 
		{
			Debug.LogError(message,context);
		}
		RecordLog (message);
	}
    public static void OnQuit()
    {
        SaveToFileSystem();
        m_LogStore.Clear();
    }
    #endregion

    #region sytem function
    private static void RecordLog(object messsage)
	{
		if (! m_bIsEnalbeRecord)
		{
			return;
		}
		if (messsage is string) 
		{
			m_LogStore.Add(messsage as string);

            if (TimeManager.Instance.Now - m_lLastTriggerTime > m_lWriteRate)
			{
				SaveToFileSystem();
				m_LogStore.Clear();
			    m_lLastTriggerTime = TimeManager.Instance.Now;
			}
		} 
	}
    private static void HandleLog(string condition, string stacktrace, LogType type)
    {
        if (type == LogType.Assert || type == LogType.Exception)
        {
            RecordLog(condition + " " + stacktrace);
        }
    }
	private static void SaveToFileSystem()
	{
		FileUtils.SaveStringFile (m_strSavePath, m_LogStore);
    }
    #endregion
}
