using RegularityGame;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MessageTreeStage : StageBase
{
    public MessageTreeStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
    }

    public override void EndStage()
    {

    }
}

