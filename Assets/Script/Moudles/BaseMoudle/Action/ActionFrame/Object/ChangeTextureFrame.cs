using UnityEngine;
using System.Collections;
using ActionEditor;
using System;

public class ChangeTextureFrame : AbstractActionFrame
{
    private stringCommonConfig texture_Config;

    public ChangeTextureFrame(ActionPlayer action, ActionFrameData data):base(action,data){
        texture_Config = m_FrameData.StringFrame;
    }

    public override void Destory()
    {
        //throw new NotImplementedException();
    }

    protected override void Execute()
    {
        if (TargetObjects == null || TargetObjects.Count <= 0)
            return;

        Texture newTexture = ResourceManager.Instance.LoadBuildInResource<Texture>(texture_Config.Value, AssetType.Texture);
        if (!newTexture)
        {
            Debug.LogWarning("The Texture is missing!");
            return;
        }
        foreach (GameObject obj in TargetObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial.mainTexture = newTexture;
        }
    }

    public override bool IsFinish(float fRealTime)
    {
        //throw new NotImplementedException();
        return false;
    }

    public override bool IsTrigger(float fRealTime)
    {
        //throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }
}
