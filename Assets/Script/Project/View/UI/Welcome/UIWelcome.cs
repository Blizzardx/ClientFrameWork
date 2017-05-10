using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Event;
using UnityEngine;

class UIWelcome:UIBase
{
    protected override PreloadAssetInfo SetSourceName()
    {
        PreloadAssetInfo res = new PreloadAssetInfo();
        res.assetName = "BuildIn/UI/Prefab/MainMenu/Window_Welcom";
        res.assetType = PerloadAssetType.BuildInAsset;
        return res;
    }
}