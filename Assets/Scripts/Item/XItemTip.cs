using System;
using XGame.Client.Packets;

public class XItemTip
{	
	static  public readonly uint ATTRNUM = 5;
	static public void ShowXItemTip(EItemBoxType type, uint dataID,int ObjectIndex)
	{
		ActionIcon_Type newType = (ActionIcon_Type)type;
		switch(newType)
		{
			case ActionIcon_Type.ActionIcon_Bag:
				ShowBagWindowItemTip(type,dataID,ObjectIndex);
			
				break;
			case ActionIcon_Type.ActionIcon_Equip:
				ShowCharInfoItemTip(type,dataID,ObjectIndex);
				
				break;
			case ActionIcon_Type.ActionIcon_PetEquip:
				ShowCharInfoItemTip(type,dataID,ObjectIndex);
				
				break;
			case ActionIcon_Type.ActionIcon_Auction:
				ShowCharInfoItemTip(EItemBoxType.Bag,dataID,ObjectIndex);
				break;
		}
	}
	
	static public void SetNeedUpdateTipPos(bool b)
	{
		XToolTipA.updatePos = false;
		XToolTipA.pos = XChatWindow.ChatToolTipPos;
	}
	
	static public void ShowItemTip(int tipIndex,XItem item)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		XCfgStrengthen scfg = XItemManager.GetStrengthenCfg((uint)item.mEquipAttr.mColor,(byte)item.mEquipAttr.mStrengthenLevel);
		if(scfg == null)
			return ;		
		
		byte ColorLevel = 1;
		if(cfgItem.IsRandom > 0)
			ColorLevel	= (byte)item.mEquipAttr.mColor;
		else
			ColorLevel	= cfgItem.QualityLevel;
		string title = XGameColorDefine.Quality_Color[(int)ColorLevel];	
		
		//1.物品名称
		title += cfgItem.Name;
		title += "\n";
		
		//2.强化等级
		
		if(item.isEquip())
		{
			if(item.mEquipAttr.mStrengthenLevel > 1)
			{
				title += "[color=fff337]";
				title += Convert.ToString(item.mEquipAttr.mStrengthenLevel - 1);
				title += "阶强化";
				title += "\n";
			}
		}
		
		string context = "\n";
		//3.基础属性		
		if(cfgItem.BaseAttrType != 0)
		{
			XCfgAttrID cfgAttrID = XCfgAttrIDMgr.SP.GetConfig(cfgItem.BaseAttrType);
			if(cfgAttrID != null)
			{
				context += cfgAttrID.EffectDes;
//				int curValue = (int)(cfgItem.BaseAttrValue * scfg.AttrRate);
//				if(cfgItem.IsRandom > 0)
//				{
//					XCfgColorValue cfgColorValue = XCfgColorValueMgr.SP.GetConfig((uint)item.mEquipAttr.mColor);
//					if(cfgColorValue != null)
//					{
//						curValue	= (int)(curValue * cfgColorValue.BaseRate);
//					}
//				}
				int curValue	= item.GetBaseAttrValue();
				context += XGameColorDefine.Quality_Color[(int)ColorLevel];
				context += Convert.ToString(curValue);
				context += "\n";
				
			}
		}
		
		//4.物品子类(物品类型)
		context += "[color=ffffff]";
		string subTypeName = XStringManager.SP.GetString((uint)(cfgItem.ItemSubType + 2000));
		context += subTypeName;
		context += "\n";
		
		//4.装备位置
//		if(cfgItem.EquipPos > 0 && cfgItem.EquipPos < (byte)EQUIP_SLOT_TYPE.EQUIP_SLOT_NUM)
//		{
//			context += "装备位置 ";
//			context += GlobalU3dDefine.EquipPos[cfgItem.EquipPos];
//			context += "\n";
//		}		
		
		//5.物品绑定信息 0不绑定，1解封后绑定，2获取绑定
		if(cfgItem.BindingType > 0 && cfgItem.BindingType < (byte)ITEM_BING_TYPE.ITEM_BIND_NUM)
		{
			context += "绑定信息 ";
			if(cfgItem.BindingType == (byte)ITEM_BING_TYPE.ITEM_BIND_RELEASE && !item.IsSeal)
			{
				context += "已绑定";
			}
			else
				context += GlobalU3dDefine.BindType[cfgItem.BindingType];
			context += "\n";
		}
		
