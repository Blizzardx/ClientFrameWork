using UnityEngine;
using System.Collections;

public class CustomMain : Singleton<CustomMain>
{
    public void Initialize()
    {
        LimitFuncSceneManager.Instance.StartLogic();
        StageManager.Instance.ChangeState(GameStateType.LoginState);
        //StageManager.Instance.ChangeState(GameStateType.TestProject2);
    }
    public void Quit()
    {
         
    }
}
