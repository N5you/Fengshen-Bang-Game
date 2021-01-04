using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUTSevenTarget : XUICtrlTemplate<XSevenTargetUI>
{
	public XUTSevenTarget()
	{
		XEventManager.SP.AddHandler(OnSevenTargetItemUpdate, EEvent.SevenTarget_ItemStatus_Update);
		XEventManager.SP.AddHandler(OnSevenTargetAllItemUpdate, EEvent.SevenTarget_AllItemStatus_Update);
	}
	
	public void OnSevenTargetItemUpdate(EEvent evt, params object[] args)
	{
		if(null == LogicUI) 
			return;
		
		int itemId = (int)(args[0]);
		LogicUI.UpdateOneTargetItem(itemId);
	}
	
	public void OnSevenTargetAllItemUpdate(EEvent evt, params object[] args)
	{
		if(null == LogicUI) 
			return;
		
		for( int i = 1; i < 7; i++ )
			LogicUI.UpdateOneTargetItem(i);
	}
}
	
