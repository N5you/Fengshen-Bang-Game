using UnityEngine;

class XUTOperTip : XUICtrlTemplate<XOperTip>
{
	public XUTOperTip()
	{
		Init();
	}
	public void Init()
	{
		RegEventAgent_CheckCreated(EEvent.Notice_OperTip,OperTipHandler);
	}
	
	public void OperTipHandler(EEvent evt, params object[] args)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		uint stringID = (uint)args[0];
		string content = (string)args[1];
		
		XCfgOperTip OperTip = XCfgOperTipMgr.SP.GetConfig(stringID);
		if(OperTip == null)
			return ;
		
		if(LogicUI.Btn != null)
		{
			LogicUI.Btn.normalSprite	= OperTip.BtnCom;
			LogicUI.Btn.hoverSprite		= OperTip.BtnHover;
			LogicUI.Btn.pressedSprite	= OperTip.BtnPress;
			LogicUI.Btn.UpdateImage();
		}
		
		LogicUI.LabelContent.text	= content;
	}
	
}
