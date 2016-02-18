using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Config;
using Config.Table;

namespace RatioGame
{
    public class RatioGameManager : SingletonTemplateMon<RatioGameManager>
    {

        //Dictionary<int, RatioGameConfig> levelConfig;

        public Transform origion;
        public List<Ball> ballList;

        public List<Color> colorList;
        public HealthBar healthbar;
        public Cat cat;
        public Text result;
        public GameObject explosion;
        public GameObject blinking;

        int health = 3;
        bool isPlaying;

        int curLevel = 1;
        public int GetMaxLevel { get { return 3; } }
        public int GetCurLevel { get { return curLevel; } }

        BallGroup ballGroup;

        int right = 0;
        int wrong = 0;

        bool isStop = false;

        RatioGuide guide;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            _instance = this;
            guide = GetComponent<RatioGuide>();
        }

        // Use this for initialization
        void Start()
        {
            Init();
            if(AppManager.Instance == null)
            {
                GameSatrt();
            }
        }

        public void GameSatrt()
        {
            Begin();
        }

        void Init()
        {
            //if (levelConfig == null) Debug.LogError("No Ratio Game Data!");
        }
        /*
        public RatioGameConfig GetCurLevelConfig()
        {
            RatioGameConfig config;
            levelConfig.TryGetValue(GetCurLevel, out config);
            if (config == null) Debug.LogError("Ratio Game Data Error!");
            return config;
        }
        */
        void Reset()
        {
            curLevel = 1;
            health = 3;
            right = 0;
            wrong = 0;
            if(ballGroup != null)
            {
                ballGroup.Clear();
                ballGroup = null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isPlaying)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200f))
                {
                    var ball = hit.transform.GetComponent<Ball>();
                    if (ball != null)
                    {
                        ball.OnClicked();
                    }
                }
            }
        }

        void Begin()
        {
            isPlaying = true;
            healthbar.InitHealthBar(health);
            if (guide.IsInGuide)
            {
                guide.GuideStart();
            }
            else
            {
                Shoot();
            }

        }

        public void Shoot()
        {
            cat.Shoot();
        }

        public void TakeDamage()
        {
            healthbar.RemoveOneHealthIcon();
            health--;
            ballGroup.Clear();
            ballGroup = null;

            AdaptiveDifficultyManager.Instance.SetUserTalent("Lose", RatioHardManager.gameId);

            if (health <= 0)
            {
                Failed();
            }
            else
            {
                Shoot();
            }
        }

        public void Fire()
        {
            //GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>("Scene", AssetType.Char));
            GameObject go = new GameObject();
            go.transform.position = origion.position;
            var group = go.AddComponent<BallGroup>();
            if(ballGroup != null)
            {
                Destroy(ballGroup.gameObject);
            }
            ballGroup = group;
            var dif = AdaptiveDifficultyManager.Instance.GetGameDifficulty("BallCount", 10);
            int count = RatioHardManager.GetBallCount();
            int speed = RatioHardManager.GetBallSpeed();
            group.Init(speed, count);
        }

        public void WinOnce()
        {
            curLevel++;
            ballGroup.Clear();
            ballGroup = null;
            if (curLevel > GetMaxLevel)
            {
                Win();
            }
            else
            {
                Shoot();
            }
        }

        public void ClickRight()
        {
            wrong = 0;
            right++;
            AdaptiveDifficultyManager.Instance.SetUserTalent("Correct", RatioHardManager.gameId);
            if (right == 3)
            {
                AdaptiveDifficultyManager.Instance.SetUserTalent("Correct3", RatioHardManager.gameId);
            }
            if(right == 5)
            {
                AdaptiveDifficultyManager.Instance.SetUserTalent("Correct5", RatioHardManager.gameId);
                right = 0;
            }
        }

        public void ClickWrong()
        {
            right = 0;
            wrong++;
            AdaptiveDifficultyManager.Instance.SetUserTalent("Wrong",RatioHardManager.gameId);
            if(wrong == 3)
            {
                AdaptiveDifficultyManager.Instance.SetUserTalent("Wrong3", RatioHardManager.gameId);
            }
            if(wrong == 5)
            {
                AdaptiveDifficultyManager.Instance.SetUserTalent("Wrong5", RatioHardManager.gameId);
                wrong = 0;
            }
        }

        void Win()
        {
            //result.enabled = true;
            result.text = "You Win";
            System.Action<bool> fun = (res) =>
            {
                if (res)
                {
                    Reset();
                    GameSatrt();
                    WindowManager.Instance.CloseAllWindow();
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }

            };
            WindowManager.Instance.OpenWindow(WindowID.WinPanel, fun);


            AdaptiveDifficultyManager.Instance.SetUserTalent("Win", RatioHardManager.gameId);
        }

        void Failed()
        {
            //result.enabled = true;
            result.text = "You Lose";
            System.Action<bool> fun = (res) =>
            {
                if (res)
                {
                    Reset();
                    GameSatrt();
                    WindowManager.Instance.CloseAllWindow();
                }
                else
                {
                    WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                    //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
                }
            };
            WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);
        }

        public List<Ball> GetBallPrefabs(int count,int materialCount)
        {
            if(materialCount > ballList.Count)
            {
                materialCount = ballList.Count;
            }
            var list = RandomSortList(ballList);
            list = list.GetRange(0, materialCount);
            if (count <= materialCount)
            {
                return list;
            }
            var res = list;
            for (int i = 0; i < count - materialCount; i++)
            {
                res.Insert(Random.Range(0, res.Count), list[Random.Range(0, list.Count)]);
            }
            
            return res;
        }

        public Ball GetBallPrefab(int index)
        {
            return ballList[index];
        }

        public List<Color> GetBallColors(int count,int colorCount)
        {
            if (colorCount > colorList.Count)
            {
                colorCount = colorList.Count;
            }
            var list = RandomSortList(colorList);
            list = list.GetRange(0, colorCount);
            if (count <= colorCount)
            {
                return list;
            }
            var res = list;
            for (int i = 0; i < count - colorCount; i++)
            {
                res.Insert(Random.Range(0, res.Count), list[Random.Range(0, list.Count)]);
            }

            return res;
        }

        public Color GetBallColor(int index)
        {
            return colorList[index];
        }

        public List<T> RandomSortList<T>(List<T> ListT)
        {
            List<T> newList = new List<T>();
            foreach (T item in ListT)
            {
                newList.Insert(Random.Range(0, newList.Count + 1), item);
            }
            return newList;
        }

        public BallGroup GetBallGroup()
        {
            return ballGroup;
        }

        public static RatioGuide GetGuideMgr()
        {
            return Instance.guide;
        }

        public static bool IsStop
        {
            get { return Instance.isStop; }
            set { Instance.isStop = value; }
        }
    }
}

