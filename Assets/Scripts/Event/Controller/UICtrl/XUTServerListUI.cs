using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTServerListUI : XUICtrlTemplate<XServerListUI>
{
	private List<ServerInfo> m_HoldServer;
	
	public XUTServerListUI()
	{
		m_HoldServer = new List<ServerInfo>();
		XEventManager.SP.AddHandler(SelectServer, EEvent.ServerList_SelectServer);
		RegEventAgent_CheckCreated(EEvent.ServerList_AddServerInfo, OnAddServerInfo);
	}
	
	public override void OnCreated(object arg)
	{
		base.OnCreated(arg);
		foreach(ServerInfo server in m_HoldServer)
			LogicUI.OnAddServerInfo(server);
	}
	
	void OnAddServerInfo(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			m_HoldServer.Add(args[0] as ServerInfo);
			return;
		}
		LogicUI.OnAddServerInfo(args[0] as ServerInfo);
	}
	
	void SelectServer(EEvent evt, params object[] args)
	{
		XLogicWorld.SP.LoginProc.ApplySelectCharServer((int)args[0]);
		LogicUI.Hide();
	}
}

