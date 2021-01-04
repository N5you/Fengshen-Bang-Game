using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XFlyItem")]
public class XFlyItem : XUIBaseLogic
{
	public XActionIcon	ActionIcon;
	
	public override void Show()
	{
		base.Show();
		
		Invoke("TimerHide",1.2f);
	}
	
	private void TimerHide()
	{
		XEventManager.SP.SendEvent(EEvent.UI_MuliHide,PanelKey);
	}
}
