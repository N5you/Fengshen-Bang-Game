
	
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTGuildCreate: XUICtrlTemplate<XGuildCreate>
{	
	
	public XUTGuildCreate()
	{
		RegEventAgent_CheckCreated(EEvent.GuildCreate_Init, OnInit);
	}
	
	
	public override void OnShow()
	{
		base.OnShow();
		
	}
	
	public void OnInit(EEvent evt, params object[] args)
	{
		UIEventListener ls = UIEventListener.Get(LogicUI.m_ButCreate);
		ls.onClick	= OnCreateGuild;
		
		UIEventListener ls2 = UIEventListener.Get(LogicUI.m_ButCancel);
		ls2.onClick	= OnCancelCreate;
	}
	
	public void OnCreateGuild(GameObject go)
	{
		LogicUI.m_LabelGuildMoney.GetComponent<UILabel>().text = ((uint)EGuildConstant.eCREATE_GUILD_MONEY).ToString();
		string guildName = LogicUI.m_LabelGuildName.GetComponent<UILabel>().text;
		XGuildManager.SP.RequestGuildCreate(guildName);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eGuildCreate);
	}
	
	public void OnCancelCreate(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eGuildCreate);
	}
	
}	
	
	
	