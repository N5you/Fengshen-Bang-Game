using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public abstract class CCfg1GroupMgrTemplate<TManager, TKey1, TItem> : XSingleton<TManager>, IConfigManager
    where TManager : class, new()
    where TItem : ITabItemWith1Key<TKey1>, new()
{
    protected SortedList<TKey1, List<TItem>> m_ItemTable = new SortedList<TKey1, List<TItem>>();

    public virtual SortedList<TKey1, List<TItem>> ItemTable { get { return m_ItemTable; } }

    public virtual bool Init(TextAsset text)
    {
        if (null == text)
        {
            Log.Write(LogLevel.ERROR, "[ERROR] Failed to init TabManager:{0}, text is null", this.ToString());
            return false;
        }

        if (null != m_ItemTable && m_ItemTable.Count > 0)
        {
            Log.Write(LogLevel.ERROR, "[ERROR] Failed to init TabManager:{0}, already inited", this.ToString());
            return false;
        }

        m_ItemTable.Clear();

        TabFile tf = new TabFile(text.name, text.text);
        while (tf.Next())
        {
            TItem item = new TItem();
            if (item.ReadItem(tf) == false)
            {
                Log.Write(LogLevel.ERROR, "[ERROR] Failed to init TabManager:{0}, read line error, line:{1}", this.ToString(), tf.CurrentLine);
                return false;
            }
            if (GetGroup(item.GetKey1()) == null)
            {
                m_ItemTable.Add(item.GetKey1(), new List<TItem>());
            }
            m_ItemTable[item.GetKey1()].Add(item);
        }
        return true;
    }

    public virtual List<TItem> GetGroup(TKey1 key1)
    {
        if (m_ItemTable.ContainsKey(key1))
        {
            return m_ItemTable[key1];
        }
        return null;
    }

    public virtual List<TItem> this[TKey1 key1]
    {
        get { return GetGroup(key1); }
    }
}
