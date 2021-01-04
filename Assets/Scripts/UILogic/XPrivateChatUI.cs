using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;

[AddComponentMenu("UILogic/XPrivateChatUI")]
public class XPrivateChatUI : XDefaultFrame 
{
	public struct playerChatInfo
	{
		public ulong id;
		public string name;				// 玩家名称
		public int level;					// 玩家等级
		public int icon;					// 玩家关系图标
	}
	
	private struct StructContent
	{
		public StructContent(int t, string n,string c)
		{
			type = t;
			name = n;
			content = c;
		}
		public int type;		 // 0 接受到的消息， 1 自己发送的消息
		public string name;
		public string content;
	}
	
	class FriendObject
	{
		private static int MaxPrivateFriend = 10000000;
		public FriendObject(XPrivateChatUI ui, GameObject sam, GameObject parent, playerChatInfo info, 
			UITable sampleParent, GameObject target, GameObject me)
		{
			// 好友列表
			_samFriend = sam;
			_parentFriend = parent;
			// 聊天内容
			_target = target;
			_me = me;
			_parent = sampleParent;
			
			privateChatUI = ui;
			_info = info;
			
			MaxPrivateFriend--;
			init(info.name, MaxPrivateFriend.ToString() + "[" + info.name + "]", sam, parent);
		}
		
		private void init(string showName, string name, GameObject sam, GameObject parent)
		{
			itemObject = XUtil.Instantiate(sam);
			itemObject.name = name;
			itemObject.transform.parent = parent.transform;
			itemObject.SetActive(true);

			_buttonImage = (UIImageButton)itemObject.transform.Find("Select_button").GetComponent<UIImageButton>();
			UIEventListener lis = UIEventListener.Get(_buttonImage.gameObject);
			lis.onClick += onSelect;
			
			_button = _buttonImage.transform.Find("Background").GetComponent<UISprite>();
			
			UIImageButton closeBtn = (UIImageButton)itemObject.transform.Find("FriendCloseButton").GetComponent<UIImageButton>();
			lis = UIEventListener.Get(closeBtn.gameObject);
			lis.onClick += onClickClose;
			
			UILabel labelName = (UILabel)itemObject.transform.Find("Label_name").GetComponent<UILabel>();
			if ( labelName )
				labelName.text = showName;
			
			XPrivateChatUI.m_allShowUserid.Add(_info.id);
			
			if ( !_read )
			{
				_button.spriteName = "11000315";
				_buttonImage.normalSprite = "11000315";
				_buttonImage.hoverSprite = "11000315";
			}
		}
		
		// 聊天列表 
		public void updatePlayerInfo(int level, int icon, bool needReposition)
		{
			if ( needReposition )
			{
				MaxPrivateFriend--;
				if ( null == itemObject )
					init(_info.name, MaxPrivateFriend.ToString() + "[" + _info.name + "]", _samFriend, _parentFriend);
				else
					itemObject.name = MaxPrivateFriend.ToString() + "[" + _info.name + "]";
			}
			_info.icon = icon;
			_info.level = level;
		}
		
		public void SetChecked(bool selected)
		{
			UISprite spriteCheck = (UISprite)itemObject.transform.Find("Sprite_checked").GetComponent<UISprite>();
			spriteCheck.gameObject.SetActive(selected);
			
			UILabel labelName = (UILabel)itemObject.transform.Find("Label_name").GetComponent<UILabel>();
			string color = selected ? "[color=FEBF02]" : "[color=A3C7EB]";
			labelName.text = color + _info.name;
		}
		
		public void onSelect(GameObject go)
		{
			privateChatUI.OnFriendChange(_info);
			SetChecked(true);
			
			_button.spriteName = "11000313";
			_buttonImage.normalSprite = "11000313";
			_buttonImage.hoverSprite = "11000314";
			_read = true;
		}
		
		private void onClickClose(GameObject go)
		{
			privateChatUI.CloseShowWindow(_info.name);
		}
		
		//聊天内容
		public void SetRecvContent(string content)
		{
			if ( m_allContent.Count > MAX_CONTENT )
			{
				RemoveLastContentObj();
			}
			m_allContent.AddLast(new StructContent(0, _info.name, content));
		}
		
		public void SetSendContent(string content, string name)
		{
			if ( m_allContent.Count > MAX_CONTENT )
			{
				RemoveLastContentObj();
			}
			m_allContent.AddLast(new StructContent(1, name, content));
		}
		
