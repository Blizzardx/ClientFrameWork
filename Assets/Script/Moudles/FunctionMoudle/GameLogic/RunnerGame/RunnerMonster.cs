using UnityEngine;
using System.Collections;

namespace RunnerGame
{
    public class RunnerMonster : RunnerBase
    {
        float time;
        protected override void Update()
        {
            base.Update();

            time += Time.deltaTime;
            if(time> 0.1f)
            {
                time = 0;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.1f, transform.forward, out hit, 2))
                {
                    Jump();
                }
            }
            
        }
    }

}
