using System;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DaJieJuStage : StageBase
{
    public DaJieJuStage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        DaJieJuLogic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("DaJieJuScene");
    }

    public override void StartStage()
    {
        DaJieJuLogic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        DaJieJuLogic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("DaJieJuScene");
    }
}