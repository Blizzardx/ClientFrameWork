using UnityEngine;
using System.Collections;

public class FireworkGameLogic : SingletonTemplateMon<FireworkGameLogic>
{
    public Vector3 m_FireFrom;
    public Vector3 m_FireTo;
    public GameObject m_FireObj;
    public float m_fFireDuringTime;
    public Vector3 m_EffectPos;
    public float m_fEffectDuringTime;
    private string m_strEffectName;
    private GameObject m_EffectObj;
    private UIWindowFirework m_Window;

    private void Awake()
    {
        _instance = this;
    }
    public void Initialize()
    {
        m_Window = (UIWindowFirework) WindowManager.Instance.GetWindow(WindowID.Firework);
    }
    public bool Fire(string effectName,Color color)
    {
        if (EffectContainer.GetEffectCount()>5)
        {
            Debuger.LogWarning("system busy");
            return false;
        }

        m_strEffectName = effectName;

        GameObject effect = EffectContainer.EffectFactory(m_strEffectName);
        if (effect == null)
        {
            Debuger.LogWarning("can't load target effect " + m_strEffectName);
            return false;
        }
        effect.transform.position = m_EffectPos;
        m_EffectObj = effect;

        SetEffectColor(m_EffectObj, color);
        m_Window.Reset();
        return true;
    }
    private void SetEffectColor(GameObject effect, Color color)
    {
        var obj = effect.transform.GetChild(0);
        if (null == obj)
        {
            return;
        }
        ParticleSystem component = obj.GetComponent<ParticleSystem>();
        component.startColor = color;
    }
    public void Exit()
    {
        WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
        //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
    }
}
