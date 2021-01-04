using UnityEngine;
using System.Collections;

class XUTReadTip : XUICtrlTemplate<XReadTip>
{
	public XUTReadTip()
	{
		RegEventAgent_CheckCreated(EEvent.ToolTip_ReadTip_Discription, OnReadTipDiscription);
		RegEventAgent_CheckCreated(EEvent.ToolTip_ReadTip_Progress, OnReadTipProgress);
	}
	
	public void OnReadTipDiscription(EEvent evt, params object[] args)
	{
		LogicUI.SetDiscription((string)(args[0]));	
	}
	
	public void OnReadTipProgress(EEvent evt, params object[] args)
	{
		if(args[0] is string)
		{
			LogicUI.SetProgress((string)(args[0]));
		}
		else
		{
			LogicUI.SetProgress((float)(args[0]), (float)(args[1]));
		}
	}
}

