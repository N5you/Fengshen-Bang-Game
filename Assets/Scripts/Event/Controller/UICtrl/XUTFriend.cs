using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
class XUTFriend : XUICtrlTemplate<XUIFriend>
{
	public XUTFriend ()
	{
		XEventManager.SP.AddHandler (UIMessage, EEvent.Freind_ErrCode);
		RegEventAgent_CheckCreated (EEvent.Friend_AddFriend,AddFriendUIContent);
		RegEventAgent_CheckCreated (EEvent.Freind_ChatAddFriend,ChatAddFriend);
		RegEventAgent_CheckCreated (EEvent.Friend_DelFriend,DelFriendUIContent);
		RegEventAgent_CheckCreated (EEvent.Friend_RemoveBlack,DelBlackUIContent);
		RegEventAgent_CheckCreated (EEvent.Friend_MoveToBlackList,AddBlackUIContent);
		XEventManager.SP.AddHandler (getMarryRequestTips, EEvent.Friend_GetMarryAgree);
		XEventManager.SP.AddHandler (reciveDivorceTips, EEvent.Friend_DivorceRecivedTip);
		XEventManager.SP.AddHandler (reciveDivorceResualt, EEvent.Friend_DivorceResualt);
		XEventManager.SP.AddHandler (NoticeAddFriend, EEvent.Friend_AddFriendTip);
		XEventManager.SP.AddHandler (updateOnlineStatus, EEvent.Friend_UpdateOnlineStatus);
		XEventManager.SP.AddHandler (updateSignature, EEvent.Friend_UpdateSignature);
		XEventManager.SP.AddHandler (updateLevel, EEvent.Friend_UpdateLevel);
		XEventManager.SP.AddHandler (HandleAddFriend, EEvent.Friend_AddFriendBtn);
		XEventManager.SP.AddHandler (HandleDelFriend, EEvent.Friend_DelFriendBtn);
		XEventManager.SP.AddHandler (presentFlower, EEvent.Friend_PresentFlowerBtn);
		XEventManager.SP.AddHandler (HandleGetMarry, EEvent.Friend_GetMarryBtn);
		XEventManager.SP.AddHandler (submitDivorce, EEvent.Friend_DivorceBtn);
		XEventManager.SP.AddHandler (HandleAddBlackPlayer, EEvent.Friend_MoveToBlackListBtn);
		XEventManager.SP.AddHandler (HandleRemoveBlackPlayer, EEvent.Friend_DelBlackBtn);
		XEventManager.SP.AddHandler (HandleSetSignature, EEvent.Friend_SetSignature);
	}

	public override void OnShow()
	{
		base.OnShow ();
		foreach (KeyValuePair<ulong,XFriendPlayer> s in XFriendManager.SP.GetFriendList()) {
			LogicUI.UpdateFriendItem (s.Value.UID);
		}

		foreach (KeyValuePair<ulong,XFriendPlayer> s in XFriendManager.SP.GetBlackList()) {
			LogicUI.UpdateBlackItem (s.Value.UID);
		}
	}

	//设置签名
	private void HandleSetSignature(EEvent evt, params object[] args)
	{
		string SignatureText = (string)args [0];
		XFriendManager.SP.HandleSetSignature (SignatureText);
	}

    #region Add&&DEL Friend

