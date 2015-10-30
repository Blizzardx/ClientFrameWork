using UnityEngine;
using System.Collections;

public class GameTestLogic: LogicBase<GameTestLogic>
{
    private bool m_bIsTouching;
    private Vector3 m_vInitCamPos;
    private Vector3 m_vInitPos;
    private Vector3 m_fDeltaDistance;
    private Transform m_Camera;

	public override void StartLogic()
	{
	   /* GameObject map =
	        GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>("Scene", AssetType.Map));
	    ComponentTool.Attach(null, map.transform);*/

        m_Camera = ComponentTool.FindChild("SceneCamera", null).transform;
        WindowManager.Instance.HideAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.WindowProject1);
        PlayerTickTask.Instance.RegisterToUpdateList(Update);

	}
	public override void EndLogic()
	{
        PlayerTickTask.Instance.UnRegisterFromUpdateList(Update);
	}

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                m_bIsTouching = true;
                m_vInitPos = Input.touches[0].position;
                m_vInitCamPos = m_Camera.position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                m_bIsTouching = false;

            }
            if (m_bIsTouching)
            {
                Vector3 currentPos = Input.touches[0].position;
                m_fDeltaDistance = (currentPos - m_vInitPos);
                m_fDeltaDistance.z = m_fDeltaDistance.y;
                m_fDeltaDistance.y = 0;
                m_Camera.position = m_vInitCamPos + m_fDeltaDistance*0.001f;
            }
        }

        /*if (Input.GetMouseButtonDown(0))
        {
            m_bIsTouching = true;
            m_vInitPos = Input.mousePosition;
            m_vInitCamPos = m_Camera.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_bIsTouching = false;

        }
        if (m_bIsTouching)
        {
            Vector3 currentPos = Input.mousePosition;
            m_fDeltaDistance = (currentPos - m_vInitPos);
            m_fDeltaDistance.z = m_fDeltaDistance.y;
            m_fDeltaDistance.y = 0;
            m_Camera.position = m_vInitCamPos + m_fDeltaDistance*0.001f;
        }*/
    }

    /// ... to do : 
}
