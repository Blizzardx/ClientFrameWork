using UnityEngine;
using System.Collections;

namespace Run
{
    public class RunGuide : MonoBehaviour
    {
        const string dir = "GUIDE/50_RunnerGame/";
        const string strJumpBefore = "Yindaoyu_#49_G_D";
        const string strJumpSuc = "Yindaoyu_#50_G_D";
        const string strTryAgain = "Yindaoyu_#51_G_D";
        const string strClickBomb = "Yindaoyu_#51a1_G_D";
        const string strHitCat = "Yindaoyu_#51a2_G_D";
        const string strEnd = "Yindaoyu_#51a3_G_D";


        enum GuideStep
        {
            BeforeJump,
            InJump,
            FromJumpToEnd,
            EndJump,
            BeforeSecondJump,
            InSecondJump,
            FromSecondJumpToEnd,
            EndSecondJump,
            BeforeThrow,
            EndThrow,
        }
        bool isInGuide = true;
        GuideStep step = GuideStep.BeforeJump;
        string key = "Run_IsGuideFinish";

        GameObject jumpGuideSprite;
        GameObject throwGuideSprite;

        void Awake()
        {
            //isInGuide = !PlayerPrefs.HasKey(key);
            isInGuide =  !PlayerManager.Instance.GetCharCounterData().GetFlag(5);
            if (isInGuide)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                var p = RunGameManager.Instance.Load("JumpGuideSprite");
                jumpGuideSprite = Instantiate(p);
                var r = jumpGuideSprite.GetComponent<RectTransform>();
                jumpGuideSprite.SetActive(false);
                r.SetParent(canvas.GetComponent<RectTransform>());
                r.localScale = Vector3.one;

                var prefab = RunGameManager.Instance.Load("ThrowGuideSprite");
                throwGuideSprite = Instantiate(prefab);
                var rec = throwGuideSprite.GetComponent<RectTransform>();
                throwGuideSprite.SetActive(false);
                rec.SetParent(canvas.GetComponent<RectTransform>());
                rec.localScale = Vector3.one;
            }
        }
        public void GuideToJump()
        {
            switch (step)
            {
                case GuideStep.BeforeJump:
                    Time.timeScale = 0;
                    AudioPlayer.Instance.PlayAudio(dir + strJumpBefore, RunGameManager.Instance.player.transform.position, false, GuideJumpAudioEnd);
                    jumpGuideSprite.SetActive(true);
                    break;
                case GuideStep.BeforeSecondJump:
                    Time.timeScale = 0;
                    AudioPlayer.Instance.PlayAudio(dir + strTryAgain, RunGameManager.Instance.player.transform.position, false, GuideSecondJumpAudioEnd);

                    break;

            }
        }

        private void GuideJumpAudioEnd(string str)
        {
            Debug.Log(str);
            step = GuideStep.InJump;
        }
        private void GuideSecondJumpAudioEnd(string str)
        {
            Debug.Log(str);
            step = GuideStep.InSecondJump;
        }

        public bool CanJump()
        {
            if (step == GuideStep.BeforeJump || step == GuideStep.BeforeSecondJump ||
                step == GuideStep.FromJumpToEnd || step == GuideStep.FromSecondJumpToEnd)
            {
                return false;
            }
            jumpGuideSprite.SetActive(false);
            Time.timeScale = 1;
            return true;
        }

        public void EndJump()
        {
            switch (step)
            {
                case GuideStep.InJump:
                    step = GuideStep.FromJumpToEnd;
                    Time.timeScale = 0;
                    AudioPlayer.Instance.PlayAudio(dir + strJumpSuc, RunGameManager.Instance.player.transform.position, false, EndJumpAudioEnd);
                    break;
                case GuideStep.InSecondJump:
                    step = GuideStep.FromSecondJumpToEnd;
                    Time.timeScale = 0;
                    AudioPlayer.Instance.PlayAudio(dir + strJumpSuc, RunGameManager.Instance.player.transform.position, false, EndSecondJumpAudioEnd);
                    break;
            }
        }

        private void EndJumpAudioEnd(string str)
        {
            Time.timeScale = 1;
            step = GuideStep.EndJump;
        }

        private void EndSecondJumpAudioEnd(string str)
        {
            Time.timeScale = 1;
            step = GuideStep.EndSecondJump;
        }

        public void BeginSecondJump()
        {
            step = GuideStep.BeforeSecondJump;
        }

        public void GuideToThrow()
        {
            step = GuideStep.BeforeThrow;
            Time.timeScale = 0;
            throwGuideSprite.SetActive(true);
            AudioPlayer.Instance.PlayAudio(dir + strClickBomb, RunGameManager.Instance.player.transform.position, false, ThrowAudioEnd);
        }

        private void ThrowAudioEnd(string str)
        {
            step = GuideStep.EndThrow;
        }

        public bool CanThrow()
        {
            if(step == GuideStep.BeforeThrow)
            {
                return false;
            }
            if(step == GuideStep.EndThrow)
            {
                throwGuideSprite.SetActive(false);
            }
            Time.timeScale = 1;
            return true;
        }

        public void HitMonster()
        {
            AudioPlayer.Instance.PlayAudio(dir + strHitCat, RunGameManager.Instance.player.transform.position, false, HitMonsterAudioEnd);
        }

        private void HitMonsterAudioEnd(string str)
        {
            IsInGuide = false;
            AudioPlayer.Instance.PlayAudio(dir + strEnd, RunGameManager.Instance.player.transform.position, false);
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
                //PlayerPrefs.SetInt(key,1);
                PlayerManager.Instance.GetCharCounterData().SetFlag(5, true);
            }
        }
    }
}
