//========================================================================
// Copyright(C): CYTX
//
// FileName : MusicGameLogic
// 
// Created by : LeoLi at 2015/12/25 17:15:58
//
// Purpose : 
//========================================================================
using Config;
using Config.Table;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using System.Text;

namespace MusicGame
{
    public class StarElementStruct
    {
        public GameObject m_Root;
        public MusicGameTool_StarController m_Controller;
        public int m_nNoteID;
    }
    public class MusicGameManager : SingletonTemplateMon<MusicGameManager>
    {
        // Read Only
        private readonly int MUSICID = 1;
        private readonly int NOTECOUNT = 8;
        private readonly string AUDIO_PATH = "MusicGame/Guitar";
        private readonly string HINTAUDIO_START = "GUIDE/60_MusicGame/Yindaoyu_#130_G_D";
        private readonly string HINTAUDIO_FALLING = "GUIDE/60_MusicGame/Yindaoyu_#131_G_D";
        private readonly string HINTAUDIO_REACH = "GUIDE/60_MusicGame/Yindaoyu_#132_G_D";
        private readonly string HINTAUDIO_POINTMONSTER = "GUIDE/60_MusicGame/Yindaoyu_#133_G_D";
        private readonly string HINTAUDIO_SUCCESS = "GUIDE/60_MusicGame/Yindaoyu_#134_G_D";
        private readonly string HINTAUDIO_FAIL = "GUIDE/60_MusicGame/Yindaoyu_#135_G_D";
        private readonly string HINTAUDIO_END = "GUIDE/60_MusicGame/Yindaoyu_#136_G_D";

        // Public Property
        public MusicGameTool_BackgroundMaskController m_BackgroundMaskController;
        public GameObject m_ElementTemplate;
        public GameObject m_StarElementRoot;
        public List<GameObject> m_LaunchPositions = new List<GameObject>();
        //public RectTransform m_GameOverPanel;
        //public Text m_LifeRemainLabel;
        public Image m_LifeRemainTen;
        public Image m_LifeRemainOne;
        public Sprite[] m_SpritesLifeNum;
        public Text m_GameStateLabel;
        public GameObject Block;
        public Button m_BackButton;
        public GameObject m_ThrowGuideSprite;
        //public HealthBar healthbar;
        // Game State
        public bool m_IsAutoMusic;
        private int m_nLifeRemain;
        private int m_nCorrectCount = 0;
        private int m_nMissCount = 0;
        private int m_Health = 99;
        private bool m_IsGuide = false;
        private bool m_IsGuideShowHint = false;
        private bool m_IsGuideShowPointing = false;
        // Game Data
        private MusicPlayer m_MusicPlayer;
        private ObjectPool<StarElementStruct> m_poolStarElement;
        private List<StarElementStruct> m_lstStarElement = new List<StarElementStruct>();
        // Difficulty Config
        private float m_fTargetRange = 1;
        private float m_fMusicSpeed = 1;
        private int m_nAllowedErrorCount = 10;
        // Setting Config
        private MusicGameNoteKeyConfig m_GameSettingConfig;
        private bool m_bIsReloading = false;