	//提示框中添加好友
	private void HandleAddFriend(EEvent evt, params object[] args)
	{
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_HandleAddFriend);
		XEventManager.SP.SendEvent (EEvent.InputMessageBox, funcOK, null, XStringManager.SP.GetString (96), UIInputValidator.Validation.None, "");
	}

	private void MessageBox_HandleAddFriend(GameObject go)
	{
		XFriendManager.SP.HandleAddFriend (XInputMessageBox.Content);
	}

	//直接添加好友
	private void ChatAddFriend(EEvent evt, params object[] args)
	{
		if (!(args [0] is string)) {
			Log.Write (LogLevel.ERROR, "Chat AddFriend, args[0] is not a string type");
			return;
		}
		string name = (string)args [0];
		XFriendManager.SP.HandleAddFriend (name);
	}

	//提示添加好友
	string noticeAddFriendName = "";

	private void NoticeAddFriend(EEvent evt, params object[] args)
	{
		noticeAddFriendName = (string)args [1];
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_NoticeAddFriend);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (135), noticeAddFriendName));
	}

	private void MessageBox_NoticeAddFriend(GameObject go)
	{
		XFriendManager.SP.HandleAddFriend (noticeAddFriendName);
	}

	//删除好友请求
	ulong m_delPlayerID = 0;
	string m_delPlayerName = "";

	private void HandleDelFriend(EEvent evt, params object[] args)
	{
		if (!(args [0] is ulong)) {
			Log.Write (LogLevel.ERROR, "delPlayerName, args[0] not a ulong type");
			return;
		}
		m_delPlayerID = (ulong)args [0];
		m_delPlayerName = (string)args [1];

		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_HandleDelFriend);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (104), m_delPlayerName));
	}

	private void MessageBox_HandleDelFriend(GameObject go)
	{
		XFriendManager.SP.HandleDelFriend (m_delPlayerID, m_delPlayerName);
	}


    #endregion

    #region updateData
	private void updateOnlineStatus(EEvent evt, params object[] args)
	{
		if (LogicUI != null) {
			LogicUI.UpdateFriendItem ((ulong)args [0]);
		}
	}

	private void updateSignature(EEvent evt, params object[] args)
	{
		if (LogicUI != null) {
			LogicUI.UpdateFriendItem ((ulong)args [0]);
		}
	}

	private void updateLevel(EEvent evt, params object[] args)
	{
		if (LogicUI != null) {
			LogicUI.UpdateFriendItem ((ulong)args [0]);
		}
	}
    #endregion


    #region PresentFlower
	ulong m_targetPlayerID = 0;
	uint m_presentNum = 0;

	private void presentFlower(EEvent evt, params object[] args)
	{
		if (LogicUI.CurPlayerContent == null) {
			XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10021);
			//XEventManager.SP.SendEvent (EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format ("Please select one player!"));
			return;
		}

		m_targetPlayerID = (ulong)args [0];
		m_presentNum = (uint)args [1];
		if ((uint)args [1] == 99) {

			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (On_CS_PresentFlowerN);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (125), XFriendManager.S_PRESENT_FLOWER_COST_REALMONEY));
		} else {

			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (On_CS_PresentFlower);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (124), XFriendManager.S_PRESENT_FLOWER_COST_GAMEMONEY));
		}
	}

	private void On_CS_PresentFlower(GameObject go)
	{
		XFriendManager.SP.HandlePresentFlower (m_targetPlayerID, m_presentNum);
	}

	private void On_CS_PresentFlowerN(GameObject go)
	{
		XFriendManager.SP.HandlePresentFlowerN (m_targetPlayerID, m_presentNum);
	}
    #endregion

    #region BlackList
	ulong m_curBlackID = 0;
	string m_curBlackName = "";

	private void HandleAddBlackPlayer(EEvent evt, params object[] args)
	{
		m_curBlackID = (ulong)args [0];
		if (XFriendManager.SP.GetFriendList ().ContainsKey (m_curBlackID)) {	
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_HandleAddBlackPlayer);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString (138));
			return;
		}
		XFriendManager.SP.HandleAddBlackPlayer (m_curBlackID, m_curBlackName);
	}

	private void MessageBox_HandleAddBlackPlayer(GameObject go)
	{
		XFriendManager.SP.HandleAddBlackPlayer (m_curBlackID, m_curBlackName);
	}

	ulong m_delBlackPlayerID = 0;
	string m_delBlackPlayerName = "";

	private void HandleRemoveBlackPlayer(EEvent evt, params object[] args)
	{
		if (!(args [0] is ulong)) {
			Log.Write (LogLevel.ERROR, "delPlayerName, args[0] not a ulong type");
			return;
		}
		m_delBlackPlayerID = (ulong)args [0];
		m_delBlackPlayerName = (string)args [1];

		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_HandleRemoveBlackPlayer);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (104), m_delBlackPlayerName));
	}

	private void MessageBox_HandleRemoveBlackPlayer(GameObject go)
	{
		XFriendManager.SP.HandleRemoveBlackPlayer (m_delBlackPlayerID);
	}
    #endregion

    #region UI Logic
	private void AddFriendUIContent(EEvent evt, params object[] args)
	{
		if (LogicUI == null)
			return;
		ulong playerID = (ulong)args [0];
		LogicUI.UpdateFriendItem (playerID);
	}

	private void DelFriendUIContent(EEvent evt, params object[] args)
	{
		if (LogicUI == null)
			return;
		ulong delUID = (ulong)args [0];
		LogicUI.DelItem (delUID);
		LogicUI.CurPlayerContent = null;
	}

	private void AddBlackUIContent(EEvent evt, params object[] args)
	{
		if (LogicUI == null)
			return;
		ulong addUID = (ulong)args [0];
		string addString = (string)args [1];
		LogicUI.UpdateBlackItem (addUID);
		XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10010, addString);
		//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (117), addString));
	}

	private void DelBlackUIContent(EEvent evt, params object[] args)
	{
		if (LogicUI == null)
			return;
		ulong delUID = (ulong)args [0];
		LogicUI.DelBlackItem (delUID);
		XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10011, LogicUI.CurPlayerContent.Name);
		//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (112), LogicUI.CurPlayerContent.Name));
		LogicUI.CurPlayerContent = null;
	}
    #endregion

    #region GetMarry
	ulong m_senderID = 0;

	private void getMarryRequestTips(EEvent evt, params object[] args)
	{
		string senderName = (string)args [0];
		m_senderID = (ulong)args [1];
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (messageBox_GetMarryAgree);
		UIEventListener.VoidDelegate funcNo = new UIEventListener.VoidDelegate (messageBox_GetMarryNotAgree);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, funcNo, string.Format (XStringManager.SP.GetString (127), senderName));
	}

	private void messageBox_GetMarryAgree(GameObject go)
	{
		getMarryAgree (true, m_senderID);
	}

	private void messageBox_GetMarryNotAgree(GameObject go)
	{
		getMarryAgree (false, m_senderID);
	}

	private void getMarryAgree(bool agree, ulong reciverID)
	{
		if (reciverID == 0)
			Log.Write (LogLevel.ERROR, "getMarryAgree, reciver ID is 0");

		CS_Friend_MarryAgree.Builder msg = CS_Friend_MarryAgree.CreateBuilder ();
		if (agree)
			msg.SetAgree (1);
		else
			msg.SetAgree (0);
		msg.SetUserID (m_senderID);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_GetMarryAgree, msg.Build ());
	}

	private void reciveDivorceTips(EEvent evt, params object[] args)
	{
		string senderName = (string)args [0];
		ulong senderid = (ulong)args [1];
		LogicUI.UpdateFriendItem (senderid);
		XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (132), senderName));
	}

	private void reciveDivorceResualt(EEvent evt, params object[] args)
	{
		string targetrName = (string)args [0];
		ulong targetrid = (ulong)args [1];
		LogicUI.UpdateFriendItem (targetrid);
		XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (133), targetrName));
	}

	private void HandleGetMarry(EEvent evt, params object[] args)
	{
		if (!(args [0] is XFriendPlayer)) {
			Log.Write (LogLevel.ERROR, "submitMarry, args[0] not a XFriendPlayer type");
			return;
		}
		XFriendPlayer Fplayer = (XFriendPlayer)args [0];
		if (XFriendManager.SP.IsMarry ()) {
			this.ShowMessageBox (XFriendErrorCode.EGetMarry_Married);
			return;
		}

		if (Fplayer.Status == 0) {
			this.ShowMessageBox (XFriendErrorCode.EGetMarry_NotOnline);
			return;
		}

		if (XLogicWorld.SP.MainPlayer.RealMoney < XFriendManager.S_GETMARRY_COST_REALMONEY) {
			this.ShowMessageBox (XFriendErrorCode.EGetMarry_NotEnoughRealMoney);
			return;
		}

		if (XLogicWorld.SP.MainPlayer.Sex == LogicUI.CurPlayerContent.Sex) {
			this.ShowMessageBox (XFriendErrorCode.EGetMarry_SameSex);
			return;
		}

		if (Fplayer.FriendlyValue < 200) {
			Debug.Log (Fplayer.FriendlyValue);
			this.ShowMessageBox (XFriendErrorCode.EGetMarry_NotEnoughFriendlyValue);
			return;
		}
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (On_CS_GetMarryRequest);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (102), XFriendManager.S_GETMARRY_COST_REALMONEY.ToString (), Fplayer.Name));
	}

	private void On_CS_GetMarryRequest(GameObject go)
	{
		CS_Friend_TargetName.Builder msg = CS_Friend_TargetName.CreateBuilder ();
		msg.TargetName = LogicUI.CurPlayerContent.Name;
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_GetMarryRequest, msg.Build ());
	}

	ulong m_DivorceRequestId = 0;

	private void submitDivorce(EEvent evt, params object[] args)
	{
		string tName = (string)args [1];
		m_DivorceRequestId = (ulong)args [0];

		if (XFriendManager.SP.MarryID != m_DivorceRequestId) {
			XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10006);
			return;
		}
		

		if (XFriendManager.SP.IsMarry ()) {
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (On_CS_DivorceRequest);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (130), tName));
		}
	}

	private void On_CS_DivorceRequest(GameObject go)
	{
		divorce (m_DivorceRequestId);
	}

	private void divorce(ulong id)
	{
		CS_Friend_TargetID.Builder msg = CS_Friend_TargetID.CreateBuilder ();
		msg.SetTargetID (m_DivorceRequestId);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_DivorceRequest, msg.Build ());
	}


    #endregion

	private void UIMessage(EEvent evt, params object[] args)
	{
		if (args [0] is uint) {
			XFriendErrorCode errorCode = (XFriendErrorCode)((uint)args [0]);
			this.ShowMessageBox (errorCode);
		} else {
			Log.Write (LogLevel.ERROR, "ShowMessageBox params Need Covert to Unit");
		}

	}

	private void ShowMessageBox(XFriendErrorCode code)
	{
		string tips = "";
		string addPlayerName = "";
		switch (code) {
		//添加好友
			case XFriendErrorCode.EAddFriend_Success:
				addPlayerName = XInputMessageBox.Content;
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10012, addPlayerName);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (91), addPlayerName));
				break;
			case XFriendErrorCode.EAddFriend_NotFoundPlayer:
				UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_ReAddFriend);
				XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString (92));
				break;
			case XFriendErrorCode.EAddFriend_MaxFriend:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10013);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (93));
				break;
			case XFriendErrorCode.EAddFriend_InBlackList:
				addPlayerName = XInputMessageBox.Content;
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10014);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (114));
				break;
			case XFriendErrorCode.EAddFriend_IsFriend:
				tips = XStringManager.SP.GetString (94);
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EAddFriend_TargetLevelRequirements:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10015);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (141));
				break;
			case XFriendErrorCode.EAddFriend_MaxFollower:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10016);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (142));
				break;
		//赠送鲜花
			case XFriendErrorCode.EPresentFlower_Success:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10017);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (97));
				break;
			case XFriendErrorCode.EPresentFlower_NotEnoughGameMoney:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10018);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (99));
				break;
			case XFriendErrorCode.EPresentFlower_NotEnoughRealMoney:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10019);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (100));
				break;
			case XFriendErrorCode.EPresentFlower_MaxPresent:
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10020);
				//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (98));
				break;

		//喜结连理
			case XFriendErrorCode.EGetMarry_Successs:
				tips = string.Format (XStringManager.SP.GetString (106));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;

			case XFriendErrorCode.EGetMarry_SameSex:
				tips = string.Format (XStringManager.SP.GetString (105));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_NotEnoughRealMoney:
				tips = string.Format (XStringManager.SP.GetString (107), XLogicWorld.SP.MainPlayer.RealMoney);
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_SenderNotEnoughRealMoney:
				tips = string.Format (XStringManager.SP.GetString (144), XLogicWorld.SP.MainPlayer.RealMoney);
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_NotEnoughFriendlyValue:
				tips = string.Format (XStringManager.SP.GetString (108));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_FriendNotEnoughFriendlyValue:
				tips = string.Format (XStringManager.SP.GetString (143));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_NotOnline:
				tips = string.Format (XStringManager.SP.GetString (109));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_NotAgree:
				tips = string.Format (XStringManager.SP.GetString (110));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_Married:
				tips = string.Format (XStringManager.SP.GetString (111));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
		//其它
			case XFriendErrorCode.EGetMarry_ErrorUID:
				tips = string.Format (XStringManager.SP.GetString (128));
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, tips);
				break;
			case XFriendErrorCode.EGetMarry_NotReciverFriend:
				XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (136)));
				break;
		}
	}

	private void MessageBox_ReAddFriend(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.Friend_AddFriendBtn);
	}
}