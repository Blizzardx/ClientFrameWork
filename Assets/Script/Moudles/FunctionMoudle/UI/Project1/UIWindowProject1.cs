using System.Collections.Generic;
using CSLE;
using UnityEngine;
using System.Collections;

public class UIWindowProject1 : WindowBase
{
    private UISprite m_SpriteDog;

    public override void OnInit()
    {
        base.OnInit();
        InitUIComponent(ref m_SpriteDog, "m_SpriteDog");

        UIEventListener.Get(m_SpriteDog.gameObject).onClick = OnClickEvent0;
        /*AddChildElementClickEvent(OnClickExit, "Button_Exit");
        AddChildElementClickEvent(OnClickEvent0, "Button_Event0");
        AddChildElementClickEvent(OnClickEvent1, "Button_Event1");
        AddChildElementClickEvent(OnClickEvent2, "Button_Event2");*/
        //EventReporter.Instance.EnterSceneReport("MainCity");
    }
    private void OnClickEvent0(GameObject go)
    {
        Debuger.Log("On OnClickEvent0");
        var value = new Dictionary<string, string>();
        value.Add("ClickCount", "10");
        //EventReporter.Instance.CustomEventReport("0",value);
    }
    private void OnClickEvent1(GameObject go)
    {
        Debuger.Log("On OnClickEvent1");
        var value = new Dictionary<string, string>();
        value.Add("ClickCount", "15");
        //EventReporter.Instance.CustomEventReport("1", value);
    }
    private void OnClickEvent2(GameObject go)
    {
        Debuger.Log("On OnClickEvent2");
        var value = new Dictionary<string, string>();
        value.Add("ClickCount", "20");
//        EventReporter.Instance.CustomEventReport("2", value);
    }
    private void OnClickExit(GameObject go)
    {
        Debuger.Log("On exit");
        Hide();
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
    }

    public override void OnClose()
    {
        base.OnClose();
        //EventReporter.Instance.ExitSceneReport("MainCity");
    }
}