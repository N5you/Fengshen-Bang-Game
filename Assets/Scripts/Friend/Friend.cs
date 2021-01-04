using System;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;
using UnityEngine;
public enum XFriendErrorCode
{
	EAddFriend_Success = 10,
	EAddFriend_NotFoundPlayer,
	EAddFriend_MaxFriend,
	EAddFriend_MaxFollower,
	EAddFriend_InBlackList,
	EAddFriend_IsFriend,
	EAddFriend_TargetLevelRequirements,

	EDelFriend_Success = 20,
	EDelFriend_NotFoundPlayer = 21,

	EPresentFlower_Success = 30,
	EPresentFlower_NotEnoughGameMoney = 31,
	EPresentFlower_NotEnoughRealMoney = 32,
	EPresentFlower_MaxPresent = 33,

	EGetMarry_Successs = 40,
	EGetMarry_NotAgree,
	EGetMarry_SameSex,
	EGetMarry_NotEnoughRealMoney,
	EGetMarry_SenderNotEnoughRealMoney,
	EGetMarry_NotEnoughFriendlyValue,
	EGetMarry_FriendNotEnoughFriendlyValue,
	EGetMarry_NotOnline,
	EGetMarry_Married,
	EGetMarry_NotReciverFriend,
	EGetMarry_ErrorUID,

	EMoveToBlackList_Success = 100,
}

public class XFriendPlayer
{
	public ulong UID;
	public string Name;
	public uint Level;
	public ushort Sex;
	public byte Relation;
	public uint FriendlyValue;
	public uint Status;
	public string Signature;

	public XFriendPlayer ()
	{
		this.UID = 0;
		this.Name = "";
		this.Level = 0;
		this.Sex = 0;
		this.Relation = 0;
		this.FriendlyValue = 0;
		this.Status = 0;
		this.Signature = "";
	}

	public XFriendPlayer (ulong uid, string name, uint level, ushort sex, byte relation, uint friendlyValue, uint status, string signature)
	{
		this.UID = uid;
		this.Name = name;
		this.Level = level;
		this.Sex = sex;
		this.Relation = relation;
		this.FriendlyValue = friendlyValue;
		this.Status = status;
		this.Signature = signature;
	}
}

public class XFriendHouse
{
	public ulong UID;
	public string PresentName;
	public uint Flowers;
	public ulong ReciveTime;

	public XFriendHouse ()
	{
		this.UID = 0;
		this.PresentName = "";
		this.Flowers = 0;
		this.ReciveTime = 0;
	}

	public XFriendHouse (ulong uid, string presentName, uint flowers, ulong reciveTime)
	{
		this.UID = uid;
		this.PresentName = presentName;
		this.Flowers = flowers;
		this.ReciveTime = reciveTime;
	}
}

public class XFriendManager : XSingleton<XFriendManager>, IComparer<XFriendHouse>
{
	//全局数据
	public static readonly uint S_FRIEND_RATIO_BASIC_NUM = 50;
	public static readonly uint S_MAX_FRIEND_USER_NUM = 150;
	public static readonly uint S_MAX_BLACK_USER_NUM = 50;
	public static readonly uint S_MAX_HOUSEINFO_DATA_NUM = 50;
	public static readonly uint S_PRESENT_FLOWER_COST_REALMONEY = 5;
	public static readonly uint S_PRESENT_FLOWER_COST_GAMEMONEY = 500;
	public static readonly uint S_GETMARRY_COST_REALMONEY = 50;
	public static readonly uint S_PRESENT_FLOWER_RM_ADD_FRIENDLYVALUE = 10;
	public static readonly uint S_PRESENT_FLOWER_GM_ADD_FRIENDLYVALUE = 2;

	//好友列表
	private SortedDictionary<ulong, XFriendPlayer> m_friendList;
	private SortedDictionary<ulong, XFriendPlayer> m_blackList;

	//收到鲜花记录
	private List<XFriendHouse> m_houseRecordList;

	//每日赠送次数
	private ushort m_everyDayPresentCount;

	public ushort EveryDayPresentCount {
		get { return this.m_everyDayPresentCount; }
		private set { m_everyDayPresentCount = value; }
	}

