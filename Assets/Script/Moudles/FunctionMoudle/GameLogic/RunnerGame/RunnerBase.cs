using UnityEngine;
using System.Collections;
using System;

namespace RunnerGame
{
    public class RunnerBase : MonoBehaviour,Ilife
    {

        protected Animator animator;
        protected CharacterController cc;
        public float initSpeed = 3f;
        public float rotateSpeed = 5f;
        Vector3[] path;
        Transform parent;
        public float speed;

        bool isArrive = true;
        int curPos;

        bool jumping = false;
        bool canJump = true;
        bool falling = false;
        bool waiting = false;
        float jumpTime;
        float delayTime;

        public float gravity = 3;
        public float jumpSpeed = 2;
        public float jumpStartRiseTime = 0.5f;
        public float jumpEndDelayTime = 0.5f;
        public float sideSpeed = 1;

        float side = 0;

        float ground;


        protected virtual void Awake()
        {
            parent = transform.parent;
            animator = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
        }

        public void Init(Vector3[] path,int pos)
        {
            this.path = path;
            parent.position = path[pos];
            Quaternion rot = Quaternion.LookRotation(path[curPos + 1] - parent.position);
            parent.rotation = rot;
            transform.localPosition = Vector3.zero;
            ground = transform.position.y;
            curPos = pos;
            speed = initSpeed;
            isArrive = false;
        }

        public void SpeedUp()
        {
            speed = 5;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            MoveRoot();

            Vector3 moveDirection = Vector3.zero;

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
                if(transform.position.y < ground + 0.1f)
                //if (cc.isGrounded)
                {
                    falling = false;
                    waiting = true;
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

            moveDirection += side * sideSpeed * transform.TransformDirection(Vector3.right);
            side = 0;
            
            cc.Move(moveDirection * Time.deltaTime);
        }

        public void Jump()
        {
            if (canJump)
            {
                animator.SetBool("Jump", true);
                jumping = true;
                canJump = false;
                jumpTime = jumpStartRiseTime; 
            }
        }

        /// <summary>
        /// -1 is left;1 is right
        /// </summary>
        /// <param name="side"></param>
        public void Turn(float side)
        {
            this.side = side;
        }

        void MoveRoot()
        {
            if (isArrive)
            {
                animator.SetFloat("Speed", 0);
                return;
            }

            if (speed > initSpeed)
            {
                speed -= Time.deltaTime;
            }
            if (speed < initSpeed)
            {
                speed = initSpeed;
            }


            if ((parent.position - path[curPos]).sqrMagnitude < 0.1f)
            {
                curPos++;
                if (curPos >= path.Length)
                {
                    isArrive = true;
                    RunnerGameManager.Instance.Reach();
                    return;
                }
            }
            var dir = path[curPos] - parent.position;
            dir = dir.normalized;
            parent.position = parent.position + dir * Time.deltaTime * speed;
            if (curPos + 1 < path.Length)
            {
                Quaternion rot = Quaternion.LookRotation(path[curPos + 1] - parent.position);
                parent.rotation = Quaternion.Slerp(parent.rotation, rot, rotateSpeed * Time.deltaTime);
            }

            animator.SetFloat("Speed", speed);
        }

        public Vector3 GetPosition()
        {
            return parent.position;
        }

        public int GetInstanceId()
        {
            return 1;
        }

        public Transform GetParent()
        {
            return parent;
        }

        public int GetCurPosIndex()
        {
            return curPos;
        }
    }
}