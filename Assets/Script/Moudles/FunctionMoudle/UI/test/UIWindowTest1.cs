using UnityEngine;
using System.Collections.Generic;

public class UIWindowTest1 : WindowBase
{
    private UIList      m_ListTest;
    private List<int>   m_TestData;

    public override void OnInit()
    {
        base.OnInit();
        m_ListTest = FindChildComponent<UIList>("UIList");
        m_ListTest.InitUIList<TestListItem>();
        m_TestData = new List<int>();
        for (int i = 0; i < 20; ++i)
        {
            m_TestData.Add(i);
        }
    }

    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        m_ListTest.SetData(m_TestData);
    }
}

public class TestListItem : UIListItemBase
{
    public override void OnInit()
    {
        base.OnInit();

    }

    public override void OnData()
    {
        base.OnData();
    }
}
