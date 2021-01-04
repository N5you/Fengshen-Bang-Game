using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

class XAuctionManager
{
    public XAuctionManager()
    {
        CMPLST = new Dictionary<EAuctionSortType, IComparer<XAuctionInfo>>();
        CMPLST.Add(EAuctionSortType.LEVEL_INC, new CmpAucion_Level_Inc());
        CMPLST.Add(EAuctionSortType.LEVEL_DEC, new CmpAucion_Level_Dec());
        CMPLST.Add(EAuctionSortType.NUMBER_INC, new CmpAucion_Number_Inc());
        CMPLST.Add(EAuctionSortType.NUMBER_DEC, new CmpAucion_Number_Dec());
        CMPLST.Add(EAuctionSortType.NAME_INC, new CmpAucion_Name_Inc());
		CMPLST.Add(EAuctionSortType.NAME_DEC, new CmpAucion_Name_Dec());
        CMPLST.Add(EAuctionSortType.QUALITY_INC, new CmpAucion_Quality_Inc());
		CMPLST.Add(EAuctionSortType.PRICE_INC, new CmpAucion_Price_Inc());
		CMPLST.Add(EAuctionSortType.PRICE_DEC, new CmpAucion_Price_Dec());

        m_AllAuction = new Dictionary<uint, Dictionary<SItemColorID, XAuctionInfo>>();
        NowFilter = new CAuctionFilter(0);
        m_ShowAuction = new List<XAuctionInfo>();
		m_showSortedAuction = new SortedList<XAuctionInfo, XAuctionInfo>(CMPLST[EAuctionSortType.LEVEL_DEC]);
        m_MySortedAuction = new SortedList<XMyAuction, XMyAuction>();
        m_MyAuctionMap = new Dictionary<uint, XMyAuction>();
    }

    public void Start()
    {
		// 每次重新打开市集界面进行初始化(主要是重新从服务器获取信息)
        m_AllAuction.Clear();
		m_showSortedAuction.Clear();
        m_ShowAuction.Clear();
        NowFilter.SetAll(0);
    }

    public void Stop()
    {
        XLogicWorld.SP.NetManager.SendEmptyPacket((int)CS_Protocol.eCS_AbortAuction);
    }

    #region PacketLogic
	
	// 所有的拍卖数据
    public void On_SC_AllAuction(PB_AllAuction msg)
    {		
        foreach (PB_AuctionItem item in msg.AllAuctionList)
        {
            this.updateAuction(item, false);
        }
		ShowCount = msg.Count / 5;
		if ( msg.Count % 5 > 0 )
			ShowCount++;
		showCurrAuctionInfo(CurrPos);
		
		XEventManager.SP.SendEvent(EEvent.auction_SetSoldNoDataVisible, m_ShowAuction.Count);
    }
	
	public void On_SC_UpdateAuctionItem(PB_AuctionItem msg)
    {
        this.updateAuction(msg, true);
		showCurrAuctionInfo(CurrPos);
    }
	
	// 玩家的拍卖数据
    public void On_SC_AllMyAuction(PB_AllMyAuction msg)
    {
		// 清空旧的数据
		m_MySortedAuction.Clear();
        m_MyAuctionMap.Clear();
		
        foreach (PB_OneAuction item in msg.AuctionsList)
        {
            this.updateMyAuction(item);
        }
        this.onUpdateMyAuction();
		
		XEventManager.SP.SendEvent(EEvent.auction_SetMyNoDataVisible, m_MySortedAuction.Count);
    }

    public void On_SC_UpdateMyAuction(PB_OneAuction msg)
    {
        this.updateMyAuction(msg);
		this.onUpdateMyAuction();
		
		XEventManager.SP.SendEvent(EEvent.auction_SetMyNoDataVisible, m_MySortedAuction.Count);
    }
	
	public void ON_SC_GetAuctionPrice(SC_UInt64 priceData)
	{
		UInt64 price = priceData.Data;
		UInt64 id = priceData.Uid;
		uint dataid = (uint)( id >>32 );
		uint color = (uint)( id & 0x00000000ffffffff );
		SItemColorID colorId = new SItemColorID(dataid, color);
		m_AllAuctionPrice[colorId] = (uint)price;
		
		XEventManager.SP.SendEvent(EEvent.auction_Update_Price, (uint)price, colorId);
	}
	
	public void On_SC_AuctionHistroyInfo(PB_AuctionHistroyInfo msg)
    {
		m_HistroyInfo.SetData(msg.TotalCount, msg.Income, msg.Spending, (UInt64)m_MySortedAuction.Count, msg.TodayIncome, msg.CanPubCount);
		XEventManager.SP.SendEvent(EEvent.auction_UpdateHistroyInfo, m_HistroyInfo);
    }
	
