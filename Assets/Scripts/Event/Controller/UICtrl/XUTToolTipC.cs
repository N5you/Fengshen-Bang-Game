
using UnityEngine;
using System.Collections;

class XUTToolTipC : XUICtrlTemplate<XToolTipC>
{	
	public XUTToolTipC()
	{
		RegEventAgent_CheckCreated(EEvent.ToolTip_C, OnToolTip);
	}
	
	public void OnToolTip(EEvent evt, params object[] args)
	{
		LogicUI.SetTipContent((string)(args[0]));
	}
	
	public override void Breathe()
	{
		if(LogicUI == null || LogicUI.gameObject.activeSelf == false)
			return ;

		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		//LogicUI.transform.position = new Vector3(Mathf.RoundToInt(vec.x+10),Mathf.RoundToInt(vec.y - 12),LogicUI.transform.position.z);
		LogicUI.transform.position = new Vector3(vec.x+10.5f,vec.y - 12.5f,LogicUI.transform.position.z);
	}
}


