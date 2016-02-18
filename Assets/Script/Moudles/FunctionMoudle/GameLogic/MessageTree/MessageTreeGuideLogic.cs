using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MessageTreeGuideLogic : Singleton<MessageTreeGuideLogic>
{
    private UIWindowGuideMsgTreeBuy m_BuyWindow;
    private UIWindowguideMsgTreeSell m_SellWindow;
    private bool m_bIsFirstTimeBuy;
    private bool m_bCanExit;
    private bool m_bEffOk;
    private bool m_bEffCancle;

    #region public interface
    public void OnClickBuy()
    {
        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.GuideMsgTreeBuy);
        m_BuyWindow = WindowManager.Instance.GetWindow(WindowID.GuideMsgTreeBuy) as UIWindowGuideMsgTreeBuy;
        m_BuyWindow.SetBlock(true);
        m_BuyWindow.SetFWPHighlightStatus(true);
        m_bIsFirstTimeBuy = true;
        PlayAudio("Yindaoyu_#85_G_D", OnBuyGuideStep2);
    }
    public void OnClickSell()
    {
        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.GuideMsgTreeSell);
        m_SellWindow = WindowManager.Instance.GetWindow(WindowID.GuideMsgTreeSell) as UIWindowguideMsgTreeSell;
        m_bCanExit = false;
        m_bEffOk = false;
        m_bEffCancle = false;
        m_SellWindow.SetBlock(true);
        m_SellWindow.SetFWPHighlightStatus(true);
        m_SellWindow.SetIcon2Sprite(-1);
        PlayAudio("Yindaoyu_#85_G_D", OnSellGuideStep2);
    }
    #endregion

    #region buy guide
    private void OnBuyGuideStep2()
    {
        m_BuyWindow.SetFWPHighlightStatus(false);
        m_BuyWindow.SetIcon1HighlightStatus(true);
        PlayAudio("Yindaoyu_#86_G_D", OnBuyGuideStep3);

    }
    private void OnBuyGuideStep3()
    {
        m_BuyWindow.SetIcon1HighlightStatus(false);
        m_BuyWindow.SetIcon0HighlightStatus(true);
        PlayAudio("Yindaoyu_#87_G_D", OnBuyGuideStep4);
    }
    private void OnBuyGuideStep4()
    {
        m_BuyWindow.SetIcon0HighlightStatus(false);
        m_BuyWindow.SetMicHighlightStatus(true);
        PlayAudio("Yindaoyu_#88_G_D", OnBuyGuideStep5);
    }
    private void OnBuyGuideStep5()
    {
        m_BuyWindow.SetMicHighlightStatus(false);
        m_BuyWindow.SetExitHighLightStatus(true);
        PlayAudio("Yindaoyu_#89_G_D", OnBuyGuideStep6);
    }
    private void OnBuyGuideStep6()
    {
        m_BuyWindow.SetExitHighLightStatus(false);
        PlayAudio("Yindaoyu_#90_G_D", OnBuyGuideStep7);
    }
    private void OnBuyGuideStep7()
    {
        m_BuyWindow.ShowFingerAnim(true);
        m_BuyWindow.SetIcon0HighlightStatus(true);
        m_BuyWindow.SetItem0HighLightStatus(true);
        m_BuyWindow.SetBlock(false);
        m_BuyWindow.SetItem0Dragable(true);
        PlayAudio("Yindaoyu_#91_G_D", ()=> { });
    }
    public void OnSetItem(int id)
    {
        if(id == 0)
        {
            m_BuyWindow.SetItemBlockStatus(0, true);
            m_BuyWindow.SetItem0Dragable(false);
            m_BuyWindow.ShowFingerAnim(false);
            m_BuyWindow.SetIcon0HighlightStatus(false);
            m_BuyWindow.SetItem0HighLightStatus(false);
            m_BuyWindow.SetBlock(true);
            StopAudio("Yindaoyu_#91_G_D");
            m_BuyWindow.SetIcon1Sprite(id);
            OnBuyGuideStep8();
        }
        if(id == 1)
        {
            OnBuyGuideStep15();
        }
    }
    private void OnBuyGuideStep8()
    {
        PlayAudio("Yindaoyu_#92_G_D", OnBuyGuideStep9,3.0f);
    }
    private void OnBuyGuideStep9()
    {
        m_BuyWindow.SetButtonStatus(true, false);
        m_BuyWindow.SetIcon2Sprite(-1);
        PlayAudio("Yindaoyu_#93_G_nantongsheng", OnBuyGuideStep10);

    }
    private void OnBuyGuideStep10()
    {
        PlayAudio("Yindaoyu_#94_G_D", OnBuyGuideStep11);
    }
    private void OnBuyGuideStep11()
    {
        PlayAudio("Yindaoyu_#95_G_D", ()=> { });
        m_BuyWindow.SetMicHighlightStatus(true);
        m_BuyWindow.SetBlock(false);
    }
    public void OnPressMic()
    {
        if(!m_bIsFirstTimeBuy)
        {
            return;
        }
        StopAudio("Yindaoyu_#95_G_D");
        PlayAudio("Yindaoyu_#96_G_D", () => { });
    }
    public void OnReleaseMic()
    {
        if(!m_bIsFirstTimeBuy)
        {
            return;
        }
        StopAudio("Yindaoyu_#96_G_D");
        m_bIsFirstTimeBuy = false;
        PlayAudio("Yindaoyu_#97_G_D", OnBuyGuideStep12,3.0f);
        m_BuyWindow.SetMicHighlightStatus(false);
        m_BuyWindow.SetBlock(true);
    }
    private void OnBuyGuideStep12()
    {
        PlayAudio("Yindaoyu_#98_G_nantongsheng", OnBuyGuideStep13);
    }
    private void OnBuyGuideStep13()
    {
        PlayAudio("Yindaoyu_#99_G_D", OnBuyGuideStep14);
    }
    private void OnBuyGuideStep14()
    {
        PlayAudio("Yindaoyu_#99_G_D", ()=> { });
        m_BuyWindow.SetIcon0HighlightStatus(true);
        m_BuyWindow.SetItem1HighLightStatus(true);
        m_BuyWindow.ShowFingerAnim(true,1);
        m_BuyWindow.SetButtonStatus(false, false);
        m_BuyWindow.SetBlock(false);
        m_BuyWindow.SetItem1Dragable(true);
    }
    private void OnBuyGuideStep15()
    {
        m_BuyWindow.SetItemBlockStatus(1, true);
        m_BuyWindow.SetItem1Dragable(false);
        m_BuyWindow.SetIcon0HighlightStatus(false);
        m_BuyWindow.SetItem1HighLightStatus(false);
        m_BuyWindow.ShowFingerAnim(false, 1);
        m_BuyWindow.SetBlock(true);
        StopAudio("Yindaoyu_#99_G_D");
        m_BuyWindow.SetIcon1Sprite(3);
        PlayAudio("Yindaoyu_#100_G_D", OnBuyGuideStep16,3.0f);
    }
    private void OnBuyGuideStep16()
    {
        PlayAudio("Yindaoyu_#101_G_D", OnBuyGuideStep17);
        m_BuyWindow.SetButtonStatus(true, true);
        m_BuyWindow.SetItemBlockStatus(6, false);
    }
    private void OnBuyGuideStep17()
    {
        PlayAudio("Yindaoyu_#102_G_D", ()=> { });
        m_BuyWindow.SetBlock(false);
    }
    public void OnBuyExit()
    {
        StopAudio("Yindaoyu_#102_G_D");
        MessageTreeLogic.Instance.BuyguidDone();
        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);

    }
    #endregion

    #region sell guide
    private void OnSellGuideStep2()
    {
        m_SellWindow.SetFWPHighlightStatus(false);
        m_SellWindow.SetIcon1HighlightStatus(true);
        PlayAudio("Yindaoyu_#86_G_D", OnSellGuideStep3);
    }
    private void OnSellGuideStep3()
    {
        m_SellWindow.SetIcon1HighlightStatus(false);
        m_SellWindow.SetIcon0HighlightStatus(true);
        PlayAudio("Yindaoyu_#87_G_D", OnSellGuideStep4);
    }
    private void OnSellGuideStep4()
    {
        m_SellWindow.SetIcon0HighlightStatus(false);
        m_SellWindow.SetExitHighLightStatus(true);
        PlayAudio("Yindaoyu_#89_G_D", OnSellGuideStep5);
    }
    private void OnSellGuideStep5()
    {
        m_SellWindow.SetExitHighLightStatus(false);
        PlayAudio("Yindaoyu_#90_G_D", OnSellGuideStep6);
    }
    private void OnSellGuideStep6()
    {
        m_SellWindow.SetItem0HighLightStatus(true);
        m_SellWindow.SetIcon0HighlightStatus(true);
        m_SellWindow.SetItem0Dragable(true);
        m_SellWindow.SetBlock(false);
        m_SellWindow.ShowFingerAnim(true, 0);
        PlayAudio("Yindaoyu_#91_G_D", ()=> { });
    }
    public void OnSellerSetItem(int id)
    {
        if (id == 0)
        {
            OnSellGuideStep7();
        }
        if (id == 1)
        {
        }
    }
    private void OnSellGuideStep7()
    {
        m_SellWindow.SetItemBlockStatus(0, true);
        m_SellWindow.SetIcon1Sprite(0);
        m_SellWindow.SetItem0HighLightStatus(false);
        m_SellWindow.SetIcon0HighlightStatus(false);
        m_SellWindow.SetItem0Dragable(false);
        m_SellWindow.SetBlock(true);
        m_SellWindow.ShowFingerAnim(false, 0);
        StopAudio("Yindaoyu_#91_G_D");
        PlayAudio("Yindaoyu_#110_G_D", OnSellGuideStep8,3.0F);
    }
    private void OnSellGuideStep8()
    {
        m_SellWindow.SetButtonStatus(true);
        m_SellWindow.SetIcon2Sprite(2);
        PlayAudio("Yindaoyu_#111_G_nantongsheng", OnSellGuideStep9);
    }
    private void OnSellGuideStep9()
    {
        PlayAudio("Yindaoyu_#112_G_D", OnSellGuideStep10);
    }
    private void OnSellGuideStep10()
    {
        m_SellWindow.SetTalkBtnStatus(true,false);
        m_SellWindow.SetMicHighlightStatus(true);
        PlayAudio("Yindaoyu_#113_G_D", OnSellGuideStep11);
    }
    private void OnSellGuideStep11()
    {
        m_SellWindow.SetBlock(false);
        m_SellWindow.SetMicHighlightStatus(false);
        m_SellWindow.SetBtncancleHighLightStatus(true);
        m_SellWindow.ShowCancleguideAnim(true);
        PlayAudio("Yindaoyu_#114_G_D", ()=> { });
        m_bEffCancle = true;
    }
    public void OnSellerClickCancle()
    {
        if (!m_bEffCancle)
        {
            return;
        }
        m_bEffCancle = false;
        m_SellWindow.SetBlock(true);
        m_SellWindow.SetBtncancleHighLightStatus(false);
        m_SellWindow.ShowCancleguideAnim(false);
        m_SellWindow.SetButtonStatus(false);
        m_SellWindow.SetIcon2Sprite(-1);
        StopAudio("Yindaoyu_#114_G_D");
        PlayAudio("Yindaoyu_#115_G_D", OnSellGuideStep12,3.0f);
    }
    private void OnSellGuideStep12()
    {
        PlayAudio("Yindaoyu_#116_G_nantongsheng", OnSellGuideStep13);
        m_SellWindow.SetButtonStatus(true);
        m_SellWindow.SetIcon2Sprite(5);
    }
    private void OnSellGuideStep13()
    {
        PlayAudio("Yindaoyu_#117_G_D", OnSellGuideStep14);
    }
    private void OnSellGuideStep14()
    {
        m_SellWindow.SetBlock(false);
        m_SellWindow.SetMicHighlightStatus(true);
        m_SellWindow.SetTalkBtnStatus(true, true);
        PlayAudio("Yindaoyu_#118_G_D", () => { });
    }
    public void OnSellerPressTalkbtn()
    {
        StopAudio("Yindaoyu_#118_G_D");
    }
    public void OnSellerReleaseTalkbtn()
    {
        PlayAudio("Yindaoyu_#119_G_D", () => { });
        m_SellWindow.SetTalkBtnStatus(true, false);
        m_SellWindow.SetMicHighlightStatus(false);
        m_SellWindow.SetBtnokHighLightStatus(true);
        m_SellWindow.ShowOkGuideAnim(true);
        m_bEffOk = true;
    }
    public void OnSellerClickOk()
    {
        if(!m_bEffOk)
        {
            return;
        }
        m_bEffOk = false;
        StopAudio("Yindaoyu_#119_G_D");
        PlayAudio("Yindaoyu_#120_G_D", ()=> { });
        m_SellWindow.SetBtnokHighLightStatus(false);
        m_SellWindow.ShowOkGuideAnim(false);
        m_SellWindow.SetButtonStatus(false);
        m_SellWindow.SetItemBlockStatus(5, false);
        m_bCanExit = true;
    }
    public void OnSellerExit()
    {
        if(!m_bCanExit)
        {
            return;
        }
        StopAudio("Yindaoyu_#120_G_D");
        MessageTreeLogic.Instance.SellguideDone();
        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
    }
    #endregion

    #region system function
    public void PlayAudio(string name,Action callBack,float delayTime = 0.5f)
    {
        name = "GUIDE/MessageTree/" + name;
        AudioPlayer.Instance.PlayAudio(name, Vector3.zero, false, (res) =>
        {
            if(delayTime <= 0.0f)
            {
                callBack();
            }
            else
            {
                TimerCollection.Callback delayFunc = () => 
                {
                    callBack();
                };
                Timer _timer = TimerCollection.GetInstance().Create(delayFunc, true, null);
                _timer.Start(delayTime);
            }            
        });
    }
    public void StopAudio(string name)
    {
        name = "GUIDE/MessageTree/" + name;
        AudioPlayer.Instance.StopAudio(name);
    }
    public void Delay(float delayTime, Action callBack)
    {
        TimerCollection.Callback delayFunc = () =>
        {
            callBack();
        };
        Timer _timer = TimerCollection.GetInstance().Create(delayFunc, true, null);
        _timer.Start(delayTime);
    }
    #endregion
    
}
