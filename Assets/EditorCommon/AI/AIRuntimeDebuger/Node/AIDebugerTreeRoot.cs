using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;


public class AIDebugerTreeRoot : AIDebugerTreeNode
{
    private int m_iID;
    private string m_strDesc;

    public AIDebugerTreeRoot(int iId,  GameObject template)
        : base("Root", template)
    {
        m_iID = iId;
        
    }
    public int ID
    {
        get { return m_iID; }
    }

    public string Desc
    {
        get { return m_strDesc; }
        set
        {
            m_strDesc = value;
            m_LabelName.text = m_strDesc;
        }
    }
}