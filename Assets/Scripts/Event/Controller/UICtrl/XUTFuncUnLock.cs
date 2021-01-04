using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTFuncUnLock : XUICtrlTemplate<XFuncUnLock>
{
	public static uint FuncID;
	private Vector3 mTargetPos;
	
	
	public XUTFuncUnLock()
	{
		FuncID	= 0;
		XEventManager.SP.AddHandler(DataHandler,EEvent.FuncUnLock_Data);
	}
	
	public void DataHandler(EEvent evt, params object[] args)
	{
		FuncID	= (uint)args[0];
		mTargetPos	= (Vector3)args[1];
		
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		LogicUI.FlySprite(mTargetPos);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(FuncID);
		if(unLock == null)
			return ;
		
		LogicUI.IsMix	= unLock.MixID > 0 ? true : false;
		
		LogicUI.Label.text	= unLock.Des;
		XUIDynamicAtlas.SP.SetSprite(LogicUI.ImageBtn.target, (int)unLock.AtlasID, unLock.IconID_Com, true, null);
		LogicUI.ImageBtn.normalSprite	= unLock.IconID_Com;
		LogicUI.ImageBtn.hoverSprite	= unLock.IconID_Hover;
		LogicUI.ImageBtn.pressedSprite	= unLock.IconID_Pressed;
	}
	
}