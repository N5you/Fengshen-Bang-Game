using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XMainPlayerInfo")]
public class XMainPlayerInfo : XUIBaseLogic
{
	private float NOTICE_MOVE_SPEED = 50.0f;
	private float CLIP_DIST = 400.0f;
	private float MID_NOTICE_TIME = 4.0f;
	TimeCalc	mTimeCalc	= new TimeCalc();
	bool mIsReduceTime = false;
	
	private string[] VIPSpriteName = {"","11000190","11000191","11000192","11000193","11000194","11000195"};
	
	public static GameObject RootParent;
	
	public UILabel VIP = null;
	public UILabel Name = null;
	public UILabel Level = null;
	public UISlider Health = null;	
	public UILabel Gold = null;
	public UILabel Silver = null;	
	public UITexture RoleHeadTex;
	public Texture	MaskTexture;
	public UILabel	BattleValue = null;
	public UISprite	SpriteFood;
	
	//System Top Notice
	public UILabel	LabelNotice;
	private Vector3	LabelStartPos;
	private Vector3 LabelEndPos;
	private bool IsNoticing = false;
	
	//System Mid Notice
	public UILabel	LabelSystemMidNotice;
	
	public bool IsFirstSetGameMoney = true;
	public bool IsFirstSetRealMoney = true;
	
	public UIImageButton	BtnAddHealth;
	
	public	UISprite 		SpriteVIP;
	
	
	public UIGrid BuffIconGrid = null;
	private SortedList<uint, XBuffIcon> m_BuffIconList;		// buff
	public UIGrid DebuffIconGrid = null;
	private SortedList<uint, XBuffIcon> m_DebuffIconList;	// debuff	
	public XBuffIcon BuffIconSample = null;
	
	public override bool Init ()
	{
		m_BuffIconList = new SortedList<uint, XBuffIcon>();
		m_DebuffIconList = new SortedList<uint, XBuffIcon>();
		
		LabelStartPos	= LabelNotice.cachedTransform.localPosition;
		LabelEndPos		= new Vector3(LabelStartPos.x - CLIP_DIST,0,LabelStartPos.z);
		
		UIEventListener ls = UIEventListener.Get(Health.gameObject);
		ls.onHover	+= HoverHealth;
		
		UIEventListener ls1 = UIEventListener.Get(BtnAddHealth.gameObject);
		ls1.onClick	+= ClickAddHealth;
		ls1.onHover	+= HoverAddHealth;
		
		UIEventListener ls2 = UIEventListener.Get(SpriteFood.gameObject);		
		ls2.onHover	+= HoverFoodSprite;
		
		RootParent = this.gameObject;
		
		return base.Init ();
	}
	
