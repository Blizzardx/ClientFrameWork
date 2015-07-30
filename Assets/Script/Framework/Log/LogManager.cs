using System.Collections.Generic;
using UnityEngine;

public class LogManager
{
	public  bool 			    IsEnableLog { get; set; }
	public  bool 			    IsEnalbeRecord { get; set; }
	public  long 	            WriteRate{ get; set; }
	private  List<string> 	    m_LogStore;
	public  string 		        SavePath{ get; set; }
    private long                m_lLastTriggerTime;
	private static LogManager   m_Instance;



	#region public interface
	public static LogManager Instance
	{
		get
		{
			if( null == Instance )
			{
				m_Instance = new LogManager();
			}
			return m_Instance;
		}
	}
    public void Initialize(bool isEnalbeLog=true,bool isEnableRecord=true,float writeRate = 30.0f)
	{
		m_LogStore          = new List<string> ();
		SavePath       		= Application.persistentDataPath + "/log.txt";
		IsEnableLog      	= isEnalbeLog;
		IsEnalbeRecord   	= isEnableRecord;
	    m_lLastTriggerTime  = TimeManager.Instance.Now;
	    WriteRate        	= (long)(writeRate*1000.0f);
        Application.RegisterLogCallback(HandleLog);
	}
    public void Log(object message)
	{
		if (IsEnableLog) 
		{
			Debug.Log(message);
		}
		RecordLog (message);
	}
	public void Log(object message,Object context)
	{
		if (IsEnableLog) 
		{
			Debug.Log(message,context);
		}
		RecordLog (message);
	}
	public void LogWarning(object message)
	{
		if (IsEnableLog) 
		{
			Debug.LogWarning(message);
		}
		RecordLog (message);
	}
	public void LogWarning(object message,Object context)
	{
		if (IsEnableLog) 
		{
			Debug.LogWarning(message,context);
		}
		RecordLog (message);
	}
	public void LogError(object message)
	{
		if (IsEnableLog) 
		{
			Debug.LogError(message);
		}
		RecordLog (message);
	}
	public void LogError(object message,Object context)
	{
		if (IsEnableLog) 
		{
			Debug.LogError(message,context);
		}
		RecordLog (message);
	}
    public void OnQuit()
    {
        SaveToFileSystem();
        m_LogStore.Clear();
    }
    #endregion

    #region sytem function
    private void RecordLog(object messsage)
	{
		if (! IsEnalbeRecord)
		{
			return;
		}
		if (messsage is string) 
		{
			m_LogStore.Add(messsage as string);

            if (TimeManager.Instance.Now - m_lLastTriggerTime > WriteRate)
			{
				SaveToFileSystem();
				m_LogStore.Clear();
			    m_lLastTriggerTime = TimeManager.Instance.Now;
			}
		} 
	}
    private void HandleLog(string condition, string stacktrace, LogType type)
    {
        if (type == LogType.Assert || type == LogType.Exception)
        {
            RecordLog(condition + " " + stacktrace);
        }
    }
	private void SaveToFileSystem()
	{
        //FileUtils.SaveStringFile(SavePath, m_LogStore);
    }
	private LogManager(){}
    #endregion
}
