using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatioGame
{
    public class BallGroup : MonoBehaviour
    {

        float speed;
        Camera cam;
        List<Ball> ballList = new List<Ball>();
        int curIndex = 0;
        int amount = 0;

        float speedDampingY = 0.00004f;
        float speedDampingZ = 0.0003f;

        float offsetScale = 0.2f;

        public void Init(float speed, int amount)
        {
            if (RatioGameManager.GetGuideMgr().IsInGuide)
            {
                InitGuide();
                return;
            }

            this.speed = speed;
            this.amount = amount;
            int mCount = RatioHardManager.GetBallMaterial();
            var prefabs = RatioGameManager.Instance.GetBallPrefabs(amount, mCount);
            int cCount = RatioHardManager.GetBallColor();
            var colors = RatioGameManager.Instance.GetBallColors(amount, cCount);
            for (int i = 0; i < amount; i++)
            {
                var ball = Instantiate(prefabs[i]);
                var renderer = ball.GetComponent<Renderer>();
                renderer.material.color = colors[i];
                ball.gameObject.name = i.ToString();
                ball.transform.parent = transform;
                ball.transform.localScale = Vector3.one * (0.8f + offsetScale * i);
                ball.Init(i, this);
                ballList.Add(ball);
            }

            var randomList = RatioGameManager.Instance.RandomSortList<Ball>(ballList);

            int half = Mathf.CeilToInt(amount / 2f);
            int mod = amount % 2;

            //set position
            float offset = 2.5f - amount * 0.1f;
            if (mod == 0)
            {
                for (int i = 0; i < half; i++)
                {
                    randomList[i].transform.localPosition = new Vector3(-(i * offset + offset / 2f), 0, 0);
                    randomList[amount - 1 - i].transform.localPosition = new Vector3(i * offset + offset / 2f, 0, 0);
                }
            }
            else
            {
                for (int i = 0; i < half; i++)
                {
                    randomList[half + i - 1].transform.localPosition = new Vector3(i * offset, 0, 0);
                    randomList[half - i - 1].transform.localPosition = new Vector3(-i * offset, 0, 0);
                }
            }
        }

        void InitGuide()
        {
            this.speed = 100;
            this.amount = 3;
            var prefab = RatioGameManager.Instance.GetBallPrefab(0);
            var color = RatioGameManager.Instance.GetBallColor(0);
            for (int i = 0; i < amount; i++)
            {
                var ball = Instantiate(prefab);
                var renderer = ball.GetComponent<Renderer>();
                renderer.material.color = color;
                ball.gameObject.name = i.ToString();
                ball.transform.parent = transform;
                ball.transform.localScale = Vector3.one * (0.8f + offsetScale * i);
                ball.Init(i, this);
                ballList.Add(ball);
                float offset = 2.5f - amount * 0.1f;
                ball.transform.localPosition = new Vector3(-offset + i*offset, 0, 0);
            }
        }
        public void OnClicked(Ball ball)
        {
            if (RatioGameManager.GetGuideMgr().IsInGuide)
            {
                if (ball.id == curIndex)
                {
                    if (RatioGameManager.GetGuideMgr().CanClick(curIndex))
                    {
                        var exp = Instantiate(RatioGameManager.Instance.explosion);
                        exp.layer = 1;
                        exp.transform.position = ball.transform.position;
                        ballList.Remove(ball);
                        Destroy(ball.gameObject);
                        curIndex++;
                    }
                }
                return;
            }

            if (ball.id == curIndex)
            {
                var exp = Instantiate( RatioGameManager.Instance.explosion);
                exp.transform.position = ball.transform.position;
                ballList.Remove(ball);
                Destroy(ball.gameObject);
                curIndex++;
                if (curIndex == amount)
                {
                    RatioGameManager.Instance.WinOnce();
                }

                RatioGameManager.Instance.ClickRight();
            }
            else
            {
                Hashtable args = new Hashtable();
                args.Add("amount", new Vector3(0.1f, 0.1f, 0.1f));
                args.Add("isLocal", true);
                args.Add("time", 0.5f);
                iTween.ShakePosition(ball.gameObject, args);

                RatioGameManager.Instance.ClickWrong();
            }
        }

        public void Clear()
        {
            foreach (var ball in ballList)
            {
                Destroy(ball);
            }
            Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if(RatioGameManager.IsStop)
            {
                return;
            }
            transform.position = transform.position - new Vector3(0, speedDampingY * speed, speedDampingZ * speed);
            if (cam == null)
            {
                cam = Camera.main;
            }
            var maxAngle = cam.fieldOfView / 2;
            var pos = transform.position + new Vector3(0,1,0);
            var angle = Vector3.Angle(cam.transform.forward, pos - cam.transform.position);
            if (angle >= maxAngle)
            {
                RatioGameManager.Instance.TakeDamage();
            }
            /*
            transform.position = transform.position - new Vector3(0, speedDamping * speed, 0);
            if (transform.position.y < -4)
            {
                RatioGameManager.Instance.TakeDamage();
            }
            */
        }

        public List<Ball> GetBalls()
        {
            return ballList;
        }

        public Ball GetCurBall()
        {
            foreach(var ball in ballList)
            {
                if (ball.id == curIndex)
                    return ball;
            }
            return null;
        }
    }
}

