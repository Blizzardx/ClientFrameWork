using UnityEngine;
using System.Collections;

public class Copenhagen01Stage: StageBase
{
    public Copenhagen01Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        Copenhagen01Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("Copenhagen01Scene");
    }

    public override void StartStage()
    {
        Copenhagen01Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        Copenhagen01Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("Copenhagen01Scene");
    }
}

