using UnityEngine;
using System.Collections;

public class AIPlanElement : MonoBehaviour
{
    public UIButton m_Button;
    public UILabel m_Label;
    public int m_iId;
    public string m_strDesc;
    public void Init(int id, string desc, UIEventListener.VoidDelegate onClick)
    {
        m_iId = id;
        m_strDesc = desc;
        m_Label.text = "ID : " + id.ToString() + "  " + desc;
        UIEventListener.Get(m_Button.gameObject).onClick = onClick;
    }
}
