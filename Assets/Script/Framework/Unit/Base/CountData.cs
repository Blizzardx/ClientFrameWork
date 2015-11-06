using UnityEngine;
using System.Collections.Generic;

public class CountData
{
    private List<byte> m_bCount;
    private List<short> m_sCount;
    private List<int> m_iCount;
    private List<bool> m_bFlag;

    public CountData()
    {
        m_bCount = new List<byte>(100);
        m_sCount = new List<short>();
        m_iCount = new List<int>();
        m_bFlag = new List<bool>();
    }
    virtual public byte GetByteCount(int index)
    {
        return m_bCount[index];
    }
    virtual public short GetShortCount(int index)
    {
        return m_sCount[index];
    }
    virtual public int GetIntCount(int index)
    {
        return m_iCount[index];
    }
    virtual public void SetByteCount(int index, byte value)
    {
        m_bCount[index] = value;
    }
    virtual public void SetShortCount(int index, short value)
    {
        m_sCount[index] = value;
    }
    virtual public void SetIntCount(int index, int value)
    {
        m_iCount[index] = value;
    }
    virtual public bool GetFlag(int index)
    {
        return m_bFlag[index];
    }
    virtual public void SetFlag(int index, bool value)
    {
        m_bFlag[index] = value;
    }
}