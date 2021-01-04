using UnityEngine;
using System.Collections;

class XUTToolTipB : XUICtrlTemplate<XToolTipB>
{	
	public XUTToolTipB()
	{
		RegEventAgent_CheckCreated(EEvent.ToolTip_B, OnToolTip);
	}
	
	public void OnToolTip(EEvent evt, params object[] args)
	{
		LogicUI.SetTipContent((string)(args[0]),(string)(args[1]),(string)(args[2]));
	}
	
	public override void Breathe()
	{
		if(LogicUI == null || LogicUI.gameObject.activeSelf == false)
			return ;

		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		//LogicUI.transform.position = new Vector3(Mathf.RoundToInt(vec.x+10),Mathf.RoundToInt(vec.y - 12),LogicUI.transform.position.z);
		LogicUI.transform.position = new Vector3(vec.x+10.5f,vec.y - 12.5f,LogicUI.transform.position.z);
	}
	public override void OnHide()
	{
		base.OnHide();
		return;
	}
	public override void OnShow()
	{
		base.OnShow();
		return;
	}
}
