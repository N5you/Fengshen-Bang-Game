using UnityEngine;
using System.Collections;

class XUTFlyItem : XUICtrlTemplate<XFlyItem>  
{
	private XItem mTargetItem;
	
	public XUTFlyItem()
	{
		//RegEventAgent_CheckCreated(EEvent.FlyItem_NewItem,FlyItemHandler);
	}
	
	public override EUIPanel GetPanelType() {return EUIPanel.eFlyItem; }
	
	public void FlyItemHandler(EEvent evt, params object[] args)
	{
		mTargetItem = (XItem)args[0];
		
		
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(mTargetItem.DataID);
		if(cfgItem == null)
			return ;
		
		LogicUI.ActionIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,mTargetItem.Color,mTargetItem.ItemCount);
		
		XUTFunctionButton xut = XUIManager.SP.GetUIControl(EUIPanel.eFunctionButton) as XUTFunctionButton;
		
		
		TweenPosition posAnim = LogicUI.GetComponent<TweenPosition>();
		if(posAnim != null)
		{
			posAnim.Reset();
			Vector3 targetPos = xut.GetBagPos();
			if(targetPos != Vector3.zero)
			{
				posAnim.to	= targetPos;
				posAnim.enabled	= true;
			}			
		}
		
		NcRotation rot = LogicUI.GetComponent<NcRotation>();
		if(rot != null)
		{
			rot.enabled	= true;
		}
	}
	
	
}
