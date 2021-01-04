using System;
using UnityEngine;

/*
public abstract class XTabElementBase
{
    public string Label { get; protected set; }
    public abstract void Read(TabFile tf, ref object value);
}

public class XTabElement<T> : XTabElementBase
{
    public XTabElement(string k)
    {
        Label = k;
    }

    public override void Read(TabFile tf, ref object value)
    {
        value = tf.Get<T>(Label);
    }
}
*/

public interface ITabItem
{
    bool ReadItem(TabFile tf);
}

public interface ITabItemWith1Key<TKey1> : ITabItem
{
    TKey1 GetKey1();
}

public interface ITabItemWith2Key<TKey1, TKey2> : ITabItemWith1Key<TKey1>
{
    TKey2 GetKey2();
}

public interface IConfigManager
{
    bool Init(TextAsset text);
}