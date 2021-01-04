 using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

class XUTShanHT : XUICtrlTemplate<XShanHT>
{
	private int GuanKaID;
	private SC_BattleResult LastResult;
	public XUTShanHT()
	{
		XEventManager.SP.AddHandler(selectMountain, EEvent.Click_SH_Btn);
		XEventManager.SP.AddHandler(UpdateBattleInfo,EEvent.ShanHe_Update_BattleInfo);
		
		XEventManager.SP.AddHandler(InitShanHeInfo,EEvent.ShanHe_Init_Info);
		
		XEventManager.SP.AddHandler(Update_ShanHT_Rank,EEvent.ShanHe_Rank);
		
		GuanKaID = 1;
	}

	//选中山脉
    public void selectMountain(EEvent evt, params object[] args)
	{
		int index = (int)args[0];
		LogicUI.selectIndex(index);
		int a = 1;
	}
	//初始化界面
    public void InitShanHeInfo(EEvent evt, params object[] args)
	{
		int a = 1;
		//LogicUI.UpdateShanHT();
	}
    public void ShanHeStartBattle(EEvent evt, params object[] args)
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_StartBattle, msg.Build());
	}
	

	
	public override void OnHide()
	{
		base.OnHide();
	}
	
	public void AddLastBattleInfo()
	{
		bool bWin = false;
		if(LastResult.Winner == EBattleGroupType.eBattleGroup_Right)
			bWin = true;
		uint lastLevel = XShanHTManager.SP.CurLevel;
		if(bWin)
		{
			OnBattleWin();
		}
		else
		{
			OnBattleLost();
		}
		if(XShanHTManager.SP.AutoFight == false)
			Show();
		else
			LogicUI.UpdateShanHT();
		if(bWin)
		{
			XShanHTManager.SP.BattleWin();
			if(lastLevel % 20 != 0)
			{
				LogicUI.m_ChuangGuan.PlayerMoveTo(lastLevel+1);
				LogicUI.m_ChuangGuan.SetEnemyFightValue(lastLevel+1);
			}
			else
			{
				uint shanHeID = XShanHTManager.SP.GetShanHeID(lastLevel);
				LogicUI.OnPassThrough(shanHeID); 
			}
		}
	}
	
    public void UpdateBattleInfo(EEvent evt, params object[] args)
	{
		LastResult = (SC_BattleResult)args[0];
		if(XShanHTManager.SP.AutoFight)
		{
			AddLastBattleInfo();
		}
	}
	public void On_Exit_Fight(GameObject go)
	{
		LogicUI.SubmitExit(null);
	}
	public void OnBattleWin()
	{
		string itemName = "";
		uint lastLevel = XShanHTManager.SP.CurLevel;
		uint exp = 0;
		uint money = 0;
		uint honour = 0;

		exp = LastResult.Bonus.BonusExp;
		money = LastResult.Bonus.GameMoney;
		honour = LastResult.Bonus.HonourValue;

		if(LastResult.Bonus.ItemListCount > 0)
		{
			PB_ItemInfo info = LastResult.Bonus.GetItemList(0);
			uint itemId = (uint)info.ItemId;
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig( itemId );
			if(cfgItem != null)
				itemName = cfgItem.Name;
		}
		LogicUI.AddBattleInfo(lastLevel,true,exp,money,honour,itemName);

	}
	public void OnBattleLost()
	{
		uint curLevel = XShanHTManager.SP.CurLevel;
		XShanHTManager.SP.LeftLife--;
		if(XShanHTManager.SP.LeftLife <= 0)
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (On_Exit_Fight);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString (179));
		}
		if(XShanHTManager.SP.AutoFight)
			LogicUI.CancelAutoBattle();
		LogicUI.AddBattleInfo(curLevel,false,0,0,0,"");
	}
	
	public void Update_ShanHT_Rank(EEvent evt, params object[] args)
	{
		SC_ShanHT_Rank msg = (SC_ShanHT_Rank)args[0];
		LogicUI.UpdateRank(msg);
		return;
	}
}


