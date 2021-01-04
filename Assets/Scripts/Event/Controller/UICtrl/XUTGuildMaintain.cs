using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTGuildMaintain: XUICtrlTemplate<XGuildMaintain>
{	
	
	//private SortedList<uint,>
	public XUTGuildMaintain()
	{
		RegEventAgent_CheckCreated(EEvent.GuildMaintain_UpdateAnno, OnUpdateGuildAnnoInfo);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		//LogicUI.Show();
		//
	}
	
	public void OnUpdateGuildAnnoInfo(EEvent evt, params object[] args)
	{
		LogicUI.UpdateAnno();
	}
	
}
	