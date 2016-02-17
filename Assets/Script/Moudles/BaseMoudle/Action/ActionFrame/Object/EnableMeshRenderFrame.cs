using UnityEngine;
using System.Collections;
using ActionEditor;
using System;

public class EnableMeshRenderFrame : AbstractActionFrame
{
    private boolCommonConfig m_Config;

    public EnableMeshRenderFrame(ActionPlayer action, ActionFrameData data):base(action,data){
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
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                Debug.LogWarning("You didn't add a Mesh Renderer to the Affected Object", obj);
                return;
            }
            meshRenderer.enabled = m_Config.Value;
        }
    }

    public override bool IsFinish(float fRealTime)
    {
        //throw new NotImplementedException();
        return false;
    }

    public override bool IsTrigger(float fRealTime)
    {
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
