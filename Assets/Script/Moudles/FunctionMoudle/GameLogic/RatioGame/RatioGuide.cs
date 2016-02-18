using UnityEngine;
using System.Collections;
using System;

namespace RatioGame
{
    public class RatioGuide : MonoBehaviour
    {
        #region declaration
        const string dir = "GUIDE/10_RatioGame/";
        const string strStart = "Yindaoyu_#52_G_D";
        const string strCatTaunt = "Yindaoyu_#53_G_Cat";
        const string strWatchout = "Yindaoyu_#54_G_D";
        const string strStupid = "Yindaoyu_#55_G_D";
        //sex
        const string strSeeDifferent = "Yindaoyu_#56_G_D";
        const string strThisIsSmall = "Yindaoyu_#58_G_D";
        const string strClickIt = "Yindaoyu_#59_G_D";
        const string strWellDone = "Yindaoyu_#60_G_D";
        const string strSeeAgain = "Yindaoyu_#62_G_D";
        const string strThisIsSmallToo = "Yindaoyu_#63_G_D";
        const string strClickItToo = "Yindaoyu_#64_G_D";
        const string strWonderful = "Yindaoyu_#65_G_D";
        const string strLastOne = "Yindaoyu_#67_G_D";
        //sex
        const string strYouAreGreat = "Yindaoyu_#68_G_D";
        //sex
        const string strDone = "Yindaoyu_#71_G_D";

        public RectTransform mask;
        public Camera guideCam;
        public RectTransform pCanvas;
        public RectTransform clickGuide;
        GameObject blink;

        bool isInGuide = true;
        string key = "Ratio_IsGuideFinish";
        Vector3 pos;
        bool canClick = false;
        #endregion

        void Awake()
        {
            //isInGuide = !PlayerPrefs.HasKey(key);
            isInGuide = !PlayerManager.Instance.GetCharCounterData().GetFlag(1);
            pos = Camera.main.transform.position;
        }

        public void GuideStart()
        {
            Debug.Log("start audio");
            AudioPlayer.Instance.PlayAudio(dir + strStart, pos, false, StartAudioEnd);
        }

        void StartAudioEnd(string str)
        {
            Debug.Log("taunt audio");
            AudioPlayer.Instance.PlayAudio(dir + strCatTaunt, pos, false, TauntAudioEnd);
        }

        void TauntAudioEnd(string str)
        {
            Debug.Log("watchout audio");
            RatioGameManager.Instance.Shoot();
            AudioPlayer.Instance.PlayAudio(dir + strWatchout, pos, false, WatchoutAudioEnd);
        }

        void WatchoutAudioEnd(string str)
        {
            StartCoroutine(Stop());
        }

        IEnumerator Stop()
        {
            yield return new WaitForSeconds(2);
            RatioGameManager.IsStop = true;
            mask.gameObject.SetActive(true);
            var balls = RatioGameManager.Instance.GetBallGroup().GetBalls();
            foreach(var ball in balls)
            {
                ball.gameObject.layer = 1;
            }
            AudioPlayer.Instance.PlayAudio(dir + strStupid, pos, false, StupidAudioEnd);
        }

        void StupidAudioEnd(string str)
        {
            AudioPlayer.Instance.PlayAudio(dir + strSeeDifferent, pos, false, SeeDifferentAudioEnd);
        }

        void SeeDifferentAudioEnd(string str)
        {
            StartCoroutine(Shining(4,true,()=> 
            {
                var balls = RatioGameManager.Instance.GetBallGroup().GetBalls();
                foreach (var ball in balls)
                {
                    ball.gameObject.layer = 0;
                }
                AudioPlayer.Instance.PlayAudio(dir + strThisIsSmall, pos, false, ThisIsSmallAudioEnd);
            }));
        }
        

        void ThisIsSmallAudioEnd(string str)
        {
            var ball = RatioGameManager.Instance.GetBallGroup().GetCurBall();
            ball.gameObject.layer = 1;
            StartCoroutine(Shining(6, false, () =>
            {
                var p = RectTransformUtility.WorldToScreenPoint(guideCam, ball.transform.position);
                clickGuide.gameObject.SetActive(true);
                clickGuide.localPosition = p;
                blink = Instantiate(RatioGameManager.Instance.blinking);
                blink.layer = 1;
                blink.transform.position = ball.transform.position;
                AudioPlayer.Instance.PlayAudio(dir + strClickIt, pos, false, ClickItAudioEnd);
            }));
        }

