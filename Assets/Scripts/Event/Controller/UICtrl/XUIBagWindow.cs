using UnityEngine;
using System;
using System.Collections;
using XGame.Client.Packets;
using System.Collections.Generic;

class XUIBagWindow : XUICtrlTemplate<XBagWindow>
{
	LogicItemSort	ItemSort = new LogicItemSort();
	private static int CurSelPage = -1;
	public static int CurSelItemType = -1;
	private bool IsIconInit = false;
	public XBagActionIcon[] BagActionIcon = new XBagActionIcon[XBagWindow.MAX_BAG_ITEM_NUM];
	
	public static GameObject OnShowForceGuideObj = null;
	
	//物品总的分类管理
	class LogicItemSort
	{
		public UISpriteGroup 	SpriteGroup;		
		
		public void OnSelectModify(int index)
		{
			Show(index,XUIBagWindow.CurSelPage,false);
		}
		
		public void Show(int itemType,int pageID,bool isForce)
		{
			if(!isForce && XUIBagWindow.CurSelItemType	== itemType && XUIBagWindow.CurSelPage	== pageID)
				return ;
			else
			{
				XUIBagWindow.CurSelItemType	= itemType;
				XUIBagWindow.CurSelPage		= pageID;
				
				if(XUIBagWindow.CurSelItemType == 0)
					ItemTotal.Show(XUIBagWindow.CurSelPage);
				else
				{
					ItemOtherType.Show(XUIBagWindow.CurSelItemType,XUIBagWindow.CurSelPage);
				}
			}
		}
		
		public LogicItemTotal		ItemTotal			= new LogicItemTotal();
		public LogicItemOtherType	ItemOtherType		= new LogicItemOtherType();
	}
	
	class LogicItemTotal
	{	
		public XBagActionIcon[] BagActionIcon;
		public UILabel	CountLabel;
		public void Init()
		{			
			for(int i = 0;i < XBagWindow.MAX_BAG_ITEM_NUM; i++)
			{
				BagActionIcon[i].ResetUIAndLogic();
				BagActionIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Bag,(uint)i);
			}
		}
		public void Show(int curSelPage)
		{
			showAllBagItem(curSelPage);
		}	
		
		public void UpdateBagItemHandler(EEvent evt, params object[] args)
		{
			if(CountLabel == null)
				return ;
			
			CountLabel.text	= XLogicWorld.SP.MainPlayer.ItemManager.GetCurItemSpaceCount(EItemBoxType.Bag).ToString() + "/" + XLogicWorld.SP.MainPlayer.ItemManager.GetFullSpaceCount(EItemBoxType.Bag).ToString();
		}
		
