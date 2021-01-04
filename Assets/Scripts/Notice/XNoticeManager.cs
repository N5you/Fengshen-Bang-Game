using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public enum ENotice_Type
{
	ENotice_Type_SystemTop,	//Top
	ENotice_Type_Operator,	//operator
	ENotice_Type_Chat,		//chat
	ENotice_Type_SystemMid, //Mid
	ENoitce_Type_CenterTip,
	ENotice_Type_Num
}

public class XNoticeManager : XSingleton<XNoticeManager>
{
	public void Init()
	{
		XEventManager.SP.AddHandler(NoticeFinishHandle, EEvent.Notice_SystemTopFinish);
		XEventManager.SP.AddHandler(NoticeMidFinishHandle, EEvent.Notice_SystemMidFinish);
		XEventManager.SP.AddHandler(NoticeFirstComeInHandle, EEvent.MainPlayer_EnterGame);
		XEventManager.SP.AddHandler(NoticeMissionFinishHandle, EEvent.Mission_ReferMission);
	}
	
	Queue<string>	SystemTopQueue = new Queue<string>();
	Queue<string>	SystemMidQueue = new Queue<string>();
	bool IsSystemTopNoticing = false;
	bool IsSystemMidNoticing = false;
		
	public  void Notice(ENotice_Type type,uint stringID,params object[] args)
	{
		switch(type)
		{
		case ENotice_Type.ENotice_Type_SystemTop:
		{
			string content = XStringManager.SP.GetString(stringID);
			content = string.Format(content,args);
			SystemTopQueue.Enqueue(content);			
			Notice_SystemTop();
		}
			break;
		case ENotice_Type.ENotice_Type_Operator:
		{
			XCfgOperTip Cfg = XCfgOperTipMgr.SP.GetConfig(stringID);
			if(Cfg == null)
				return ;
			string content = string.Format(Cfg.Content,args);
			XUTOperTip operTip = XUIManager.SP.GetUIControl(EUIPanel.eOperTip) as XUTOperTip;
			if(operTip == null)
				return ;
			
			if(operTip.IsShow())
				return ;
			
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eOperTip);
			XEventManager.SP.SendEvent(EEvent.Notice_OperTip,stringID,content);
			
		}
			break;
		case ENotice_Type.ENotice_Type_Chat:
		{
			string content = XStringManager.SP.GetString(stringID);
			content = string.Format(content,args);
			XEventManager.SP.SendEvent(EEvent.Chat_Notice,content);
		}
			break;
		case ENotice_Type.ENotice_Type_SystemMid:
		{
			string content = XStringManager.SP.GetString(stringID);
			content = string.Format(content,args);
			SystemMidQueue.Enqueue(content);
			Notice_SystemMid();
		}
			break;
		case ENotice_Type.ENoitce_Type_CenterTip:
		{
			string content = XStringManager.SP.GetString(stringID);
			content = string.Format(content,args);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.BigFont_Up,content);
		}
			break;
		default:
			break;
		}
	}

	private  void Notice_SystemTop()
	{
		if(IsSystemTopNoticing)
			return ;
		
		XUTMainPlayerInfo UIMainInfo = XUIManager.SP.GetUIControl(EUIPanel.eMainPlayerInfo) as XUTMainPlayerInfo;
		if(UIMainInfo == null)
			return ;
		
		if(SystemTopQueue.Count <= 0)
			return ;
		
		if(!UIMainInfo.IsResourceLoaded())
			return ;
		
		IsSystemTopNoticing	= true;
		
		string content = SystemTopQueue.Peek();
		
		UIMainInfo.SendSystemTopNotice(content);
	}
	
	private void Notice_SystemMid()
	{
		XUTMainPlayerInfo UIMainInfo = XUIManager.SP.GetUIControl(EUIPanel.eMainPlayerInfo) as XUTMainPlayerInfo;
		if(UIMainInfo == null)
			return ;
		
		if(SystemMidQueue.Count <= 0)
			return ;
		
		if(!UIMainInfo.IsResourceLoaded())
			return ;
		
		if(IsSystemMidNoticing)
		{
//			UIMainInfo.ReduceSystemMidTime();
			return ;
		}
//		
		IsSystemMidNoticing	= true;
		
		string content = SystemMidQueue.Peek();
		
		bool isNeedCut = false;
//		if(SystemMidQueue.Count >= 2)
//			isNeedCut	= true;
		
		UIMainInfo.SendSystemMidNotice(content,isNeedCut);
	}
	
	public void NoticeFinishHandle(EEvent evt, params object[] args)
	{
		IsSystemTopNoticing	 = false;
		SystemTopQueue.Dequeue();
		
		if(SystemTopQueue.Count <= 0)
			return ;
		
		Notice_SystemTop();
	}
	
	public void NoticeMidFinishHandle(EEvent evt, params object[] args)
	{
		IsSystemMidNoticing	 = false;
		SystemMidQueue.Dequeue();
		
		if(SystemMidQueue.Count <= 0)
			return ;
		
		Notice_SystemMid();
	}
	
	public void NoticeFirstComeInHandle(EEvent evt, params object[] args)
	{
		XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemTop,171,XLogicWorld.SP.MainPlayer.Name);
	}
	
	public void NoticeMissionFinishHandle(EEvent evt, params object[] args)
	{
		uint missionID	= (uint)args[0];
		XCfgMission cfg = XCfgMissionMgr.SP.GetConfig(missionID);
		if(cfg == null)
			return ;
		
		float moneyParam = 1;
		float expParam = 1;
		float gangExpParam = 1;
		
		if(1== cfg.isDynamic)
		{
			XCfgMissionsDynamic missionDyn = XCfgMissionsDynamicMgr.SP.GetConfig((uint)( XLogicWorld.SP.MainPlayer.Level) );
			XCfgMissionsDynamic missionDynBase = XCfgMissionsDynamicMgr.SP.GetConfig((uint)( cfg.needLevel ) );
			
			moneyParam = missionDyn.moneyParam/missionDynBase.moneyParam;
			expParam = missionDyn.expParam/missionDynBase.expParam;
			gangExpParam = missionDyn.gangExpParam/missionDynBase.gangExpParam;	
		}
		
		int awardMoney = Mathf.CeilToInt(cfg.awardMoney*moneyParam);
		int awardExp = Mathf.CeilToInt(cfg.awardExp*expParam);
		uint awardIngot = cfg.awardIngot;
		
		if(awardMoney > 0)
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,505,awardExp);
		if(awardExp > 0)
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,507,awardMoney);
		if(awardIngot > 0)
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,506,awardIngot);
	}
}