	public void ON_SC_AuctionResult(int result)
	{
		uint showstrid;
		switch ( (EAuctionResult)result )
		{
		case EAuctionResult.PUBLISH_SUCCESS:
			showstrid = 226u;
			
			break;
		case EAuctionResult.PUBLISH_NOT_IN_BAG:
			showstrid = 227u;
			
			break;
		case EAuctionResult.PUBLISH_NOT_ENOUGH_COUNT:
			showstrid = 228u;
			
			break;
		case EAuctionResult.PUBLISH_ITEM_CANNOT_PUB:
			showstrid = 229u;
			
			break;
		case EAuctionResult.PUBLISH_NOT_ENOUGH_PLAYER:
			showstrid = 230u;
			
			break;
		case EAuctionResult.PUBLISH_NOT_ENOUGH_PUBCOUNT:
			showstrid = 231u;
			
			break;
		case EAuctionResult.PUBLISH_PUB_MAX_ITEMS:
			showstrid = 232u;
			
			break;
		case EAuctionResult.PUBLISH_NOT_ENOUGH_SAVEINFO:
			showstrid = 233u;
			
			break;
		case EAuctionResult.PUBLISH_ITEM_NULL:
			showstrid = 234u;
			
			break;
		case EAuctionResult.PUBLISH_ITEM_DELFAILED:
			showstrid = 235u;
			
			break;
		case EAuctionResult.BUY_SUCCESS:
			showstrid = 236u;
			
			break;
		case EAuctionResult.BUYAUCTION_NUMBER_ZERO:
			showstrid = 237u;
			
			break;
		case EAuctionResult.BUYAUCTION_ITEM_SOLDALL:
			showstrid = 238u;
			
			break;
		case EAuctionResult.BUYAUCTION_ITEM_NOTENOUGH:
			showstrid = 239u;
			
			break;
		case EAuctionResult.BUYAUCTION_BAG_NOTENOUGH:
			showstrid = 240u;
			
			break;
		case EAuctionResult.BUYAUCTION_REALMONEY_NOTENOUGH:
			showstrid = 241u;
			
			break;
		case EAuctionResult.CANCEL_SUCCESS:
			showstrid = 242u;
			
			break;
		case EAuctionResult.CANCELAUCTION_NOT_OWNTOYOU:
			showstrid = 243u;
			
			break;
		case EAuctionResult.CANCELAUCTION_NOT_FIND:
			showstrid = 244u;
			
			break;
		case EAuctionResult.CANCELAUCTION_NOTENOUGH_BAG:
			showstrid = 245u;
			
			break;
		default:
			return;
		}
		
		XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(showstrid));
		//XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemTop, showstrid);
	}

    public void RequestAllAuction(string name, int minLvl, int maxLvl, int colorLvl)
    {
		m_AllAuction.Clear();
		m_ShowAuction.Clear();
		m_showSortedAuction.Clear();
		CurrPos = 1;
		
		CS_FindAuction.Builder msg = CS_FindAuction.CreateBuilder();
		msg.SetName(name);
		if ( -1 != minLvl )
			msg.SetMinLevel((uint)minLvl);
		if ( -1 != maxLvl )
			msg.SetMaxLevel((uint)maxLvl);
		msg.SetColor((uint)colorLvl);
		msg.SetItemType((uint)m_itemType);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_AllAuction, msg.Build());
		
		m_bsearched = true;
    }

    public void RequestMyAuction()
    {
		//--4>TODO: check
        XLogicWorld.SP.NetManager.SendEmptyPacket((int)CS_Protocol.eCS_MyAuction);
    }

    public void AddAuction(uint realInidex, uint number)
    {
		//--4>TODO: check
		Msg_PairUU.Builder builder = Msg_PairUU.CreateBuilder();
		builder.SetFirst(realInidex);
        builder.SetSecond(number);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_AddAuction, builder.Build());
    }

    public void BuyAuction(SItemColorID idColor, uint number)
    {
		if ( !m_AllAuctionPrice.ContainsKey(idColor) )
			return;
		
		m_auction2buy = idColor;
		m_auction2count = number;
		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(MessageBox_BuyAuction);
		XCfgItem item = XCfgItemMgr.SP.GetConfig(idColor.ItemDataID);
		uint money = m_AllAuctionPrice[idColor] * number;
		string str2show = string.Format(XStringManager.SP.GetString (246u), item.Name, number, money);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, str2show);
    }
	
	private void MessageBox_BuyAuction(GameObject go)
	{
		CS_BuyAuction.Builder builder = CS_BuyAuction.CreateBuilder();
        builder.SetItemDataID(m_auction2buy.ItemDataID);
        builder.SetQuality(m_auction2buy.Quality);
        builder.SetNumber(m_auction2count);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_BuyAuction, builder.Build());
	}

	public void DelAuction(uint auctionID)
    {
		Msg_UInt.Builder builder = Msg_UInt.CreateBuilder();
        builder.SetData(auctionID);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_DelAuction, builder.Build());
		
		// 后续再决定是否需要加确认按钮
