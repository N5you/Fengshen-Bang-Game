using UnityEngine;
using System.Collections;
using XGame.Client.Base.Pattern;
using Google.ProtocolBuffers;
using XGame.Client.Packets;
using System.Collections.Generic;
using XGame.Client.Network;
using XGame.Client.Base;

//shop mgr
public class XShopItemMgr:XSingleton<XShopItemMgr>
{
	enum EItemSellType
	{
		eItemType     = 0,  //物品模板货币类型
		eCurType      = 1,
		eCurConItem = 2,
		
	}
	
	private uint m_npcID = 0;
	public uint npcID{
		set{ m_npcID = value; }
		get{ return m_npcID; }
	}
	
	// 商店物品价格相关信息
	public class ShopItemPriceMsg
	{
		public uint m_price = 0;
		public ECS_CURRENCY_TYPE m_priceType = 0;
		public uint m_convertItemID = 0;
		public uint m_uLvl;
		public string m_strSpriteName = "";
	}
	
	// 回购物品最大数
	public static int ShopBuyBackNum = 10;
	// 回购物品列表
	private XItem[] m_buyBackList = new XItem[ShopBuyBackNum];
	
	public XShopItemMgr()
	{
		
	}
	
	public XItem getBuyBackItem(int index)
	{
		if(ShopBuyBackNum<=index)
			return null;
		
		return m_buyBackList[index];
	}
	// 得到商店物品价格相关
	public void resolveShopItemPriceMsg( uint NpcID, uint itemID, out ShopItemPriceMsg itemShopMsg)
	{
		itemShopMsg = new ShopItemPriceMsg();
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID);
		if(cfgItem == null)
			return ;
		
		XCfgShopItem cfgShopItem = XCfgShopItemMgr.SP.GetConfig(NpcID, itemID);
		if(cfgShopItem == null)
			return;
		
