 using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

class XUTZhanYaoLu : XUICtrlTemplate<XZhanYaoLu>
{
	private bool isInit = false;
	public XUTZhanYaoLu()
	{
		XEventManager.SP.AddHandler(Update_Base_Info,EEvent.ZYL_UpdateInfo);
		XEventManager.SP.AddHandler(Update_CD_Time,EEvent.ZYL_UpdateTime);
		
		XEventManager.SP.AddHandler(BattleEnd,EEvent.ZYL_BattleEnd);	
	}

	
    public void BattleEnd(EEvent evt, params object[] args)
	{
		bool isWin = (bool)args[0];
		XZhanYaoLuManager.SP.LeftFightCnt -= 1;
		XZhanYaoLuManager.SP.LeftCDTime = (int)XZhanYaoLu.CD_TIME;
		XZhanYaoLuManager.SP. IsAllreadyKilled= isWin;
		if(isWin)
		{
			XZhanYaoLuManager.SP.ComboCnt += 1;		
		}
		else
		{
			XZhanYaoLuManager.SP.ComboCnt  = 0;
		}
		//XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ShanHe_StartBattle, msg.Build());
	}
	

	
	public override void OnHide()
	{
		base.OnHide();
	}
	public override void OnShow()
	{
		//test
		//XZhanYaoLuManager.SP.GuanKaID = 5;
		//没有初始化过 就像服务器请求选关
		
		if(LogicUI == null)
			return;
		LogicUI.UpdateInfoUI();
		
		if(isInit == false)
		{
			XZhanYaoLuManager.SP.ApplyMonsterInfo();
		}
	}
	public void OnBattleWin()
	{
	}
	public void OnBattleLost()
	{
	}
	
	
    public void Update_Base_Info(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return;
		LogicUI.UpdateInfoUI();
	}
	
	public void Update_CD_Time(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return;
		LogicUI.Update_Time();
	}
}


