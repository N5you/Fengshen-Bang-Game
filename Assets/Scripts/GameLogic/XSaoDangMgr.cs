using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public class XSaoDangManager : XSingleton<XSaoDangManager>
{
	private int m_ClientSceneID;
	private int m_ClientSceneLevel;
	private int m_LeftCnt;
	
	public SC_BattleResult m_Result;
	
	public XSaoDangManager()
	{
		m_ClientSceneID = 0;
		m_ClientSceneLevel = 0;
		m_LeftCnt = 0;
	}
	
	public int ClientSceneID
	{
		get
		{
			return m_ClientSceneID;
		}
		set
		{
			m_ClientSceneID = value;
		}
	}
	
	public int ClientSceneLevel
	{
		get
		{
			return m_ClientSceneLevel;
		}
		set
		{
			m_ClientSceneLevel = value;
		}
	}
	
	public int LeftCnt
	{
		get
		{
			return m_LeftCnt;
		}
		set
		{
			m_LeftCnt = value;
		}
	}
	
	public void ApplyStartSaoDang()
	{
		CS_SaoDang_Start.Builder msg =  CS_SaoDang_Start.CreateBuilder();
		msg.ClientSceneID = (uint)ClientSceneID;
		msg.ClientSceneLevel = (uint)ClientSceneLevel;
		msg.Count = (uint)LeftCnt;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SaoDang_Start, msg.Build());
	}
	
	
	public void ON_SC_SaoDang_Result(SC_BattleResult msg)
	{
		m_Result = 	msg;
		XEventManager.SP.SendEvent(EEvent.SaoDang_Battle_Result,msg);
	}
}