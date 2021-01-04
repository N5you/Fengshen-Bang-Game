using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XOperTip")]
public class XOperTip : XUIBaseLogic
{
	public UILabel			LabelContent;
	public UIImageButton	Btn;
	public UISprite			Sprite;
	
	private float UI_STAY_TIME = 3.0f;
	
	public override void Show()
	{
		base.Show();
		
		UIPanel panel = GetComponent<UIPanel>();
		panel.alpha	= 1.0f;
		Invoke("BeginDisplay",UI_STAY_TIME);
	}
	
	public override bool Init()
	{	
       	base.Init();
		
		UIEventListener ls = UIEventListener.Get(Sprite.gameObject);
		ls.onHover	= SpriteHover;
		
		if(Btn != null)
			Btn.gameObject.SetActive(false);
		
		return true;
		
	}
	
	public void SpriteHover(GameObject go, bool state)
	{
		if(state)
		{
			CancelInvoke();
		}
		else
		{
			BeginDisplay();
		}
	}
	
	private void BeginDisplay()
	{
		TweenAlpha Alpha = GetComponent<TweenAlpha>();
		if(Alpha == null)
			return ;
		Alpha.Reset();
		Alpha.enabled	= true;
	}
	
	public void EndHide()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eOperTip);
	}
	
}
