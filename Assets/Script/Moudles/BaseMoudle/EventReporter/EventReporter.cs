using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EventReporter : Singleton<EventReporter>
{
    public void EnterSceneReport(string sceneName)
    {
        Debuger.Log("Enter scene " + sceneName);
        TencentMtaMgr.Instance.EnterScene(sceneName);
    }
    public void ExitSceneReport(string sceneName)
    {
        Debuger.Log("exit scene " + sceneName);
        TencentMtaMgr.Instance.ExitScene(sceneName);
    }
    public void CustomEventReport(string eventId, Dictionary<string, string> value)
    {
        TencentMtaMgr.Instance.ReportCustomEvent(eventId, value);
    }
	
}
