using System;
using Framework.Event;
using UnityEngine;

public class UILoading : UIBase
{
    protected override void OnInit()
    {
        base.OnInit();
        EventDispatcher.Instance.RegistEvent(0, OnProcess);
    }

    protected override void OnClose()
    {
        base.OnClose();
        EventDispatcher.Instance.UnregistEvent(0, OnProcess);
    }

    private void OnProcess(EventElement obj)
    {
        Debug.Log("UILoading process " + (float)obj.eventParam);
    }

    protected override PreloadAssetInfo SetSourceName()
    {
        PreloadAssetInfo res = new PreloadAssetInfo();
        res.assetName = "BuildIn/UI/Prefab/Loading/UIWindow_Loading";
        res.assetType = PerloadAssetType.BuildInAsset;
        return res;
    }
}
