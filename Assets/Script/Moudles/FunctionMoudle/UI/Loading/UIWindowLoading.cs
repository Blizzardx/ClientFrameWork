using UnityEngine;
using System.Collections.Generic;

public class UIWindowLoading : WindowBase
{
    private UIList m_ListTest;
    private List<int> m_ListData;

    public override void OnInit()
    {
        base.OnInit();
        m_ListTest = FindChildComponent<UIList>("UIList");
        m_ListTest.InitUIList<ListTestItem>();
        m_ListData = new List<int>();
        for (int i = 0; i < 10; ++i)
        {
            m_ListData.Add(i);
        }
    }

    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        m_ListTest.SetData(m_ListData);
    }
}

public class ListTestItem : UIListItemBase
{
    
}
