using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public abstract class CCfg1KeyMgrTemplate<TManager, TKey, TItem> : XSingleton<TManager>, IConfigManager
    where TManager : class, new()
    where TItem : ITabItemWith1Key<TKey>, new()
{
    protected SortedList<TKey, TItem> m_ItemTable = new SortedList<TKey, TItem>();

    public virtual  SortedList<TKey, TItem> ItemTable { get { return m_ItemTable; } }

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
                continue;
            }
            if (m_ItemTable.ContainsKey(item.GetKey1()))
            {
                Log.Write(LogLevel.ERROR, "[ERROR] Failed to init TabManager:{0}, multi key:{1}, line:{2}", this.ToString(), item.GetKey1(), tf.CurrentLine);
                continue;
            }
            m_ItemTable.Add(item.GetKey1(), item);
        }
        return true;
    }

    public virtual TItem GetConfig(TKey key)
    {
        if (m_ItemTable.ContainsKey(key))
        {
            return m_ItemTable[key];
        }
        return default(TItem);
    }

    public virtual bool Has(TKey key)
    {
        return m_ItemTable.ContainsKey(key);
    }

    public virtual TItem this[TKey key]
    {
        get { return GetConfig(key); }
    }
}
