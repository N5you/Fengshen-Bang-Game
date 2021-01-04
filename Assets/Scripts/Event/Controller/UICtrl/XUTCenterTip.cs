using UnityEngine;
using System.Collections;

class XUTCenterTip : XUICtrlTemplate<XCenterTip>
{
	public XUTCenterTip()
	{
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		XEventManager.SP.AddHandler(OnCenterTip, EEvent.ToolTip_CenterTip);
	}
	
	private void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eCenterTip);
	}
	
	private void OnCenterTip(EEvent evt, params object[] args)
	{
		if(null == LogicUI) return;
		float scale = 1.0f;
		if(args.Length >2 && null != args[2])
			scale = (float)(args[2]);
		LogicUI.OnCenterTip((ECenterTipStyle)(args[0]), (string)(args[1]), scale);
	}
}