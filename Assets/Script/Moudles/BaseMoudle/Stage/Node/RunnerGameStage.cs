using RegularityGame;
using UnityEngine;
using System.Collections;


public class RunnerGameStage : StageBase
{
    public RunnerGameStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        Run.RunGameManager.Instance.GameStart();
        //RatioGame.RatioGameManager.Instance.GameSatrt();
        EventReporter.Instance.EnterSceneReport("Runner game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Runner game scene ");

    }
}