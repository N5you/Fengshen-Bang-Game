using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTPVPResult : XUICtrlTemplate<XPVPResult>
{	
	private uint 				mGameMoney;
	private uint 				mHonour;
	private EBattleResultFlag	mFlag;
	private bool				mIsWin;
	
	private static string[] strWinSprite = new string[4]{"11007004","11007003","11007002","11007001"};
	
	private string Lose1SpriteName = "11008002";
	private string Lose2SpriteName = "11008003";
	private string Lose3SpriteName = "11008004";
	private string Lose4SpriteName = "11008005";
	
	public XUTPVPResult()
	{
		RegEventAgent_CheckCreated(EEvent.PVPResult_Reset, PVPResetHandler);
		//RegEventAgent_CheckCreated(EEvent.PVPResult_Set, PVPSetDataHandler);
		XEventManager.SP.AddHandler(PVPSetDataHandler,EEvent.PVPResult_Set);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		LogicUI.MoneyLabel.text	= mGameMoney.ToString();
		LogicUI.HounourLabel.text	= mHonour.ToString();
		
		if(mIsWin)
			LogicUI.Sprite.spriteName	= strWinSprite[(int)mFlag];
		else
		{
			if(mFlag == EBattleResultFlag.ebrf_Lose1)
			LogicUI.Sprite.spriteName	= Lose1SpriteName;
		else if(mFlag == EBattleResultFlag.ebrf_Lose2)
			LogicUI.Sprite.spriteName	= Lose2SpriteName;
		else if(mFlag == EBattleResultFlag.ebrf_Lose3)
			LogicUI.Sprite.spriteName	= Lose3SpriteName;
		else if(mFlag == EBattleResultFlag.ebrf_Lose4)
			LogicUI.Sprite.spriteName	= Lose4SpriteName;
		}
	}
	
	public void PVPResetHandler(EEvent evt, params object[] args)
	{
		mGameMoney	= 0;
		mHonour		= 0;
		mFlag		= EBattleResultFlag.ebrf_Lose1;
		mIsWin		= false;
	}
	
	public void PVPSetDataHandler(EEvent evt, params object[] args)
	{
		mFlag		= (EBattleResultFlag)args[0];
		mGameMoney	= (uint)args[1];
		mHonour		= (uint)args[2];
		mIsWin		= (bool)args[3];
	}
}


