using UnityEngine;
using System.Collections;

public class scriptMain
{
    private static scriptMain m_Instance;
    public static scriptMain Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new scriptMain();
            }
            return m_Instance;
        }
    }

    public void Initialize()
    {
        Debuger.Log("Script initialize finished");
    }
    public void Update()
    {
    }
    public void Quit()
    {

    }

}
public class UIWindowTest4 : WindowBase
{

}