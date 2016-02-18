//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ItemManager
//
// Created by : Baoxue at 2015/11/30 17:45:58
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using Moudles.BaseMoudle.Character;
using NetWork.Auto;

public class ItemManager : Singleton<ItemManager>
{
    private CharBagData m_CharBagData;

    public void Initialize()
    {
        m_CharBagData = PlayerManager.Instance.GetCharBagData();
        if (m_CharBagData.ItemInfoList.Count == 0)
        {
            RandomInit();
        }
    }
    public ItemConfig GetItemConfig(int id)
    {
        return ConfigManager.Instance.GetItemConfig(id);
    }
    public void AddItem(int id)
    {
        for (int i = 0; i < m_CharBagData.ItemInfoList.Count; ++i)
        {
            if (m_CharBagData.ItemInfoList[i].ItemId == id)
            {
                return;
            }
        }
        var item = new NetWork.Auto.ItemInfo();
        item.ItemId = id;
        item.Count = 1;
        m_CharBagData.ItemInfoList.Add(item);
        SaveData();
    }
    public void RemoveItem(int id)
    {
        for (int i = 0; i < m_CharBagData.ItemInfoList.Count; ++i)
        {
            if (m_CharBagData.ItemInfoList[i].ItemId == id)
            {
                m_CharBagData.ItemInfoList.RemoveAt(i);
                break;
            }
        }
        SaveData();
    }
    public bool IsExistItem(int id)
    {
        for (int i = 0; i < m_CharBagData.ItemInfoList.Count; ++i)
        {
            if (m_CharBagData.ItemInfoList[i].ItemId == id)
            {
                return m_CharBagData.ItemInfoList[i].Count > 0;
            }
        }
        return false;
    }
    private void SaveData()
    {
        PlayerManager.Instance.GetCharBagData().ItemInfoList = m_CharBagData.ItemInfoList;
    }
    public void RandomInit()
    {
        List<int> store = new List<int>();
        for (int i = 0; i < 7; ++i)
        {
            store.Add(i);
        }

        for (int i = 0; i < 4; ++i)
        {
            int random = UnityEngine.Random.Range(0, store.Count);
            var item = new ItemInfo();
            item.Count = 1;
            item.ItemId = store[random];
            m_CharBagData.ItemInfoList.Add(item);
            store.RemoveAt(random);
        }
        SaveData();
    }

    #region test code 
    /*
    private List<int> m_FireworkPlanList;
    public void AddItem(int id)
    {
        CheckInit();
        for (int i = 0; i < m_FireworkPlanList.Count; ++i)
        {
            if (m_FireworkPlanList[i] == id)
            {
                return;
            }
        }
        m_FireworkPlanList.Add(id);
        SaveData();
    }
    public void RemoveItem(int id)
    {
        CheckInit();
        for (int i=0;i<m_FireworkPlanList.Count;++i)
        {
            if(m_FireworkPlanList[i] == id)
            {
                m_FireworkPlanList.RemoveAt(i);
                break;
            }
        }
        SaveData();
    }
    public bool IsExistItem(int id)
    {
        CheckInit();
        for (int i=0;i<m_FireworkPlanList.Count;++i)
        {
           if(m_FireworkPlanList[i] == id)
            {
                return true;
            }
        }
        return false;
    }
    private void CheckInit()
    {
        if(null == m_FireworkPlanList)
        {
            m_FireworkPlanList = new List<int>();
            LoadData();
        }
    }
    private void SaveData()
    {
        PlayerPrefs.SetInt("mt_list_count", m_FireworkPlanList.Count);
        for(int i=0;i<m_FireworkPlanList.Count;++i)
        {
            PlayerPrefs.SetInt("mt_list_" + i, m_FireworkPlanList[i]);
        }
    }
    private void LoadData()
    {
        int count = PlayerPrefs.GetInt("mt_list_count", 0);
        int isInit = PlayerPrefs.GetInt("mt_list_isInit", 0);

        if (isInit == 0)
        {
            RandomInit();
        }
        else
        {
            for (int i = 0; i < count; ++i)
            {
                m_FireworkPlanList.Add(PlayerPrefs.GetInt("mt_list_" + i));
            }
        }        
    }
    private void RandomInit()
    {
        List<int> store = new List<int>();
        for(int i=0;i<7;++i)
        {
            store.Add(i);
        }

        for(int i=0;i<4;++i)
        {
            int random = UnityEngine.Random.Range(0, store.Count);
            m_FireworkPlanList.Add(store[random]);
            store.RemoveAt(random);
        }
        SaveData();
        PlayerPrefs.SetInt("mt_list_isInit", 1);
    }*/
    #endregion
}

