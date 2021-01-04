using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTFriendCharmRank : XUICtrlTemplate<XUIFriendCharmRank>
{	 
	public XUTFriendCharmRank()
	{
		RegEventAgent_CheckCreated(EEvent.Friend_UpdateRankInfo, OnUpdateInfo);
	}
	
	public void OnUpdateInfo(EEvent evt, params object[] args)
	{
		if(LogicUI!= null)
		{
			LogicUI.UpdateInfo();
		}else{
			Log.Write(LogLevel.ERROR,"XUTFriendCharmRank, OnUpdateInfo, the logicUI is null");
		}
	}
}
