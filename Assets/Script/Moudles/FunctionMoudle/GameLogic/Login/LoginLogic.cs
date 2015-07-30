using UnityEngine;
using System.Collections;

public class LoginLogic : LogicBase<LoginLogic>
{
    public override void StartLogic()
    {
        Debuger.Log("StartLogic LoginLogic");
        
    }
    public override void EndLogic()
    {
        Debuger.Log("EndLogic LoginLogic");
    }
}
