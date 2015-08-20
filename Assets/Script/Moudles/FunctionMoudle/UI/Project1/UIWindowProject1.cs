using UnityEngine;
using System.Collections;

public class UIWindowProject1 : WindowBase
{
    private UISprite m_SpriteDog;

    public override void OnInit()
    {
        base.OnInit();
        InitUIComponent(ref m_SpriteDog, "m_SpriteDog");

        UIEventListener.Get(m_SpriteDog.gameObject).onClick = OnClickDog;
    }
    private void OnClickDog(GameObject go)
    {
        Debuger.Log("On click dog");
    }

    public override void OnOpen(object param)
    {
        base.OnOpen(param);
    }
}