	//鲜花数
	private uint m_myFlowers;

	public uint Myflowers {
		get { return this.m_myFlowers; }
		private set { m_myFlowers = value; }
	}

	//心情签名
	private string m_mySignature;

	public string MySignature {
		get { return this.m_mySignature; }
		private set { m_mySignature = value; }
	}

	//喜结连理对象ID
	private ulong m_marryID;

	public ulong MarryID {
		private set { m_marryID = value; }
		get { return m_marryID; }
	}

	private List<ulong> m_presentObject;

	public XFriendManager ()
	{
		m_friendList = new SortedDictionary<ulong, XFriendPlayer> ();
		m_blackList = new SortedDictionary<ulong, XFriendPlayer> ();
		m_houseRecordList = new List<XFriendHouse> ();
		m_presentObject = new List<ulong> ();
		m_mySignature = "";
		m_myFlowers = 0;
		m_everyDayPresentCount = 0;
		m_marryID = 0;
	}

	~XFriendManager ()
	{
		m_friendList.Clear ();
		m_blackList.Clear ();
		m_houseRecordList.Clear ();
	}

    #region logic

	public void HandleSetSignature(string text)
	{
		//判断是否存在违规的字符，使用屏蔽字库接口，暂时为真
		bool isNormalChar = true;
		if (isNormalChar) {
			CS_Friend_SetSignature.Builder msg = CS_Friend_SetSignature.CreateBuilder ();
			msg.SText = text;
			XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_SetSignature, msg.Build ());
			this.SetSignature (text);
		} else {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10029);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, XStringManager.SP.GetString (94));
		}
	}

	public void HandleAddFriend(string name)
	{
		uint nameStrLen = 2;
		if(name.Length < nameStrLen)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10008,nameStrLen);
			return;
		}
		if (name == XLogicWorld.SP.MainPlayer.Name) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10009);
			return;
		}
		FeatureUnLock unlockcfg = FeatureUnLockMgr.SP.GetConfig(8);
		if(unlockcfg == null)
			Debug.Log("Friend, unlockcfg is null");
		if(XLogicWorld.SP.MainPlayer.Level < unlockcfg.RequireLevel)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10007,unlockcfg.RequireLevel);
			return;
		}
		if (GetFriendList ().Count >= XVipManager.SP.GetVipAttri(EVipConst.eVip_FriMax)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10013);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (93)));
			return;
		}
		if (InFriendList (name)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10022);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (116)));
			return;
		}
		if (InBlackList (name)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10014);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (114)));
			return;
		}

		CS_Friend_TargetName.Builder msg = CS_Friend_TargetName.CreateBuilder ();
		msg.SetTargetName (name);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_AddFriend, msg.Build ());
	}

	public void HandleDelFriend(ulong playerID, string playerName)
	{
		if(!m_friendList.ContainsKey(playerID))
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10024);
			return;
		}
		if (this.MarryID == playerID) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10023);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (131)));
			return;
		}

		CS_Friend_TargetID.Builder msg = new CS_Friend_TargetID.Builder ();
		msg.SetTargetID (playerID);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_DelFriend, msg.Build ());
	}

	public void HandleAddBlackPlayer(ulong targetID, string targetName)
	{
		if (this.m_blackList.ContainsKey (targetID)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10026);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (114)));
			return;
		}

		if (this.MarryID == targetID) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10025);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (137)));
			return;
		}

		if (this.m_blackList.Count >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BlaMax)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10027);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (115)));
			return;
		}

		CS_Friend_AddToBlackList.Builder msg = CS_Friend_AddToBlackList.CreateBuilder ();
		msg.TargetId = targetID;
		msg.TargetName = targetName;
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_MoveToBlack, msg.Build ());
	}

	public void HandleRemoveBlackPlayer(ulong taregetID)
	{
		if (!this.m_blackList.ContainsKey (taregetID)) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10028);
			return;
		}

		CS_Friend_TargetID.Builder msg = new CS_Friend_TargetID.Builder ();
		msg.SetTargetID (taregetID);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_DelBlackUser, msg.Build ());
	}
    #endregion

	public bool IsMarry()
	{
		if (m_marryID != 0)
			return true;
		else
			return false;
	}

	public bool InFriendList(string name)
	{
		foreach (XFriendPlayer s in this.m_friendList.Values) {
			if (s.Name == name)
				return true;
		}
		return false;
	}

	public bool InBlackList(string name)
	{
		foreach (XFriendPlayer s in this.m_blackList.Values) {
			if (s.Name == name)
				return true;
		}
		return false;
	}

	//获取好友数据列表
	public SortedDictionary<ulong, XFriendPlayer> GetFriendList()
	{
		if (this.m_friendList == null) {
			Log.Write ("获取好友管理器中好友数据出错");
			return null;
		}
		return this.m_friendList;
	}

	public XFriendPlayer GetFriendPlayer(ulong ID)
	{
		XFriendPlayer tempPlayer;
		if(m_friendList.TryGetValue(ID,out tempPlayer))
			return tempPlayer;
		else
			return null;
	}

	//获取黑名单数据列表
	public SortedDictionary<ulong, XFriendPlayer> GetBlackList()
	{
		if (this.m_blackList == null) {
			Log.Write ("GetBlackList Wrong");
			return null;
		}
		return this.m_blackList;
	}

	public XFriendPlayer GetBlackPlayer(ulong ID)
	{
		XFriendPlayer tempPlayer;
		if(m_friendList.TryGetValue(ID,out tempPlayer))
			return tempPlayer;
		else
			return null;
	}

	//获得最近50条收到鲜花的数量
	public List<XFriendHouse> GetHouseRecordList()
	{
		if (this.m_houseRecordList == null) {
			Log.Write ("获得收到鲜花的日志数据出错！");
			return null;
		}
		return this.m_houseRecordList;
	}
	public List<ulong> GetPresentObjectList()
	{
		if (this.m_presentObject == null) {
			Log.Write ("m_presentObject Wrong");
			return null;
		}
		return this.m_presentObject;
	}

	public void HandlePresentFlowerN(ulong targeterID, uint num)
	{
		if (XLogicWorld.SP.MainPlayer.RealMoney <= XFriendManager.S_PRESENT_FLOWER_COST_REALMONEY) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10004,XLogicWorld.SP.MainPlayer.RealMoney.ToString());
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
			return;
		}

		if(XFriendManager.SP.GetPresentObjectList().Contains(targeterID))
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10002);
			return;
		}
		
		if (XFriendManager.SP.EveryDayPresentCount >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyGivefowCount)) {
			
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10003,XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyGivefowCount).ToString());
			//XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, (uint)XFriendErrorCode.EPresentFlower_MaxPresent);
			return;
		}
		
		ulong _uid = targeterID;
		uint _num = num;
		CS_Friend_PresentFlowers.Builder msg = CS_Friend_PresentFlowers.CreateBuilder ();
		msg.SetTargetID (_uid);
		msg.SetTargetNum (_num);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_PresentFlower, msg.Build ());
	}
	public void HandlePresentFlower(ulong targeterID, uint num)
	{
		if (XLogicWorld.SP.MainPlayer.GameMoney <= XFriendManager.S_PRESENT_FLOWER_COST_GAMEMONEY) {
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10005);
			return;
		}

		if(XFriendManager.SP.GetPresentObjectList().Contains(targeterID))
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10002);
			return;
		}

		if (XFriendManager.SP.EveryDayPresentCount >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyGivefowCount)) {

			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10003,XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyGivefowCount).ToString());
			//XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, (uint)XFriendErrorCode.EPresentFlower_MaxPresent);
			return;
		}

		ulong _uid = targeterID;
		uint _num = num;
		CS_Friend_PresentFlowers.Builder msg = CS_Friend_PresentFlowers.CreateBuilder ();
		msg.SetTargetID (_uid);
		msg.SetTargetNum (_num);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_Friend_PresentFlower, msg.Build ());
	}

	public void HandleGetMarry(XFriendPlayer fPlayer)
	{
		if (fPlayer != null) {
			if (m_friendList.ContainsKey (fPlayer.UID)) {
				//判断对方是否已婚

			} else {
				Log.Write (LogLevel.ERROR, "HandleGetMarry, m_friendList isn't Contains fPlayer UID");
			}
		} else {
			Log.Write (LogLevel.ERROR, "HandleGetMarry, fPlayer is Null");
		}
	}

	public void SetSignature(string name)
	{
		if (this.m_mySignature != name)
			this.m_mySignature = name;
	}

	private void RemoveBlackUser(ulong uid)
	{
		if (this.m_blackList.ContainsKey (uid)) {
			this.m_blackList.Remove (uid);
			XEventManager.SP.SendEvent (EEvent.Friend_RemoveBlack, uid);
		}
	}

	private void AddBlackPlayer(XFriendPlayer fPlayer)
	{
		if (fPlayer == null)
			return;
		if (this.m_friendList.ContainsKey (fPlayer.UID)) {
			this.m_friendList.Remove (fPlayer.UID);
			this.m_blackList.Add (fPlayer.UID, fPlayer);
			XEventManager.SP.SendEvent (EEvent.Friend_DelFriend, fPlayer.UID);
			XEventManager.SP.SendEvent (EEvent.Friend_MoveToBlackList, fPlayer.UID, fPlayer.Name);
		} else {
			this.m_blackList.Add (fPlayer.UID, fPlayer);
			XEventManager.SP.SendEvent (EEvent.Friend_MoveToBlackList, fPlayer.UID);
		}
	}

	private void AddFriend(ulong uid, XFriendPlayer fplayer)
	{
		if (fplayer != null && fplayer.UID == uid) {
			if (this.m_friendList.Count < S_MAX_FRIEND_USER_NUM) {
				if (!this.m_friendList.ContainsKey (uid)) {
					this.m_friendList.Add (uid, fplayer);
					XEventManager.SP.SendEvent(EEvent.Friend_AddFriend,uid);
				}
			}
		} else {
			Log.Write (LogLevel.ERROR, "Friend.cs addFriend, fplayer is null or fplayer's uid not equal uid");
		}
	}

	private void DelFriend(ulong uid)
	{
		if (this.m_friendList.ContainsKey (uid)) {
			string name = this.m_friendList [uid].Name;
			this.m_friendList.Remove (uid);
			XEventManager.SP.SendEvent (EEvent.Friend_DelFriend, uid);
			XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (103), name));
		}
	}

	private void UpdateReciveFlowerRecord(ulong uid, string name, uint flowers, ulong reciveTime)
	{
		XFriendHouse data = new XFriendHouse (uid, name, flowers, reciveTime);
		if (data != null) {
			if (m_houseRecordList.Count < S_MAX_HOUSEINFO_DATA_NUM) {
				this.m_houseRecordList.Add (data);
				if (m_houseRecordList.Count > 0) {
					Comparison<XFriendHouse> sorter = new Comparison<XFriendHouse> (this.Compare);
					this.m_houseRecordList.Sort (sorter);
				}
			} else {
				this.m_houseRecordList.RemoveAt ((int)S_MAX_HOUSEINFO_DATA_NUM - 1);
				Comparison<XFriendHouse> sorter = new Comparison<XFriendHouse> (this.Compare);
				this.m_houseRecordList.Sort (sorter);
				this.m_houseRecordList.Add (data);
			}
		} else {
			Log.Write (LogLevel.ERROR, "XFriendManager,UpdateReciveFlowerRecord, The Argument is Null!");
		}
	}

	private void UpdateLevel(ulong uid, uint level)
	{
		this.m_friendList [uid].Level = level;
		XEventManager.SP.SendEvent (EEvent.Friend_UpdateLevel, uid, level);
	}

	private void UpdateSignature(ulong uid, string signature)
	{
		this.m_friendList [uid].Signature = signature;
		XEventManager.SP.SendEvent (EEvent.Friend_UpdateSignature, uid, signature);
	}

	private void UpdateOnlineStatus(ulong uid, uint onlineStatus)
	{
		this.m_friendList [uid].Status = onlineStatus;
		XEventManager.SP.SendEvent (EEvent.Friend_UpdateOnlineStatus, uid, onlineStatus);
	}

	private void addPresetnObject(ulong uid)
	{
		if (!m_presentObject.Contains (uid))
		{
			m_presentObject.Add (uid);
			Debug.Log ("addPresentObject, addnew PresentObject uid:" + uid);
		}
		else
		{
			Debug.Log ("addPresentObject, presentObjectList already has uid:" + uid);
		}
	}


	public void ON_SC_Clinet_ReciveInitAllData(SC_Friend_AllFriendData msg)
	{
		if (msg.UserInfoCount > 0) {
			foreach (SC_Friend_UserInfo s in msg.UserInfoList) {
				XFriendPlayer tempPlayer = new XFriendPlayer ();
				tempPlayer.UID = s.UID;
				tempPlayer.Level = s.Level;
				tempPlayer.Sex = (ushort)s.Sex;
				tempPlayer.Relation = (byte)s.Relation;
				tempPlayer.FriendlyValue = s.FriendlyValue;
				tempPlayer.Name = s.Name;
				tempPlayer.Signature = s.Singnature;
				tempPlayer.Status = s.Status;
				if (s.Relation == 0) {
					if (!m_friendList.ContainsKey (tempPlayer.UID)) {
						m_friendList.Add (tempPlayer.UID, tempPlayer);
					}
				} else {
					if (!m_friendList.ContainsKey (tempPlayer.UID)) {
						m_blackList.Add (tempPlayer.UID, tempPlayer);
					}
				}
			}
		}
		if (msg.HouseInfoCount > 0) {
			foreach (SC_Friend_FlowerHouseInfo s in msg.HouseInfoList) {
				XFriendHouse houseInfo = new XFriendHouse ();
				this.UpdateReciveFlowerRecord (s.UID, s.Name, s.Flowers, s.ReciveTime);
			}
		}

		this.m_myFlowers = msg.Flowers;
		this.m_everyDayPresentCount = (ushort)msg.PresentCount;
		this.SetSignature (msg.Signature);
		if (msg.HasMarryID)
			this.m_marryID = msg.MarryID;
	}

	public void ON_SC_Clinet_RecivePresentFlowerResualt(SC_Friend_ErrorCode msgData)
	{
		uint errResualt = msgData.Code;
		if (errResualt == (uint)XFriendErrorCode.EPresentFlower_Success)
			++m_everyDayPresentCount;
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, errResualt);
	}

	public void ON_SC_Client_ReciveAddFriend(SC_Friend_UserInfo msgData)
	{
		uint errResualt = (uint)XFriendErrorCode.EAddFriend_Success;
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, errResualt);
		if (msgData.HasUID) {
			//获得添加好友数据
			XFriendPlayer tempPlayer = new XFriendPlayer ();
			tempPlayer.UID = msgData.UID;
			tempPlayer.Name = msgData.Name;
			tempPlayer.Level = msgData.Level;
			tempPlayer.Sex = (ushort)msgData.Sex;
			tempPlayer.Signature = msgData.Singnature;
			tempPlayer.Status = msgData.Status;
			tempPlayer.FriendlyValue = msgData.FriendlyValue;
			this.AddFriend (tempPlayer.UID, tempPlayer);
		}
	}

	public void ON_SC_Client_ReciveDelFriend(ulong UID)
	{
		this.DelFriend (UID);
	}

	public void ON_SC_Client_ReciveDelBlack(ulong UID)
	{
		this.RemoveBlackUser (UID);
	}

	public void ON_SC_Client_ReciveAddToBlackList(XFriendPlayer fPlayer)
	{
		this.AddBlackPlayer (fPlayer);
	}

	public void ON_SC_Client_ReciveErrorCode(uint code)
	{
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, code);
	}
 
	public void ON_SC_Client_ReciveGetMarryAgree(ulong senderID)
	{
		if (m_friendList.ContainsKey (senderID)) {
			string senderName = m_friendList [senderID].Name;
			XEventManager.SP.SendEvent (EEvent.Friend_GetMarryAgree, senderName, senderID);
			return;
		}
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, (uint)XFriendErrorCode.EGetMarry_NotReciverFriend);
	}

	public void ON_SC_Client_ReciveGetMarryResualt(bool isSender, ulong senderID, ulong reciverID)
	{
		if (isSender) {
			m_marryID = reciverID;
			string reciverName = m_friendList [reciverID].Name;
			XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (129), reciverName));
		} else {
			m_marryID = senderID;
			string senderName = m_friendList [senderID].Name;
			XEventManager.SP.SendEvent (EEvent.MessageBox, null, null, string.Format (XStringManager.SP.GetString (129), senderName));
		}
	}

	public void ON_SC_Client_ReciveDivorceToReciver(ulong senderID)
	{
		if (senderID == this.MarryID) {
			if (m_friendList.ContainsKey (senderID)) {
				this.MarryID = 0;
				string senderName = m_friendList [senderID].Name;
				XEventManager.SP.SendEvent (EEvent.Friend_DivorceRecivedTip, senderName, senderID);
				return;
			}
		}
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, (uint)XFriendErrorCode.EGetMarry_ErrorUID);
	}

	public void ON_SC_Client_ReciveDivorceResualt(ulong targetID)
	{
		if (targetID == this.MarryID) {
			if (m_friendList.ContainsKey (targetID)) {
				this.MarryID = 0;
				string targetName = m_friendList [targetID].Name;
				XEventManager.SP.SendEvent (EEvent.Friend_DivorceResualt, targetName, targetID);
				return;
			}
		}
		XEventManager.SP.SendEvent (EEvent.Freind_ErrCode, (uint)XFriendErrorCode.EGetMarry_ErrorUID);
	}

	public void ON_SC_Client_ReciveUpdateLevel(ulong uid, uint level)
	{
		if (m_friendList.ContainsKey (uid)) {
			this.UpdateLevel (uid, level);
		}
	}

	public void ON_SC_Client_ReciveUpdateSignature(ulong uid, string contents)
	{
		if (m_friendList.ContainsKey (uid)) {
			this.UpdateSignature (uid, contents);
		}
	}

	public void ON_SC_Client_ReciveUpdateOnlineStatus(ulong uid, uint status)
	{
		if (m_friendList.ContainsKey (uid)) {
			this.UpdateOnlineStatus (uid, status);
		}
	}

	public void ON_SC_Client_ReciveUpdatePresentCount(uint count)
	{
		if (m_everyDayPresentCount != 0) {
			EveryDayPresentCount = (ushort)count;
		}
	}

	public void ON_SC_Client_ReciveRefreshPresentObject()
	{
		if (m_presentObject.Count > 0)
		{
			m_presentObject.Clear ();
		}
	}

	public void ON_SC_Client_AddPresentObject(ulong targetID)
	{
		addPresetnObject (targetID);
	}

	public void ON_SC_Client_ReciveUpdateHouseInfo(ulong uid, string name, uint flowers, ulong reciveTime)
	{
		if (m_houseRecordList != null) {
			this.UpdateReciveFlowerRecord (uid, name, flowers, reciveTime);
			this.Myflowers += flowers;
		}
	}

	string noticeAddFriendName = "";
	public void ON_SC_Client_ReciveAddFriendTip(ulong uid, string name)
	{
		if (!m_friendList.ContainsKey (uid)) {
			//XEventManager.SP.SendEvent (EEvent.Friend_AddFriendTip, uid, name);
			noticeAddFriendName = name;
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (MessageBox_NoticeAddFriend);
			XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, string.Format (XStringManager.SP.GetString (135), noticeAddFriendName));
		}
	}
	private void MessageBox_NoticeAddFriend(GameObject go)
	{
		XFriendManager.SP.HandleAddFriend (noticeAddFriendName);
	}


    #region IComparer[XFriendHouse] implementation
	public int Compare(XFriendHouse x, XFriendHouse y)
	{
		if (x is XFriendHouse && y is XFriendHouse) {
			if (x.ReciveTime > y.ReciveTime) {
				return -1;
			} else if (x.ReciveTime < y.ReciveTime) {
					return 1;
				}
			return 0;
		}
		throw new NotImplementedException ();
	}
    #endregion
}
