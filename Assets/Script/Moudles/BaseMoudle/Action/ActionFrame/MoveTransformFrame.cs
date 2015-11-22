using UnityEngine;
using System.Collections;
using System;
using ActionEditor;

public class MoveTransformFrame : AbstractActionFrame
{
    private MoveTransformFrameConfig m_Config;

    public MoveTransformFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.MoveTransformFrame;
    }

    public override void Destory()
    {
        throw new NotImplementedException();
    }

    protected override void Execute()
    {
        if (m_Config.Path.Count <= 0) { Debug.Log("No Path"); return; }
        Vector3[] path = new Vector3[m_Config.Path.Count];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = m_Config.Path[i].GetVector3();
        }
        Hashtable args = new Hashtable();
        args.Add("path", path);
        args.Add("easeType", iTween.EaseType.linear);
        args.Add("time", m_Config.MoveTime);
        args.Add("orienttopath", m_Config.IsAutoRotate);
        foreach (GameObject obj in TargetObjects)
        {
            iTween.MoveTo(obj, args);
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

    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);
    }
}
