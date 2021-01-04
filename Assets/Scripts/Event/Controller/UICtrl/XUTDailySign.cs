using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUTDailySign : XUICtrlTemplate<XUIDailySign>
{
	public XUTDailySign()
	{
		RegEventAgent_CheckCreated(EEvent.DailySign_Status, handleDailySignStatus);
		RegEventAgent_CheckCreated(EEvent.DailySign_Result, handleDailySignResult);
		
		RegEventAgent_CheckCreated(EEvent.DailySign_Hide, handleDailySignHide);
	}
	
	private void handleDailySignStatus(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.SetDailySignStatus((ulong)args[0], (ulong)args[1]);
	}
	
	private void handleDailySignResult(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return; 
		
		LogicUI.SetDailySignStatus((ulong)args[0], (ulong)args[1]);
		
		LogicUI.StartFinishEffect((ulong)args[1]);
	}
	
	private void handleDailySignHide(EEvent evt, params object[] args)
	{
		LogicUI.Invoke("HideSelf", 1.1f);
	}
}
	
