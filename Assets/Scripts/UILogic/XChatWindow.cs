using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XChatWindow")]
public class XChatWindow : XUIBaseLogic
{
	public static GameObject RootParent;
	
	public static Vector3	ChatToolTipPos;
	public static float BottomYPos = 0f;
	public static Transform PlayerGuideKeybordRefernce;
	public static bool isBiaoQingOper = false;
	
	class ChatPanel
	{
		public ChatPanel(GameObject parent, GameObject sample, int maxCount)
		{
			_parent = parent;
			_sample = sample;
			_maxCount = maxCount;
		}
		
		public void SetChatContent(string msg)
		{
			GameObject itemObject = XUtil.Instantiate(_sample);
			itemObject.transform.parent = _parent.transform;
			
			
			UILabel label = itemObject.transform.Find("Label").GetComponent<UILabel>();
			if ( null == label )
				return;
			label.text = msg;
			NGUITools.AddWidgetCollider(label.gameObject);
			
			UIEventListener lis = UIEventListener.Get(label.gameObject);
			lis.onClickHyperLink	+= ClickName;
			
			m_allObjs.AddFirst(itemObject);
			if ( m_allObjs.Count >= _maxCount )
			{
				GameObject.Destroy(m_allObjs.Last.Value);
				m_allObjs.RemoveLast();
			}
		}

		private void ClickName(GameObject go,string str)
		{
			HyperLinkMgr.SP.Process(str);
		}
		
		private LinkedList<GameObject> m_allObjs = new LinkedList<GameObject>();
		private int _maxCount;
		private GameObject _parent;
		private GameObject _sample;
	}
	
	class BiaoQIng
	{
		public BiaoQIng(XChatWindow parent, GameObject obj, int index)
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
		
		XChatWindow _parent;
		private int _index;
	}
	
	public GameObject BKGROUND_SPRITE;
	public UIScrollBar[] Scroll_Bar;
	public UISprite[] Scroll_Bar_bk;
	public UISprite[] Scroll_Bar_fr;
	public GameObject Button_1zonghe;
	public GameObject Button_2world;
	public GameObject Button_3system;
	public GameObject Button_4banghui;
	
	public GameObject Button_6gmcommands;
	
	public UILabel[] BtnList_Label;
	public UILabel Button_Current;
	
	public UIInput InputChat;
	public GameObject ButtonSubmit;
	
	private EChatType cuType;
	
	private SortedList<string, XItem> m_allChatItem = new SortedList<string, XItem>();
	
	private LinkedList<SC_Chat> m_allPrivateChat = new LinkedList<SC_Chat>();
	public GameObject PrivateChat_parent;
	public GameObject Btn_PrivteChat;
	public UILabel Label_PrivateCount;
	private int currNewMsgCount = 0;
	
	// 综合频道
	public UITable Tab_zonghe;
	public GameObject Label_zonghe;
	private ChatPanel Zonghe_Content;
	
	// 世界
	public UITable Tab_world;
	public GameObject Label_world;
	private ChatPanel World_Content;
	
	// 系统
	public UITable Tab_system;
	public GameObject Label_system;
	private ChatPanel System_Content;
	
	// 帮会
	public UITable Tab_banghui;
	public GameObject Label_banghui;
	private ChatPanel Banghui_Content;
	
	public GameObject Button_Max;		// 聊天框放大
	public GameObject Button_Min;		// 聊天框缩小
	private int bMax = 2;
	
	public GameObject[] ScrollButton_Down;
	
	public GameObject[] Object_Panel;
	
	private int _currentSel = 0;
	
	public UISprite[] BiaoQing_Sprite;
	public UIImageButton[] BiaoQing_SelBtn;
	public UIImageButton BiaoQingBtn;
	public GameObject BiaoQingPanel;
	
	public UISprite BkGround;
	public GameObject ScrollBarParent;
	
	private SortedList<int, string> mapBiaoQingId2Str = new SortedList<int, string>();
	private SortedList<string, int> mapBiaoQingStr2Id = new SortedList<string, int>();
	
