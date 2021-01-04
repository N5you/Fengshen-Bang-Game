using System;
using UnityEngine;
using XGame.Client.Packets;
using System.Collections;
using System.Collections.Generic;

public enum ESWAP_ERROR
{
	ESWAP_ERROR_OK = 0,
	ESWAP_ERROR_OTHER,
	ESWAP_ERROR_SRC_EMPTY,
	ESWAP_ERROR_SRC_IS_NOT_EQUIP,
	ESWAP_ERROR_SLOT,
	ESWAP_ERROR_CAREER,
	ESWAP_ERROR_LEVEL,
	ESWAP_ERROR_Pet,
}


public class XItemManager
{	
	//äžä»¶è£å€æå€å¯ä»¥é¶åµå®ç³çæ°é
	static readonly public int MAX_INLAY_GEM_NUM = 5;
	//æå€§éæºå±æ§æ°ç	
	static readonly public int MAX_RANDOM_ATTR_NUM = 3;
	//æå€ç¬Šå°æ°ç	
	static readonly public int MAX_EQUIP_FUYIN_NUM = 4;		
    
    public XItem[] AllItemBox{ get; private set; }
	
	private XCharacter 		m_Owner;	
	private XItemSpaceMgr	m_SpaceMgr;

	private enum EQUIP_OP
	{
		EQUIP_OP_ADD = 1,	//穿
		EQUIP_OP_SUB = -1,	//脱
	}
	
	#region property and init
	public XItemManager(XCharacter xgo)
	{
		m_Owner	= xgo;
		AllItemBox = new XItem[(int)EBAG_DATA_POS.ROLE_MAX_ITEM_NUM];
		for(int i = 0; i < (int)EBAG_DATA_POS.ROLE_MAX_ITEM_NUM; i++)
		{
			AllItemBox[i] = new XItem();
		}
	}

    ~XItemManager()
    {
    }
	
	public void ClearAllItemData(EItemBoxType boxType)
	{
		ushort beginIndex 	= XItemManager.GetBeginIndex(boxType);
		ushort endIndex		= XItemManager.GetEndIndex(boxType);
		for(int i = beginIndex; i <= endIndex; i++)
		{
			XItem item = AllItemBox[i];
			item.Reset();
		}
	}
	
	public void SetSpaceMgr(XItemSpaceMgr mgr)
	{
		m_SpaceMgr	= mgr;
	}
	#endregion
	
	
	#region Base Function
	//srcæ¯æ¿èµ·çç©å,targetæ¯èŠæŸå°çç©å	
	public ESWAP_ERROR IsCanSwapItem(EItemBoxType srcType,ushort srcIndex,EItemBoxType targetType,ushort targetIndex)
	{
		XItem srcItem		= GetItem((uint)srcType,(short)srcIndex);
		XItem targetItem	= GetItem ((uint)targetType,(short)targetIndex);
		
		if(srcType == targetType && srcIndex == targetIndex)
			return ESWAP_ERROR.ESWAP_ERROR_OTHER;

		if (srcItem.IsEmpty())
			return ESWAP_ERROR.ESWAP_ERROR_SRC_EMPTY;

		if (targetType == EItemBoxType.Equip)
		{
			//æäžä»¶ç©åæŸå°è£å€æ§œäœ			
			if (!srcItem.isEquip())
				return ESWAP_ERROR.ESWAP_ERROR_SRC_IS_NOT_EQUIP;

			//æ¯èŸæ§œäœ
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
			if (cfgItem == null)
				return ESWAP_ERROR.ESWAP_ERROR_SRC_EMPTY;
			
			ushort tempPos = (ushort)GetEquipPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);

			if (tempPos != targetIndex && (EQUIP_SLOT_TYPE)cfgItem.EquipPos != EQUIP_SLOT_TYPE.EQUIP_SLOT_FUYIN)
				return ESWAP_ERROR.ESWAP_ERROR_SLOT;
			
			if(!IsCanEquipByCareer(cfgItem.Index,m_Owner.DynGet(EShareAttr.esa_Class)))
				return ESWAP_ERROR.ESWAP_ERROR_CAREER;
			
			if(m_Owner.Level < cfgItem.ItemLevel)
				return ESWAP_ERROR.ESWAP_ERROR_LEVEL;
			
			if((EQUIP_SLOT_TYPE)cfgItem.EquipPos == EQUIP_SLOT_TYPE.EQUIP_SLOT_FUYIN)
			{				
				int fuyinIndex = targetIndex - (int)EQUIP_POS.EQUIP_POS_FUYIN_START;
				if(fuyinIndex >= 6)
					return ESWAP_ERROR.ESWAP_ERROR_CAREER;
				
				if(m_Owner.Level < GlobalU3dDefine.FuYinOpenLevel[fuyinIndex]) 
					return ESWAP_ERROR.ESWAP_ERROR_LEVEL;
			}
			
			return ESWAP_ERROR.ESWAP_ERROR_OK;
		}
		else if(targetType == EItemBoxType.PetEquip)
		{
			if (!srcItem.isEquip())
				return ESWAP_ERROR.ESWAP_ERROR_SRC_IS_NOT_EQUIP;

			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
			if (cfgItem == null)
				return ESWAP_ERROR.ESWAP_ERROR_SRC_EMPTY;
			
			int tempPos = GetEquipPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);
			
