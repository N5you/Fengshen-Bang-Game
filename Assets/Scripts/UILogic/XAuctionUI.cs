using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XAuctionUI")]
public class XAuctionUI : XDefaultFrame
{
	[System.Serializable]
	public class SoldHistroyInfoMgr
	{
		public SoldHistroyInfoMgr()
		{
		}
		
		public void Init(XAuctionUI parent)
		{
			_parent = parent;
			UIEventListener lis = UIEventListener.Get(Button_ToSold);
			lis.onClick = OnClickToSold;
		}
		
		public void UpdateInfo(AuctionHistyoyInfo info)
		{
			Label_CG_TotalCount.text = string.Format(XStringManager.SP.GetString(1050u), info.TotalCount);
			Label_Total_Income.text = info.TotalIncome.ToString();
			Label_Total_Spending.text = info.TotalSpending.ToString();
			Label_OnSold_Count.text = string.Format(XStringManager.SP.GetString(1050u), info.OnSoldCount);
			Label_Today_Income.text = info.TodayIncome.ToString();
			Label_Today_CanPub.text = string.Format(XStringManager.SP.GetString(1051u), info.LeftPubCount);
		}
		
		public void OnClickToSold(GameObject go)
		{
			_parent.ShowSoldPanel();
		}
		
		public UILabel Label_CG_TotalCount;								// 出售总量
		public UILabel Label_Total_Income;								// 总收益
		public UILabel Label_Total_Spending;							// 总花销
		public UILabel Label_OnSold_Count;								// 当前正在出售数量
		public UILabel Label_Today_Income;								// 当天收益
		public UILabel Label_Today_CanPub;								// 当天可上架次数
		public GameObject Button_ToSold;								// 出售按钮
		
		private XAuctionUI _parent;
	}
	
	[System.Serializable]
	public class SoldPanelInfoMgr
	{
		public SoldPanelInfoMgr()
		{
		}
		
		public void Init(XAuctionUI parent)
		{
			_parent = parent;
			UIEventListener lis = UIEventListener.Get(Button_Sold.gameObject);
			lis.onClick = OnClickSold;
			
			lis = UIEventListener.Get(Button_ShowHisInfo.gameObject);
			lis.onClick = onClickShowHisInfo;
			
			_sellAction.SetUIIcon(Sold_Action);
			
			reset();
		}
		
		public void UpdateItemSoldInfo(uint id, UInt64 price, UInt64 pubCount)
		{
			_sellAction.IsCanToolTip = true;
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((int)ActionIcon_Type.ActionIcon_Bag, (short)id);
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
			
			_sellAction.SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, item.Color, 1);
			_sellAction.SetLogicData(ActionIcon_Type.ActionIcon_Auction, id);
			
			_realIndex = (ushort)id;
			_number = item.ItemCount;
			
			Label_Item_Name.text = XGameColorDefine.Quality_Color[(int)item.Color] + cfgItem.Name;
			if ( 0u != price )
			{
				Label_Item_CurrPrice.text = price.ToString();
				MoneyIcon_Obj.SetActive(true);
			}
			else
			{
				Label_Item_CurrPrice.text = XStringManager.SP.GetString(1060u);
				MoneyIcon_Obj.SetActive(false);
			}
			Label_Item_Count.text = item.ItemCount.ToString();
		}
		
		private void OnClickSold(GameObject go)
		{
			if ( _number > 0 )
				XLogicWorld.SP.AuctionManager.AddAuction((uint)_realIndex, _number);
			reset();
		}
		
		public void reset()
		{
			_number = 0;
			_sellAction.IsCanToolTip = false;
			_sellAction.Reset();
			Label_Item_Name.text = "";
			Label_Item_CurrPrice.text = "";
			MoneyIcon_Obj.SetActive(false);
			Label_Item_Count.text = "";
		}
		
		private void onClickShowHisInfo(GameObject go)
		{
			_parent.ShowHistroyInfoPanel();
		}
		
		public void UpdatePrice(uint price, SItemColorID idcolor)
		{
			Label_Item_CurrPrice.text = price.ToString();
			MoneyIcon_Obj.SetActive(true);
		}
		
		public void UpdateCanPubCount(UInt64 pubCount)
		{
			Label_CanPub_Count.text = string.Format(XStringManager.SP.GetString(1051u), pubCount);
		}
		
		public UIImageButton Button_Sold;							// 出售按钮
		public UIImageButton Button_ShowHisInfo;					// 查查看历史信息按钮
		
