using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RegularityGame
{
    public class RegularityGuide : SingletonTemplateMon<RegularityGuide>
    {
        const string dir = "GUIDE/20_RegularityGame/";
        const string strHowToGo = "Yindaoyu_#153_G_BG";
        const string strFollowOrder = "Yindaoyu_#154_G_D";
        const string strThisIsQ = "Yindaoyu_#155_G_D";
        const string strSeeDifferent = "Yindaoyu_#156_G_D";
        const string strWhatPut = "Yindaoyu_#158_G_D";
        const string strPutBanana = "Yindaoyu_#159_G_D";
        const string strGreat = "Yindaoyu_#160_G_D";
        const string strYouAreClever = "Yindaoyu_#163_G_D";


        RegularityAlphaGameLogic gameLogic;

        bool isInGuide = true;
        string key = "Ratio_IsGuideFinish";
        Vector3 audioPos;
        UIRegGuide ui;

        private Animator m_PlayerAnim;
        private SimpleUIGrid m_Grid;
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

        void Awake()
        {
            _instance = this;
            gameLogic = FindObjectOfType<RegularityAlphaGameLogic>();
            audioPos = Camera.main.transform.position;
            ui = FindObjectOfType<UIRegGuide>();
            m_ElementList = new List<PilesAlphaElement>();
            m_PlayerAnim = gameLogic.m_ObjPlayer.GetComponent<Animator>();
            m_Grid = gameLogic.m_ObjElementRoot.GetComponent<SimpleUIGrid>();
        }

        void Start()
        {
            /*
            if (ResourceManager.Instance == null)
            {
                new GameObject("ResourceManager", typeof(ResourceManager));
                ResourceManager.Instance.Initialize();
            }
            GuideStart();
            */
        }

        public void GuideStart()
        {
            for(int i = 0; i < 9; i++)
            {
                GameObject elem = GameObject.Instantiate(gameLogic.m_ObjElementTemplate);
                elem.name = i.ToString();
                string name = i % 2 == 0 ? "Piles/Apple" : "Piles/Banana";
                bool isVisable = i == 5 ? false : true;

                PilesAlphaElement dataElem = new PilesAlphaElement(elem, name, isVisable, gameLogic.m_strEmptyTextureName);
                ComponentTool.Attach(gameLogic.m_ObjElementRoot.transform, dataElem.m_ObjRoot.transform);
                m_ElementList.Add(dataElem);

                //if (!isVisable)
                {
                  //  m_iCurrentOptionIndex = i - 1;
                }
            }
            m_Grid.Reposition();
            SetPlayerPos(0);

            //OpenWindow();

            //m_UIWindow.ResetAnswer(m_CurrentConfig.AnswerList);
            m_iCurrentJumpedIndex = 0;
            TriggerJumpToTarget(1);
        }

        private void SetPlayerPos(int index)
        {
            gameLogic.m_ObjPlayer.transform.position = m_ElementList[index].m_ObjPlayerAttachRoot.transform.position;
        }

        private void JumpMove(float deltaTime)
        {
            Vector3 tmpVector3 = gameLogic.m_ObjPlayer.transform.position;
            tmpVector3 = m_vInitPos + m_vDir * deltaTime * m_fSpeedX;
            tmpVector3.y = m_vInitPos.y + deltaTime * m_fSpeedY - 0.5f * gameLogic.m_fG * deltaTime * deltaTime;
            gameLogic.m_ObjPlayer.transform.position = tmpVector3;
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
            m_PlayerAnim.CrossFade(name, gameLogic.m_fPlayAnimCrossDuringTime);
        }

        private void TriggerJumpToTarget(int index)
        {
            //check index
            if (index < 0 || index >= m_ElementList.Count)
            {
                return;
            }
            m_iCurrentJumpedIndex = index;
            float time = gameLogic.m_fJumpEndMoveTime - gameLogic.m_fJumpStartMoveTime;
            Vector3 targetPos = m_ElementList[index].m_ObjPlayerAttachRoot.transform.position;
            m_fSpeedX = Vector3.Distance(gameLogic.m_ObjPlayer.transform.position, targetPos) / time;
            m_fSpeedY = (float)(0.5f * gameLogic.m_fG * time);
            m_vDir = targetPos - gameLogic.m_ObjPlayer.transform.position;
            m_vDir.Normalize();
            m_vInitPos = gameLogic.m_ObjPlayer.transform.position;
            m_bIsJump = true;
        }

        private void OnJumpDone()
        {
            
            if(m_iCurrentJumpedIndex == 4)
            {
                AudioPlayer.Instance.PlayAudio(dir + strHowToGo, audioPos, false,HowToGoAudioEnd);
            }
            else if(m_iCurrentJumpedIndex == 8)
            {
                Finish();
            }
            else
            {
                TriggerJumpToTarget(++m_iCurrentJumpedIndex);
            }
        }

        private void ResetPlayerPos()
        {
            SetPlayerPos(m_iCurrentJumpedIndex);
        }

        void HowToGoAudioEnd(string str)
        {
            AudioPlayer.Instance.PlayAudio(dir + strFollowOrder, audioPos, false, FollowOrderAudioEnd);
        }

        void FollowOrderAudioEnd(string str)
        {
            ui.ShowMask();
            //set all highlight
            foreach(var item in m_ElementList)
            {
                var go = item.m_ObjRoot;
                go.layer = 1;
                for(int i = 0; i < go.transform.childCount; i++)
                {
                    go.transform.GetChild(i).gameObject.layer = 1;
                }
            }
            StartCoroutine(Shining(1, m_ElementList, () => {
                //cancel all highlight
                foreach (var item in m_ElementList)
                {
                    var go = item.m_ObjRoot;
                    go.layer = 1;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.layer = 0;
                    }
                }
                //set five highlight
                var five = m_ElementList[5].m_ObjRoot;
                five.layer = 1;
                for(int i = 0; i < five.transform.childCount; i++)
                {
                    five.transform.GetChild(i).gameObject.layer = 1;
                }
                var list = new List<PilesAlphaElement>();
                list.Add(m_ElementList[5]);
                StartCoroutine(Shining(2,list,()=> {
                    five.layer = 0;
                    for (int i = 0; i < five.transform.childCount; i++)
                    {
                        five.transform.GetChild(i).gameObject.layer = 0;
                    }
                    AudioPlayer.Instance.PlayAudio(dir + strThisIsQ, audioPos, false, ThisIsQAudioEnd);
                }));
            }));
        }

        void ThisIsQAudioEnd(string str)
        {
            ui.HideMask();
            AudioPlayer.Instance.PlayAudio(dir + strSeeDifferent, audioPos, false, SeeDifferentAudioEnd);
        }

        void SeeDifferentAudioEnd(string str)
        {
            var list = new List<PilesAlphaElement>();
            list.Add(m_ElementList[0]);
            list.Add(m_ElementList[1]);
            StartCoroutine(Shining(1, list,()=> {
                var list1 = new List<PilesAlphaElement>();
                list1.Add(m_ElementList[2]);
                list1.Add(m_ElementList[3]);
                StartCoroutine(Shining(1, list1, () =>
                {
                    var list2 = new List<PilesAlphaElement>();
                    list2.Add(m_ElementList[4]);
                    list2.Add(m_ElementList[5]);
                    StartCoroutine(Shining(1, list2, () =>
                    {
                        var list3 = new List<PilesAlphaElement>();
                        list3.Add(m_ElementList[6]);
                        list3.Add(m_ElementList[7]);
                        StartCoroutine(Shining(1, list3, () =>
                        {
                            var list4 = new List<PilesAlphaElement>();
                            list4.Add(m_ElementList[4]);
                            list4.Add(m_ElementList[5]);
                            StartCoroutine(Shining(2, list4, () =>
                            {
                                ui.Shining(2,true, () => {
                                    AudioPlayer.Instance.PlayAudio(dir + strWhatPut, audioPos, false, WhatPutAudioEnd);
                                });
                            }));
                        }));
                    }));
                }));
            }));
        }

        void WhatPutAudioEnd(string str)
        {
            ui.Shining(3, false, () =>
            {
                AudioPlayer.Instance.PlayAudio(dir + strPutBanana, audioPos, false, PutBananaAudioEnd);
            });
        }

        void PutBananaAudioEnd(string str)
        {
            ui.ShowFinger();
            ui.CanClick(ClickBanana);
        }

        void ClickBanana()
        {
            m_ElementList[5].m_Materials.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>("Piles/Banana", AssetType.Texture);
            TriggerToShowEnableOption(m_ElementList[5].m_ObjOptionRoot);
            ui.HideFinger();
            AudioPlayer.Instance.PlayAudio(dir + strGreat, audioPos, false, GreatAudioEnd);
        }

        void GreatAudioEnd(string str)
        {
            TriggerJumpToTarget(++m_iCurrentJumpedIndex);
        }

        void Finish()
        {
            for (int i = 0; i < m_ElementList.Count; ++i)
            {
                m_ElementList[i].m_ObjRoot.transform.parent = null;
                GameObject.Destroy(m_ElementList[i].m_ObjRoot);
            }
            m_ElementList.Clear();
            ui.Hide();

            PlayerManager.Instance.GetCharCounterData().SetFlag(2, true);

            gameLogic.Initialize();
        }

        IEnumerator Shining(int amount, List<PilesAlphaElement> list, Action callback = null)
        {
            while (amount > 0)
            {
                yield return new WaitForSeconds(0.5f);
                foreach (var item in list)
                {
                    var renderers = item.m_ObjRoot.GetComponentsInChildren<Renderer>();
                    foreach(var r in renderers)
                    {
                        r.enabled = !r.enabled;
                    }
                }
                yield return new WaitForSeconds(0.5f);
                foreach (var item in list)
                {
                    var renderers = item.m_ObjRoot.GetComponentsInChildren<Renderer>();
                    foreach (var r in renderers)
                    {
                        r.enabled = !r.enabled;
                    }
                }
                amount--;
            }
            if (callback != null)
            {
                callback();
            }
        }

        private void TriggerToShowEnableOption(GameObject optionObj, Action CallBack = null)
        {
            TweenScale tween = optionObj.GetComponent<TweenScale>();
            if (null != tween)
            {
                Destroy(tween);
            }
            tween = optionObj.AddComponent<TweenScale>();
            tween.from = Vector3.zero;
            tween.to = Vector3.one;
            tween.duration = 0.5f;
            tween.AddOnFinished(() =>
            {
                Destroy(tween);
                if(CallBack != null)
                {
                    CallBack();
                }
            });
        }

        void Update()
        {
            if (m_bIsJump)
            {
                float deltaTime = TimeManager.Instance.GetDeltaTime();
                m_fCurrentTime += deltaTime;

                if (m_fCurrentTime >= gameLogic.m_fJumpStartAnimPlayTime && !m_bIsTriggerJumpStarAnim)
                {
                    TriggerJumpStarAnim();
                    m_bIsTriggerJumpStarAnim = true;
                }
                if (m_fCurrentTime >= gameLogic.m_fJumpEndAnimPlayTime && !m_bIsTriggerJumpEndAnim)
                {
                    TriggerJumpEndAnim();
                    m_bIsTriggerJumpEndAnim = true;
                }
                if (m_fCurrentTime >= gameLogic.m_fJumpStartMoveTime && m_fCurrentTime <= gameLogic.m_fJumpEndMoveTime)
                {
                    JumpMove(m_fCurrentTime - gameLogic.m_fJumpStartMoveTime);
                }
                if (m_fCurrentTime >= gameLogic.m_fJumpEndTime)
                {
                    ClearJump();
                }
            }
        }

        public bool IsInGuide
        {
            get
            {
                return isInGuide;
            }
            set
            {
                isInGuide = value;
                PlayerPrefs.SetInt(key, 1);
            }
        }      
    }
}
