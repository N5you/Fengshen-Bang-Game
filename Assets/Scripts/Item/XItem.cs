using System;
using XGame.Client.Packets;
using System.Collections.Generic;

public class XItem
{
	public class EquipAttr
	{
		public UInt32[]			mGemID	= new UInt32[(int)EItem_Equip.MAX_INLAY_GEM_NUM];
		public UInt16			mStrengthenLevel = 1;
		public EItem_Quality	mColor = EItem_Quality.EITEM_QUALITY_WHITE;
		public UInt16[]			mAttrID	= new UInt16[(int)EItem_Equip.MAX_RANDOM_ATTR_NUM];
		public UInt32[]			mAttrValue	= new UInt32[(int)EItem_Equip.MAX_RANDOM_ATTR_NUM];
		public bool				mIsSeal;
	}
	
	public static readonly int INVALID_ITEM_ID = 0;
    public UInt64 GUID { get; set; }
	public UInt32 DataID {get; set; }
    public UInt16 ItemIndex { get; set; }
    public UInt32 IsBinding { get; set; }
    public UInt16 ItemCount {
		get
		{
			return mItemCount;
		}
		set
		{ 
			UInt32 DataId = DataID;
			if(0 == value)
			{				
				Reset();
			}
			else
				mItemCount	= value;			
			XEventManager.SP.SendEvent(EEvent.Bag_ItemNumChanged, DataId);
			
			EItemBoxType Box;
			ushort index;
			XItemManager.GetContainerType(ItemIndex,out Box,out index);
			if(Box == EItemBoxType.Bag)
				XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,index);
			else if(Box == EItemBoxType.Equip)
				XEventManager.SP.SendEvent(EEvent.Bank_ItemUpdate,index);
		}
	}	
	
	public EItem_Quality	Color{
		get
		{
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
			if (cfgItem == null)
				return EItem_Quality.EITEM_QUALITY_WHITE;
			
			EItem_Quality color;
			if(cfgItem.IsRandom > 0)
				color	= (EItem_Quality)mEquipAttr.mColor;
			else
				color	= (EItem_Quality)cfgItem.QualityLevel;
			
			return color;
		}
	}
	
	public bool	IsSeal
	{
		get
		{
			return mEquipAttr.mIsSeal;
		}
		set
		{
			if(mEquipAttr.mIsSeal != value)
			{
				mEquipAttr.mIsSeal	= value;
			}
		}
	}
	
	public ushort MaxPileCount
	{
		get
		{
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
			return cfgItem.PileAmount;
		}
	}
	
	
	private UInt16 mItemCount;
	public EquipAttr mEquipAttr = new EquipAttr();
	
	public XItem()
	{
		GUID 		= 0;
		DataID 		= 0;
		ItemIndex 	= 0;
		IsBinding 	= 0;
		mItemCount 	= 0;
	}
	
	public XItem(uint dataID)
	{
		GUID 		= 0;
		DataID 		= dataID;
		ItemIndex 	= 0;
		IsBinding 	= 0;
		mItemCount 	= 1;
		mEquipAttr.mStrengthenLevel = 1;
	}
	
	public void Reset()
	{
		GUID 		= 0;
		DataID 		= 0;		
		IsBinding 	= 0;
		mItemCount 	= 0;
	}
	
	public bool IsEmpty()
	{
		return DataID ==  INVALID_ITEM_ID;
	}
	
	public bool isEquip()
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if (cfgItem == null)
			return false;

		if (cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_WEAPON || 
			cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR ||
			cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_FUYIN)
			return true;

		return false;
	}
	
	public ushort GetBaseAttrID()
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return 0;
		
		return (ushort)cfgItem.BaseAttrType;
	}
	
	public int GetBaseAttrValue()
	{
		return GetBaseAttrValueByStrengthenLevel((byte)mEquipAttr.mStrengthenLevel);
	}
	
	public int	GetBaseAttrValueByStrengthenLevel(byte strengthenLevel)
	{
		//fix Color
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return 0;
		
		XCfgStrengthen scfg = XItemManager.GetStrengthenCfg((uint)mEquipAttr.mColor,strengthenLevel);
		if(scfg == null)
			return 0;
		
		int curValue = (int)(cfgItem.BaseAttrValue * scfg.AttrRate);
		if(cfgItem.IsRandom == 0)
		{			
			return curValue;
		}
		else
		{
			XCfgColorValue cfgColorValue = XCfgColorValueMgr.SP.GetConfig((uint)mEquipAttr.mColor);
			if(cfgColorValue != null)
			{
				curValue	= (int)(curValue * cfgColorValue.BaseRate);
			}
			
			return curValue;
		}
		
		return curValue;
	}
	
	public ushort GetMagicAttrID(uint index)
	{
		if(IsHasDynamicMagicAttr())
		{
			return mEquipAttr.mAttrID[index];
		}
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return 0;
		
		return cfgItem.MagicAttrType[index];
	}
	
	public int GetMagicAttrValue(uint index)
	{
		return GetMagicAttrValueByStrengthenLevel(index,(byte)mEquipAttr.mStrengthenLevel);
	}
	
	public int GetMagicAttrValueByStrengthenLevel(uint index,byte strengthenLevel)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return 0;
		
		XCfgStrengthen scfg = XItemManager.GetStrengthenCfg((uint)mEquipAttr.mColor,strengthenLevel);
		if(scfg == null)
			return 0;
		
		if(cfgItem.IsRandom == 0)
		{
			if(!IsHasDynamicMagicAttr())
			{
				if(cfgItem.MagicAttrType[index] != 0)
				{
					return (int)(cfgItem.MagicAttrValue[index] * scfg.AttrRate);
				}
			}
			else
			{
				return (int)(mEquipAttr.mAttrValue[index]* scfg.AttrRate);
			}
		}
		else
		{
			if(!IsHasDynamicMagicAttr())
			{
				if(cfgItem.MagicAttrType[index] != 0)
				{
					return (int)(cfgItem.MagicAttrValue[index] * scfg.AttrRate);
				}
			}
			else
			{
				XCfgColorValue cfgColorValue = XCfgColorValueMgr.SP.GetConfig((uint)mEquipAttr.mColor);
				int retValue = 0;
				if(cfgColorValue != null)
					retValue =  (int)(mEquipAttr.mAttrValue[index]* cfgColorValue.MagicRate * scfg.AttrRate);
				
				return retValue;
			}
		}
		
		return 0;
	}
	
	public bool IsCanUse(int dutyID,int level)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if (cfgItem == null)
			return false;
		
		return true;
	}
	
	public XItem Clone()
	{
   		return (XItem)base.MemberwiseClone();
	}
	
	public bool IsInlayGem(uint gemid)
	{
		XCfgItem orign = XCfgItemMgr.SP.GetConfig(gemid);
		if(orign == null)
			return false;
		
		XCfgStrengthen cfgStrengthen = XItemManager.GetStrengthenCfg((uint)mEquipAttr.mColor,(byte)mEquipAttr.mStrengthenLevel);
		if(cfgStrengthen == null)
			return false;
		
		if(cfgStrengthen.SlotNum <= GetGemNum())
			return false;
		
		for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM; i++)
		{
			if(mEquipAttr.mGemID[i] > 0)
			{
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(mEquipAttr.mGemID[i]);
				if(cfgItem == null)
					continue ;
				
				if(cfgItem.GemType == orign.GemType)
					return false;
			}
		}
		
		return true;
	}
	
	public void AddGem(byte index,uint gemID)
	{
//		for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM; i++)
//		{
//			if(mEquipAttr.mGemID[i] == 0)
//			{
//				mEquipAttr.mGemID[i] = gemID;
//				return ;
//			}
//		}
		if(index >= (byte)EItem_Equip.MAX_INLAY_GEM_NUM)
			return ;
		
		mEquipAttr.mGemID[index] = gemID;
	}
	
	public byte GetGemNum()
	{
		if(!isEquip())
			return 0;
		
		byte num = 0;
		for(int i =0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM;i++)
		{
			if(mEquipAttr.mGemID[i] > 0)
				num++;
		}
		
		return num;
	}
	
	public uint GetGemID(int index)
	{
		if(!isEquip() || index >= (int)EItem_Equip.MAX_INLAY_GEM_NUM)
			return 0;
		
		return mEquipAttr.mGemID[index];		
	}
	
	public void InitFromPB(PB_ItemInfo info)
	{
		GUID		= info.ItemGUID;
		DataID		= info.ItemId;
		ItemIndex	= (UInt16)info.Position;
		IsBinding   = info.IsBinding;
		ItemCount   = (UInt16)info.ItemCount;	
		
		if(info.HasAttr)
		{
			for(int i = 0; i < info.Attr.GemIDCount;i++)
			{
				mEquipAttr.mGemID[i]	= info.Attr.GemIDList[i];
			}
			
			mEquipAttr.mStrengthenLevel	= (UInt16)info.Attr.StrengthenLevel;
			mEquipAttr.mColor			= (EItem_Quality)info.Attr.Color;
			mEquipAttr.mIsSeal			= info.Attr.IsSeal > 0 ? true : false;
			for(int i = 0; i < info.Attr.AttrArrayCount;i++)
			{
				mEquipAttr.mAttrID[i]	= (ushort)info.Attr.AttrArrayList[i].AttrID;
				mEquipAttr.mAttrValue[i]= (ushort)info.Attr.AttrArrayList[i].Value;
			}			
		}
	}  
	
	public void CopyData(XItem srcItem)
	{
		GUID = srcItem.GUID;
		DataID	= srcItem.DataID;
		ItemIndex = srcItem.ItemIndex;
		IsBinding = srcItem.IsBinding;
		ItemCount = srcItem.ItemCount;
		
		for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM;i++)
		{
			mEquipAttr.mGemID[i]	= srcItem.mEquipAttr.mGemID[i];
		}
		
		mEquipAttr.mStrengthenLevel	= srcItem.mEquipAttr.mStrengthenLevel;
		mEquipAttr.mColor			= srcItem.mEquipAttr.mColor;
		mEquipAttr.mIsSeal			= srcItem.mEquipAttr.mIsSeal;
		for(int i = 0; i < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
		{
			mEquipAttr.mAttrID[i]	= srcItem.mEquipAttr.mAttrID[i];
			mEquipAttr.mAttrValue[i]= srcItem.mEquipAttr.mAttrValue[i];
		}
	}
	
	public bool IsHasDynamicMagicAttr()
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return false;
		
		return cfgItem.IsRandomAttr > 0;
	}
	
	
	public bool IsCanStrengthen()
	{
		if(!isEquip())
			return false;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return false;
		
		if(cfgItem.IsCanStrengthen <= 0)
			return false;
		
		XCfgStrengthen cfg = XItemManager.GetStrengthenCfg((uint)Color,(byte)mEquipAttr.mStrengthenLevel);
		if(cfg == null)
			return false;
		
		return cfg.IsCanStrengthen > 0;
	}
	
	public bool IsCanDecompose()
	{
		if(!isEquip())
			return false;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null)
			return false;
		
		if(cfgItem.IsCanExplode == 0)
			return false;	
		
		byte ColorLevel = 1;
		if(cfgItem.IsRandom > 0)
			ColorLevel	= (byte)mEquipAttr.mColor;
		else
			ColorLevel	= cfgItem.QualityLevel;
		
		List<XCfgDecompose> list = XCfgDecomposeMgr.SP.ItemTable;
		for(int i = 0; i < list.Count;i++)
		{
			XCfgDecompose temp = list[i];
			
			if(temp.EquipLevel == cfgItem.ItemLevel && temp.EquipColorLevel == ColorLevel && temp.StrengthenLevel == (byte)mEquipAttr.mStrengthenLevel)
			{
				return temp.IsCanDecompose > 0;
			}
		}
		
		return false;		
	}
	
	public void RemoveGem(uint gemID)
	{
		for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM; i++)
		{
			if(mEquipAttr.mGemID[i] == gemID)
			{
				mEquipAttr.mGemID[i] = 0;
				return ;
			}
		}
	}
	
	public void RemoveGemAll()
	{
		for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM; i++)
		{
			mEquipAttr.mGemID[i] = 0;
		}
	}
	
	public enum ECanSell
    {
		eCantSell = 1,  //不能出售
		eCanSell  = 0,  //可出售 
	}
	
	public bool CanSell()
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(DataID);
		if(cfgItem == null) return false;
		
		return (ECanSell)cfgItem.IsCanSell == ECanSell.eCanSell;
	}
}