		public UILabel Label_Item_Name;								// 物品名称
		public UILabel Label_Item_CurrPrice;						// 物品当前价格
		public GameObject MoneyIcon_Obj;							// 金钱图标
		public UILabel Label_Item_Count;							// 物品个数
		public UILabel Label_CanPub_Count;							// 当前玩家当天可发布的数量
		public XActionIcon Sold_Action;								// 需要出售物品显示信息
		
		private XAuctionUI _parent;
		private XAuctionSellIcon _sellAction = new XAuctionSellIcon();
		
		private ushort _realIndex;										// 物品在包裹中的位置
		private uint _number = 0;										// 出售数量
	}
	
	[System.Serializable]
	public class MyAuctionInfoMgr
	{
		private class MyOneAuctionInfo : XBaseActionIcon
		{
			public MyOneAuctionInfo(GameObject obj, XMyAuction auction)
			{
				_id = auction.ID;
				
				// 物品显示
				Transform transform = obj.transform.Find("ActionIcon");
				if ( null == transform )
					return;
				SetUIIcon(transform.GetComponent<XActionIcon>());
				XCfgItem cfgItem = auction.Config;
				IconData = (int)cfgItem.Index;
				IsCanPopMenu = false;
				if(null != cfgItem)
					SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, (EItem_Quality)(auction.ItemColorID.Quality), 1);
				
				// 数量
				transform = obj.transform.Find("Label_Count");
				if ( null == transform )
					return;
				transform.GetComponent<UILabel>().text = auction.Number.ToString();
				
				// 剩余时间
				transform = obj.transform.Find("Label_LeftTime");
				if ( null == transform )
					return;
				
				uint leftSec = auction.RemainTime;
				uint leftHour = leftSec / (60 * 60);
				uint leftMin = 0u;
				leftSec = leftSec % (60 * 60);
				if ( leftHour > 0 )
				{   
					if ( leftSec > 0 )
						leftHour++;
				}
				else
				{
					leftMin = leftSec / 60;
					leftSec = leftSec % 60;
					if ( leftSec > 0 )
						leftMin++;
				}
				
				string leftTimeStr = "";
				if ( leftHour > 0 )
					leftTimeStr = string.Format(XStringManager.SP.GetString(1062u), leftHour);
				else if ( leftMin > 0 )
					leftTimeStr = string.Format(XStringManager.SP.GetString(1063u), leftMin);

				transform.GetComponent<UILabel>().text = leftTimeStr;
				
				// 名称
				transform = obj.transform.Find("Label_Name");
				if ( null == transform )
					return;
				transform.GetComponent<UILabel>().text = XGameColorDefine.Quality_Color[(int)auction.ItemColorID.Quality] + cfgItem.Name;
				
				// 下线按钮
				transform = obj.transform.Find("OffLineButton");
				if ( null == transform )
					return;
				UIEventListener.Get(transform.gameObject).onClick = OnClickItemCancel;
				
				transform = obj.transform.Find("BK");
				if ( null != transform )
					bkObj = transform.gameObject;
			}
			
			public void ShowBk(int index)
			{
				int t = 1;
				bkObj.SetActive((t & index) == 0);
			}
			
			private void OnClickItemCancel(GameObject go)
			{
				XLogicWorld.SP.AuctionManager.DelAuction(_id);
			}
			
			private uint _id;
			private GameObject bkObj;
		}
		
		public MyAuctionInfoMgr()
		{
		}
		
		public void Show(SortedList<XMyAuction, XMyAuction> mySortedAuction)
		{
			Clear();
			int index = 0;
			foreach ( KeyValuePair<XMyAuction, XMyAuction> auction in mySortedAuction )
			{
				GameObject obj = XUtil.Instantiate(AuctionInfoSample);
				obj.SetActive(true);
				obj.transform.parent = ObjParentTable.transform;
				ObjParentTable.repositionNow = true;
				m_allMyAuctionObj.AddFirst(obj);
				MyOneAuctionInfo info = new MyOneAuctionInfo(obj, auction.Value);
				info.ShowBk(index++);
			}
		}
		
		private void Clear()
		{
			foreach ( GameObject obj in m_allMyAuctionObj )
			{
				GameObject.Destroy(obj);
			}
		}
		
		public void SetNoDataAppear(bool see)
		{
			GameObject_NoDataLabel.SetActive(see);
		}
		
		private LinkedList<GameObject> m_allMyAuctionObj = new LinkedList<GameObject>(); 
		public GameObject AuctionInfoSample;
		public UITable ObjParentTable;
		public GameObject GameObject_NoDataLabel;
	}
	
	[System.Serializable]
	public class SoldAuctionInfoMgr
	{
		class AuctionNode
		{
			private int _nodeId;
			private GameObject itemObject;
			private UICheckbox CheckBoxObj;
			private UICheckbox CheckBoxControlButton;
			
			private LinkedList<UICheckbox> m_allChildCheckBox = new LinkedList<UICheckbox>();
			
			public AuctionNode()
			{
			}
			
			public GameObject init(string name, int nodeID, GameObject parent, UITree rootTree, AuctionNode parentNode)
			{
				if ( null == parent )
				{
					itemObject = rootTree.insertNode(name);
					UIEventListener lis = UIEventListener.Get(itemObject.GetComponent<UITreeParentNode>().m_controlCheckBox);
					lis.onClick += onClick;
					
					lis = UIEventListener.Get(itemObject.GetComponent<UITreeParentNode>().m_controlObj);
					lis.onClick += OnClickTreeControlButton;
					CheckBoxControlButton = itemObject.GetComponent<UITreeParentNode>().m_controlObj.GetComponent<UICheckbox>();
					
					CheckBoxObj = itemObject.GetComponent<UITreeParentNode>().m_controlCheckBox.GetComponent<UICheckbox>();
				}
				else
				{
					itemObject = rootTree.insertItem(name, parent);
					NGUITools.AddWidgetCollider(itemObject, true);
					UIEventListener lis = UIEventListener.Get(itemObject);
					lis.onClick += onClick ;
					
					CheckBoxObj = itemObject.GetComponent<UICheckbox>();
					if ( CheckBoxObj.isChecked )
						CheckBoxObj.isChecked = false;
					
					parentNode.AddChildCheckBox(CheckBoxObj);
				}
	
				_nodeId = nodeID;
				return itemObject;
			}
			
			public void AddChildCheckBox(UICheckbox boxObject)
			{
				m_allChildCheckBox.AddLast(boxObject);
			}
			
			public void OnClickTreeControlButton(GameObject go)
			{
				if ( CheckBoxControlButton.isChecked )
					return;
				
				foreach ( UICheckbox obj in m_allChildCheckBox )
				{
					if ( obj.isChecked )
					{
						obj.isChecked = false;
						XLogicWorld.SP.AuctionManager.ChangeFilter(0);
					}
				}
			}
			
			public void OpenTree()
			{
				itemObject.GetComponent<UITreeParentNode>().SetSelected(true, true);
			}
			
			private void onClick(GameObject go)
			{
				if ( CheckBoxObj.isChecked )
					XLogicWorld.SP.AuctionManager.ChangeFilter(_nodeId);
				else
					XLogicWorld.SP.AuctionManager.ChangeFilter(0);
			}
		}
			
		public SoldAuctionInfoMgr()
		{
		}
		
		public void Init()
		{
			tree.SetScrollBar(bar);
			
			UIEventListener lis;
			lis = UIEventListener.Get(Butten_Search.gameObject);
			lis.onClick = OnClickSearch;
			
			lis = UIEventListener.Get(Button_Level_Sort.gameObject);
			lis.onClick = onClickButtonLevelSort;
			
			lis = UIEventListener.Get(Button_Price_Sort.gameObject);
			lis.onClick = onClickButtonPriceSort;
			
			lis = UIEventListener.Get(Button_Name_Sort.gameObject);
			lis.onClick = onClickButtonNameSort;
			
			PopList_Search.items.Clear();
			for ( int i = 1052; i <= 1058; i++ )
			{
				string color = XGameColorDefine.Quality_Color[i - 1052];
				PopList_Search.items.Add(color + XStringManager.SP.GetString((uint)i));
				m_allColorLevels[color + XStringManager.SP.GetString((uint)i)] = i - 1052;
			}
			
			bool firstOpen = false;
			SortedList<int, GameObject> parentObjList = new SortedList<int, GameObject>();
			SortedList<int, AuctionNode> parentNodeList = new SortedList<int, AuctionNode>();
			foreach( KeyValuePair<ushort, SortedList<ushort, XCfgAcutionConfig>> parentConfig in XCfgAcutionConfigMgr.SP.ItemTable )
			{
				SortedList<ushort, XCfgAcutionConfig> configChild = parentConfig.Value;
				foreach ( KeyValuePair<ushort, XCfgAcutionConfig> config in configChild)
				{
					if ( !parentObjList.ContainsKey(parentConfig.Key) )
					{
						string nameParent = config.Value.ItemTypeName;
						int idParent = config.Value.ItemType << 16;
						AuctionNode node = new AuctionNode();
						parentObjList[parentConfig.Key] = node.init(nameParent, idParent, null, tree, null);
						parentNodeList[parentConfig.Key] = node;
						if ( !firstOpen )							//  默认第一个树节点为打开状态
						{
							node.OpenTree();
							firstOpen = true;
						}
					}
					string name = config.Value.ItemsSubName;
					int id = ( (config.Value.ItemType << 16) | config.Value.ItemSubType );
					new AuctionNode().init(name, id, parentObjList[parentConfig.Key], tree, parentNodeList[parentConfig.Key]);
				}
			}
			
			Label_Curr_Total_Count.text = "0/0";
			lis = UIEventListener.Get(Button_Show_Next.gameObject);
			lis.onClick = ShowNext;
			lis = UIEventListener.Get(Button_Show_privous.gameObject);
			lis.onClick = ShowPre;
			
			lis = UIEventListener.Get(Button_BuyIncrease.gameObject);
			lis.onClick = onClickBuyNumIncrease;
			
			lis = UIEventListener.Get(Button_BuyDecrerase.gameObject);
			lis.onClick = onClickBuyNumDecrease;
			
			lis = UIEventListener.Get(Buttom_Max.gameObject);
			lis.onClick = onClickButMax;
			
			lis = UIEventListener.Get(Button_Buy.gameObject);
			lis.onClick = onClickBuyAuction;
			
			Radiao_UserSelect.onRadioChanged = userSelect;
			
			AuctionInfoObjs[0] = new XBaseActionIcon();
			AuctionInfoObjs[0].IsCanDrag = false;
			AuctionInfoObjs[0].IsCanDrop = false;
			AuctionInfoObjs[0].IsCanPopMenu = false;
			AuctionInfoObjs[0].SetUIIcon(AuctionShowObjs[0].transform.Find("ActionIcon1").GetComponent<XActionIcon>());
			
			AuctionInfoObjs[1] = new XBaseActionIcon();
			AuctionInfoObjs[1].IsCanDrag = false;
			AuctionInfoObjs[1].IsCanDrop = false;
			AuctionInfoObjs[1].IsCanPopMenu = false;
			AuctionInfoObjs[1].SetUIIcon(AuctionShowObjs[1].transform.Find("ActionIcon2").GetComponent<XActionIcon>());
			
			AuctionInfoObjs[2] = new XBaseActionIcon();
			AuctionInfoObjs[2].IsCanDrag = false;
			AuctionInfoObjs[2].IsCanDrop = false;
			AuctionInfoObjs[2].IsCanPopMenu = false;
			AuctionInfoObjs[2].SetUIIcon(AuctionShowObjs[2].transform.Find("ActionIcon3").GetComponent<XActionIcon>());
			
			AuctionInfoObjs[3] = new XBaseActionIcon();
			AuctionInfoObjs[3].IsCanDrag = false;
			AuctionInfoObjs[3].IsCanDrop = false;
			AuctionInfoObjs[3].IsCanPopMenu = false;
			AuctionInfoObjs[3].SetUIIcon(AuctionShowObjs[3].transform.Find("ActionIcon4").GetComponent<XActionIcon>());
			
			AuctionInfoObjs[4] = new XBaseActionIcon();
			AuctionInfoObjs[4].IsCanDrag = false;
			AuctionInfoObjs[4].IsCanDrop = false;
			AuctionInfoObjs[4].IsCanPopMenu = false;
			AuctionInfoObjs[4].SetUIIcon(AuctionShowObjs[4].transform.Find("ActionIcon5").GetComponent<XActionIcon>());
			
			LevelLabelSort.text = XStringManager.SP.GetString(1071);
			PriceLabelSort.text = XStringManager.SP.GetString(1073);
			NameLabelSort.text = XStringManager.SP.GetString(1069);
			
			Label_PlayerMoney.text = XLogicWorld.SP.MainPlayer.RealMoney.ToString();
		}
		
		private void OnClickSearch(GameObject go)
		{
			string name = Label_Search_Name.text;
			int minLvl = -1;
			string minLvlString = Label_Search_MinLevel.text;
			if ( !string.IsNullOrEmpty(minLvlString) )
				int.TryParse(Label_Search_MinLevel.text, out minLvl);
			int maxLvl = -1;
			string maxLvlString = Label_Search_MaxLevel.text;
			if ( !string.IsNullOrEmpty(maxLvlString) )
				int.TryParse(Label_Search_MaxLevel.text, out maxLvl);
			int color = m_allColorLevels[PopList_Search.textLabel.text];
			
			XLogicWorld.SP.AuctionManager.RequestAllAuction(name, minLvl, maxLvl, color);
			
			for( int i = 0; i < AuctionShowObjs.Length; i++ )
			{
				AuctionShowObjs[i].SetActive(false);
			}
			Label_Curr_Total_Count.text = "0/0";
			
			GameObject_FindClickTip.SetActive(false);
			RetSetSoldPanel();
		}
		
		public void Show(List<XAuctionInfo> allSortedAuction, string count)
		{
			Label_Curr_Total_Count.text = count;
			
			for( int i = 0; i < AuctionShowObjs.Length; i++ )
			{
				AuctionShowObjs[i].SetActive(false);
			}
			
			for( int i = 0; i < allSortedAuction.Count; i++ )
			{
				AuctionShowObjs[i].SetActive(true);
				
				UILabel label = AuctionShowObjs[i].transform.Find("Label_Count").GetComponent<UILabel>();
				string countStr = allSortedAuction[i].Number.ToString();
				if ( allSortedAuction[i].Number <= 0 )
					countStr = "[color=FF0000]" + countStr;
				label.text = countStr;
				
				label = AuctionShowObjs[i].transform.Find("Label_Level").GetComponent<UILabel>();
				label.text = allSortedAuction[i].Level.ToString();
				
				label = AuctionShowObjs[i].transform.Find("Label_Name").GetComponent<UILabel>();
				label.text = XGameColorDefine.Quality_Color[allSortedAuction[i].Config.QualityLevel] + allSortedAuction[i].Config.Name;
				
				label = AuctionShowObjs[i].transform.Find("Label_Price").GetComponent<UILabel>();
				label.text = allSortedAuction[i].Price.ToString();;
				
				AuctionInfoObjs[i].SetSprite(allSortedAuction[i].Config.IconAtlasID, 
				allSortedAuction[i].Config.IconID, (EItem_Quality)(allSortedAuction[i].ItemColorID.Quality), 1);
				AuctionInfoObjs[i].IconData = (int)allSortedAuction[i].Config.Index;
			}
			
			if ( allSortedAuction.Count > 0 )
			{
				userSelect(0);
				Radiao_UserSelect.CurrentSelect = 0;
			}
		}
		
		public void ShowNext(GameObject go)
		{
			XLogicWorld.SP.AuctionManager.ShowAuctionInfoPosChange(1, true);
		}
		
		public void ShowPre(GameObject go)
		{
			XLogicWorld.SP.AuctionManager.ShowAuctionInfoPosChange(-1, false);
		}
		
		private void onClickButtonLevelSort(GameObject go)
		{
			LevelDes = !LevelDes;
			uint nameId = 1070u;
			if ( LevelDes )
				nameId = 1071u;
			if ( LevelDes )
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.LEVEL_DEC);
			else
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.LEVEL_INC);
			
			LevelLabelSort.text = XStringManager.SP.GetString(nameId);
		}
		
		private void onClickButtonPriceSort(GameObject go)
		{
			PriceDes = !PriceDes;
			uint nameId = 1072u;
			if ( PriceDes )
				nameId = 1073u;
			if ( PriceDes )
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.PRICE_DEC);
			else
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.PRICE_INC);
			PriceLabelSort.text = XStringManager.SP.GetString(nameId);
		}
		
		private void onClickButtonNameSort(GameObject go)
		{
			NameDes = !NameDes;
			uint nameId = 1068u;
			if ( NameDes )
				nameId = 1069u;
			if ( NameDes )
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.NAME_DEC);
			else
				XLogicWorld.SP.AuctionManager.ChangeSortType(EAuctionSortType.NAME_INC);
			NameLabelSort.text = XStringManager.SP.GetString(nameId);
		}
		
		private void onClickBuyNumIncrease(GameObject go)
		{
			if ( null == _buyInfo )
				return;
			XCfgItem item = XCfgItemMgr.SP.GetConfig(_buyInfo.ItemColorID.ItemDataID);
			if ( _buyCount >= _buyInfo.Number || _buyCount >= item.PileAmount )
				return;
			else
				_buyCount++;
			int totalCost = (int)(_buyCount * _buyInfo.Price);
			Label_MoneyCost.text = totalCost.ToString();
			Label_BuyCount.text = _buyCount.ToString();
		}
		
		private void onClickBuyNumDecrease(GameObject go)
		{
			if ( null == _buyInfo )
				return;
			if ( _buyCount <= 1 )
				return;
			_buyCount--;
			int totalCost = (int)(_buyCount * _buyInfo.Price);
			Label_MoneyCost.text = totalCost.ToString();
			Label_BuyCount.text = _buyCount.ToString();
		}
		
		private void onClickButMax(GameObject go)
		{
			if ( null == _buyInfo )
				return;
			XCfgItem item = XCfgItemMgr.SP.GetConfig(_buyInfo.ItemColorID.ItemDataID);
			_buyCount = (int)_buyInfo.Number;
			if ( _buyInfo.Number > item.PileAmount )
				_buyCount = (int)item.PileAmount;
			int totalCost = (int)(_buyCount * _buyInfo.Price);
			Label_MoneyCost.text = totalCost.ToString();
			Label_BuyCount.text = _buyCount.ToString();
		}
		
		private void onClickBuyAuction(GameObject go)
		{
			if ( null == _buyInfo )
				return;
			if ( _buyCount == 0 )
				XLogicWorld.SP.AuctionManager.ON_SC_AuctionResult((int)EAuctionResult.BUYAUCTION_ITEM_SOLDALL);
			else
				XLogicWorld.SP.AuctionManager.BuyAuction(_buyInfo.ItemColorID, (uint)_buyCount);
		}
		
		public void RetSetSoldPanel()
		{
			_buyInfo = null;
			Label_MoneyCost.text = "0";
			Label_BuyCount.text = "0";
		}
		
		private void userSelect(int index)
		{
			XLogicWorld.SP.AuctionManager.onUserSelect(index);
		}
		
		public void UpdateItem2BuyInfo(XAuctionInfo info)
		{
			if ( info.Number <= 0 )
				_buyCount = 0;
			else
				_buyCount = 1;
			_buyInfo = info;
			Label_BuyCount.text = _buyCount.ToString();
			int totalCost = (int)(_buyCount * _buyInfo.Price);
			Label_MoneyCost.text = totalCost.ToString();
		}
		
		public void UpdateUserRealMoney(long money)
		{
			Label_PlayerMoney.text = money.ToString();
		}
		
		public void SetNoDataLabelAppeared(bool see)
		{
			GameObject_NoData.SetActive(see);
			GameObject_Fanye.SetActive(!see);
		}
		
		public void SetClickTipAppeared(bool see)
		{
			GameObject_FindClickTip.SetActive(see);
			GameObject_Fanye.SetActive(!see);
		}
		
		public UITree tree;
		public UIScrollBar bar;
		
		// 查询条件输入控件
		public UIInput Label_Search_Name;
		public UIInput Label_Search_MinLevel;
		public UIInput Label_Search_MaxLevel;
		public UIPopupList PopList_Search;
		public UIImageButton Butten_Search;
		private SortedList<string, int> m_allColorLevels = new SortedList<string, int>();
		
		// 查询为空需要显示的控件
		public GameObject GameObject_NoData;
		public GameObject GameObject_FindClickTip;
		public GameObject GameObject_Fanye;
		
		// 拍卖物显示控件
		public UILabel Label_Curr_Total_Count;
		public UIImageButton Button_Show_Next;
		public UIImageButton Button_Show_privous;
		
		// 拍卖排序控件
		public UIImageButton Button_Level_Sort;
		public UIImageButton Button_Price_Sort;
		public UIImageButton Button_Name_Sort;
		
		public GameObject[] AuctionShowObjs;
		private XBaseActionIcon[] AuctionInfoObjs = new XBaseActionIcon[5];
		
		// 购买信息界面
		public UIImageButton Button_Buy;
		public UIImageButton Buttom_Max;
		public UIImageButton Button_BuyIncrease;
		public UIImageButton Button_BuyDecrerase;
		public UILabel Label_BuyCount;
		public UILabel Label_MoneyCost;
		public UILabel Label_PlayerMoney;
		
		// 物品选择控件
		public UIRadioButton Radiao_UserSelect;
		private int _buyCount;
		private XAuctionInfo _buyInfo;
		
		public UILabel PriceLabelSort;
		private bool PriceDes = false;
		public UILabel LevelLabelSort;
		private bool LevelDes = false;
		public UILabel NameLabelSort;
		private bool NameDes = false;
	}
	
	public UISpriteGroup GroupPage_Select;							// 切换选择页面
	public GameObject ChuShow_Object;								// 玩家出售信息界面
	public GameObject GouMai_Object;								// 拍卖信息界面
	
	public SoldHistroyInfoMgr soldInfoMgr = new SoldHistroyInfoMgr();							// 历史出售信息管理器
	public GameObject ChuShowInfo_Object;														// 历史出售信息界面
	public SoldPanelInfoMgr SoldItemMgr = new SoldPanelInfoMgr();								// 出售信息管理器
	public GameObject ChuShowItem_Object;														// 历史出售信息界面
	
	public MyAuctionInfoMgr myAuctionInfoMgr = new MyAuctionInfoMgr();							// 玩家信息管理器
	
	public SoldAuctionInfoMgr soldAuctionInfoMgr = new SoldAuctionInfoMgr();					// 拍卖物品管理器
	
	public static bool AuctionShow = false;
	
	public override bool Init ()
	{
		base.Init();
		
		GroupPage_Select.mModify += OnUserSelectPage;
		
		soldInfoMgr.Init(this);
		SoldItemMgr.Init(this);
		soldAuctionInfoMgr.Init();
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eAuction);
	}

    public override void Show()
    {
        base.Show();
		AuctionShow = true;
    }

    public override void Hide()
    {
        base.Hide();
		
		AuctionShow = false;
    }
	
	private void OnUserSelectPage(int index)
	{
		ChuShow_Object.SetActive(1==index);
		GouMai_Object.SetActive(0==index);
		
		if ( 1 == index )
			XLogicWorld.SP.AuctionManager.RequestMyAuction();
	}
	
	public void ShowHistroyInfoPanel()
	{
		ChuShowInfo_Object.SetActive(true);
		ChuShowItem_Object.SetActive(false);
	}
	
	public void ShowSoldPanel()
	{
		ChuShowInfo_Object.SetActive(false);
		ChuShowItem_Object.SetActive(true);
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eBagWindow);
	}
	
	public void ShowAllAuctionInfo(List<XAuctionInfo> allAuction, string count)
	{
		soldAuctionInfoMgr.Show(allAuction, count);
	}
	
	public void ShowMyAuctionInfo(SortedList<XMyAuction, XMyAuction> mySortedAuction)	
	{
		myAuctionInfoMgr.Show(mySortedAuction);
	}
	
	public void ShowHistroyInfo(AuctionHistyoyInfo info)	
	{
		soldInfoMgr.UpdateInfo(info);
		SoldItemMgr.UpdateCanPubCount(info.LeftPubCount);
	}
	
	public void UserRealMoneyChange(long money)
	{
		soldAuctionInfoMgr.UpdateUserRealMoney(money);
	}
	
	public void SetSoldDataNoDataVisible(bool visible)
	{
		soldAuctionInfoMgr.SetNoDataLabelAppeared(visible);
	}
	
	public void SetMyDataNoDataVisible(bool visible)
	{
		myAuctionInfoMgr.SetNoDataAppear(visible);
	}
	
	public void GragPublish(uint id, UInt64 price, UInt64 pubCount)	
	{
		SoldItemMgr.UpdateItemSoldInfo(id, price, pubCount);
		if ( GouMai_Object.activeSelf )
		{
			GroupPage_Select.Select(1);
			OnUserSelectPage(1);
			XLogicWorld.SP.AuctionManager.RequestMyAuction();
		}
		
		if ( ChuShowInfo_Object.activeSelf )
			ShowSoldPanel();
	}
	
	public void UserSelectAuction(XAuctionInfo info)
	{
		soldAuctionInfoMgr.UpdateItem2BuyInfo(info);
	}
	
	public void UpdateSoldPrice(uint price, SItemColorID idcolor)
	{
		SoldItemMgr.UpdatePrice(price, idcolor);
	}
	
	public void ReSet2SoldInfo()
	{
		SoldItemMgr.reset();
	}
}