using System;

class XUTScenePassed : XUICtrlTemplate<XScenePassed>
{
	private string mName;
	private int mStarResult;
	private int mTotalExp;
	private int mExtExp;
	private int mSceneLevel;
	public XUTScenePassed()
	{
		XEventManager.SP.AddHandler(SetResult,EEvent.SubScene_SetResult);
		XEventManager.SP.AddHandler(SetAwardItem,EEvent.SubScene_AwardSel);
	}
	
	public override void OnShow()
	{
		base.OnShow();		
	
		/*
		战斗评价	分数	显示图片	评分星数
		完胜		5	完胜		3颗星
		大胜		4	大胜		2颗星
		小胜		3	大胜		2颗星
		险胜		2	胜利		1颗星
		惜败		0	失败		0
		小败		-1	失败		0
		大败		-3	失败		0	
		惨败		-5	失败		0
		 */
		int starsCount = 0;
		if ( 5 == mStarResult )		
		{
			starsCount = 3;
		}
		else if ( 4 == mStarResult || 3 == mStarResult )
		{
			starsCount = 2;
		}
		else if ( 2 == mStarResult )
		{
			starsCount = 1;
		}
		else
		{
			starsCount = 0;
		}
		LogicUI.SetBattleResult(starsCount, mTotalExp, mExtExp, mSceneLevel * 200);
		LogicUI.MoneyCost.text = string.Format(XStringManager.SP.GetString(1075), mSceneLevel * 200);
	}
	
	public void SetResult(EEvent evt, params object[] args)
	{
		mName	= (string)args[0];
		mStarResult	= (int)args[1];
		mTotalExp = (int)args[2];
		mExtExp = (int)args[3];
		mSceneLevel = (int)args[4];
	}
	
	public void SetAwardItem(EEvent evt, params object[] args)
	{
		uint ItemIndex = (uint)args[0];
		if(LogicUI != null)
			LogicUI.SetItemData((int)ItemIndex);
		LogicUI.Invoke("ClickLeaveScene",1.8f);
	}
}
