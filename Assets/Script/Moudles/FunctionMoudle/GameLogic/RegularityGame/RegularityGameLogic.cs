using System;
using Config;
using Config.Table;
using System.Collections.Generic;
using UnityEngine;

namespace RegularityGame
{
    public class PilesElement
    {
        public PilesElement(GameObject optionRoot,GameObject obj, string mame, Material material, bool isVisable)
        {
            m_OptionRoot = optionRoot;
            m_ObjRoot = obj;
            m_strName = mame;
            m_Materials = material;
            m_bIsVisable = isVisable;
        }

        public GameObject m_OptionRoot;
        public GameObject m_ObjRoot;
        public Material m_Materials;
        public string m_strName;
        public bool m_bIsVisable;
    }

    public class RegularityGameLogic : SingletonTemplateMon<RegularityGameLogic>
    {
        public Camera m_SceneCamera;
        public GameObject m_ObjRoot;
        public GameObject m_ElementTemplate;
        public int m_iCount;
        public GameObject m_ObjLookatTarget;
        public float m_fSpace;
        public float m_fPlayerFixY;
        public GameObject m_Player;
        public float m_fJumpSpendTime;
        public float m_fG;
        public float m_fResetPosSpendTime;
        public float m_fMovePilesTop;
        public float m_fMovePilesBottom;
        public float m_fMovePilesDuringtime;
        public float m_fShakeRange;
        public float m_fShakeDuringtime;
        public string m_strEmptyTextureName;
        public Vector3 m_InitScale;
        public float m_fShowOptionDuringTime;
        public float m_fJumpUpAnimLengthOffset;
        public float m_fjumpDownAnimLengthOffset;
        public float m_fJumpDownPlayOffset;
        public float m_fPlayerFixZ;

        private Animator m_PlayerAnim;
        private SimpleUIGrid m_Grid;
        private float m_fHalfLength;
        private float m_fMinX;
        private List<PilesElement> m_ElementList;
        private UIWindowRegularity m_UIWindow;
        private bool m_bIsJumping;
        private float m_fInitTime;
        private float m_fVx;
        private float m_fVy;
        private Vector3 m_vDir;
        private Vector3 m_vInitPos;
        private int m_iCurrentJumpIndex;
        private int m_iTargetJumpIndex;
        private bool m_bIsSucceed;
        private bool m_bIsResetPos;
        private bool m_bIsMovingPiles;
        private int m_iMovingPilesIndex;
        private RegularityGameConfig m_CurrentConfig;
        private float m_fLeftTime;
        private int m_iLimitCount;
        private bool m_bIsInit;
        private RegularityGameSettingTable m_GameSettingConfig;
        private bool m_bIsFinished;
        private RegularityGameDifficultyManager m_DiffMgr;
        private bool m_bIsResetByUI;
        private bool m_bLastStatusIsWin;
        private int m_iKeepStatusCount;
        private Action m_OnShakeEndCallBack;
        private bool m_bIsShake;
        private int m_iLeftRedFlower;
        private string m_strCuringCaculateName;
        private bool m_bIsPlayingAnim;
        private Action m_OnPlayEndCallBack;
        private float m_fCurringPlaytime;
        private float m_fCurrentPlayingAnimLength;
        private bool m_bIsPlayingJumpDownAnim;

