
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTVip: XUICtrlTemplate<XVip>
{
	public XUTVip()
	{
		//RegEventAgent_CheckCreated(EEvent.GuildCreate_Init, OnInit);
		RegEventAgent_CheckCreated(EEvent.Vip_UpdateInfo, UpdateInfo);
	}
	
	
	public override void OnShow()
	{
		base.OnShow();
		
	}
	
	
	public void UpdateInfo(EEvent evt, params object[] args)
	{
		LogicUI.UpdateInfo();
	}
}

