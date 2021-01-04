using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class XUTShopDialog : XUICtrlTemplate<XShopDialog>
{
	
	public XUTShopDialog()
	{
		//XEventManager.SP.AddHandler(checkNpcDialog, EEvent.NpcDialog_CheckSignal);
		
		//XEventManager.SP.AddHandler(bindNpc, EEvent.NpcDialog_BindNpc);
	}
	
	public override void OnHide()
	{
		base.OnHide();
		
		XEventManager.SP.SendEvent(EEvent.ShopDialog_ChangeVisiable,false);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		XEventManager.SP.SendEvent(EEvent.ShopDialog_ChangeVisiable,true);
	}
	
}
