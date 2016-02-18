using UnityEngine;
using System.Collections;
using System;

public class LimitFuncSceneManager :LogicBase<LimitFuncSceneManager>
{
    public override void EndLogic()
    {
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, TriggerSceneState);
    }

    public override void StartLogic()
    {
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, TriggerSceneState);
    }

    private void TriggerSceneState(MessageObject obj)
    {
        if (!(obj.msgValue is GameLogicSceneType))
        {
            return;
        }
        var map = ConfigManager.Instance.GetLimitFuncSceneConfig();
        if(null == map)
        {
            Debuger.Log("can't load limit func scene config ");
            return;
        }

        int sceneid = (int)(((GameLogicSceneType)(obj.msgValue)));
        foreach (var elem in map.LimitFuncSceneConfigMap)
        {
            if(elem.Key == sceneid)
            {
                for(int i=0;i<elem.Value.Count;++i)
                {
                    HandleTarget target = HandleTarget.GetHandleTarget(null);
                    if (LimitMethods.HandleLimitExec(target, elem.Value[i].LimitId, null))
                    {
                        FuncMethods.HandleFuncExec(target, elem.Value[i].FuncId, null);
                    }
                    HandleTarget.CollectionHandlerTargetInstance(target);
                }
            }
        }
    }
}
