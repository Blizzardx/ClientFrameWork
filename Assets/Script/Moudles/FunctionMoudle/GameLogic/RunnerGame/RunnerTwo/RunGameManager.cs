using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Run
{
    public enum GameState
    {
        RunningOnNormal,
        RunningOnLoop,
        WaitForThrow,
        AfterThrow,
        GameOver
    }

    [RequireComponent(typeof(DynamicRoad))]
    public class RunGameManager : SingletonTemplateMon<RunGameManager>
    {
        [System.Serializable]
        public class RoadInfo
        {
            public string roadName;
            public string obstacleName;
        }

        [System.Serializable]
        public class LevelData
        {
            public string levelName;
            public List<RoadInfo> roads;
            public string loopRoad;
        }

        public LevelData levelData;
        
        public List<Transform> roadPrefabList;
        public List<Transform> obstaclePrefabList;
        public Transform loopRoadPrefab;
        DynamicRoad dynamicRoad;
        public RunCharacter player;
        public RunMonster monster;
        public RunnerGame.CameraFollow cam;
        public RawImage bombImage;
        public Transform bombPrefab;
        public ParticleSystem bombEffect;
        public HealthBar healthBar;
        int health = 3;
        GameState state;
        int needThrowCount = 3;
        int count;
        RunGuide guide;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            _instance = this;
            dynamicRoad = GetComponent<DynamicRoad>();
            guide = GetComponent<RunGuide>();
        }

        // Use this for initialization
        void Start()
        {
            Init();
            if(AppManager.Instance == null)
            {
                GameStart();
            }
        }

        void Init()
        {
            InitConfigData();

            var res = Load(levelData.levelName, levelData.loopRoad);
            loopRoadPrefab = res.transform;


            dynamicRoad.Init(roadPrefabList, loopRoadPrefab);
        }
        public GameObject Load(string level,string name)
        {
            if (ResourceManager.Instance == null)
            {
                new GameObject("ResourceManager", typeof(ResourceManager));
                ResourceManager.Instance.Initialize();
            }
            return ResourceManager.Instance.LoadBuildInResource<GameObject>("RunnerGame/" + level+"/"+name, AssetType.Model);
        }

        public GameObject Load(string name)
        {
            if (ResourceManager.Instance == null)
            {
                new GameObject("ResourceManager", typeof(ResourceManager));
                ResourceManager.Instance.Initialize();
            }
            return ResourceManager.Instance.LoadBuildInResource<GameObject>("RunnerGame/" + name, AssetType.Model);
        }


        void InitConfigData()
        {
            var config = ConfigManager.Instance.GetRunnerGameSettingConfig();
            player.InitData(config.InitSpeed, config.Gravity, config.JumpSpeed, config.JumpStartRiseTime, config.JumpEndDelayTime);
            monster.InitData(config.InitSpeed, config.Gravity, config.JumpSpeed, config.JumpStartRiseTime, config.JumpEndDelayTime);
        }

        public void GameStart()
        {
            healthBar.InitHealthBar(health);
            SpawnPlayer();
            SpawnMonster();
            state = GameState.RunningOnNormal;

            if (InGuide())
            {
                needThrowCount++;
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            if (player != null && player.transform.position.z > dynamicRoad.trigger.position.z)
            {
                switch (state)
                {
                    case GameState.RunningOnNormal:
                        EnterRunningOnLoop();
                        break;
                    case GameState.RunningOnLoop:
                        EnterWaitForThrow();
                        break;
                    case GameState.WaitForThrow:
                        dynamicRoad.AddLoopFloor();
                        dynamicRoad.RemoveFloor();
                        break;
                    case GameState.AfterThrow:
                        EnterRunningOnNormal();
                        break;
                }
            }
        }

        void EnterWaitForThrow()
        {
            Debug.Log("Enter Wait For Throw");
            dynamicRoad.AddLoopFloor();
            dynamicRoad.RemoveFloor();

            bombImage.gameObject.SetActive(true);
            state = GameState.WaitForThrow;

            if (InGuide())
            {
                GetGuideMgr().GuideToThrow();
            }
        }

        void EnterRunningOnNormal()
        {
            Debug.Log("Enter Running On Normal");
            dynamicRoad.AddNormalFloor();
            dynamicRoad.RemoveFloor();
            state = GameState.RunningOnNormal;
        }

        void EnterRunningOnLoop()
        {
            Debug.Log("Enter Running On Loop");
            dynamicRoad.AddLoopFloor();
            dynamicRoad.RemoveFloor();
            state = GameState.RunningOnLoop;
        }

        void SpawnPlayer()
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(0, 50, -20), -Vector3.up, out hit))
            {
                player.Init(hit.point);
                cam.SetTaget(player.transform);
                player.StartRun();
            }
        }

        void SpawnMonster()
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(0, 50, -5), -Vector3.up, out hit))
            {
                monster.Init(hit.point,player);
                monster.StartRun();
            }
        }
        public void Throw()
        {
            bombImage.gameObject.SetActive(false);
            Debug.Log("Throw");
            var b = Instantiate(bombPrefab);
            b.position = player.transform.position;
            StartCoroutine(Track(b));
            count++;
            state = GameState.AfterThrow;
        }

        IEnumerator Track(Transform b)
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                b.position = Vector3.Lerp(player.transform.position, monster.transform.position, time);
                var dis = b.position - monster.transform.position;
                if(dis.sqrMagnitude < 0.5f)
                {
                    break;
                }
                yield return null;
            }

            monster.TakeDamage();

            var eff = Instantiate(bombEffect);
            eff.transform.position = b.position;

            Destroy(b.gameObject);
            
            if(count >= needThrowCount)
            {
                Win();
            }

            if (InGuide())
            {
                GetGuideMgr().HitMonster();
            }
        }

        public void TakeDamage()
        {
            if (state == GameState.GameOver)
                return;

            health--;
            healthBar.RemoveOneHealthIcon();

            if (health <= 0)
            {
                Failed();
            }

            if (InGuide())
            {
                if(health == 2 && dynamicRoad.finishCount == 1)
                {
                    GetGuideMgr().BeginSecondJump();
                }
            }
        }

        void Win()
        {
            player.StopRun();
            state = GameState.GameOver;
            System.Action<bool> fun = (res) =>
            {
                if (res)
                {
                    StageManager.Instance.ChangeState(GameStateType.RunnerGameState);
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }

            };
            WindowManager.Instance.OpenWindow(WindowID.WinPanel, fun);
        }

        void Failed()
        {
            player.StopRun();
            state = GameState.GameOver;
            System.Action<bool> fun = (res) =>
            {
                if (res)
                {
                    StageManager.Instance.ChangeState(GameStateType.RunnerGameState);
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }
            };
            WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);

        }

        public static bool InGuide()
        {
            if(Instance.guide != null)
            {
                return Instance.guide.IsInGuide;
            }
            return false;
        }

        public static RunGuide GetGuideMgr()
        {
            return Instance.guide;
        }

        public static GameState GetGameState()
        {
            return Instance.state;
        }

    }
}
