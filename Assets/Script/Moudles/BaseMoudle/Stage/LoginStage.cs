using UnityEngine;
using System.Collections;

public class LoginStage : StageBase
{
    public LoginStage(GameStateType type) : base(type)
    {
    }

    public override void StartStage()
    {
        Debug.Log("StartStage LoginStage");
        LoginLogic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        Debug.Log("EndStage LoginStage");
        LoginLogic.Instance.EndLogic();
    }
}
