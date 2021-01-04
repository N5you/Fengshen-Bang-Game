using UnityEngine;
using System.Collections;

class XUTFirstLogin : XUICtrlTemplate<XFirstLogin> 
{
	public static IProcessLoad	CurLoading;
	
	public override void Breathe()
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf )
			return ;
		
		if(CurLoading == null)
			return ;
		
		LogicUI.SetDiscription(CurLoading.GetProcessText());
		LogicUI.SetProgress((float)CurLoading.GetCurRate());
		
	}
}

