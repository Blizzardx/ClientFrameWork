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
        //throw new NotImplementedException();
    }

    protected override void Execute()
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
        return false;
    }

    public override bool IsTrigger(float fRealTime)
    {
        //Debug.Log("ff");
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time && fRealTime <= m_FrameData.Time + 0.5f)
        {
            return true;
        }

        return false;
    }

    public override void Pause(float fTime)
    {
        //throw new NotImplementedException();
    }

    public override void Play()
    {
        //throw new NotImplementedException();
    }

    public override void Stop()
    {
        //throw new NotImplementedException();
    }
}