		//6.物品要求等级
		if(cfgItem.UseLevelLimitHigh > 0)
		{
			if(XLogicWorld.SP.MainPlayer.Level < cfgItem.UseLevelLimitHigh)
			{
				context += "要求等级";	
				context += "[color=ff0000]" + Convert.ToString(cfgItem.UseLevelLimitHigh);
			}
			else
			{
				if(cfgItem.UseLevelLimitLow > 0)
				{
					context += "要求等级 ";
					context += Convert.ToString(cfgItem.UseLevelLimitLow);
					context += "--";
					context += Convert.ToString(cfgItem.UseLevelLimitHigh);
				}
			}
			
			context += "\n";
		}
		
		//7.装备职业要求
		if(cfgItem.CareerRequire > 0)
		{
			context += "要求职业 ";
//			for(byte i = 1; i < (byte)EJob_Type.JobClass_Num; i++)
//			{
//				if(((1 << i) & cfgItem.CareerRequire) > 0)
//				{
//					context += GlobalU3dDefine.JobName[i];
//					context += " ";
//				}
//			}
			context += XItemManager.GetItemCareerName(cfgItem.Index);
			context += "\n";
		}
		
		//8.专业熟练度
		if(cfgItem.MajorRequire > 0)
		{
			context += "要求专业 ";
			XCareerInfo info = XProductManager.SP.GetCareerInfo(cfgItem.MajorRequire);
			if(info == null)
				context += "[color=ff0000]";
			
			if(info != null && (info.Exp < cfgItem.CareerExp || info.Level < cfgItem.MajorLevel))
				context += "[color=ff0000]";
			
			context += XProductManager.GetCareerName(cfgItem.MajorRequire,cfgItem.MajorLevel);
			context += cfgItem.CareerExp.ToString();
			context += "\n";
		}
		
		//空行		
		context +="\n";
		
		if(cfgItem.BindingType == (byte)ITEM_BING_TYPE.ITEM_BIND_RELEASE && item.IsSeal)
		{
			context += "[color=ff0000]";
			context +=	"未解封";
			context += "\n";
		}
		