        void ClickItAudioEnd(string str)
        {
            canClick = true;
        }

        public bool CanClick(int index)
        {
            if (canClick)
            {
                switch (index)
                {
                    case 0:
                        AudioPlayer.Instance.PlayAudio(dir + strWellDone, pos, false, WellDoneAudioEnd);
                        break;
                    case 1:
                        AudioPlayer.Instance.PlayAudio(dir + strWonderful, pos, false, WonderfulAudioEnd);
                        break;
                    case 2:
                        AudioPlayer.Instance.PlayAudio(dir + strYouAreGreat, pos, false, YouAreGreatAudioEnd);
                        break;
                }
                clickGuide.gameObject.SetActive(false);
                Destroy(blink);
                canClick = false;
                return true;
            }
            return false;
        }
        #region after click one ball
        void WellDoneAudioEnd(string str)
        {
            AudioPlayer.Instance.PlayAudio(dir + strSeeAgain, pos, false, SeeAgainAudioEnd);
        }

        void SeeAgainAudioEnd(string str)
        {
            var balls = RatioGameManager.Instance.GetBallGroup().GetBalls();
            foreach (var ball in balls)
            {
                ball.gameObject.layer = 1;
            }
            StartCoroutine(Shining(4, true, () =>
            {
                foreach (var ball in balls)
                {
                    ball.gameObject.layer = 0;
                }
                AudioPlayer.Instance.PlayAudio(dir + strThisIsSmallToo, pos, false, ThisIsSmallTooAudioEnd);
            }));
        }

        void ThisIsSmallTooAudioEnd(string str)
        {
            var ball = RatioGameManager.Instance.GetBallGroup().GetCurBall();
            ball.gameObject.layer = 1;
            StartCoroutine(Shining(6, false, () =>
            {
                var p = RectTransformUtility.WorldToScreenPoint(guideCam, ball.transform.position);
                clickGuide.gameObject.SetActive(true);
                clickGuide.localPosition = p;
                blink = Instantiate(RatioGameManager.Instance.blinking);
                blink.layer = 1;
                blink.transform.position = ball.transform.position;
                AudioPlayer.Instance.PlayAudio(dir + strClickItToo, pos, false, ClickItTooAudioEnd);
            }));
        }

        void ClickItTooAudioEnd(string str)
        {
            canClick = true;
        }
        #endregion

        #region after click two ball
        void WonderfulAudioEnd(string str)
        {
            AudioPlayer.Instance.PlayAudio(dir + strLastOne, pos, false, LastOneAudioEnd);
        }

        void LastOneAudioEnd(string str)
        {
            var ball = RatioGameManager.Instance.GetBallGroup().GetCurBall();
            ball.gameObject.layer = 1;
            StartCoroutine(Shining(6, false, () =>
            {
                var p = RectTransformUtility.WorldToScreenPoint(guideCam, ball.transform.position);
                clickGuide.gameObject.SetActive(true);
                clickGuide.localPosition = p;
                blink = Instantiate(RatioGameManager.Instance.blinking);
                blink.layer = 1;
                blink.transform.position = ball.transform.position;
                canClick = true;
            }));            
        }
        #endregion

        #region after click three ball
        void YouAreGreatAudioEnd(string str)
        {
            AudioPlayer.Instance.PlayAudio(dir + strDone, pos, false, DoneAudioEnd);
        }

        void DoneAudioEnd(string str)
        {
            IsInGuide = false;
            RatioGameManager.IsStop = false;
            mask.gameObject.SetActive(false);
            RatioGameManager.Instance.Shoot();
        }
        #endregion

        IEnumerator Shining(int amount, bool isAll, Action callback = null)
        {
            var balls = RatioGameManager.Instance.GetBallGroup().GetBalls();

            while (amount > 0)
            {
                if (isAll)
                {
                    foreach (var ball in balls)
                    {
                        var render = ball.GetComponent<Renderer>();
                        render.enabled = !render.enabled;
                    }
                }
                else
                {
                    var ball = RatioGameManager.Instance.GetBallGroup().GetCurBall();
                    var render = ball.GetComponent<Renderer>();
                    render.enabled = !render.enabled;
                }

                yield return new WaitForSeconds(0.5f);
                amount--;
            }
            if (callback != null)
            {
                callback();
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
                //PlayerPrefs.SetInt(key, 1);
                PlayerManager.Instance.GetCharCounterData().SetFlag(1, true);
            }
        }
    }

}
