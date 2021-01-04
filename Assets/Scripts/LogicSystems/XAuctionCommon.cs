using System;
using UnityEngine;
using XGame.Client.Packets;
using System.Collections;
using System.Collections.Generic;

enum EAuctionSortType
{
	LEVEL_INC = 0,
	LEVEL_DEC,
	NUMBER_INC,
	NUMBER_DEC,
	PRICE_INC,
	PRICE_DEC,
	NAME_INC,
	NAME_DEC,
	QUALITY_INC,
}

enum EAuctionResult
{
	PUBLISH_SUCCESS					= 0,		// 发布成功
	PUBLISH_NOT_IN_BAG 				= 1,		// 不能发布非包裹中的物品
	PUBLISH_NOT_ENOUGH_COUNT 		= 2,		// 数量不足
	PUBLISH_ITEM_CANNOT_PUB 		= 3,		// 物品不能够被拍卖
	PUBLISH_NOT_ENOUGH_PLAYER 		= 4,		// 玩家数量达到最大了，不能再发布了
	PUBLISH_NOT_ENOUGH_PUBCOUNT 	= 5,		// 拍卖数量为0了，不能再拍卖了
	PUBLISH_PUB_MAX_ITEMS			= 6,		// 拍卖物品达到最大数量了（60）
	PUBLISH_NOT_ENOUGH_SAVEINFO 	= 7,		// 物品达到最大数量了（1024）
	PUBLISH_ITEM_NULL				= 8,		// 拍卖物品为空
	PUBLISH_ITEM_DELFAILED			= 9,		// 删除物品失败

	BUY_SUCCESS						= 20,		// 购买成功
	BUYAUCTION_NUMBER_ZERO			= 21,		// 购买数量不能为空
	BUYAUCTION_ITEM_SOLDALL			= 22,		// 物品已售完
	BUYAUCTION_ITEM_NOTENOUGH		= 23,		// 物品数量不够
	BUYAUCTION_BAG_NOTENOUGH		= 24,		// 背包空间不够
	BUYAUCTION_REALMONEY_NOTENOUGH	= 25,		// 背包空间不够

	CANCEL_SUCCESS					= 30,		// 下架成功
	CANCELAUCTION_NOT_OWNTOYOU		= 31,		// 此物品不属于你，不能下架
	CANCELAUCTION_NOT_FIND			= 32,		// 物品未找到
	CANCELAUCTION_NOTENOUGH_BAG 	= 33,		//	背包空间不够
};

public struct SItemColorID : IComparable<SItemColorID>
{
    public uint ItemDataID { get; private set; }
    public byte Quality { get; private set; }

    public bool IsValid() { return ItemDataID > 0 && Quality > 0; }

    public SItemColorID(uint id , uint quality )
    {
        ItemDataID = id;
        Quality = (byte)quality;
    }

    public void Clear()
    {
        ItemDataID = Quality = 0;
    }

    public int CompareTo(SItemColorID other)
    {
		if (ItemDataID == other.ItemDataID && Quality == other.Quality)
        {
			return 0;
        }
        if (ItemDataID < other.ItemDataID || (ItemDataID == other.ItemDataID && Quality < other.Quality))
        {
            return -1;
        }
		return 1;
    }

    public override bool Equals(System.Object obj)
    {
        if (null == obj || GetType() != obj.GetType())
        {
            return false;
        }
        SItemColorID other = (SItemColorID)obj;
        return ItemDataID == other.ItemDataID && Quality == other.Quality;
    }
}

class CAuctionFilter
{
    public ushort ItemType { get; set; }
    public ushort ItemSubType { get; set; }

	public CAuctionFilter(int id)
    {
        this.SetAll(id);
    }

    public void SetAll(int id)
    {
        ItemType = (ushort)(id >> 16);
        ItemSubType = (ushort)(id & 0x00FF);
    }

    public bool CheckType()
    {
		return ItemType > 0;
    }

    public bool IsFit(XAuctionInfo info)
    {
		if ( CheckType() && 
			(info.Config.ItemType == ItemType) && 
			( 0 == ItemSubType || info.Config.ItemSubType == ItemSubType)  
		   )
		{
            return true;
		}
        return false;
    }
}

// 一条拍卖物品信息
public class XAuctionInfo
{
    public SItemColorID ItemColorID { get; private set; }
	public uint	Number { get; private set; }
	public uint	Price { get; private set; }
    public ushort Level { get; private set; }

    internal XCfgItem Config { get; private set; }
	
	private XAuctionInfo() { }