		private void showAllBagItem(int pageID)
		{
			ShowPage(pageID);
			//显示背包中的物品
			for(short i = 0;i < XBagWindow.MAX_BAG_ITEM_NUM; i++)
			{
				//清空图标
				BagActionIcon[i].Reset();
				//该栏位是否已经购买
				int Base = pageID * XBagWindow.MAX_BAG_ITEM_NUM;
				bool isOpen = XItemSpaceMgr.SP.IsSet((short)(Base + i));
				BagActionIcon[i].ShowUnOpen(isOpen);
				XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)EItemBoxType.Bag,(short)(Base + i));
				if(tempItem == null || tempItem.IsEmpty())
					continue;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
				if(cfgItem == null)
					continue;
				int tt = (int)tempItem.Color;
				BagActionIcon[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount,true,tempItem.mEquipAttr.mStrengthenLevel);
			}
			
			CountLabel.text	= XLogicWorld.SP.MainPlayer.ItemManager.GetCurItemSpaceCount(EItemBoxType.Bag).ToString() + "/" + XLogicWorld.SP.MainPlayer.ItemManager.GetFullSpaceCount(EItemBoxType.Bag).ToString();
		}
		
		private void ShowPage(int pageID)
		{
			int startIndex = pageID * (int)EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
			for(int i = 0; i < XBagWindow.MAX_BAG_ITEM_NUM; i++)
			{
				BagActionIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Bag,(uint)(startIndex + i) );
			}
		}
	}
	
	class LogicItemOtherType
	{
		private int[] UIType2ItemType = {0,1,2,4,8};
		public XBagActionIcon[] BagActionIcon;
		private ArrayList mTempItemList = new ArrayList();
		public void Show(int itemType,int curSelPage)
		{
			ReSetUI();
			mTempItemList.Clear();
			//根据类型找到背包中的所有相同类型的物品
			int beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
			int endIndex	= XItemManager.GetEndIndex(EItemBoxType.Bag);
			int i = 0;
			for(i = beginIndex; i <= endIndex; i++)
			{ 
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)i);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				if(cfgItem.TypeTag == UIType2ItemType[itemType])
					mTempItemList.Add(LogicItem.ItemIndex);
			}	
			
			int Base = curSelPage * XBagWindow.MAX_BAG_ITEM_NUM;
			if(Base >= mTempItemList.Count)
				return ;
			
			for(i = 0;i < XBagWindow.MAX_BAG_ITEM_NUM && i < mTempItemList.Count; i++)
			{
				int tempIndex = Base + i;
				ushort itemIndex = (ushort)mTempItemList[tempIndex];
				BagActionIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Bag,(uint)itemIndex);
				
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)itemIndex);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				BagActionIcon[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,LogicItem.Color,LogicItem.ItemCount,true,LogicItem.mEquipAttr.mStrengthenLevel);
			}
		}
		
		private void ReSetUI()
		{
			for(int i = 0;i < XBagWindow.MAX_BAG_ITEM_NUM; i++)
			{				
				BagActionIcon[i].ResetUIAndLogic();
			}
		}		
	}
	
	public XUIBagWindow()
	{
		XEventManager.SP.AddHandler(OnFirstOpenBagWindow, EEvent.Bag_FirstOpen);
		XEventManager.SP.AddHandler(OnUpdateBagWindow, EEvent.Bag_Update);
		XEventManager.SP.AddHandler(OnUpdateSprite, EEvent.Bag_SetSprite);
		XEventManager.SP.AddHandler(OnChangePage,EEvent.Bag_ChangePage);
		XEventManager.SP.AddHandler(OnUpdateItemSpace,EEvent.Bag_UpdateItemSpace);
		XEventManager.SP.AddHandler(OnUpdateItem,EEvent.Bag_ItemUpdate);
		XEventManager.SP.AddHandler(SealEventHandler,EEvent.Bag_ItemSeal);
		XEventManager.SP.AddHandler(ItemArrangeHandler,EEvent.Bag_Arrange);
		XEventManager.SP.AddHandler(BagItemUseGuide, EEvent.Bag_Guide);
		XEventManager.SP.AddHandler(ItemSort.ItemTotal.UpdateBagItemHandler, EEvent.Bag_UpdateNum);
		
		RegEventAgent_CheckCreated(EEvent.Bag_ShowEffect, ShowBagItemEffect);
	}
	
	//初始化UI挂接部分,整个背包所有功能用同一组UI控件
	private void InitIcon(XActionIcon[] xai)
	{
		if(IsIconInit)
			return ;
		
		for(int i = 0;i < XBagWindow.MAX_BAG_ITEM_NUM; i++)
		{
			BagActionIcon[i]	= new XBagActionIcon();
			BagActionIcon[i].SetUIIcon(xai[i]);
		}
		
		IsIconInit	= true;
	}
	
	public void OnChangePage(EEvent evt, params object[] args)
	{
		int pageID = (int)args[1];
		ItemSort.Show(CurSelItemType,pageID,false);
	}
	
	public void OnUpdateItem(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		ushort currIndex= (ushort)args[0];
		int tempCurPageID = (int)currIndex/(int)XBagWindow.MAX_BAG_ITEM_NUM;
		if(tempCurPageID == LogicUI.CurPageID)		
		{
			int index = (int)currIndex%(int)XBagWindow.MAX_BAG_ITEM_NUM;
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)EShareItemBox.eshItemBox_Bag,(short)index);
			if(item == null)
				return ;
			
			if(item.IsEmpty())
				ItemSort.ItemTotal.BagActionIcon[index].Reset();
			else
			{
				//更新所有该栏位物品数据
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				if(cfgItem == null)
					return ;
			
			ItemSort.ItemTotal.BagActionIcon[index].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,item.Color,item.ItemCount,true,item.mEquipAttr.mStrengthenLevel);
			}			
		}
		
		//通知角色界面更新
		XEventManager.SP.SendEvent(EEvent.Bag_UpdateItemContainer);
		
	}
	
	public void OnUpdateItemSpace(EEvent evt, params object[] args)
	{	
		if(LogicUI == null && !LogicUI.gameObject.activeSelf)
			return ;

		ushort curIndexPos = (ushort)args[0];
		ushort willOpenPos = (ushort)args[1];

		for(ushort i = curIndexPos; i <= willOpenPos; i++)
		{
			int tempCurPageID = (int)i/(int)XBagWindow.MAX_BAG_ITEM_NUM;
			if(tempCurPageID == LogicUI.CurPageID)
			{
				int index = (int)i%(int)XBagWindow.MAX_BAG_ITEM_NUM;
				ItemSort.ItemTotal.BagActionIcon[index].ShowUnOpen(true);
			}
		}
		
//		uint currIndex= (uint)args[0];
//		bool open		= (bool)args[1];
//		int tempCurPageID = (int)currIndex/(int)XBagWindow.MAX_BAG_ITEM_NUM;
//		if(tempCurPageID == LogicUI.CurPageID)		
//		{
//			int index = (int)currIndex%(int)XBagWindow.MAX_BAG_ITEM_NUM;
//			ItemSort.ItemTotal.BagActionIcon[index].ShowUnOpen(open);
//		}
	}
	
	public override void OnShow()
	{	
//		if(!XItemPacketProcess.SP.IsOpenedBag)
//		{
//			XItemPacketProcess.SP.IsOpenedBag = true;
//			XEventManager.SP.SendEvent(EEvent.Bag_FirstOpen, EUIPanel.eBagWindow);
//		}
		
		InitIcon(LogicUI.ActionIconArray);
		string name = XLogicWorld.SP.MainPlayer.Name;
		ItemSort.SpriteGroup	= LogicUI.ItemSort;
		ItemSort.SpriteGroup.mModify	= new UISpriteGroup.OnSelectModify(ItemSort.OnSelectModify);
		ItemSort.ItemTotal.BagActionIcon		= BagActionIcon;
		ItemSort.ItemOtherType.BagActionIcon	= BagActionIcon;
		ItemSort.ItemTotal.CountLabel			= LogicUI.CountLabel;
		ItemSort.ItemTotal.Init();
		ItemSort.Show(0,0,true);
	}
	
	public override void OnHide()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipA);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.ePopMenu);
		base.OnHide();
	}
	
	public void OnFirstOpenBagWindow(EEvent evt, params object[] args)
	{
		CS_FirstOpenBag.Builder builder = CS_FirstOpenBag.CreateBuilder();		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_FirstOpenBag, builder.Build());		
	}
	
	public void OnUpdateBagWindow(EEvent evt, params object[] args)
	{
		if(LogicUI != null && LogicUI.gameObject.activeSelf)
			ItemSort.Show(0,0,true);
	}
	
	public void OnUpdateSprite(EEvent evt, params object[] args)
	{
		EItemBoxType srcType = (EItemBoxType)args[1];
		ushort		 srcIndex = (ushort)args[2];

		if(srcIndex > (ushort)EBAG_DATA.ROLE_MAX_COM_BAG_ITEM_NUM)
			return ;
		
		int slotIndex = srcIndex % (ushort)EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
		XItem srcItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)srcType, (short)srcIndex);
		if (srcItem.IsEmpty())
		{
			if(ItemSort.ItemTotal.BagActionIcon != null)
				ItemSort.ItemTotal.BagActionIcon[slotIndex].Reset();
			return ;
		}

		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
		if (cfgItem == null)
			return;
		
		if(LogicUI != null)
			ItemSort.ItemTotal.BagActionIcon[slotIndex].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,srcItem.Color,srcItem.ItemCount,true,srcItem.mEquipAttr.mStrengthenLevel);
	}
	
	public void SealEventHandler(EEvent evt, params object[] args)
	{
		if(XMouseStateMgr.SP.mouseState == EMouseState.EMouseState_Seal)
		{
			XMouseStateMgr.SP.mouseState = EMouseState.EMouseState_None;
			CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
		}
		else
		{
			XMouseStateMgr.SP.mouseState = EMouseState.EMouseState_Seal;
			CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_Seal);
		}
	}
	
	public void ItemArrangeHandler(EEvent evt, params object[] args)
	{
		XItemPacketProcess.SP.SendItemArrange();
	}
	
	public void ShowBagItemEffect(EEvent evt, params object[] args)
	{
		if ( LogicUI == null )
			return;
		
		LogicUI.ShowItemEffect((int)args[0], (int)args[1], (uint)args[2]);
	}
	
	private void BagItemUseGuide(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		EItem_Type itemType = (EItem_Type)args[0];
		
		//根据类型找到背包中的所有相同类型的物品
		int beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
		int endIndex	= XItemManager.GetEndIndex(EItemBoxType.Bag);
		int pos = -1;
		for(int i = beginIndex; i <= endIndex; i++)
		{ 
			XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)i);
			if(LogicItem == null || LogicItem.IsEmpty())
				continue;
				
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
			if(cfgItem == null)
				continue;
				
			if( (int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip != (int)args[1] &&
				(EItem_Type)cfgItem.ItemType == itemType)
			{
				pos = i;
				break;
			}
			else if ( (int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip == (int)args[1] && 
				XNewPlayerGuideManager.SP.ItemId == cfgItem.Index )
			{
				pos = i;
				break;
			}
		}	
		if ( pos <0 )
			return;
		
		if ( null == BagActionIcon[pos].mUIIcon )
			return;
		
		GameObject obj =  BagActionIcon[pos].mUIIcon.gameObject;
		Vector3 showPos = new Vector3(Mathf.Round(BagActionIcon[pos].mUIIcon.transform.localPosition.x - 241), 
			Mathf.Round(BagActionIcon[pos].mUIIcon.transform.localPosition.y + 133), -250);
		
//		if ( (int)XNewPlayerGuideManager.GuideType.Guide_ItemEquip == (int)args[1] )
//		{
//			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_ItemEquip, 1, showPos, parentObj);
//		}
//		else if ( (int)XNewPlayerGuideManager.GuideType.Guide_PetUse == (int)args[1] )
//		{
//			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_PetUse, 1, showPos, parentObj);
//		}
//		if ( (int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click == (int)args[1] )
//		{
//			XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Drag, 
//				(int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click, 2, showPos, parentObj);
//		}
		if ( (int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip == (int)args[1] ) 
		{
			XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip, 2, showPos, obj);
		}
	}
}