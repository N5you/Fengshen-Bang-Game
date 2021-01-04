using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;


public class XPlayHintMgr : XSingleton<XPlayHintMgr>
{	
	public string PlayName;
	public string QuestName;
	public uint   QuestID;
	public uint	  NeedLevel;
	public uint	  AtlasID;
	public string SpriteName;
	
	public void Init()
	{
		XEventManager.SP.AddHandler(LevelUp,EEvent.Update_Level);
		XEventManager.SP.AddHandler(OnMissionFinished,EEvent.Mission_ReferMission);
	}
	
	public void ShowUI()
	{
		ReflashData();
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePlayHint);
	}
	
	public void LevelUp(EEvent evt, params object[] args)
	{
		ReflashData();
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePlayHint);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePlayHint);		
	}
	
	private void OnMissionFinished(EEvent evt, params object[] args)
	{
		ReflashData();
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePlayHint);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePlayHint);		
	}
	
	private void ReflashData()
	{
		PlayName	= "";
		QuestName	= "";
		QuestID		= 0;
		NeedLevel	= 0;
		AtlasID		= 0;
		SpriteName	= "";
		
		XCfgPlayHint cfg = null;
		foreach(XCfgPlayHint tempCfg in XCfgPlayHintMgr.SP.ItemTable)
		{
			if(tempCfg.OpenLevel == XLogicWorld.SP.MainPlayer.Level && tempCfg.OpenQuestID != 0)
			{
				if(!XMissionManager.SP.hasReferMissionInList(tempCfg.OpenQuestID))
				{
					cfg	= tempCfg;
					break;
				}
			}
			else if(tempCfg.OpenLevel > XLogicWorld.SP.MainPlayer.Level)
			{
				cfg = tempCfg;
				break;
			}
		}
		
		if(cfg == null)
		{
			Log.Write(LogLevel.WARN,"not find Next Play Hint Player Level is {0}",XLogicWorld.SP.MainPlayer.Level);
			return;
		}
		
		PlayName	= cfg.OpenPlayName;
		QuestID		= cfg.OpenQuestID;
		QuestName	= cfg.OpenQuestName;
		NeedLevel	= cfg.OpenLevel;
		AtlasID		= cfg.AtlasID;
		SpriteName	= cfg.SpriteName;
	}
}
