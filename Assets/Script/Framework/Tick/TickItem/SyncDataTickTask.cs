using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SyncDataTickTask : AbstractTickTask
{
    protected override bool FirstRunExecute()
    {
        return false;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_SYNC_DATA;
    }
    protected override void Beat()
    {
        
    }

    private void Flush()
    {

    }
}

