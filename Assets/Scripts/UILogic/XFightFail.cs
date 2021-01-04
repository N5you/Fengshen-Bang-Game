using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XFightFail")]
public class XFightFail : XUIBaseLogic
{
	public GameObject ButtonConfirm = null;
	public UISprite ResultFont;
	
	public XFightFail()
	{
		XEventManager.SP.AddHandler(OnLeaveFightSce, EEvent.Fight_LeaveSce);
	}
	
	public override bool Init()
	{
		base.Init();
		UIEventListener listen = UIEventListener.Get(ButtonConfirm.gameObject);
		listen.onClick += OnClickConfirm;
		return true;
	}
	
	public override void Show()
	{
		base.Show();
		
		TweenScale scaleEffect = ResultFont.gameObject.GetComponent<TweenScale>();
		if(scaleEffect != null)
		{
			scaleEffect.Reset();
			scaleEffect.enabled	= true;
		}
		
		Invoke("showFinish", 1.5f);
		///
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eBattleFailGuide);
	}
	
	void showFinish()
	{
		OnClickConfirm(null);
	}

	public void OnClickConfirm(GameObject go)
	{
		XBattleManager.SP.LeaveFightScenePVE(false);
		Hide();
	}	
	
	public void OnLeaveFightSce(EEvent evt, params object[] args)
	{
		XBattleManager.SP.LeaveFightScenePVE(false);
		Hide();
	}
	
}
