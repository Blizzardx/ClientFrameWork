using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWindowMsgTreeSelectPanel:WindowBase
{
    public override void OnInit()
    {
        base.OnInit();
        AddChildElementClickEvent(OnClickSell,"Button_Sell");
        AddChildElementClickEvent(OnClickBuy, "Button_Buy");
        AddChildElementClickEvent(OnClickBack, "Sprite_Exit");
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        if (MessageTreeLogic.Instance.m_bIsFirstTimePlay)
        {
            PlayAudio("GUIDE/MessageTree/Yindaoyu_#84_G_D");
            MessageTreeLogic.Instance.m_bIsFirstTimePlay = false;
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        AudioPlayer.Instance.StopAudio("GUIDE/MessageTree/Yindaoyu_#84_G_D");
    }

    public override void OnHide()
    {
        base.OnHide();
        AudioPlayer.Instance.StopAudio("GUIDE/MessageTree/Yindaoyu_#84_G_D");
    }

    private void OnClickBuy(GameObject go)
    {
        if (MessageTreeLogic.Instance.CheckIsFirstTimeBuy())
        {
            MessageTreeGuideLogic.Instance.OnClickBuy();
        }
        else
        {
            MessageTreeLogic.Instance.OnClickBuy();
        }
    }
    private void OnClickSell(GameObject go)
    {
        if (MessageTreeLogic.Instance.CheckIsFirstTimeSell())
        {
            MessageTreeGuideLogic.Instance.OnClickSell();
        }
        else
        {
            MessageTreeLogic.Instance.OnClickSell();
        }
    }
    private void OnClickBack(GameObject go)
    {
        Hide();
        WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
    }

    private void PlayAudio(string name)
    {
        AudioPlayer.Instance.PlayAudio(name, Vector3.zero,false, (res) => { });
    }
}
