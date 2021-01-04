using UnityEngine;
class XUTTargetInfo : XUICtrlTemplate<XTargetInfo>
{
	private bool IsInit;
	private string mName;
	private int mLevel;
	private int mCombatPower;
	private ulong mUID;
	
	public XUTTargetInfo ()
	{
		XEventManager.SP.AddHandler (XSelectDataHandler, EEvent.ObjSel_SetData);
		RegEventAgent_CheckCreated (EEvent.TargetInfo_ChatPrivate, OnClickChatPrivate);
		RegEventAgent_CheckCreated (EEvent.TargetInfo_AddFriend, OnClickAddFriend);
		RegEventAgent_CheckCreated (EEvent.TargetInfo_LookInfo, OnClickLookInfo);
	}
	
	public void XSelectDataHandler(EEvent evt, params object[] args)
	{
		mName = (string)args [0];
		mLevel = (int)args [1];
		mUID = (ulong)args [2];
	}
	
	public override void OnShow()
	{
		base.OnShow ();
		if (LogicUI != null) {
			LogicUI.SetName (mName);
			LogicUI.SetLevel (string.Format ("{0}", mLevel));
			LogicUI.SetCombatPower (""); //string.Format("{0}", mCombatPower);
		}
		else {
			Log.Write (LogLevel.ERROR, "XUTTargetInfo, LogicUI is nulL");
		}
		ShowHead();
	}
	
	public void ShowHead()
	{
		if (!IsInit) {
			IsInit = true;
			XPlayer tempOtherPlayer = XLogicWorld.SP.ObjectManager.GetObject<XPlayer> (EObjectType.OtherPlayer, mUID);
			if (tempOtherPlayer != null) {
				XModelRTTMgr.SP.AddModelRTT (tempOtherPlayer.ModelId, LogicUI.RoleHeadTex, 0f, -1.8f, 1f);
			}else
			{
				Log.Write (LogLevel.ERROR, "XUTTargetInfo, tempOtherPlayer is nulL");
			}
		}
	}
	
	public void OnClickChatPrivate(EEvent evt, params object[] args)
	{
		if (mUID != 0)
			XEventManager.SP.SendEvent (EEvent.Chat_OpenPrivate, mName, mUID);
	}
	
	public void OnClickAddFriend(EEvent evt, params object[] args)
	{
		if (mUID != 0)
			XFriendManager.SP.HandleAddFriend (mName);
	}
	
	public void OnClickLookInfo(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent (EEvent.Chat_ShowPlayerInfoReq, mName);
	}
}
