using ActionEditor;
using UnityEngine;
using System.Collections;

public class AddStateEffectFrame : AbstractActionFrame {
	
	private AddStateEffectFrameConfig m_FrameConfig;
	
	public AddStateEffectFrame(ActionPlayer action, ActionFrameData data)
		: base(action, data)
	{
		m_FrameConfig = m_FrameData.AddStateEffectFrame;
	}
	
	public override bool IsTrigger(float fRealTime)
	{
		if (null == m_FrameData)
		{
			return false;
		}
		
		if (fRealTime >= m_FrameData.Time)
		{
			return true;
		}
		
		return false;
	}
	
	public override bool IsFinish(float fRealTime)
	{
		return true;
	}
	
	public override void Play()
	{
		
	}
	
	protected override void Execute()
	{
		OnTrigger();
	}
	
	public override void Pause(float fTime)
	{
		
	}
	
	public override void Stop()
	{
		
	}
	
	public override void Destory()
	{
        uint id = (uint)(m_FrameConfig.InstanceId);
        GameObject objInstance = EffectContainer.GetInstance(id);
		if (null != objInstance)
		{
			GameObject.Destroy(objInstance);
		}
	}
	
	private void OnTrigger()
	{
        if (m_FrameConfig.IsAttach)
        {
            // try get attach obj
            GameObject obj = null;
            switch (m_FrameConfig.EntityType)
            {
                case EntityType.Camera:
                    obj = GlobalScripts.Instance.mGameCamera.transform.parent.gameObject;
                    break;
                case EntityType.Npc:
                    Ilife npc = LifeManager.GetLife(m_FrameConfig.AttachNpcId);
                    if (null == npc || (!(npc is Npc)))
                    {
                        Debuger.LogError("Play audio : can't load npc by id " + m_FrameConfig.AttachNpcId);
                        return;
                    }
                    obj = ((CharTransformData)((Npc)(npc)).GetTransformData()).GetGameObject();
                    break;
                case EntityType.Player:
                    if (null == PlayerManager.Instance.GetPlayerInstance())
                    {
                        Debuger.LogError("Play audio : can't load player");
                        return;
                    }
                    obj =
                        ((CharTransformData)(PlayerManager.Instance.GetPlayerInstance().GetTransformData())).GetGameObject();
                    break;
            }
            var objInstance = CreateEffect();
            if(null == objInstance)
            {
                return;
            }
            var root = GetAttachPoint(obj);

            objInstance.transform.parent = root == null ? null: root.transform ;
            objInstance.transform.localPosition = m_FrameConfig.Pos.GetVector3();
            objInstance.transform.localEulerAngles = m_FrameConfig.Rot.GetVector3();
        }
        else
        {
            var objInstance = CreateEffect();
            if (null == objInstance)
            {
                return;
            }
            objInstance.transform.position = m_FrameConfig.Pos.GetVector3();
            objInstance.transform.eulerAngles = m_FrameConfig.Rot.GetVector3();
        }

        
	}
    private GameObject CreateEffect()
    {
        //try get target
        uint id = (uint)(m_FrameConfig.InstanceId);
        GameObject objInstance = EffectContainer.EffectFactory(m_FrameConfig.EffectName, id);
        

        return objInstance;
    }
    private GameObject GetAttachPoint(GameObject root)
    {
        if(root == null || string.IsNullOrEmpty(m_FrameConfig.AttachPoingName))
        {
            return root;
        }
        
        return ComponentTool.FindChild(m_FrameConfig.AttachPoingName, root);
    }
}
