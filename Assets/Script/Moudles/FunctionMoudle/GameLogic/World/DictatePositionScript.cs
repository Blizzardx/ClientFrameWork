using UnityEngine;
using System.Collections;

public class DictatePositionScript : MonoBehaviour
{

    //PlayerCharacter m_player;
    float lastTime = 0f;
    float curtTime = 0f;
    bool isRendering = false;

    public bool IsInCamera()
    {
        return isRendering;
    }

    // Use this for initialization
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    void Update()
    {
        isRendering = curtTime != lastTime ? true : false;
        lastTime = curtTime;
    }

    void OnWillRenderObject()
    {
        curtTime = Time.time;
    }

    void OnGUI()
    {
        GUILayout.TextField(gameObject.name + " : " + isRendering.ToString());

        //if (m_player == null)
        //{
        //    m_player = PlayerManager.Instance.GetPlayerInstance();
        //    return;
        //}
        //Vector3 distance = transform.position - ((CharTransformData)m_player.GetTransformData()).GetGameObject().transform.position;
        //GUILayout.TextField(gameObject.name + " : " + distance.magnitude.ToString());
    }

}
