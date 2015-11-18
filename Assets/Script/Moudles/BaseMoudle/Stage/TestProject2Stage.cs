using UnityEngine;
using System.Collections;

public class TestProject2Stage : StageBase
{
    public TestProject2Stage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.CloseAllWindow();
    }

    public override void EndStage()
    {
        
    }
}