	// 处理聊天信息
	// 玩家信息格式 [T(1)N(name)C(classlevel)]
	// 物品信息格式 [T(2)N(name)U(ownerid)D(id)L(level)]
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener listenSubmit = UIEventListener.Get(ButtonSubmit);
		listenSubmit.onClick += SubmitChat;
		UIEventListener listenInput = UIEventListener.Get(InputChat.gameObject);
		listenInput.onSelect += OnToggleChat;
		listenInput.onSubmit += SubmitChat;
		
		UIEventListener listenButton;
		listenButton = UIEventListener.Get(Button_1zonghe.gameObject);
		listenButton.onClick	+= onClickZongheBtn;
		
		listenButton = UIEventListener.Get(Button_2world.gameObject);
		listenButton.onClick	+= onClickWorldBtn;
		
		listenButton = UIEventListener.Get(Button_3system.gameObject);
		listenButton.onClick	+= onClickSystemBtn;
		
		listenButton = UIEventListener.Get(Button_4banghui.gameObject);
		listenButton.onClick	+= onClickBanghuiBtn;

        listenButton = UIEventListener.Get(Button_6gmcommands.gameObject);
        listenButton.onClick += onClickGMCommandsBtn;
		
		listenButton = UIEventListener.Get(Btn_PrivteChat);
		listenButton.onClick += onShowPrivateChat;
		
		listenButton = UIEventListener.Get(Button_Max.gameObject);
		listenButton.onClick += onClickMax;
		
		listenButton = UIEventListener.Get(Button_Min.gameObject);
		listenButton.onClick += onClickMin;
		
		listenButton = UIEventListener.Get(BiaoQingBtn.gameObject);
		listenButton.onClick += OnClickShowBiaoQing;
		
		Tab_zonghe.onReposition += _afterReposition;
		Tab_world.onReposition += _afterReposition;
		Tab_system.onReposition += _afterReposition;
		Tab_banghui.onReposition += _afterReposition;
		
		cuType = EChatType.eChatPlayer_Chat;
		
		Zonghe_Content = new ChatPanel(Tab_zonghe.gameObject, Label_zonghe, 300);
		World_Content = new ChatPanel(Tab_world.gameObject, Label_world,300);
		System_Content = new ChatPanel(Tab_system.gameObject, Label_system,300);
		Banghui_Content = new ChatPanel(Tab_banghui.gameObject, Label_banghui,300);
		
		// 初始化默认为综合频道
		onClickZongheBtn(gameObject);
		
		ChatToolTipPos = new Vector3(gameObject.transform.position.x + BKGROUND_SPRITE.transform.localScale.x,
			gameObject.transform.position.y + BKGROUND_SPRITE.transform.localScale.y, gameObject.transform.position.z);
		
		BottomYPos = gameObject.transform.position.y;
		
		Invoke("showUI", 30);
		
		PlayerGuideKeybordRefernce = transform;
		
		UIPanel.addColliderPanel("Scroll_Panel_1Zonghe");
		UIPanel.addColliderPanel("Scroll_Panel_2World");
		UIPanel.addColliderPanel("Scroll_Panel_3System");
		UIPanel.addColliderPanel("Scroll_Panel_4Banghui");
		
		_initBiaoQing();
		
		BkGround.alpha = 0f;
		
		RootParent = this.gameObject;
		
