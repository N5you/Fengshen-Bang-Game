using UnityEngine;

class XUTGetAward : XUICtrlTemplate<XGetAward>
{
	public XUTGetAward()
	{
		XEventManager.SP.AddHandler(FreshHandler,EEvent.UI_FreshAward);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		//get Pos
		Vector3 pos = FeatureDataUnLockMgr.SP.GetAwardPos();
		
		//set Pos		
		LogicUI.transform.localPosition	= new Vector3(Mathf.RoundToInt(pos.x),Mathf.RoundToInt((pos.y - 65)),pos.z);
		
		freshUI();
	}
	
	public void freshUI()
	{
		int count = FeatureDataUnLockMgr.SP.mAwardList.Count;
		
		if(count == 0)
			return ;
		
		LogicUI.Clear();
		
		foreach(FeatureDataUnLockMgr.AwardData data in FeatureDataUnLockMgr.SP.mAwardList.Values)
		{
			LogicUI.AddAward(data.Index,data.Type);
		}
	}
	
	public void FreshHandler(EEvent evt, params object[] args)
	{
		freshUI();
	}
} 

