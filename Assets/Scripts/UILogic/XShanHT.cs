using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;

public enum GuanKa_Type
{
	GuanKa_Type_Normal, 		
	GuanKa_Type_Elite,			
	GuanKa_Type_Boss,
	GuanKa_Type_Num,
}
public enum GuanKa_State
{
	GuanKa_State_Not_Reach, 		
	GuanKa_State_Reach,			
	GuanKa_State_Pass,
	GuanKa_State_Num,
}

[AddComponentMenu("UILogic/XShanHT")]
public class XShanHT : XDefaultFrame
{
	private static readonly int MAX_SHAN_HE_NUM = 5;
	public static readonly int MAX_BATTLE_RESULT_NUM = 5;

	public static readonly int MAX_GUAN_KA_NUM = 20;

	private string normalSpriteName = "11000755";
	private string selectSpriteName = "11000757";
	

	
	private int  SelectIndx  = 0;
	public GameObject m_ShanHeRoot1 = null;
	public GameObject m_ShanHeRoot2 = null;
	
	[System.Serializable]	

	public class XuanGuan
	{
		[System.Serializable]	
		public class SelBtn
		{
			public UIImageButton	Btn;
			public UILabel Name;
			public UILabel Num;
			private int mIndex;
					
			public void Init(int index)
			{			
				UIEventListener ls = UIEventListener.Get(Btn.gameObject);
				ls.onClick	= Click;
				mIndex = index;
				XCfgShanHTBase curConfig = XCfgShanHTBaseMgr.SP.GetConfig((uint)index + 1);
				Name.text = curConfig.Name;
				Num.text = string.Format(XStringManager.SP.GetString(160), curConfig.StartIndex, curConfig.EndIndex);
			}
			
			public  void Click (GameObject go)
			{
				XEventManager.SP.SendEvent(EEvent.Click_SH_Btn,mIndex);
			}
		}
		
		public SelBtn[] 		SelBtnList = new SelBtn[MAX_SHAN_HE_NUM];
		public GameObject  		MyInfoPanel = null;
		public GameObject  		RankInfoPanel = null;
		public GameObject  		ShanHeInfoPanel = null;
		public UIImageButton 	StartBattle = null;	
		
		public bool Init()
		{
			for( int i = 0; i < MAX_SHAN_HE_NUM; ++i )
			{
				SelBtnList[i].Init(i);
			}
			return true;
		}	
	}

	public XuanGuan   m_XuanGuan = null;
	

	[System.Serializable]	

	public class ChuangGuan
	{

		[System.Serializable]	
		public class GuanKaInfo
		{
			private static string[] IconGrey = new string[3]{
				"11000764",
				"11000773",
				"11000774",
			};
			
			private static string[] IconLight = new string[3]{
				"11000761",
				"11000762",
				"11000763",
			};
			
			private static string[] BoxElite = new string[(int)GuanKa_State.GuanKa_State_Num]{
				"11000277",
				"11000277",
				"11000282",
			};
			
			private static string[] BoxBoss = new string[(int)GuanKa_State.GuanKa_State_Num]{
				"11000274",
				"11000274",
				"11000283",
			};
			private string ArrowDown = "11000766";
			private string ArrowDown_b = "11000767";
			private string ArrowUp = "11000768";
			private string ArrowUp_b = "11000769";
			
			public GameObject   mIconRoot;
			public UISprite 	mIcon;
			public UILabel		mName;
			public UISprite 	mArrow;
			public UISprite 	mBaoXiang;
			public bool          mbOpen;
			public bool          mbPass;
			
			public uint			mGuanKaIndex;
			
			public void Init(uint index,GameObject go)
			{
				mIconRoot = go;
				
				mGuanKaIndex = index;
				mIcon = go.transform.Find("icon").GetComponent<UISprite>();
				mArrow = go.transform.Find("arrow").GetComponent<UISprite>();
				mName = go.transform.Find("name").GetComponent<UILabel>();
				mBaoXiang = go.transform.Find("baoxiang").GetComponent<UISprite>();
				mName.text = string.Format(XStringManager.SP.GetString(164),index);
				if(index == 20)
					mArrow.gameObject.SetActive(false);
				
				UIEventListener ls = UIEventListener.Get(mBaoXiang.gameObject);
				ls.onHover += BaoXiangHover;
			}
			
