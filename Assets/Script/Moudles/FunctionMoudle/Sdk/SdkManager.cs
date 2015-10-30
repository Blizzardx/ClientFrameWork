using UnityEngine;
using System.Collections;

public class SdkManager : Singleton<SdkManager>
{
    public void InitSdk()
    {
        //tencent mta
        TencentMtaMgr.Instance.Initialize();
    }
}