        #region MonoBehavior
        void Awake()
        {
            _instance = this;
        }
        void Start()
        {
            // Lauch Root
            GameObject launchroot = GameObject.Find("LaunchPositions");
            if (launchroot == null)
            {
                Debug.LogError("LaunchPositions not found");
                return;
            }
            for (int i = 0; i < launchroot.transform.childCount; i++)
            {
                m_LaunchPositions.Add(launchroot.transform.GetChild(i).gameObject);
            }
            MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ENABLE_BLOCK, OnEnableBlock);
            MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_DISABLE_BLOCK, OnDisableBlock);
        }
        void OnDestroy()
        {
            MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ENABLE_BLOCK, OnEnableBlock);
            MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_DISABLE_BLOCK, OnDisableBlock);
        }
        void Update()
        {
            if (m_MusicPlayer == null)
                return;
            m_MusicPlayer.Update();

            // Check Input
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200f))
                {
                    var monster = hit.transform.GetComponent<MusicGameTool_MonsterController>();
                    if (monster != null)
                    {
                        int noteID = monster.OnClicked();
                        //Debug.Log("Click Monster : " + noteID.ToString());
                        if (!m_IsAutoMusic)
                        {
                            PlayMusicAudio(noteID);
                        }
                        //Check Click
                        if (CheckClick(hit.transform.position.y, noteID)) //Success
                        {
                            if (m_IsAutoMusic && !m_IsGuide)
                            {
                                PlayMusicAudio(noteID);
                            }
                            monster.PlayAnim(true);
                            // GUIDE
                            if (m_IsGuide)
                            {
                                GuideSucess();
                                return;
                            }
                            // User Talent Report
                            m_nCorrectCount++;
                            m_nMissCount = 0;
                            if (m_nCorrectCount >= 5)
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Correct5);
                            }
                            else if (m_nCorrectCount >= 3)
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Correct3);
                            }
                            else
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Correct);
                            }
                        }
                        else //Fail
                        {
                            monster.PlayAnim(false);
                            // GUIDE
                            if (m_IsGuide)
                            {
                                GuideFail();
                                return;
                            }
                            m_nLifeRemain--;
                            if (m_nLifeRemain <= 0)
                            {
                                GameEndLogic();
                            }
                            // User Talent Report
                            m_nMissCount++;
                            m_nCorrectCount = 0;
                            if (m_nMissCount >= 3)
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong3);
                            }
                            else if (m_nMissCount >= 2)
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong2);
                            }
                            else
                            {
                                MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong);
                            }
                        }
                    }
                }
            }

            // Check Star Pos
            for (int i = 0; i < m_lstStarElement.Count; i++)
            {
                // GUIDE Check Points
                if (m_IsGuide && m_lstStarElement.Count > 0)
                {
                    if (m_lstStarElement[0].m_Root.transform.position.y < -5 && !m_IsGuideShowPointing)
                    {
                        m_lstStarElement[0].m_Controller.PauseFall();
                        m_MusicPlayer.PauseGuide();
                        m_IsGuideShowPointing = true;
                        m_BackgroundMaskController.SetActive(true);
                        m_BackgroundMaskController.SetMaskPos(m_lstStarElement[0].m_Root.transform.position);
                        m_lstStarElement[0].m_Controller.Shinging();
                        PlayHintAudio(HINTAUDIO_REACH, (a) =>
                        {
                            m_ThrowGuideSprite.SetActive(true);
                            PlayHintAudio(HINTAUDIO_POINTMONSTER, (b) =>
                           {
                               m_ThrowGuideSprite.SetActive(false);
                               Block.SetActive(false);
                           });
                        });
                    }
                    else if (m_lstStarElement[0].m_Root.transform.position.y < 0 && !m_IsGuideShowHint)
                    {
                        m_IsGuideShowHint = true;
                        m_lstStarElement[0].m_Controller.PauseFall();
                        m_MusicPlayer.PauseGuide();
                        m_BackgroundMaskController.SetActive(true);
                        m_BackgroundMaskController.SetMaskPos(m_lstStarElement[0].m_Root.transform.position);
                        m_lstStarElement[0].m_Controller.Shinging();
                        PlayHintAudio(HINTAUDIO_FALLING, (a) =>
                        {
                            m_lstStarElement[0].m_Controller.StopShinging();
                            m_BackgroundMaskController.SetActive(false);
                            m_lstStarElement[0].m_Controller.ResumeFall();
                            m_MusicPlayer.ResumeGuide();
                        });
                    }
                }

                if (m_lstStarElement[i].m_Root.transform.position.y < -7)
                {
                    StarElementStruct star = m_lstStarElement[i];
                    star.m_Controller.StopFall(() =>
                    {
                        m_poolStarElement.Store(star);
                        // GUIDE
                        if (m_IsGuide)
                        {
                            return;
                        }
                        m_nLifeRemain--;
                        if (m_nLifeRemain <= 0)
                        {
                            GameEndLogic();
                        }
                    });
                    m_lstStarElement.RemoveAt(i);
                    // GUIDE
                    if (m_IsGuide)
                    {
                        return;
                    }
                    // User Talent Report
                    m_nMissCount++;
                    m_nCorrectCount = 0;
                    if (m_nMissCount >= 3)
                    {
                        MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong3);
                    }
                    else if (m_nMissCount >= 2)
                    {
                        MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong2);
                    }
                    else
                    {
                        MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Wrong);
                    }
                }
            }

            // Update Life UI
            if (m_nLifeRemain >= 0)
            {
                //m_LifeRemainLabel.text = m_nLifeRemain.ToString();
                if (m_nLifeRemain >= 10)
                {
                    m_LifeRemainTen.sprite = m_SpritesLifeNum[m_nLifeRemain / 10 % 10];
                }
                else
                {
                    m_LifeRemainTen.sprite = m_SpritesLifeNum[0];
                }
                m_LifeRemainOne.sprite = m_SpritesLifeNum[m_nLifeRemain % 10];
            }

            // Test 
            //testMethod();
        }
        #endregion

        #region Public Interface
        public void Initialize()
        {
            // setup elements
            m_StarElementRoot = GameObject.Find("StarElementRoot");
            if (m_StarElementRoot == null)
            {
                Debug.LogError("m_StarElementRoot not found");
                return;
            }
            m_poolStarElement = new ObjectPool<StarElementStruct>(10, ResetStarElement, InitStarElement);
            // setting config
            m_GameSettingConfig = ConfigManager.Instance.GetMusicGameSettingTable().MusicConfigMap[MUSICID];
            if (m_GameSettingConfig == null)
            {
                Debug.LogError("Music Game : Setting Config not exsist");
                return;
            }
            // game data
            m_MusicPlayer = new MusicPlayer(m_GameSettingConfig, GenerateStar, MusicEndLogic);
            // init scene
            if (CheckFirstPlay())
            {
                StartGuide();
            }
            else
            {
                ReLoadGame();
                ReLoadScene();
            }
        }
        public void Replay()
        {
            ReLoadScene();
            if (m_Health <= 0)
            {
                ReLoadGame();
            }
        }
        public void SetAutoMusic(bool auto)
        {
            m_IsAutoMusic = auto;
        }
        #endregion

        #region System Functions
        private void ReLoadScene()
        {
            // diff config
            m_fTargetRange = MusicGameHelper.GetRangeDifficulty();
            m_fMusicSpeed = MusicGameHelper.GetSpeedDifficulty();
            m_nAllowedErrorCount = MusicGameHelper.GetErrorDifficulty();
            Debuger.Log("TargetRange: " + m_fTargetRange.ToString() + "  MusicSpeed : " + m_fMusicSpeed.ToString() + "  ErrorCount : " + m_nAllowedErrorCount);
            //state
            Block.SetActive(false);
            m_BackButton.enabled = true;
            m_MusicPlayer.Reset();
            //start play
            m_MusicPlayer.SetMusicSpeed(m_fMusicSpeed);
            m_MusicPlayer.Play();
            //m_GameOverPanel.gameObject.SetActive(false);
            m_nLifeRemain = m_nAllowedErrorCount;
        }
        private void ReLoadGame()
        {
            m_Health = 99;
            //healthbar.InitHealthBar(m_Health);
        }
        private void ResetStarElement(StarElementStruct elem)
        {
            elem.m_Controller.StartFall();
        }
        private void InitStarElement(StarElementStruct elem)
        {
            elem.m_Root = GameObject.Instantiate(MusicGameManager.Instance.m_ElementTemplate);
            ComponentTool.Attach(MusicGameManager.Instance.m_StarElementRoot.transform, elem.m_Root.transform);
            elem.m_Root.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            elem.m_Controller = elem.m_Root.GetComponent<MusicGameTool_StarController>();
            if (elem.m_Controller == null)
            {
                Debug.LogError("MusicGameTool_StarController not found!");
                return;
            }
            elem.m_Controller.StartFall();
        }
        private void GenerateStar(int noteID)
        {
            StarElementStruct star = m_poolStarElement.New();
            star.m_nNoteID = noteID;
            if (m_LaunchPositions.Count < noteID)
            {
                Debug.LogError("the noteID of star element does not exist");
                return;
            }
            star.m_Root.transform.position = m_LaunchPositions[noteID - 1].transform.position;
            m_lstStarElement.Add(star);
            //PlayAudio(noteID);
        }
        private void MusicEndLogic()
        {
            Invoke("GameEndLogic", 6f);
        }
        private void GameEndLogic()
        {
            if (m_nLifeRemain > 0)
            {
                GameWin();
            }
            else
            {
                m_Health--;
                //healthbar.RemoveOneHealthIcon();
                if (m_Health <= 0)
                {
                    GameLose();
                }
                else
                {
                    GameLosePoint();
                }
            }
            Block.SetActive(true);
            m_BackButton.enabled = false;
            m_MusicPlayer.Reset();
            for (int i = 0; i < m_lstStarElement.Count; i++)
            {
                StarElementStruct star = m_lstStarElement[i];
                star.m_Controller.StopFall(() =>
                {
                    m_poolStarElement.Store(star);
                });
            }
            m_lstStarElement.Clear();
        }
        private void PlayMusicAudio(int noteID)
        {
            StringBuilder audioPathSB = new StringBuilder(AUDIO_PATH);
            audioPathSB.Append(noteID.ToString());
            AudioPlayer.Instance.PlayAudio(audioPathSB.ToString(), Vector3.zero, false);
        }
        private void PlayHintAudio(string audio, Action<string> finishiCallback)
        {
            AudioPlayer.Instance.PlayAudio(audio, Vector3.zero, false, finishiCallback);
        }
        private bool CheckClick(float position, int noteID)
        {
            float minDis = 10f;
            int index = -1;
            for (int i = 0; i < m_lstStarElement.Count; i++)
            {
                if (m_lstStarElement[i].m_nNoteID == noteID)
                {
                    var dis = Mathf.Abs(m_lstStarElement[i].m_Root.transform.position.y - position);
                    if (dis < m_fTargetRange)
                    {
                        if (dis < minDis)
                        {
                            minDis = dis;
                            index = i;
                        }
                    }
                }
            }
            if (index != -1)
            {
                StarElementStruct star = m_lstStarElement[index];
                star.m_Controller.Clear();
                m_poolStarElement.Store(star);
                m_lstStarElement.RemoveAt(index);
                return true;
            }
            return false;
        }
        private void GameLose()
        {
            MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Lose);
            //m_GameStateLabel.text = "游戏失败";
            Action<bool> fun = (res) =>
            {
                if (res)
                {
                    Replay();
                    WindowManager.Instance.HideWindow(WindowID.LosePanel);
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }
            };
            WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);
        }
        private void GameLosePoint()
        {
            if(m_bIsReloading)
            {
                return;
            }
            MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Lose);
            //m_GameStateLabel.text = "再来一次";
            Action<bool> fun = (res) =>
            {
                if (res)
                {
                    Replay();
                    WindowManager.Instance.HideWindow(WindowID.LosePanel);
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }
            };
            WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);
        }
        private void GameWin()
        {
            MusicGameHelper.ReportEvent(MusicGameHelper.MusicGameEventType.Win);
            //m_GameStateLabel.text = "游戏胜利";
            Action<bool> fun = (res) =>
            {
                if (res)
                {
                    Replay();
                    WindowManager.Instance.HideWindow(WindowID.WinPanel);
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }
            };
            WindowManager.Instance.OpenWindow(WindowID.WinPanel, fun);
            ReLoadGame();
        }
        private void OnDisableBlock(MessageObject obj)
        {
            //Replay();

            m_BackButton.enabled = true;
            Block.SetActive(false);

            Time.timeScale = 1;
            //m_MusicPlayer.Play();
        }
        private void OnEnableBlock(MessageObject obj)
        {
            m_BackButton.enabled = false;
            Block.SetActive(true);

            Time.timeScale = 0;

            //m_MusicPlayer.Pause();

            //m_MusicPlayer.Reset();
            //for (int i = 0; i < m_lstStarElement.Count; i++)
            //{
            //    StarElementStruct star = m_lstStarElement[i];
            //    star.m_Controller.StopFall(() =>
            //    {
            //        m_poolStarElement.Store(star);
            //    });
            //}
            //m_lstStarElement.Clear();
        }
        private bool CheckFirstPlay()
        {
            if (PlayerManager.Instance.GetCharCounterData().GetFlag(6))
            {
                return false;
            }
            return true;
        }
        private void StartGuide()
        {
            //state
            Block.SetActive(true);
            m_BackButton.enabled = false;
            m_MusicPlayer.Reset();
            m_IsGuide = true;
            m_IsGuideShowHint = false;
            m_IsGuideShowPointing = false;
            //start guide
            PlayHintAudio(HINTAUDIO_START, (a) =>
            {
                m_MusicPlayer.StartGuide();
            });
        }
        private void GuideSucess()
        {
            if (m_lstStarElement.Count > 0)
            {
                StarElementStruct star = m_lstStarElement[0];
                star.m_Controller.StopShinging();               
                star.m_Controller.Clear();
                m_poolStarElement.Store(star);
                m_lstStarElement.RemoveAt(0);
            }
            m_IsGuideShowPointing = false;
            m_BackgroundMaskController.SetActive(false);
            m_MusicPlayer.Reset();
            m_IsGuide = false;

            m_bIsReloading = true;
            // start game
            PlayHintAudio(HINTAUDIO_SUCCESS, (aa) =>
            {
                PlayHintAudio(HINTAUDIO_END, (b) =>
                {
                    m_bIsReloading = false;
                    ReLoadGame();
                        ReLoadScene();
                    });
            });
            PlayerManager.Instance.GetCharCounterData().SetFlag(6, true);
        }
        private void GuideFail()
        {
            if (m_lstStarElement.Count > 0)
            {
                StarElementStruct star = m_lstStarElement[0];
                star.m_Controller.StopShinging();  
                star.m_Controller.Clear();
                m_poolStarElement.Store(star);
                m_lstStarElement.RemoveAt(0);
            }
            m_IsGuideShowPointing = false;
            m_BackgroundMaskController.SetActive(false);
            Block.SetActive(true);
            PlayHintAudio(HINTAUDIO_FAIL, (a) =>
            {
                m_MusicPlayer.ResumeGuide();
            });
        }
        #endregion

        //private void testMethod()
        //{
        //    if (Input.GetKeyUp(KeyCode.Alpha1))
        //    {
        //        GenerateStar(1);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha2))
        //    {
        //        GenerateStar(2);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha3))
        //    {
        //        GenerateStar(3);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha4))
        //    {
        //        GenerateStar(4);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha5))
        //    {
        //        GenerateStar(5);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha6))
        //    {
        //        GenerateStar(6);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha7))
        //    {
        //        GenerateStar(7);
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha8))
        //    {
        //        GenerateStar(8);
        //    }
        //}
    }

}