using ActionEditor;
using UnityEngine;
using System.Collections;

public class EntityPlayAnimFrame : AbstractActionFrame
{
    private EntityPlayAnimationConfig m_FrameConfig;

    public EntityPlayAnimFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_FrameConfig = m_FrameData.EntityPlayAnim;
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

    }

    private void OnTrigger()
    {
        GameObject obj = null;
        switch (m_FrameConfig.EntityType)
        {
            case EntityType.Camera:
                obj = GlobalScripts.Instance.mGameCamera.transform.parent.gameObject;
                break;
            case EntityType.Npc:
                Ilife npc = LifeManager.GetLife(m_FrameConfig.CharId);
                if (null == npc || (!(npc is Npc)))
                {
                    Debuger.LogError("EntityPlayanim : can't load npc by id " + m_FrameConfig.CharId);
                    return;
                }
                obj = ((CharTransformData)((Npc)(npc)).GetTransformData()).GetGameObject();
                break;
            case EntityType.Player:
                if (null == PlayerManager.Instance.GetPlayerInstance())
                {
                    Debuger.LogError("EntityPlayanim : can't load player ");
                    return;
                }
                obj =
                    ((CharTransformData)(PlayerManager.Instance.GetPlayerInstance().GetTransformData())).GetGameObject();
                break;
        }
        Animation desAnim = obj.GetComponent<Animation>();
        if (null == desAnim)
        {
            desAnim = obj.AddComponent<Animation>();
        }
        GameObject animsource = ResourceManager.Instance.LoadBuildInResource<GameObject>("AnimationStore", AssetType.Animation);
        if (null == animsource)
        {
            Debuger.LogError("EntityPlayanim : can't load animation source store");
            return;
        }
        Animation sourceAnim = animsource.GetComponent<Animation>();
        if (null == sourceAnim)
        {
            Debuger.LogError("EntityPlayanim : can't load animation source store");
            return;
        }
        AnimationClip clip = sourceAnim.GetClip(m_FrameConfig.AnimName);
        if (null == clip)
        {
            Debuger.LogError("EntityPlayanim : can't load target clip on source anim store " + m_FrameConfig.AnimName + " count " + sourceAnim.GetClipCount());
            return;
        }
        
        desAnim.clip = clip;
        desAnim.Play();
    }
}