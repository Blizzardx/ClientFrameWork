using UnityEngine;
using System.Collections;

namespace Run
{
    public class RunPlayer : RunCharacter
    {
        bool isBlinking = false;
        float blinkTime = 1;
        float blinkDuration = 0.1f;
        float blinkLastTime;
        float blinkCount;
        float blinkMaxCount = 5;
        Renderer[] renderers;
        float time;

        protected override void Start()
        {
            base.Start();
            renderers = GetComponentsInChildren<Renderer>();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetMouseButtonDown(0))
            {
                switch (RunGameManager.GetGameState())
                {
                    case GameState.RunningOnNormal:
                        Jump();
                        break;
                    case GameState.RunningOnLoop:
                        Jump();
                        break;
                    case GameState.WaitForThrow:
                        Throw();
                        break;
                    case GameState.AfterThrow:
                        Jump();
                        break;
                }
            }
            time += Time.deltaTime;
            if (RunGameManager.InGuide() && time > 0.1f)
            {
                time = 0;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out hit, 2))
                {
                    if (hit.transform.GetComponent<Obstacle>() != null)
                    {
                        RunGameManager.GetGuideMgr().GuideToJump();
                    }
                }
            }


            if (isBlinking)
            {
                blinkLastTime += Time.deltaTime;
                if(blinkLastTime > blinkDuration)
                {
                    blinkLastTime = 0;
                    blinkCount++;
                    foreach(var r in renderers)
                    {
                        r.enabled = !r.enabled;
                    }
                }

                if (blinkCount > blinkMaxCount)
                {
                    isBlinking = false;
                    foreach(var r in renderers)
                    {
                        r.enabled = true;
                    }
                }
            }
        }

        public override void Jump()
        {
            if (RunGameManager.InGuide())
            {
                if (RunGameManager.GetGuideMgr().CanJump())
                {
                    base.Jump();
                }
            }
            else
            {
                base.Jump();
            }
        }

        public override void EndJumpEvent()
        {
            if (RunGameManager.InGuide())
            {
                RunGameManager.GetGuideMgr().EndJump();
            }
        }

        void Throw()
        {
            if (RunGameManager.InGuide())
            {
                if (RunGameManager.GetGuideMgr().CanThrow())
                {
                    RunGameManager.Instance.Throw();
                }
            }
            else
            {
                RunGameManager.Instance.Throw();
            }           
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Obstacle>())
            {
                RunGameManager.Instance.TakeDamage();
                Blink();
            }
        }

        void Blink()
        {
            isBlinking = true;
            blinkLastTime = 0;
            blinkCount = 0;
        }

    }

}
