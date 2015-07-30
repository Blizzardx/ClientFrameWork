using UnityEngine;
using System.Collections;

public class LoginStage : StageBase
{
    public LoginStage(GameStateType type) : base(type)
    {
    }

    public override void StartStage()
    {
        Debuger.Log("StartStage LoginStage");
        LoginLogic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        Debuger.Log("EndStage LoginStage");
        LoginLogic.Instance.EndLogic();
    }
}
