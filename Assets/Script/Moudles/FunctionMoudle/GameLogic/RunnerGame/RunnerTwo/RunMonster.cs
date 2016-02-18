using UnityEngine;
using System.Collections;

namespace Run
{
    public class RunMonster : RunCharacter
    {
        float time;
        RunCharacter target;
        float distance;
        bool isSpeedUp;

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            time += Time.deltaTime;
            if (time > 0.1f)
            {
                time = 0;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out hit, 2))
                {
                    if(hit.transform.GetComponent<Obstacle>() != null)
                    {
                        Jump();
                    }
                }
            }
            if (isSpeedUp)
            {
                var dis = Mathf.Abs(target.transform.position.z - transform.position.z);
                if(dis < distance)
                {
                    speed = Mathf.Lerp(speed, initSpeed * 2, Time.deltaTime);
                }
                else
                {
                    isSpeedUp = false;
                    speed = initSpeed;
                }
            }
        }

        public void Init(Vector3 position,RunCharacter target)
        {
            base.Init(position);
            isSpeedUp = false;
            this.target = target;
            distance = Mathf.Abs(target.transform.position.z - transform.position.z);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();

            isSpeedUp = true;

            animator.SetTrigger("Freeze");
            Freeze();
        }
    }

}
