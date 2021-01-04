using UnityEngine;
using System.Collections;

class XUTMessageBox : XUICtrlTemplate<XMessageBox>
{
	public XUTMessageBox()
	{
		XEventManager.SP.AddHandler(PreMessageBox, EEvent.MessageBox);
		RegEventAgent_CheckCreated(EEvent.MessageBox, OnMessageBox);
	}
	
	public override void OnHide()
	{
	}
	
	private void PreMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eMessageBox);
	}
	
	private void OnMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eMessageBox);
		LogicUI.MessageBox(args[0], args[1], args[2]);
	}
}