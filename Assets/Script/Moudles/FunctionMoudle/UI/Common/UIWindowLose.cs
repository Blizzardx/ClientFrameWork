using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWindowLose : WindowBase
{
    private Action<bool> m_OnCallBack;

    public override void OnInit()
    {
        base.OnInit();
        AddChildElementClickEvent(OnClickOk, "UIButton_ResetElem");
        AddChildElementClickEvent(OnClickCancle, "UIButton_BackElem");
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);

        if (param is Action<bool>)
        {
            m_OnCallBack = param as Action<bool>;
        }
    }
    private void OnClickCancle(GameObject go)
    {
        if (null != m_OnCallBack)
        {
            m_OnCallBack(false);
        }

    }
    private void OnClickOk(GameObject go)
    {
        if (null != m_OnCallBack)
        {
            m_OnCallBack(true);
        }
    }
}
