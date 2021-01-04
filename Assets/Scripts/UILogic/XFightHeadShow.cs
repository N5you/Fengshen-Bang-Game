using UnityEngine;

[AddComponentMenu("UILogic/XFightHeadShow")]
public class XFightHeadShow : XUIBaseLogic
{
	public UISlider	LeftSlider;
	public UISlider	RightSlider;
	public UISlider SoulSlider;
	
	public GameObject m_SoulEffectDemo = null;
	
	public GameObject m_SoulFullDemo = null;
	
	public GameObject m_ExplosionDemo = null;
	
	public GameObject m_EmptyDemo = null;
	
	public UILabel	LeftBattleValue;
	public UILabel	RightBattleValue;	
	
	public GameObject m_OverBase = null;
	public UIImageButton OverBtn;
	public UIButton		ReplayBtn;
	public UIButton		SpeedUp;
	public UIButton		SpeedDown;
	
	public bool 				isSpeedUp = false;
	public float 				initSpeed = 1.2f;
	
	public override bool Init()
	{	
		//战斗录像开始就1.2倍;
		Time.timeScale = initSpeed;
		
		UIEventListener Listen = UIEventListener.Get(OverBtn.gameObject);
		Listen.onClick	+= overFight;
		
		UIEventListener ListenR = UIEventListener.Get(ReplayBtn.gameObject);
		ListenR.onClick	+= ReplayFight;
		
		UIEventListener ListenUp = UIEventListener.Get(SpeedUp.gameObject);
		ListenUp.onClick	+= HandleSpeedUp;
		
		UIEventListener ListenDown = UIEventListener.Get(SpeedDown.gameObject);
		ListenDown.onClick	+= HandleSpeedDown;
		
       	return base.Init();
	}
	
	public override void Show()
	{
		base.Show ();
		
		SetSkipBtnEnable();
		Time.timeScale = initSpeed;
		isSpeedUp = false;
		SpeedUp.transform.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(541);
	}
	public override void Hide()
	{
		base.Hide();
		Time.timeScale = 1.0f;
	}
	
	
	public void SetSkipBtnEnable()
	{
		bool hasPassThrough = XLogicWorld.SP.SubSceneManager.mHasPassThrough;
		if(XLogicWorld.SP.MainPlayer.IsInSubScene && !hasPassThrough)
		{
			OverBtn.CommonTips = XStringManager.SP.GetString(540);
			OverBtn.normalSprite = "11000145";
			OverBtn.hoverSprite = "11000145";
			OverBtn.pressedSprite = "11000145";
			OverBtn.disabledSprite = "11000145";
		}
		else
		{
			OverBtn.CommonTips = "";
			OverBtn.normalSprite = "11000143";
			OverBtn.hoverSprite = "11000144";
			OverBtn.pressedSprite = "11000145";
			OverBtn.disabledSprite = "11000145";
		}
		OverBtn.UpdateImage();
	}
	
	public void overFight(GameObject go)
	{
		bool hasPassThrough = XLogicWorld.SP.SubSceneManager.mHasPassThrough;
		//test
		hasPassThrough =true;
		
		if(hasPassThrough || !XLogicWorld.SP.MainPlayer.IsInSubScene )
		{
			XEventManager.SP.SendEvent(EEvent.Show_Fight_Result_Direct);
		}
	}

	public void ReplayFight(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Show_Fight_Replay);
	}
	
	public void HandleSpeedUp(GameObject go)
	{
		if(isSpeedUp)
		{
			Time.timeScale -= 1.0f;
			isSpeedUp = false;
			SpeedUp.transform.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(541);
		}
		else
		{
			Time.timeScale += 1.0f;	
			isSpeedUp = true;
			SpeedUp.transform.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(542);
		}
	}
	
	public void HandleSpeedDown(GameObject go)
	{		
		Time.timeScale -= 1.0f;
	}
	
	public void SetBattleCutState(bool isBattleCutScene )
	{
		if(isBattleCutScene )
		{	
			//ReplayBtn.gameObject.SetActive(false );
			SpeedUp.gameObject.SetActive(false );
			//SpeedDown.gameObject.SetActive(false );
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eChatWindow);
			
			//hung right
			OverBtn.gameObject.transform.parent = SpeedUp.transform.parent;
			
			OverBtn.gameObject.transform.localPosition = SpeedUp.transform.localPosition;
			
			
		}else
		{
			//ReplayBtn.gameObject.SetActive(true );
			SpeedUp.gameObject.SetActive(true );
			//SpeedDown.gameObject.SetActive(true );
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eChatWindow);
			
			OverBtn.gameObject.transform.parent = m_OverBase.transform;
			OverBtn.gameObject.transform.localPosition = Vector3.zero;
		}
		
		
	}
}