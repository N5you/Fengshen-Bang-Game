using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public abstract class CCfg2KeyMgrTemplate<TManager, TKey1, TKey2, TItem> : XSingleton<TManager>, IConfigManager
    where TManager : class, new()
    where TItem : ITabItemWith2Key<TKey1, TKey2>, new()
{
    protected SortedList<TKey1, SortedList<TKey2, TItem>> m_ItemTable = new SortedList<TKey1, SortedList<TKey2, TItem>>();

    public virtual SortedList<TKey1, SortedList<TKey2, TItem>> ItemTable { get { return m_ItemTable; } }

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
                m_ItemTable.Add(item.GetKey1(), new SortedList<TKey2, TItem>());
            }
            else if (m_ItemTable[item.GetKey1()].ContainsKey(item.GetKey2()))
            {
                Log.Write(LogLevel.ERROR, "[ERROR] Failed to init TabManager:{0}, multi key1:{1}, key2:{2}, line:{2}",
                    this.ToString(), item.GetKey1(), item.GetKey2(), tf.CurrentLine);
                return false;
            }
            m_ItemTable[item.GetKey1()].Add(item.GetKey2(), item);
        }
        return true;
    }

    public virtual TItem GetConfig(TKey1 key1, TKey2 key2)
    {
        if (m_ItemTable.ContainsKey(key1) && m_ItemTable[key1].ContainsKey(key2))
        {
            return m_ItemTable[key1][key2];
        }
        return default(TItem);
    }

    public virtual SortedList<TKey2, TItem> GetGroup(TKey1 key1)
    {
        if (m_ItemTable.ContainsKey(key1))
        {
            return m_ItemTable[key1];
        }
        return null;
    }

    public virtual bool Has(TKey1 key1, TKey2 key2)
    {
        return m_ItemTable.ContainsKey(key1) && m_ItemTable[key1].ContainsKey(key2);
    }

    public virtual SortedList<TKey2, TItem> this[TKey1 key1]
    {
        get { return GetGroup(key1); }
    }

    public virtual TItem this[TKey1 key1, TKey2 key2]
    {
        get { return GetConfig(key1, key2); }
    }
}
