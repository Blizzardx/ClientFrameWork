using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UGUI
{
    public class UIAnimation : MonoBehaviour
    {
        public int framerate = 20;
        public bool ignoreTimeScale = true;
        public List<Sprite> frames = new List<Sprite>();

        Image sprite;
        float mUpdate = 0f;
        int mIndex = 0;
        // Use this for initialization
        void Start()
        {
            sprite = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (framerate != 0 && frames != null && frames.Count > 0)
            {
                float time = ignoreTimeScale ? RealTime.time : Time.time;

                if (mUpdate < time)
                {
                    mUpdate = time;
                    mIndex = NGUIMath.RepeatIndex(framerate > 0 ? mIndex + 1 : mIndex - 1, frames.Count);
                    mUpdate = time + Mathf.Abs(1f / framerate);

                    if (sprite != null)
                    {
                        sprite.sprite = frames[mIndex];
                    }
                }
            }
        }
    }

}
