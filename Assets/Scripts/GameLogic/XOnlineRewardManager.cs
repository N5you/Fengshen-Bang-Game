using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
public class XOnlineRewardManager : XSingleton<XOnlineRewardManager>
{
	private uint m_GetID;
	private bool m_isCanGet;
	public static uint MAXLEVEL_TO_SHOW_ONLINEREWARD = 250;

	public bool IsCanGet { get { return m_isCanGet; } private set { m_isCanGet = value; } }

	public uint GetID { get { return m_GetID; } private set { m_GetID = value; } }

	public XOnlineRewardManager ()
	{
		m_GetID = 0;
		m_isCanGet = false;	
		XEventManager.SP.AddHandler (checkGetReward, EEvent.UI_OnOriginal);
	}
	
	public bool IsGetAllReward(uint getID)
	{
		XCfgOnlineReward cfg = XCfgOnlineRewardMgr.SP.GetConfig (getID);
		if (cfg == null)
			return true;
		return false;
	}
	
	public bool IsLastGetID()
	{
		XCfgOnlineReward cfg = XCfgOnlineRewardMgr.SP.GetConfig (m_GetID + 1);
		if (cfg == null)
			return true;
		return false;
	}

	public bool HandleGetItem()
	{
		//判断背包是否已满
		short emptyPos = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNoPile (EItemBoxType.Bag);
		if (emptyPos == -1) {
			XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 19);
			return false;
		}

		CS_Empty.Builder builder = CS_Empty.CreateBuilder ();
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_OnlineRewardGetItem, builder.Build ());
		return true;
	}

	public void ON_SC_TimeToGet(uint getID)
	{
		Debug.Log ("Now You Can GetReward:" + getID.ToString ());
		this.IsCanGet = true;
	}

	public void ON_SC_NewEvent(uint getID)
	{
		this.m_GetID = getID;
	}
	
	private void checkGetReward(EEvent evt, params object[] args)
	{
		if (!IsGetAllReward (m_GetID)) {
			XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eOnlineReawrd);
			XEventManager.SP.SendEvent (EEvent.OnlineReward_NewEvent, m_GetID);
		}
		else {
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eOnlineReawrd);
		}
	}
	
	public uint GetTime()
	{
		XCfgOnlineReward cfg = XCfgOnlineRewardMgr.SP.GetConfig (m_GetID);
		if (cfg != null) {
			return cfg.GetTime;
		}
		else {
			return 0;
		}
	}
	
	public uint GetNextTime()
	{
		XCfgOnlineReward cfg = XCfgOnlineRewardMgr.SP.GetConfig (m_GetID + 1);
		if (cfg != null) {
			return cfg.GetTime;
		}
		else {
			return 0;
		}
	}
	
	public string GetCurRewardItemName()
	{
		XCfgOnlineReward cfg = XCfgOnlineRewardMgr.SP.GetConfig (m_GetID);
		if(cfg == null)
		{
			Log.Write (LogLevel.ERROR, "the getid is not in OnlineReward List");
			return "";
		}
		XCfgItem iCfg = XCfgItemMgr.SP.GetConfig (cfg.RewardItemID);
		if (iCfg == null) {
			Log.Write (LogLevel.ERROR, "the id is not in Item List");
			return "";
		}
		return iCfg.Name;
	}
}