		public void ShowContentObj(int type, string name,string content)
		{
			GameObject itemObj = null;
			if ( 0 == type )
				itemObj = XUtil.Instantiate(_target);
			else
				itemObj = XUtil.Instantiate(_me);
			itemObj.transform.parent = _parent.gameObject.transform;
			itemObj.SetActive(true);
			_parent.Reposition();
			_parent.repositionNow = true;
			
			UILabel labelName = null;
			UILabel labelContent = null;
			labelName = itemObj.transform.Find("Label_MsgName").GetComponent<UILabel>();
			labelContent = itemObj.transform.Find("Label_MsgContent").GetComponent<UILabel>();
			labelName.text = name;
			labelContent.text = content;
			m_allContentObj.AddLast(itemObj);
			_parent.Reposition();
		}
		
		public void ShowAllContent()
		{
			LinkedListNode<StructContent> begin = m_allContent.First;
			while ( begin != null )
			{
				ShowContentObj(begin.Value.type, begin.Value.name, begin.Value.content);
				begin = begin.Next;
			}
		}
		
		public void ClearAllContent()
		{
			foreach( GameObject obj in m_allContentObj )
			{
				obj.transform.parent = _parent.transform.parent;
				GameObject.Destroy(obj);
			}
			m_allContentObj.Clear();
		}
		
		public void RemoveLastContentObj()
		{
			m_allContent.RemoveFirst();
			GameObject obj = m_allContentObj.First.Value;
			GameObject.Destroy(obj);
			m_allContentObj.RemoveFirst();
		}
		
		public void SetMessageReadStatus(bool read)
		{
			_read = read;
			if ( !read && null != _button )
			{
				_button.spriteName = "11000315";
				_buttonImage.normalSprite = "11000315";
				_buttonImage.hoverSprite = "11000315";
			}
		}
		
		// 聊天内容
		private GameObject _target;
		private GameObject _me;
		private bool _read = true;
		private UITable _parent;
		private LinkedList<StructContent> m_allContent = new LinkedList<StructContent>();
		private LinkedList<GameObject> m_allContentObj = new LinkedList<GameObject>();
		
		// 好友列表
		private GameObject _samFriend;
		private UISprite _button;
		private UIImageButton _buttonImage;
		private GameObject _parentFriend;
		public playerChatInfo _info;					
		private int level;
		private string _content;
		public GameObject itemObject;
		private XPrivateChatUI privateChatUI;
		
		private  const int MAX_CONTENT = 100;
	}
	
	class BiaoQIng
	{
		public BiaoQIng(XPrivateChatUI parent, GameObject obj, int index)
		{
			_index = index;
			_parent = parent;
			
			UIEventListener lis = UIEventListener.Get(obj);
			lis.onClick += OnSelectQiaoQIng;
		}
		
		private void OnSelectQiaoQIng(GameObject go)
		{
			_parent.SetBiaoQing(_index);	
		}
		
		XPrivateChatUI _parent;
		private int _index;
	}
	
	public UIScrollBar FriendsScrollBarl;
	public UIGrid	  FriendsGrid;									// 玩家聊天列表
	public GameObject FriendObject_Sampel;							// 玩家列表显示实例
	private static SortedList<string, FriendObject> m_allFriendObjects = new SortedList<string, FriendObject>(); // 所有聊天玩家信息，以玩家名称作为key
	private SortedList<string, string> m_allClosedContent = new SortedList<string, string>();		      // 保存完结聊天信息
	
	public static BetterList<ulong> m_allShowUserid = new BetterList<ulong>();
	
	public UIScrollBar ContentsScrollBarl;
	public UITable TabContent_Show;									// 聊天内容显示的tab
	public GameObject ObjContentShow_Target;						// 接收到的信息 显示控件
	public GameObject ObjContentShow_Me;							// 玩家发送的信息 显示控件
	public UIInput Input_SendMsg;									// 玩家输入框
	
	public UILabel Label_Level;										// 玩家等级
	public UISprite Sprite_Realitve;								// 关系图标
	
	public UILabel Label_NameShow;	

	public GameObject Btn_Friend;
	public GameObject Btn_Add2Black;
	public GameObject Btn_SendMsg;
	public GameObject Btn_Close;		
	
	public static string currentUser = null;
	private ulong currentId = 0uL;
	
	public static bool isActive = false;
	
	public UISprite[] BiaoQing_Sprite;
	public UIImageButton[] BiaoQing_SelBtn;
	public UIImageButton BiaoQingBtn;
	public GameObject BiaoQingPanel;
	
	private SortedList<int, string> mapBiaoQingId2Str = new SortedList<int, string>();
	private SortedList<string, int> mapBiaoQingStr2Id = new SortedList<string, int>();
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener lis = UIEventListener.Get(Btn_Friend);
		lis.onClick += onAddFriend;
		