	public static XAuctionInfo Get(PB_AuctionItem item)
    {
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.ItemDataID);
        if (cfgItem == null)
        {
            return null;
        }
        XAuctionInfo info = new XAuctionInfo();
        info.ItemColorID = new SItemColorID(item.ItemDataID, item.Quality);
		info.Number = item.Number;
        info.Level = cfgItem.ItemLevel;
        info.Config = cfgItem;
		info.Price = item.Price;
        return info;
    }
	
	public override bool Equals(System.Object obj)
    {
        if (null == obj || GetType() != obj.GetType())
        {
            return false;
        }
		XAuctionInfo other = (XAuctionInfo)obj;
        return ItemColorID.Equals(other.ItemColorID);
    }
}

public class CmpAucion_Level_Inc : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Level < y.Level || (x.Level == y.Level && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
		return 1;
    }
}

public class CmpAucion_Level_Dec: IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Level > y.Level || (x.Level == y.Level && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
		return 1;
    }
}

public class CmpAucion_Number_Inc : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Number < y.Number || (x.Number == y.Number && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
		return 1;
    }
}

public class CmpAucion_Number_Dec: IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Number > y.Number || (x.Number == y.Number && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
		return 1;
    }
}


public class CmpAucion_Price_Inc : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Price < y.Price || (x.Price == y.Price && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
		return 1;
    }
}

public class CmpAucion_Price_Dec: IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Price > y.Price || (x.Price == y.Price && x.ItemColorID.CompareTo(y.ItemColorID) > 0))
        {
            return -1;
        }
		return 1;
    }
}

public class CmpAucion_Name_Inc : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Config.Name.CompareTo(y.Config.Name) < 0 || (x.Config.Name.CompareTo(y.Config.Name) == 0 && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
        return 1;
    }
}

public class CmpAucion_Name_Dec : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.Config.Name.CompareTo(y.Config.Name) > 0 || (x.Config.Name.CompareTo(y.Config.Name) == 0 && x.ItemColorID.CompareTo(y.ItemColorID) > 0))
        {
            return -1;
        }
        return 1;
    }
}

public class CmpAucion_Quality_Inc : IComparer<XAuctionInfo>
{
    public int Compare(XAuctionInfo x, XAuctionInfo y)
    {
        if (x.ItemColorID.CompareTo(y.ItemColorID) == 0)
        {
            return 0;
        }
        if (x.ItemColorID.Quality < y.ItemColorID.Quality || (x.ItemColorID.Quality == y.ItemColorID.Quality && x.ItemColorID.CompareTo(y.ItemColorID) < 0))
        {
            return -1;
        }
        return 1;
    }
}

public class XMyAuction : IComparable<XMyAuction>
{
    public uint ID { get; private set; }
    public SItemColorID ItemColorID { get; private set; }
    public uint RemainTime { get; private set; }
    public uint Number { get; private set; }
    public uint Price { get; private set; }
	internal XCfgItem Config { get; private set; }

    private XMyAuction() { }

    public static XMyAuction Get(PB_OneAuction msg)
    {
        if (null == msg)
        {
            return null;
        }
        XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(msg.ItemDataID);
        XMyAuction myAuction = new XMyAuction();
        myAuction.ID = msg.AuctionID;
        myAuction.ItemColorID = new SItemColorID(msg.ItemDataID, msg.Quality);
        myAuction.RemainTime = msg.RemainTime;
        myAuction.Number = msg.Number;
        myAuction.Price = msg.Price;
		myAuction.Config = cfgItem;
		
        return myAuction;
    }

    public int CompareTo(XMyAuction other)
    {
        if (ID == other.ID)
        {
            return 0;
        }
        if (RemainTime < other.RemainTime || (RemainTime == other.RemainTime && ID < other.ID))
        {
            return -1;
        }
        return 1;
    }
}


public struct AuctionHistyoyInfo
{
	public UInt64 TotalCount {get;set;}
	public UInt64 TotalIncome {get;set;}
	public UInt64 TotalSpending {get;set;}
	public UInt64 OnSoldCount {get;set;}
	public UInt64 TodayIncome {get;set;}
	public UInt64 LeftPubCount {get;set;}
	
	public void SetData(UInt64 totalCount, UInt64 totalIncome, UInt64 totalSpending,
		UInt64 onSoldCount, UInt64 todayIncome, UInt64 leftPubCount)
	{
		TotalCount = totalCount;
		TotalIncome = totalIncome;
		TotalSpending = totalSpending;
		OnSoldCount = onSoldCount;
		TodayIncome = todayIncome;
		LeftPubCount = leftPubCount;
	}
}