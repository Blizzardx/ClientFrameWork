using UnityEngine;
using System.Collections;

public class AnimationController
{
    private Animator m_Animator;

    public void Initialize(GameObject loader)
    {
        m_Animator = loader.GetComponent<Animator>();
    }
    public void PlayAnimation(string state)
    {
        m_Animator.Play(state);
    }
}
