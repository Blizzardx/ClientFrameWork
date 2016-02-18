using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Moudles.BaseMoudle.Converter;
using Cache;
using NetWork.Auto;

namespace RunnerGame
{
    enum GameMode
    {
        Catch,
        Escape
    }
    enum GameState
    {
        None,
        Preview,
        Playing,
        Finished,
    }
    public class RunnerGameManager : SingletonTemplateMon<RunnerGameManager>
    {
        public PathManager pathNode;
        public RunnerBase player;
        public RunnerBase monster;

        public CameraFollow cam;
        public HealthBar healthbar;
        int health = 3;

        ObstacleManager obsMg;

        Vector3[] truePath;

        GameMode mode;
        GameState state;
        float playTime;

        //for test
        public Text test;
        public Button catchBtn;
        public Button escapeBtn;


        void Awake()
        {
            _instance = this;
            obsMg = GetComponent<ObstacleManager>();
        }

        // Use this for initialization
        void Start()
        {
            InitPath();
            if (null == AppManager.Instance)
            {
                TimeManager.Instance.Initialize();
                CacheManager.Init(Application.persistentDataPath + "/Cache");
                LogManager.Instance.Initialize(true, true);
                ResourceManager.Instance.Initialize();
                TickTaskManager.Instance.InitializeTickTaskSystem();
                MessageManager.Instance.Initialize();
                ConverterManager.Instance.Initialize();
                //WindowManager.Instance.Initialize();
                AssetUpdateManager.Instance.CheckUpdate(() =>
                {
                    //catchBtn.onClick.AddListener(() => { Begin(GameMode.Catch); HideBtn(); });
                    //escapeBtn.onClick.AddListener(() => { Begin(GameMode.Escape); HideBtn(); });
                    Begin(GameMode.Catch);
                }, false);
            }
            else
            {
                //catchBtn.onClick.AddListener(() => { Begin(GameMode.Catch); HideBtn(); });
                //escapeBtn.onClick.AddListener(() => { Begin(GameMode.Escape); HideBtn(); });
                Begin(GameMode.Catch);
            }

        }

        void HideBtn()
        {
            catchBtn.gameObject.SetActive(false);
            escapeBtn.gameObject.SetActive(false);
        }

        void InitPath()
        {
            Transform[] path;
            path = pathNode.path.ToArray();
            var length = iTween.PathLength(path);
            int count = (int)(length / 2);
            truePath = new Vector3[count + 1];
            float part = length / count;
            for (int i = 0; i < count; i++)
            {
                var pos = iTween.PointOnPath(path, (part * i) / length);
                truePath[i] = pos;
            }
            truePath[count] = path[path.Length - 1].position;
        }

        void Begin(GameMode mode)
        {
            this.mode = mode;
            /*
            state = GameState.Preview;
            switch (mode)
            {
                case GameMode.Catch:
                    //player.Init(truePath, 0);
                    monster.Init(truePath, 0);
                    break;
                case GameMode.Escape:
                    player.Init(truePath, 5);
                    monster.Init(truePath, 0);
                    break;
            }
            */
            state = GameState.Playing;
            player.Init(truePath, 0);
            monster.Init(truePath, 5);
            playTime = 0;
            cam.SetTaget(player.GetParent());
            obsMg.StartSpawn(monster);
            healthbar.InitHealthBar(health);
        }       

        public void Reach (){
            
            switch (mode)
            {
                case GameMode.Catch:
                    Win();
                    break;
                case GameMode.Escape:
                    Win();
                    break;
            }
            
        }

        void Win()
        {
            state = GameState.Finished;
            ShowLog("Win!");
        }

        void Failed()
        {
            state = GameState.Finished;
            ShowLog("Failed!");
        }

        // Update is called once per frame
        void Update()
        {
            if (null == AppManager.Instance)
            {
                TickTaskManager.Instance.Update();
            }
            switch (state)
            {
                case GameState.Preview:
                    PreiewUpdate();
                    break;
                case GameState.Playing:
                    PlayingUpdate();
                    break;
                case GameState.Finished:
                    break;
            }
        }

        
        void PreiewUpdate()
        {
            playTime += Time.deltaTime;
            if(playTime >= 2)
            {
                state = GameState.Playing;
                switch (mode)
                {
                    case GameMode.Catch:
                        playTime = 0;
                        player.Init(truePath, 0);
                        cam.SetTaget(player.GetParent());
                        obsMg.StartSpawn(monster);
                        healthbar.InitHealthBar(health);
                        break;
                    case GameMode.Escape:
                        break;
                }
            }
        }

        void PlayingUpdate()
        {
            playTime += Time.deltaTime;
            switch (mode)
            {
                case GameMode.Catch:
                    /*
                    if ((player.GetPosition() - monster.GetPosition()).sqrMagnitude < 4)
                    {
                        Debug.Log("Win");
                        Win();
                    }
                    */
                    break;
                case GameMode.Escape:

                    break;
            }
        }
        void ShowLog(string str)
        {
            test.text = str;
            test.enabled = true;
        }

        public void TakeDamage()
        {
            health--;
            healthbar.RemoveOneHealthIcon();

            if(health <= 0)
            {
                Failed();
            }
        }

        #region path method
        public bool IsInPath(int index)
        {
            return index < truePath.Length;
        }

        public Vector3 GetPosition(int index)
        {
            return truePath[index];
        }

        public Quaternion GetRotation(int index)
        {
            Quaternion rot = Quaternion.identity;
            if (index == truePath.Length-1)
            {
                rot = Quaternion.LookRotation(truePath[index] - truePath[index - 1]);
            }
            else
            {
                rot = Quaternion.LookRotation(truePath[index + 1] - truePath[index]);
            }
            return rot;
        }
        #endregion

        #region Obstacle Method
        public Transform GetCurObstacle()
        {
            return obsMg.GetCurObstacle();
        }

        public void RemoveCurObstacle()
        {
            obsMg.RemoveObstacle();
        }
        #endregion
    }
}