		uint iPrice			= 0;
		uint iPriceType 	= 0;
		uint iConItem		= 0;
		uint iConItemCount = 0;
		switch((uint)cfgShopItem.sellType)
		{
		case (uint)EItemSellType.eItemType:
				itemShopMsg.m_price			 = cfgItem.BuyPrice;
				itemShopMsg.m_priceType  = (ECS_CURRENCY_TYPE)cfgItem.ShopMoneyType;
			break;
		case (uint)EItemSellType.eCurType:
				itemShopMsg.m_price			 = cfgShopItem.specialCoinHowMuch;
				itemShopMsg.m_priceType  = (ECS_CURRENCY_TYPE)cfgShopItem.specialCoinType;
			break;
		case (uint)EItemSellType.eCurConItem:
				itemShopMsg.m_priceType  		= ECS_CURRENCY_TYPE.ECURRENCY_TYPE_ITEM;
				itemShopMsg.m_price					= cfgShopItem.convertItemCount;
				itemShopMsg.m_convertItemID 	= cfgShopItem.convertItemID;
			break;
		default:    break;
		}
	}
	
	
	// 得到商店回购物品价格相关
	public void resolveShopItemBackPriceMsg( uint itemID ,out ShopItemPriceMsg itemShopMsg)
	{
		itemShopMsg = new ShopItemPriceMsg();
		
		XCfgItem itemBase = XCfgItemMgr.SP.GetConfig(itemID);
		if(itemBase == null) return;
		
		//XCfgShopItem shopItemBase = XCfgShopItemMgr.SP.GetConfig(NpcID,itemID);
		float fMultiple = 1.0f;
		if(itemBase.IsRandom == 1)
		{
			XCfgColorValue cfgColorValue = XCfgColorValueMgr.SP.GetConfig(itemBase.QualityLevel);
			fMultiple = cfgColorValue.PriceRate;
		}
		
		itemShopMsg.m_priceType = ECS_CURRENCY_TYPE.ECURRENCY_TYPE_GAME_MONEY;
		itemShopMsg.m_price = (uint)(itemBase.SellPrice * fMultiple);
	}
		
		
	// 得到声望商店物品价格相关
	public void resolveShengWangItemPriceMsg(uint itemID, out ShopItemPriceMsg itemShopMsg)
	{
		itemShopMsg = new ShopItemPriceMsg();
		
		/*XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID);
		if(cfgItem == null)
			return ;*/
		
		XCfgShengWangItem cfgShengWangItem = XCfgShengWangItemMgr.SP.GetConfig(itemID);
		if(cfgShengWangItem == null)
			return;

		itemShopMsg.m_price			 = cfgShengWangItem.MoneyValue;
		itemShopMsg.m_priceType  = (ECS_CURRENCY_TYPE)cfgShengWangItem.MoneyType;
		itemShopMsg.m_uLvl			 = cfgShengWangItem.PrestigeLvBuy;
	}
	
	// 获取货币图标
	public string resolvePriceTypeUISpriteName(ECS_CURRENCY_TYPE priceType)
	{
		string strSprite = "";
		switch( priceType )
		{
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_GAME_MONEY:
			strSprite = "11000057";
			break;
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_REAL_MONEY:
			strSprite = "11000056";
			break;
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_REPUTATION:
			strSprite = "11000060";
			break;
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_ITEM:
			strSprite = "11000061";
			break;
		}
		
		return strSprite;
		
	}
	// 购买物品 请求服务器
	public void requestBuyItemToBagIndex(EShopBuyType buyType,uint npcID, uint itemID, int bagIndex)
	{
		ushort emptyPosCount = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		if(emptyPosCount == 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10);
			return ;
		}
		
		ShopItemPriceMsg itemShopMsg = null;
		if(EShopBuyType.eComBuy == buyType)
		{
			resolveShopItemPriceMsg(npcID, itemID, out itemShopMsg);
		}
		else
		{
			resolveShengWangItemPriceMsg(itemID, out itemShopMsg);
		}
		
		if(itemShopMsg == null || itemShopMsg.m_priceType == (uint)ECS_CURRENCY_TYPE.ECURRENCY_TYPE_NONE)
		{
			return;
		}
		
		switch(itemShopMsg.m_priceType)
		{
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_GAME_MONEY:
			if(itemShopMsg.m_price > XLogicWorld.SP.MainPlayer.GameMoney)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 294);
				return;
			}
			break;
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_REAL_MONEY:
			if(itemShopMsg.m_price > XLogicWorld.SP.MainPlayer.RealMoney)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 295);
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
				return;
			}
			break;
		case ECS_CURRENCY_TYPE.ECURRENCY_TYPE_REPUTATION:
			if(itemShopMsg.m_price > XLogicWorld.SP.MainPlayer.ShengWangValue)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 359);
			}
			break;
		case 	ECS_CURRENCY_TYPE.ECURRENCY_TYPE_ITEM:
			if(itemShopMsg.m_price > XLogicWorld.SP.MainPlayer.GetItmeCount(itemShopMsg.m_convertItemID))
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 296);
				return;
			}	
			break;
		}
		 	
		CS_Try_BuyItem.Builder msg = CS_Try_BuyItem.CreateBuilder();
		msg.SetBuyType((uint)buyType);
		msg.SetNpcId(npcID);
		msg.SetItemId(itemID);
		msg.SetBagIndex(bagIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_TryBuyItem,msg.Build() );
	}
	
	public void requestBuyItemToBagIndex(uint itemID,int bagIndex, ActionIcon_Type actionType)
	{
		//requestBuyItemToBagIndex(EShopBuyType.eComBuy, m_npcID,itemID,bagIndex );
		if(actionType == ActionIcon_Type.ActionIcon_SWShop)
		{
			requestBuyItemToBagIndex(EShopBuyType.eShwBuy, m_npcID,itemID,bagIndex);
		}
		else
		{
			requestBuyItemToBagIndex(EShopBuyType.eComBuy, m_npcID,itemID,bagIndex);	
		}
	}
	
	public void requestBuyItem(uint itemID, ActionIcon_Type actionType)
	{
		if(actionType == ActionIcon_Type.ActionIcon_SWShop)
		{
			requestBuyItemToBagIndex(EShopBuyType.eShwBuy, m_npcID,itemID,0);
		}
		else
		{
			requestBuyItemToBagIndex(EShopBuyType.eComBuy, m_npcID,itemID,0);	
		}
	}
	
	public void requestBuyItem(uint npcID,uint itemID)
	{
		requestBuyItemToBagIndex(EShopBuyType.eComBuy, npcID,itemID,0);	
	}
	
	// 回购物品
	public void requestBuyBackItem(uint buyBackIndex)
	{
		requestBuyBackItem(buyBackIndex,0 );
	}
	
	// 回购物品
	public void requestBuyBackItem(uint buyBackIndex,int bagIndex)
	{
		CS_Try_BuyBackItem.Builder msg = CS_Try_BuyBackItem.CreateBuilder();
		
		msg.SetBuyBackIndex((int)buyBackIndex );
		msg.SetBagIndex(bagIndex);
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_TryBuyBackItem,msg.Build() );
	}
	
	
	
	// 向商店出售物品 请求服务器
	public void requestSellItem(uint index)
	{
		if(XLogicWorld.SP.GetMainPlayer().CanSellItem((uint)EItemBoxType.Bag, (short)index))
		{
			CS_UInt.Builder msg = CS_UInt.CreateBuilder();
			msg.SetData(index);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_TrySellItem,msg.Build() );
		}
		else 
		{
			XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,293);
		}
		
	}
	// 向服务器请求限购物品
	public void requestLimiteBuyItemList( uint npcID )
	{
		CS_UInt.Builder msg = CS_UInt.CreateBuilder();
		msg.SetData(npcID);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestNPCLimitList,msg.Build() );
	}
	//  服务器下发回购list
	public void updateBuyBackList(SC_ItemList msg)
	{
		//first clear
		for(int i=0;i<ShopBuyBackNum;i++ )
		{
			m_buyBackList[i] = null;
		}
		
		for (int i = 0; i < msg.ItemListCount; ++i)
        {
			PB_ItemInfo pbInfo = msg.GetItemList(i); 
			XItem temp = new XItem();
			temp.InitFromPB(pbInfo);
			
			m_buyBackList[i]=temp;
			
        }
		
		//update UI
		XEventManager.SP.SendEvent(EEvent.ShopDialog_UpdateBuyBack,null);
	}
	
	//  购买失败
	public void BuyItemFail(SC_UInt64 msg)
	{
		if((EShopItemMessage)msg.Data == EShopItemMessage.esim_NotEnoughLimitCount) 
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,298);
		}
	}
	
	
}
