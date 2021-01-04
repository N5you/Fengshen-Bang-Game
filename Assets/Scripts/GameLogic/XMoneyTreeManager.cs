using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

class XMoneyTreeManager : XSingleton<XMoneyTreeManager>
{
	
	private uint m_GetGameMoney;
	private uint m_CostRealMoney;
	private uint m_curShakeTime;
	private uint m_LeftCount;

	public uint LeftCount {
		get {
			XCfgMoneyTree curConfig = XCfgMoneyTreeMgr.SP.GetConfig ((uint)XLogicWorld.SP.MainPlayer.Level);
			LeftCount = curConfig.MaxShakeEveryDay - this.m_curShakeTime;
			return m_LeftCount;
		}
		private set { m_LeftCount = value; }
	}
	
	public XMoneyTreeManager ()
	{
		this.m_GetGameMoney = 0;
		this.m_CostRealMoney = 0;
		this.m_curShakeTime = 0;
		this.m_LeftCount = 0;
	}

	public bool SendShake()
	{
		XCfgMoneyTree curConfig = XCfgMoneyTreeMgr.SP.GetConfig ((uint)XLogicWorld.SP.MainPlayer.Level);

		this.m_CostRealMoney = GetCostRealMoney ();

		if (XLogicWorld.SP.MainPlayer.RealMoney <= m_CostRealMoney) {
			XEventManager.SP.SendEvent (EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format (XStringManager.SP.GetString (83), m_CostRealMoney));
			return false;
		}

		if ((uint)m_curShakeTime >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyYaoqCount)) {
			XEventManager.SP.SendEvent (EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format (XStringManager.SP.GetString (82), curConfig.MaxShakeEveryDay));
			XEventManager.SP.SendEvent (EEvent.MoneyTree_MaxShake, curConfig.MaxShakeEveryDay);
			return false;
		}
		
		++m_curShakeTime;
		CS_Empty.Builder builder = CS_Empty.CreateBuilder ();
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_MoneyTree_Shake, builder.Build ());
		return true;
	}

	public uint GetCostRealMoney()
	{
		XCfgMoneyTree xCfgMoneyTree = XCfgMoneyTreeMgr.SP.GetConfig ((uint)XLogicWorld.SP.MainPlayer.Level);
		uint costRealMoney = (uint)(m_curShakeTime * xCfgMoneyTree.CostRealMoney + xCfgMoneyTree.FirstCostRealMoney);
		m_CostRealMoney = costRealMoney;
		return costRealMoney;
	}

	public bool IsMaxShake()
	{
		XCfgMoneyTree xCfgMoneyTree = XCfgMoneyTreeMgr.SP.GetConfig ((uint)XLogicWorld.SP.MainPlayer.Level);
		if (m_curShakeTime >= xCfgMoneyTree.MaxShakeEveryDay)
			return true;
		return false;
	}


    #region packets
	public void ON_SC_Resualt(int getGameMoney, int isCrit, int curShakeTimes)
	{
		int crit = isCrit;
		this.m_GetGameMoney = (uint)getGameMoney;
		this.m_curShakeTime = (uint)curShakeTimes;
		XEventManager.SP.SendEvent (EEvent.MoneyTree_Resualt, getGameMoney, crit, curShakeTimes);
	}

	public void ON_SC_SendShake(int lastShakeTimes)
	{
		this.m_curShakeTime = (uint)lastShakeTimes;
		XEventManager.SP.SendEvent (EEvent.MoneyTree_ShakeTimes, m_curShakeTime);
	}
	
	public void ON_SC_ResetShakeTime()
	{
		//m_ShakeTime = 0;
	}
    #endregion
}