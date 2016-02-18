using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Config.Table;
using Config;
using System;

namespace RegularityGame
{
    public class PilesAlphaElement
    {
        public PilesAlphaElement(GameObject objRoot,string name, bool isVisable,string emptyTextureName)
        {
            m_ObjRoot = objRoot;
            m_ObjOptionRoot = ComponentTool.FindChild("Option", m_ObjRoot);
            m_ObjPlayerAttachRoot = ComponentTool.FindChild("PlayerAttachPos", m_ObjRoot);
            m_strName = name;
            m_bIsVisable = isVisable;

            m_MeshRenderer = m_ObjOptionRoot.GetComponent<MeshRenderer>();
            m_Materials = new Material(ResourceManager.Instance.LoadBuildInResource<Material>("Piles/PilesMaterial", AssetType.Materials));
            if (m_bIsVisable)
            {
                m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(m_strName, AssetType.Texture);
            }
            else
            {
                m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(emptyTextureName, AssetType.Texture);
            }
            m_MeshRenderer.material = m_Materials;
        }

        public GameObject   m_ObjOptionRoot;
        public GameObject   m_ObjRoot;
        public GameObject   m_ObjPlayerAttachRoot;
        public Material     m_Materials;
        public MeshRenderer m_MeshRenderer;
        public string       m_strName;
        public bool         m_bIsVisable;
    }
    public enum RegularityStatus
    {
        InitScene,
        Repostion,
        JumpToTarget,
        WaitForInput,
        Win,
        Lose,
    }
    public class RegularityAlphaGameLogic : SingletonTemplateMon<RegularityAlphaGameLogic>
    {
        public GameObject m_ObjElementRoot;
        public GameObject m_ObjElementTemplate;
        public GameObject m_ObjPlayer;
        public string m_strEmptyTextureName;

        //
        public float m_fG;

        //jump param
        public float m_fJumpStartAnimPlayTime;
        public float m_fJumpEndAnimPlayTime;
        public float m_fJumpStartMoveTime;
        public float m_fJumpEndMoveTime;
        public float m_fJumpEndTime;
        public float m_fPlayAnimCrossDuringTime;

        //piles
        public float m_fMovePilesTop;
        public float m_fMovePilesBottom;
        public float m_fMovePilesDuringtime;
        public Vector3 m_InitScale;
        public float m_fShowOptionDuringTime;

        private Animator m_PlayerAnim;
        private SimpleUIGrid m_Grid;
        private RegularityGameConfig m_CurrentConfig;
        private RegularityGameSettingTable m_GameSettingConfig;
        private RegularityGameDifficultyManager m_DiffMgr;
        private List<PilesAlphaElement> m_ElementList;
        private UIWindowRegularity m_UIWindow;
        private bool m_bIsJump;
        private int m_iLeftRedFlower;
        private int m_iLeftWinCount;

        private float m_fSpeedX;
        private float m_fSpeedY;
        private float m_fInitTime;
        private Vector3 m_vDir;
        private Vector3 m_vInitPos;
        private float m_fCurrentTime;
        private bool m_bIsTriggerJumpStarAnim;
        private bool m_bIsTriggerJumpEndAnim;
        private int m_iCurrentJumpedIndex;
        private int m_iCurrentOptionIndex;
        private int m_iGoalIndex;
        private bool m_bIsCorrect;
        private RegularityStatus m_Status;
        private string m_strCuringCaculateName;
        private bool m_bLastStatusIsWin;
        private int m_iKeepStatusCount;


        #region public interface
        public void Initialize()
        {
            m_ElementList = new List<PilesAlphaElement>();
            m_GameSettingConfig = ConfigManager.Instance.GetRegularityGameSetting();
            m_DiffMgr = new RegularityGameDifficultyManager();
            m_PlayerAnim = m_ObjPlayer.GetComponent<Animator>();
            m_Grid = m_ObjElementRoot.GetComponent<SimpleUIGrid>();

            ReloadScene();

            m_iLeftRedFlower = 3;
            m_iLeftWinCount = m_GameSettingConfig.PlayCountLimit;
            m_UIWindow.SetLeftCount(m_GameSettingConfig.PlayCountLimit);
            m_UIWindow.SetLeftTime(m_GameSettingConfig.PlayTime);
            m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
        }
        public void OnReset()
        {
            m_iLeftRedFlower = 3;
            m_iLeftWinCount = m_GameSettingConfig.PlayCountLimit;
            m_UIWindow.SetLeftCount(m_GameSettingConfig.PlayCountLimit);
            m_UIWindow.SetLeftTime(m_GameSettingConfig.PlayTime);
            m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
            m_iKeepStatusCount = -1;
            ReloadScene();
        }
        public void OnBack()
        {
            WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
            //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
        }
        #endregion

