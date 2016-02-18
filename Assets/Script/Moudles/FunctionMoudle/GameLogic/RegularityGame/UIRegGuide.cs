using UnityEngine;
using System.Collections;
using System;

namespace RegularityGame
{
    public class UIRegGuide : MonoBehaviour
    {
        public UIGrid grid;
        public GameObject itemPrefab;
        public GameObject mask;
        public UIWidget finger;

        GameObject apple;
        GameObject banana;
        // Use this for initialization
        void Start()
        {
            apple = Instantiate(itemPrefab);
            apple.SetActive(true);
            ComponentTool.Attach(grid.transform, apple.transform);
            UITexture appleT = apple.GetComponent<UITexture>();
            appleT.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>("Piles/Apple", AssetType.Texture);
            banana = Instantiate(itemPrefab);
            banana.SetActive(true);
            ComponentTool.Attach(grid.transform, banana.transform);
            UITexture bananaT = banana.GetComponent<UITexture>();
            bananaT.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>("Piles/Banana", AssetType.Texture);
            finger.transform.position = apple.transform.position;

            //UIEventListener.Get(child).onClick = OnClickOption;

            //m_OptionalMap.Add(name, new RegularityOptionalElement(name, child));
        }
        
        public void ShowMask()
        {
            mask.SetActive(true);
        }
        public void HideMask()
        {
            mask.SetActive(false);
        }

        Action cal;
        public void CanClick(Action callback = null)
        {
            cal = callback;
            UIEventListener.Get(banana).onClick = OnClickBanana;
        }

        void OnClickBanana(GameObject go)
        {
            TriggerToShowEnableOption(go);
            if (cal != null)
            {
                cal();
                cal = null;
            }
        }

        public void Shining(int amount,bool isAll, Action callback = null)
        {
            StartCoroutine(S(amount,isAll, callback));
        }

        public void ShowFinger()
        {
            finger.alpha = 1;
        }

        public void HideFinger()
        {
            finger.alpha = 0;
        }

        public void Hide()
        {
            grid.gameObject.SetActive(false);
        }

        IEnumerator S(int amount,bool isAll, Action callback = null)
        {
            while (amount > 0)
            {
                yield return new WaitForSeconds(0.5f);
                var aprs = apple.GetComponentsInChildren<UITexture>();
                if (isAll)
                {
                    foreach (var r in aprs)
                    {
                        r.enabled = !r.enabled;
                    }
                }
                
                var bars = banana.GetComponentsInChildren<UITexture>();
                foreach (var r in bars)
                {
                    r.enabled = !r.enabled;
                }
                yield return new WaitForSeconds(0.5f);
                if (isAll)
                {
                    foreach (var r in aprs)
                    {
                        r.enabled = !r.enabled;
                    }
                }
                foreach (var r in bars)
                {
                    r.enabled = !r.enabled;
                }
                amount--;
            }
            if (callback != null)
            {
                callback();
            }
        }

        private void TriggerToShowEnableOption(GameObject optionObj)
        {
            TweenScale tween = optionObj.GetComponent<TweenScale>();
            if (null != tween)
            {
                GameObject.Destroy(tween);
            }
            tween = optionObj.AddComponent<TweenScale>();
            tween.from = Vector3.one;
            tween.to = Vector3.zero;
            tween.duration = 0.5f;
            tween.AddOnFinished(() =>
            {
                GameObject.Destroy(tween);
                //CallBack();
            });
        }
    }
}
