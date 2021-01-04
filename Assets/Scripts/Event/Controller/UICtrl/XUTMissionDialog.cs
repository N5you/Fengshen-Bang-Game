using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class XUTMissionDialog : XUICtrlTemplate<XMissionDialog> 
{
	
	public XUTMissionDialog()
	{
		//XEventManager.SP.AddHandler(checkNpcDialog, EEvent.NpcDialog_CheckSignal);
		
		//XEventManager.SP.AddHandler(bindNpc, EEvent.NpcDialog_BindNpc);
	}
	
	public override void OnHide()
	{
		base.OnHide();
	}
}
