using UnityEngine;
using System.Collections;

namespace RegularityGame
{
    public class SimpleUIGrid : MonoBehaviour
    {
        public float m_fSpace;

        // Use this for initialization
        void Start()
        {
        }
        public void Reposition()
        {
            float startX = transform.position.x - (transform.childCount - 1)*m_fSpace*0.5f;
            for (int i = 0; i < transform.childCount; ++i)
            {
                Vector3 tmp = transform.GetChild(i).transform.position;
                tmp.x = startX;
                startX += m_fSpace;
                transform.GetChild(i).transform.position = tmp;
            }
        }
    }
}

