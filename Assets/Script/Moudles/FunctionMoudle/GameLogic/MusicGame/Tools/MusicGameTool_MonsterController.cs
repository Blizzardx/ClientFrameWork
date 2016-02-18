using UnityEngine;
using System.Collections;

public class MusicGameTool_MonsterController : MonoBehaviour {

    public int NoteID;
    public Animator m_Animator;
    public ParticleSystem m_Particle;

    public void Start()
    {
        m_Animator = GetComponent<Animator>();
        if (m_Animator == null)
        {
            Debug.LogError("Animator Not Found");
        }
        m_Particle = GetComponentInChildren<ParticleSystem>();
        m_Particle.gameObject.SetActive(false);
    }

    public int OnClicked()
    {
        return NoteID;
    }
    
    public void PlayAnim (bool sucess)
    {
        if (m_Animator == null)
        {
            return;
        }
        if (sucess)
        {
            m_Animator.Play("eat");
        }
        else
        {
            m_Animator.Play("miss");
        }
    }

    public void Shining ()
    {
        m_Particle.gameObject.SetActive(true);
        Invoke("StopShining", 10f);
    }
    public void StopShining()
    {
        m_Particle.gameObject.SetActive(false);
    }
}
