using RegularityGame;
using UnityEngine;
using System.Collections;

public class RegularityAlphaStage : StageBase
{
    public RegularityAlphaStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        bool isFirstPlay = !PlayerManager.Instance.GetCharCounterData().GetFlag(2);
        if (isFirstPlay)
        {
            RegularityGuide.Instance.GuideStart();
        }
        else
        {
            RegularityAlphaGameLogic.Instance.Initialize();
        }
        //set : bool isFirstPlay = !PlayerManager.Instance.GetCharCounterData().SetFlag(2,true);


        EventReporter.Instance.EnterSceneReport("Regularity game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Regularity game scene ");
    }
}