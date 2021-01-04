using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;

public class XGrowthTargetManger  : XSingleton<XGrowthTargetManger>
{
	public XGrowthTargetManger()
	{
	}
	
	public void ON_SC_GrowthData_Coming(SC_GrowthTarget targetData)
	{
		switch ( targetData.Type )
		{
		case EGrowthTargetType.eGrowthTarget_SendStatusAll:
			_handleStatusAll(targetData);
			break;
			
		case EGrowthTargetType.eGrowthTarget_SendStatus:
			_handleStatus(targetData);
			break;
			
		case EGrowthTargetType.eGrowthTarget_GetAwardItemFailed:
			_handleAwardItems(targetData);
			break;
			
		default:
			break;
		}
	}
	
	public void ON_SC_Player_LevelUp()
	{
		XEventManager.SP.SendEvent(EEvent.GROWH_SHOWTARGETS);
	}
	
	private void _handleStatus(SC_GrowthTarget data)
	{
		for (int i = 0; i < data.GrowthListCount; i++)
        {
            GrowthTargetStatus obj = data.GetGrowthList(i);
			AllTargetStatus[obj.Targetid] = obj.Status;
			if ( obj.Status == 0x01 )
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_Cultivate, 900039u, 0);
            XEventManager.SP.SendEvent(EEvent.GROWTH_STATUS, obj.Targetid, obj.Status);
        }
	}
	
	private void _handleStatusAll(SC_GrowthTarget data)
	{
		for (int i = 0; i < data.GrowthListCount; i++)
        {
            GrowthTargetStatus obj = data.GetGrowthList(i);
			AllTargetStatus[obj.Targetid] = obj.Status;
			if ( obj.Status == 0x01 )
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_Cultivate, 900039u, 0);
            XEventManager.SP.SendEvent(EEvent.GROWTH_STATUSALL, obj.Targetid, obj.Status);
        }
	}
	
	private void _handleAwardItems(SC_GrowthTarget data)
	{
		for (int i = 0; i < data.GrowthListCount; i++)
        {
            GrowthTargetStatus obj = data.GetGrowthList(i);
            XEventManager.SP.SendEvent(EEvent.GROWTH_AWARDFAILED, obj.Targetid);
        }
	}
	
	public int GetTargetStatus(int targetid)
	{
		if ( AllTargetStatus.ContainsKey(targetid) )
			return AllTargetStatus[targetid];
		else
			return 0;
	}
	
	private SortedList<int, int> AllTargetStatus = new SortedList<int, int>();
}
   