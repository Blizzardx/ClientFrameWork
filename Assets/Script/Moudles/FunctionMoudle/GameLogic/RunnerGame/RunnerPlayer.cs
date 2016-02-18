using UnityEngine;
using System.Collections;

namespace RunnerGame
{
    public class RunnerPlayer : RunnerBase
    {
        int count;
        bool haveBomb;
        protected override void Update()
        {
            base.Update();

            if (Input.GetMouseButtonDown(0))
            {
                Jump();
            }
            int dir = 0;
            if (Input.GetKey(KeyCode.A))
            {
                dir = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir = 1;
            }
            Turn(dir);

            var o = RunnerGameManager.Instance.GetCurObstacle();
            if (o != null)
            {
                var dis = (o.position - transform.position).sqrMagnitude;
                if (dis < 2f)
                {
                    RunnerGameManager.Instance.RemoveCurObstacle();
                    count++;
                    if(count >= 3)
                    {
                        haveBomb = true;
                    }

                }
                //Debug.Log((o.position - transform.position).sqrMagnitude);

            }


        }

        public void OnTriggerEnter(Collider other)
        {
            RunnerGameManager.Instance.TakeDamage();
        }
    }

}
