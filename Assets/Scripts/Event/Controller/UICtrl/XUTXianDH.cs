using UnityEngine;
using System;
using System.Timers;
using XGame.Client.Packets;

class XUTXianDH : XUICtrlTemplate<XXianDH>
{
	private static uint MIN_PK_LEVEL = 32;
	private static uint MAX_BUY_CHALLENGE_COUNT = 10;
	
	private uint mCurRank;
	private uint mContinueCount;
	private uint mChallengeCount;
	private uint mLeftChallengeCount;
	private uint mBestRank;
	private uint mMaxContinueWin;
	private UInt64	mLeftTime;
	private Timer mTimer;
	private uint mCDTime;
	private Timer mCDTimer;
	
	public uint LeftChallengeCount
    {
        get
        {
            return mChallengeCount;
        }
        set 
		{ 
			mChallengeCount = value;
			
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_XianDaoHui, (int)mChallengeCount, 0);
		}
    }
	
	
	public XUTXianDH()
	{
		RegEventAgent_CheckCreated(EEvent.PKData_Other,InitDataHandler);
		RegEventAgent_CheckCreated(EEvent.PKData_SingleObject,InitPKObjectHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_PKPlayer,CheckPKPlayer);
		RegEventAgent_CheckCreated(EEvent.PVPResult_Award,CheckPKPlayer);
		RegEventAgent_CheckCreated(EEvent.PKData_Record,InitRecordHandler);
		RegEventAgent_CheckCreated(EEvent.PKData_Notice,PKNoticeHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_SortRecord,PKSortRecordHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_ClearSortRecord,PKClearSortRecordHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_BuyChallenge,PKBuyChallengeHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_AddSpeed,PKAddSpeedHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_Failed,PKFailedHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_UI_BuyChallenge,UI_BuyChanllegeHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_UI_AddSpeed,UI_AddSpeedHandle);
		RegEventAgent_CheckCreated(EEvent.UI_ShowChallengeCount,UI_ShowChallengeCountHandle);
		RegEventAgent_CheckCreated(EEvent.PKData_Tip,UI_TipDataHandle);	
		RegEventAgent_CheckCreated(EEvent.PKData_UI_AddSpeed_Tip,UI_TipAddSpeedHandle);	
		RegEventAgent_CheckCreated(EEvent.PKData_PKDataGetFinish,PKDataGetFinish);
		
	}
	
	private uint GetCostRealMoney()
	{
		if(mCDTime == 0 || mCDTime >= 10 * 60)
			return 0;
		
		uint leftMin = mCDTime / 60;
		uint leftSecond = mCDTime % 60;
		
		uint NeedMoney = 10;
		if(leftSecond > 0)
			NeedMoney	= leftMin + 1;
		
		return NeedMoney;
	}
	
	public override bool IsCanShow()
	{
//		if(XLogicWorld.SP.MainPlayer.Level	< MIN_PK_LEVEL)
//		{
//			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,219);
//			return false;
//		}
		
		return true;
	}
	public override void OnShow()
	{
		base.OnShow();
		
		LogicUI.SpriteGroup.CurrentSelect	= 0;
		CS_Empty.Builder	builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_GetPKObjectList, builder.Build());
	}
	
	public override void OnHide()
	{
		base.OnHide();
		CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
		LogicUI.ClearModel();
	}
	
	public void InitDataHandler(EEvent evt, params object[] args)
	{
		mCurRank 			= (uint)args[0];
		mContinueCount 		= (uint)args[1];
		mLeftChallengeCount = (uint)args[2];
		mBestRank			= (uint)args[3];
		mMaxContinueWin		= (uint)args[4];
		mLeftTime			= (UInt64)args[5];
		mCDTime				= 10 * 60 - (uint)args[6];
		LeftChallengeCount		= (uint)args[7];
		
		if(mLeftChallengeCount >= MAX_BUY_CHALLENGE_COUNT)
			LogicUI.AddCount.isEnabled	= false;
		else
			LogicUI.AddCount.isEnabled	= true;
		LogicUI.AddCount.UpdateImage();
		
		
		if(mCDTime == 0)
			LogicUI.AddSpeed.isEnabled	= false;
		else
			LogicUI.AddSpeed.isEnabled	= true;	
		LogicUI.AddSpeed.UpdateImage();
		
		mLeftTime			+= 60;
		mCDTime				+= 1;
		
		XPKAward Cfg = null;
		foreach(XPKAward temp in XPKAwardMgr.SP.ItemTable.Values)
		{
			if(temp.RankID <= mCurRank && temp.RankIDLimit >= mCurRank)
			{
				Cfg = temp;
				break;
			}
		}
		
		if(LogicUI == null || Cfg == null)
			return ;
		
		LogicUI.AwardGameMoney.text	= Cfg.GameMoney.ToString();
		LogicUI.AwardHonour.text	= Cfg.Honour.ToString();
		XTitle cfgTitle = XTitleMgr.SP.GetConfig(Cfg.TitleID);
		if(cfgTitle != null)
			LogicUI.AwardTitle.text		= cfgTitle.TitleName;
		XCfgItem tempItem = XCfgItemMgr.SP.GetConfig(Cfg.ItemID);
		if(tempItem != null)
		{
			LogicUI.AwardItemName.text	= XGameColorDefine.Quality_Color[tempItem.QualityLevel] + tempItem.Name;
		}
		
		LogicUI.CurRank.text			= mCurRank.ToString();
		LogicUI.ContinueCount.text		= mContinueCount.ToString();
		LogicUI.CurChallengeCount.text	= mLeftChallengeCount.ToString();
		GetLeftTime(null,null);
		GetCDTime(null,null);
		
		LogicUI.Info_Rank.text			= mCurRank.ToString();
		LogicUI.Info_BestRank.text		= mBestRank.ToString();
		LogicUI.Info_CurContinueWin.text= mContinueCount.ToString();
		LogicUI.Info_MaxContinueWin.text= mMaxContinueWin.ToString();
		LogicUI.Info_BattleValue.text	= XUIRoleInformation.CalcBattleValue(XLogicWorld.SP.MainPlayer).ToString();
		
		if(mTimer == null)
		{
			mTimer	= new Timer(1000 * 60);
			mTimer.Elapsed += new ElapsedEventHandler(GetLeftTime);
			mTimer.Start();	
		}
		
		if(mCDTimer == null)
		{
			mCDTimer	= new Timer(1000);
			mCDTimer.Elapsed += new ElapsedEventHandler(GetCDTime);
			mCDTimer.Start();
		}
	}
	
	public void InitRecordHandler(EEvent evt, params object[] args)
	{
		int index 			= (int)args[0];
		string Name 		= (string)args[1];
		UInt64 GUID			= (UInt64)args[2];
		PKRecordType type 	= (PKRecordType)args[3];
		uint Rank			= (uint)args[4];
		
		if(LogicUI == null)
			return ;
		
		switch(type)
		{
		case PKRecordType.PKRecordType_WinAndUpdate:
			{				
				string content = string.Format(XStringManager.SP.GetString(207),Rank);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		case PKRecordType.PKRecordType_WinAndNoChange:
			{
				string content = XStringManager.SP.GetString(208);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		case PKRecordType.PKRecordType_FailAndNoChange:
			{
				string content = XStringManager.SP.GetString(223);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		case PKRecordType.PKRecordType_BeAttack_FailAndUpdate:
			{
				string content = string.Format(XStringManager.SP.GetString(209),Name,GUID,Name,Rank);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		case PKRecordType.PKRecordType_BeAttack_FailAndNoChange:
			{
				string content = string.Format(XStringManager.SP.GetString(210),Name,GUID,Name);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		case PKRecordType.PKRecordType_BeAttack_Win:
			{
				string content = string.Format(XStringManager.SP.GetString(211),Name,GUID,Name);
				LogicUI.RecordList[index].Record.text		= content;
				LogicUI.RecordList[index].LookBattle.text	= XStringManager.SP.GetString(218);
			}
			break;
		default:
			break;
		}
		
		NGUITools.AddWidgetCollider(LogicUI.RecordList[index].Record.gameObject);
		NGUITools.AddWidgetCollider(LogicUI.RecordList[index].LookBattle.gameObject);
	}
	
	public void PKNoticeHandle(EEvent evt, params object[] args)
	{
		PKNoticeType type = (PKNoticeType)args[0];
		string name = (string)args[1];
		UInt64 guid = (UInt64)args[2];
		string BeAttackName = (string)args[3];
		UInt64 BeAttackGUID = (UInt64)args[4];
		
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		switch(type)
		{
		case PKNoticeType.PKNoticeType_FirstWin:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(212),name,guid,name);
			break;
		case PKNoticeType.PKNoticeType_6To10:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(213),name,guid,name,mContinueCount + 1);
			break;
		case PKNoticeType.PKNoticeType_10To20:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(214),name,guid,name,mContinueCount + 1);
			break;
		case PKNoticeType.PKNoticeType_B20:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(215),name,guid,name,mContinueCount + 1);
			break;
		case PKNoticeType.PKNoticeType_King:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(217),name,guid,name,BeAttackName,BeAttackGUID,BeAttackName);
			break;
		case PKNoticeType.PKNoticeType_Stop:
			LogicUI.Notice.text	= string.Format(XStringManager.SP.GetString(216),name,guid,name,BeAttackName,BeAttackGUID,BeAttackName,mContinueCount + 1);
			break;
		}
		
		NGUITools.AddWidgetCollider(LogicUI.Notice.gameObject);
	}
	
	public void PKDataGetFinish(EEvent evt, params object[] args)
	{
		LogicUI.ShowNewPlayerGuide();
	}
	public void PKClearSortRecordHandle(EEvent evt, params object[] args)
	{
		LogicUI.ClearSortRecord();
	}
	
	public void PKSortRecordHandle(EEvent evt, params object[] args)
	{
		int index	= (int)args[0];
		UInt32 Rank = (UInt32)args[1];
		string Name	= (string)args[2];
		UInt32 Level= (UInt32)args[3];
		UInt32 BattleValue = (UInt32)args[4];
		PKSortType type = (PKSortType)args[5];
		
		LogicUI.AddSortRecord(index,Rank,Name,Level,BattleValue,type);
	}
	
	public void PKBuyChallengeHandle(EEvent evt, params object[] args)
	{
		LeftChallengeCount = (UInt32)args[0];
		mLeftChallengeCount = (UInt32)args[1];
		
		LogicUI.CurChallengeCount.text	= mLeftChallengeCount.ToString();
		
		if(LeftChallengeCount >= MAX_BUY_CHALLENGE_COUNT)
			LogicUI.AddCount.isEnabled = false;
		else
			LogicUI.AddCount.isEnabled = true;
		
		LogicUI.AddCount.UpdateImage();
	}	
	
	public void PKAddSpeedHandle(EEvent evt, params object[] args)
	{
		mCDTime	= 0;
		mCDTimer.Stop();
		LogicUI.CDTime.text	= "";
		LogicUI.AddSpeed.isEnabled = false;
		LogicUI.AddSpeed.UpdateImage();
		
	}
	
	public void PKFailedHandle(EEvent evt, params object[] args)
	{
		mLeftChallengeCount = (uint)args[0];
		mCDTime				= 10 * 60 - (uint)args[1];
		mCDTime	+= 1;
		
		if(LogicUI == null)
			return ;
		
		if(mCDTimer != null)
			mCDTimer.Stop();
		
		LogicUI.CurChallengeCount.text	= mLeftChallengeCount.ToString();
		GetCDTime(null,null);
		if(mCDTimer != null)
			mCDTimer.Start();
	}
	
	public void UI_BuyChanllegeHandle(EEvent evt, params object[] args)
	{
		TipBuyChanllege();
	}
	
	public void UI_AddSpeedHandle(EEvent evt, params object[] args)
	{
		TipAddSpeed();
	}
	
	public void UI_TipDataHandle(EEvent evt, params object[] args)
	{
		uint CurRank = (uint)args[0];
		
		XPKAward Cfg = null;
		foreach(XPKAward temp in XPKAwardMgr.SP.ItemTable.Values)
		{
			if(temp.RankID <= CurRank && temp.RankIDLimit >= CurRank)
			{
				Cfg = temp;
				break;
			}
		}
		
		if(LogicUI == null || Cfg == null)
			return ;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(Cfg.ItemID);
		if ( null == cfgItem )
			return;
		
		string ItemName = XGameColorDefine.Quality_Color[cfgItem.QualityLevel] + cfgItem.Name;
		XTitle cfgTitle = XTitleMgr.SP.GetConfig(Cfg.TitleID);
		string content = string.Format(XStringManager.SP.GetString(710),CurRank,Cfg.GameMoney,Cfg.Honour,ItemName,Cfg.ItemCount,cfgTitle.TitleName);
		XEventManager.SP.SendEvent(EEvent.ToolTip_C,content);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipC);
	}
	
	public void UI_TipAddSpeedHandle(EEvent evt, params object[] args)
	{
		if(mCDTime == 0)
			return ;
		
		uint cost = GetCostRealMoney();
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(AddSpeed);
		string content = string.Format(XStringManager.SP.GetString(711),cost);
		XEventManager.SP.SendEvent(EEvent.ToolTip_C,content);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipC);
	}
	
	public void UI_ShowChallengeCountHandle(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipC);
		
		uint cost = 0;
		XCfgBuyChallenge cfgChallenge = XCfgBuyChallengeMgr.SP.GetConfig(LeftChallengeCount+1);
		if(cfgChallenge != null)
			cost	= cfgChallenge.GameMoney;
		
		string str = string.Format(XStringManager.SP.GetString(251),cost);
		XEventManager.SP.SendEvent(EEvent.ToolTip_C,str);
	}
	
	public void InitPKObjectHandle(EEvent evt, params object[] args)
	{
		int index 				= (int)args[0];
		string Name 			= (string)args[1];
		uint Level 				= (uint)args[2];
		uint ObjCurRank			= (uint)args[3];
		uint ClothesModelID 	= (uint)args[4];
		uint WeaponModelID		= (uint)args[5];
		
		LogicUI.AddPKObject(index,Name,Level,ObjCurRank,ClothesModelID,WeaponModelID);
	}
	
	public void CheckPKPlayer(EEvent evt, params object[] args)
	{
		if(mLeftChallengeCount <= 0)
		{
			TipBuyChanllege();
			return ;
		}
		
		if(mCDTime > 0)
		{
			TipAddSpeed();
			return ;
		}
		
		uint PKRank = (uint)args[0];		
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(PKRank);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PKPlayer, builder.Build());
	}
	
	private void GetLeftTime(object sender, ElapsedEventArgs e)
	{
		if(mLeftTime <= 0)
		{
			LogicUI.LeftTime.text	= "";
			if(mTimer != null)
				mTimer.Stop();
			return ;
		}
		
		mLeftTime	-= 60;
		
		if(mLeftTime <= 0)
		{
			LogicUI.LeftTime.text	= "";
			if(mTimer != null)
				mTimer.Stop();
			return ;
		}
		
		UInt64 Day 	= mLeftTime / (3600 * 24);
		UInt64 Hour = (mLeftTime - (Day * 3600 * 24))/3600;
		UInt64 Min 	= (mLeftTime - (Day * 3600 * 24) - (Hour * 3600)) / 60;
		string temp = "";
		if(Day > 0)
		{
			temp = string.Format(XStringManager.SP.GetString(220),Day,Hour,Min);
		}
		else if(Hour > 0)
		{
			temp =  string.Format(XStringManager.SP.GetString(221),Hour,Min);
		}
		else if(Min > 0)
		{
			temp =  string.Format(XStringManager.SP.GetString(222),Min);
		}
		
		LogicUI.LeftTime.text			= temp;
		
		return ;		
	}
	
	private void GetCDTime(object sender, ElapsedEventArgs e)
	{
		if(mCDTime <= 0)
		{
			if(mCDTimer != null)
				mCDTimer.Stop();
			LogicUI.CDTime.text	= "";
			return ;
		}
		
		mCDTime--;
		
		if(mCDTime <= 0)
		{
			if(mCDTimer != null)
				mCDTimer.Stop();
			LogicUI.CDTime.text	= "";
			return ;
		}
		
		uint Min 	= mCDTime / 60;
		uint second = mCDTime % 60;
		
		string temp = "";
		temp =  string.Format("{0}:{1}",Min,second);
		
		LogicUI.CDTime.text			= temp;
		
		return ;		
	}
	
	public  void BuyChanllegeCount(GameObject go)
	{
		ConfirmBuyChanllege(go);
	}
	
	public  void AddSpeed(GameObject go)
	{
		ConfirmAddSpeed(go);
	}
	
	public void ConfirmBuyChanllege(GameObject go)
	{
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PK_Buy_Challenge, builder.Build());	
	}
	
	public void ConfirmAddSpeed(GameObject go)
	{
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PK_Add_Speed, builder.Build());	
	}
	
	private void TipBuyChanllege()
	{
		uint cost = 0;
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(BuyChanllegeCount);
		XCfgBuyChallenge cfgChallenge = XCfgBuyChallengeMgr.SP.GetConfig(LeftChallengeCount+1);
		if(cfgChallenge != null)
		{
			cost = cfgChallenge.GameMoney;
			if(cost > XLogicWorld.SP.MainPlayer.RealMoney)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,34);
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
				return ;
			}
			string content = string.Format(XStringManager.SP.GetString(224),cost);
			XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
		}
	}
	
	private void TipAddSpeed()
	{
		if(mCDTime == 0)
			return ;
		
		uint cost = GetCostRealMoney();
		if(cost > XLogicWorld.SP.MainPlayer.RealMoney)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,34);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
			return ;
		}
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(AddSpeed);
		string content = string.Format(XStringManager.SP.GetString(225),cost);
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
	}

}
