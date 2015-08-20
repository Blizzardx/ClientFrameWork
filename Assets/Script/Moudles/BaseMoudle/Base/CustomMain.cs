using UnityEngine;
using System.Collections;

public class CustomMain : Singleton<CustomMain>
{
    public void Initialize()
    {
        StageManager.Instance.ChangeState(GameStateType.TestProject1);
        //StageManager.Instance.ChangeState(GameStateType.TestProject2);
    }
    public void Quit()
    {
         
    }
}
