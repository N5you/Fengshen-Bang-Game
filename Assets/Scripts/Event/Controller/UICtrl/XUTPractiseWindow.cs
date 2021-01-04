using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTPractise : XUICtrlTemplate<UIPractiseWindow>
{
	public XUTPractise()
	{
		RegEventAgent_CheckCreated(EEvent.GROWTH_STATUS, ON_SC_GrowthStatus);
		RegEventAgent_CheckCreated(EEvent.GROWTH_STATUSALL, ON_SC_GrowthStatusAll);
		RegEventAgent_CheckCreated(EEvent.GROWTH_AWARDFAILED, ON_SC_GrowthAwardFailed);
		RegEventAgent_CheckCreated(EEvent.GROWH_SHOWTARGETS, ON_SC_ShowTargets);
		
		XEventManager.SP.AddHandler(ON_SC_GrowthGetAwardItems, EEvent.GROWTH_GETAWARDITEMS);
	}
	
	public void ON_SC_GrowthStatus(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.UpdateTargetStatus((int)args[0], (int)args[1]);
	}
	
	public void ON_SC_GrowthStatusAll(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.SetTargetStatus((int)args[0], (int)args[1]);
	}
	
	public void ON_SC_GrowthAwardFailed(EEvent evt, params object[] args)
	{
	}
	
	public void ON_SC_ShowTargets(EEvent evt, params object[] args)
	{
		LogicUI.showAllTargets();
	}
	
	public void ON_SC_GrowthGetAwardItems(EEvent evt, params object[] args)
	{
		if ( args.Length < 1 )
			return;
		
		CS_GrowthTarget.Builder msg = CS_GrowthTarget.CreateBuilder();
		msg.SetTargetid((int)args[0]);
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_GrowthTargetGetAwardItem, msg.Build());
	}
}

