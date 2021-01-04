using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XPVPResult")]
public class XPVPResult : XUIBaseLogic
{
	public UISprite Sprite;
	public UIImageButton BtnOK;
	public UIImageButton BtnReplay;
	
	public UILabel	HounourLabel;
	public UILabel  MoneyLabel;
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener ls = UIEventListener.Get(BtnOK.gameObject);
		ls.onClick	+= ClickHandle;
		
		UIEventListener ls1 = UIEventListener.Get(BtnOK.gameObject);
		ls1.onClick	+= ReplayHandle;
		return true;
	}
	
	public void ClickHandle(GameObject go)
	{
		XBattleManager.SP.LeaveFightScenePVP();
		Hide();
	}
	
	public void ReplayHandle(GameObject go)
	{
		
	}
	
	public override void Show()
	{
		base.Show();
		
		if(Sprite == null)
			return ;
		
		TweenPosition posEffect = Sprite.gameObject.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
		
		TweenScale scaleEffect = Sprite.gameObject.GetComponent<TweenScale>();
		if(scaleEffect != null)
		{
			scaleEffect.Reset();
			scaleEffect.enabled	= true;
		}
	}
	
	
}
