using UnityEngine;
using System.Collections;
using System;

public class MusicGameTool_StarController : MonoBehaviour
{
    [Range(0f, 10f)]
    public float FallingSpeed = 3f;
    [Range(0f, 5f)]
    public float FadeOutSpeed = 2f;
    public ParticleSystem m_Particle;
    // Data 
    private MeshRenderer m_MeshRenderer;
    private Action m_StopFallCallback;
    // State
    private bool m_isFalling;
    private float m_fRenderAlpha = 1f;

    #region MonoBehavior
    void Awake()
    {
        //this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_TintColor",new Color(1f,0f,0f,0.5f));
        m_MeshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        Clear();
        m_Particle = GetComponentInChildren<ParticleSystem>();
        m_Particle.gameObject.SetActive(false);
    }
    void Update()
    {
        if (m_isFalling)
        {
            transform.Translate(-Vector3.up * FallingSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Public Interface
    public void StartFall()
    {
        m_MeshRenderer.enabled = true;
        m_MeshRenderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 1f));
        m_fRenderAlpha = 1f;
        m_isFalling = true;
    }
    public void PauseFall()
    {
        m_isFalling = false;
    }
    public void ResumeFall()
    {
        m_isFalling = true;
    }
    public void StopFall(Action callback = null)
    {
        StartCoroutine(RenderFadeOut());
        m_StopFallCallback = callback;
    }
    public void Clear()
    {
        m_isFalling = false;
        m_MeshRenderer.enabled = false;
        //this.gameObject.SetActive(false);
    }
    public void Shinging ()
    {
        m_Particle.gameObject.SetActive(true);
        Invoke("StopShinging", 10f);
    }
    public void StopShinging()
    {
        m_Particle.gameObject.SetActive(false);
    }
    #endregion

    #region System Method
    IEnumerator RenderFadeOut()
    {
        while (m_fRenderAlpha > 0)
        {
            m_MeshRenderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, m_fRenderAlpha));
            m_fRenderAlpha -= FadeOutSpeed * Time.deltaTime;
            yield return null;
        }
        m_StopFallCallback();
        Clear();
    }
    #endregion
}
