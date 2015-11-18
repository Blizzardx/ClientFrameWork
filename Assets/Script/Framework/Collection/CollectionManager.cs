using UnityEngine;
using System.Collections;

public class CollectionManager : Singleton<CollectionManager>
{
    public void SocketClosed()
    {
        Definer.DoCollection();

        BasicCollection();

        //change to login panel
        StageManager.Instance.ChangeState(GameStateType.LoginState);

    }

    public void TimeOut()
    {
        Definer.DoCollection();

        BasicCollection();

        // try to reconnect
        StageManager.Instance.ChangeState(GameStateType.ReConnect);
        AudioManager.Instance.Destructor();
    }

    private void BasicCollection()
    {
        StageManager.Instance.Destructor();

        //clear network layer
        NetWorkManager.Instance.RestSocketStatus();

        // clear common logic
        WindowManager.Instance.CloseAllWindow();
        
        TerrainManager.Instance.CloseTerrain();
        //release assets
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