		return true;
	}
	
	void Update ()
	{
		if ( null == UICamera.mainCamera )
			return;
		
		Vector3 v = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
		Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(BkGround.transform);
		if ( bounds.min.x < v.x && bounds.max.x > v.x &&
			bounds.min.y < v.y && bounds.max.y > v.y)
		{
			BkGround.alpha = 1f;
			ScrollBarParent.SetActive(true);
		}
		else
		{
			BkGround.alpha = 0f;
			ScrollBarParent.SetActive(false);
		}
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
	
	private void _afterReposition()
	{
		Scroll_Bar[_currentSel].scrollValue = 1;
	}
		
	private void showUI()
	{
		XNewPlayerGuideManager.SP.handleDirection((int)XNewPlayerGuideManager.GuideType.Guide_Mouse);
	} 
	
	private void OnClickShowBiaoQing(GameObject go)
	{
		BiaoQingPanel.SetActive(!BiaoQingPanel.activeSelf);
		if ( BiaoQingPanel.activeSelf )
			isBiaoQingOper = true;
	}
	
	public void HideBiaoQingUI()
	{
		BiaoQingPanel.SetActive(false);
	}
	
	public void onClickZongheBtn(GameObject go)
	{
		Tab_zonghe.gameObject.SetActive(true);
		Tab_zonghe.Reposition();
		Tab_zonghe.repositionNow = true;
	 	Tab_world.gameObject.SetActive(false);
	 	Tab_system.gameObject.SetActive(false);
	 	Tab_banghui.gameObject.SetActive(false);
		
		Button_Current.text = BtnList_Label[0].text;
		cuType = EChatType.eChatPlayer_Chat;
		
		Scroll_Bar[0].gameObject.SetActive(true);
		Scroll_Bar[1].gameObject.SetActive(false);
		Scroll_Bar[2].gameObject.SetActive(false);
		Scroll_Bar[3].gameObject.SetActive(false);
		
		_currentSel = 0;
	}
	
	public void onClickWorldBtn(GameObject go)
	{
		Tab_zonghe.gameObject.SetActive(false);
	 	Tab_world.gameObject.SetActive(true);
	 	Tab_system.gameObject.SetActive(false);
	 	Tab_banghui.gameObject.SetActive(false);
		
		Button_Current.text = BtnList_Label[1].text;
		cuType = EChatType.eChatPlayer_Chat;
		Tab_world.Reposition();
		Tab_world.repositionNow = true;
		
		Scroll_Bar[0].gameObject.SetActive(false);
		Scroll_Bar[1].gameObject.SetActive(true);
		Scroll_Bar[2].gameObject.SetActive(false);
		Scroll_Bar[3].gameObject.SetActive(false);
		
		_currentSel = 1;
	}
	
	public void onClickSystemBtn(GameObject go)
	{
		Tab_zonghe.gameObject.SetActive(false);
	 	Tab_world.gameObject.SetActive(false);
	 	Tab_system.gameObject.SetActive(true);
		Tab_system.Reposition();
		Tab_system.repositionNow = true;
	 	Tab_banghui.gameObject.SetActive(false);
		
		Scroll_Bar[0].gameObject.SetActive(false);
		Scroll_Bar[1].gameObject.SetActive(false);
		Scroll_Bar[2].gameObject.SetActive(true);
		Scroll_Bar[3].gameObject.SetActive(false);
		
		_currentSel = 2;
	}
	
	public void onClickBanghuiBtn(GameObject go)
	{
		Tab_zonghe.gameObject.SetActive(false);
	 	Tab_world.gameObject.SetActive(false);
	 	Tab_system.gameObject.SetActive(false);
	 	Tab_banghui.gameObject.SetActive(true);
		Tab_banghui.Reposition();
		Tab_banghui.repositionNow = true;
		
		Button_Current.text = BtnList_Label[3].text;
		cuType = EChatType.eChatFaction_Chat;
		
		Scroll_Bar[0].gameObject.SetActive(false);
		Scroll_Bar[1].gameObject.SetActive(false);
		Scroll_Bar[2].gameObject.SetActive(false);
		Scroll_Bar[3].gameObject.SetActive(true);
		
		_currentSel = 3;
	}

    public void onClickGMCommandsBtn(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGMWindow);
    }
	
	public void SubmitChat(GameObject go)
	{
		if ( cuType == EChatType.eChatSystem_Notice || cuType == EChatType.eChatSystem_Tip )
			return;
		
		// 处理聊天框中的物品数据，采用自定义格式进行显示
		// [T(2)N(name)U(ownerid)D(id)L(level)]
		string content = InputChat.text;	
		foreach ( KeyValuePair<string, XItem> keyItem in m_allChatItem )
		{
			while ( content.IndexOf(keyItem.Key) >= 0 )
			{			
				XItem item = keyItem.Value;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				string data = string.Format("[T(2)N({0})U({1})D({2})L({3})]", cfgItem.Name, 
					XLogicWorld.SP.MainPlayer.ID, item.GUID, cfgItem.QualityLevel);
				content = content.Replace(keyItem.Key, data);
			}
		}
		m_allChatItem.Clear();
		
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
		
		XEventManager.SP.SendEvent(EEvent.Chat_SendChatMsg, content, cuType);
		InputChat.text = "";
	}
	
	public void OnToggleChat(GameObject go, bool bSelected)
	{
		XEventManager.SP.SendEvent(EEvent.Chat_ToggleChat, bSelected);
	}
	
	public void OrderChat()
	{
		InputChat.selected = true;
	}
	
	public void On_Set_ItemData(XItem item)
	{
		string keyData = "[" + XCfgItemMgr.SP.GetConfig(item.DataID).Name + "]";
		if ( m_allChatItem.ContainsKey(keyData) )
			return;
		
		m_allChatItem.Add(keyData, item);
		InputChat.text += keyData;
		OrderChat();
	}
	
	public void SetBiaoQing(int index)
	{
		XCfgPhizConfig config = XCfgPhizConfigMgr.SP.GetConfig(index);
		InputChat.text += config.Shortening;
		OrderChat();
		OnClickShowBiaoQing(gameObject);
	}
	
	public void On_SC_Chat(SC_Chat msg)
	{
		switch ( msg.ChatType )
		{
		case EChatType.eChatPlayer_Chat:
			handlePlayerChat(msg);
			
			break;
		case EChatType.eChatPlayer_Private:
			handlePlayerPrivate(msg);
				
			break;
		case EChatType.eChatFaction_Chat:
			handleFactionChat(msg);
			
			break;
		case EChatType.eChatFaction_Notice:
			handleFactionNotice(msg);
			
			break;
		case EChatType.eChatSystem_Notice:
			handleSystemNotice(msg);
			
			break;
		case EChatType.eChatSystem_Tip:
			handleSystemTip(msg);
			
			break;
		case EChatType.eChatPlayer_OpenPrivateChat:
			handleOpenPrivateChat(msg);
			
			break;
		case EChatType.eChatPlayer_Failed_TooFast:
			handleChatFailed(msg.ChatType);
			
			break;
		case EChatType.eChatPlayer_Failed_InGag:
			handleChatFailed(msg.ChatType);

			break;
		default:
			break;
		}	
	}
	
	private void handleChatFailed(EChatType failedType)
	{
		switch ( failedType )
		{
		case EChatType.eChatPlayer_Failed_TooFast:
			{
				Zonghe_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1044));
				if ( _currentSel == 0 )
				{
					Tab_zonghe.Reposition();
					Tab_zonghe.repositionNow = true;
				}
		
				System_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1044));
				if ( _currentSel == 2 )
				{
					Tab_system.Reposition();
					Tab_system.repositionNow = true;
				}
			
				World_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1044));
				if ( _currentSel == 1 )
				{
					Tab_world.Reposition();
					Tab_world.repositionNow = true;
				}
			}
			
			break;
		case EChatType.eChatPlayer_Failed_InGag:
			{
				Zonghe_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1065));
				if ( _currentSel == 0 )
				{
					Tab_zonghe.Reposition();
					Tab_zonghe.repositionNow = true;
				}
		
				System_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1065));
				if ( _currentSel == 2 )
				{
					Tab_system.Reposition();
					Tab_system.repositionNow = true;
				}
			
				World_Content.SetChatContent("[color=00ff00]" + XStringManager.SP.GetString(1065));
				if ( _currentSel == 1 )
				{
					Tab_world.Reposition();
					Tab_world.repositionNow = true;
				}
			}
			
			break;    
		default:
			break;
		}
	}
	
	private void handleOpenPrivateChat(SC_Chat msg)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.ePrivateChat);
		XEventManager.SP.SendEvent(EEvent.Chat_OpenPrivateUI, msg.ObjectName, msg.ObjectId, msg.ObjectLevel, msg.ObjectIcon);
	}
	
	private void handleSystemNotice(SC_Chat msg)
	{
		string output = "";
		handleChatData(msg.ChatData , ref output);
		string strContent = "[color=ff1515]" + XStringManager.SP.GetString(1031) + output;
		
		Zonghe_Content.SetChatContent(strContent);
		if ( _currentSel == 0 )
		{
			Tab_zonghe.Reposition();
			Tab_zonghe.repositionNow = true;
		}
		
		System_Content.SetChatContent(strContent);
		if ( _currentSel == 2 )
		{
			Tab_system.Reposition();
			Tab_system.repositionNow = true;
		}
	}
	
	private void handleSystemTip(SC_Chat msg)
	{
		string content2show = "[color=00ff00]" + msg.ChatData;
		Zonghe_Content.SetChatContent(content2show);
		if ( _currentSel == 0 )
		{
			Tab_zonghe.Reposition();
			Tab_zonghe.repositionNow = true;
		}
		
		System_Content.SetChatContent(content2show);
		if ( _currentSel == 2 )
		{
			Tab_system.Reposition();
			Tab_system.repositionNow = true;
		}
	}
	
	private void handlePlayerChat(SC_Chat msg)
	{
		if ( !msg.HasObjectName )
			return;
		
		string strContent = "";
		string color = XGameColorDefine.JJ_Color[msg.ClassLevel];
		strContent += "[link=1][linkdata=T(2)N(";
		strContent += msg.ObjectName;
		strContent += ")D(";
		strContent += msg.ObjectId;
		strContent += ")]";
		strContent += color.Replace("[", "[link");
		strContent += msg.ObjectName + ": ";
		strContent += "[link=0][color=a1f9e1]";
		
		string output = "";
		handleChatData(msg.ChatData , ref output);
		strContent += output;
		
		string content2show = "[color=fff337]" + string.Format(XStringManager.SP.GetString(1035), strContent);
		Zonghe_Content.SetChatContent(content2show);
		if ( _currentSel == 0 )
		{
			Tab_zonghe.Reposition();
			Tab_zonghe.repositionNow = true;
		}
		
		World_Content.SetChatContent(content2show);
		if ( _currentSel == 1 )
		{
			Tab_world.Reposition();
			Tab_world.repositionNow = true;
		}
	}
	
	private void handlePlayerPrivate(SC_Chat msg)
	{
		if ( XPrivateChatUI.isActive && XPrivateChatUI.UserInShow(msg.ObjectId) /*&& msg.ObjectName == XPrivateChatUI.currentUser */)  
		{
			XEventManager.SP.SendEvent(EEvent.Chat_SetPrivateChatData, msg.ObjectName, 
				msg.ObjectId, msg.ObjectLevel, msg.ObjectIcon, msg.ChatData, 0);
		}
		else 
		{
			currNewMsgCount++;
		}
		if ( currNewMsgCount > 0 )
			PrivateChat_parent.SetActive(true);
		
		Label_PrivateCount.text = "[color=a1f9e1]" + string.Format(XStringManager.SP.GetString(1040), currNewMsgCount);
		
		// 先保存到内存中
		m_allPrivateChat.AddFirst(msg);
	}
	
	private void onShowPrivateChat(GameObject go)
	{
		currNewMsgCount = 0;
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.ePrivateChat);
		
		foreach( SC_Chat msg  in m_allPrivateChat )
		{
			XEventManager.SP.SendEvent(EEvent.Chat_SetPrivateChatData, msg.ObjectName, msg.ObjectId, 
				msg.ObjectLevel, msg.ObjectIcon, msg.ChatData, 1);
		}
		
		PrivateChat_parent.SetActive(false);
		XEventManager.SP.SendEvent(EEvent.Chat_PrivateUserChange, m_allPrivateChat.Last.Value.ObjectName);
		m_allPrivateChat.Clear();
	}
	
	private void handleFactionChat(SC_Chat msg)
	{
		if ( !msg.HasObjectName )
			return;
		
		string color = "[linkcolor=00ff00]";
		
		string strContent = "";
		strContent += "[link=1][linkdata=T(2)N(";
		strContent += msg.ObjectName;
		strContent += ")]";
		strContent += color.Replace("[", "[link");
		strContent += msg.ObjectName + ": ";
		strContent += "[link=0][color=00ff00]";
		
		string output = "";
		handleChatData(msg.ChatData , ref output);
		strContent += output;
		
		strContent = string.Format(XStringManager.SP.GetString(1034), strContent);
		
		Zonghe_Content.SetChatContent(strContent);
		if ( _currentSel == 0 )
		{
			Tab_zonghe.Reposition();
			Tab_zonghe.repositionNow = true;
		}
		
		Banghui_Content.SetChatContent(strContent);
		if ( _currentSel == 3 )
		{
			Tab_banghui.Reposition();
			Tab_banghui.repositionNow = true;
		}
	}
	
	private void handleFactionNotice(SC_Chat msg)
	{
		string strContent1 = "[linkcolor=00ff00]" + XStringManager.SP.GetString(1032) + msg.ChatData;
		string strContent2 = "[linkcolor=00ff00]" + XStringManager.SP.GetString(1033) + msg.ChatData;
		
		Zonghe_Content.SetChatContent(strContent1);
		if ( _currentSel == 0 )
		{
			Tab_zonghe.Reposition();
			Tab_zonghe.repositionNow = true;
		}
		
		Banghui_Content.SetChatContent(strContent2);
		if ( _currentSel == 3 )
		{
			Tab_banghui.Reposition();
			Tab_banghui.repositionNow = true;
		}
	}
	
	// 处理聊天信息
	// 玩家信息格式 [T(1)N(name)C(classlevel)]
	// 物品信息格式 [T(2)N(name)U(ownerid)D(id)L(level)]
	public static void handleChatData(string input, ref string output)
	{
		int startPos = 0;
		while ( true )
		{
			string dumpStr = "";
			string clampStr = "";
			int insertPos = input.IndexOf("[", startPos);
			int endPos = input.IndexOf("]", startPos);
			string lastStr = input;
			if ( -1 == insertPos || -1 == endPos || !HyperLinkBase.GetClampStr(ref input, ref dumpStr, "[", "]", startPos) )
				break;
			
			bool find = false;
			string handledStr = "";
			if ( HyperLinkBase.GetClampStr(ref dumpStr, ref clampStr, "T(", ")",0) )
			{
				int type = int.Parse(clampStr);	
				switch ( type )
				{
				case 1:							// 处理玩家信息
					find = handleChatName(dumpStr, ref handledStr);
					
					break;
				case 2:							// 处理物品信息
					find = handleChatItem(dumpStr, ref handledStr);
					
					break;
				default:
					break;
				}
			}
			if ( find )
			{
				input = input.Insert(insertPos, handledStr);
				startPos += handledStr.Length;
			}
			else
			{
				input = lastStr;
				startPos = endPos + 1;
			}
		}
		output = input;
	}
	
	//  处理玩家信息
	public static bool handleChatName(string input, ref string output)
	{
		string name = "";
		string classLevel = "";
		if ( !HyperLinkBase.GetClampStr(ref input, ref name, "N(", ")",0) )
			return false;
		
		if ( !HyperLinkBase.GetClampStr(ref input, ref classLevel, "C(", ")",0) )
			return false;

		string strContent = "";
		string color = XGameColorDefine.JJ_Color[int.Parse(classLevel) > XGameColorDefine.JJ_Color.Length ? 0 : int.Parse(classLevel)];
		strContent += "[link=1][linkdata=T(2)N(";
		strContent += name;
		strContent += ")]";
		strContent += color.Replace("[", "[link");
		strContent += name;
		strContent += "[link=0]";
		
		output = strContent;
		return true;
	}
	
	// 处理物品信息
	public static bool handleChatItem(string input, ref string output)
	{
		string name = "";
		string level = "";
		string id = "";
		string userId = "";
		if ( !HyperLinkBase.GetClampStr(ref input, ref name, "N(", ")",0) )
			return false;
		
		if ( !HyperLinkBase.GetClampStr(ref input, ref userId, "U(", ")",0) )
			return false;
		
		if ( !HyperLinkBase.GetClampStr(ref input, ref id, "D(", ")",0) )
			return false;
		
		if ( !HyperLinkBase.GetClampStr(ref input, ref level, "L(", ")",0) )
			return false;
		
		// [link=1][linkdata=T(1)D1(userid)D2(itemid)]结束[link=0]
		int lv = int.Parse(level) > XGameColorDefine.Quality_Color.Length ? 0 : int.Parse(level);
		string strContent = "";
		string color = XGameColorDefine.Quality_Color[lv];
		strContent += "[link=1][linkdata=T(1)N(";
		strContent += name;
		strContent += ")D1(";
		strContent += userId;
		strContent += ")D2(";
		strContent += id;
		strContent += ")]";
		strContent += color.Replace("[", "[link");
		strContent += string.Format(XStringManager.SP.GetString(1042), name);
		strContent += "[link=0]";
		
		output = strContent;
		return true;
	}
	
	private void onClickMax(GameObject go)
	{
		if ( 2 == bMax )
			return;
		
		for ( int i = 0; i < Scroll_Bar.Length; i++ )
		{
			Scroll_Bar[i].transform.position = new Vector3(Scroll_Bar[i].transform.position.x, 
				Scroll_Bar[i].transform.position.y + 85f,
				Scroll_Bar[i].transform.position.z);
		}
		
		for ( int i = 0; i < Scroll_Bar_bk.Length; i++ )
		{
			Scroll_Bar_bk[i].transform.localScale = new Vector3(Scroll_Bar_bk[i].transform.localScale.x,
				Scroll_Bar_bk[i].transform.localScale.y + 85f,
				Scroll_Bar_bk[i].transform.localScale.z);
		}
		
		for ( int i = 0; i < Scroll_Bar_bk.Length; i++ )
		{
			Scroll_Bar_fr[i].transform.localScale = new Vector3(Scroll_Bar_fr[i].transform.localScale.x,
				Scroll_Bar_fr[i].transform.localScale.y + 85f,
				Scroll_Bar_fr[i].transform.localScale.z);
		}
		
		for( int i = 0; i < ScrollButton_Down.Length; i++ )
		{
			ScrollButton_Down[i].transform.position = new Vector3(ScrollButton_Down[i].transform.position.x, 
				ScrollButton_Down[i].transform.position.y - 85f,
				ScrollButton_Down[i].transform.position.z);
		}
		
		BKGROUND_SPRITE.transform.localScale = new Vector3(BKGROUND_SPRITE.transform.localScale.x,
			BKGROUND_SPRITE.transform.localScale.y + 85f,
			BKGROUND_SPRITE.transform.localScale.z);
		
		PrivateChat_parent.transform.localPosition =  new Vector3(PrivateChat_parent.transform.localPosition.x,
			PrivateChat_parent.transform.localPosition.y + 85f,
			PrivateChat_parent.transform.localPosition.z);
		
		for ( int i = 0; i < Object_Panel.Length; i++ )
		{
			UIPanel panel = Object_Panel[i].GetComponent<UIPanel>();
			if ( panel != null )
			{
				panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y + 43, panel.clipRange.z, panel.clipRange.w + 85);
			}
		}
		bMax++;
	}
	
	private void onClickMin(GameObject go)
	{
		if ( 0 == bMax )
			return;
		
		for ( int i = 0; i < Scroll_Bar.Length; i++ )
		{
			Scroll_Bar[i].transform.position = new Vector3(Scroll_Bar[i].transform.position.x, 
				Scroll_Bar[i].transform.position.y - 85f,
				Scroll_Bar[i].transform.position.z);
		}
		
		for ( int i = 0; i < Scroll_Bar_bk.Length; i++ )
		{
			Scroll_Bar_bk[i].transform.localScale = new Vector3(Scroll_Bar_bk[i].transform.localScale.x,
				Scroll_Bar_bk[i].transform.localScale.y - 85f,
				Scroll_Bar_bk[i].transform.localScale.z);
		}
		
		for ( int i = 0; i < Scroll_Bar_fr.Length; i++ )
		{
			Scroll_Bar_fr[i].transform.localScale = new Vector3(Scroll_Bar_fr[i].transform.localScale.x,
				Scroll_Bar_fr[i].transform.localScale.y - 85f,
				Scroll_Bar_fr[i].transform.localScale.z);
		}
		
		for( int i = 0; i < ScrollButton_Down.Length; i++ )
		{
			ScrollButton_Down[i].transform.position = new Vector3(ScrollButton_Down[i].transform.position.x, 
				ScrollButton_Down[i].transform.position.y + 85f,
				ScrollButton_Down[i].transform.position.z);
		}
		
		BKGROUND_SPRITE.transform.localScale = new Vector3(BKGROUND_SPRITE.transform.localScale.x,
			BKGROUND_SPRITE.transform.localScale.y - 85f,
			BKGROUND_SPRITE.transform.localScale.z);
		
		PrivateChat_parent.transform.localPosition =  new Vector3(PrivateChat_parent.transform.localPosition.x,
				PrivateChat_parent.transform.localPosition.y - 85f,
				PrivateChat_parent.transform.localPosition.z);
		
		for ( int i = 0; i < Object_Panel.Length; i++ )
		{
			UIPanel panel = Object_Panel[i].GetComponent<UIPanel>();
			if ( panel != null )
			{
				panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y - 43, panel.clipRange.z, panel.clipRange.w - 85);
			}
		}		
		bMax--;
	}
}
