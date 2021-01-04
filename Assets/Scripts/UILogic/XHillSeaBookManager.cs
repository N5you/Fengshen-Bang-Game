using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using XGame.Client.Network;

public class XHillSeaBookManager : XSingleton<XHillSeaBookManager > {
	
	public static uint m_uiMaxBossCount = 16; 
	
	private uint[] m_uiBossKillCountList;
	
	private uint m_uiBossKillIndex = 0;
	
	private uint m_uiFreeKillCount = 1;
	
	private uint m_uiBossTotalCount = 0;
	
	public uint freeKillCount{
		get{return m_uiFreeKillCount;}
	}
	
	public uint bossTotalCount{
		get{return m_uiBossTotalCount;}
	}
	
	public void init()
	{
		m_uiBossKillCountList = new uint[m_uiMaxBossCount];
		
		m_uiBossTotalCount =(uint) XCfgHillSeaBookMgr.SP.ItemTable.Count;
		
	}
	
	public void updateHillSeaBook(SC_HillSeaBookMsg msg)
	{
		m_uiBossKillIndex =(uint) msg.BossID;
		
		for(int i=0;i<msg.BossKillCountList.Count;i++ )
		{
			m_uiBossKillCountList[i] = msg.BossKillCountList[i];
		}
		
		XEventManager.SP.SendEvent(EEvent.HillSeaBook_Message,null );
	}
	
	public uint bossIndex{
		get{
			return m_uiBossKillIndex;
		}
	}
	
	public uint getBossKillCount(uint bossID)
	{
		if(m_uiBossKillCountList.Length<=bossID )
		{	
			Log.Write(LogLevel.ERROR,"hillSeaBook bossID out of range " );
			return 0;
		}
		
		return m_uiBossKillCountList[bossID];
	}
	
}

