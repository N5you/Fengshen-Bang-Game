using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XAwardInfo")]
public class XAwardInfo : XUIBaseLogic
{
	public UILabel	label;
	public UIImageButton	Btn;
	
	public override bool Init()
	{	
       	base.Init();
		
		UIEventListener ls = UIEventListener.Get(Btn.gameObject);
		ls.onClick	= ClickHandle;
		
		return true;
	}
	
	public void ClickHandle(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eAwardInfo);
	}
}