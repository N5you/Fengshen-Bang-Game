using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUTDayActivity : XUICtrlTemplate<XUIDayActivity> 
{
	public XUTDayActivity()
	{
		RegEventAgent_CheckCreated(EEvent.DayActivity_UpdateActivityValue,UpdateCurActivityValue);
		RegEventAgent_CheckCreated(EEvent.DayActivity_ResetInfo,ResetUI);
	}
	
	public override void OnShow()
	{
		base.OnShow ();
		if(LogicUI == null)
			return;
		//设置当前活跃度数值
		SetCurActivityValue(XDayActivityManager.SP.CurActivityValue);
		
		//设置滑动条100分比
		SetProgressValue(XDayActivityManager.SP.CurActivityValue);
		
		//更新数据
		UpdateData();
	}

	private void UpdateData()
	{
		XCfgDayActivityBase xcfg = null;
		foreach(KeyValuePair<uint,XDayActivityItem> s in XDayActivityManager.SP.GetActivityItemList())
		{
			xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(s.Key);
			if(xcfg == null)
			{
				Debug.LogError("The DayActivityBase Config not found ItemID:" + s.Key);
				return;
			}
			XUIDayActivity.XUIDayActivityItem items = LogicUI.GetUIActivityItem(s.Key);
			if(items == null)
			{
				LogicUI.AddActivityItem(s.Value.ItemID,(int)s.Value.Status,s.Value.CurProgress);
			}else
			{
				LogicUI.UpdateActivityItem(s.Value.ItemID,(int)s.Value.Status,s.Value.CurProgress);
			}
		}
		
		int idx = 0;
		foreach(KeyValuePair<uint,XDayActivityAward> s in XDayActivityManager.SP.GetAwardList())
		{
			XUIDayActivity.XUIDayActivityAward awardItem = LogicUI.GetUIActivityAward(s.Key);
			if(awardItem == null)
			{
				LogicUI.AddAwardItem(s.Key,idx,s.Value.Status);
			}
			else
			{
				LogicUI.UpdateAward(s.Key,s.Value.Status);
			}
			++idx;
		}
	}
	
	public void SetAwardStatus(uint _awardID,int _status)
	{
		XUIDayActivity.XUIDayActivityAward awardItem = LogicUI.GetUIActivityAward(_awardID);
		if(awardItem != null)
		{
			awardItem.SetStatus((XDayActivityAward.EAwardStatus)_status);
		}
	}
	
	public void SetProgressValue(int _value)
	{
		if(LogicUI == null)
			return;
		LogicUI.SetProgressValue(_value);
	}
	
	public void SetCurActivityValue(int _activityValue)
	{
		if(LogicUI == null)
			return;
		LogicUI.SetCurActivityValue(_activityValue);
	}
	
	private void UpdateCurActivityValue(EEvent evt,params object[] args)
	{
		if(LogicUI == null)
			return;
		Debug.Log("SetCurActivityValue:"+(uint)args[0]);
		SetCurActivityValue((int)args[0]);
	}
			
	private void ResetUI(EEvent evt,params object[] args)
	{
		if(LogicUI == null)
			return;
		Debug.Log("SetCurActivityValue:"+(int)args[0]);
		LogicUI.Reset();
	}
}
