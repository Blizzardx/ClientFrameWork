using UnityEngine;
using System.Collections;

namespace RatioGame
{
    public class Ball : MonoBehaviour
    {

        public int id;
        BallGroup parent;

        public void Init(int id, BallGroup parent)
        {
            this.id = id;
            this.parent = parent;
        }

        public void OnClicked()
        {
            parent.OnClicked(this);
        }
    }
}

