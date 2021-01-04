using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("UILogic/XOnlineReward")]
public class XOnlineReward : XUIBaseLogic
{
    public UILabel Tips;
    public GameObject ClickButton;
    public GameObject EffactePostion;

    protected XU3dEffect m_effect;

    public override bool Init()
    {
        base.Init();

        UIEventListener listenerClickButton = UIEventListener.Get(ClickButton);
        listenerClickButton.onClick += OnClickButton;

        return true;
    }

    void OnClickButton(GameObject go)
    {
        if (ClickButton == null)
        {
            Log.Write(LogLevel.ERROR, string.Format("ClickButton not Found", ClickButton));
            return;
        }
        XEventManager.SP.SendEvent(EEvent.OnlineReward_GetItem);
    }

    public void SetTipText(string text)
    {
        if (Tips == null)
        {
            Log.Write(LogLevel.ERROR, string.Format("Tips not Found", Tips));
            return;
        }
        this.Tips.text = text;
    }
}
