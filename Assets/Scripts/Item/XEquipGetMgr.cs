using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public class XEquipGetMgr : XSingleton<XEquipGetMgr>	
{
	private List<UInt64>	HintItemList = new List<UInt64>();
	
	
	public XEquipGetMgr()
	{
		XEventManager.SP.AddHandler(LevelUpHandle,EEvent.Update_Level);
	}
	
	public void LevelUpHandle(EEvent evt, params object[] args)
	{
		FlushEquipHint();
	}
	
	public void FlushEquipHint()
	{
		XUIManager.SP.HideAllByPanelType(EUIPanel.eZhuangBeiTiShi);
		HintItemList.Clear();
		BagAllEquipHint();
	}
	
	public void BagAllEquipHint()
	{
		ushort beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex		= XItemManager.GetEndIndex(EItemBoxType.Bag);
		for(int i = beginIndex; i <= endIndex; i++)
		{
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.AllItemBox[i];
			if(item.IsEmpty())
				continue;
			
			GetNewEquip(item);
		}
	}
	
	public void GetNewEquip(XItem item)
	{
		XCharacter equipCh = IsNeedHint(item);
		if(equipCh == null)
			return ;
		
		HintItemList.Add(item.GUID);
		
		ESceneType curSceneType = XLogicWorld.SP.SceneManager.LoadedSceneType;
		if(ESceneType.NormalScene == curSceneType || ESceneType.ClientScene == curSceneType)
		{
			uint UIKey = XUIManager.SP.GetMuliUIObjectKey(EUIPanel.eZhuangBeiTiShi);
			XUTZhuangBeiTiShi ItemUI = XUIManager.SP.GetMuliUIObject(UIKey) as XUTZhuangBeiTiShi;
			ItemUI.SetItemData(equipCh,item);
			XEventManager.SP.SendEvent(EEvent.UI_MuliShow,UIKey);
		}
	}
	
	public void ShowAllHintEquip()
	{
		if(HintItemList.Count == 0)
			return ;
		
		
		List<UInt64> DelList = new List<UInt64>();
		foreach(UInt64 ItemGuid in HintItemList)
		{
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(ItemGuid);			
			XCharacter equipCh = IsNeedHint(item);
			if(equipCh == null)
			{
				DelList.Add(ItemGuid);
			}
			else
			{
				uint UIKey = XUIManager.SP.GetMuliUIObjectKey(EUIPanel.eZhuangBeiTiShi);
				XUTZhuangBeiTiShi ItemUI = XUIManager.SP.GetMuliUIObject((uint)UIKey) as XUTZhuangBeiTiShi;
				ItemUI.SetItemData(equipCh,item);
				XEventManager.SP.SendEvent(EEvent.UI_MuliShow,UIKey);
			}
		}
		
		foreach(UInt64 Guid in DelList)
		{
			HintItemList.Remove(Guid);
		}
	}
	
	//是否需要提示
	public  XCharacter IsNeedHint(XItem item)
	{
		if(item == null)
			return null;
		
		if ( !XNewPlayerGuideManager.SP.CanShowHint(item) )
			return null;
		
		//是否需要鉴定
		if(item.IsSeal)
			return null;
		
		//玩家自身
		if(ObjIsNeedHint(XLogicWorld.SP.MainPlayer,item))
		{			
			return XLogicWorld.SP.MainPlayer;
		}
		
		for(int i = 0;i < XLogicWorld.SP.PetManager.AllPet.Length; i++)
		{
			if(XLogicWorld.SP.PetManager.AllPet[i] == null)
				continue;
			if(ObjIsNeedHint(XLogicWorld.SP.PetManager.AllPet[i],item))
			{
				return XLogicWorld.SP.PetManager.AllPet[i];
			}
		}
		
		return null;
	}
	
	private bool ObjIsNeedHint(XCharacter ch,XItem item)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return false;
		
		//是否是装备
		if(!item.isEquip())
			return false;
		
		//等级是否符合
		if(ch.Level < cfgItem.RequireEquipUseLevel)
			return false;
		
		//职业是否符合
		if(!XItemManager.IsCanEquipByCareer(item.DataID,ch.DynGet(EShareAttr.esa_Class)))
			return false;
			
		//相同部位没有装备，直接提示
		ushort tempPos = (ushort)ch.ItemManager.GetEquipPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);
		if (tempPos != (int)EQUIP_POS.EQUIP_POS_INVALID)
		{
			int realIndex = XItemManager.GetRealItemIndex(ch,tempPos);
			XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)realIndex);
			if(tempItem.IsEmpty())
				return true;
			else
			{
				//有装备的话
				if(!IsGoodAttr(tempItem,item))
					return false;
				
				//是否可以装备现在
				EItemBoxType srcType;
				ushort	srcIndex;
				XItemManager.GetContainerType(item.ItemIndex,out srcType,out srcIndex);
				
				EItemBoxType targetType;
				ushort	targetIndex;
				XItemManager.GetContainerType(tempItem.ItemIndex,out targetType,out targetIndex);
				ESWAP_ERROR error = ch.ItemManager.IsCanSwapItem(srcType, srcIndex, targetType, targetIndex);
				if(error != ESWAP_ERROR.ESWAP_ERROR_OK)
					return false;
			}
		}
		
		return true;
	}
	
	private bool IsGoodAttr(XItem self,XItem newGet)
	{
		if((int)self.Color < (int)newGet.Color)
			return true;

		XCfgItem src = XCfgItemMgr.SP.GetConfig(self.DataID);
		if(src == null)
			return false;

		XCfgItem target = XCfgItemMgr.SP.GetConfig(newGet.DataID);
		if(target == null)
			return false;

		if(src.BaseAttrValue < target.BaseAttrValue)
			return true;

		return false;
	}
}
