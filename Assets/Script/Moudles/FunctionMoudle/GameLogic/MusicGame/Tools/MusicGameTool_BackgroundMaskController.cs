using UnityEngine;
using System.Collections;

public class MusicGameTool_BackgroundMaskController : MonoBehaviour
{
    //public GameObject test;
    MeshRenderer m_MeshRender;

    // Use this for initialization
    void Start()
    {
        m_MeshRender = GetComponent<MeshRenderer>();
        if (m_MeshRender)
            m_MeshRender.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (test != null)
        //{
        //    SetMaskPos(test.transform.position);
        //}
    }

    public void SetActive(bool active)
    {
        if (m_MeshRender)
            m_MeshRender.enabled = active;
    }

    public void SetMaskPos(Vector3 starPos)
    {
        float yValue = Mathf.InverseLerp(-6f, 6f, starPos.y);
        if (m_MeshRender)
        {
            m_MeshRender.material.SetFloat("_Posy", yValue);
        }

        float xValue = Mathf.InverseLerp(-10.67f, 10.67f, starPos.x);
        if (m_MeshRender)
        {
            m_MeshRender.material.SetFloat("_Posx", xValue);
        }

    }

}
