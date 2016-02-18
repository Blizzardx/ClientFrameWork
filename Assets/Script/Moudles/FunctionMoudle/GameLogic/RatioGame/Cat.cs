using UnityEngine;
using System.Collections;

namespace RatioGame
{
    public class Cat : MonoBehaviour
    {
        Animator anim;

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Shoot()
        {
            anim.SetTrigger("Fire");
        }

        void Fire()
        {
            RatioGameManager.Instance.Fire();
        }
    }
}
