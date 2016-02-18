using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGUIAnimation : MonoBehaviour {

    public int framerate = 20;
    public bool ignoreTimeScale = true;
    public List<Texture> frames = new List<Texture>();

    UITexture sprite;
    float mUpdate = 0f;
    int mIndex = 0;
    // Use this for initialization
    void Start()
    {
        sprite = GetComponent<UITexture>();
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
                    sprite.mainTexture = frames[mIndex];
                }
            }
        }
    }
}