        private void Awake()
        {
            _instance = this;
        }
        // Use this for initialization
        public void Initialize()
        {
            m_GameSettingConfig = ConfigManager.Instance.GetRegularityGameSetting();
            m_iLimitCount = m_GameSettingConfig.PlayCountLimit;
            m_fLeftTime = m_GameSettingConfig.PlayTime;
            m_bIsFinished = false;
            m_DiffMgr = new RegularityGameDifficultyManager();
            m_bLastStatusIsWin = true;
            m_iLeftRedFlower = 3;
            m_PlayerAnim = m_Player.GetComponent<Animator>();
            ReLoadScene();
            m_UIWindow.SetLeftCount(m_iLimitCount);
            m_UIWindow.SetLeftTime(m_fLeftTime);
            m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
        }
        private void ReLoadScene()
        {
            if (!m_bIsResetByUI)
            {
                m_CurrentConfig = m_DiffMgr.GetDifficulty();
            }
            m_bIsResetByUI = false;

            m_ElementList = new List<PilesElement>();
            for (int i = 0; i < m_CurrentConfig.OptionList.Count; ++i)
            {
                string name = m_CurrentConfig.OptionList[i].Name;
                GameObject elem = GameObject.Instantiate(m_ElementTemplate);
                elem.name = i.ToString();
                MeshRenderer renderer = ComponentTool.FindChildComponent<MeshRenderer>("Option", elem);
                Material elemMaterial =
                    new Material(ResourceManager.Instance.LoadBuildInResource<Material>("Piles/PilesMaterial",
                        AssetType.Materials));
                if (m_CurrentConfig.OptionList[i].IsVisable)
                {
                    elemMaterial.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(name,
                        AssetType.Texture);
                }
                else
                {
                    elemMaterial.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(m_strEmptyTextureName,
                        AssetType.Texture);
                }
                renderer.material = elemMaterial;
                elem.SetActive(true);
                m_ElementList.Add(new PilesElement(renderer.gameObject,elem, name, elemMaterial, m_CurrentConfig.OptionList[i].IsVisable));
                ComponentTool.Attach(m_ObjRoot.transform, elem.transform);
            }
            m_Grid = m_ObjRoot.GetComponent<RegularityGame.SimpleUIGrid>();
            m_Grid.Reposition();
            FixPosition();

            m_bIsInit = true;
        }
        private void FixPosition()
        {
            if (m_ElementList.Count < 1)
            {
                return;
            }
            m_fMinX = m_ElementList[0].m_ObjRoot.transform.position.x;
            m_fHalfLength = (m_ElementList[m_ElementList.Count - 1].m_ObjRoot.transform.position.x -
                             m_ElementList[0].m_ObjRoot.transform.position.x)/2.0f;

            for (int i = 0; i < m_ElementList.Count; ++i)
            {
                float x = m_ElementList[i].m_ObjRoot.transform.position.x - m_fMinX - m_fHalfLength;
                float z = x*x*m_fSpace;

                m_ElementList[i].m_ObjRoot.transform.position =
                    new Vector3(m_ElementList[i].m_ObjRoot.transform.position.x,
                        m_ElementList[i].m_ObjRoot.transform.position.y, z);
                //m_ElementList[i].m_ObjRoot.transform.LookAt(m_ObjLookatTarget.transform);
                //m_ElementList[i].m_ObjRoot.transform.forward *= -1;
                m_ElementList[i].m_ObjRoot.transform.eulerAngles = new Vector3(0.0f,
                    m_ElementList[i].m_ObjRoot.transform.eulerAngles.y, 0.0f);
            }
            SetPlayerPos(0);
            OpenWindow();
        }
        private void SetPlayerPos(int index)
        {
            if (m_ElementList.Count <= 0)
            {
                return;
            }
            m_Player.transform.position = new Vector3(m_ElementList[index].m_ObjRoot.transform.position.x, m_fPlayerFixY,
                m_fPlayerFixZ);

            m_Player.transform.forward = m_ElementList[index].m_ObjRoot.transform.right;
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
            if (m_bIsJumping || m_bIsMovingPiles || m_bIsFinished || m_bIsShake)
            {
                return;
            }
            for (int i = 0; i < m_ElementList.Count; ++i)
            {
                if (!m_ElementList[i].m_bIsVisable)
                {
                    CheckCorrect(i, m_ElementList[i].m_strName, arg1);
                    m_UIWindow.OnDisableOption(arg1);
                    m_strCuringCaculateName = arg1;
                    break;
                }
            }
        }
        private void CheckCorrect(int index, string answerName, string targetName)
        {
            m_bIsSucceed = answerName == targetName;
            m_ElementList[index].m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>(
            targetName, AssetType.Texture);
            TriggerToShowEnableOption(m_ElementList[index].m_OptionRoot,() =>
            {
                if (!m_bIsSucceed)
                {
                    m_iTargetJumpIndex = index;
                }
                else
                {
                    m_iTargetJumpIndex = m_ElementList.Count - 1;
                }
                m_iCurrentJumpIndex = 0;
                TriggerToJump(m_fJumpSpendTime);
                Debuger.Log(" res : " + m_bIsSucceed);
            });
        }
        private void TriggerToJump(float spendtime,bool isPlayAnim = true)
        {
            Action tmp = () => 
            {
                // trigger player move
                JumpToTarget(++m_iCurrentJumpIndex, spendtime, out m_fVx, out m_fVy, out m_vDir);
                m_bIsJumping = true;
                m_fInitTime = TimeManager.Instance.GetTime();
                m_vInitPos = m_Player.transform.position;
            };
            if(isPlayAnim)
            {
                PlayJumpAnim("JumpUp", () =>
                {
                    tmp();
                });
            }
            else
            {
                tmp();
            }            
        }
        private void JumpToTarget(int targetIndex, float time, out float vx, out float vy, out Vector3 dir)
        {
            Vector3 targetPos = m_ElementList[targetIndex].m_ObjRoot.transform.position;
            targetPos.y = m_fPlayerFixY;
            vx = Vector3.Distance(m_Player.transform.position, targetPos)/time;
            vy = (float) (0.5f*m_fG*time);
            dir = targetPos - m_Player.transform.position;
            dir.Normalize();
        }
        private void OnJumpFinished()
        {
            if (m_bIsSucceed)
            {
                //event report
                if (m_bLastStatusIsWin)
                {
                    ++ m_iKeepStatusCount;
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
                

                if (m_iLimitCount == 1)
                {
                    m_UIWindow.SetLeftCount(0);
                    //trigger next stage
                    m_UIWindow.OnWin();
                    m_bIsFinished = true;
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Win);
                }
                else
                {
                    --m_iLimitCount;
                    m_UIWindow.SetLeftCount(m_iLimitCount);
                    OnReset();
                }
            }
            else
            {
                //event report
                if (!m_bLastStatusIsWin)
                {
                    ++ m_iKeepStatusCount;
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
                -- m_iLeftRedFlower;
                m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
                
                m_iMovingPilesIndex = m_iCurrentJumpIndex;
                m_bIsResetPos = true;
                m_iCurrentJumpIndex = -1;
                TriggerToJump(m_fJumpSpendTime,false);
                //play anim
                m_bIsMovingPiles = true;
                GameObject obj = m_ElementList[m_iMovingPilesIndex].m_ObjRoot;
                TweenPosition tween = obj.AddComponent<TweenPosition>();
                tween.from = obj.transform.localPosition;
                tween.from.y = m_fMovePilesTop;
                tween.to = obj.transform.localPosition;
                tween.to.y = m_fMovePilesBottom;
                tween.worldSpace = false;
                tween.duration = m_fMovePilesDuringtime;
                tween.AddOnFinished(OnMovePilesBottomFinished);
            }
        }
        private void OnMovePilesBottomFinished()
        {
            GameObject obj = m_ElementList[m_iMovingPilesIndex].m_ObjRoot;
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
            GameObject obj = m_ElementList[m_iMovingPilesIndex].m_ObjRoot;
            TweenPosition tween = obj.GetComponent<TweenPosition>();
            Destroy(tween);

            m_UIWindow.OnEnableOption(m_strCuringCaculateName);
            m_strCuringCaculateName = string.Empty;
            TriggerToShowDisableOption(m_ElementList[m_iMovingPilesIndex].m_OptionRoot, () =>
            {
                m_ElementList[m_iMovingPilesIndex].m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture2D>(m_strEmptyTextureName,
                           AssetType.Texture);
                m_bIsMovingPiles = false;
                if (m_iLeftRedFlower <= 0)
                {
                    //trigger to end game
                    m_UIWindow.OnLose();
                    m_bIsFinished = true;
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Lose);
                }
            });
        }
        private void ResetScene()
        {
            ClearScene();

            ReLoadScene();

            m_UIWindow.ResetAnswer(m_CurrentConfig.AnswerList);
        }
        private void Update()
        {
            if (m_bIsFinished)
            {
                return;
            }
            if (!m_bIsInit)
            {
                return;
            }
            if (m_bIsJumping)
            {
                float deltaTime = TimeManager.Instance.GetTime() - m_fInitTime;
                if (!m_bIsResetPos && deltaTime >= m_fJumpDownPlayOffset && !m_bIsPlayingJumpDownAnim)
                {
                    m_bIsPlayingJumpDownAnim = true;
                    PlayJumpAnim("JumpDown", () => 
                    {
                        m_bIsPlayingJumpDownAnim = false;
                        if (m_iCurrentJumpIndex < m_iTargetJumpIndex)
                        {
                            Shake(() =>
                            {
                                m_bIsJumping = false;
                                m_PlayerAnim.CrossFade("Idle", 0.2f);
                                TriggerToJump(m_fJumpSpendTime);
                            });

                        }
                        else
                        {
                            if (m_bIsSucceed)
                            {
                                Shake(() =>
                                {
                                    m_bIsJumping = false;
                                    m_PlayerAnim.CrossFade("Idle", 0.2f);
                                    OnJumpFinished();
                                });
                            }
                            else
                            {
                                m_PlayerAnim.CrossFade("Idle", 0.2f);
                                // enter
                                OnJumpFinished();
                            }
                        }
                    });
                }
            
                if (deltaTime >= m_fJumpSpendTime)
                {
                    m_bIsJumping = false;
                    SetPlayerPos(m_iCurrentJumpIndex);
                    if (m_bIsResetPos)
                    {
                        m_bIsResetPos = false;
                    }
                    
                    return;
                }
                Vector3 tmpVector3 = m_Player.transform.position;
                tmpVector3 = m_vInitPos + m_vDir * deltaTime * m_fVx;
                tmpVector3.y = deltaTime * m_fVy - 0.5f * m_fG * deltaTime * deltaTime;
                tmpVector3.y+= m_fPlayerFixY;
                m_Player.transform.position = tmpVector3;
            }
            if(m_bIsPlayingAnim)
            {
                m_fCurringPlaytime += TimeManager.Instance.GetDeltaTime();
                if (m_fCurringPlaytime >= m_fCurrentPlayingAnimLength)
                {
                    OnAnimPlayedEnd();
                }
            }
            if (m_fLeftTime > 0.0f)
            {
                m_fLeftTime -= TimeManager.Instance.GetDeltaTime();
                m_UIWindow.SetLeftTime(m_fLeftTime);
                if (m_fLeftTime <= 0.0f)
                {
                    //trigger to end game
                    m_UIWindow.OnLose();
                    m_bIsFinished = true; 
                    m_DiffMgr.ReportEvent(RegularityGameDifficultyManager.RegularityEventType.Lose);
                }
            }
        }
        public void OnReset()
        {
            if (m_bIsJumping || m_bIsMovingPiles)
            {
                return;
            }
            ResetScene();
        }
        public void ResetSceneByUI(int index)
        {
            m_bIsFinished = false;
            m_bIsJumping = false;
            m_bIsMovingPiles = false;
            m_iCurrentJumpIndex = 0;
            m_iLimitCount = m_GameSettingConfig.PlayCountLimit;
            m_fLeftTime = m_GameSettingConfig.PlayTime;
            m_UIWindow.SetLeftCount(m_iLimitCount);
            m_UIWindow.SetLeftTime(m_fLeftTime);
            m_iLeftRedFlower = 3;
            m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
            var Config = ConfigManager.Instance.GetRegularityGameConfig();
            m_CurrentConfig = Config.RegularityConfigMap[index];

            m_iKeepStatusCount = -1;
            m_bIsResetByUI = true;
            OnReset();
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
        public void OnBack()
        {
            WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
            //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
        }
        private void Shake(Action callBack)
        {
            m_bIsJumping = false;
            m_bIsShake = true;

            TweenPosition tween = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.AddComponent<TweenPosition>();
            tween.from = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.transform.localPosition;
            tween.to = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.transform.localPosition;
            tween.to.y = tween.from.y - m_fShakeRange;
            tween.duration = m_fShakeDuringtime;
            tween.onFinished.Add(new EventDelegate(OnShakeBottomEnd));
            m_Player.transform.parent = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.transform;
            m_OnShakeEndCallBack = callBack;
        }
        private void OnShakeBottomEnd()
        {
            TweenPosition tween = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.AddComponent<TweenPosition>();
            GameObject.Destroy(tween);

            tween = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.AddComponent<TweenPosition>();
            tween.onFinished.Clear();

            tween.from = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.transform.localPosition;
            tween.to = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.transform.localPosition;
            tween.to.y = tween.from.y + m_fShakeRange;
            tween.duration = m_fShakeDuringtime;
            tween.onFinished.Add(new EventDelegate(OnShakeTopEnd));
        }
        private void OnShakeTopEnd()
        {
            TweenPosition tween = m_ElementList[m_iCurrentJumpIndex].m_ObjRoot.AddComponent<TweenPosition>();
            GameObject.Destroy(tween);
            m_Player.transform.parent = null;
            m_bIsShake = false;
            m_OnShakeEndCallBack();
        }
        public void Restart()
        {
            m_bIsFinished = false;
            m_bIsJumping = false;
            m_bIsMovingPiles = false;
            m_iCurrentJumpIndex = 0;
            m_iLimitCount = m_GameSettingConfig.PlayCountLimit;
            m_fLeftTime = m_GameSettingConfig.PlayTime;
            m_UIWindow.SetLeftCount(m_iLimitCount);
            m_UIWindow.SetLeftTime(m_fLeftTime);
            m_iLeftRedFlower = 3;
            m_UIWindow.SetLeftFlower(m_iLeftRedFlower);
            m_iKeepStatusCount = -1;
            m_iCurrentJumpIndex = 0;
            OnReset();
        }
        private void TriggerToShowEnableOption(GameObject optionObj,Action CallBack)
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
        private void PlayJumpAnim(string name,Action CallBack)
        {
            m_bIsPlayingAnim = true;
            m_fCurringPlaytime = 0.0f;
            m_OnPlayEndCallBack = CallBack;
            m_PlayerAnim.CrossFade(name, 0.2f);
            float offset = name.Equals("JumpUp") ? m_fJumpUpAnimLengthOffset : m_fjumpDownAnimLengthOffset;
            m_fCurrentPlayingAnimLength =  offset;
            //Debuger.Log("length : " + m_fCurrentPlayingAnimLength);
        }
        private void OnAnimPlayedEnd()
        {
            m_fCurringPlaytime = 0.0f;
            m_bIsPlayingAnim = false;
            m_OnPlayEndCallBack();
        }
    }
}