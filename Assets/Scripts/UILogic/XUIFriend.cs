using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
[AddComponentMenu("UILogic/XUIFriend")]
public class XUIFriend : XDefaultFrame
{
	public class FriendContent
	{
		public UIImageButton FButton;
		private XUIFriend xui;
		private XFriendPlayer playerInfo;
		private UISprite iconStatus;
		private UILabel itemName;
		private UILabel itemLevel;
		private UILabel itemSignature;

		public void Init(XUIFriend ui, XFriendPlayer playerInfo, GameObject PreFriendBtn, GameObject FuncListGrid)
		{
			this.xui = ui;
			this.playerInfo = playerInfo;
			GameObject itemObject = GameObject.Instantiate (PreFriendBtn) as GameObject;
			itemObject.SetActive (true);
			itemObject.transform.parent = FuncListGrid.gameObject.transform;
			itemObject.transform.localPosition = new Vector3 (0.0f, 0.0f, 0f);
			itemName = itemObject.transform.Find ("Name").GetComponent<UILabel> ();
			itemName.text = playerInfo.Name;
			itemLevel = itemObject.transform.Find ("Level").GetComponent<UILabel> ();
			itemLevel.text = string.Format ("Lv{0}", playerInfo.Level);
			itemSignature = itemObject.transform.Find ("Signature").GetComponent<UILabel> ();
			itemSignature.text = string.Format (playerInfo.Signature);
			iconStatus = itemObject.transform.Find ("Status").GetComponent<UISprite> ();
			this.UpdateStatus ();

			//因为要考虑排序，但是U3D窗口中的排位规则是0在前面，要把非在线用户排后面，所以把用户状态标识反过来
			byte status = 0;
			if (playerInfo.Status == 0)
				status = 1;
			else
				status = 0;
			itemObject.name = string.Format ("{0}_{1}_{2}_FriendContent", status, playerInfo.Name, playerInfo.Level);

			this.FButton = itemObject.GetComponent<UIImageButton> ();
			UIEventListener fbListener = UIEventListener.Get (FButton.gameObject);
			fbListener.onSelect += this.onSelect;
			fbListener.onHover += this.onHover;
		}

		private void onSelect(GameObject go, bool isSelect)
		{
			if (isSelect) {
				this.xui.ShowBlackGather (false);
				this.xui.ShowRightContainer (true);
				this.xui.ShowFriendGather (true);
				if (XFriendManager.SP.MarryID != playerInfo.UID) {
					if (this.xui.RightFriendContainerBtn [6].gameObject.activeInHierarchy) {
						xui.RightFriendContainerBtn [6].gameObject.SetActive (false);
						xui.RightFriendContainerBtn [3].gameObject.SetActive (true);
					}
				} else {
					if (this.xui.RightFriendContainerBtn [3].gameObject.activeInHierarchy) {
						xui.RightFriendContainerBtn [3].gameObject.SetActive (false);
						xui.RightFriendContainerBtn [6].gameObject.SetActive (true);
					}
				}
				this.xui.CurPlayerContent = this.playerInfo;
			}
		}
		
