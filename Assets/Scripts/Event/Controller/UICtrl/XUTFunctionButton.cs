using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using UnityEngine;

class XUTFunctionButton : XUICtrlTemplate<XFunctionButton>
{	
	public XUTFunctionButton()
	{
		RegEventAgent_CheckCreated(EEvent.Attr_Exp, OnExpChanged);
		XEventManager.SP.AddHandler(OnLevelChanged,EEvent.Attr_Dynamic);
		RegEventAgent_CheckCreated(EEvent.FUNCTION_BUTTON_STARTEFFECT, OnShowEffect);
		
		XEventManager.SP.AddHandler(OnStopEffet, EEvent.FUNCTION_BUTTON_STOPEFFECT);
	}
	
	private void RefreshExpSlider()
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		XMainPlayer mp = XLogicWorld.SP.MainPlayer;
		uint exp = mp.Exp;
		int level = mp.Level;
		
		XCfgPlayerLvUp cfg = XCfgPlayerLvUpMgr.SP.GetConfig((uint)(level));
		if(null == cfg) return;
		uint maxExp = cfg.MaxExp;
		float p = exp / (float)maxExp;
		LogicUI.SetRoleExp(exp,maxExp,p);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		RefreshExpSlider();
		XEventManager.SP.SendEvent(EEvent.BottomFuncBtn_Ready);
	}
	
	public void AddBtnDirect(List<uint> canUnLockList)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		LogicUI.AddBtnDirect(canUnLockList);
	}
	
	public void AddBtn(List<uint> canUnLockList,bool isNeedAnim)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		CoroutineManager.StartCoroutine(ShowAnim(canUnLockList,isNeedAnim));
		
	}
	
	public void AddBtn(uint canUnLockID,bool isNeedAnim)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		LogicUI.AddBtn(canUnLockID,isNeedAnim);
	}
	
	private void OnShowEffect(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.StartEffect((uint)args[0], (uint)args[1]);
	}
	
	private void OnStopEffet(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.StopEffect((uint)args[0], (uint)args[1]);
	}
	
	private void OnExpChanged(EEvent evt, params object[] args)
	{
		RefreshExpSlider();
	}
	
	private void OnLevelChanged(EEvent evt, params object[] args)
	{
		XCharacter ch = (XCharacter)(args[0]);
		EShareAttr attr = (EShareAttr)(args[1]);
		if(ch != XLogicWorld.SP.MainPlayer) return;
		if(EShareAttr.esa_Level != attr) return;
		RefreshExpSlider();
	}
	
	public Vector3 FindFinalPos(uint canUnLockID)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return Vector3.zero;
		
		return LogicUI.FindFinalPos(canUnLockID);
	}

	public IEnumerator ShowAnim(List<uint> canUnLockList,bool isNeedAnim)
	{
		if(!isNeedAnim)
		{
			LogicUI.AddBtnDirect(canUnLockList);
		}
		else
		{
			for(int i = 0; i < canUnLockList.Count; i++)
			{
				FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(canUnLockList[i]);
				if(unLock == null)
					continue;
			
				if(unLock.Group != (uint)FeatureType.EFeatureType_Func)
					continue;
				
				Vector3 UIPos = LogicUI.FindFinalPos(canUnLockList[i]);
				XEventManager.SP.SendEvent(EEvent.FuncUnLock_Data,canUnLockList[i],UIPos);
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eFuncUnLock);

				LogicUI.AddBtn(canUnLockList[i],isNeedAnim);
				yield return new WaitForSeconds(3f); 
			}
		}
	}
	
	public Vector3 GetBagPos()
	{
		if(LogicUI == null)
			return Vector3.zero;
		
		return LogicUI.GetBagPos();
	}
}