using RegularityGame;
using UnityEngine;
using System.Collections;

public class FireworkStage : StageBase
{
    public FireworkStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.Firework);
        FireworkGameLogic.Instance.Initialize();
        EventReporter.Instance.EnterSceneReport("Firework game scene");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Firework game scene");
    }
}