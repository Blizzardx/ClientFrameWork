using UnityEngine;
using System.Collections;

public class MaincityStage : StageBase
{
    public MaincityStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        Debuger.Log("StartStage maincity stage");
        MainCityLogic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        Debuger.Log("EndStage maincity stage");
        MainCityLogic.Instance.EndLogic();
    }
}
