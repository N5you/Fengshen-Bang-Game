using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XPlayHint")]
public class XPlayHint : XUIBaseLogic
{
	public UILabel	Level;
	public UILabel	Quest;
	public UISprite	BKSprite;
	public UISprite	PlaySprite;
	
	public override bool Init()
	{	
       	UIEventListener ls = UIEventListener.Get(BKSprite.gameObject);
		ls.onClick	+= ClickBK;
		return true;
	}
	
	public void ClickBK(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePractice);
	}
}
