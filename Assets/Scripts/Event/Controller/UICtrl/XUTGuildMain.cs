
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTGuildMain: XUICtrlTemplate<XGuildMain>
{	
	private		STGuildBaseInfo										m_stGuildBaseInfo					 = null;
	// 帮会建筑数据
	public 		STGuildConstInfo[] 								m_stGuildConstInfo 				= 	null;
	// 帮会接口
	public		byte[] 													m_iInter 								= 	null;
	
	//private SortedList<uint,>
	public override void OnShow()
	{
		base.OnShow();
		//LogicUI.Show();
		if(XGuildManager.SP.CheckGuildDataState((uint)EGuildConstant.eGET_BAS_GUILD_INFO) == false)
		{
			XGuildManager.SP.RequestGuildBaseInfo();
			return;
		}
		
		LogicUI.ShowGuildBaseInfo(m_stGuildBaseInfo, m_stGuildConstInfo, m_iInter);
	}
	
	public XUTGuildMain()
	{
		RegEventAgent_CheckCreated(EEvent.GuildMain_Init, OnInit);
		RegEventAgent_CheckCreated(EEvent.GuildMain_UpdateInfo, OnUpdateGuildInfo);
		//RegEventAgent_CheckCreated(EEvent.GuildList_UpdateGuildSynInfo, OnUpdateGuildSynInfo);
		//RegEventAgent_CheckCreated(EEvent.GuildList_UpdateSelfApplyState, OnUpdateAllApplyState);
	}
	
	// 初始化 
	public void OnInit(EEvent evt, params object[] args)
	{
		m_stGuildBaseInfo 		= XGuildManager.SP.m_stGuildBaseInfo;
		m_stGuildConstInfo		= XGuildManager.SP.m_stGuildConstInfo;
		m_iInter					= XGuildManager.SP.m_iInter;
		
		UIEventListener ls = UIEventListener.Get(LogicUI.m_ButGuildMgr);
		ls.onClick	= OnShowGuildMgr;
		
		UIEventListener ls2 = UIEventListener.Get(LogicUI.m_ButAllGuild);
		ls2.onClick	= OnShowAllGuildList;
		
		UIEventListener ls3 = UIEventListener.Get(LogicUI.m_ButMain);
		ls3.onClick	= OnShowSelfGuildList;
		
		UIEventListener ls4 = UIEventListener.Get(LogicUI.m_ButMainJia);
		ls4.onClick	= OnSpeedUpMain;
	}
	
	
	public void OnShowGuildMgr(GameObject go)
	{
		if(XLogicWorld.SP.MainPlayer.GuildId == 0 && XGuildManager.SP.m_stGuildBaseInfo.uMasterId != XLogicWorld.SP.MainPlayer.ID)
			return;
		
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eGuildMaintain);
	}
	
	public void OnShowAllGuildList(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eGuildList);
	}
	
	public void OnShowSelfGuildList(GameObject go)
	{ 
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eGuildInfo);
	}
	
	public void OnSpeedUpMain(GameObject go)
	{ 
		
	}
	
	public void OnUpdateGuildInfo(EEvent evt, params object[] args)
	{
		LogicUI.ShowGuildBaseInfo(m_stGuildBaseInfo, m_stGuildConstInfo, m_iInter);
	}
	
}