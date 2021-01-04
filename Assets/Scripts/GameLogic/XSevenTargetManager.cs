using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public class SevenTargetItem
{
	public uint status;
	public uint status1;
	public uint status2;
	public uint status3;
}

public class XSevenTargetManager : XSingleton<XSevenTargetManager>
{
	private SortedList<uint, SevenTargetItem> m_allSevenTarget = new SortedList<uint, SevenTargetItem>();
	
	public UInt64 m_userFirstLoginTime = 0;
	public UInt64 m_currTime			= 0;
	public XSevenTargetManager()
	{
	}
	
	public void ON_SC_AllSevenTargetStatus(SC_SevenListData msg)
	{
		m_userFirstLoginTime = msg.PlayerCreateTime;
		m_currTime = msg.CurrentTime;
		
		for( int i = 0; i < msg.DataListList.Count; i++ )
		{
			SC_SevenItemData itemdata = msg.GetDataList(i);
			SevenTargetItem item = new SevenTargetItem();
			item.status = itemdata.Status;
			if ( itemdata.HasStatus1 )
				item.status1 = itemdata.Status1;			
			if ( itemdata.HasStatus2 )
				item.status2 = itemdata.Status2;			
			if ( itemdata.HasStatus3 )
				item.status3 = itemdata.Status3;
			m_allSevenTarget[itemdata.Id] = item;
		}
		
		XEventManager.SP.SendEvent(EEvent.SevenTarget_AllItemStatus_Update);
	}
	
	public bool Finish()
	{
		bool finish = false;
		ulong goTime = m_currTime - m_userFirstLoginTime;
		ulong goDay = goTime / (24 * 60 * 60);
		if ( goDay > 10 )
			finish = true;
		if ( m_allSevenTarget.ContainsKey(7) && 0x11 == m_allSevenTarget[7].status )
			finish = true;
		
		return finish;
	}
	
	public void ON_SC_ItemTargetStatus(SC_SevenItemData msg)
	{
		SevenTargetItem item = new SevenTargetItem();
		item.status = msg.Status;
		if ( msg.HasStatus1 )
			item.status1 = msg.Status1;			
		if ( msg.HasStatus2 )
			item.status2 = msg.Status2;			
		if ( msg.HasStatus3 )
			item.status3 = msg.Status3;
		m_allSevenTarget[msg.Id] = item;
		
		XEventManager.SP.SendEvent(EEvent.SevenTarget_ItemStatus_Update, (int)msg.Id);
	}
	
	public void GetAwardById(int id)
	{
		SevenTargetItem item = GetTargetItemStatus((uint)id);
		if ( null == item || item.status != 0x01 )
		{
			return;
		}
		if ( XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag) < 2u )
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 19);
			return;
		}
		
		CS_Int.Builder msg = CS_Int.CreateBuilder();
		msg.SetData(id);
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SevenTargetGetAward, msg.Build());
	}
	
	public void GetAllSevenTarget()
	{
		CS_Int.Builder msg = CS_Int.CreateBuilder();
		msg.SetData(0);
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCC_SevenTargetGetAll, msg.Build());
	}
	
	public SevenTargetItem GetTargetItemStatus(uint id)
	{
		if ( m_allSevenTarget.ContainsKey(id) )
			return m_allSevenTarget[id];
		else
			return null;
	}
}