		bool Section3HasContent = false;
		//需要解封
		if(!item.IsSeal)
		{
			//12.装备魔法属性
//			if(!item.IsHasDynamicMagicAttr())
//			{
//				for(int i = 0; i < ATTRNUM; i++)
//				{
//					if(cfgItem.MagicAttrType[i] != 0)
//					{
//						XCfgAttrID cfgAttrID = XCfgAttrIDMgr.SP.GetConfig(cfgItem.MagicAttrType[i]);
//						if(cfgAttrID == null)
//							continue ;
//						context += "[color=00FF00]" + cfgAttrID.EffectDes;
//						context += Convert.ToString((int)(cfgItem.MagicAttrValue[i] * scfg.AttrRate));
//						context += "\n";
//						
//						Section3HasContent	= true;
//					
//					}
//				}
//			}
//			else
//			{
//				for(int i = 0; i < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
//				{
//					if(item.mEquipAttr.mAttrID[i] > 0)
//					{
//						XCfgAttrID cfgAttrID = XCfgAttrIDMgr.SP.GetConfig(item.mEquipAttr.mAttrID[i]);
//						if(cfgAttrID == null)
//							continue ;
//						context += "[color=00FF00]" + cfgAttrID.EffectDes;
//						context += Convert.ToString((int)(item.mEquipAttr.mAttrValue[i]*scfg.AttrRate));
//						context += "\n";
//						
//						Section3HasContent	= true;	
//					}
//				}
//			}
			
			for(uint i = 0; i < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
			{
				ushort AttrID = item.GetMagicAttrID(i);
				XCfgAttrID cfgAttrID = XCfgAttrIDMgr.SP.GetConfig((uint)AttrID);
				if(cfgAttrID == null)
					continue;
				context += "[color=00FF00]" + cfgAttrID.EffectDes;
				context += Convert.ToString(item.GetMagicAttrValue(i));
				context += "\n";
				
				Section3HasContent	= true;
			}
			
			if(Section3HasContent)
				context += "\n";
			
			bool Section4HasContent = false;
			//13.宝石
			if(scfg.SlotNum > 0 || item.GetGemNum() > 0)
			{
				Section4HasContent = true;
				context += "[color=FFFFFF]" + "已镶嵌宝石(";
				context += Convert.ToString(item.GetGemNum());
				context += "/";
				context += Convert.ToString(scfg.SlotNum);
				context += ")\n";
				
				if(item.GetGemNum() > 0)
					context += "\n";
				
				for(int i = 0; i < (int)EItem_Equip.MAX_INLAY_GEM_NUM;i++)
				{
					if(item.GetGemID(i) > 0)
					{
						XCfgItem tempGemCfg = XCfgItemMgr.SP.GetConfig(item.GetGemID(i));
						if(tempGemCfg == null)
							continue;
						
						context += XGameColorDefine.Quality_Color[tempGemCfg.QualityLevel];
						context += tempGemCfg.Name;
						context += ": ";
						
						XCfgAttrID cfgAttrID = XCfgAttrIDMgr.SP.GetConfig(tempGemCfg.GemAttrType);
						if(cfgAttrID == null)
							continue;
						context += "[color=00FF00]";
						context += cfgAttrID.EffectDes;	
						context += Convert.ToString(tempGemCfg.GemAttrValue);
						context += "\n";
					}
				}
			}
			
			if(Section4HasContent)
				context += "\n";
		}
		else
		{
			//空行
			context += "\n";
		}
		
		//9.使用效果
		if(!String.IsNullOrEmpty(cfgItem.EffectText))
		{
			if(cfgItem.EffectText[0] != '0')
			{
				context += "[color=00ff00]";				
				context +=cfgItem.EffectText;
				context += "\n";
			}
		}
		
		//15.附加说明
		if(!string.IsNullOrEmpty(cfgItem.AppendText))
		{
			if(cfgItem.AppendText[0] != '0')
			{
				context += "[color=fff337]";
				context += cfgItem.AppendText;
				context += "\n";
				//空行
				context += "\n";
			}
		}
		
		//16.售价
		if(cfgItem.SellPrice > 0)
		{
			context += "[color=00ff00]";
			context += "售价:";
			context += Convert.ToString(cfgItem.SellPrice);
		}
		else
		{
			context += "[color=ff0000]";
			context += "不可出售";
		}
		
		//显示TIP
		XEventManager.SP.SendEvent(EEvent.ToolTip_A, tipIndex,title,context,true,cfgItem.IconAtlasID,cfgItem.IconID,item.Color,item.mEquipAttr.mStrengthenLevel);
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipA);	
		
	}
	
	static private void ShowBagWindowItemTip(EItemBoxType type,uint dataID,int objectIndex)
	{
		XItem item = null;
		item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)dataID);
		if(item == null || item.IsEmpty())
			return ;
		
		ShowItemTip(0,item);
		
		if(item.isEquip())
		{
			//获取同部位装备
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
			if (cfgItem == null)
				return ;
			
			//XItem otherItem = XLogicWorld.SP.MainPlayer.ItemManager.GetAnyoneEquipByPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);
			//if(otherItem == null)
			//	return ;
			int equipPos = XLogicWorld.SP.MainPlayer.ItemManager.GetEquipPos((EQUIP_SLOT_TYPE)cfgItem.EquipPos);
			int ItemPos = XItemManager.GetRealItemIndex(XUIRoleInformation.CurRoInfoSelChar,equipPos);
			XItem otherItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)ItemPos);
			if(otherItem.IsEmpty())
				return ;			
			
			ShowItemTip(1,otherItem);
		}
	}
	
	static private void ShowCharInfoItemTip(EItemBoxType type,uint dataID,int objectIndex)
	{
		//ShowBagWindowItemTip(type,dataID,objectIndex);
		XItem item = null;
		item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)dataID);
		if(item == null || item.IsEmpty())
			return ;
		
		ShowItemTip(0,item);
		
	}
	
	static public void ShowShopItemTip(uint dataIndex)
	{
		XItem item = XShopItemMgr.SP.getBuyBackItem((int)dataIndex);
		if(null == item)
			return;
		
		ShowItemTip(0,item );
	}
	
	static public void ShowMailItemTip(uint dataIndex)
	{
		XItem item = XLogicWorld.SP.MailManager.GetItem(dataIndex);
		if(null == item)
			return;
		
		ShowItemTip(0,item);
	}
}