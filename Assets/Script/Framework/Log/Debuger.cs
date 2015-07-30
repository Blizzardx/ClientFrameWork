using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Core.Utils;

public class Debuger 
{
	public static bool 			m_bIsEnableLog;
	public static bool 			m_bIsEnalbeRecord;
	public static readonly int 	m_iMaxLogCount 	= 	20;
	public static List<string> 	m_LogStore;
	public static string 		m_strSavePath;

	public static void Initialize(bool isEnalbeLog=true,bool isEnableRecord=true)
	{
		m_LogStore = new List<string> (m_iMaxLogCount);
		m_strSavePath = Application.persistentDataPath + "/log.txt";
		m_bIsEnableLog = isEnalbeLog;
		m_bIsEnalbeRecord = isEnableRecord;
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
	private static void RecordLog(object messsage)
	{
		if (! m_bIsEnalbeRecord)
		{
			return;
		}
		if (messsage is string) 
		{
			m_LogStore.Add(messsage as string);

			if( m_LogStore.Count >= m_iMaxLogCount )
			{
				SaveToFileSystem();
				m_LogStore.Clear();
			}
		} 
	}
	private static void SaveToFileSystem()
	{
		FileUtils.SaveStringFile (m_strSavePath, m_LogStore);
	}

}
