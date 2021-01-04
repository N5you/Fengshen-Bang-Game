using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

class XDynamicAttrMgr
{
    public void Update(IList<Msg_PairII> lst)
    {
		foreach (Msg_PairII info in lst)
		{
            this.Set(info.First, info.Second);
		}
    }

    public void Set(int id, int value)
    {
        if (!m_AllAttr.Contains(id))
        {
            m_AllAttr.Add(id, value);
        }
        else
        {
            m_AllAttr[id] = value;
        }
    }

    public int Get(EShareAttr aIndex)
    {
		int id = (int)aIndex;
        if (m_AllAttr.Contains(id))
        {
            return (int)m_AllAttr[id];
        }
        return 0;
    }

    public bool Has(EShareAttr aIndex)
    {
        return m_AllAttr.Contains((int)aIndex);
    }

    private Hashtable m_AllAttr = new Hashtable(4);
}