			public  void BaoXiangHover(GameObject go, bool bHover)
			{	
				if(bHover)
				{
					uint Index = XShanHTManager.SP.GetCurShanHe();
					XCfgShanHTLevel curCfg = XCfgShanHTLevelMgr.SP.GetConfig(Index * 20  - 20 + mGuanKaIndex);
					if(curCfg == null)
						return;
					XCfgBoxTip curConfig = XCfgBoxTipMgr.SP.GetConfig(curCfg.TipID);
					if(curConfig == null)
						return;
					string strTip = curConfig.Content;
					XEventManager.SP.SendEvent(EEvent.ToolTip_B,XStringManager.SP.GetString(182),"",strTip);
					XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipB);
				}
				else
				{
					XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipB);
				}
			}
			
			
			public void SetState(GuanKa_State state,GuanKa_Type type)
			{
				switch(state)
				{
				case GuanKa_State.GuanKa_State_Not_Reach:
					mIcon.spriteName = IconLight[(int)type];
					if(type == GuanKa_Type.GuanKa_Type_Boss)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxBoss[(int)state];
					}
					else if(type == GuanKa_Type.GuanKa_Type_Elite)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxElite[(int)state];
					}
					else
					{
						mBaoXiang.gameObject.SetActive(false);
						mBaoXiang.spriteName = "";
					}
					if(mGuanKaIndex % 2 == 0)
						mArrow.spriteName = ArrowUp;
					else
						mArrow.spriteName = ArrowDown;
					break;
				case GuanKa_State.GuanKa_State_Reach:
					mIcon.spriteName = IconLight[(int)type];
					if(type == GuanKa_Type.GuanKa_Type_Boss)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxBoss[(int)state];
					}
					else if(type == GuanKa_Type.GuanKa_Type_Elite)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxElite[(int)state];
					}
					else
					{
						mBaoXiang.gameObject.SetActive(false);
						mBaoXiang.spriteName = "";
					}
					if(mGuanKaIndex % 2 == 0)
						mArrow.spriteName = ArrowUp;
					else
						mArrow.spriteName = ArrowDown;
					break;
				case GuanKa_State.GuanKa_State_Pass:
					mIcon.spriteName = IconGrey[(int)type];
					if(type == GuanKa_Type.GuanKa_Type_Boss)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxBoss[(int)state];
					}
					else if(type == GuanKa_Type.GuanKa_Type_Elite)
					{
						mBaoXiang.gameObject.SetActive(true);
						mBaoXiang.spriteName = BoxElite[(int)state];
					}
					else
					{
						mBaoXiang.gameObject.SetActive(false);
						mBaoXiang.spriteName = "";
					}
					if(mGuanKaIndex % 2 == 0)
						mArrow.spriteName = ArrowUp_b;
					else
						mArrow.spriteName = ArrowDown_b;
					break;				
				}
			}
		}
		
		[System.Serializable]	
		public class BattleInfo
	 	{
			public GameObject InfoRoot;
			public UILabel ViewBattle;
			public UILabel Name;
			public UILabel Index;
			public UILabel Result;
			public UILabel Exp;
			public UILabel Rep;
			public UILabel Money;
			public UILabel ItemName;
		
			private int mIndex;
			public void Init(int index)
			{
				ViewBattle = InfoRoot.transform.Find("Label_0").GetComponent<UILabel>();
				Name = InfoRoot.transform.Find("Label_1").GetComponent<UILabel>();
				Index = InfoRoot.transform.Find("Label_2").GetComponent<UILabel>();
				Result = InfoRoot.transform.Find("Label_3").GetComponent<UILabel>();
				Exp = InfoRoot.transform.Find("Label_4").GetComponent<UILabel>();
				Rep = InfoRoot.transform.Find("Label_5").GetComponent<UILabel>();
			    Money = InfoRoot.transform.Find("Label_6").GetComponent<UILabel>();
				ItemName = InfoRoot.transform.Find("Label_7").GetComponent<UILabel>();
				
				ViewBattle.text = "";
				Name.text = "";
				Index.text = "";
				Result.text = "";
				Exp.text = "";
				Rep.text = "";
				Money.text = "";
				ItemName.text = "";
				
				if(ViewBattle != null)
				{
					UIEventListener ls = UIEventListener.Get(ViewBattle.gameObject);
					ls.onClick	= Click;
				}
				mIndex = index;
			}
		
			public  void Click (GameObject go)
			{
			}
		}
		public UIImageButton	 backUpBtn;
		public UIImageButton	 BtnStart;
		public UIImageButton	 BtnAuto;
		public UIImageButton    BtnBag;
		public UIImageButton    BtnBuZheng;
		
		public UILabel 			 GuanKaName;
		public UILabel 			 EnemyBattleValue;
		public UILabel 			 MyBattleValue;
		public UISprite          	PlayerIocn;
		public uint                  PlayerIocnPos;
		public Vector3         	PlayerStartPos = new Vector3(-255,168,0);
		public GameObject 		Sample_Up;
		public GameObject 		Sample_Down;
		public GameObject 		GuanKaInfoRoot;
		public GuanKaInfo[] 		GuanKaInfoList = new GuanKaInfo[MAX_GUAN_KA_NUM];
		public BattleInfo[] 			BattleInfoList = new BattleInfo[MAX_BATTLE_RESULT_NUM];
		
		public UISprite[]          	LifeSprite = new UISprite[3];
	
		public void Init()
		{
			for( int i = 0; i < MAX_BATTLE_RESULT_NUM; ++i )
			{
				BattleInfoList[i].Init(i);				
			}
			for( int i = 0; i < MAX_GUAN_KA_NUM; ++i )
			{
				GameObject instantObj; 
				if(i % 2 == 0)
					instantObj = XUtil.Instantiate(Sample_Up);
				else
					instantObj = XUtil.Instantiate(Sample_Down);
			
				instantObj.transform.localPosition = new Vector3(Sample_Up.transform.localPosition.x + ((i /2)* 106),0.0f,0.0f);
				GuanKaInfoList[i].Init((uint)i + 1,instantObj);
			}
			Sample_Up.SetActive(false);
			Sample_Down.SetActive(false);
		}
		public void AddBattleResult(uint level,bool bWin,uint exp, uint money,uint honour,string itemName)
		{
			for(int i = MAX_BATTLE_RESULT_NUM -1; i > 0 ; --i)
			{
				for ( int idx=0; idx < BattleInfoList[i].InfoRoot.transform.childCount; idx++ )
				{
					UILabel label = BattleInfoList[i].InfoRoot.transform.GetChild(idx).GetComponent<UILabel>();
					if ( label )
					{
						label.color = BattleInfoList[i-1].InfoRoot.transform.GetChild(idx).GetComponent<UILabel>().color;
						label.text = BattleInfoList[i-1].InfoRoot.transform.GetChild(idx).GetComponent<UILabel>().text;
					}
				}
			}
			
			for ( int idx=0; idx < BattleInfoList[0].InfoRoot.transform.childCount; idx++ )
			{
				UILabel label = BattleInfoList[0].InfoRoot.transform.GetChild(idx).GetComponent<UILabel>();
				if ( label )
				{
					TweenColorEx colorAni = label.gameObject.GetComponent<TweenColorEx>();
					if(colorAni)
					{
						if(bWin)
							colorAni.src = new Color(172f/255,164f/255,141f/255);
						else
							colorAni.src = new Color(1f,84f/255,0f);
						colorAni.Reset();
						colorAni.enabled	= true;
					}
				}
			}

			BattleInfoList[0].ViewBattle.text = XStringManager.SP.GetString(163);
			uint shanHeID = XShanHTManager.SP.GetShanHeID(level);
			XCfgShanHTBase cShanHe = XCfgShanHTBaseMgr.SP.GetConfig(shanHeID);
			
			uint index = (level % 20 != 0) ? level % 20 : 20;
			string strIdx = string.Format(XStringManager.SP.GetString(164),index);
			if(bWin)
			{
				BattleInfoList[0].Result.text = XStringManager.SP.GetString(165);
				BattleInfoList[0].Exp.text = XStringManager.SP.GetString(167) + exp.ToString();
				BattleInfoList[0].Money.text = XStringManager.SP.GetString(168) + money.ToString();
				BattleInfoList[0].Rep.text = XStringManager.SP.GetString(169) + honour.ToString();	
				BattleInfoList[0].ItemName.text = XStringManager.SP.GetString(170) + "  " +itemName;
				BattleInfoList[0].Name.text = cShanHe.Name;
				BattleInfoList[0].Index.text = strIdx;
			}
			else
			{
				BattleInfoList[0].Exp.text ="";
				BattleInfoList[0].Money.text ="";
				BattleInfoList[0].Rep.text = "";
				BattleInfoList[0].ItemName.text ="";
				BattleInfoList[0].Result.text =XStringManager.SP.GetString(166);
				BattleInfoList[0].Name.text =  cShanHe.Name;
				BattleInfoList[0].Index.text =  strIdx;
				//"[color=ff5400]" 
			}
		}
		
		public void updatePosByLevel(uint curLevel)
		{
			if(curLevel == 0)
				return;
			int targetIndex;
			if( curLevel % 20 == 0)
				targetIndex = 20;
			else
				targetIndex = (int)curLevel % 20;
			
			PlayerIocnPos = (uint)targetIndex;
			
			if(targetIndex > 10)
			{
				GuanKaInfoRoot.transform.localPosition = new Vector3(-530, 0 ,0);
			}
			else
			{
				GuanKaInfoRoot.transform.localPosition = new Vector3(0, 0 ,0);
			}
			
			if(curLevel % 2 != 0)
				PlayerIocn.gameObject.transform.localPosition =  new Vector3(PlayerStartPos.x + (targetIndex - 1) * 53,PlayerStartPos.y ,0);
			else
				PlayerIocn.gameObject.transform.localPosition =  new Vector3(PlayerStartPos.x + (targetIndex - 1) * 53,PlayerStartPos.y-90 ,0);
		}
		
		public void SetEnemyFightValue(uint id)
		{
			XCfgShanHTLevel curCfg = XCfgShanHTLevelMgr.SP.GetConfig(id);
			if(curCfg == null)
				return;
			EnemyBattleValue.text = curCfg.FightValue.ToString();
		}
		public void PlayerMoveTo(uint targetPos)
		{
			TweenPosition posEffect = UITweener.Begin<TweenPosition>(PlayerIocn.gameObject,1);
			if(posEffect != null)
			{
				uint targetIndex;
				if(targetPos % 20 == 0)
					targetIndex = 20;
				else
					targetIndex = targetPos % 20;
				if(targetPos % 2 != 0)
				{
					posEffect.from	= new Vector3(PlayerStartPos.x + (targetIndex - 2) * 53,PlayerStartPos.y - 90 ,0);
					posEffect.to	= new Vector3(PlayerStartPos.x + (targetIndex - 1) * 53,PlayerStartPos.y ,0);
				}
				else
				{
					posEffect.from	= new Vector3(PlayerStartPos.x + (targetIndex - 2) * 53,PlayerStartPos.y ,0);
					posEffect.to	= new Vector3(PlayerStartPos.x + (targetIndex - 1) * 53,PlayerStartPos.y - 90 ,0);
				}
				posEffect.enabled	= true;
				XCfgShanHTLevel LastCfg = XCfgShanHTLevelMgr.SP.GetConfig(targetPos - 1);
				XCfgShanHTLevel curCfg = XCfgShanHTLevelMgr.SP.GetConfig(targetPos);
				if(LastCfg != null && curCfg != null)
				{
					GuanKaInfoList[targetIndex - 2].SetState(GuanKa_State.GuanKa_State_Pass,(GuanKa_Type)LastCfg.Type);
					GuanKaInfoList[targetIndex - 1].SetState(GuanKa_State.GuanKa_State_Reach,(GuanKa_Type)curCfg.Type);
					GuanKaName.text = curCfg.Name;
				}
			}
			if(targetPos == 11)
			{
				TweenPosition posEffect2 = UITweener.Begin<TweenPosition>(GuanKaInfoRoot,2);
				if(posEffect2 != null)
				{
					Vector3 temp = GuanKaInfoRoot.transform.localPosition;
					posEffect2.from	= temp;
					posEffect2.to	= new Vector3(temp.x - 530,temp.y ,temp.z);
					posEffect2.enabled	= true;
				}	
			}
		}
	}
	public ChuangGuan   m_ChuangGuan = null;
	
	
	public override bool Init()
	{
        base.Init();
		
		UIEventListener ls1 = UIEventListener.Get(m_XuanGuan.StartBattle.gameObject);
		ls1.onClick	+= enterMountain;
		
		UIEventListener ls2 = UIEventListener.Get(m_ChuangGuan.backUpBtn.gameObject);
		ls2.onClick	+= backUpToShanHe1;
		
		UIEventListener ls3 = UIEventListener.Get(m_ChuangGuan.BtnStart.gameObject);
		ls3.onClick	+= ClickStartBattle;
		
		UIEventListener ls4 = UIEventListener.Get(m_ChuangGuan.BtnAuto.gameObject);
		ls4.onClick	+= ClickAutoBattle;
		
		UIEventListener ls5 = UIEventListener.Get(m_ChuangGuan.BtnBag.gameObject);
		ls5.onClick	+= ClickBag;
		
		UIEventListener ls6 = UIEventListener.Get(m_ChuangGuan.BtnBuZheng.gameObject);
		ls6.onClick	+= ClickBuZheng;
		
		m_XuanGuan.Init();
		m_ChuangGuan.Init();
		return true;
	}
	
	public override void Close()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eShanHT);
	}
	
	public void enterMountain(GameObject go)
	{
		if(SelectIndx == 0)
			return;
		uint leftTimes =  XShanHTManager.SP.LefTimes;
		if(leftTimes > 0)
		{
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStartStep);
			EnterChuangGuan((uint)SelectIndx,false);
		}
		else
		{
			uint leftBuyCnt = XShanHTManager.SP.LeftBuyCnt;
			if(leftBuyCnt > 0)
			{
				UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(SubmitForceEnter);
				string strTip = string.Format(XStringManager.SP.GetString(183),(4 - leftBuyCnt) * 20);
				XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, strTip);
			}
			else
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,12);
		}
	}
	
	public void EnterChuangGuan(uint ID, bool bForce)
	{
		CS_ShanHT_Enter.Builder msg = CS_ShanHT_Enter.CreateBuilder();
		msg.ShanHeID = ID;
		msg.IsForce = bForce;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_Enter, msg.Build());
		
		if(bForce)
			XShanHTManager.SP.LeftBuyCnt = XShanHTManager.SP.LeftBuyCnt - 1;
		else
			XShanHTManager.SP.LefTimes = XShanHTManager.SP.LefTimes - 1;

	    XCfgShanHTBase curConfig = XCfgShanHTBaseMgr.SP.GetConfig((uint)ID);
		uint level = curConfig.StartIndex;		
		XShanHTManager.SP.CurLevel = level;
		XShanHTManager.SP.LeftLife = 3;
		m_ChuangGuan.PlayerIocnPos = 1;
		UpdateShanHT();
		
		GameObject obj2 =  m_ChuangGuan.BtnStart.gameObject;
		Vector3 showPos2 = new Vector3(obj2.transform.localPosition.x + 50, 
			obj2.transform.localPosition.y + 131, 
			obj2.transform.localPosition.z);
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart, 
			2, showPos2, obj2);
	}
	public void SubmitForceEnter(GameObject go)
	{
		EnterChuangGuan((uint)SelectIndx,true);
	}
	public void backUpToShanHe1(GameObject go)
	{
		uint curLevel = XShanHTManager.SP.LeftLife;
		if(curLevel != 0)
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(SubmitExit);
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString(175));
		}
		else
		{
			SubmitExit(null);
		}
	}
	
	public void SubmitExit(GameObject go)
	{
		XShanHTManager.SP.CurLevel = 0;	
		CS_UInt.Builder	msg = CS_UInt.CreateBuilder();
		msg.SetData(0);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_Exit, msg.Build());
		ShowXuanGuan();
	}

	public void ClickStartBattle(GameObject go)
	{
		CheckStartBattle(false);
		
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart);
	}
	public void ClickAutoBattle(GameObject go)
	{
		if(XShanHTManager.SP.AutoFight)
		{
			CancelAutoBattle();
		}
		else
		{
			if(CheckStartBattle(true))
					AutoBattleTimerStart(null);
		}
	}
	private void SureToFight(GameObject go)
	{
		StartBattleToServer(false);
	}
	
	private void SureToAutoFight(GameObject go)
	{
		StartBattleToServer(true);
	}
	
	public void StartBattleToServer(bool auto)
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		if(auto)
		{
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_StartAutoBattle, msg.Build());
		}
		else
		{
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_StartBattle, msg.Build());
		}
	}
	

	public bool CheckStartBattle(bool auto)
	{
		//检测进入次数
		if(XShanHTManager.SP.LeftLife <= 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,11);
			return false;
		}
		
		//检测背包状态
		int nSpace = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		if(nSpace <= 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10);
			return false;
		}
		
		if(nSpace <= 3)
		{
			UIEventListener.VoidDelegate funcOK;
			if(auto)
				funcOK = new UIEventListener.VoidDelegate(AutoBattleTimerStart);
			else
				funcOK = new UIEventListener.VoidDelegate(SureToFight);
			
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString(172));
			return false;
		}
		//检测体力  目前客户端无法获得体力上限
		if(!auto)
			StartBattleToServer(false);
		return true;
	}
	public void OnPassThrough(uint ShanHeID)
	{	
		CancelAutoBattle();
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (SubmitExit);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString(178));
		
		XCfgShanHTBase cShanHe = XCfgShanHTBaseMgr.SP.GetConfig(ShanHeID);
		if(cShanHe == null)
			return;
		for( uint i = 0; i < MAX_GUAN_KA_NUM; ++i )
		{
			uint guanKaID = cShanHe.StartIndex + i;
			XCfgShanHTLevel curConfig = XCfgShanHTLevelMgr.SP.GetConfig(guanKaID);
			m_ChuangGuan.GuanKaInfoList[i].SetState(GuanKa_State.GuanKa_State_Pass,(GuanKa_Type)curConfig.Type);
		}
	}
	public void CancelAutoBattle()
	{
		XShanHTManager.SP.AutoFight = false;
		CancelInvoke();
		m_ChuangGuan.BtnAuto.transform.Find("Label").GetComponent<UILabel>().text = XStringManager.SP.GetString(161);
		m_ChuangGuan.BtnAuto.enabled = true;
	}
	public void AutoBattleTimerStart(GameObject go)
	{
		XShanHTManager.SP.AutoFight = true;
		InvokeRepeating("StartAutoBattle", 2,2);
		m_ChuangGuan.BtnAuto.transform.Find("Label").GetComponent<UILabel>().text = XStringManager.SP.GetString(162);
		m_ChuangGuan.BtnAuto.enabled = false;
	}
	
	public void StartAutoBattle()
	{
		//check Auto
		//检测背包状态
		int nSpace = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		if(nSpace <= 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10);
			//XEventManager.SP.SendEvent(EEvent.MessageBox, null, null, XStringManager.SP.GetString(176));
			CancelAutoBattle();
			return;
		}
		
		StartBattleToServer(true);
	}
	public void ClickBag(GameObject go)
	{
		//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, "aaaa");
		TweenColorEx posEffect = go.transform.Find("Label").GetComponent<TweenColorEx>();
		if(posEffect != null)
		{
			if(posEffect.enabled == true)
				posEffect.enabled = false;
			else
			{
				posEffect.Reset();
				posEffect.enabled = true;
			}
		}
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eBagWindow);
	}
	
	public void ClickBuZheng(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFormation);
	}
	
	public void selectIndex(int index)
	{
		SelectIndx = index+1;
		for( int i = 0; i < MAX_SHAN_HE_NUM; ++i )
		{
			if(!m_XuanGuan.SelBtnList[i].Btn.isEnabled)
				break;
			if(i == index)
			{
				m_XuanGuan.SelBtnList[i].Btn.normalSprite = selectSpriteName;
				m_XuanGuan.SelBtnList[i].Btn.hoverSprite = selectSpriteName;
				m_XuanGuan.SelBtnList[i].Btn.pressedSprite = selectSpriteName;
			}
			else
			{
				m_XuanGuan.SelBtnList[i].Btn.normalSprite = normalSpriteName;
				m_XuanGuan.SelBtnList[i].Btn.hoverSprite = normalSpriteName;
				m_XuanGuan.SelBtnList[i].Btn.pressedSprite = normalSpriteName;
			}
			m_XuanGuan.SelBtnList[i].Btn.UpdateImage();
		}
		ShowRankInfo(SelectIndx);
	}
	
    public void ShanHeQueryRank()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_QueryRank, msg.Build());
	}
	
	public void UpdateRank(SC_ShanHT_Rank msg)
	{
		for(int i = 0; i < 5 ; i++)
		{
			if(i < msg.RankListCount)
			{
				SC_ShanHT_Rank.Types.Int_RankInfo res = msg.GetRankList(i);
				m_XuanGuan.RankInfoPanel.transform.GetChild(i).Find("Label_Name").GetComponent<UILabel>().text = res.Name;
				m_XuanGuan.RankInfoPanel.transform.GetChild(i).Find("Label_Level").GetComponent<UILabel>().text = res.MaxLevel.ToString() + XStringManager.SP.GetString(181);
			}
			else
			{
				m_XuanGuan.RankInfoPanel.transform.GetChild(i).Find("Label_Name").GetComponent<UILabel>().text = "";
				m_XuanGuan.RankInfoPanel.transform.GetChild(i).Find("Label_Level").GetComponent<UILabel>().text  = "";
			}
		}
		ShowRankInfo(SelectIndx);
	}
	public void ShowRankInfo(int index)
	{
		if(XShanHTManager.SP.mRnakInfo == null)
			return;
		if(XShanHTManager.SP.mRnakInfo.InfoListCount < index)
		{
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(0).Find("Label_Name").GetComponent<UILabel>().text = "";
			m_XuanGuan.ShanHeInfoPanel.transform.Find("1").Find("Label_Info").GetComponent<UILabel>().text = "";
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(1).Find("Label_Name").GetComponent<UILabel>().text = "";
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(1).Find("Label_Info").GetComponent<UILabel>().text = "";
		}
		else
		{
			SC_ShanHT_Rank.Types.Int_ShanHeInfo res = XShanHTManager.SP.mRnakInfo.GetInfoList(index - 1);
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(0).Find("Label_Name").GetComponent<UILabel>().text = res.MinTimeName;
			uint dayNum = res.DayNum;
			if(dayNum == 0)
				dayNum = 1;
			m_XuanGuan.ShanHeInfoPanel.transform.Find("1").Find("Label_Info").GetComponent<UILabel>().text = string.Format(XStringManager.SP.GetString(191),dayNum); 
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(1).Find("Label_Name").GetComponent<UILabel>().text = res.MinLevelName;
			m_XuanGuan.ShanHeInfoPanel.transform.GetChild(1).Find("Label_Info").GetComponent<UILabel>().text = res.PassLevel.ToString();
		}
	}
	public void updateXuanGuan()
	{
		if(m_XuanGuan.MyInfoPanel)
		{
			uint maxLevel = XShanHTManager.SP.getMaxLevel();
			int battleValue = XUIRoleInformation.CalcBattleValue(XLogicWorld.SP.MainPlayer);

			m_XuanGuan.MyInfoPanel.transform.Find("Label_Fighting").GetComponent<UILabel>().text = battleValue.ToString();
			m_XuanGuan.MyInfoPanel.transform.Find("Label_MaxLevel").GetComponent<UILabel>().text = string.Format(XStringManager.SP.GetString(164),maxLevel);

			uint leftTimes =  XShanHTManager.SP.LefTimes ;
			string enterStr,numStr,btnStr;
			if(leftTimes > 0)
			{
				numStr = leftTimes.ToString();
				enterStr =XStringManager.SP.GetString(184);
				btnStr =XStringManager.SP.GetString(187);
				m_XuanGuan.StartBattle.isEnabled = true;
			}
			else
			{
				uint leftBuyCnt =  XShanHTManager.SP.LeftBuyCnt;
				if(leftBuyCnt > 0)
				{
					numStr = leftBuyCnt.ToString();
					enterStr =XStringManager.SP.GetString(185);
					btnStr =XStringManager.SP.GetString(188);
					m_XuanGuan.StartBattle.isEnabled = true;
				}
				else
				{
					numStr = "";
					enterStr =XStringManager.SP.GetString(186);
					btnStr =XStringManager.SP.GetString(188);
					m_XuanGuan.StartBattle.isEnabled = false;
				}
			}
			m_XuanGuan.MyInfoPanel.transform.Find("Label_LeftTimes").GetComponent<UILabel>().text = numStr;
			m_XuanGuan.MyInfoPanel.transform.Find("Label_EnterState").GetComponent<UILabel>().text = enterStr;
			m_XuanGuan.StartBattle.transform.Find("Label").GetComponent<UILabel>().text = btnStr;
			
			for( int i = 0; i < MAX_SHAN_HE_NUM; ++i )
			{
				XCfgShanHTBase curConfig = XCfgShanHTBaseMgr.SP.GetConfig((uint)i + 1);
				if(curConfig.StartIndex-1 <= maxLevel)
				{
					m_XuanGuan.SelBtnList[i].Btn.isEnabled =  true;
					m_XuanGuan.SelBtnList[i].Btn.transform.Find("Back1").GetComponent<UISprite>().spriteName = curConfig.TotemLight.ToString();
				}
				else
				{
					m_XuanGuan.SelBtnList[i].Btn.isEnabled =  false;
					m_XuanGuan.SelBtnList[i].Btn.transform.Find("Back1").GetComponent<UISprite>().spriteName = curConfig.TotemDark.ToString();
				}
				m_XuanGuan.SelBtnList[i].Btn.transform.Find("Back2").GetComponent<UISprite>().spriteName = curConfig.NameSprite.ToString();
				m_XuanGuan.SelBtnList[i].Btn.UpdateImage();
			}	
			ShanHeQueryRank();
		}		
	}
	public void UpdateShanHT()
	{
		uint curLevel = XShanHTManager.SP.CurLevel;
		if(curLevel == 0)
			ShowXuanGuan();
		else
		{
			ShowChuangGuan();
		}
	}
	public override void Show()
	{
		base.Show();
		UpdateShanHT();
		
		GameObject obj =  m_XuanGuan.StartBattle.gameObject;
		Vector3 showPos = new Vector3(m_XuanGuan.StartBattle.transform.localPosition.x, 
			m_XuanGuan.StartBattle.transform.localPosition.y - 36, 
			m_XuanGuan.StartBattle.transform.localPosition.z);
		XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStartStep, 
			(int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart,
			1, showPos, obj);
		
		if ( m_ChuangGuan.BtnStart.transform.parent.parent.gameObject.activeSelf )
		{
			GameObject obj2 =  m_ChuangGuan.BtnStart.gameObject;
			Vector3 showPos2 = new Vector3(obj2.transform.localPosition.x + 50, 
				obj2.transform.localPosition.y + 131, 
				obj2.transform.localPosition.z);
			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart, 
				2, showPos2, obj2);
		}
	}
	

	public void ShowChuangGuan()
	{
		m_ShanHeRoot1.SetActive(false);
		m_ShanHeRoot2.SetActive(true);
		
		uint curLevel = XShanHTManager.SP.CurLevel;		
		if(curLevel == 0)
			return;
		uint shanHeID = XShanHTManager.SP.GetShanHeID(curLevel);
		XCfgShanHTBase cShanHe = XCfgShanHTBaseMgr.SP.GetConfig(shanHeID);
		for( uint i = 0; i < MAX_GUAN_KA_NUM; ++i )
		{
			uint guanKaID = cShanHe.StartIndex + i;
			XCfgShanHTLevel curConfig = XCfgShanHTLevelMgr.SP.GetConfig(guanKaID);
			if(curLevel < guanKaID)
			{
				m_ChuangGuan.GuanKaInfoList[i].SetState(GuanKa_State.GuanKa_State_Not_Reach,(GuanKa_Type)curConfig.Type);
			}
			else if(curLevel == guanKaID)
			{
				m_ChuangGuan.GuanKaInfoList[i].SetState(GuanKa_State.GuanKa_State_Reach,(GuanKa_Type)curConfig.Type);
				m_ChuangGuan.GuanKaName.text = curConfig.Name;
			}
			else
			{
				m_ChuangGuan.GuanKaInfoList[i].SetState(GuanKa_State.GuanKa_State_Pass,(GuanKa_Type)curConfig.Type);
			}
		}
		
		uint lifeNum = XShanHTManager.SP.LeftLife;
		for(uint i = 0; i < 3; ++i)
		{
			if( lifeNum > i )
				m_ChuangGuan.LifeSprite[i].spriteName = "11000750";
			else
				m_ChuangGuan.LifeSprite[i].spriteName = "11000751";
		}

		int battleValue = XUIRoleInformation.CalcBattleValue(XLogicWorld.SP.MainPlayer);
		m_ChuangGuan.MyBattleValue.text = battleValue.ToString();
		m_ChuangGuan.updatePosByLevel(curLevel);
		m_ChuangGuan.SetEnemyFightValue(curLevel);
	}

	public void ShowXuanGuan()
	{
		m_ShanHeRoot1.SetActive(true);
		m_ShanHeRoot2.SetActive(false);
		CancelAutoBattle();
		selectIndex(0);
		updateXuanGuan();	
	}
	public void AddBattleInfo(uint level,bool bWin,uint exp, uint money,uint honour,string itemName)
	{
		m_ChuangGuan.AddBattleResult(level, bWin, exp,  money, honour, itemName);
	}
}