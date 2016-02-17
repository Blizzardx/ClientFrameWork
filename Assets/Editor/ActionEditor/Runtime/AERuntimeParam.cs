using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AERuntimeParam
{
    private List<Npc> m_CreatedNpcList;
    private PlayerCharacter m_PlayerChar;
    private ActionParam m_RuntimeParam;

    public List<Npc> GetNpcList()
    {
        if (null == m_CreatedNpcList)
        {
            m_CreatedNpcList = new List<Npc>();
        }
        return m_CreatedNpcList;
    }
    public PlayerCharacter GetPlayer()
    {
        return m_PlayerChar;
    }
    public void SetNpcList(List<Npc> list)
    {
        m_CreatedNpcList = list;
    }
    public void SetPlayer(PlayerCharacter player)
    {
        m_PlayerChar = player;
        PlayerManager.Instance.SetPlayerInstance(m_PlayerChar);
    }
    public ActionParam GetRuntimeActionParam()
    {
        CreateRuntimeParam();
        return m_RuntimeParam;
    }
    private void CreateRuntimeParam()
    {
        m_RuntimeParam = new ActionParam();
        FuncContext context = FuncContext.Create();
        m_RuntimeParam.Object = context;
        Npc target = null;
        if (m_CreatedNpcList != null && m_CreatedNpcList.Count > 0)
        {
            target = m_CreatedNpcList.ToArray()[0];
        }

        context.Put(FuncContext.ContextKey.User, m_PlayerChar);
        context.Put(FuncContext.ContextKey.Target, target);

        PlayerManager.Instance.SetPlayerInstance(m_PlayerChar);
    }
    public void ClearData()
    {
        PlayerManager.Instance.ClearPlayer();
        if (null != m_CreatedNpcList)
        {
            foreach (var elem in m_CreatedNpcList)
            {
                elem.Distructor();
            }
            m_CreatedNpcList.Clear();
        }
    }
}