using ActionEditor;
using UnityEngine;
using System.Collections;

public class Runtime_AttackExecFrame : AbstractActionFrame
{
    private Runtime_AttackExecFrameConfig m_FrameConfig;

    public Runtime_AttackExecFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_FrameConfig = m_FrameData.Runtime_AttackExec;
    }

    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time)
        {
            return true;
        }

        return false;
    }

    public override bool IsFinish(float fRealTime)
    {
        return true;
    }

    public override void Play()
    {

    }

    protected override void Execute()
    {
        OnTrigger();
    }

    public override void Pause(float fTime)
    {

    }

    public override void Stop()
    {

    }

    public override void Destory()
    {

    }

    private void OnTrigger()
    {
        //try get target
        var param = m_ActionPlayer.GetActionParam();
        if (null != param && null != param.Object && param.Object is FuncContext)
        {
            FuncContext context = param.Object as FuncContext;
            if (!context.ContainsKey(FuncContext.ContextKey.LimitId) ||
                !context.ContainsKey(FuncContext.ContextKey.FunctionId))
            {
                return;
            }

            int limitId = (int)(context.Get(FuncContext.ContextKey.LimitId));
            int funcId = (int)(context.Get(FuncContext.ContextKey.LimitId));
            HandleTarget target = null;
            Ilife user = context.Get(FuncContext.ContextKey.User) as Ilife;
            if (context.ContainsKey(FuncContext.ContextKey.Target))
            {
                //locked skill
                target = HandleTarget.GetHandleTarget(user,context.Get(FuncContext.ContextKey.Target) as Ilife);
            }
            else
            {
                //target is null,unlocked skill
                int targetId = (int)(context.Get(FuncContext.ContextKey.TargetId));
                target = TargetMethods.GetTargetList(user, targetId, null);
            }

            //exec attack
            if (LimitMethods.HandleLimitExec(target, limitId, null))
            {
                FuncMethods.HandleFuncExec(target, funcId, null);
            }
        }
    }
}