		private void onHover(GameObject go, bool isHover)
		{
			if (isHover) {
				string name = string.Format ("{0}{1}", "[color=00FF00]", itemName.text);

				//友好度处理
				uint curLevelTotalValue = 0;
				string friendlyName = "";
				string friendRelation = "";
				bool isMaxFriendlyLevel = false;

				for (uint cnt = 1; cnt != XCfgFriendlyLevelMgr.SP.ItemTable.Count; ++cnt) {
					XCfgFriendlyLevel friendlyCfg = XCfgFriendlyLevelMgr.SP.GetConfig (cnt);
					if (friendlyCfg == null && cnt == 1) {
						Debug.LogError ("the XCfgFriendlyLevel not found config");
						return;
					}
					if (friendlyCfg == null)
						return;
					if (XCfgFriendlyLevelMgr.SP.ItemTable.Count == cnt) {
						if (playerInfo.FriendlyValue > friendlyCfg.FriendlyValue) {
							isMaxFriendlyLevel = true;
							curLevelTotalValue = friendlyCfg.FriendlyValue;
							break;
						}
					}
					if (playerInfo.FriendlyValue > friendlyCfg.FriendlyValue)
						continue;

					curLevelTotalValue = friendlyCfg.FriendlyValue;
					friendlyName = friendlyCfg.FriendlyName;
					break;
				}

				if (playerInfo.UID != XFriendManager.SP.MarryID)
					friendRelation = string.Format ("{0}{1}", XUIFriend.FRIENDRELATION [0], ":");
				else
					friendRelation = string.Format ("{0}{1}", XUIFriend.FRIENDRELATION [1], ":");
				string content = "";

				if (!isMaxFriendlyLevel) {
					content = string.Format ("{1}{2}{3}/{4}{6}{5}{0}", "\n", "[color=FFFFFF]", friendRelation, playerInfo.FriendlyValue, curLevelTotalValue, friendlyName, "   ");
				} else {
					content = string.Format ("{1}{2}{3}/{4}{0}", "\n", "[color=FFFFFF]", friendRelation, playerInfo.FriendlyValue, friendlyName);
				}

				//签名处理
				string tempSignature = itemSignature.text;
				if (tempSignature == "")
					tempSignature = "无签名";
				content = string.Format ("{0}{1}{2}", content, "[color=F89A4B]", tempSignature);

				XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipB);
				XEventManager.SP.SendEvent (EEvent.ToolTip_B, name, "", content);
			} else {
				XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
			}
		}

		public void UpdateStatus()
		{
			if (playerInfo.Status == 1) {

				if (playerInfo.Sex == 1)
					iconStatus.spriteName = "11000224";
				else
					iconStatus.spriteName = "11000222";

				if (playerInfo.UID == XFriendManager.SP.MarryID)
					iconStatus.spriteName = "11000280";
			} else {
				if (playerInfo.Sex == 2)
					iconStatus.spriteName = "11000225";
				else
					iconStatus.spriteName = "11000223";

				if (playerInfo.UID == XFriendManager.SP.MarryID)
					iconStatus.spriteName = "11000281";
			}
		}

		public void UpdateInfo(ulong playerID)
		{
			XFriendPlayer fplayer = XFriendManager.SP.GetFriendList () [playerID];
			this.itemLevel.text = string.Format ("Lv{0}", fplayer.Level);
			this.itemSignature.text = fplayer.Signature;
			UpdateStatus ();
			uint status = fplayer.Status;
			if (status == 0)
				status = 1;
			else
				status = 0;
			this.FButton.gameObject.name = string.Format ("{0}_{1}_{2}_FriendContent", status, fplayer.Name, fplayer.Level);
		}

		public void UpdateInfo(uint level)
		{
			this.itemLevel.text = string.Format ("Lv{0}", level);
		}

