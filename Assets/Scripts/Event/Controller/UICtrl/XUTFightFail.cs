using System;
using XGame.Client.Packets;

class XUTFightFail : XUICtrlTemplate<XFightFail>
{	
	private EBattleResultFlag	mFlag;
	
	private string Lose1SpriteName = "11008002";
//	private string Lose2SpriteName = "11008003";
//	private string Lose3SpriteName = "11008004";
//	private string Lose4SpriteName = "11008005";
	
    public XUTFightFail()
    {
		XEventManager.SP.AddHandler(FailLevelSet,EEvent.CopySceneResult_FailLevel);
    }
	
	public void FailLevelSet(EEvent evt, params object[] args)
	{
		mFlag = (EBattleResultFlag)args[0];
	}

    public override void OnCreated(object arg)
    {
        base.OnCreated(arg);
    }	
	
	public override void OnShow()
	{
		base.OnShow();
		
		
//		if(mFlag == EBattleResultFlag.ebrf_Lose1)
//			LogicUI.ResultFont.spriteName	= Lose1SpriteName;
//		else if(mFlag == EBattleResultFlag.ebrf_Lose2)
//			LogicUI.ResultFont.spriteName	= Lose2SpriteName;
//		else if(mFlag == EBattleResultFlag.ebrf_Lose3)
//			LogicUI.ResultFont.spriteName	= Lose3SpriteName;
//		else if(mFlag == EBattleResultFlag.ebrf_Lose4)
//			LogicUI.ResultFont.spriteName	= Lose4SpriteName;
		
	}
	public override void OnHide()
	{
		base.OnHide();
	}
}