	public void HoverHealth(GameObject go, bool state)
	{
		if(state)
		{
			string str = string.Format(XStringManager.SP.GetString(266),XLogicWorld.SP.MainPlayer.Power,200.0f);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
			XEventManager.SP.SendEvent(EEvent.ToolTip_B,str,"","");	
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipB);
		}
	}
	// ========
	public void HoverAddHealthTip()
	{
		uint BuyCount = XLogicWorld.SP.MainPlayer.HealthBuyCount;
		
		if(BuyCount >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyTiliCount))
		{
			BtnAddHealth.CommonTips = string.Format(XStringManager.SP.GetString(731), XLogicWorld.SP.MainPlayer.VIPLevel + 1, XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyTiliCount ,XLogicWorld.SP.MainPlayer.VIPLevel + 1));
		}
		else
		{
			BtnAddHealth.CommonTips = XStringManager.SP.GetString(730);
		}
	}
	
	public void HoverAddHealth(GameObject go, bool state)
	{
		HoverAddHealthTip();
		if(state)
		{
			uint curVIP = XLogicWorld.SP.MainPlayer.VIPLevel;
			XCfgHealthBuy CfgBuy = XCfgHealthBuyMgr.SP.GetConfig(curVIP);
			if(CfgBuy == null)
				return ;
			
			if(CfgBuy.BuyCount == 0)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
				return ;
			}
			
			uint BuyCount = XLogicWorld.SP.MainPlayer.HealthBuyCount;
			
			if(BuyCount >= CfgBuy.BuyCount)
				return ;
			
			uint Cost = CfgBuy.FirstCost + BuyCount * CfgBuy.CostDelta;
			
			string str = string.Format(XStringManager.SP.GetString(267),Cost,CfgBuy.HealthValue);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
			XEventManager.SP.SendEvent(EEvent.ToolTip_B,str,"","");	
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipB);
		}
	}
	
	public void HoverFoodSprite(GameObject go, bool state)
	{
		if(state)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
			XEventManager.SP.SendEvent(EEvent.ToolTip_B,XStringManager.SP.GetString(268),"","");
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipB);
		}
	}
	
	public void ClickAddHealth(GameObject go)
	{		
		uint BuyCount = XLogicWorld.SP.MainPlayer.HealthBuyCount;
		
		if(BuyCount >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyTiliCount))
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
			return ;
		}
		
		uint curVIP = XLogicWorld.SP.MainPlayer.VIPLevel;
		XCfgHealthBuy CfgBuy = XCfgHealthBuyMgr.SP.GetConfig(curVIP);
		if(CfgBuy == null)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,27);
			return ;
		}		
		
		uint Cost = CfgBuy.FirstCost + BuyCount * CfgBuy.CostDelta;
		if(XLogicWorld.SP.MainPlayer.RealMoney < Cost)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,15);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
			return ;
		}
		
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(ClickAddHealthOK);
		string str = string.Format(XStringManager.SP.GetString(267),Cost,CfgBuy.HealthValue);
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,str);	
	}
	
	public void ClickAddHealthOK(GameObject go)
	{
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_BuyHealth, builder.Build());
	}
	
	public void SetVIPLevel(uint level)
	{
		if(level > 6)
			return ;
		if(level == 0)
		{
			SpriteVIP.gameObject.SetActive(false);
		}
		else
		{
			SpriteVIP.gameObject.SetActive(true);
			SpriteVIP.spriteName	= VIPSpriteName[level];	
		}
	}
	
	public void SetBuyHealthCount(uint count)
	{
		uint curVIP = XLogicWorld.SP.MainPlayer.VIPLevel;
		XCfgHealthBuy CfgBuy = XCfgHealthBuyMgr.SP.GetConfig(curVIP);
		if(CfgBuy == null)
			return ;
		
		if(CfgBuy.BuyCount == 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
			return ;
		}
		
		uint BuyCount = XLogicWorld.SP.MainPlayer.HealthBuyCount;
		
		if(BuyCount >= CfgBuy.BuyCount)
		{
			BtnAddHealth.isEnabled	= false;
			BtnAddHealth.UpdateImage();
		}
		else
		{
			BtnAddHealth.isEnabled	= true;
			BtnAddHealth.UpdateImage();
		}
	}
	
	public override void Show ()
	{
		base.Show ();
		BuffIconSample.gameObject.SetActive(false);
	}
	
	public void SetName(string _name)
	{
		Name.text = _name;
	}

	public void SetLevel(int _level)
	{
		Level.text = "" + _level;
	}

	public void SetGold(long _gold)
	{
		if(Gold == null)
			return ;
		Gold.text = "" + _gold;
		
		if(IsFirstSetRealMoney)
		{
			IsFirstSetRealMoney = false;
			return ;
		}
		
		TweenColorEx colorEx = Gold.GetComponent<TweenColorEx>();
		if(colorEx)
		{
			colorEx.Reset();
			colorEx.enabled	= true;
		}
		
		
	}

	public void SetSilver(long _silver)
	{
		if(Silver == null)
			return ;
		
		Silver.text = "" + _silver;
		
		if(IsFirstSetGameMoney)
		{
			IsFirstSetGameMoney = false;
			return ;
		}
		
		TweenColorEx colorEx = Silver.GetComponent<TweenColorEx>();
		if(colorEx)
		{
			colorEx.Reset();
			colorEx.enabled	= true;
		}
		
		
	}

	public void SetHealth(float _per)
	{
		Health.sliderValue = _per;
	}