		public void UpdateInfo(string signature)
		{
			this.itemSignature.text = signature;
		}
	}

	public class BlackContent
	{
		private XUIFriend xui;
		private XFriendPlayer playerInfo;
		public GameObject FuncGatherBlack;
		public UIImageButton FButton;

		public void Init(XUIFriend ui, XFriendPlayer playerInfo, GameObject PreBlackBtn, GameObject FuncListGrid)
		{
			this.xui = ui;
			this.playerInfo = playerInfo;

			GameObject itemObject = GameObject.Instantiate (PreBlackBtn) as GameObject;
			itemObject.name = string.Format ("{0}_BlackContent", playerInfo.UID);
			itemObject.SetActive (true);
			itemObject.transform.parent = FuncListGrid.gameObject.transform;
			itemObject.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);

			itemObject.transform.Find ("Name").GetComponent<UILabel> ().text = playerInfo.Name;
			itemObject.transform.Find ("Level").GetComponent<UILabel> ().text = string.Format ("Lv{0}", playerInfo.Level);

			this.FButton = itemObject.GetComponent<UIImageButton> ();
			UIEventListener fbListener = UIEventListener.Get (FButton.gameObject);
			fbListener.onSelect += this.onSelect;
		}

		private void onSelect(GameObject go, bool isSelect)
		{
			if (isSelect) {
				this.xui.ShowFriendGather (false);
				this.xui.ShowRightContainer (true);
				this.xui.ShowBlackGather (true);
				this.xui.CurPlayerContent = this.playerInfo;
				Debug.Log ("current select:" + xui.CurPlayerContent.Name + "_" + xui.CurPlayerContent.UID);
			}
		}
	}

	public static readonly string[] FRIENDRELATION = new string[3] {
		"好友",
		"夫妻",
		"黑名"
	};

	//动态UIPrefabs
	public GameObject PreFriendBtn;
	public GameObject PreBlackBtn;
	public UIGrid[] FuncListGrid;
	//功能按钮
	public GameObject RightContainer;
	public GameObject FuncGatherFriend;
	public GameObject FuncGatherBlack;
	public UIImageButton[] RightFriendContainerBtn = new UIImageButton[8];
	public UIImageButton[] RightBlackContainerBtn = new UIImageButton[2];
	public UILabel MyFlowers;
	public UIInputSignature SignatureInput;
	public UICheckbox[] NavCheckBox = new UICheckbox[2];
	public UIImageButton AddFriendsBtn;
	public UIImageButton CharmRankBtn;
	public UIImageButton FlowreHouseBtn;
	private SortedList<ulong, FriendContent> myFriendContents = new SortedList<ulong, FriendContent> ();
	private SortedList<ulong, BlackContent> myBlackContents = new SortedList<ulong, BlackContent> ();

	//当前选择的用户
	public XFriendPlayer CurPlayerContent = new XFriendPlayer ();

	public override bool Init()
	{
		base.Init ();
		InitUI ();
		return true;
	}

	public override void Hide()
	{
		base.Hide ();
		ShowRightContainer (false);
		ShowBlackGather (false);
		ShowFriendGather (false);
		CurPlayerContent = null;
	}

	public override void Show()
	{
		base.Show ();
		this.MyFlowers.text = XFriendManager.SP.Myflowers.ToString ();
	}

	//  界面初始化
	public void InitUI()
	{
		UIEventListener listenToChat = UIEventListener.Get (RightFriendContainerBtn [0].gameObject);
		listenToChat.onClick += OnToChat;

		UIEventListener listenSendFlower = UIEventListener.Get (RightFriendContainerBtn [1].gameObject);
		listenSendFlower.onClick += OnClickSendFlower;
		listenSendFlower.onHover += OnHoverPresentFlower;

		UIEventListener listenSendFlowersN = UIEventListener.Get (RightFriendContainerBtn [2].gameObject);
		listenSendFlowersN.onClick += OnClickSendFlowerN;
		listenSendFlowersN.onHover += OnHoverPresentFlowerN;

		UIEventListener listenSendDiamond = UIEventListener.Get (RightFriendContainerBtn [3].gameObject);
		listenSendDiamond.onClick += OnClickSendDiamond;
		listenSendDiamond.onHover += OnHoverSendDiamond;

		UIEventListener listenLookInfo = UIEventListener.Get (RightFriendContainerBtn [4].gameObject);
		listenLookInfo.onClick += OnClickLookInfo;

		UIEventListener listenAddFrinedBtn = UIEventListener.Get (AddFriendsBtn.gameObject);
		listenAddFrinedBtn.onClick += OnClickAddFriend;

		UIEventListener listenDelFriend = UIEventListener.Get (RightFriendContainerBtn [5].gameObject);
		listenDelFriend.onClick += OnClickDelFriend;

		UIEventListener listenDivorce = UIEventListener.Get (RightFriendContainerBtn [6].gameObject);
		listenDivorce.onClick += OnClickDivorce;

		UIEventListener listenDelBlack = UIEventListener.Get (RightBlackContainerBtn [0].gameObject);
		listenDelBlack.onClick += OnClickRemoveInBlackList;

		UIEventListener listenCharmRankBtn = UIEventListener.Get (CharmRankBtn.gameObject);
		listenCharmRankBtn.onClick += OnClickCharmRankBtn;

		UIEventListener listenFlowerHouseBtn = UIEventListener.Get (FlowreHouseBtn.gameObject);
		listenFlowerHouseBtn.onClick += OnClickFlowerHouseBtn;
		
		NavCheckBox [0].onStateChange += new UICheckbox.OnStateChange (SelectFriend);
		NavCheckBox [1].onStateChange += new UICheckbox.OnStateChange (SelectBlack);

		
		if (string.IsNullOrEmpty (XFriendManager.SP.MySignature)) {
			SignatureInput.text = XStringManager.SP.GetString (134);
		} else {
			SignatureInput.text = XFriendManager.SP.MySignature;
		}

		UIEventListener listernSingnatureLable = UIEventListener.Get (SignatureInput.gameObject);
		listernSingnatureLable.onSelect += OnSingnaturePress;

		UIEventListener lsExit = UIEventListener.Get(ButtonExit.gameObject);
		lsExit.onClick += closePanel;
		

		//InitData
		ShowRightContainer (false);

		//初始化好友数据
		foreach (KeyValuePair<ulong, XFriendPlayer> s in XFriendManager.SP.GetFriendList()) {

			if (!myFriendContents.ContainsKey (s.Value.UID)) {
				this.UpdateFriendItem (s.Value.UID);
			}
		}

		//初始化黑名单数据
		foreach (KeyValuePair<ulong, XFriendPlayer> s in XFriendManager.SP.GetBlackList()) {
			if (!myBlackContents.ContainsKey (s.Value.UID)) {
				this.UpdateBlackItem (s.Value.UID);
			}
		}

		SetUILabelFlowers (XFriendManager.SP.Myflowers);
		this.RightFriendContainerBtn [6].gameObject.SetActive (false);
	}

	private void SelectFriend(bool state)
	{
		if (state) {
			SelectTabData (NavCheckBox [0].userdata);
		}
	}

	private void SelectBlack(bool state)
	{
		if (state) {
			SelectTabData (NavCheckBox [1].userdata);
		}
	}

	public void SelectTabData(int index)
	{
		for (int i = 0; i < 2; i++) {
			if (i == index) {
				FuncListGrid [i].gameObject.SetActive (true);
			} else {
				FuncListGrid [i].gameObject.SetActive (false);
				ShowRightContainer (false);
				CurPlayerContent = null;
			}
		}
	}

	private void closePanel(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFriend);
	}
    #region UILogic
	public void SetUILabelFlowers(uint number)
	{
		this.MyFlowers.text = number.ToString ();
	}

	public void SetFriendCount(int number)
	{
		if (NavCheckBox [0].gameObject != null) {
			UILabel count = NavCheckBox [0].transform.Find ("Label").GetComponent<UILabel> ();
			count.text = string.Format ("好友({0})", number.ToString ());
		}
	}

	public void SetBlackCount(int number)
	{
		if (NavCheckBox [1].gameObject != null) {
			UILabel count = NavCheckBox [1].transform.Find ("Label").GetComponent<UILabel> ();
			count.text = string.Format ("黑名单({0})", number.ToString ());
		}
	}

	public void UpdateFriendItem(ulong playerID)
	{
		XFriendPlayer friendUser = new XFriendPlayer ();
		if (XFriendManager.SP.GetFriendList ().ContainsKey (playerID)) {
			if (!this.myFriendContents.ContainsKey (playerID)) {
				friendUser = XFriendManager.SP.GetFriendList () [playerID];
				FriendContent iContent = new FriendContent ();
				iContent.Init (this, friendUser, PreFriendBtn, FuncListGrid [0].gameObject);
				this.myFriendContents.Add (friendUser.UID, iContent);
				FuncListGrid [0].Reposition ();
				SetFriendCount (myFriendContents.Count);
			} else {
				this.myFriendContents [playerID].UpdateInfo (playerID);
				FuncListGrid [0].repositionNow = true;
			}
		} else {
			Log.Write (LogLevel.ERROR, "XUIFriend.cs, UpdateFriendItem() the playerID not in GetFriendList()");
		}
	}

	public void UpdateBlackItem(ulong playerID)
	{
		XFriendPlayer blackPlayer = new XFriendPlayer ();
		if (XFriendManager.SP.GetBlackList ().ContainsKey (playerID)) {
			if (!myBlackContents.ContainsKey (playerID)) {
				blackPlayer = XFriendManager.SP.GetBlackList () [playerID];
				BlackContent iContent = new BlackContent ();
				iContent.Init (this, blackPlayer, PreBlackBtn, FuncListGrid [1].gameObject);
				this.myBlackContents.Add (blackPlayer.UID, iContent);
				FuncListGrid [1].repositionNow = true;
				SetBlackCount (myBlackContents.Count);
			}

		} else {

		}
	}

	public void DelItem(ulong UID)
	{
		if (myFriendContents.ContainsKey (UID)) {
			GameObject.Destroy (myFriendContents [UID].FButton.gameObject);
			myFriendContents [UID].FButton.gameObject.SetActive (false);
			myFriendContents.Remove (UID);
			FuncListGrid [0].repositionNow = true;
			SetFriendCount (myFriendContents.Count);
		}
	}

	public void DelBlackItem(ulong UID)
	{
		if (myBlackContents.ContainsKey (UID)) {
			GameObject.Destroy (myBlackContents [UID].FButton.gameObject);
			myBlackContents [UID].FButton.gameObject.SetActive (false);
			myBlackContents.Remove (UID);
			FuncListGrid [1].repositionNow = true;
			SetBlackCount (myBlackContents.Count);
		}

	}

	public void ClearAllFriendItem()
	{
		foreach (KeyValuePair<ulong, FriendContent> s in myFriendContents) {
			GameObject.Destroy (s.Value.FButton.gameObject);
		}
		CurPlayerContent = null;
		myFriendContents.Clear ();
	}

	//  显示功能按钮
	public void ShowFriendGather(bool _isShow)
	{
		if (_isShow) {
			FuncGatherFriend.SetActive (true);
			return;
		}
		FuncGatherFriend.SetActive (false);
	}

	public void ShowBlackGather(bool _isShow)
	{
		if (_isShow) {
			FuncGatherBlack.SetActive (true);
			return;
		}
		FuncGatherBlack.SetActive (false);
	}

	public void ShowRightContainer(bool _isShow)
	{
		if (_isShow) {
			RightContainer.SetActive (true);
			return;
		}
		RightContainer.SetActive (false);
	}
    #endregion

    #region UIEventListenerEventArgs
	//发起私人聊天
	private void OnToChat(GameObject go)
	{
		if (CurPlayerContent != null) {
			string _name = CurPlayerContent.Name;
			ulong _uid = CurPlayerContent.UID;
			uint _level = CurPlayerContent.Level;
			int _status = 1;
			XEventManager.SP.SendEvent (EEvent.Chat_OpenPrivate, _name, _uid);
		}
	}

	//签名框
	private string tempText = "";

	private void OnSingnaturePress(GameObject go, bool inputSelect)
	{
		if (inputSelect) {
			tempText = SignatureInput.text;
			if (SignatureInput.text == XStringManager.SP.GetString (134)) {
				SignatureInput.text = "";
			}
		} else {
			if (SignatureInput.text != tempText) {
				if (SignatureInput.text == "" && XFriendManager.SP.MySignature == "") {
					SignatureInput.text = XStringManager.SP.GetString (134);
				} else {
					XEventManager.SP.SendEvent (EEvent.Friend_SetSignature, SignatureInput.text);
				}				
			}
		}
	}

	private void OnClickSendFlower(GameObject go)
	{
		if (CurPlayerContent != null) {
			uint sendFlowerNum = 1;
			XEventManager.SP.SendEvent (EEvent.Friend_PresentFlowerBtn, CurPlayerContent.UID, sendFlowerNum);
		}
	}

	private void OnClickSendFlowerN(GameObject go)
	{
		if (CurPlayerContent != null) {
			uint sendFlowerNum = 99;
			XEventManager.SP.SendEvent (EEvent.Friend_PresentFlowerBtn, CurPlayerContent.UID, sendFlowerNum);
		}
	}

	private void OnClickFlowerHouseBtn(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.UI_Toggle, EUIPanel.eFriendFlowerHouse);
	}

	private void OnClickCharmRankBtn(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.UI_Toggle, EUIPanel.eFriendCharmRank);
	}

	private void OnClickAddFriend(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.Friend_AddFriendBtn);
	}

	private void OnClickDelFriend(GameObject go)
	{
		if (CurPlayerContent != null) {
			XEventManager.SP.SendEvent (EEvent.Friend_DelFriendBtn, CurPlayerContent.UID, CurPlayerContent.Name);
			//local debug
			//XEventManager.SP.SendEvent (EEvent.Friend_DelFriend, CurPlayerContent.UID);
		}
	}

	private void OnClickSendDiamond(GameObject go)
	{
		if (CurPlayerContent != null) {
			XEventManager.SP.SendEvent (EEvent.Friend_GetMarryBtn, CurPlayerContent);
		}
	}
	private void OnHoverSendDiamond(GameObject go, bool state)
	{
		if(state)
		{
			string content = "";
			content += "[color=00ff00]";
			content += "一枚戒指 " + XFriendManager.S_GETMARRY_COST_REALMONEY.ToString() + "元宝";
			content += '\n';
			content += "[color=ffffff]";
			content += "与对方喜结连理不离不散";
			content += '\n';
			content += " ";
			content += '\n';
			content += "[color=f89a4b]";
			content += "双方亲密值达到5000";
			content += '\n';
			content += "双方必须同时在线";

			XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipC);
			XEventManager.SP.SendEvent (EEvent.ToolTip_C, content);
		}else
		{
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipC);
		}
	}

	private void OnHoverPresentFlower(GameObject go, bool state)
	{
		if(state)
		{
			string content = "";
			content += "[color=00ff00]";
			content += "1朵鲜花 " + XFriendManager.S_PRESENT_FLOWER_COST_GAMEMONEY.ToString() + "金";
			content += '\n';
			content += "[color=ffffff]";
			content += "与对方亲密值+" + XFriendManager.S_PRESENT_FLOWER_GM_ADD_FRIENDLYVALUE.ToString();
			XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipC);
			XEventManager.SP.SendEvent (EEvent.ToolTip_C, content);
		}else
		{
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipC);
		}
	}

	private void OnHoverPresentFlowerN(GameObject go, bool state)
	{
		if(state)
		{
			string content = "";
			content += "[color=00ff00]";
			content += "99朵鲜花 " + XFriendManager.S_PRESENT_FLOWER_COST_REALMONEY.ToString() + "元宝";
			content += '\n';
			content += "[color=ffffff]";
			content += "与对方亲密值+" + XFriendManager.S_PRESENT_FLOWER_RM_ADD_FRIENDLYVALUE.ToString();
			XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipC);
			XEventManager.SP.SendEvent (EEvent.ToolTip_C, content);
		}else
		{
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipC);
		}
	}

	private void OnClickLookInfo(GameObject go)
	{
		if (CurPlayerContent != null) {
			if (CurPlayerContent.Status == 1) {
				XEventManager.SP.SendEvent (EEvent.Chat_ShowPlayerInfoReq, CurPlayerContent.Status);
			} else {
				XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10001, CurPlayerContent.Name);
			}
		}
	}

	private void OnClickRemoveInBlackList(GameObject go)
	{
		if (CurPlayerContent != null) {
			XEventManager.SP.SendEvent (EEvent.Friend_DelBlackBtn, CurPlayerContent.UID, CurPlayerContent.Name);
		}
	}

	private void OnClickDivorce(GameObject go)
	{
		if (CurPlayerContent != null) {
			XEventManager.SP.SendEvent (EEvent.Friend_DivorceBtn, CurPlayerContent.UID, CurPlayerContent.Name);
		}
	}
    #endregion
}