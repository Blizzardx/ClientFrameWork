using UnityEngine;
using System.Collections;

public class CollectionManager : Singleton<CollectionManager>
{
    public void SocketClosed()
    {
        Definer.DoCollection();

        //change to login panel
        StageManager.Instance.ChangeState(GameStateType.LoginState);

    }

    public void TimeOut()
    {
        Definer.DoCollection();

        // try to reconnect
        StageManager.Instance.ChangeState(GameStateType.ReConnect);

    }
}
