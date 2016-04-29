using Assets.Scripts.Framework.Network;
using Core;
using NetWork;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MessageTreeLogic : Singleton<MessageTreeLogic>
{
    public enum MTStatus
    {
        None,
        Sell,
        Buy,
    }
    public enum ChatStatus
    {
        DisConnect,
        Record,
        Playing,
        CD,
        ConnectFree,
       
    }
    private bool                m_bIsActive;
    private MTStatus            m_CurrentState;
    private float               m_fPintTickTime = 1.0f;
    private float               m_fCurrentTickTime;
    private MTPingRequest       m_PingRequest;
    private AsyncMTPingRequest  m_AsyncPingRequest;
    private BuyResponse         m_BuyResp;
    private SellResponse        m_SellResp;
    private long m_iChatId;
    private long m_iSaleId;
    public bool m_bIsFirstTimePlay = true;

    // sell
    private Action m_OnSellCallBack;
    private Action<int> m_OnSellerBidCallBack;
    private Action<OpenChatEvent> m_OnOpenChatCallBack;
    private Action<bool,bool> m_OnAccRefCallBack;
    private Action m_OnResetBuyerCallBack;
    private int m_iBuyerBidItemId;

    //buy
    private Action<BuyResponse> m_OnInitBuyDealCallBack;
    private Action<BidResponse> m_OnBuyBidRespCallBack;
    private Action<bool> m_BidResponseCallBack;
    private int m_BuyerBidItemId;
    private int m_iSellerItemId;

    //chat
    private bool m_bIsEnalbeChat;
    private bool m_bIsChatActive;
    private float m_fChatCd = 10.0f;
    private float m_fCurrentChatCd;
    private float m_fRecordVoiceLength = 10.0f;
    private ChatStatus m_CurrentChatStatus;
    private List<AudioClip> m_WaitingForPlayList;
    private bool m_bIsClosing;
    private float m_fCurrentPlayingAudioLength;
    private float m_fCurrentPlayedAudioLength;
    private int m_SellerBidItemId;

    #region public interface
    public void OnClickBuy()
    {
        WindowManager.Instance.HideWindow(WindowID.MsgTreeSelectPanel);

        BuyRequest request = new BuyRequest();        
        AsyncBuyRequest async = new AsyncBuyRequest(request);
        async.TryRequest();
    }
    public void OnClickSell()
    {
        // send msg and block 
        WindowManager.Instance.HideWindow(WindowID.MsgTreeSelectPanel);

        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSell);
    }
    public void OnClickExitDeal()
    {
        if (m_bIsClosing)
        {
            // do noting
            return;
        }
        if (m_CurrentChatStatus == ChatStatus.DisConnect)
        {
            if(m_CurrentState == MTStatus.Sell)
            {
                CloseSaleRequest closeRequest = new CloseSaleRequest();
                closeRequest.SaleId = m_iSaleId;

                AsyncCloseSaleRequest async1 = new AsyncCloseSaleRequest(closeRequest);
                async1.TryRequest();
                m_bIsClosing = true;
            }
            else
            {
                WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
                WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
                ChangeMTStatus(MTStatus.None);
                WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
            }
             return;
            
        }
        CloseChatRequest request = new CloseChatRequest();
        request.ChatId = m_iChatId;

        AsyncCloseChatRequest async = new AsyncCloseChatRequest(request);
        async.TryRequest();
        m_bIsClosing = true;
    }
    public void OnCloseChatResponse(CloseChatResponse resp)
    {
        m_bIsClosing = false;
        if (resp.Success)
        {
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
            ChangeMTStatus(MTStatus.None);
            ChangeChatStatus(ChatStatus.DisConnect);
            WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
        }
        else
        {
            TipManager.Instance.Alert("", "你已经掉线", "OK", (res) => { ClearSale(); });
        }
    }
    public MTStatus GetCurrentStatus()
    {
        return m_CurrentState;
    }
    public bool CheckIsFirstTimeBuy()
    {
        return ! PlayerManager.Instance.GetCharCounterData().GetFlag(8);
    }
    public void BuyguidDone()
    {
        PlayerManager.Instance.GetCharCounterData().SetFlag(8, true);
    }
    public bool CheckIsFirstTimeSell()
    {
        return !PlayerManager.Instance.GetCharCounterData().GetFlag(9);
    }
    public void SellguideDone()
    {
        PlayerManager.Instance.GetCharCounterData().SetFlag(9, true);
    }
    #endregion

    #region system
    private void ChangeMTStatus(MTStatus status)
    {
        if(status == m_CurrentState)
        {
            return;
        }
        switch(m_CurrentState)
        {
            case MTStatus.Buy:
                ExitBuy();
                break;
            case MTStatus.Sell:
                ExitSell();
                break;
            case MTStatus.None:
                break;
        }
        switch(status)
        {
            case MTStatus.Buy:
                EnterBuy();
                break;
            case MTStatus.Sell:
                EnterSell();
                break;
            case MTStatus.None:
                break;
        }
        m_CurrentState = status;
    }
    private void SendPing()
    {
        TryInitPingRequest();
        m_AsyncPingRequest.TryRequest();
    }
    private void TryInitPingRequest()
    {
        if(null == m_PingRequest)
        {
            m_PingRequest = new MTPingRequest();
        }
        if(null == m_AsyncPingRequest)
        {
            m_AsyncPingRequest = new AsyncMTPingRequest(m_PingRequest);
        }
        m_PingRequest.SaleId = m_iSaleId;
    }
    private void Update()
    {
        if(m_CurrentState != MTStatus.None)
        {
            m_fCurrentTickTime += TimeManager.Instance.GetDeltaTime();
            if (m_fCurrentTickTime >= m_fPintTickTime)
            {
                m_fCurrentTickTime = 0;
                SendPing();
            }

            if (!m_bIsChatActive && m_bIsEnalbeChat)
            {
                m_fCurrentChatCd += TimeManager.Instance.GetDeltaTime();
                if (m_fCurrentChatCd >= m_fChatCd)
                {
                    m_bIsChatActive = true;
                }
            }
        }

        UpdateChat();
        
    }
    private bool CheckToTip(BuyResponse buy,SellResponse sell)
    {
        if(null != buy)
        {
            return buy.Success;
        }
        if(null != sell)
        {
            return sell.Success;
        }
        return false;
    }
    private void OnEventListResponse(MessageObject obj)
    {
        if(!(obj.msgValue is MEventList))
        {
            return;
        }
        if (m_CurrentState == MTStatus.None)
        {
            // do noting
            return;
        }
        MEventList list = obj.msgValue as MEventList;

        if (list == null || list.Events == null || list.Events.Count == 0)
        {
            return;
        }
        bool havHandlerCloseChat = false;
        bool havHandlerCloseSale = false;
        for (int i=0;i<list.Events.Count;++i)
        {
            short type = list.Events[i].ActionType;

            if(type == MEventIdConstants.CLOSE_CHAT)
            {
                if(havHandlerCloseChat)
                {
                    continue;
                }
                if(!CheckChatId(list.Events[i].CloseChat.ChatId))
                {
                    continue;
                }
                //handle close chat
                CloseChat(list.Events[i].CloseChat);
                havHandlerCloseChat = true;
            }
            if (type == MEventIdConstants.CLOSE_SALE)
            {
                if (havHandlerCloseSale)
                {
                    continue;
                }
                if (!CheckSaleId(list.Events[i].CloseSale.SaleId))
                {
                    continue;
                }
                //handle close sale
                CloseSale(list.Events[i].CloseSale);
                havHandlerCloseSale = true;
            }
            if (type == MEventIdConstants.OPEN_CHAT)
            {
                OpenChat(list.Events[i].OpenChat);
            }
            if (type == MEventIdConstants.BID)
            {
                //bid
                HandleBid(list.Events[i].Bid);
            }
            if (type == MEventIdConstants.CHAT_INFO)
            {
                if (!CheckChatId(list.Events[i].ChatMessage.ChatId))
                {
                    continue;
                }
                //handle chat
                HandleChat(list.Events[i].ChatMessage);
            }
            if (type == MEventIdConstants.BID_ACCEPT)
            {
                if (!CheckChatId(list.Events[i].AcceptBid.ChatId))
                {
                    continue;
                }
                //handle accept bid
                AcceptBid();
            }
            if (type == MEventIdConstants.BID_REFUSED)
            {
                if (!CheckChatId(list.Events[i].RefuseBid.ChatId))
                {
                    continue;
                }
                RefuseBid();
            }
        }
        
    }
    private void HandleBid(BidEvent bid)
    {
        m_iBuyerBidItemId = bid.OfferId;
        if (null != m_OnSellerBidCallBack)
        {
            m_OnSellerBidCallBack(bid.OfferId);
        }
    }
    private void CloseSale(CloseSaleEvent closeSale)
    {
        ChangeMTStatus(MTStatus.None);
        WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
        WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
        PlayAudioTip(closeSale.MessageType);
    }
    private void CloseChat(CloseChatEvent closeChat)
    {
        if (m_CurrentState == MTStatus.Buy)
        {
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
            ChangeMTStatus(MTStatus.None);
            WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
        }
        else
        {
            //init panel
            if (null != m_OnResetBuyerCallBack)
            {
                m_OnResetBuyerCallBack();
            }

            ChangeChatStatus(ChatStatus.DisConnect);
        }
        m_bIsEnalbeChat = false;
        PlayAudioTip(closeChat.MessageType);
    }
    private void OpenChat(OpenChatEvent chat)
    {
        m_iChatId = chat.ChatId; 
        ChangeChatStatus(ChatStatus.ConnectFree);
        if (null != m_OnOpenChatCallBack)
        {
            m_OnOpenChatCallBack(chat);
        }
    }
    private void HandleChat(ChatInfoEvent chat)
    {
        PlayRecord(chat);
    }
    private void RefuseBid()
    {
        if (null != m_BidResponseCallBack)
        {
            m_BidResponseCallBack(false);
        }
    }
    private void AcceptBid()
    {
        ItemManager.Instance.RemoveItem(m_BuyerBidItemId);
        ItemManager.Instance.AddItem(m_iSellerItemId);
        if (null != m_BidResponseCallBack)
        {
            m_BidResponseCallBack(true);
        }
    }
    public void OnCloseSaleResponse(CloseSaleResponse resp)
    {
        m_bIsClosing = false;
        if (resp.Success)
        {
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
            WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
            ChangeMTStatus(MTStatus.None);
            WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
        }
        else
        {
            TipManager.Instance.Alert("", "你已经掉线", "OK", (res) => { ClearSale(); });
        }
    }
    private void PlayAudioTip(MTMessageType type)
    {
        switch(type)
        {
            case MTMessageType.chatClosed:
                //PlayAudio("");
                break;
            case MTMessageType.saleClosed:
                //PlayAudio("");
                break;
            case MTMessageType.aotherCloseChat:
                PlayAudio("Yindaoyu_#121a4_G_D");
                break;
            case MTMessageType.aotherCloseSale:
                PlayAudio("Yindaoyu_#121a4_G_D");
                break;
            case MTMessageType.anotherAway:
                PlayAudio("Yindaoyu_#121a4_G_D");
                break;
            case MTMessageType.chatTimeout:
                PlayAudio("Yindaoyu_#121a3_G_D");
                break;
            case MTMessageType.saleTimeout:
                PlayAudio("Yindaoyu_#121a3_G_D");
                break;
        }
    }
    private void PlayAudio(string name)
    {
        name = "GUIDE/MessageTree/" + name;
        if(!AudioPlayer.Instance.IsPlayingAudio(name))
        {
            AudioPlayer.Instance.PlayAudio(name, Vector3.zero, false);
        }        
    }
    private bool CheckSaleId(long saleId)
    {
        return m_iSaleId == saleId;
    }
    private bool CheckChatId(long chatId)
    {
        return m_iChatId == chatId;
    }
    public void ClearSale()
    {
        WindowManager.Instance.CloseWindow(WindowID.MsgTreeBuy);
        WindowManager.Instance.CloseWindow(WindowID.MsgTreeSell);
        ChangeMTStatus(MTStatus.None);
        ChangeChatStatus(ChatStatus.DisConnect);
        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
    }
    #endregion

    #region buy
    private void EnterBuy()
    {
        LifeTickTask.Instance.RegisterToUpdateList(Update);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_MESSAGE_EVENT_LIST, OnEventListResponse);
        m_bIsEnalbeChat = true;
    }
    private void ExitBuy()
    {
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_MESSAGE_EVENT_LIST, OnEventListResponse);
        m_bIsEnalbeChat = false;
    }
    public void OnBuyResponse(BuyResponse resp)
    {
        if(CheckToTip(resp, null))
        {
            m_iSellerItemId = resp.ItemId;
            m_iChatId = resp.ChatId;
            m_iSaleId = resp.SaleId;
            ChangeMTStatus(MTStatus.Buy);
            ChangeChatStatus(ChatStatus.ConnectFree);
            WindowManager.Instance.OpenWindow(WindowID.MsgTreeBuy);
            if(null != m_OnInitBuyDealCallBack)
            {
                m_OnInitBuyDealCallBack(resp);
            }
        }
        else
        {
            TipManager.Instance.Alert("没有在线卖家");
            // show tip
            WindowManager.Instance.OpenWindow(WindowID.MsgTreeSelectPanel);
        }
    }
    public void OnBidItem(int id)
    {
        BidRequest request = new BidRequest();
        request.OfferId = id;
        request.ChatId = m_iChatId;
        m_BuyerBidItemId = id;
    AsyncBidRequest async = new AsyncBidRequest(request);
        async.TryRequest();
    }
    public void OnBidResponse(BidResponse resp)
    {
        if(null != m_OnBuyBidRespCallBack)
        {
            m_OnBuyBidRespCallBack(resp);
        }
    }
    public void RegisterInitBuyDeal(Action<BuyResponse> callBack)
    {
        m_OnInitBuyDealCallBack = callBack;
    }
    public void RegisterBidResp(Action<BidResponse> callBack)
    {
        m_OnBuyBidRespCallBack = callBack;
    }
    public void RegisterBidResponse(Action<bool> callBack)
    {
        m_BidResponseCallBack = callBack;
    }
    #endregion

    #region sell
    public void OnClickSellItem(int itemId, Action CallBack)
    {
        if (m_CurrentState == MTStatus.Sell)
        {
            PlayAudio("Yindaoyu_#121a1_G_D");
            return;
        }
        m_OnSellCallBack = CallBack;
        WindowManager.Instance.OpenWindow(WindowID.Loading);
        SellRequest request = new SellRequest();
        request.ItemId = itemId;
        m_SellerBidItemId = itemId;
        AsyncSellRequest async = new AsyncSellRequest(request);
        async.TryRequest();
    }
    private void EnterSell()
    {
        LifeTickTask.Instance.RegisterToUpdateList(Update);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_MESSAGE_EVENT_LIST, OnEventListResponse);
        m_bIsEnalbeChat = true;

        //notic ui
        if(null != m_OnSellCallBack)
        {
            m_OnSellCallBack();
        }
    }
    private void ExitSell()
    {
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_MESSAGE_EVENT_LIST, OnEventListResponse);
        m_bIsEnalbeChat = false;
    }
    public void OnSellResponse(SellResponse resp)
    {
        m_iSaleId = resp.SaleId;

        WindowManager.Instance.OpenWindow(WindowID.MsgTreeSell);
        if(CheckToTip(null,resp))
        {
            ChangeMTStatus(MTStatus.Sell);
        }
        else
        {
            // show tip
            TipManager.Instance.Alert("当前图纸不可出售");
        }
    }
    public void OnSellAcceptResponse(AcceptBidResponse resp)
    {
        if(resp.Success)
        {
            ItemManager.Instance.RemoveItem(m_SellerBidItemId);
            ItemManager.Instance.AddItem(m_iBuyerBidItemId);
        }
        if(null != m_OnAccRefCallBack)
        {
            m_OnAccRefCallBack(true, resp.Success);
        }
    }
    public void OnSellRefuseResponse(RefuseBidResponse resp)
    {
        if (null != m_OnAccRefCallBack)
        {
            m_OnAccRefCallBack(false, resp.Success);
        }
    }
    public void OnClickAcceptBid()
    {
        AcceptBidRequest request = new AcceptBidRequest();
        request.ChatId = m_iChatId;
        AsyncAcceptBidRequest async = new AsyncAcceptBidRequest(request);
        async.TryRequest();
    }
    public void OnClickRefuseBid()
    {
        RefuseBidRequest request = new RefuseBidRequest();
        request.ChatId = m_iChatId;
        AsyncRefuseBidRequest async = new AsyncRefuseBidRequest(request);
        async.TryRequest();
    }
    public void RegisterBidCallBack(Action<int> callBack)
    {
        m_OnSellerBidCallBack = callBack;
    }
    public void REgisterSellerOpenChatCallBack(Action<OpenChatEvent> callBack)
    {
        m_OnOpenChatCallBack = callBack;
    }
    public void RegisterSellerAccRefCallBack(Action<bool, bool> callBack)
    {
        m_OnAccRefCallBack = callBack;
    }
    public void RegisterSellerInitBuyerCallBack(Action callBack)
    {
        m_OnResetBuyerCallBack = callBack;
    }
    #endregion

    #region chat
    public void EndRecord()
    {
        if (m_CurrentChatStatus == ChatStatus.Record)
        {
            MicroPhoneInput.getInstance().StopRecord();
            ChangeChatStatus(ChatStatus.CD);
            UploadChat();
        }
        // TEST CODE
        //MicroPhoneInput.getInstance().StopRecord();
        //UploadChat();
    }
    public void StartRecord()
    {
        //// TEST CODE
        //AudioSource aud1 = AudioPlayer.Instance.GetRecordAudioSource();
        //MicroPhoneInput.getInstance().StartRecord(10, ref aud1);

        ChangeChatStatus(ChatStatus.Record);
        if (m_CurrentChatStatus == ChatStatus.Record)
        {
            //start record
            AudioSource aud = AudioPlayer.Instance.GetRecordAudioSource();
            MicroPhoneInput.getInstance().StartRecord(10, ref aud);
        }
        else if(m_CurrentChatStatus == ChatStatus.CD)
        {
            PlayAudio("Yindaoyu_#121a8_G_D");
        }
    }
    public void PlayRecord(ChatInfoEvent voice)
    {
        // trigger to download
        DownloadChat(voice.VoiceName, voice.VoiceDate);
        // and try to play
        Debuger.Log("play voice " + voice.VoiceDate + voice.VoiceName);
    }
    private void ChangeChatStatus(ChatStatus status)
    {
        if(status == m_CurrentChatStatus)
        {
            return;
        }
        switch(status)
        {
            case ChatStatus.ConnectFree:
                {
                    if (m_CurrentChatStatus == ChatStatus.Playing || m_CurrentChatStatus == ChatStatus.CD || m_CurrentChatStatus == ChatStatus.DisConnect)
                    {
                        m_CurrentChatStatus = status;
                    }
                }
                break;
            case ChatStatus.CD:
                {
                    if (m_CurrentChatStatus == ChatStatus.Record)
                    {
                        m_CurrentChatStatus = status;
                    }
                }
                break;
            case ChatStatus.Playing:
                {
                    if (m_CurrentChatStatus != ChatStatus.ConnectFree)
                    {
                        //add to wait for play list list
                    }
                    else
                    {
                        m_CurrentChatStatus = status;
                    }                          
                }
                break;
            case ChatStatus.Record:
                {
                    if (m_CurrentChatStatus == ChatStatus.ConnectFree)
                    {
                        m_CurrentChatStatus = status;
                    }
                }
                break;
             case ChatStatus.DisConnect:
                {
                    m_CurrentChatStatus = status;
                }
                break;
        }
    }
    private void UpdateChat()
    {
        if(m_CurrentChatStatus == ChatStatus.CD)
        {
            m_fCurrentChatCd += TimeManager.Instance.GetDeltaTime();
            if(m_fCurrentChatCd >= m_fChatCd)
            {
                m_fCurrentChatCd = 0;
                ChangeChatStatus(ChatStatus.ConnectFree);
            }
        }
        if(m_CurrentChatStatus == ChatStatus.Playing)
        {
            m_fCurrentPlayedAudioLength += TimeManager.Instance.GetDeltaTime();
            if(m_fCurrentPlayedAudioLength >= m_fCurrentPlayingAudioLength)
            {
                m_fCurrentPlayedAudioLength = 0;
                ChangeChatStatus(ChatStatus.ConnectFree);
            }
        }
        CheckPlayList();
    }
    private void AddToWaitingForPlayList(AudioClip clip)
    {
        if(null == m_WaitingForPlayList)
        {
            m_WaitingForPlayList = new List<AudioClip>();
        }
        m_WaitingForPlayList.Add(clip);
    }
    private void TriggerToPlayAudio()
    {
        AudioClip clip = m_WaitingForPlayList[0];
        m_fCurrentPlayedAudioLength = 0;
        m_fCurrentPlayingAudioLength = clip.length;
        AudioPlayer.Instance.PlayAudio(clip, Vector3.zero, false);
        m_WaitingForPlayList.RemoveAt(0);
    }
    private void CheckPlayList()
    {
        if(m_WaitingForPlayList.Count > 0 && m_CurrentChatStatus != ChatStatus.Playing)
        {
            ChangeChatStatus(ChatStatus.Playing);
            if(m_CurrentChatStatus == ChatStatus.Playing)
            {
                // triggerto play
                TriggerToPlayAudio();
            }
        }
    }
    public void OnSendChatResponse(SendChatResponse resp)
    {
        if (!resp.Success)
        {
            TipManager.Instance.Alert("发送语音失败，请重录");
        }
    }
    private void UploadChat()
    {
        var array = MicroPhoneInput.ConvertClipToByte(AudioPlayer.Instance.GetRecordAudioSource().clip);
        if (null == array)
        {
            return;
        }

        Debuger.Log("array length : " + array.Length);

        List<KeyValuePair<string, object>> param = new List<KeyValuePair<string, object>>();
        param.Add(new KeyValuePair<string, object>("charId", PlayerManager.Instance.GetCharBaseData().CharId));
        
        string resp = HttpUtil.UploadFile(HttpManager.CHAT_UPLOAD_URL, "testFile.bytes", array, param, "", "");
        OnUploadChatFinishCallBack(resp);
    }
    private void DownloadChat(string voicename,string date)
    {
        string url = HttpManager.CHAT_DOWNLOAD_URL + "/" + date + "/" + voicename;

        AssetFile remoteVersionFile = new AssetFile(voicename, "", url);

        List<AssetFile> tmpDownloadList = new List<AssetFile>();
        tmpDownloadList.Add(remoteVersionFile);

        //trigger download remote versin config
        AssetsDownloader_Sync.Instance.BeginDownload(
            tmpDownloadList,
            (file, fileInfo) =>
            {
                OnDownloadChatFinishCallBack(file);
            },
            (e, fileInfo) =>
            {
            },
            (process, fileInfo) =>
            {

            },
            () =>
            {
                
            });
    }
    public void OnUploadChatFinishCallBack(string jsonContent)
    {
        //decode json
        Dictionary<string, object> root = Json.Deserialize(jsonContent) as Dictionary<string, object>;
        if(!root.ContainsKey("errCode"))
        {
            TipManager.Instance.Alert("发生未知错误，请重试");
            return;
        }
        string errorCode = root["errCode"] as string;
        if(!string.IsNullOrEmpty(errorCode))
        {
            TipManager.Instance.Alert("发生未知错误，请重试");
            return;
        }

        string voicename = root["voiceName"] as string;
        string date = root["date"] as string;

        SendChatRequest request = new SendChatRequest();
        request.ChatId = m_iChatId;
        request.VoiceDate = date;
        request.VoiceName = voicename;

        AsyncSendChatRequest async = new AsyncSendChatRequest(request);
        async.TryRequest();

        // TEST CODE
        //DownloadChat(voicename,date);
    }
    public void OnDownloadChatFinishCallBack(byte[] file)
    {
        //decode byte to audio clip
        AudioClip clip = MicroPhoneInput.ConvertByteToClip(file);
        AddToWaitingForPlayList(clip);
        
        // TEST CODE
        //AudioPlayer.Instance.PlayAudio(clip, Vector3.zero, false);
    }
    #endregion

    public static string ConvertIdToName(int id)
    {
        switch(id)
        {
            case 0:
                return "firecat";
            case 1:
                return "firestar";
            case 2:
                return "fireheart";
            case 3:
                return "fireround";
            case 4:
                return "firehand";
            case 5:
                return "firemoney";
            case 6:
                return "firebig";
            default:
                return string.Empty;
        }
    }
    public static int ConvertNameToId(string name)
    {
        if(name == "firecat")
        {
            return 0;
        }
        if (name == "firestar")
        {
            return 1;
        }
        if (name == "fireheart")
        {
            return 2;
        }
        if (name == "fireround")
        {
            return 3;
        }
        if (name == "firehand")
        {
            return 4;
        }
        if (name == "firemoney")
        {
            return 5;
        }
        if (name == "firebig")
        {
            return 6;
        }
        return -1;
    }
}
