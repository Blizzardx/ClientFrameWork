using RegularityGame;
using UnityEngine;
using System.Collections;

public class RegularityStage : StageBase
{
    public RegularityStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        RegularityGameLogic.Instance.Initialize();
        EventReporter.Instance.EnterSceneReport("Regularity game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Regularity game scene ");
    }
}