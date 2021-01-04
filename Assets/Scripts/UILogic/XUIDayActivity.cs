using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUIDayActivity : XDefaultFrame
{
	[System.Serializable]
	public class XUIDayActivityItem
	{
		class XUIDayActivityItemIcon
		{
			UISprite icon;
			
			public XUIDayActivityItemIcon()
			{
				
			}
			
			public void SetIcon(UISprite _icon)
			{
				icon = _icon;
			}
			
			public void SetIconSprite(string _strSprite)
			{
				if(icon != null)
					icon.spriteName = _strSprite;
			}
			
			public void SetIconTips(string _strText)
			{
				if(icon != null)
					icon.CommonTips = _strText;
			}
		}
		
		private static readonly int MAX_AWARDICON = 4;
		private uint mItemID = 0;
		private ushort mOpenPanelId = 0;
		private UILabel mItemActivityValue;
		private UILabel mItemName;
		private UILabel mProgress;
		private UISprite mProgressComplate;
		private UIImageButton mItemButton;
		private XActionIcon[] 	mAwardItemIconArr = new XActionIcon[4];
		private XUIDayActivityItemIcon[] mItemIconArr = new XUIDayActivityItemIcon[4];
		private XDayActivityItem.EActivityStatus mStatus;
		private string mTips = "";
		
		public XUIDayActivityItem(uint _itemID, GameObject _go)
		{
			mItemID = _itemID;

			//链接PanelID
			mOpenPanelId = 0;

			//该活动项目名称
			mItemName = _go.gameObject.transform.Find("Label_ItemName").GetComponent<UILabel>();
			if(mItemName == null)	
				Log.Write(LogLevel.ERROR, "XUIDayActivityItem mItemName is null, not found Label_ItemName");
			
			//该活动项目增加的活跃度值
			mItemActivityValue = _go.gameObject.transform.Find("Label_ActivityValue").GetComponent<UILabel>();
			if(mItemActivityValue == null)
				Log.Write(LogLevel.ERROR, "XUIDayActivityItem mItemActivityValue is null, not found Label_ActivityValue");
			
			//该活动项目链接的UI
			mItemButton = _go.gameObject.transform.Find("EntryLink").GetComponent<UIImageButton>();
			if(mItemButton == null)
				Log.Write(LogLevel.ERROR, "XUIDayActivityItem mItemButton is null, not found EntryLink");
			
			mProgress = _go.gameObject.transform.Find("Progress_Label").GetComponent<UILabel>();
			if(mItemActivityValue == null)
				Log.Write(LogLevel.ERROR, "XUIDayActivityItem mProgress is null, not found Progress_Label"); 
			
			mProgressComplate = _go.gameObject.transform.Find("Progress_Sprite").GetComponent<UISprite>();
			if(mProgressComplate == null)
				Log.Write(LogLevel.ERROR, "XUIDayActivityItem mProgressComplate is null, not found Progress_Sprite");
			
			/*//该活动项目的奖励显示
			for(int cnt = 0;cnt != MAX_AWARDICON; ++cnt)
			{
				mAwardItemIconArr[cnt] = _go.gameObject.transform.FindChild("ActionIcon" + cnt).GetComponent<XActionIcon>();
				if(mAwardItemIconArr[cnt] == null)
					Log.Write(LogLevel.ERROR, "XUIDayActivityItem mAwardItemArr" + cnt + "is null, not found ActionIcon" + cnt);
			
			}*/
					
			for(int cnt = 0;cnt != MAX_AWARDICON; ++cnt)
			{
				UISprite icon = _go.gameObject.transform.Find("ItemAwardTips" + cnt + "/icon").GetComponent<UISprite>();
			
				if(icon == null)
					Log.Write(LogLevel.ERROR, "XUIDayActivityItem icon" + cnt + "is null, not found ItemAwardTips" + cnt);
				
				mItemIconArr[cnt].SetIcon(icon);
				
			}
				
			UIEventListener listenerEntryLink = UIEventListener.Get (mItemButton.gameObject);
			listenerEntryLink.onClick += OnClickEntryLink;
			listenerEntryLink.onHover += OnHoverEntryLink;
		}
		
		private void OnClickEntryLink(GameObject go)
		{
			if(mStatus == XDayActivityItem.EActivityStatus.Available)
			{
				EUIPanel panel = EUIPanel.eCount;
				try{
					panel = (EUIPanel)mOpenPanelId;
				}catch{
					Log.Write(LogLevel.ERROR,"XUIDayActivityItem, OpenUI panleID ERROR");	
				}
				if(!XUIManager.SP.GetUIControl(panel).IsLogicShow)
				{
					XEventManager.SP.SendEvent(EEvent.UI_Show, panel);
				}
				Debug.Log("Open Panel:" + panel.ToString());
			}
			else if(mStatus == XDayActivityItem.EActivityStatus.NotAvailable)
			{
				XCfgDayActivityBase xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(mItemID);
				if(xcfg == null)
				{
					Debug.LogError("xcfg not found mItemID:"+mItemID);
					return;
				}
				FeatureUnLock fxcfg = FeatureUnLockMgr.SP.GetConfig(xcfg.FUID);
				if(fxcfg == null)
				{
					Debug.LogError("fxcfg not found xcfg.FUID");
					return;
				}
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,9890,fxcfg.RequireLevel);
			}
		}
		
		private void OnHoverEntryLink(GameObject _go,bool _state)
		{
			if(_state)
			{
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipC);
				XEventManager.SP.SendEvent(EEvent.ToolTip_C, mTips);
			}else
			{
				XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
			}
		}
		
		private void SetLinkStatus()
		{
			switch(mStatus)
			{
				case XDayActivityItem.EActivityStatus.Available:
					SetSprite("");
					//SetTips("NotAvailable");
					break;
				case XDayActivityItem.EActivityStatus.NotAvailable:
					SetSprite("");
					//SetTips("Wahaha");
					break;
				case XDayActivityItem.EActivityStatus.Complated:
					SetSprite("");
					//SetTips("Wahaha");
					break;
			}
		}
		
		private void SetSprite(string str)
		{
			mItemButton.normalSprite = str;
			mItemButton.hoverSprite = str;
			mItemButton.pressedSprite = str;
			mItemButton.UpdateImage();
		}
		
		public void SetIcon()
		{
			XCfgDayActivityBase xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(mItemID);
			if(xcfg == null)
				return;
			for(int cnt = 0;cnt != MAX_AWARDICON; ++cnt)
			{
				
	
			}
		}
		
		public void SetCurProgress(uint _curValue, uint _totalValue)
		{
			if(_curValue < _totalValue)
			{
				if(mProgressComplate.gameObject.activeInHierarchy)
					mProgressComplate.gameObject.SetActive(false);
				mProgress.text = _curValue.ToString() + "/" + _totalValue.ToString();
			}
			else
			{
				mProgress.text = "";
				mProgressComplate.gameObject.SetActive(true);
			}
		}
		
		public void SetStatus(int _status)
		{
			mStatus = (XDayActivityItem.EActivityStatus)_status;
			if(mStatus == XDayActivityItem.EActivityStatus.NotAvailable)
			{
				
			}
			
			else if(mStatus == XDayActivityItem.EActivityStatus.Available)
			{
				
			}
			
			else if(mStatus == XDayActivityItem.EActivityStatus.Complated)
			{
				
			}
		}
		
		public void SetName(string _strName)
		{
			if(mItemName == null)
				return;
			mItemName.text = _strName;
		}
		
		public void SetEntryItemName(string _strName)
		{
			UILabel entryName = mItemButton.transform.Find("Label").GetComponent<UILabel>();
			if(entryName == null)
			{
				Debug.LogError("EntryItem text is null, not found Label");
				return;
			}
			entryName.text = _strName;
		}
		
		public void SetActivityValue(int _value)
		{
			if(mItemActivityValue == null)
				return;
			mItemActivityValue.text = _value.ToString();
		}
		
		public void SetLinkUi(ushort _panelID)
		{
			if(mOpenPanelId == _panelID)
				return;
			mOpenPanelId = _panelID;
		}
	}
	
	[System.Serializable]
	public class XUIDayActivityAward
	{
		private static readonly string[] str = {"11001150","11001151","11001152"};
		private uint mAwardID;
		private string mTips;
		private XDayActivityAward.EAwardStatus mAwardStatus;
		private UIImageButton mButton;
		
		public XUIDayActivityAward(uint _mAwardID, XDayActivityAward.EAwardStatus _AwardStatus, GameObject _go)
		{
			mAwardID = _mAwardID;
			mAwardStatus = _AwardStatus;
			mButton = _go.gameObject.GetComponent<UIImageButton>();
			if(mButton == null)
			{
				Debug.LogError("XUIDayActivityAward mButton is null");
				return;
			}
			UIEventListener listenerButton = UIEventListener.Get(mButton.gameObject);
			listenerButton.onClick += OnClick;
			listenerButton.onHover += OnHover;
		}
		
		private void OnClick(GameObject _go)
		{
			if(mAwardStatus == XDayActivityAward.EAwardStatus.Available)
			{
				XDayActivityManager.SP.RequestGetReward(mAwardID);
				Debug.Log("RequestGetReward awardID:"+ mAwardID);
			}
			else if(mAwardStatus == XDayActivityAward.EAwardStatus.NotAvailable)
			{
				
			}
			else if(mAwardStatus == XDayActivityAward.EAwardStatus.Received)
			{
			
			}
		}
		
		private void OnHover( GameObject _go,bool _state)
		{
			if(_state)
			{
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipC);
				XEventManager.SP.SendEvent(EEvent.ToolTip_C, mTips);
			}else
			{
				XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
			}
		}
		
		public void SetStatus(XDayActivityAward.EAwardStatus _AwardStatus)
		{
			switch(_AwardStatus)
			{
				case XDayActivityAward.EAwardStatus.NotAvailable:
					SetSprite(str[1]);
					SetTips("NotAvailable");
					break;
				case XDayActivityAward.EAwardStatus.Available:
					SetSprite(str[0]);
					SetTips("Wahaha");
					break;
				case XDayActivityAward.EAwardStatus.Received:
					SetSprite(str[2]);
					SetTips("Wahaha");
					break;
			}
		}

		public void SetTips(string _tips)
		{
			mTips = _tips;
		}
		
		private void SetSprite(string _spriteStr)
		{
			mButton.normalSprite = _spriteStr;
			mButton.hoverSprite = _spriteStr;
			mButton.pressedSprite = _spriteStr;
			mButton.UpdateImage();
		}
	}
	
	private static readonly int MAX_AWARD_ITEM = 5;
	public GameObject ItemPrefab;
	public UIGrid ActivityItemList;
	public UILabel CurActivityValue;
	public UISlider ActivityProgressSlider;
	
	public UIImageButton[] AwardItemList = new UIImageButton[MAX_AWARD_ITEM];
	public SortedList<uint,XUIDayActivityItem> mUIItemlist = new SortedList<uint, XUIDayActivityItem>();
	private SortedList<uint,XUIDayActivityAward> mAwardList = new SortedList<uint, XUIDayActivityAward>();
	
	public override bool Init ()
	{
		base.Init ();
		if(ItemPrefab == null)
		{
			Log.Write(LogLevel.ERROR,"ItemPrefab is null");
			return false;
		}
		if(ActivityItemList == null)
		{
			Log.Write(LogLevel.ERROR,"ActivityItemList is null");
			return false;
		}
		
		if(CurActivityValue == null)
		{
			Log.Write(LogLevel.ERROR,"CurActivityValue is null");
			return false;
		}
		
		if(ActivityProgressSlider == null)
		{
			Log.Write(LogLevel.ERROR,"ActivityProgressSlider is null");
			return false;
		}
		
		for(int cnt = 0; cnt != MAX_AWARD_ITEM; ++cnt)
		{
			if(AwardItemList[cnt] == null)
			{
				Log.Write(LogLevel.ERROR,"AwardItem" + cnt + "is null");
				return false;
			}
		}
		
		UIEventListener listernExit = UIEventListener.Get(ButtonExit.gameObject);
		listernExit.onClick += HideThisUI;
		return true;
	}
	
	public override void Hide ()
	{
		base.Hide ();
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
	}
	
	private void HideThisUI(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eDayActivity);
	}
	
	public void SetCurActivityValue(int _value)
	{
		if(CurActivityValue == null)
			return;
		CurActivityValue.text = _value.ToString();
	}
	
	public void SetProgressValue(int _value)
	{
		if(_value > 100)
			_value = 100;
		float minValue = (float)_value / 100;
		ActivityProgressSlider.sliderValue = minValue;
		ActivityProgressSlider.ForceUpdate();
	}
	
	public XUIDayActivityItem GetUIActivityItem(uint _itemID)
	{
		if(mUIItemlist.ContainsKey(_itemID))
		{
			return mUIItemlist[_itemID];
		}
		return null;
	}
	
	public void UpdateActivityItem(uint _itemID,int _status, uint _curProgress)
	{
		XCfgDayActivityBase xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(_itemID);
		if(xcfg == null)
		{
			Debug.LogError("XUIDayActivity UpdateActivityItem not found itemid:" + _itemID);
			return;
		}
		if(!mUIItemlist.ContainsKey(_itemID))
		{
			Debug.LogError("XUIDayActivity UpdateActivityItem itemid not in mUIItemlist:" + _itemID);
			return;
		}
		
		mUIItemlist[_itemID].SetCurProgress(_curProgress,xcfg.Completions);
		mUIItemlist[_itemID].SetStatus(_status);
		ActivityItemList.repositionNow = true;
	}
				
	public void AddActivityItem(uint _itemID,int _status, uint _curProgress)
	{
		XCfgDayActivityBase xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(_itemID);
		if(xcfg == null)
		{
			Debug.LogError("XUIDayActivity AddActivityItem not found itemid:" + _itemID);
			return;
		}
		if(mUIItemlist.ContainsKey(_itemID))
		{
			Debug.LogError("XUIDayActivity AddActivityItem itemid in mUIItemlist:" + _itemID);
			return;
		}
		GameObject tempGameObject = XUtil.Instantiate(ItemPrefab,ActivityItemList.transform);
		tempGameObject.name =  _status.ToString() + "_" + "Sort_" + xcfg.SortLevel.ToString() + "_" + _itemID.ToString() + xcfg.Name.ToString();
		if(!tempGameObject.activeInHierarchy)
			tempGameObject.SetActive(true);
		XUIDayActivityItem item = new XUIDayActivityItem(_itemID,tempGameObject);
		item.SetName(xcfg.Name);
		item.SetEntryItemName(xcfg.ButtonName);
		item.SetCurProgress(_curProgress,xcfg.Completions);
		item.SetActivityValue(xcfg.VitalityValue);
		item.SetStatus(_status);
		item.SetLinkUi(xcfg.LinkUI);
		mUIItemlist.Add(_itemID,item);
		ActivityItemList.repositionNow = true;
	}

	
	public XUIDayActivityAward GetUIActivityAward(uint _awardID)
	{
		if(mAwardList.ContainsKey(_awardID))
		{
			return mAwardList[_awardID];
		}
		return null;
	}
	
	public void UpdateAward(uint _awardID,XDayActivityAward.EAwardStatus _status)
	{
		if(!mAwardList.ContainsKey(_awardID))
		{
			return;
		}
		mAwardList[_awardID].SetStatus(_status);
	}
	
	public void AddAwardItem(uint _awardID,int idx, XDayActivityAward.EAwardStatus _status)
	{
		if(mAwardList.ContainsKey(_awardID))
		{
			return;
		}
		XUIDayActivityAward awardItem = new XUIDayActivityAward(_awardID,_status,AwardItemList[idx].gameObject);
		awardItem.SetStatus(_status);
		mAwardList.Add(_awardID,awardItem);
	}
}