//		m_auction2del = auctionID;
//		UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(MessageBox_DelAuction);
//		XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString (104)));
    }
	
	private void MessageBox_DelAuction(GameObject go)
	{
		if ( 0u == m_auction2del )
			return;
		
		Msg_UInt.Builder builder = Msg_UInt.CreateBuilder();
        builder.SetData(m_auction2del);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_DelAuction, builder.Build());
	}

    #endregion PacketLogic

    #region UI About

    public void ChangeFilter(int id)
    {
		m_itemType = id;
		return;
		
        NowFilter.SetAll(id);
		m_ShowAuction.Clear();
		m_showSortedAuction.Clear();
		foreach ( KeyValuePair<uint, Dictionary<SItemColorID, XAuctionInfo>> data in m_AllAuction )
		{
			foreach(KeyValuePair<SItemColorID, XAuctionInfo> tdata in data.Value )
			{
				if ( NowFilter.IsFit(tdata.Value) )
				{
					m_ShowAuction.Add(tdata.Value);
					m_showSortedAuction[tdata.Value] = tdata.Value;
				}
			}
		}
		ShowCount = m_ShowAuction.Count / 5;
		if ( m_ShowAuction.Count % 5 > 0 )
			ShowCount++;
		CurrPos = 1;
		showCurrAuctionInfo(1);
		
		if ( m_bsearched )
			XEventManager.SP.SendEvent(EEvent.auction_SetSoldNoDataVisible, m_ShowAuction.Count);
    }

    public void ChangeSortType(EAuctionSortType sortType)
    {
		m_showSortedAuction = new SortedList<XAuctionInfo,XAuctionInfo>(m_showSortedAuction, CMPLST[sortType]);
		m_ShowAuction.Clear();
		foreach( KeyValuePair<XAuctionInfo, XAuctionInfo> it in m_showSortedAuction )
		{
			m_ShowAuction.Add(it.Value);
		}
		CurrPos = 1;
		showCurrAuctionInfo(1);
    }

    #endregion UI About

    #region Logic

    private void updateAuction(PB_AuctionItem item, bool bUpdate)
    {
        XAuctionInfo newInfo = XAuctionInfo.Get(item);
		if (null == newInfo)
        {
            return;
        }
        ushort mainType = newInfo.Config.ItemType;
		ushort subType = newInfo.Config.ItemSubType;
		uint type = (uint)((mainType << 16) | subType);
        if (!m_AllAuction.ContainsKey(type))
        {
			m_AllAuction.Add(type, new Dictionary<SItemColorID, XAuctionInfo>());
        }
		Dictionary<SItemColorID, XAuctionInfo> dicGroup = m_AllAuction[type];
        XAuctionInfo oldInfo = null;

		// 先删除再增加
        if (dicGroup.ContainsKey(newInfo.ItemColorID))
        {
            oldInfo = dicGroup[newInfo.ItemColorID];
			dicGroup.Remove(oldInfo.ItemColorID);
			//m_ShowAuction.Remove(oldInfo);
        }
        if ( newInfo.Number > 0 && !bUpdate )
        {
			dicGroup.Add(newInfo.ItemColorID, newInfo);
			m_ShowAuction.Add(newInfo);
			m_showSortedAuction.Add(newInfo, newInfo);
			m_AllAuctionPrice[newInfo.ItemColorID] = newInfo.Price;
        }
		if ( bUpdate )
		{
			dicGroup.Add(newInfo.ItemColorID, newInfo);
			for ( int i = 0; i < m_ShowAuction.Count; i++ )
			{
				if ( m_ShowAuction[i].Equals(newInfo ))
				{
					m_ShowAuction[i] = newInfo;
					break;
				}
			}
			m_showSortedAuction[newInfo] = newInfo;
			m_AllAuctionPrice[newInfo.ItemColorID] = newInfo.Price;
		}
    }

    private void updateMyAuction(PB_OneAuction msg)
    {
        XMyAuction myNew = XMyAuction.Get(msg);
        if (null == myNew)
        {
            return;
        }
		// 先删除再增加
        if (m_MyAuctionMap.ContainsKey(myNew.ID))
        {
            XMyAuction myOld = m_MyAuctionMap[myNew.ID];
            m_MyAuctionMap.Remove(myNew.ID);
            m_MySortedAuction.Remove(myOld);
        }
        if (myNew.ItemColorID.IsValid() && myNew.Number > 0)
        {
            m_MyAuctionMap.Add(myNew.ID, myNew);
            m_MySortedAuction.Add(myNew, myNew);
			m_AllAuctionPrice[myNew.ItemColorID] = myNew.Price;
        }
    }
	
	public void showCurrAuctionInfo(int index)
	{
		List<XAuctionInfo> auction2Show = new List<XAuctionInfo>();
		int startIndex = (index - 1) * 5;
		for (int i = 0; i < 5 && startIndex < m_ShowAuction.Count; startIndex++, i++ )
		{
			auction2Show.Add(m_ShowAuction[startIndex]);
		}
		XEventManager.SP.SendEvent(EEvent.auction_UpdateAuction, auction2Show, CurrPos.ToString() + "/" + ShowCount.ToString());
	}
	
	public void ShowAuctionInfoPosChange(int index, bool increase)
	{
		if ( increase && CurrPos>= ShowCount )
			return;
		if ( !increase && CurrPos <=1 )
			return;
		CurrPos = CurrPos + index;
		if ( (CurrPos - 1) * 5 >= m_ShowAuction.Count )
		{
			CS_AuctionNextPage.Builder builder = CS_AuctionNextPage.CreateBuilder();
			builder.SetIndex((uint)CurrPos);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ALLAuctionNextPage, builder.Build());
		}
		else
		{
			showCurrAuctionInfo(CurrPos);
		}
	}

    private void onUpdateMyAuction()
    {
        XEventManager.SP.SendEvent(EEvent.auction_UpdateMyAuction, m_MySortedAuction, m_HistroyInfo);
    }
	
	public void onUserSelect(int index)
	{
		int infoIndex = (CurrPos-1)*5 + index;
		if ( infoIndex < 0 || infoIndex >= m_ShowAuction.Count )
			return;
		XEventManager.SP.SendEvent(EEvent.auction_UserSelectAuction, m_ShowAuction[infoIndex]);
	}
	
	public uint GetItemPrice(uint id)
	{
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((int)ActionIcon_Type.ActionIcon_Bag, (short)id);
		if ( null == item )
			return 0u;
		SItemColorID colorId = new SItemColorID(item.DataID, (uint)item.Color);
		if ( m_AllAuctionPrice.ContainsKey(colorId) )
		{
			return m_AllAuctionPrice[colorId];
		}
		else
		{
			CS_UInt64.Builder builder = CS_UInt64.CreateBuilder();
			UInt64 dataid = (UInt64)item.DataID;
			dataid = (dataid << 32) | (uint)item.Color;
			builder.SetData(dataid);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_AuctionGetPrice, builder.Build());
			return 0u;
		}
		return 0u;  
	}

    #endregion Logic

    #region Members

	// 当前过滤条件
    public CAuctionFilter NowFilter { get; private set; }
    // 所有拍卖信息, 按照品物主类别进行存放
    private Dictionary<uint, Dictionary<SItemColorID, XAuctionInfo>> m_AllAuction;
	// 条件过滤后用于显示的拍卖信息列表
    private List<XAuctionInfo> m_ShowAuction;
	private SortedList<XAuctionInfo, XAuctionInfo> m_showSortedAuction;
	private int ShowCount = 0;
	private int CurrPos = 0;
	// 所有拍卖信息的排序方法
    public Dictionary<EAuctionSortType, IComparer<XAuctionInfo>> CMPLST { get; private set; }

	// 所有我的拍卖信息
    private SortedList<XMyAuction, XMyAuction> m_MySortedAuction;
    private Dictionary<uint, XMyAuction> m_MyAuctionMap;
	
	// 历史销售信息
	public AuctionHistyoyInfo m_HistroyInfo = new AuctionHistyoyInfo();
	
	private Dictionary<SItemColorID, uint> m_AllAuctionPrice = new Dictionary<SItemColorID, uint>();
	
	private uint m_auction2del = 0u;
	private SItemColorID m_auction2buy = new SItemColorID();
	private uint m_auction2count = 0u;
	
	bool m_bsearched = false;
	
	int m_itemType = 0;

    #endregion Members
}
