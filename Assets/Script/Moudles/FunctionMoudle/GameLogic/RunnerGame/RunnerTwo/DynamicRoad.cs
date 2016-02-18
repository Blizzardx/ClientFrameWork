using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Run
{
    public class DynamicRoad : MonoBehaviour
    {
        [HideInInspector]
        public Transform trigger;
        float distanceFloor = 50;
        int curStep;
        Queue<Transform> floors = new Queue<Transform>();
        List<Obstacle> obstacles = new List<Obstacle>();
        List<Obstacle> obsToRemove = new List<Obstacle>();

        List<Transform> floorPrefabList;
        List<Transform> ObstaclePrefabList;
        Transform loopFloorPrefab;
        int curFloor;
        [HideInInspector]
        public int finishCount;

        // Use this for initialization
        void Start()
        {
            var prefab = RunGameManager.Instance.Load("Trigger");
            trigger = Instantiate(prefab).GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if(RunGameManager.Instance.player != null)
            {
                if(obstacles != null && obstacles.Count > 0)
                {
                    List<Obstacle> toRemove = new List<Obstacle>();
                    foreach(var o in obstacles)
                    {
                        if(o.transform.position.z < RunGameManager.Instance.player.transform.position.z - 1)
                        {
                            finishCount++;
                            toRemove.Add(o);
                        }
                    }
                    foreach(var o in toRemove)
                    {
                        obstacles.Remove(o);
                        obsToRemove.Add(o);
                        //Destroy(o.gameObject);
                    }
                }
            }
        }

        public void Reset()
        {
            curStep = 0;
            curFloor = 0;
            while (floors != null && floors.Count > 0)
            {
                var f = floors.Dequeue();
                f.gameObject.SetActive(false);
                Destroy(f.gameObject);
            }
            floors = new Queue<Transform>();
        }

        public void Init(List<Transform> floorList,Transform loop)
        {
            floorPrefabList = floorList;
            loopFloorPrefab = loop;
            ObstaclePrefabList = RunGameManager.Instance.obstaclePrefabList;

            AddLoopFloor();
            AddNormalFloor();
        }

        void AddFloor(Transform prefab)
        {
            var go = Instantiate(prefab);
            var t = go.transform;
            t.position = GetNextPosition();
            floors.Enqueue(t);
            trigger.position = t.position - new Vector3(0,0,20);
            curStep++;
            
        }

        void AddObstacle(Transform prefab)
        {
            var go = Instantiate(prefab);
            var t = go.transform;
            t.position = GetNextPosition();
            var list = t.GetComponentsInChildren<Obstacle>();
            obstacles.AddRange(list);
        }

        public void AddNormalFloor()
        {
            if(curFloor >= floorPrefabList.Count)
            {
                curFloor = 0;
                Debug.Log("restart born");
            }

            AddObstacle(ObstaclePrefabList[curFloor]);
            var go = floorPrefabList[curFloor];
            curFloor++;
            AddFloor(go);
        }

        public void AddLoopFloor()
        {

            AddFloor(loopFloorPrefab);
        }

        public void RemoveFloor()
        {
            var toRemove = floors.Dequeue();
            Destroy(toRemove.gameObject);
        }

        public Vector3 GetNextPosition()
        {
            return new Vector3(0, 0, curStep * distanceFloor);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(0,0,(curStep-1)*distanceFloor);
        }
    }

}
