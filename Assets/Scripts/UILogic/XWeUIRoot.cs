using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Base.Pattern;

public enum EUIAnchor
{
    eTop_Left = 0,
    eTop,
    eTop_Right,
    eLeft,
    eCenter,
    eRight,
    eBottom_Left,
    eBottom,
    eBottom_Right,
    eWindows,
    eUnknow,
    eCount
}

[AddComponentMenu("UILogic/XWeUIRoot")]
public class XWeUIRoot : MonoBehaviour
{ 
	public Transform[] UIAnchors = new Transform[(int)EUIAnchor.eCount];
    public HUDRoot hudRoot;
}
//
//class XWeUIManager : XSingleton<XWeUIManager>
//{
//    private XWeUIRoot m_WeUIRoot = null;
//
//    public void Init()
//    {
//        GameObject go = XUtil.Instantiate(StaticResourceManager.SP.WeUIRoot, LogicApp.SP.transform);
//        if (null == go)
//        {
//            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIManager, 没有指定WeUIRoot的索");
//            return;
//        }
//        m_WeUIRoot = go.GetComponent<XWeUIRoot>();
//
//        UICamera.UnPressDelegate += OnUnPress;
//    }
//
//
//    public void CreateUI(EUIPanel ePanelId)
//    {
//        m_WeUIRoot.CreateUI(ePanelId, true);
//    }
//
//    public void OriginalUI(EUIPanel ePanelId)
//    {
//        m_WeUIRoot.CreateUI(ePanelId, false);
//    }
//
//    public void OnUnPress()
//    {
//        if (!XUIPopMenu.isOper)
//        {
//            XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.ePopMenu);
//            XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipA);
//        }
//
//        XUIPopMenu.isOper = false;
//    }
//}
