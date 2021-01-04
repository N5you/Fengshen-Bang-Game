using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class XUIFunctionBottomTR : XUICtrlTemplate<XFunctionBottomTR>
{
	bool showFinish = false;
	private SortedList<uint, UInt64> m_cachedEffect = new SortedList<uint, UInt64>();
	
	public XUIFunctionBottomTR()
	{
		RegEventAgent_CheckCreated(EEvent.FUNCTION_BUTTON_STARTEFFECT, OnShowEffect);
		RegEventAgent_CheckCreated(EEvent.UI_AddAward, OnAddAward);
		
		
		XEventManager.SP.AddHandler(OnStopEffet, EEvent.FUNCTION_BUTTON_STOPEFFECT);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		XEventManager.SP.SendEvent(EEvent.TopFuncBtn_Ready);
	}
	
	public void AddBtnDirect(List<uint> canUnLockList)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		LogicUI.AddBtnDirect(canUnLockList);
		showFinish = true;
		handleCachedEffect();
	}
	
	public void AddBtn(uint funcID,bool isNeedAnim)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		LogicUI.AddBtn(funcID,isNeedAnim);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFuncUnLock);
	}
	
	private void OnShowEffect(EEvent evt, params object[] args)
	{
		if ( args.Length < 3 )
			return;
		
		if ( showFinish )
		{
			LogicUI.StartEffect((uint)args[0], (uint)args[1], (int)args[2]);
		}
		else
		{
			uint tempValue1 = (uint)args[1];
			UInt64 t = (UInt64)tempValue1;
			t = t << 32;
			int tempValue2 = (int)args[2];
			UInt64 t1 = (UInt64)tempValue2;
			t = t | t1;
			m_cachedEffect[(uint)args[0]] = t;
		}
	}
	
	private void OnAddAward(EEvent evt, params object[] args)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		LogicUI.AddAwardTail();
	}
	
	private void OnStopEffet(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		LogicUI.StopEffect((uint)args[0], (uint)args[1], 0);
	}
	
	private void handleCachedEffect()
	{
		foreach ( KeyValuePair<uint,UInt64> item in m_cachedEffect )
		{
			UInt64 t = item.Value;
			uint t1 = (uint)(t >> 32);
			int t2 = (int)(t & 0x00000000FFFFFFFF);
			LogicUI.StartEffect(item.Key, t1, t2);
		}
		m_cachedEffect.Clear();
	}
	
	public Vector3 GetAwardPos()
	{
		return LogicUI.GetAwardPos();
	}
	
	public void DelTRButtion(uint featurid)
	{
		if ( LogicUI )
			LogicUI.DelButton(featurid);
	}
}