			int targetEquipPos = (int)targetIndex % (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
		
			if (tempPos != targetEquipPos && (EQUIP_SLOT_TYPE)cfgItem.EquipPos != EQUIP_SLOT_TYPE.EQUIP_SLOT_FUYIN)
				return ESWAP_ERROR.ESWAP_ERROR_SLOT;
			
			XCharacter	CurSelObj = XUIRoleInformation.CurRoInfoSelChar;
			XPet pet = CurSelObj as XPet;
			if(pet == null)
				return ESWAP_ERROR.ESWAP_ERROR_OTHER;
			
			XCfgPetBase cfgPet = XCfgPetBaseMgr.SP.GetConfig(pet.PetID);
			if(cfgPet == null)
				return ESWAP_ERROR.ESWAP_ERROR_OTHER;
	
			if(!IsCanEquipByCareer(cfgItem.Index,(int)cfgPet.ClassType2))
				return ESWAP_ERROR.ESWAP_ERROR_CAREER;
			
			if(pet.Level < cfgItem.ItemLevel)
				return ESWAP_ERROR.ESWAP_ERROR_LEVEL;
			
			if((EQUIP_SLOT_TYPE)cfgItem.EquipPos == EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION)
				return ESWAP_ERROR.ESWAP_ERROR_Pet;
			
			if((EQUIP_SLOT_TYPE)cfgItem.EquipPos == EQUIP_SLOT_TYPE.EQUIP_SLOT_FUYIN)
			{				
				int fuyinIndex = targetEquipPos - (int)EQUIP_POS.EQUIP_POS_FUYIN_START;
				if(fuyinIndex >= 6)
					return ESWAP_ERROR.ESWAP_ERROR_CAREER;
				
				if(pet.Level < GlobalU3dDefine.FuYinOpenLevel[fuyinIndex]) 
					return ESWAP_ERROR.ESWAP_ERROR_LEVEL;
			}

			return ESWAP_ERROR.ESWAP_ERROR_OK;
		}
		else if(targetType == EItemBoxType.Bank)
		{
			//ç®æ æ äœéç©º,äžåè®žçŽæ¥ææŸå°ä»åºäž­æç©åçæ äœäž
			if (srcType != EItemBoxType.Bank)
				return ESWAP_ERROR.ESWAP_ERROR_OTHER;
			
			return ESWAP_ERROR.ESWAP_ERROR_OK;
		}
		else if(targetType == EItemBoxType.Bag)
		{
			if (targetItem.IsEmpty())
				return ESWAP_ERROR.ESWAP_ERROR_OK;

			//èŠå€æ­èåäž­çç©åèœåŠæŸå°æºæ äœäž			
			if(srcType == EItemBoxType.Equip)
			{
				//èåäž­ç©åæ¯åŠäžºåéšäœçè£å€
				if(!targetItem.isEquip())
					return ESWAP_ERROR.ESWAP_ERROR_SRC_IS_NOT_EQUIP;
				
							//æ¯èŸæ§œäœ
				XCfgItem srcCfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
				if (srcCfgItem == null)
					return ESWAP_ERROR.ESWAP_ERROR_SRC_EMPTY;
				
				XCfgItem targetCfgItem = XCfgItemMgr.SP.GetConfig(targetItem.DataID);
				if(targetCfgItem == null)
					return ESWAP_ERROR.ESWAP_ERROR_SRC_EMPTY;
				
				if(srcCfgItem.EquipPos != targetCfgItem.EquipPos)
					return ESWAP_ERROR.ESWAP_ERROR_SLOT;		
			}

			return ESWAP_ERROR.ESWAP_ERROR_OK;
		}

