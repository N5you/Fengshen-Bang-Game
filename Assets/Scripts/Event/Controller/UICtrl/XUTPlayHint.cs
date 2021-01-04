using UnityEngine;
using XGame.Client.Packets;

class XUTPlayHint : XUICtrlTemplate<XPlayHint>
{
	public override void OnShow()
	{
		LogicUI.Level.text	= string.Format(XStringManager.SP.GetString(713),XPlayHintMgr.SP.PlayName,XPlayHintMgr.SP.NeedLevel);
		
		if(XPlayHintMgr.SP.QuestID > 0)
			LogicUI.Quest.text	= string.Format(XStringManager.SP.GetString(714),XPlayHintMgr.SP.QuestName);
		else
			LogicUI.Quest.text	= "";

		XUIDynamicAtlas.SP.SetSprite(LogicUI.PlaySprite,(int)XPlayHintMgr.SP.AtlasID,XPlayHintMgr.SP.SpriteName,true,null);
		
		return ;
	}
}