	    lis = UIEventListener.Get(Btn_Add2Black);
		lis.onClick += onAddBlack;
		
		lis = UIEventListener.Get(Btn_SendMsg);
		lis.onClick += onSendMsg;
		
		lis = UIEventListener.Get(Input_SendMsg.gameObject);
		lis.onSelect += OnToggleChat;
		lis.onSubmit += onSendMsg;
	
		lis = UIEventListener.Get(Btn_Close);
		lis.onClick += onCloseCurShow;
		
		TabContent_Show.onReposition += onRepositionAfter;
		
		lis = UIEventListener.Get(BiaoQingBtn.gameObject);
		lis.onClick += OnClickShowBiaoQing;
		
		_initBiaoQing();
		
		return true;
	}
	
	public override void Show()
	{
		isActive = true;
		base.Show();
	}
	
	public override void Hide()
	{
		isActive = false;
		base.Hide();
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePrivateChat);
	}
	
	private void onRepositionAfter()
	{
		ContentsScrollBarl.scrollValue = 1;
	}
	
	public static bool UserInShow(ulong id)
	{
		return m_allShowUserid.Contains(id);
	}
	
	public void OnToggleChat(GameObject go, bool bSelected)
	{
		XEventManager.SP.SendEvent(EEvent.Chat_ToggleChat, bSelected);
	}
	
	public void ON_SC_SetChatData(string playerName, ulong id, int level, int icon, string content, int reposition)
	{
		FriendObject obj = null;
		if ( !m_allFriendObjects.TryGetValue(playerName, out obj) )
		{
			playerChatInfo info;
			info.name = playerName;
			info.level = level;
			info.icon = icon;
			info.id = id;
			obj = new FriendObject(this, FriendObject_Sampel, FriendsGrid.gameObject, info, TabContent_Show, ObjContentShow_Target, ObjContentShow_Me);
			m_allFriendObjects.Add(playerName, obj);
		}
		
		// 如果是当前玩家，则需要马上更新聊天显示框中的数据,否则切换聊天玩家
		if ( playerName == currentUser && content.Length > 0 )
		{
			obj.ShowContentObj(0, playerName, content);
			TabContent_Show.Reposition();
			TabContent_Show.repositionNow = true;
			Label_Level.text =  string.Format(XStringManager.SP.GetString(1043), level.ToString());
			Sprite_Realitve.spriteName =  (icon==1 ? "11000224" : "11000222");
			Label_NameShow.text = playerName;
			obj.SetMessageReadStatus(true);
		}
		else
		{
			obj.SetMessageReadStatus(false);
		}
		obj.SetRecvContent(content);
		obj.updatePlayerInfo(level, icon, 1 == reposition);
		FriendsGrid.repositionNow = true;
		
		FriendsScrollBarl.scrollValue = 0;
	}
	
	public void On_UserChange(string playerName)
	{
		if ( currentUser == playerName  )
			return;
		
		FriendObject obj;
		if ( !m_allFriendObjects.TryGetValue(playerName, out obj) )
			return;
		obj.onSelect(gameObject);
	}
	
	public void On_SC_OpenPrivateChat(string playerName, ulong id, int level, int icon)
	{
		FriendObject obj = null;
		if ( !m_allFriendObjects.TryGetValue(playerName, out obj) )
		{
			playerChatInfo info;
			info.name = playerName;
			info.level = level;
			info.icon = icon;
			info.id = id;
			obj = new FriendObject(this, FriendObject_Sampel, FriendsGrid.gameObject, info, TabContent_Show, ObjContentShow_Target, ObjContentShow_Me);
			m_allFriendObjects.Add(playerName, obj);
		}
		else
		{
			obj.updatePlayerInfo(level, icon, true);
		}
		FriendsGrid.Reposition();
		Label_Level.text = string.Format(XStringManager.SP.GetString(1043), level.ToString());
		Sprite_Realitve.spriteName =  (icon==1 ? "11000224" : "11000222");
		if ( currentUser == playerName )
			return;
		FriendsScrollBarl.scrollValue = 0;
		obj.onSelect(gameObject);
	}
	
	internal void OnFriendChange(playerChatInfo info)
	{
		Input_SendMsg.selected = true;
		if ( currentUser == info.name )
			return;
		
		if ( null != currentUser && currentUser.Length > 0 && m_allFriendObjects.ContainsKey(currentUser) )
			m_allFriendObjects[currentUser].SetChecked(false);
		
		if ( null != currentUser && currentUser.Length > 0 )
			m_allFriendObjects[currentUser].ClearAllContent();
		m_allFriendObjects[info.name].ShowAllContent();
		TabContent_Show.Reposition();
		TabContent_Show.repositionNow = true;
		currentUser = info.name;
		currentId = info.id;
		Label_Level.text = string.Format(XStringManager.SP.GetString(1043), info.level.ToString());
		Label_NameShow.text = info.name;
		Sprite_Realitve.spriteName =  (info.icon == 1 ? "11000224" : "11000222");
	}
	
	private void onAddFriend(GameObject go)
	{
		if ( !string.IsNullOrEmpty(currentUser) )
			XEventManager.SP.SendEvent(EEvent.Freind_ChatAddFriend, currentUser);
	}
	
	private void onAddBlack(GameObject go)
	{
		if ( string.IsNullOrEmpty(currentUser) )
			return;
		
		FriendObject obj;
		if ( m_allFriendObjects.TryGetValue(currentUser, out obj) )
			XEventManager.SP.SendEvent(EEvent.Friend_MoveToBlackListBtn, obj._info.id, currentUser);
	}
	
	private void onCloseCurShow(GameObject go)
	{
		CloseShowWindow(currentUser);
	}
	
	internal void CloseShowWindow(string name)
	{
		FriendObject obj = null;
		if ( null == name || name.Length <= 0 || !m_allFriendObjects.TryGetValue(name, out obj) || null == obj )
			return;

		if ( name == currentUser )
		{
			obj.ClearAllContent();
			Label_NameShow.text = "";
			Label_Level.text = "";
			currentUser = "";
			currentId = 0L;
		}
		
		obj.itemObject.transform.parent = transform;
		obj.ClearAllContent();
		GameObject.Destroy(obj.itemObject);
		XPrivateChatUI.m_allShowUserid.Remove(obj._info.id);
		FriendsGrid.repositionNow = true;
		TabContent_Show.Reposition();
		TabContent_Show.repositionNow = true;
		
		if ( FriendsGrid.transform.childCount <= 0 )
			Hide();
	}
	
	private void onSendMsg(GameObject go)
	{
		Input_SendMsg.selected = true;
		
		if ( Input_SendMsg.text.Length <= 0 || null == currentUser || currentUser.Length <= 0 )
			return;
		
		string content = Input_SendMsg.text;
		foreach( KeyValuePair<string, int> item in mapBiaoQingStr2Id )
		{
			while ( content.IndexOf(item.Key) >= 0 )
			{	
				XCfgPhizConfig config = XCfgPhizConfigMgr.SP.GetConfig(item.Value);
				string biaoQingFormat = "[sprite=1400.{0}.22.22]";
				string bq = string.Format(biaoQingFormat, config.Sprite);
				content = content.Replace(item.Key, bq);
			}
		}
		
		XEventManager.SP.SendEvent(EEvent.Chat_SendChatMsg, content, EChatType.eChatPlayer_Private, currentUser, currentId);
		
		FriendObject obj = null;
		if ( m_allFriendObjects.TryGetValue(currentUser, out obj) )
			obj.SetSendContent(content, XLogicWorld.SP.MainPlayer.Name);
		
		m_allFriendObjects[currentUser].ShowContentObj(1, XLogicWorld.SP.MainPlayer.Name, content);
		Input_SendMsg.text = "";
	}
	
	private void _initBiaoQing()
	{
		int i = 0;
		foreach( KeyValuePair<int, XCfgPhizConfig> item in XCfgPhizConfigMgr.SP.ItemTable )
		{
			if ( i >= 12 )
				return;
			XUIDynamicAtlas.SP.SetSprite(BiaoQing_Sprite[i], 1400, item.Value.Sprite);
			new BiaoQIng(this, BiaoQing_SelBtn[i].gameObject, item.Key);
			BiaoQing_SelBtn[i].SetTipString(item.Value.Tip);
			mapBiaoQingId2Str[item.Key] = item.Value.Shortening;
			mapBiaoQingStr2Id[item.Value.Shortening] = item.Key;
			i++;
		}
	}
	
	private void OnClickShowBiaoQing(GameObject go)
	{
		BiaoQingPanel.SetActive(!BiaoQingPanel.activeSelf);
		if ( BiaoQingPanel.activeSelf )
			XChatWindow.isBiaoQingOper = true;
	}
	
	public void HideBiaoQingUI()
	{
		BiaoQingPanel.SetActive(false);
	}
	
	public void SetBiaoQing(int index)
	{
		XCfgPhizConfig config = XCfgPhizConfigMgr.SP.GetConfig(index);
		Input_SendMsg.text += config.Shortening;
		Input_SendMsg.selected = true;
		OnClickShowBiaoQing(gameObject);
	}
}
