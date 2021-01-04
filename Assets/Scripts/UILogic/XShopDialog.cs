using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XShopDialog")]
public class XShopDialog : XDefaultFrame {
	
	// ui对象----------------------------------------------------------------------------------------------------------
	// 10个商店物品控件对象
	public GameObject[] m_ShopList;
	// 购买 回购
	public UISprite[] m_BuyListBtn = null;
	// 下一页按钮对象
	public GameObject m_NextPageBtn	= null;
	// 上一页按钮对象
	public GameObject m_LastPageBtn = null;
	// 显示当前页lab对象
	public UILabel m_PageShowLabel = null;
	// 整个ui背景图
	public GameObject m_BackGround = null;
	// 弹出购买框
	public GameObject m_ShopLit = null;
	
	// 数据对象---------------------------------------------------------------------------------------------------------
	private bool m_bBuyListState = true;
	
	// 当前商店对应npcId
	private uint m_NpcID;
	
	// 单页最大物品数量
	static public uint m_pageItemTotalCount = 10;
	
	// 当前页
	private uint m_uiCurrentPage = 1;
	
	// 单npc商店所有物品list
	private SortedList<uint,XCfgShopItem> m_npcBuyGroup = null;
	// 限卖物品<物品id，物品某时最大数量>list
	private SortedList<uint,uint> m_limiteList = new SortedList<uint, uint>();
	// 当前可买的物品list,多页显示
	private ArrayList m_CurrentBuyItemList = new ArrayList();
	
	
	// 事件逻辑数据 以后要去掉，走统一接口(放到shopitem中)
	private itemCallBack[] m_itemCallBackList;
	
	public class itemCallBack
	{
		public virtual void clickCallBack(GameObject _go){}
		
		public static void dorpCallBack(GameObject _go,GameObject draggedObject)
		{
			if(_go == draggedObject ) //source is self is invalid 
			{
				return;
			}
			
			if( ActionIcon_Type.ActionIcon_Shop == XDragMgr.SP.IconType ||
				ActionIcon_Type.ActionIcon_ShopBuyBack == XDragMgr.SP.IconType)
			{
				return;
			}
			
			UICamera.currentTouch.DropIsDeal = true;
			XDragMgr.SP.IsDraging = false;
			XEventManager.SP.SendEvent(EEvent.Cursor_ClearIcon);
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eCursor);
			
			if(ActionIcon_Type.ActionIcon_Bag == XDragMgr.SP.IconType )
			{
				XShopItemMgr.SP.requestSellItem( XDragMgr.SP.IconData );
			}
		}
		
