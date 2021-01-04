
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGuildCreate")]
public class XGuildCreate: XDefaultFrame
{
	
	public GameObject	m_ButCreate;
	public GameObject m_ButCancel;
	
	public GameObject m_LabelGuildName;
	public GameObject m_LabelGuildMoney;
	
	
	public override bool Init()
	{
		base.Init();
		XEventManager.SP.SendEvent(EEvent.GuildCreate_Init);
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		return true;
	}


	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGuildCreate);
	}
}
