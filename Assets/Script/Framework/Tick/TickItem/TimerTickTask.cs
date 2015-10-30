﻿using UnityEngine;
using System.Collections;

public class TimerTickTask : AbstractTickTask
{
    protected override bool FirstRunExecute()
    {
        return true;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_TIME_CORRECT;
    }
    protected override void Beat()
    {
        TimerCollection.GetInstance().Update();
    }
}
