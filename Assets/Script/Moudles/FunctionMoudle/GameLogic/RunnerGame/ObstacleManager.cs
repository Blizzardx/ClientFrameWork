using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RunnerGame
{
    public class ObstacleManager : MonoBehaviour
    {
        RunnerBase runner;
        bool isSpawning = false;
        float time;
        float interval;
        bool isNeedSpawn;
        public Transform prefab;

        LinkedList<Transform> list = new LinkedList<Transform>();

        public void StartSpawn(RunnerBase runner)
        {
            this.runner = runner;
            isSpawning = true;
            isNeedSpawn = true;
            time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (isSpawning)
            {
                time += Time.deltaTime;
                if(time >= interval)
                {
                    isNeedSpawn = true;
                }
                if (isNeedSpawn)
                {
                    Spawn();
                    interval = GetRandomTime();
                    time = 0;
                    isNeedSpawn = false;
                }
            }
        }

        void Spawn()
        {
            int distance = 5;
            int posIndex = runner.GetCurPosIndex() + distance;
            if (RunnerGameManager.Instance.IsInPath(posIndex)){
                Vector3 pos = RunnerGameManager.Instance.GetPosition(posIndex);
                Quaternion rot = RunnerGameManager.Instance.GetRotation(posIndex);
                var t = Instantiate(prefab, pos, rot) as Transform;
                list.AddLast(t);
            }
        }

        float GetRandomTime()
        {
            return 5;
        }

        public Transform GetCurObstacle()
        {
            var node = list.First;
            if (node == null)
                return null;
            return node.Value;
        }

        public void RemoveObstacle()
        {
            list.RemoveFirst();
        }
    }

}
