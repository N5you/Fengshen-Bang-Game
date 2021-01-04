using UnityEngine;
using System;
using System.Collections;
using XGame.Client.Packets;
using System.Collections.Generic;

class XUTMessageBoxWithNoCancel : XUICtrlTemplate<XMessageBox>
{
	public XUTMessageBoxWithNoCancel()
	{
		XEventManager.SP.AddHandler(PreMessageBox, EEvent.MessageBoxWithNoCancel);
		RegEventAgent_CheckCreated(EEvent.MessageBoxWithNoCancel, OnMessageBox);
	}
	
	public override void OnHide()
	{
	}
	
	private void PreMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eMessageBoxWithNoCancel);
	}
	
	private void OnMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eMessageBoxWithNoCancel);
		LogicUI.MessageBox(args[0], args[1], args[2]);
	}
}