		return ESWAP_ERROR.ESWAP_ERROR_OTHER;
	}

	public bool SwapItem(EItemBoxType srcType, ushort srcIndex, EItemBoxType targetType, ushort targetIndex)
	{
		ESWAP_ERROR error = IsCanSwapItem(srcType, srcIndex, targetType, targetIndex);
		if (error != ESWAP_ERROR.ESWAP_ERROR_OK)
		{
			if(error == ESWAP_ERROR.ESWAP_ERROR_CAREER)
			{
				XItem temp	= GetItem((uint)srcType,(short)srcIndex);
				if(temp != null)
				{					
					XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,13,GetItemCareerName(temp.DataID));
				}				
			}
			else if(error == ESWAP_ERROR.ESWAP_ERROR_Pet)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,10055);
			}
			else if(error == ESWAP_ERROR.ESWAP_ERROR_LEVEL)
			{
				XItem temp	= GetItem((uint)srcType,(short)srcIndex);
				XCfgItem cfg = XCfgItemMgr.SP.GetConfig(temp.DataID);
				if(cfg == null)
					return false;
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,33,cfg.ItemLevel);
			}
			return false;
		}

		//çæ­£çäº€æ¢ç©åæ°æ		
		XItem srcItem = GetItem((uint)srcType, (short)srcIndex);
		XItem tempSrcItem	= new XItem();
		tempSrcItem.CopyData(srcItem);
		XItem targetItem = GetItem((uint)targetType,(short)targetIndex);
		
		if(!srcItem.IsEmpty() && !targetItem.IsEmpty() && IsCanPile(srcItem.ItemIndex,targetItem.ItemIndex,srcItem.ItemCount))
		{
			targetItem.ItemCount += srcItem.ItemCount;
			srcItem.Reset();
		}
		else
		{
			SetItem(srcType, srcIndex,targetItem);
			SetItem(targetType, targetIndex, tempSrcItem);
		}
		
		//å·æ°çé¢
		if (srcType == EItemBoxType.Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow,srcType,srcIndex);
		else if (srcType == EItemBoxType.Equip || srcType == EItemBoxType.PetEquip)
			XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation,srcType,srcIndex);

		if (targetType == EItemBoxType.Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow,targetType,targetIndex);
		else if (targetType == EItemBoxType.Equip || targetType == EItemBoxType.PetEquip)
			XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation,targetType,targetIndex);
		
		XEventManager.SP.SendEvent(EEvent.Bag_UpdateNum,EUIPanel.eBagWindow);
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateUI);

		return true;
	}    

    public XItem GetItem(uint boxType, short pos)
    {
		if(!IsPosValid((EItemBoxType)boxType,pos))
			return null;
		
        if (boxType >= (uint)EItemBoxType.Begin && boxType < (uint)EItemBoxType.End)
        {
            ushort realIndex = GetBeginIndex((EItemBoxType)boxType);
			return AllItemBox[realIndex + pos];
        }
        return null;
    }
	
	public XItem GetItem(uint pos)
	{
		if(pos >= 0 && pos < (uint)EBAG_DATA_POS.ROLE_MAX_ITEM_NUM)
		{
			return AllItemBox[pos];
		}
		
		return null;
	}
	
	public bool IsHasKey(uint keyLevel)
	{
		//åªåšèåäž­æçŽ		
		ushort startIndex = GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex		= GetEndIndex(EItemBoxType.Bag);
		
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			XItem item = GetItem(i);
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
			if(cfgItem == null)
				continue;
			
			if(cfgItem.UnLockLevel >= keyLevel)
				return true;
		}
		
		return false;
	}
	
	public XItem GetItemByGUID(UInt64 GUID)
	{
		//åªåšèåäž­æçŽ		
		ushort startIndex = GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex		= GetEndIndex(EItemBoxType.Equip);
		
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			XItem item = GetItem(i);
			if(!item.IsEmpty() && item.GUID == GUID)
				return item;
		}
		
		startIndex = GetBeginIndex(EItemBoxType.PetEquip);
		endIndex		= GetEndIndex(EItemBoxType.PetEquip);
		
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			XItem item = GetItem(i);
			if(!item.IsEmpty() && item.GUID == GUID)
				return item;
		}
		
		return null;
	}
	
	//åªåšèåäž­æçŽ	
	public int GetItemByDataID(uint itemID)
	{
		//åªåšèåäž­æçŽ		
		ushort startIndex = GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex		= GetEndIndex(EItemBoxType.Bag);
		
		int leftNum = 0;
		
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			XItem item = GetItem(i);
			if(!item.IsEmpty() && item.DataID == itemID)
			{
				leftNum += item.ItemCount;				
			}
		}
		
		return leftNum;
	}
	
	public bool IsPosValid(EItemBoxType type,int pos)
	{
		if(EItemBoxType.Bag == type)
		{
			return pos >= 0 && pos < (int)EBAG_DATA.ROLE_MAX_COM_BAG_ITEM_NUM;
		}
		else if(EItemBoxType.Equip == type)
		{
			return pos >= 0 && pos < (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
		}
		else if(EItemBoxType.Bank == type)
		{
			return pos >= 0 && pos < (int)EBAG_DATA.ROLE_BANK_ITEM_NUM;
		}
		else if(EItemBoxType.PetEquip == type)
		{
			return pos >= 0 && pos < (int)EBAG_DATA.ROLE_MAX_PET_EQUIP_NUM;
		}
		return false;
	}
	
	public bool IsCanStrengthen(UInt64 guid)
	{
		XItem item = GetItemByGUID(guid);
		if(item == null || !item.isEquip())
			return false;
		
		if(!item.IsCanStrengthen())
			return false;
		
		//ææïŒéé		
		XCfgStrengthen cfg = GetStrengthenCfg((uint)item.Color,(byte)item.mEquipAttr.mStrengthenLevel);
		if(cfg == null)
			return false;
		
		for(int i = 0; i < 2;i++)
		{
			int num = GetItemByDataID(cfg.MaterialID[i]);
			if(num < cfg.MaterialNum[i])
				return false;
		}
		
		if(m_Owner == null)
			return false;
		
		XMainPlayer player = m_Owner as XMainPlayer;
		if(player == null)
			return false;
		
		if(player.GameMoney < cfg.GameMoney)
			return false;
		
		return true;
	}

	public void SetItem(EItemBoxType type, ushort index,XItem item)
	{
		XItem srcItem = GetItem((uint)type, (short)index);
		if(srcItem == null || item == null)
			return ;		
		EquipEffect(srcItem, EQUIP_OP.EQUIP_OP_SUB);
		srcItem.CopyData(item);
		//æ¢å€çŽ¢åŒ
		srcItem.ItemIndex = XItemManager.GetItemIndex(type, index);
		EquipEffect(item, EQUIP_OP.EQUIP_OP_ADD);
		
		if(type == EItemBoxType.Equip)
		{			
			ChangeEquip(srcItem);
		}
	}

	private void EquipEffect(XItem item, EQUIP_OP op)
	{
		if (item.ItemIndex < XItemManager.GetBeginIndex(EItemBoxType.Equip) || item.ItemIndex > XItemManager.GetEndIndex(EItemBoxType.Equip))
			return;

		//è£å€å±æ§æ·»å ææ	
	}
	
	private void ChangeEquip(XItem Item)
	{	
		if (Item.ItemIndex == GetRealItemIndex(m_Owner,GetEquipPos(EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR)))
		{
			m_Owner.ArmourItemID = Item.IsEmpty() ? 0 : Item.DataID;
		}
		if (Item.ItemIndex == GetRealItemIndex(m_Owner,GetEquipPos(EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON)))
		{	
			XPlayer player = m_Owner as XPlayer;
			if(player != null)
				player.WeaponItemID = Item.IsEmpty() ? 0 : Item.DataID;
		}
		
		if ( Item.ItemIndex == GetRealItemIndex(m_Owner,GetEquipPos(EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION)))
		{
			m_Owner.FashionId = Item.DataID;
		}
	}
	
	public XItem GetAnyoneEquipByPos(EQUIP_SLOT_TYPE slotType)
	{
		int equipPos  = GetEquipPos(slotType);
		int realIndex = GetRealItemIndex(m_Owner,equipPos);
		XItem item = GetItem((uint)realIndex);
		if(!item.IsEmpty())
			return item;
		
		XMainPlayer mainPlayer = m_Owner as XMainPlayer;
		if(mainPlayer == null)
			return null;
		
		for(int i = 0; i < XLogicWorld.SP.PetManager.AllPet.Length; i++)
		{
			XPet pet = XLogicWorld.SP.PetManager.AllPet[i];
			if(pet == null)
				continue;
			int temp = GetRealItemIndex(pet,equipPos);
			item = GetItem((uint)temp);
			if(!item.IsEmpty())
				return item;			
		}
		
		return null;
	}

	//æ ¹æ®è£å€éšäœåŸå°èåäž­çæ äœ
	public int GetEquipPos(EQUIP_SLOT_TYPE slotType)
	{
		int final = (int)EQUIP_POS.EQUIP_POS_INVALID;
		switch(slotType)
		{
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON://æ­Šåš
			final = (int)EQUIP_POS.EQUIP_POS_WEAPON;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR://é ç²
			final =  (int)EQUIP_POS.EQUIP_POS_ARMOUR;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_AMULET://æ€ç¬Š
			final =  (int)EQUIP_POS.EQUIP_POS_AMULET;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_RING://ææ
			final =  (int)EQUIP_POS.EQUIP_POS_RING;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_MAGIC_WEAPON://æ³å®
			final =  (int)EQUIP_POS.EQUIP_POS_MAGIC_WEAPON;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_BOARD://ä»€ç
			final =  (int)EQUIP_POS.EQUIP_POS_BOARD;
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOT_FUYIN://ç¬Šå°
			{
				//è·åäžäžªç©ºçç¬Šå°äœçœ				
				ushort equipStartPos = GetBeginIndex(EItemBoxType.Equip);
				for(int i = 0;i < (int)EQUIP_POS.EQUIP_POS_FUYIN_NUM;i++)
				{
					int pos = (int)EQUIP_POS.EQUIP_POS_FUYIN_START + i;
					int realPos = (int)equipStartPos + pos;
					if(AllItemBox[realPos].IsEmpty())
					{
						final = pos;
						break;
					}
				}
			}
			break;
		case EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION:
			final = (int)EQUIP_POS.EQUIP_POS_FASHION;
			break;
		}		
		//final += (int)EBAG_DATA_POS.ROLE_EQUIP_START;
		return final;
	}    
	
	public short GetEmptyPosNoPile(EItemBoxType posType )
	{
		ushort index = 0;
		ushort startIndex = 0;	
		ushort endIndex	= 0;
	
		startIndex  = GetBeginIndex(posType);
		endIndex	= GetEndIndex(posType);
	
		for(index = startIndex; index <= endIndex;index++)
		{
			if (AllItemBox[index].IsEmpty())
			{
				if(m_SpaceMgr != null && m_SpaceMgr.IsSet((short)index))
					return (short)(index - startIndex);
				else if(m_SpaceMgr == null)
					return (short)(index - startIndex);
			}
				
		}
	
		return -1;
	}
	
	public void DelItem(UInt16 itemIndex,uint num)
	{
		XItem item = GetItem(itemIndex);
		if(item == null)
			return ;
		
		if(item.ItemCount < num)
			return ;
		
		item.ItemCount = (ushort)(item.ItemCount - num);
		
		EItemBoxType Box;
		ushort index;
		XItemManager.GetContainerType(itemIndex,out Box,out index);
		if(Box == EItemBoxType.Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,index);
		else if(Box == EItemBoxType.Equip)
			XEventManager.SP.SendEvent(EEvent.Bank_ItemUpdate,index);
	}
	
	public void DelItem(UInt16 itemIndex)
	{
		XItem item = GetItem(itemIndex);
		if(item == null)
			return ;
		
		item.ItemCount = 0;
		
		EItemBoxType Box;
		ushort index;
		XItemManager.GetContainerType(itemIndex,out Box,out index);
		if(Box == EItemBoxType.Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,index);
		else if(Box == EItemBoxType.Equip)
			XEventManager.SP.SendEvent(EEvent.Bank_ItemUpdate,index);
	}
	
	public XItem AddItem(UInt16 itemIndex,uint itemData,ushort itemNum)
	{
		if(!AllItemBox[itemIndex].IsEmpty() && AllItemBox[itemIndex].DataID != itemData)
		{
			Log.Write(LogLevel.ERROR,"addItem error");
			return null;
		}
		
		XItem item = null;
		if(AllItemBox[itemIndex].IsEmpty())
		{
			item = new XItem();
			item.DataID = itemData;
			item.ItemIndex	= itemIndex;
			AllItemBox[itemIndex]	= item;
			
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemData);
			if(cfgItem != null && cfgItem.PileAmount >= itemNum)
			{
				item.ItemCount	= itemNum;
			}
			else
				item.ItemCount	= 1;
		}
		else
		{
			AllItemBox[itemIndex].ItemCount	+= itemNum;
		}

		//éç¥UIæŽæ°
		EItemBoxType Box;
		ushort index;
		XItemManager.GetContainerType(itemIndex,out Box,out index);
		if(Box == EItemBoxType.Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,index);
		else if(Box == EItemBoxType.Equip)
			XEventManager.SP.SendEvent(EEvent.Bank_ItemUpdate,index);
		
		return item;
	}
	
	public void GetAllEquip(EItemBoxType type,List<UInt64> equipList)
	{		
		ushort startIndex = GetBeginIndex(type);
		ushort endIndex	 = GetEndIndex(type);
		for(ushort i = startIndex;i <= endIndex; i++)
		{
			XItem item = GetItem((uint)i);
			if(!item.IsEmpty() && item.isEquip())
			{
				equipList.Add(item.GUID);
			}
		}
	}
	
	public ushort GetEmptyPosNum(EItemBoxType type)
	{
		ushort begin = GetBeginIndex(type);
		ushort end	 = GetEndIndex(type);
		
		ushort count = 0;
		for(ushort i = begin ; i <= end; i++)
		{
			XItem item = GetItem(i);
			if(item != null && item.IsEmpty())
			{
				if(m_SpaceMgr != null && m_SpaceMgr.IsSet((short)i))
					count++;
				else if(m_SpaceMgr == null)
					count++;			
			}
				
		}
		
		return count;
	}
	
	public uint GetCurItemSpaceCount(EItemBoxType type)
	{
		ushort startIndex = GetBeginIndex(type);
		ushort endIndex		= GetEndIndex(type);
		
		uint count = 0;
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			XItem srcItem = GetItem((uint)i);
			if(srcItem.IsEmpty())
				continue;
			count++;
		}
		
		return count;
	}
	
	public int GetFullSpaceCount(EItemBoxType type)
	{
		if(m_SpaceMgr == null)
			return 0;
		
		return m_SpaceMgr.GetSpace(type);
	}
	
	public bool IsCanPile(ushort srcItemIndex,ushort targetItemIndex,ushort itemCount)
	{
		XItem srcItem = GetItem(srcItemIndex);
		if(srcItem.IsEmpty())
			return false;
		
		XItem targetItem = GetItem(targetItemIndex);
		if(targetItem.IsEmpty())
			return true;
		
		if(targetItem.DataID == srcItem.DataID)
		{
			int maxPileCount = targetItem.MaxPileCount;
			if(targetItem.ItemCount + itemCount > maxPileCount)
				return false;
			
			return true;			
		}
		
		return false;
	}
	
	#endregion
	
	#region static Function	
	
	public static int GetRealItemIndex(XCharacter obj,int equipPos)
	{
		int final = equipPos;
		if(obj == null)
			return final;	
		XPet pet = obj as XPet;
		if(pet == null)
		{
			final += (int)EBAG_DATA_POS.ROLE_EQUIP_START;
		}
		else
		{
			final += (int)(EBAG_DATA_POS.ROLE_PET_EQUIP_START + (int)(pet.Index - 1) * (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM);
		}
		return final;
	}
	public static uint GetArmourID(uint armourGroupID,int appearLevel,int sex,int job)
	{
		if(appearLevel < 1 || appearLevel > 5)
			return 0;
		List<XCfgArmourGroup> list = XCfgArmourGroupMgr.SP.ItemTable;
		for(int i = 0; i < list.Count;i++)
		{
			XCfgArmourGroup temp = list[i];
			if(temp.EquipGroupID == armourGroupID && temp.Sex == sex && temp.JobID == job)
				return temp.LevelID[appearLevel-1];
		}
		
		return 0;
	}
	
	public static ushort GetBeginIndex(EItemBoxType type)
	{
		ushort ret = 0;
		switch(type)
		{
		case EItemBoxType.Bag:
			ret = (ushort)EBAG_DATA_POS.ROLE_COM_BAG_ITEM_START;
			break;
		case EItemBoxType.Equip:
			ret =  (ushort)EBAG_DATA_POS.ROLE_EQUIP_START;
			break;
		case EItemBoxType.Bank:
			ret =  (ushort)EBAG_DATA_POS.ROLE_BANK_ITEM_START;
			break;
		case EItemBoxType.PetEquip:
			ret =  (ushort)EBAG_DATA_POS.ROLE_PET_EQUIP_START;
			break;
		}
		
		return ret;
	}
	
	public static ushort GetEndIndex(EItemBoxType type)
	{
		ushort ret = 0;
		switch(type)
		{
		case EItemBoxType.Bag:
			ret =  (ushort)EBAG_DATA_POS.ROLE_COM_BAG_ITEM_END;
			break;
		case EItemBoxType.Equip:
			ret =  (ushort)EBAG_DATA_POS.ROLE_EQUIP_END;
			break;
		case EItemBoxType.Bank:
			ret =  (ushort)EBAG_DATA_POS.ROLE_BANK_ITEM_END;
			break;
		case EItemBoxType.PetEquip:
			ret =  (ushort)EBAG_DATA_POS.ROLE_PET_EQUIP_END;
			break;
		}
		
		return ret;
	}
	
	public static void GetContainerType(ushort realIndex,out EItemBoxType type,out ushort index)
	{
		if (realIndex >= GetBeginIndex(EItemBoxType.Bag) && realIndex <= GetEndIndex(EItemBoxType.Bag))
		{
			type	= EItemBoxType.Bag;
			index	= (ushort)(realIndex - GetBeginIndex(EItemBoxType.Bag));
			return;
		}
	
		if (realIndex >= GetBeginIndex(EItemBoxType.Equip) && realIndex <= GetEndIndex(EItemBoxType.Equip))
		{
			type	= EItemBoxType.Equip;
			index	= (ushort)(realIndex - GetBeginIndex(EItemBoxType.Equip));
			return;
		}
	
		if (realIndex >= GetBeginIndex(EItemBoxType.Bank) && realIndex <= GetEndIndex(EItemBoxType.Bank))
		{
			type	= EItemBoxType.Bank;
			index	= (ushort)(realIndex - GetBeginIndex(EItemBoxType.Bank));
			return;
		}
		
		if (realIndex >= GetBeginIndex(EItemBoxType.PetEquip) && realIndex <= GetEndIndex(EItemBoxType.PetEquip))
		{
			type	= EItemBoxType.PetEquip;
			index	= (ushort)(realIndex - GetBeginIndex(EItemBoxType.PetEquip));
			return;
		}

		type 	= EItemBoxType.Bag;
		index 	= 0;
	}

	public static ushort GetItemIndex(EItemBoxType type, ushort index)
	{
		ushort ret = 0;
		switch (type)
		{
			case EItemBoxType.Bag:
				ret = (ushort)(EBAG_DATA_POS.ROLE_COM_BAG_ITEM_START + index);
				break;
			case EItemBoxType.Equip:
				ret = (ushort)(EBAG_DATA_POS.ROLE_EQUIP_START + index);
				break;
			case EItemBoxType.Bank:
				ret = (ushort)(EBAG_DATA_POS.ROLE_BANK_ITEM_START + index);
				break;
			case EItemBoxType.PetEquip:
				ret = (ushort)(EBAG_DATA_POS.ROLE_PET_EQUIP_START + index);
				break;
		}

		return ret;
	}
	
	public static uint GetClothesModelID(uint Job,uint sex,uint dataID,uint strengthenLevel,uint color)
	{
		uint ModelId = 0;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(dataID);
		
		if(cfgItem != null)
		{
			int appearLevel = 0;
			XCfgStrengthen cfgStrengthen = GetStrengthenCfg(color,(byte)strengthenLevel);
			if(cfgStrengthen != null)
				appearLevel	= cfgStrengthen.AppearLevel;
				
			uint tempModelID = XItemManager.GetArmourID(cfgItem.ArmourID,appearLevel,(int)sex,(int)Job);
			if(tempModelID != 0)
				ModelId	= tempModelID;
		}
		else
		{
			uint md = 0;		
			XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)Job);
			if(playerBase != null)
			{
                if (sex == (uint)EShareSex.eshSex_Male)
                {
                    md = playerBase.MaleModel;
                }
                else if (sex == (uint)EShareSex.eshSex_Female)
                {
                    md = playerBase.FemaleModel;
                }
			}
            if (md > 0)
            {
                ModelId = md;
            }
		}		
		
		return ModelId;
		
	}
	
	public static uint GetWeaponModelID(uint Job,uint sex,uint dataID,uint strengthenLevel,uint color)
	{
		uint WeaponModelId = 0;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(dataID);
		if(cfgItem != null)
		{
			int appearLevel = 0;
			XCfgStrengthen cfgStrengthen = GetStrengthenCfg(color,(byte)strengthenLevel);
			if(cfgStrengthen != null)
				appearLevel	= cfgStrengthen.AppearLevel;
			//--------------------------------------------------
			uint tempModelID = XItemManager.GetArmourID(cfgItem.ModelId,appearLevel,(int)sex,(int)Job);
			WeaponModelId = tempModelID;
		}
		else
		{
			XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)Job);
			if(playerBase != null)
			{
				WeaponModelId	= playerBase.DefaultWeapon;
			} 
		}
		
		return WeaponModelId;
	}
	
	public static bool IsCanEquipByCareer(uint itemID,int CareerID)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID);
		if(cfgItem == null)
			return false;
		
		if(cfgItem.CareerRequire == 0)
			return true;
		
		if((cfgItem.CareerRequire & ( 1 << CareerID - 1)) > 0)
			return true;
		
		return false;
	}
	
	public static string GetItemCareerName(uint itemID)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID);
		if(cfgItem == null)
			return "";
		
		for(int career = (int)EShareClass.eshClass_ZhanShi; career <= (int)EShareClass.eshClass_GongJianShou; career++)
		{
			if((cfgItem.CareerRequire & ( 1 << career - 1)) > 0)
			{
				return XStringManager.SP.GetString((uint)(252 + career - 1));
			}
		}
		
		return "";
	}
	
	public static void FlyItemAnimation(XItem item)
	{
		uint UIKey = XUIManager.SP.GetMuliUIObjectKey(EUIPanel.eFlyItem);
		XUTFlyItem flyItemUI = XUIManager.SP.GetMuliUIObject(UIKey) as XUTFlyItem;
		//XEventManager.SP.SendEvent(EEvent.FlyItem_NewItem,item);
		flyItemUI.FlyItemHandler(EEvent.FlyItem_NewItem,item);
		XEventManager.SP.SendEvent(EEvent.UI_MuliShow,UIKey);
	}
	
	public static void CharSwapItem(EItemBoxType srcType, ushort srcIndex, EItemBoxType targetType, ushort targetIndex)
	{
		//if(XUIRoleInformation.CurSelIndex <= 0)
		//宠物的身上装备也是 在主角的物品管理其里面
		if(XLogicWorld.SP.MainPlayer.ItemManager.SwapItem(srcType,srcIndex,targetType,targetIndex) == false)
			return;
		
		ushort realSrcIndex = XItemManager.GetItemIndex(srcType,srcIndex);
		ushort realTargetIndex = XItemManager.GetItemIndex(targetType, targetIndex);

		//通知服务器，如果服务器判断不能交换，会发送错误包返回，在接受的时候，再换回来
		CS_SwapItem.Builder builder = CS_SwapItem.CreateBuilder();
		builder.SetPosition0(realSrcIndex);
		builder.SetPosition1(realTargetIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SwapItem, builder.Build());
	}
	
    #endregion
    //ç©ååºæ¬å±æ§ç¹æ§å€æ	 æ¯åŠå¯å
	#region 
	public bool CanSellItem(uint boxType, short pos)
	{
		 XItem pItem = GetItem( boxType,  pos);
		 if(pItem == null) return false;
		 
		 return pItem.CanSell();
	}
	
	public static XCfgStrengthen GetStrengthenCfg(uint colorLevel,byte strengthenLevel)
	{
		XCfgStrengthen cfgStrengthen = null;
		foreach(XCfgStrengthen cfgStr in XCfgStrengthenMgr.SP.ItemTable)
		{
			if(cfgStr.EquipColorLevel == colorLevel && cfgStr.StrengthenLevel == strengthenLevel)
			{
				cfgStrengthen = cfgStr;
			}
		}
		
		return cfgStrengthen;
	}
	
	public static XCfgStrengthen GetStrengthenCfg(uint colorLevel,byte strengthenLevel,uint equipLevel)
	{
		XCfgStrengthen cfgStrengthen = null;
		foreach(XCfgStrengthen cfgStr in XCfgStrengthenMgr.SP.ItemTable)
		{
			if(cfgStr.EquipColorLevel == colorLevel && cfgStr.StrengthenLevel == strengthenLevel && cfgStr.EquipLevel == equipLevel)				
			{
				cfgStrengthen = cfgStr;
			}
		}
		
		return cfgStrengthen;
	}
	
	public static void UseItem(XItem item)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		XUIPopMenu uiPopMenu = XUIManager.SP.GetUIControl(EUIPanel.ePopMenu) as XUIPopMenu;
		if(uiPopMenu == null)
			return ;
		
//		EItemBoxType type;
//		ushort index = 0;
//		GetContainerType(item.ItemIndex,type,index);
		uiPopMenu.OnPopMenuEquip(EEvent.PopMenu_Equip,null);
		
		
	}
	
	#endregion
	
}
