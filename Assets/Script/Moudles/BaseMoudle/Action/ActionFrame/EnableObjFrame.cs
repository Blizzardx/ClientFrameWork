using UnityEngine;
using System.Collections;
using System;
using ActionEditor;

public class EnableObjFrame : AbstractActionFrame
{

    private boolCommonConfig m_Config;

    public EnableObjFrame(ActionPlayer action, ActionFrameData data):base(action,data){
        m_Config = m_FrameData.BoolFrame;
    }

    public override void Destory()
    {
        throw new NotImplementedException();
    }

    public override void Execute()
    {
        if (TargetObjects == null || TargetObjects.Count <= 0)
            return;
        foreach (GameObject obj in TargetObjects)
        {
            obj.SetActive(m_Config.Value);
        }
    }

    public override bool IsFinish(float fRealTime)
    {
        throw new NotImplementedException();
    }

    public override bool IsTrigger(float fRealTime)
    {
        throw new NotImplementedException();
    }

    public override void Pause(float fTime)
    {
        throw new NotImplementedException();
    }

    public override void Play()
    {
        throw new NotImplementedException();
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }
}
