
using System;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;
using UnityEngine;


public class XVipManager : XSingleton<XVipManager>
{
	
	/*public XCfgVipBase  GetXCfgBaseAward(uint uVipLvl)
	{
		return XCfgVipBaseMgr.SP.GetConfig(uVipLvl);
	}*/

	/*public XCfgVipAward  GetXCfgVipAward(uint uVipLvl)
	{
		return XCfgVipAwardMgr.SP.GetConfig(uVipLvl);
	}*/
	
	
	public uint GetVipAttri(EVipConst eType)
	{	
		return GetVipAttri(eType, XLogicWorld.SP.MainPlayer.VIPLevel);
	}
	
	
	public uint GetVipAttri(EVipConst eType, uint vipLvl)
	{
		XCfgVipBase xCfgVipBase = XCfgVipBaseMgr.SP.GetConfig(vipLvl);//GetXCfgBaseAward(XLogicWorld.SP.MainPlayer.VIPLevel);
		if (xCfgVipBase == null)
		{
			return 0;
		}
	
		switch (eType)
		{
		case EVipConst.eVip_BuyTiliCount:	
			return xCfgVipBase.BuyTiliCount;
		case EVipConst.eVip_BuyGivefowCount:
			return xCfgVipBase.BuyGivefowCount;
		case EVipConst.eVip_BuyYaoqCount:
			return xCfgVipBase.BuyYaoqCount;
		case EVipConst.eVip_BuyShhjKillCount:
			return xCfgVipBase.BuyShhjKillCount;
		case EVipConst.eVip_BuyZhylCount:	
			return xCfgVipBase.BuyZhylCount;
		case EVipConst.eVip_BuyXdhCombatCount:
			return xCfgVipBase.BuyXdhCombatCount;
		case EVipConst.eVip_FreeXdhCombatCount:	
			return xCfgVipBase.FreeXdhCombatCount;
		case EVipConst.eVip_BuyAucPutawayCount:	
			return xCfgVipBase.BuyAucPutawayCount;
		case EVipConst.eVip_BuyShhCombatCount:
			return xCfgVipBase.BuyShhCombatCount;
		case EVipConst.eVip_BuyJlCount:	
			return xCfgVipBase.BuyJlCount;
		case EVipConst.eVip_FreeShhCombatCount:
			return xCfgVipBase.FreeShhCombatCount;
		case EVipConst.eVip_BeSkipCombatAni:
			return xCfgVipBase.BeSkipCombatAni;
		case EVipConst.eVip_SignMoneyMul:
			return xCfgVipBase.SignMoneyMul;
		case EVipConst.eVip_BeUseLd:
			return xCfgVipBase.BeUseLd;
		case EVipConst.eVip_FriMax:
			return xCfgVipBase.FriMax;
		case EVipConst.eVip_BlaMax:
			return xCfgVipBase.BlaMax;
	
		default:
			break;
		}
			
		return 0;
	}
	
	
	
	

	public float GetFloatVipAttri(EVipConst eType)
	{
		XCfgVipBase xCfgVipBase = XCfgVipBaseMgr.SP.GetConfig(XLogicWorld.SP.MainPlayer.VIPLevel);//GetXCfgBaseAward(XLogicWorld.SP.MainPlayer.VIPLevel);
		if (xCfgVipBase == null)
		{
			return 0;
		}
		
	switch (eType)
		{
		case EVipConst.eVip_SitExpMul:
			return xCfgVipBase.SitExpMul;
		}
		return 0;
	}
	
	
	public void VipTest()
	{
		XCfgVipBase xCfgVipBase = XCfgVipBaseMgr.SP.GetConfig(0);//GetXCfgBaseAward(XLogicWorld.SP.MainPlayer.VIPLevel);
		if (xCfgVipBase == null)
		{
			return ;
		}
		
		XCfgVipBase xCfgVipBase2 = XCfgVipBaseMgr.SP.GetConfig(1);//GetXCfgBaseAward(XLogicWorld.SP.MainPlayer.VIPLevel);
		if (xCfgVipBase2 == null)
		{
			return ;
		}
		
		
		
		XCfgVipAward xCfgVipAward2 = XCfgVipAwardMgr.SP.GetConfig(1);//GetXCfgBaseAward(XLogicWorld.SP.MainPlayer.VIPLevel);
		if (xCfgVipAward2 == null)
		{
			return ;
		}
	}
	
	public void OnRequestVipInfo(uint uVipLvl)
	{
		if(CanGotVipAward(uVipLvl))
		{
			CS_UInt.Builder msg = CS_UInt.CreateBuilder();
		msg.SetData(uVipLvl);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Vip_GotAward, msg.Build() );
		}
	}
	//=====================================================================
	public bool CheckVipAwardState(uint vipLvl)
	{
		if(vipLvl > (uint)EVipConst.eMAX_VIP_LVL || vipLvl <= 0)
			return false;
		
		uint uPos = vipLvl - 1;
		uint uState =  (uint)(1 << (int)uPos);
		if((XLogicWorld.SP.MainPlayer.VipAward & uState) != 0)
			return true;
		
		return false;
	}
	
	public void SetVipAwardState(uint vipLvl)
	{
		if(vipLvl > (uint)EVipConst.eMAX_VIP_LVL || vipLvl <= 0)
			return;
		
		uint uPos = vipLvl - 1;
		uint uState =  (uint)(1 << (int)uPos);
		
		XLogicWorld.SP.MainPlayer.VipAward |= uState;
	}
	
	public void RemoveVipAwardState(uint vipLvl)
	{
		if(vipLvl > (uint)EVipConst.eMAX_VIP_LVL || vipLvl <= 0)
			return;
		
		uint uPos = vipLvl - 1;
		uint uState =  (uint)(1 << (int)uPos);
		
		XLogicWorld.SP.MainPlayer.VipAward &= ~uState;
	}
	
	public void ClearVipAwardState()
	{
		XLogicWorld.SP.MainPlayer.VipAward = 0;
	}
	
	public bool CanGotVipAward(UInt32 uVipLvl)
	{
		if(CheckVipAwardState(uVipLvl))
			return false;
		if(uVipLvl > XLogicWorld.SP.MainPlayer.VIPLevel)
			return false;
		
		return true;
	}
	
}

