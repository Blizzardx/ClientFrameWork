using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWindowGM : WindowBase
{
    private UIInput m_Input;
    

    public override void OnInit()
    {
        base.OnInit();
        
        m_Input = FindChildComponent<UIInput>("Input_GM");
        AddChildElementClickEvent(OnClickOpenLog, "Button_OpenLog");
        AddChildElementClickEvent(OnClickChangeToGM, "Button_ChangeToGM");
        AddChildElementClickEvent(OnClickOK, "Button_OK"); 
        AddChildElementClickEvent(OnClickExit, "Button_Exit"); 

    }
    private void OnClickExit(GameObject go)
    {
        Hide();
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
    }
    private void OnClickOpenLog(GameObject go)
    {
        GM.Instance.InputGM(GM.m_strOpenLog);
    }
    private void OnClickChangeToGM(GameObject go)
    {
        GM.Instance.InputGM(GM.m_strChaneToGM);
    }
    private void OnClickOK(GameObject go)
    {
        if(GM.Instance.InputGM(m_Input.value))
        {
            TipManager.Instance.Alert("成功");
        }
        else
        {
            TipManager.Instance.Alert("失败");
        }
    }
}
