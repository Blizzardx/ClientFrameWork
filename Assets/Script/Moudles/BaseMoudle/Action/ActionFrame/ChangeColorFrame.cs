using UnityEngine;
using System.Collections;
using ActionEditor;
using System;

public class ChangeColorFrame : AbstractActionFrame
{
    private Common.Auto.ThriftVector3 color_config;

    public ChangeColorFrame(ActionPlayer action, ActionFrameData data):base(action,data){
        color_config = m_FrameData.Vector3Frame;
    }

    public override void Destory()
    {
        throw new NotImplementedException();
    }

    protected override void Execute()
    {
        if (TargetObjects == null || TargetObjects.Count <= 0)
            return;

        Vector3 colRGB = color_config.GetVector3();
        Color newColor = new Color(colRGB.x,colRGB.y,colRGB.z);

        foreach (GameObject obj in TargetObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial.color = newColor;
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
