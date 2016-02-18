using UnityEngine;
using System.Collections;

namespace Run
{
    public class RunCharacter : MonoBehaviour
    {
        protected CharacterController cc;
        protected Animator animator;
        protected float initSpeed = 5f;
        protected float speed;
        protected bool isRunning = false;
        protected bool isFreezing = false;
        float freezeTime = 1;
        float freezeRemainTime;

        bool jumping = false;
        bool canJump = true;
        bool falling = false;
        bool waiting = false;
        float jumpTime;
        float delayTime;

        float gravity = 2;
        float jumpSpeed = 5;
        public float jumpStartRiseTime = 0.5f;
        public float jumpEndDelayTime = 0.5f;

        float ground;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            Vector3 moveDirection = Vector3.zero;

            if (isFreezing)
            {
                freezeRemainTime -= Time.deltaTime;
                if(freezeRemainTime <= 0)
                {
                    isFreezing = false;
                }
            }

            if (waiting)
            {
                delayTime -= Time.deltaTime;
                if (delayTime <= 0)
                {
                    canJump = true;
                    waiting = false;
                }
            }
            if (falling)
            {
                if (transform.position.y < ground + 0.1f)
                {
                    falling = false;
                    waiting = true;
                    EndJumpEvent();
                    animator.SetBool("Jump", false);
                    delayTime = jumpEndDelayTime;
                }
            }

            moveDirection.y -= gravity;

            if (jumping)
            {
                if (jumpTime <= 0)
                {
                    jumping = false;
                    falling = true;
                }
                jumpTime -= Time.deltaTime;
                moveDirection.y = jumpSpeed;
            }

            if (isRunning && !isFreezing)
            {
                moveDirection += transform.forward * speed;
            }
            
            cc.Move(moveDirection * Time.deltaTime);
            animator.SetFloat("Speed", cc.velocity.z);
        }

        public virtual void EndJumpEvent()
        {

        }

        public void Init(Vector3 position)
        {
            transform.position = position;
            ground = transform.position.y;           
        }

        public void InitData(double initSpeed, double gravity, double jumpSpeed, double jumpStartTime, double jumpEndTime)
        {
            this.initSpeed = (float)initSpeed;
            this.gravity = (float)gravity;
            this.jumpSpeed = (float)jumpSpeed;
            this.jumpStartRiseTime = (float)jumpStartTime;
            this.jumpEndDelayTime = (float)jumpEndTime;
        }

        public virtual void Jump()
        {
            if (canJump)
            {
                animator.SetBool("Jump", true);
                jumping = true;
                canJump = false;
                jumpTime = jumpStartRiseTime;
            }
        }

        public void Freeze()
        {
            isFreezing = true;
            freezeRemainTime = freezeTime;
        }

        public void StartRun()
        {
            isRunning = true;
            speed = initSpeed;
        }

        public void StopRun()
        {
            isRunning = false;
        }

        public virtual void TakeDamage()
        {

        }
    }

}
