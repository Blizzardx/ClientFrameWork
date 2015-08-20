using UnityEngine;
using System.Collections;

public class TestProject1Stage: StageBase
{
    public TestProject1Stage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        Debuger.Log("start logic");

		GameTestLogic.Instance.StartLogic();
    }
    public override void EndStage()
    {
		GameTestLogic.Instance.EndLogic();
    }
}
