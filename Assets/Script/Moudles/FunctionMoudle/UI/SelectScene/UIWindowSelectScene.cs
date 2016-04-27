using UnityEngine;
using System.Collections.Generic;

public class UIWindowSelectScene : WindowBase
{
    private GameObject m_ObjWorldListRoot;
    private GameObject m_ObjNodeGameListRoot;
    private GameObject m_ObjDebugRoot;
    private GameObject m_ObjReleaseRoot;
    private List<GameObject> m_ObjGuideList;

    //test code
    public static  int m_iIndex = -1;

    public override void OnInit()
    {
        base.OnInit();

        m_ObjDebugRoot = FindChild("DebugRoot");
        m_ObjReleaseRoot = FindChild("ReleaseRoot");

        m_ObjWorldListRoot = FindChild("WorldRoot");
        m_ObjNodeGameListRoot = FindChild("NodeGameRoot");
        AddChildElementClickEvent(OnClickLeft, "UIButton_Left");
        AddChildElementClickEvent(OnClickRight, "UIButton_Right");
		AddChildElementClickEvent(OnClickBack, "Button_Exit");
        m_ObjGuideList = new List<GameObject>();

        for (int i = 0; i < m_ObjWorldListRoot.transform.childCount; ++i)
        {
            UIEventListener.Get(m_ObjWorldListRoot.transform.GetChild(i).gameObject).onClick = OnClickWorldGame;
        }
        for (int i = 0; i < m_ObjNodeGameListRoot.transform.childCount; ++i)
        {
            UIEventListener.Get(m_ObjNodeGameListRoot.transform.GetChild(i).gameObject).onClick = OnClickNodeGame;
        }

        for (int i = 0; i < 5; ++i)
        {
            AddChildElementClickEvent(OnClickReleaseGame, "Scene_" + i);
        }
        for (int i = 0; i < 5; ++i)
        {
            m_ObjGuideList.Add(FindChild("Guide_" + i));
            m_ObjGuideList[i].SetActive(false);
        }
        m_ObjDebugRoot.SetActive(AppManager.Instance.m_bIsDebugMode);
        m_ObjReleaseRoot.SetActive(!AppManager.Instance.m_bIsDebugMode);
        if (!AppManager.Instance.m_bIsDebugMode)
        {
            CheckShowGuide();
        }
    }
    private void OnClickRight(GameObject go)
    {
        m_ObjNodeGameListRoot.SetActive(false);
        m_ObjWorldListRoot.SetActive(true);
    }
    private void OnClickLeft(GameObject go)
    {
        m_ObjNodeGameListRoot.SetActive(true);
        m_ObjWorldListRoot.SetActive(false);
        
    }
	void OnClickBack (GameObject go)
	{
		StageManager.Instance.ChangeState (GameStateType.LoginState);
	}
    private void OnClickReleaseGame(GameObject go)
    {
        int index = 0;
        if (!int.TryParse(go.name.Substring(6), out index))
        {
            Debuger.LogWarning("error on decode scene id");
            return;
        }

        m_iIndex = -1;

        switch (index)
        {
            case 0:
                m_iIndex = 0;
                //MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 0));
                break;
            case 1:
                if (!GetStageIsUnlock(11))
                {
                    return;
                }
                m_iIndex = 3;
                //MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 3));
                break;
            case 2:
                if (!GetStageIsUnlock(12))
                {
                    return;
                }
                m_iIndex = 8;
                //MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 8));
                break;
            case 3:
                if (!GetStageIsUnlock(13))
                {
                    return;
                }
                m_iIndex = 9;
                //MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 9));
                break;
            case 4:
                if (!GetStageIsUnlock(14))
                {
                    return;
                }
                m_iIndex = 5;
                //MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 5));
                break;
        }
        if(-1 != m_iIndex)
        {
            StageManager.Instance.ChangeState(GameStateType.TestPreFlightStage);
        }
    }
    //test code
    public static void OnExitFlightGame()
    {
        if (-1 == m_iIndex)
        {
            return;
        }
        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, m_iIndex));
        m_iIndex = -1;
    }
    private void OnClickWorldGame(GameObject go)
    {
        int index = 0;
        if (!int.TryParse(go.name.Substring(6), out index))
        {
            Debuger.LogWarning("error on decode scene id");
            return;
        }

        switch (index)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, index));
                break;
                
            case 11:
                AdaptiveDifficultyManager.Instance.ResetUserTalentToDefault();
                Debuger.Log("reset user talent");
                break;
        }
    }
    private void OnClickNodeGame(GameObject go)
    {
        int index = 0;
        if (!int.TryParse(go.name.Substring(6), out index))
        {
            Debuger.LogWarning("error on decode scene id");
            return;
        }

        switch (index)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_NODE_GAME, index));
                break;
                
        }
    }
    private bool GetStageIsUnlock(int index)
    {
        return PlayerManager.Instance.GetCharCounterData().GetFlag(index);
    }
    private void CheckShowGuide()
    {
        int index = GetCurrentSelectedIndex();
        for (int i = 0; i < m_ObjGuideList.Count; ++i)
        {
            m_ObjGuideList[i].SetActive(i == index);
        }
    }
    private int GetCurrentSelectedIndex()
    {
        int index = 1;
        for (int i = 11; i < 15; ++i,++index)
        {
            if (!PlayerManager.Instance.GetCharCounterData().GetFlag(i))
            {
                return index-1;
            }
        }
        return -1;
    }
}