		public virtual void dragCallBack(GameObject go, Vector2 delta){}
	}
	
	public class shopItemBuyCallBack:itemCallBack
	{
		public uint m_npcID = 0;
		public uint m_itemID = 0;
		
		public override void clickCallBack(GameObject _go)
		{
			if(-2==UICamera.currentTouchID)//right button
			{
				XShopItemMgr.SP.requestBuyItem(m_npcID,m_itemID);
			}
		}
		
		public override void dragCallBack(GameObject go, Vector2 delta)
		{
			
		}
		
	}
	
	public class shopItemBuyBackCallBack:itemCallBack
	{
		public uint m_BuyBacklistIndex = 0;
		
		public override void clickCallBack(GameObject _go)
		{
			if(-2==UICamera.currentTouchID)//right
			{
				XShopItemMgr.SP.requestBuyBackItem(m_BuyBacklistIndex);
			}
		}
		
		public override void dragCallBack(GameObject go, Vector2 delta)
		{
			
		}
	}
	 // 比较参数
	 private class XCfgShopItemCompare : System.Collections.IComparer
	 {
            public int Compare(object x, object y)
            {
                return (int)(((XCfgShopItem)x).Sequencs - ((XCfgShopItem)y).Sequencs);
            }
      }
	
	public override bool Init()
	{
		bool b = base.Init();
		// 隐藏npc界面
		XEventManager.SP.AddHandler(eventHideSelf,EEvent.NpcDialog_FarDistance);
		// 更新购买list
		XEventManager.SP.AddHandler(updateBuyBackList,EEvent.ShopDialog_UpdateBuyBack);
		// 更新限购list
		XEventManager.SP.AddHandler(updateBuyLimitedList,EEvent.ShopDialog_NpcLimiteList);
				
		// 购买
		UIEventListener buyListener = UIEventListener.Get( m_BuyListBtn[0].gameObject );
		buyListener.onClick += buyListShowCallBack;
		// 回购
		UIEventListener buyBackListener = UIEventListener.Get( m_BuyListBtn[1].gameObject );
		buyBackListener.onClick += buyBackListShowCallBack;
		// 放掉鼠标（ui背景图时间，相当于整个ui框）
		UIEventListener backGroundListener = UIEventListener.Get( m_BackGround );
		backGroundListener.onDrop += itemCallBack.dorpCallBack;
		// 下一页
		UIEventListener.Get(m_LastPageBtn).onClick = pageBtnUpCallBack;
		// 上一页
		UIEventListener.Get(m_NextPageBtn).onClick = pageBtnDownCallBack;
		
		m_uiCurrentPage=0;
		// 事件方法
		m_itemCallBackList = new itemCallBack[m_ShopList.Length];
		
		makeListByCurrentState();
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return b;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eShopDialog);
	}
	
	public override void Show()
	{
		base.Show();
		
		initShop();
	}
	
	private void initShop()
	{
		m_NpcID = XShopItemMgr.SP.npcID;
		
		if(0 == m_NpcID)
			return;
		
		m_uiCurrentPage = 1;
		
		XCfgNpcBase npcBase = XCfgNpcBaseMgr.SP.GetConfig(m_NpcID);
		
		m_npcBuyGroup = XCfgShopItemMgr.SP.GetGroup(m_NpcID);
		
		XShopItemMgr.SP.requestLimiteBuyItemList(m_NpcID);
		
		makeCurrentBuyItem();
		
		makeListByCurrentState();
	}
	// 隐藏商店UI
	private void eventHideSelf(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eShopDialog);
	}
	
	// 清理所有事件（m_ShopList<XShopItem>）
	private void clearCallBackList()
	{
		for(int i=0;i<m_ShopList.Length;i++ )
		{
			if(null==m_itemCallBackList[i]) //not need delete call back
			{
				continue;
			}
			//two UI
			
			UIEventListener Listener = UIEventListener.Get(m_ShopList[i].GetComponent<XShopItem>().m_itemBackGround);
			Listener.onClick -=  m_itemCallBackList[i].clickCallBack;
			//Listener.onDrop -=  m_itemCallBackList[i].dorpCallBack;
			//Listener.onDrag -=  m_itemCallBackList[i].dragCallBack;
			
			Listener = UIEventListener.Get(m_ShopList[i].GetComponent<XShopItem>().m_itemActionIcon.gameObject);
			//Listener.onClick -= m_itemCallBackList[i].clickCallBack;
			//Listener.onDrop -= m_itemCallBackList[i].dorpCallBack;
			Listener.onDrag -= m_itemCallBackList[i].dragCallBack;
			
		}
		
		for(int i=0;i<m_itemCallBackList.Length;i++ )
		{
			m_itemCallBackList[i] = null;
		}
		
	}
	
	// 将单页所有物品ui，赋值所有数据
	private void makeBuyList()
	{
	  	hideShopItemList();
	  	clearCallBackList();
		
		if(null == m_npcBuyGroup)
			return;
		
		//get current page list to show 
		//SortedList<uint,XCfgShopItem> currentPageList = getItemListPage(m_uiCurrentPage,m_CurrentBuyItemList);
		//if(null == currentPageList)
		//	return;
		
		if(m_CurrentBuyItemList.Count <= 0) return;
		// 当前页起始索引
		int uStartIndex = (int)((m_uiCurrentPage-1)*m_pageItemTotalCount);
		
		if(uStartIndex < 0 || uStartIndex >= m_CurrentBuyItemList.Count)  return;
		
		// ui索引
		int uiCurrentItemIndex = 0;
		// 数据索引
		int uCurrentIndex  = uStartIndex;
		int uEndIndex 	   = (int)(uStartIndex + m_pageItemTotalCount);
		if(uEndIndex >= m_CurrentBuyItemList.Count)
			uEndIndex = m_CurrentBuyItemList.Count;
		
	  	for(; uCurrentIndex < uEndIndex; uCurrentIndex++)
	  	{	
			//if(uiCurrentItemIndex >= m_ShopList.length) return;
			
			XCfgShopItem cfgShopItem = m_CurrentBuyItemList[uCurrentIndex] as  XCfgShopItem;
			
			m_ShopList[uiCurrentItemIndex].SetActive(true);
			if(!(m_ShopList[uiCurrentItemIndex].GetComponent<XShopItem>().setBuyItemLogic(m_NpcID, cfgShopItem.itemID)))
				continue;
			
			//call back
			shopItemBuyCallBack itemCall = new shopItemBuyCallBack();
			
			itemCall.m_itemID = cfgShopItem.itemID;
			itemCall.m_npcID = m_NpcID;
			
			//two UI
			UIEventListener Listener = UIEventListener.Get(m_ShopList[uiCurrentItemIndex].GetComponent<XShopItem>().m_itemBackGround);
			Listener.onClick +=  itemCall.clickCallBack;
			//istener.onDrop  +=  itemCall.dorpCallBack;
			//Listener.onDrag  +=  itemCall.dragCallBack;
			
			XShopItem shopItem = m_ShopList[uiCurrentItemIndex].GetComponent<XShopItem>();
			Listener = UIEventListener.Get(shopItem.m_itemActionIcon.gameObject);
			
			
			Listener.onClick += itemCall.clickCallBack;
			//Listener.onDrop  +=  itemCall.dorpCallBack;
			Listener.onDrag  +=  itemCall.dragCallBack;
			
			m_itemCallBackList[uiCurrentItemIndex] = itemCall;
			
			//call back end
			uiCurrentItemIndex++;
		}
	  	
	}
	// 显示回购list ui，ui赋值相应数据
	private void makeBuyBackList()
	{
		hideShopItemList();
		clearCallBackList();
		
		for(int i=0;i< XShopItemMgr.ShopBuyBackNum;i++ )
		{
			XItem item = XShopItemMgr.SP.getBuyBackItem(i);
			
			if(null==item)//not has item
			{
				continue;
			}
			
			m_ShopList[i].GetComponent<XShopItem>().setBuyBackItemLogic( i, item.ItemCount);
			m_ShopList[i].SetActive(true);
			
			//call back
			shopItemBuyBackCallBack itemCall = new shopItemBuyBackCallBack();
			itemCall.m_BuyBacklistIndex = (uint)i;
			
			//two UI
			UIEventListener Listener = UIEventListener.Get(m_ShopList[i].GetComponent<XShopItem>().m_itemBackGround);
			Listener.onClick += itemCall.clickCallBack;
			
			Listener = UIEventListener.Get(m_ShopList[i].GetComponent<XShopItem>().m_itemActionIcon.gameObject);
			Listener.onClick += itemCall.clickCallBack;
						
			m_itemCallBackList[i] = itemCall;
			
			//call back end
			
			
		}
	}
	// 得到当前页所有物品list（已经没有用了）
	private SortedList<uint,XCfgShopItem> getItemListPage(uint iPage,SortedList<uint,XCfgShopItem> BaseItemList)
	{
		uint iStartIndex = (iPage-1)*m_pageItemTotalCount;
		
		if( 0 > iStartIndex || BaseItemList.Count < iStartIndex )
		{
			return null;
		}
		
		SortedList<uint,XCfgShopItem> pageSubItemList = new SortedList<uint, XCfgShopItem>();
		
		uint iCurrentIndex = iStartIndex;
		foreach(KeyValuePair<uint,XCfgShopItem> kvpItem in BaseItemList)
		{
			if(iCurrentIndex >= BaseItemList.Count ) //over exit
			{
				break;
			}
			
			pageSubItemList.Add(kvpItem.Key,kvpItem.Value );
		}
		
		return pageSubItemList;
	}
	
	private void makeListByCurrentState()
	{
		makeListByCurrentState(1);
	}
	// 显示商品或回购商品
	private void makeListByCurrentState(uint uiPage)
	{
		//m_uiCurrentPage = uiPage;
		
		//start from 1 page
		if(m_bBuyListState)
		{
			//request limite item list
			m_limiteList.Clear();
			makeBuyList();
		}else
		{
			m_uiCurrentPage = 1;
			makeBuyBackList();
		}
		// 更新页显示
		updatePageLabel( m_uiCurrentPage,getPageTotalCount() );
	}
	// 商品按钮 
	private void buyListShowCallBack(GameObject _go)
	{
		m_bBuyListState = true;
		makeListByCurrentState();
	}
	// 回购按钮
	private void buyBackListShowCallBack(GameObject _go)
	{
		m_bBuyListState = false;
		makeListByCurrentState();
	}
	// 隐藏所有shopitem
	private void hideShopItemList()
	{
		foreach(GameObject objItem in m_ShopList)
		{
			objItem.SetActive(false);
		}
	}
	// 服务器下发回购
	private void updateBuyBackList(EEvent evt, params object[] args)
	{
		if(false==m_bBuyListState)// is buyBack state
		{
			makeBuyBackList();
		}
	}
	
	// 服务器下发，更新限购list
	public void updateBuyLimitedList(EEvent evt, params object[] args)
	{
		SC_NPCLimitShopItemList msg = args[0] as SC_NPCLimitShopItemList;
		
		//current buy plane is show and need npcID is right
		if( m_NpcID != (uint)msg.NpcID  )
		{
			return;
		}
		
		m_limiteList.Clear();
		
		//first clear
		for(int i=0;i<msg.LimitShopItemListCount;i++ )
		{
			LimitShopItem itemLimited = msg.GetLimitShopItemList(i);
			
			m_limiteList.Add(itemLimited.ItemID,itemLimited.CurrentCount);
		}
		
		makeCurrentBuyItem(); //update current by new limited list
		
		makeListByCurrentState();  //update show buy item list
	}
	// 上一页
	private void pageBtnUpCallBack(GameObject _go)
	{
		m_uiCurrentPage = pageRange(m_uiCurrentPage-1);
		
		updatePageLabel( m_uiCurrentPage,getPageTotalCount() );
		
		makeListByCurrentState();
	}
	// 下一页
	private void pageBtnDownCallBack(GameObject _go)
	{
		m_uiCurrentPage = pageRange(m_uiCurrentPage + 1);
		
		updatePageLabel( m_uiCurrentPage,getPageTotalCount() );
		
		makeListByCurrentState();
	}
	
	// 获取当前npc商店可以卖的所有物品，主要核查限购物品数量
	private void makeCurrentBuyItem()
	{
		m_CurrentBuyItemList.Clear();
		XCfgItem itemBase = null;
		if(m_npcBuyGroup == null)
			return ;
		
		foreach(KeyValuePair<uint,XCfgShopItem> kvpItem in m_npcBuyGroup )
		{
			//limit shop item special
			if(kvpItem.Value.maxNum > 0)
			{
				uint count;
				if(!m_limiteList.TryGetValue( kvpItem.Key,out count) ) //not have this limited item
				{
					continue;
				}else if( 0==count ) //the limited item not enough 
				{
					continue;
				}
			}
			itemBase = XCfgItemMgr.SP.GetConfig(kvpItem.Key);
			if(itemBase == null) continue;
			m_CurrentBuyItemList.Add( kvpItem.Value );
		}
		XCfgShopItemCompare cfgShopItemCompare = new XCfgShopItemCompare();
		m_CurrentBuyItemList.Sort(cfgShopItemCompare);
	}
	
	private void updatePageLabel( uint uiCurrentPage,uint uiTotalPage )
	{
		m_PageShowLabel.text= uiCurrentPage.ToString() + "/" +uiTotalPage.ToString();
	}
	// 当前物品需要多少页来显示
	private uint getPageTotalCount()
	{
		if(m_bBuyListState)
		{
			float fPage = (float)(m_CurrentBuyItemList.Count)/(float)m_pageItemTotalCount;
			
			return (uint)Mathf.CeilToInt( fPage );
		}else
		{
			return 1;
		}
	}
	
	private uint pageRange( uint pageCount )
	{
		return System.Math.Max( 1 ,System.Math.Min( pageCount ,getPageTotalCount() )  );
	}
	
}
