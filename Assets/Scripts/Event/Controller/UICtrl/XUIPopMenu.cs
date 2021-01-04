
using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using System;

class XUIPopMenu : XUICtrlTemplate<XPopMenu>
{	
	public static bool isOper = false;
	private EItemBoxType type;
	private int data;
	private bool m_bShowShopDialog = false;
	private string name;
	private System.UInt64 id;
	
	public XUIPopMenu()
	{
		RegEventAgent_CheckCreated(EEvent.PopMenu_Data, OnGetPopMenuData);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Equip, OnPopMenuEquip);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Use, OnPopMenuUse);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Drop, OnPopMenuDrop);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Split, OnPopMenuSplit);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Decompose, OnPopMenuDecompose);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Compose, OnPopMenuCompose);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Upload, OnPopMenuUpload);
		RegEventAgent_CheckCreated(EEvent.PopMenu_NameData, OnPopNameData);
		RegEventAgent_CheckCreated(EEvent.PopMenu_SendMsg, OnSendMessage);
		RegEventAgent_CheckCreated(EEvent.PopMenu_SendEmail, OnSendEmail);
		RegEventAgent_CheckCreated(EEvent.PopMenu_LookDetail, OnLookDetail);
		RegEventAgent_CheckCreated(EEvent.PopMenu_AddFriend, OnAddFriend);
		RegEventAgent_CheckCreated(EEvent.PopMenu_BuyItem, OnBuyItemClick);
		RegEventAgent_CheckCreated(EEvent.PopMenu_BuyBackItem, OnBuyBackItemClick);
		RegEventAgent_CheckCreated(EEvent.PopMenu_Auction, OnClickAuction);
		//RegEventAgent_CheckCreated(EEvent.PopMenu_GuildKickMem, OnClickGuildKickMem);
		
		RegEventAgent_CheckCreated(EEvent.PopMenu_GuildKickMem,OnClickGuildKickMem);
		
		XEventManager.SP.AddHandler(OnShopVisiableChange,EEvent.ShopDialog_ChangeVisiable);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		//isOper	= true;
	}
	
	public void OnPopNameData(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		name = (string)args[0];
		id = (ulong)args[1];
		LogicUI.AddBtn(new XPopMenu.XBtnName(XStringManager.SP.GetString(1036), EEvent.PopMenu_SendMsg));
		// LogicUI.AddBtn(new XPopMenu.XBtnName(XStringManager.SP.GetString(1037), EEvent.PopMenu_SendEmail));
		LogicUI.AddBtn(new XPopMenu.XBtnName(XStringManager.SP.GetString(1038), EEvent.PopMenu_LookDetail));
		LogicUI.AddBtn(new XPopMenu.XBtnName(XStringManager.SP.GetString(1039), EEvent.PopMenu_AddFriend));
	}
	
	public void OnSendMessage(EEvent evt, params object[] args)
	{
		if ( id != XLogicWorld.SP.MainPlayer.ID )
			XEventManager.SP.SendEvent(EEvent.Chat_OpenPrivate, name, id);
	}	
	
	public void OnSendEmail(EEvent evt, params object[] args)
	{
	}	
	
	public void OnLookDetail(EEvent evt, params object[] args)
	{
		if ( id != XLogicWorld.SP.MainPlayer.ID )
			XEventManager.SP.SendEvent(EEvent.Chat_ShowPlayerInfoReq, name);
	}	
	
	public void OnAddFriend(EEvent evt, params object[] args)
	{
		if ( id != XLogicWorld.SP.MainPlayer.ID )
			XEventManager.SP.SendEvent(EEvent.Freind_ChatAddFriend, name);
	}
	
	public void OnBuyItemClick(EEvent evt, params object[] args)
	{
		LogicUI.AddBtn(new XPopMenu.XBtnBuy(XStringManager.SP.GetString(86),(uint)args[0], (ActionIcon_Type)args[1])  );
	}
	
	public void OnBuyBackItemClick(EEvent evt, params object[] args)
	{
		LogicUI.AddBtn(new XPopMenu.XBtnBuyBack(XStringManager.SP.GetString(87) ,(uint)args[0] )  );
	}
	
	public void OnClickAuction(EEvent evt, params object[] args)
	{
		 XEventManager.SP.SendEvent(EEvent.auction_DragPublish, EUIPanel.eBagWindow, (uint)type,(uint)data);
	}
	
	public void OnClickGuildKickMem(EEvent evt, params object[] args)
	{
		LogicUI.AddBtn(new XPopMenu.XButGuildKickMem(XStringManager.SP.GetString(717), (UInt64)args[0] ) );
	}
	
	public void OnGetPopMenuData(EEvent evt, params object[] args)
	{
		type 	= (EItemBoxType)args[1];
		data	= (int)args[2];
		
		//在这里根据物品数据，生成不同的选项
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		if(LogicUI == null)
			return ;
		
		// 物品使用引导中只显示装备
		bool onShowGuide = XNewPlayerGuideManager.SP.OnItemGuide;
		
		//1.装备部位是否有效
		if(cfgItem.EquipPos > (byte)EQUIP_SLOT_TYPE.EQUIP_SLOT_NONE && cfgItem.EquipPos <= (byte)EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION)
		{
			EItemBoxType equipType;
			ushort equipData;
			XItemManager.GetContainerType(item.ItemIndex,out equipType,out equipData);			
			if(equipType == EItemBoxType.Bag)
				LogicUI.AddBtn(new XPopMenu.XBtnEquip(XStringManager.SP.GetString(24)));
			else if(equipType == EItemBoxType.Equip || equipType == EItemBoxType.PetEquip)
				LogicUI.AddBtn(new XPopMenu.XBtnEquip(XStringManager.SP.GetString(25)));
		}
		
		//2.是否能合成
		if(cfgItem.IsCanCombine > 0)
		{
			LogicUI.AddBtn(new XPopMenu.XBtnCompose(XStringManager.SP.GetString(26)));
		}
		
		//4.是否能使用
		if(cfgItem.UseType == 1)
		{
			LogicUI.AddBtn(new XPopMenu.XBtnUse(XStringManager.SP.GetString(43)));
		}
		
		//5.是否能拆分
		if(cfgItem.PileAmount > 1 && !onShowGuide)
		{
			if(item.ItemCount > 1)
				LogicUI.AddBtn(new XPopMenu.XBtnSplit(XStringManager.SP.GetString(28)));
		}		
		//6.是否能分解
		if(cfgItem.IsCanExplode > 0 && !onShowGuide)
		{
			LogicUI.AddBtn(new XPopMenu.XBtnDecompose(XStringManager.SP.GetString(29)));
		}
		
		//3.是否能丢弃
		if(cfgItem.IsCanDrop == 0 && !onShowGuide)
		{
			LogicUI.AddBtn(new XPopMenu.XBtnDrop(XStringManager.SP.GetString(27)));
		}
		
		//是否能出售
		if( EItemBoxType.Bag==type && 	// only bag type can sell
			0==cfgItem.IsCanSell &&     // can sell
			m_bShowShopDialog )         // only can sell when shopDialog is showing
		{
			LogicUI.AddBtn(new XPopMenu.XBtnSell(XStringManager.SP.GetString(58), data)  );
		}
		
		// 是否可以拍卖
		if ( EItemBoxType.Bag==type &&
			cfgItem.IsCanAuction == 0 &&  
			0 == item.IsBinding && 
			XAuctionUI.AuctionShow && 
			!onShowGuide)
		{
			LogicUI.AddBtn(new XPopMenu.XBtnAuction(XStringManager.SP.GetString(265)));
		}
		
		if ( !onShowGuide )
			LogicUI.AddBtn( new XPopMenu.XBtnUpload(XStringManager.SP.GetString(89)));
	}
	
	public void OnPopMenuEquip(EEvent evt, params object[] args)
	{
		Hide();
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		//装备部位是否有效
		if(cfgItem.EquipPos <= (byte)EQUIP_SLOT_TYPE.EQUIP_SLOT_NONE)
			return ;
		
		if(type == EItemBoxType.Equip || type == EItemBoxType.PetEquip)
		{
			short pos = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNoPile(EItemBoxType.Bag);
			if(pos != -1)
			{
				XItemManager.CharSwapItem(type,(ushort)data,EItemBoxType.Bag,(ushort)pos);
			}
		}
		else if(type == EItemBoxType.Bag)
		{
//			if((EItem_Type)cfgItem.ItemType == EItem_Type.EITEM_TYPE_ARMOR ||
//				(EItem_Type)cfgItem.ItemType == EItem_Type.EITEM_TYPE_WEAPON )
//			{
//				XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_ItemEquip);
//			}
			if( cfgItem.Index == XNewPlayerGuideManager.SP.ItemId )
			{
				XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip);
			}
			
			XUIRoleInformation roleUI = XUIManager.SP.GetUIControl(EUIPanel.eRoleInformation) as XUIRoleInformation;
			
			int equipPos = XLogicWorld.SP.MainPlayer.ItemManager.GetEquipPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);
			int itemIndex = XItemManager.GetRealItemIndex(roleUI.GetCurSelObject(),equipPos);
				
			EItemBoxType targetType;
			ushort targetIndex;
			XItemManager.GetContainerType((ushort)itemIndex,out targetType,out targetIndex);			
			XItemManager.CharSwapItem(type,(ushort)data,targetType,targetIndex);
			
		}	
	}
	
	public void OnPopMenuUse(EEvent evt, params object[] args)
	{
		Hide();
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		if(cfgItem.ItemType	== (ushort)EItem_Type.EITEM_TYPE_GOODS && cfgItem.ItemSubType == (ushort)EITEM_GOODS_SUB_TYPE.EITEM_GOODS_SUB_TYPE_LD)
		{			
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eRoleInformation);
			XEventManager.SP.SendEvent(EEvent.CharInfo_LingDan);
		}
		
		if(cfgItem.IsContainer > 0)
		{
			ushort emptyPosNum = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
			int addItemCount = 0;
			for(int i = 0; i < 6;i++)
			{
				if(cfgItem.CTItemID[i] > 0)
					addItemCount++;
			}
			
			if(emptyPosNum < addItemCount)
			{				
				string temp = string.Format(XStringManager.SP.GetString(64),addItemCount);
				XEventManager.SP.SendEvent(EEvent.Chat_Notice,temp);
				return ;
			}
			
			if(!XLogicWorld.SP.MainPlayer.ItemManager.IsHasKey(cfgItem.LockLevel))
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,14);
				return ;
			}
		}
		else if(cfgItem.AddHealth > 0)
		{
			if(XLogicWorld.SP.MainPlayer.EatFoodCount > 3)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
				return ;
			}
		}
		
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,(ushort)data);
		XItemPacketProcess.SP.SendUseItem(realIndex);

	}
	
	public void OnPopMenuDrop(EEvent evt, params object[] args)
	{
		if(type != EItemBoxType.Bag)
			return ;
		
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(DropItemOK);
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,(ushort)data);
		
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(realIndex);
		if(item.IsEmpty())
			return ;
		XCfgItem CfgDelItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		string DelTemp = XGameColorDefine.Quality_Color[(int)item.Color] + CfgDelItem.Name;
		string content = "[color=ffffff]";
		content += string.Format(XStringManager.SP.GetString(72),DelTemp);
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
		
		Hide();
	}
	
	public void DropItemOK(GameObject go)
	{
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,(ushort)data);
		XItemPacketProcess.SP.SendItemDrop(realIndex);
	}
	
	public void OnPopMenuSplit(EEvent evt, params object[] args)
	{
		Hide();
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,(ushort)data);
		
		XMouseOp op = XMouseStateMgr.SP.GetMouseOp(EMouseState.EMouseState_Split);
		if(op == null)
			return ;		
		XItemSplit split = op as XItemSplit;
		if(split == null)
			return ;		
		split.SrcItemIndex	= realIndex;
		
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(SplitItemOK);
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(realIndex);
		XEventManager.SP.SendEvent(EEvent.InputMessageBoxSetMaxValue,(int)item.ItemCount);
		string content = "";
		XEventManager.SP.SendEvent(EEvent.InputMessageBox,funcOK,null,content);	
	}
	
	public void OnPopMenuDecompose(EEvent evt, params object[] args)
	{
		Hide();
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		if(!item.IsCanDecompose())
		{
			XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(59));
			return ;
		}
		
		//只有背包中的物品能分解
		ushort start = XItemManager.GetBeginIndex(EItemBoxType.Bag);
		ushort end	 = XItemManager.GetEndIndex(EItemBoxType.Bag);
		
		if(item.ItemIndex >= start && item.ItemIndex <= end)
			XItemPacketProcess.SP.SendItemDecompose((ushort)data);
	}
	
	public void OnPopMenuUpload(EEvent evt, params object[] args)
	{
		Hide();
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XEventManager.SP.SendEvent(EEvent.Chat_SetChatItemData, item);	
	}
	
	public void OnPopMenuCompose(EEvent evt, params object[] args)
	{
		Hide();
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)data);
		if(item == null)
			return ;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		if(cfgItem.IsCanCombine == 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Chat,255);			
			return ;
		}
		
		
		int itemTotalCount = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(item.DataID);
		if(itemTotalCount < (int)cfgItem.CombineCostAmount)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Chat,256,cfgItem.CombineCostAmount);
			return ;
		}
		
		XItemPacketProcess.SP.SendItemCombine((ushort)data);
		
		
	}
	
	private void OnShopVisiableChange(EEvent evt, params object[] args)
	{
		m_bShowShopDialog = (bool)args[0];
	}
	
	private void SplitItemOK(GameObject go)
	{
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,(ushort)data);
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(realIndex);
		if(item.IsEmpty())
			return ;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		XMouseOp op = XMouseStateMgr.SP.GetMouseOp(EMouseState.EMouseState_Split);
		if(op == null)
			return ;		
		XItemSplit split = op as XItemSplit;
		if(split == null)
			return ;
		
		int itemCount = 0;
		if(int.TryParse(XInputMessageBox.Content,out itemCount))
		{
			split.SplitItemCount	= itemCount;
		}
		
		XMouseStateMgr.SP.mouseState	= EMouseState.EMouseState_Split;
		
		CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_Split);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCursor);
		XEventManager.SP.SendEvent(EEvent.Cursor_UpdateIconData,EUIPanel.eCursor,cfgItem.IconAtlasID,cfgItem.IconID,item.Color,item.ItemCount);
	}
}