        #region system
        void Awake()
        {
            _instance = this;
        }
        private void ReloadScene()
        {
            ClearScene();
            ChangeToStatus(RegularityStatus.InitScene);
            m_CurrentConfig = m_DiffMgr.GetDifficulty();
            for (int i = 0; i < m_CurrentConfig.OptionList.Count; ++i)
            {
                string name = m_CurrentConfig.OptionList[i].Name;
                bool isVisable = m_CurrentConfig.OptionList[i].IsVisable;
                GameObject elem = GameObject.Instantiate(m_ObjElementTemplate);
                elem.name = i.ToString();

                PilesAlphaElement dataElem = new PilesAlphaElement(elem, name, isVisable, m_strEmptyTextureName);
                ComponentTool.Attach(m_ObjElementRoot.transform, dataElem.m_ObjRoot.transform);
                m_ElementList.Add(dataElem);

                if (!isVisable)
                {
                    m_iCurrentOptionIndex = i-1;
                }
            }
            m_Grid.Reposition();
            SetPlayerPos(0);

            OpenWindow();
            
            m_UIWindow.ResetAnswer(m_CurrentConfig.AnswerList);
            m_iCurrentJumpedIndex = 0;
            TriggerJumpToTarget(1);
        }
        private void ClearScene()
        {
            for (int i = 0; i < m_ElementList.Count; ++i)
            {
                m_ElementList[i].m_ObjRoot.transform.parent = null;
                GameObject.Destroy(m_ElementList[i].m_ObjRoot);
            }
            m_ElementList.Clear();
        }
        private void SetPlayerPos(int index)
        {
            m_ObjPlayer.transform.position = m_ElementList[index].m_ObjPlayerAttachRoot.transform.position;
        }
        private void SetOption(bool isCorrect)
        {
            if(m_Status != RegularityStatus.WaitForInput)
            {
                Debuger.Log("system busy");
                return;
            }
            m_bIsCorrect = isCorrect;
            if (isCorrect)
            {
                m_iGoalIndex = m_ElementList.Count - 1;
            }
            else
            {
                m_iGoalIndex = m_iCurrentJumpedIndex + 1;
            }
            TriggerJumpToTarget(++m_iCurrentJumpedIndex);
            ChangeToStatus(RegularityStatus.JumpToTarget);
        }
        private void OnJumpFinished()
        {
            if(m_bIsCorrect)
            {
                ReportCorrectEvent();

                --m_iLeftWinCount;
                if (m_iLeftWinCount <= 0)
                {
                    //show window
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Win);
                    ChangeToStatus(RegularityStatus.Win);
                    m_UIWindow.OnWin();
                }
                else
                {
                    ReloadScene();
                }
            }
            else
            {
                ReportWrongEvent();

                --m_iLeftRedFlower;
                m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
                if (m_iLeftRedFlower <= 0)
                {
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Lose);
                    ChangeToStatus(RegularityStatus.Lose);
                    m_UIWindow.OnLose();
                }
                else
                {
                    //jump back
                    TriggerJumpToTarget(--m_iCurrentJumpedIndex);
                    ChangeToStatus(RegularityStatus.Repostion);
                    TriggerToShowPilesAnim();
                }
                
            }
        }
        private void ChangeToStatus(RegularityStatus status)
        {
            m_Status = status;
        }
        private void OpenWindow()
        {
            RegularityWindowParam param = new RegularityWindowParam();
            param.m_OptionalList = m_CurrentConfig.AnswerList;
            param.onReleaseCallBack = OnOptionalReleaseCallBack;
            param.configTable = ConfigManager.Instance.GetRegularityGameConfig(); ;
            WindowManager.Instance.OpenWindow(WindowID.Regularity, param);
            m_UIWindow = (WindowManager.Instance.GetWindow(WindowID.Regularity)) as UIWindowRegularity;
        }
        private void OnOptionalReleaseCallBack(string arg1, Vector3 arg2)
        {
            if (m_Status != RegularityStatus.WaitForInput)
            {
                return;
            }
            for (int i = 0; i < m_ElementList.Count; ++i)
            {
                if (!m_ElementList[i].m_bIsVisable)
                {
                    //ChangeToStatus(RegularityStatus.JumpToTarget);
                    m_ElementList[i].m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>(arg1, AssetType.Texture);
                    m_UIWindow.OnDisableOption(arg1);
                    TriggerToShowEnableOption(m_ElementList[i].m_ObjOptionRoot, () =>
                    {
                        m_strCuringCaculateName = arg1;
                        SetOption(arg1 == m_ElementList[i].m_strName);
                    });
                    
                    break;
                }
            }
        }
        private void ReportCorrectEvent()
        {
            //event report
            if (m_bLastStatusIsWin)
            {
                ++m_iKeepStatusCount;
            }
            else
            {
                m_iKeepStatusCount = 0;
            }
            m_bLastStatusIsWin = true;
            switch (m_iKeepStatusCount)
            {
                case 0:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Correct);
                    break;
                case 1:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Correct2);
                    break;
                default:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Correct3);
                    break;
            }
        }
        private void ReportWrongEvent()
        {
            //event report
            if (!m_bLastStatusIsWin)
            {
                ++m_iKeepStatusCount;
            }
            else
            {
                m_iKeepStatusCount = 0;
            }
            m_bLastStatusIsWin = false;
            switch (m_iKeepStatusCount)
            {
                case 0:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Wrong);
                    break;
                case 1:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Wrong2);
                    break;
                default:
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Wrong3);
                    break;
            }
        }
        #endregion

        #region jump
        private void JumpMove(float deltaTime)
        {
            Vector3 tmpVector3 = m_ObjPlayer.transform.position;
            tmpVector3 = m_vInitPos + m_vDir * deltaTime * m_fSpeedX;
            tmpVector3.y = m_vInitPos.y + deltaTime * m_fSpeedY - 0.5f * m_fG * deltaTime * deltaTime;
            m_ObjPlayer.transform.position = tmpVector3;
        }
        private void TriggerJumpStarAnim()
        {
            PlayAnim("JumpUp");
        }
        private void TriggerJumpEndAnim()
        {
            PlayAnim("JumpDown");
        }
        private void ClearJump()
        {
            m_fCurrentTime = 0.0f;
            m_bIsJump = false;
            m_bIsTriggerJumpEndAnim = false;
            m_bIsTriggerJumpStarAnim = false;
            ResetPlayerPos();

            OnJumpDone();
        }
        private void PlayAnim(string name)
        {
            m_PlayerAnim.CrossFade(name, m_fPlayAnimCrossDuringTime);
        }
        private void TriggerJumpToTarget(int index)
        {
            //check index
            if (index < 0 || index >= m_ElementList.Count)
            {
                return;
            }
            m_iCurrentJumpedIndex = index;
            float time = m_fJumpEndMoveTime - m_fJumpStartMoveTime;
            Vector3 targetPos = m_ElementList[index].m_ObjPlayerAttachRoot.transform.position;            
            m_fSpeedX = Vector3.Distance(m_ObjPlayer.transform.position, targetPos) / time;
            m_fSpeedY = (float)(0.5f * m_fG * time);
            m_vDir = targetPos - m_ObjPlayer.transform.position;
            m_vDir.Normalize();
            m_vInitPos = m_ObjPlayer.transform.position;
            m_bIsJump = true;
        }
        private void OnJumpDone()
        {
            if(m_Status == RegularityStatus.InitScene || m_Status == RegularityStatus.Repostion)
            {
                if (m_iCurrentJumpedIndex < m_iCurrentOptionIndex)
                {
                    TriggerJumpToTarget(++m_iCurrentJumpedIndex);
                }
                else
                {
                    ChangeToStatus(RegularityStatus.WaitForInput);
                }
            }
            if (m_Status == RegularityStatus.JumpToTarget)
            {
                if (m_iCurrentJumpedIndex < m_iGoalIndex)
                {
                    TriggerJumpToTarget(++m_iCurrentJumpedIndex);
                }
                else
                {
                    OnJumpFinished();
                }
            }
            
        }
        private void ResetPlayerPos()
        {
            SetPlayerPos(m_iCurrentJumpedIndex);
        }
        #endregion

        #region piles move
        private void TriggerToShowPilesAnim()
        {
            GameObject obj = m_ElementList[m_iCurrentOptionIndex + 1].m_ObjRoot;
            TweenPosition tween = obj.AddComponent<TweenPosition>();
            tween.from = obj.transform.localPosition;
            tween.from.y = m_fMovePilesTop;
            tween.to = obj.transform.localPosition;
            tween.to.y = m_fMovePilesBottom;
            tween.worldSpace = false;
            tween.duration = m_fMovePilesDuringtime;
            tween.AddOnFinished(OnMovePilesBottomFinished);
        }
        private void OnMovePilesBottomFinished()
        {
            GameObject obj = m_ElementList[m_iCurrentOptionIndex + 1].m_ObjRoot;
            TweenPosition tween = obj.GetComponent<TweenPosition>();
            Destroy(tween);
            tween = obj.AddComponent<TweenPosition>();
            tween.from = obj.transform.localPosition;
            tween.from.y = m_fMovePilesBottom;
            tween.to = obj.transform.localPosition;
            tween.to.y = m_fMovePilesTop;
            tween.worldSpace = false;
            tween.duration = m_fMovePilesDuringtime;
            tween.AddOnFinished(OnMovePilesTopFinished);
            tween.enabled = true;
        }
        private void OnMovePilesTopFinished()
        {
            GameObject obj = m_ElementList[m_iCurrentOptionIndex + 1].m_ObjRoot;
            TweenPosition tween = obj.GetComponent<TweenPosition>();
            Destroy(tween);
            TriggerToShowDisableOption(m_ElementList[m_iCurrentOptionIndex + 1].m_ObjOptionRoot, () =>
            {
                m_ElementList[m_iCurrentOptionIndex + 1].m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(m_strEmptyTextureName,
                           AssetType.Texture);

                m_UIWindow.OnEnableOption(m_strCuringCaculateName);
            });
        }
        private void TriggerToShowEnableOption(GameObject optionObj, Action CallBack)
        {
            TweenScale tween = optionObj.GetComponent<TweenScale>();
            if (null != tween)
            {
                GameObject.Destroy(tween);
            }
            tween = optionObj.AddComponent<TweenScale>();
            tween.from = Vector3.zero;
            tween.to = m_InitScale;
            tween.duration = m_fShowOptionDuringTime;
            tween.AddOnFinished(() =>
            {
                GameObject.Destroy(tween);
                CallBack();
            });
        }
        private void TriggerToShowDisableOption(GameObject optionObj, Action CallBack)
        {
            TweenScale tween = optionObj.GetComponent<TweenScale>();
            if (null != tween)
            {
                GameObject.Destroy(tween);
            }
            tween = optionObj.AddComponent<TweenScale>();
            tween.from = m_InitScale;
            tween.to = Vector3.zero;
            tween.duration = m_fShowOptionDuringTime;
            tween.AddOnFinished(() =>
            {
                GameObject.Destroy(tween);
                optionObj.transform.localScale = m_InitScale;
                CallBack();
            });
        }
        #endregion

        void Update()
        {            
            if (m_bIsJump)
            {
                float deltaTime = TimeManager.Instance.GetDeltaTime();
                m_fCurrentTime += deltaTime;

                if (m_fCurrentTime >= m_fJumpStartAnimPlayTime && !m_bIsTriggerJumpStarAnim)
                {
                    TriggerJumpStarAnim();
                    m_bIsTriggerJumpStarAnim = true;
                }
                if (m_fCurrentTime >= m_fJumpEndAnimPlayTime && !m_bIsTriggerJumpEndAnim)
                {
                    TriggerJumpEndAnim();
                    m_bIsTriggerJumpEndAnim = true;
                }
                if (m_fCurrentTime >= m_fJumpStartMoveTime && m_fCurrentTime <= m_fJumpEndMoveTime)
                {
                    JumpMove(m_fCurrentTime - m_fJumpStartMoveTime);
                }
                if (m_fCurrentTime >= m_fJumpEndTime)
                {
                    ClearJump();
                }
            }
        }
    }
}