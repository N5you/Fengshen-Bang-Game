using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public class DailySignManager : XSingleton<DailySignManager>
{
	ulong mDailySigned = 0;
	ulong mDailyStatus = 0;
		
	
	public DailySignManager()
	{
	}
	
	public void ON_SC_DailyStatus(SC_DailySignStatus msg)
	{
		mDailySigned = msg.DailySigned;
		mDailyStatus = msg.DailyStatus;
		
		XEventManager.SP.SendEvent(EEvent.DailySign_Status, mDailySigned, mDailyStatus);
	}
	
	public void ON_SC_DailyResult(SC_DailySignStatus msg)
	{
		mDailySigned = msg.DailySigned;
		mDailyStatus = msg.DailyStatus;
		XEventManager.SP.SendEvent(EEvent.DailySign_Result, mDailySigned, mDailyStatus);
		if ( GetCanFinish() )
			XEventManager.SP.SendEvent(EEvent.DailySign_Hide);
	}
	
	public void GetAward(int index)
	{
		SC_Int.Builder msg = SC_Int.CreateBuilder();
		msg.SetData(index);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_DailySignGetAward, msg.Build());
	}
	
	public bool GetCanAward(int index)
	{
		ulong tag1 = 1;
		tag1 = (tag1 << index) & mDailySigned;
		ulong tag2 = 1;
		tag2 = (tag2 << index) & mDailyStatus;
		
		if ( tag2 == 1 )
			return false;
		if ( tag1 == 0 )
			return false;
		
		return true;
	}
	
	public bool GetCanFinish()
	{
		for ( int i = 0; i < 30; i++ )
		{
			ulong tag1 = 1;
			tag1 = (mDailySigned >> i) & tag1;
			ulong tag2 = 1;
			tag2 = (mDailyStatus >> i) & tag2;
			if ( 1 == tag1 && 0 == tag2 )
				return false;
		}
		return true;
	}
}