//	public void SetStrenth(float _per)
//	{
//		Strenth.sliderValue = _per;
//	}
	
	// 根据buff类型获得buff的Gird和List
	private void getBuffGL(EBuffType buffType, out UIGrid grid, out SortedList<uint, XBuffIcon> list)
	{
		if(EBuffType.eBuffType_Buff == buffType)
		{
			grid = BuffIconGrid;
			list = m_BuffIconList;
			return;
		}
		grid = DebuffIconGrid;
		list = m_DebuffIconList;
	}
	
	public void AddBuffIcon(EBuffType buffType, uint BuffId, byte layer, int nAtlasId, string spriteName)
	{
		UIGrid grid; SortedList<uint, XBuffIcon> list;
		getBuffGL(buffType, out grid, out list);
	
		if(list.ContainsKey(BuffId))
		{
			Log.Write(LogLevel.WARN, "try to addbuff icon which has exists: {0}", BuffId);
			return;
		}
		XBuffIcon newIcon = XUtil.Instantiate<XBuffIcon>(BuffIconSample);
		newIcon.BuffId = BuffId;
		newIcon.gameObject.SetActive(true);
		newIcon.SetSprite(nAtlasId, spriteName);
		newIcon.SetLayer(layer);
		newIcon.transform.parent = grid.transform;
		newIcon.onMouseEnter += OnBuffIconMouseEnter;
		newIcon.onMouseExit += OnBuffIconMouseExit;
		newIcon.onRightClick += OnBuffIconRightClick;
		grid.repositionNow = true;
		list.Add(BuffId, newIcon);
	}
	
	public void SetBuffLayer(EBuffType buffType, uint BuffId, byte layer)
	{
		UIGrid grid; SortedList<uint, XBuffIcon> list;
		getBuffGL(buffType, out grid, out list);
		
		if(!list.ContainsKey(BuffId))
		{
			Log.Write(LogLevel.WARN, "try to set layer of buff which does not exists : {0}", BuffId);
			return;
		}
		list[BuffId].SetLayer(layer);
	}
	
	public void RemoveBuffIcon(EBuffType buffType, uint BuffId)
	{
		UIGrid grid; SortedList<uint, XBuffIcon> list;
		getBuffGL(buffType, out grid, out list);
		
		if(!list.ContainsKey(BuffId))
		{
			Log.Write(LogLevel.WARN, "try to remove bufficon which does not exists :{0}", BuffId);
			return;
		}
		list[BuffId].transform.parent = transform;
		Destroy(list[BuffId].gameObject);
		list.Remove(BuffId);
		grid.repositionNow = true;
	}
	
	public void OnBuffIconMouseEnter(uint BuffId)
	{
		XEventManager.SP.SendEvent(EEvent.buff_DisplayBuff, BuffId);
	}
	
	public void OnBuffIconMouseExit(uint BuffId)
	{
		XEventManager.SP.SendEvent(EEvent.buff_StopDisplayBuff, BuffId);
	}
	
	public void OnBuffIconRightClick(uint BuffId)
	{
		XEventManager.SP.SendEvent(EEvent.buff_RemoveBuff, BuffId);
	}
	
	public void NoticeAnim(string content)
	{
		if(LabelNotice == null)
			return ;
		
		UILabel temp = XUtil.Instantiate(LabelNotice);
		
		temp.text	= content;
		TweenPosition AnimPos = temp.GetComponent<TweenPosition>();
		if(AnimPos == null)
			return ;
		
		Vector2 size = temp.relativeSize;
		float realSize = size.x * temp.cachedTransform.localScale.x;
		
		AnimPos.Reset();
		AnimPos.from	= LabelStartPos;
		AnimPos.to		= new Vector3(LabelEndPos.x - realSize,LabelEndPos.y,LabelEndPos.z);
		
		float dist = Mathf.Abs(AnimPos.to.x - AnimPos.from.x);
		float time = dist / NOTICE_MOVE_SPEED;
		AnimPos.duration	= time;
		
		NcAutoDestruct  autoDes = temp.GetComponent<NcAutoDestruct>();
		if(autoDes != null)
		{
			autoDes.m_fLifeTime	= time + 1;
			autoDes.enabled	= true;
		}
		
		AnimPos.enabled = true;
		
		Invoke("NoticeFinish",time - 5);
	}
	
	public void NoticeFinish()
	{
		XEventManager.SP.SendEvent(EEvent.Notice_SystemTopFinish);
	}
	
	public void 	MidNoticeAnim(string content,bool isNeedCut)
	{
		if(LabelSystemMidNotice == null)
			return ;
		
		LabelSystemMidNotice.alpha = 1;
		LabelSystemMidNotice.text = content;
		LabelSystemMidNotice.color = new Color(LabelSystemMidNotice.color.r,LabelSystemMidNotice.color.g,LabelSystemMidNotice.color.b,1.0f);
		
		mTimeCalc.BeginTimeCalc(MID_NOTICE_TIME,true);
//		mIsReduceTime	= false;
//		
//		if(isNeedCut)
//		{
//			mIsReduceTime	= true;
//			mTimeCalc.BeginTimeCalc(MID_NOTICE_TIME / 2,true);
//		}
//		else
//		{
//			mTimeCalc.BeginTimeCalc(MID_NOTICE_TIME,true);
//		}
	}
	
	public void MidNoticeFinish()
	{
		TweenAlpha tweenAlpha = LabelSystemMidNotice.GetComponent<TweenAlpha>();
		if(tweenAlpha != null)
		{
			tweenAlpha.Reset();
			tweenAlpha.enabled	= true;
		}
		
		Invoke("MidNoticeAlphaFinish",1.8f);
	}
	
	private void MidNoticeAlphaFinish()
	{
		XEventManager.SP.SendEvent(EEvent.Notice_SystemMidFinish);
	}
	
	public void ReduceSystemMidTime()
	{
//		if(mIsReduceTime)
//			return ;
//		
//		mTimeCalc.CountTime(mTimeCalc.GetLeftTime()/2);
//		
//		mIsReduceTime	= true;
	}
	
	void Update()
	{
		if(mTimeCalc.IsStart())
		{
			if(!mTimeCalc.CountTime(Time.deltaTime))
				return ;
		}
		if(mTimeCalc.CountTime(Time.deltaTime))
		{
			//时间到了
			MidNoticeFinish();
		}
	}
}
