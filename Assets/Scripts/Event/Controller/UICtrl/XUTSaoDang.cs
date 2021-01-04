 using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

class XUTSaoDang : XUICtrlTemplate<XSaoDang>
{
	public XUTSaoDang()
	{
		XEventManager.SP.AddHandler(Update_Battle_Result,EEvent.SaoDang_Battle_Result);
	}
	public override void OnHide()
	{
		base.OnHide();
	}
	public override void OnShow()
	{
		base.OnShow();
	}
    public void Update_Battle_Result(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return;
		LogicUI.Update_Battle_Result();
	}
}