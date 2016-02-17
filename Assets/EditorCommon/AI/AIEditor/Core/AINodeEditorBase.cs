using UnityEngine;
using System.Collections;

public class AINodeEditorBase : MonoBehaviour
{
    public string m_strNodeName;
    protected AIDebugerTreeNode m_EditNode;
    public void SetWindowStatus(bool status)
    {
        gameObject.SetActive(status);
    }
    public void PariserNode()
    {
        
    }
    public virtual AIDebugerTreeNode GetNode()
    {
        return new AIDebugerTreeNode(m_strNodeName, UIWindow_BTViewPanel.GetInstance.m_NodeTempalte); ;
    }
    public virtual void InitPanel(AIDebugerTreeNode node)
    {
        m_EditNode = node;
    }

    virtual public void ResetToRef()
    {

    }
}
