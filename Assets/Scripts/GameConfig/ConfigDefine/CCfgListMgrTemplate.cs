using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public abstract class CCfgListMgrTemplate<TManager, TItem> : XSingleton<TManager>, IConfigManager
    where TManager : class, new()
    where TItem : ITabItem, new()
{
    protected List<TItem> m_ItemTable = new List<TItem>();

    public virtual List<TItem> ItemTable { get { return m_ItemTable; } }

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
            m_ItemTable.Add(item);
        }
        return true;
    }
}
