using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

class XUIStrengthenWindow : XUICtrlTemplate<XStrengthenWindow>
{
	class LeftSingleItem
	{
		XStrengthenActionIcon	ActionIcon;
		UILabel					Content;
	}
	
	class LeftOperObjectArray
	{
		private XStrengthenActionIcon[] StrengthenItemList;
		private UISpriteGroup		SpriteGroup;
		private XStrengthenWindow	UI;
		public int[]				mPetIndex = new int[9];
		public RightOperArray		RightOper = new RightOperArray();
		private int CurSelIndex 	= -1;
		private ulong[]	ItemIndexArray	= new ulong[6];
		public ulong				SelItemGUID; 
		private int SelItemIndex	= 0;		
		
		public void Init(XStrengthenWindow LogicUI,XStrengthenActionIcon[] array)
		{
			UI					= LogicUI;
			StrengthenItemList	= array;
			SpriteGroup			= UI.SpriteGroupObj;
			SpriteGroup.mModify	= ChangeSelObject;
			
			RightOper.SpriteGroup			= UI.FuncSpriteGroup;
			RightOper.mOperStrengthen.UIObj	= UI.Strengthen;
			RightOper.mOperInlay.UIObj		= UI.Inlay;
			RightOper.Init();
		}
		
		public void SetSelItem()
		{
			
		}
		
		private bool HasItem()
		{			
			for(int i =0; i < 6; i++)
			{
				if(ItemIndexArray[i] != 0)
					return true;
			}
			
			return false;
		}
		
		public void SetObjectData(int index)
		{	
//			if(CurSelIndex == index)
//			{
//				return ;
//			}
			
			CurSelIndex	= index;
			XCharacter	CurCh = null;
			
			if(index == 0)
			{				
				CurCh	= XLogicWorld.SP.MainPlayer;
			}				
			else
			{
				int PetRealIndex = mPetIndex[index];
				CurCh = XLogicWorld.SP.PetManager.AllPet[PetRealIndex];	
			}
			
			ShowEquip(CurCh);
			RightOper.Show(SelItemGUID);
		}
		
		public void ChangeSelObject(int index)
		{
			CurSelIndex	= index;
			XCharacter	CurCh = null;
			
			if(index == 0)
			{				
				CurCh	= XLogicWorld.SP.MainPlayer;
			}				
			else
			{
				int PetRealIndex = mPetIndex[index];
				CurCh = XLogicWorld.SP.PetManager.AllPet[PetRealIndex];	
			}
			
			ShowEquip(CurCh);
			//SelItemGUID	= 0;
			RightOper.Show(SelItemGUID);
		}
		
		
		public XCharacter GetCurCharacter()
		{
			if(CurSelIndex <= 0)
			{
				return XLogicWorld.SP.MainPlayer;
			}
			else
			{
				if(CurSelIndex < XStrengthenWindow.MAX_PET_NUM)
				{
					int PetRealIndex = mPetIndex[CurSelIndex];
					XCharacter CurCh = XLogicWorld.SP.PetManager.AllPet[PetRealIndex];	
					return CurCh;
				}
			}
			
			return null;
		}
		
		//获得当前选中的对象的索引(与显示位置无关)	
		public  int GetCurSelObjIndex()
		{	
			XCharacter CurCh = GetCurCharacter();
			int ObjectIndex = 0;
			XMainPlayer player = CurCh as XMainPlayer;
			if(player != null)
				ObjectIndex = 0;
			else
			{
				XPet pet = CurCh as XPet;
				ObjectIndex	= (int)pet.Index;		
			}
			return ObjectIndex;
		}
		
		public void ShowEquip(XCharacter ch)
		{
			int ObjectIndex = GetCurSelObjIndex();
			ushort EquipStartIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Equip);
			int EquipEndIndex	= EquipStartIndex + XStrengthenWindow.SHOW_ICON_NUM - 1;
			int Count = 0;
			
			for(int i = 0;i < 6;i++)
			{
				ItemIndexArray[i] = 0;
			}
			
			for(ushort EquipIndex = EquipStartIndex;EquipIndex <= EquipEndIndex; EquipIndex++)
			{
				EItemBoxType type;
				int ItemIndex;
				if(ObjectIndex == 0)
				{	
					type = EItemBoxType.Equip;
					ItemIndex = (int)(EquipIndex - EquipStartIndex);
				}
				else
				{
					type = EItemBoxType.PetEquip;
					ItemIndex = (int)(EquipIndex - EquipStartIndex) + (ObjectIndex - 1) * (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
				}
				
				StrengthenItemList[EquipIndex-EquipStartIndex].Reset();
				UI.StrengthenItemList[EquipIndex-EquipStartIndex].mLabel.text	= "";
				UI.StrengthenItemList[EquipIndex-EquipStartIndex].SetActive(false);
				
				XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)ItemIndex);
				if(tempItem == null || tempItem.IsEmpty())
					continue;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
				if(cfgItem == null)
					continue;
				
				ItemIndexArray[Count]	= tempItem.GUID;
				
				StrengthenItemList[Count].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,ItemIndex,ObjectIndex);
				StrengthenItemList[Count].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount,false);
				
				UI.StrengthenItemList[Count].mLabel.text	= XGameColorDefine.Quality_Color[(int)tempItem.Color] + cfgItem.Name;
				
				Count++;
			}
			
			if(ItemIndexArray[SelItemIndex] != 0)
			{
				UI.StrengthenItemList[SelItemIndex].SetActive(true);
				SelItemGUID	= ItemIndexArray[SelItemIndex];
			}
			else
			{
				SelItemGUID	= 0;
			}
		}
		
		public void OnItemClick(EEvent evt, params object[] args)
		{
			int index = (int)args[0];			
			SelItemGUID	= ItemIndexArray[index];
			if(SelItemGUID == 0)
				return ;
			
			SelItemIndex	= index;
			
			UI.ActiveItem(SelItemIndex);
			RightOper.Show(SelItemGUID);
			
			// 新手引导
			if ( RightOper.opItem == RightOperArray.OP_ITEM.OP_ITEM_INLAY )
			{
				XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Item_Sel);
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eBagWindow);
			}
		}
		
		public void OnUpdateItem(EEvent evt, params object[] args)
		{
			if(UI == null || !UI.gameObject.activeSelf)
				return ;
			
			SetObjectData(CurSelIndex);
		}
		
		public void OnReflash(EEvent evt, params object[] args)
		{
			if(UI == null || !UI.gameObject.activeSelf)
				return ;
			
			Show();
		}
		
		
		public void Show()
		{
			int petCount = XLogicWorld.SP.PetManager.GetPetCount();
			SpriteGroup.SetVisible(1 + petCount);
			string tempStr = XLogicWorld.SP.MainPlayer.Name;
			tempStr += "\n";
			tempStr += XLogicWorld.SP.MainPlayer.Level.ToString() + "级";
			SpriteGroup.SetText(0,tempStr);
			
			int count = 1;
			for(uint i = XPetManager.PET_INDEX_BEGIN; i < XPetManager.PET_INDEX_END && count <= 8 ; i++)
			{
				XPet pet = XLogicWorld.SP.PetManager.AllPet[(int)i];
				if(pet == null)
				{					
					continue;
				}
				mPetIndex[count]	= (int)pet.Index;
				string TempPetName 	= pet.Name;
				TempPetName	+= "\n";
				TempPetName	+= pet.Level.ToString() + "级";
				SpriteGroup.SetText((int)count,TempPetName); 
				count++;
			}
			
			SpriteGroup.ReSet();
			SetObjectData(0);
			
//			// 新手引导
//			if(HasItem())
//			{
//				Vector3 showPos = new Vector3(Mathf.Round(UI.StrengthenItemList[0].Parent.transform.localPosition.x - 189),
//				Mathf.Round(UI.StrengthenItemList[0].Parent.transform.localPosition.y + 380),
//				Mathf.Round(UI.StrengthenItemList[0].Parent.transform.localPosition.z));
//				if(RightOper.opItem == XUIStrengthenWindow.RightOperArray.OP_ITEM.OP_ITEM_INLAY)
//				{
//					XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Item_Sel, 
//					(int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click_item, 1, showPos, 
//					UI.StrengthenItemList[0].Parent);					
//				}				
//
//				XNewPlayerGuideManager.SP.handleRoleStrengthOrInlay(0);
//			}
//			
//			//显示镶嵌的箭头
//			Vector3 showPos1 = new Vector3(Mathf.Round(UI.FuncSpriteGroup.GetSprite(1).gameObject.transform.parent.localPosition.x + 158),
//				Mathf.Round(UI.FuncSpriteGroup.GetSprite(1).gameObject.transform.parent.localPosition.y + 215),
//				Mathf.Round(UI.FuncSpriteGroup.GetSprite(1).gameObject.transform.parent.localPosition.z));
//			
//			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click_item, 
//					1, showPos1,
//					UI.FuncSpriteGroup.gameObject);
//			
//			
			
		}
	}
	
	class RightOperArray
	{
		public enum OP_ITEM
		{
			OP_ITEM_STRENGTHEN,
			OP_ITEM_INLAY,
			OP_ITEM_XILIAN,
		}
		
		public OperStrengthen	mOperStrengthen	= new OperStrengthen();
		public OperInlay		mOperInlay		= new OperInlay();
		public OperXiLian		mOperXiLian		= new OperXiLian();
		public UISpriteGroup	SpriteGroup;
		public int CurSelIndex 	= -1;
		public ulong			SelItemGUID;
		public OP_ITEM opItem	= OP_ITEM.OP_ITEM_STRENGTHEN;
		public ItemSelContainer	Container;
		
		public void Init()
		{
			SpriteGroup.mModify	= OnSelectModify;
			SpriteGroup.SetVisible(2);
			mOperStrengthen.Init();
			mOperInlay.Init();
			mOperXiLian.Init();
		}
		
		public void OnSelectModify(int index)
		{
			CurSelIndex	= index;
			if(index == 0)
			{
				opItem	= OP_ITEM.OP_ITEM_STRENGTHEN;
				mOperStrengthen.Show(SelItemGUID);
				mOperInlay.Hide();
				mOperXiLian.Hide();
			}
			else if(index == 1)
			{
				opItem	= OP_ITEM.OP_ITEM_INLAY;
				mOperInlay.Show(SelItemGUID);
				mOperStrengthen.Hide();
				mOperXiLian.Hide();
				
				
				//XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click_item);
			}
			else
			{
				opItem	= OP_ITEM.OP_ITEM_XILIAN;
				mOperStrengthen.Hide();
				mOperInlay.Hide();
				mOperXiLian.Show(SelItemGUID);
			}
			
			Container.Show((uint)index);
		}
		
		public void Show(ulong itemGUID)
		{
			if(CurSelIndex == -1)
				CurSelIndex	= 0;
			
			SelItemGUID	= itemGUID;
			
			OnSelectModify(CurSelIndex);
		}
	}
	
	class OperStrengthen
	{
		public XStrengthenWindow.StrengthenUIObj	UIObj;
		private XStrengthenActionIcon	ActionIcon = new XStrengthenActionIcon();
		private XStrengthenActionIcon[] MatActionIconArray = new XStrengthenActionIcon[2];
		private ulong SelItemGUID;
		private bool IsInit = false;
		private static uint StrengthenEffectID = 900059;
		
		public void Init()
		{
			if(!IsInit)
			{
				UIObj.Init();
				IsInit	= true;
				ActionIcon.SetUIIcon(UIObj.OperItemIcon);
				
				for(int i = 0; i < 2;i++)
				{
					MatActionIconArray[i]	= new XStrengthenActionIcon();
					MatActionIconArray[i].SetUIIcon(UIObj.materialList[i]);
				}
				
				
			}
		}
		
		public void SetActive(bool isActive,bool isCanStrengthen)
		{
			UIObj.mStrenghtenUI.SetActive(isActive);
			UIObj.IconBKSprite.gameObject.SetActive(isActive);
			UIObj.SeperatorMid.gameObject.SetActive(isActive);
			UIObj.SeperatorBottom.gameObject.SetActive(isActive);
			UIObj.EmptyLabel.gameObject.SetActive(!isActive);
			if(!isActive)
			{
				if(isCanStrengthen)
					UIObj.EmptyLabel.text	= XStringManager.SP.GetString(508);
				else
					UIObj.EmptyLabel.text	= XStringManager.SP.GetString(509);
			}
			else
			{
				UIObj.EmptyLabel.text	= "";
			}
			
			ActionIcon.SetVisible(isActive);
		}
		
		public void OnStartStrengthen(EEvent evt, params object[] args)
		{
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(SelItemGUID);
			if(item == null)
				return ;
			
			bool isCan = XLogicWorld.SP.MainPlayer.ItemManager.IsCanStrengthen(SelItemGUID);
			if(isCan)
			{
				XU3dEffect strengthenEffect = new XU3dEffect(StrengthenEffectID,OnEffectLoaded);
				
				//通知服务器
				XItemPacketProcess.SP.SendItemComStrengthen(item.ItemIndex);
			}
			else
			{			
				XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(60));
			}
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_Strength_Click);
		}
		
		public  void OnEffectLoaded(XU3dEffect self)
		{
			self.Layer	= GlobalU3dDefine.Layer_UI_2D;
			self.Parent	= UIObj.OperItemIcon.gameObject.transform;
			self.LocalPosition	= new Vector3(0,0,-50);
			//self.Scale	= new Vector3(300,300,1);
		}
		
		public void OnDirectStrengthen(EEvent evt, params object[] args)
		{
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(SelItemGUID);
			if(item == null)
				return ;
			
			XCfgStrengthen cfgStrengthen = null;
			foreach(XCfgStrengthen cfgStr in XCfgStrengthenMgr.SP.ItemTable)
			{
				if(cfgStr.EquipColorLevel == (uint)item.Color && cfgStr.StrengthenLevel == (byte)item.mEquipAttr.mStrengthenLevel)
				{
					cfgStrengthen = cfgStr;
				}
			}		
			
			if(cfgStrengthen == null)
				return ;
			
			long realMoney = XLogicWorld.SP.MainPlayer.RealMoney;
			if(realMoney < cfgStrengthen.RealMoney)
			{
				XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(60));
				return ;
			}
			
			XU3dEffect strengthenEffect = new XU3dEffect(StrengthenEffectID,OnEffectLoaded);
			
			//通知服务器
			XItemPacketProcess.SP.SendItemMoneyStrengthen(item.ItemIndex);
		}
		
		public void Reflash(EEvent evt, params object[] args)
		{
			Show(SelItemGUID);
		}
		
		public void Show(ulong itemGUID)
		{
			if(UIObj == null)
				return ;
			//激活页面
			SetActive(true,true);
			SelItemGUID	= itemGUID;
			//更新UI
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(itemGUID);	
			if(item == null || !item.isEquip() || !item.IsCanStrengthen())
			{
				UIObj.ClearAttrLabel();
				UIObj.ClearStrengthenInfo();
				SetActive(false,item == null);
				return ;
			}
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
			if(cfgItem == null)
				return ;
			EItemBoxType boxType;
			ushort index;
			XItemManager.GetContainerType(item.ItemIndex,out boxType,out index);
			
			ActionIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,item.Color,1);	
			ActionIcon.SetLogicData((ActionIcon_Type)boxType,(uint)index);
			UIObj.StrengthenItemName.text	= cfgItem.Name;			
			UIObj.StrengthenItemName.text 	+= "\n";
			if(item.mEquipAttr.mStrengthenLevel > 1)
			{
				UIObj.StrengthenItemName.text	+= Convert.ToString(item.mEquipAttr.mStrengthenLevel - 1);
				UIObj.StrengthenItemName.text	+= "阶强化";
			}
				
			//显示材料
			XCfgStrengthen Nextcfg = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel+1));
			if(Nextcfg == null)
				return ;
			float curAttrRate = 0f;
			XCfgStrengthen curCfg = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel));
			if(curCfg == null)
				curAttrRate = 1.0f;
			else
				curAttrRate = curCfg.AttrRate;
				
			if(item == null || !item.isEquip() || !item.IsCanStrengthen())
				return ;
				
			//基本属性
			XCfgAttrID attrID = XCfgAttrIDMgr.SP.GetConfig(cfgItem.BaseAttrType);
			if(attrID == null)
				return ;
			
			int afterAttrValue = item.GetBaseAttrValueByStrengthenLevel((byte)(item.mEquipAttr.mStrengthenLevel + 1));
			int deltaAttrValue = afterAttrValue - item.GetBaseAttrValue();
			
			
			UIObj.AttrLabelL[0].text	= attrID.EffectDes;			
			UIObj.AttrLabelL[0].text	+= Convert.ToString((int)(item.GetBaseAttrValue()));
	
			UIObj.AttrLabelR[0].text	= "+";
			UIObj.AttrLabelR[0].text	+= Convert.ToString(deltaAttrValue);
			
			//直接强化的TIP
			XCfgStrengthen cfg = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)item.mEquipAttr.mStrengthenLevel);
			if(cfg == null)
				return ;
			
			XCfgStrengthen cfgRealMoney = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)item.mEquipAttr.mStrengthenLevel,cfgItem.ItemLevel);
			if(cfgRealMoney == null)
				return ;
			
			string str = XStringManager.SP.GetString(150);
			str = String.Format(str,cfgRealMoney.RealMoney);
			UIObj.BtnDirectStrengthen.CommonTips	= str;
			
			//魔法属性			
			for(uint i = 0; i < (uint)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
			{
				ushort MagicAttrID = item.GetMagicAttrID(i);
				int MagicAttrValue = item.GetMagicAttrValue(i);
				
				XCfgAttrID tempAttrID = XCfgAttrIDMgr.SP.GetConfig((uint)MagicAttrID);
				if(tempAttrID == null)
					continue;
				
				UIObj.AttrLabelL[i+1].text	= tempAttrID.EffectDes;
				UIObj.AttrLabelL[i+1].text	+= Convert.ToString(MagicAttrValue);
				
				int afterMagicAttrValue = item.GetMagicAttrValueByStrengthenLevel(i,(byte)(item.mEquipAttr.mStrengthenLevel + 1));
				int deltaMagicAttrValue	= afterMagicAttrValue - item.GetMagicAttrValue(i);
				
				UIObj.AttrLabelR[i+1].text	= "+";
				UIObj.AttrLabelR[i+1].text	+= Convert.ToString(deltaMagicAttrValue);
			}
			
			bool isCanStrengthen = true;
			for(int i = 0; i < XStrengthenWindow.MAX_MATERIAL_NUM; i++)
			{
				MatActionIconArray[i].SetVisible(false);
				uint materialID = curCfg.MaterialID[i];
				XCfgItem tempCfg = XCfgItemMgr.SP.GetConfig(materialID);
				if(tempCfg == null)
					continue;
				
				MatActionIconArray[i].SetVisible(true);
				
				MatActionIconArray[i].SetSprite(tempCfg.IconAtlasID,tempCfg.IconID,(EItem_Quality)tempCfg.QualityLevel,1);
				MatActionIconArray[i].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,(uint)materialID);
				
				int bagMaterialNum = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(materialID);
				UIObj.materialNum[i].text	= "";
				if(bagMaterialNum < curCfg.MaterialNum[i])
				{
					isCanStrengthen	= false;
					UIObj.materialNum[i].text += "[color=ff0000]";
				}
			
				UIObj.materialNum[i].text += Convert.ToString(bagMaterialNum);
				UIObj.materialNum[i].text += "/";
				UIObj.materialNum[i].text += Convert.ToString(curCfg.MaterialNum[i]);
				
				if(curCfg.GameMoney == 0)
				{
					UIObj.MoneySprite.gameObject.SetActive(false);
					UIObj.Money.text	= "";
				}
				else
				{
					UIObj.MoneySprite.gameObject.SetActive(true);
					
					XCfgStrengthen curCfgMoney = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel),cfgItem.ItemLevel);					
					UIObj.Money.text	= Convert.ToString(curCfgMoney.GameMoney);
					
					long GameMoney = XLogicWorld.SP.MainPlayer.GameMoney;					
					if(GameMoney < (long)curCfgMoney.GameMoney)
					{
						isCanStrengthen	= false;
					}
				}
				
				// 设置按钮字体状态
				UIObj.BtnStrengthen.isEnabled	= isCanStrengthen;
				UIObj.BtnStrengthen.UpdateImage();
				
				long RealMoney = XLogicWorld.SP.MainPlayer.RealMoney;
				UIObj.BtnDirectStrengthen.isEnabled = (RealMoney >= (long)curCfg.RealMoney);
				UIObj.BtnDirectStrengthen.UpdateImage();
			}			
		}
		
		public void Hide()
		{
			if(UIObj == null)
				return ;
			
			UIObj.mStrenghtenUI.SetActive(false);
		}
	}
	
	class OperInlay
	{
		public XStrengthenWindow.InlayUIObj	UIObj;
		private XStrengthenActionIcon	ActionIcon 		= new XStrengthenActionIcon();
		private XStrengthenActionIcon[]	InlayGemIcon	= new XStrengthenActionIcon[3];
		private ulong SelItemGUID;
		private bool IsInit = false;
		
		public void Init()
		{
			if(!IsInit)
			{
				UIObj.Init();
				IsInit	= true;
				ActionIcon.SetUIIcon(UIObj.OperItemIcon);
				
				for(int i =0; i < 3; i++)
				{
					InlayGemIcon[i] = new XStrengthenActionIcon();
					InlayGemIcon[i].SetUIIcon(UIObj.mUIInlayGemIcon[i]);
				}
			}
		}
		
		public void OnGemInlay(EEvent evt, params object[] args)
		{
			int index = (int)args[0];
			
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(SelItemGUID);
			if(item == null)
				return ;
			
			if(InlayGemIcon[index].IsHasItem)
				XItemPacketProcess.SP.SendItemGemRemove(item.ItemIndex,InlayGemIcon[index].DataID);
			else if(InlayGemIcon[index].IsNewSet)
				XItemPacketProcess.SP.SendItemGemInlay((ushort)item.ItemIndex,(ushort)InlayGemIcon[index].ItemIndex,(uint)index);
		}
		
		public void OnGemRemoveAll(EEvent evt, params object[] args)
		{
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(SelItemGUID);
			if(item == null)
				return ;
			XItemPacketProcess.SP.SendItemGemRemoveAll(item.ItemIndex);
		}
		
		public void SetActive(bool isActive)
		{
			UIObj.mInlayUI.SetActive(isActive);
			UIObj.IconBKSprite.gameObject.SetActive(isActive);
			//UIObj.SeperatorMid.gameObject.SetActive(isActive);
			//UIObj.SeperatorBottom.gameObject.SetActive(isActive);
			UIObj.EmptyLabel.gameObject.SetActive(!isActive);
			if(!isActive)
			{				
				UIObj.EmptyLabel.text	= XStringManager.SP.GetString(508);				
			}
			else
			{
				UIObj.EmptyLabel.text	= "";
			}
			
			ActionIcon.SetVisible(isActive);
		}
		
		public void Show(ulong itemGUID)
		{
			if(UIObj == null)
				return ;
			
			SelItemGUID	= itemGUID;
			//UIObj.mInlayUI.SetActive(true);
			SetActive(true);
			
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(itemGUID);	
			if(item == null || !item.isEquip())
			{
				UIObj.ClearInlayInfo();
				SetActive(false);
				return ;
			}
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
			if(cfgItem == null)
				return ;
			EItemBoxType boxType;
			ushort index;
			XItemManager.GetContainerType(item.ItemIndex,out boxType,out index);
			
			ActionIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,item.Color,1);	
			ActionIcon.SetLogicData((ActionIcon_Type)boxType,(uint)index);
			UIObj.StrengthenItemName.text	= cfgItem.Name;			
			UIObj.StrengthenItemName.text 	+= "\n";
			if(item.mEquipAttr.mStrengthenLevel > 1)
			{
				UIObj.StrengthenItemName.text	+= Convert.ToString(item.mEquipAttr.mStrengthenLevel - 1);
				UIObj.StrengthenItemName.text	+= "阶强化";
			}
			
			XCfgStrengthen curCfg = XItemManager.GetStrengthenCfg((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel));
			if(curCfg == null)
				return ;
			
			bool isHasGem = false;
			for(int i = 0; i < 3; i++)
			{
				InlayGemIcon[i].Label		= UIObj.FreeGemLabel[i];
				InlayGemIcon[i].Btn			= UIObj.mUIInlayBtn[i];
				
				if(i < curCfg.SlotNum)
				{
					UIObj.UnLockSprite[i].gameObject.SetActive(false);
					UIObj.mUIInlayBtn[i].gameObject.SetActive(true);
					if(item.mEquipAttr.mGemID[i] > 0)
					{
						XCfgItem cfgGemItem = XCfgItemMgr.SP.GetConfig(item.mEquipAttr.mGemID[i]);
						if(cfgItem == null)
							continue;
						XCfgAttrID cfgGemAttr = XCfgAttrIDMgr.SP.GetConfig(cfgGemItem.GemAttrType);
						if(cfgGemAttr == null)
							continue;
						
						isHasGem	= true;
						
						//显示上部宝石图标
						XUIDynamicAtlas.SP.SetSprite(UIObj.mGemSprite[i],(int)cfgGemItem.IconAtlasID,cfgGemItem.IconID);
						UIObj.mGemLabel[i].text	= XGameColorDefine.Quality_Color[cfgGemItem.QualityLevel];
						UIObj.mGemLabel[i].text	+= cfgGemItem.Name;
						UIObj.mGemLabel[i].text	+= "  ";
						UIObj.mGemLabel[i].text	+= "[color=00ff00]" + cfgGemAttr.EffectDes;
						UIObj.mGemLabel[i].text	+= Convert.ToString(cfgGemItem.GemAttrValue);
						
						//显示下部宝石Icon
						InlayGemIcon[i].SetSprite(cfgGemItem.IconAtlasID,cfgGemItem.IconID,(EItem_Quality)cfgGemItem.QualityLevel,1);
						InlayGemIcon[i].IsHasItem	= true;
						InlayGemIcon[i].IsNewSet	= false;
						InlayGemIcon[i].IsLock		= false;
						InlayGemIcon[i].DataID		= item.mEquipAttr.mGemID[i];
						InlayGemIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,item.mEquipAttr.mGemID[i]);
						
						UIObj.FreeGemLabel[i].gameObject.SetActive(false);
						
						UIObj.mUIInlayBtn[i].isEnabled = true;
						UIObj.mUIInlayBtn[i].UpdateImage();
						UILabel label = UIObj.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
						if(label == null)
							continue;
						label.text	= "摘除";
					}
					else
					{		
						UIObj.mGemSprite[i].atlas			= UIObj.StrengthenAtlas;
						UIObj.mGemSprite[i].spriteName		= UIObj.mGemBKName;
						UIObj.mGemLabel[i].text				= "未镶嵌宝石";
					
						InlayGemIcon[i].Reset();
						InlayGemIcon[i].IsHasItem	= false;
						InlayGemIcon[i].IsNewSet	= false;
						InlayGemIcon[i].DataID		= 0;
						InlayGemIcon[i].IsLock		= false;
						InlayGemIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,0);
						UIObj.FreeGemLabel[i].gameObject.SetActive(true);
						
						UIObj.mUIInlayBtn[i].isEnabled = false;
						UIObj.mUIInlayBtn[i].UpdateImage();
						UILabel label = UIObj.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
						if(label == null)
							continue;
						label.text	= "镶嵌";
					}
				}
				else
				{
					UIObj.mGemSprite[i].atlas		= UIObj.StrengthenAtlas;
					UIObj.mGemSprite[i].spriteName	= UIObj.mGemBKName;
					UIObj.mGemLabel[i].text			= "强化后开启";
					
					InlayGemIcon[i].Reset();
					InlayGemIcon[i].IsHasItem	= false;
					InlayGemIcon[i].DataID		= 0;
					InlayGemIcon[i].IsNewSet	= false;
					InlayGemIcon[i].IsLock		= true;
					InlayGemIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,0);
					
					UIObj.mUIInlayBtn[i].gameObject.SetActive(false);
					UIObj.UnLockSprite[i].gameObject.SetActive(true);
					
					UIObj.FreeGemLabel[i].gameObject.SetActive(false);
					
//					UILabel label = LogicUI.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
//					if(label == null)
//						continue;
//					label.text	= "镶嵌";
				}
			}
			
			for(int i = 0; i < 3;i++)
			{			
				InlayGemIcon[i].NeedStrengthenItem	= item;
			}
			
			if(isHasGem)
			{
				UIObj.mUIInlayBtn[3].isEnabled	= true;
				UIObj.mUIInlayBtn[3].UpdateImage();
			}
			else
			{
				UIObj.mUIInlayBtn[3].isEnabled	= false;
				UIObj.mUIInlayBtn[3].UpdateImage();
			}
		}
		
		public void Hide()
		{
			if(UIObj == null)
				return ;
			
			UIObj.mInlayUI.SetActive(false);
		}
	}
	
	class OperXiLian
	{
		private ulong SelItemGUID;
		
		public void Init()
		{
			
		}
		
		public void Show(ulong itemGUID)
		{
			
		}
		
		public void Hide()
		{
			
		}
	}
	
	public class ItemSelContainer
	{
		public XBagActionIcon[] mActionList = new XBagActionIcon[XStrengthenWindow.MAX_ITEM_SEL_NUM];
		public ArrayList		mAllTypeItemList = new ArrayList();
		private uint 			CurSelPage 	= 0;
		private uint			PageNum		= 1;
		private bool			isInit = false;
		public  uint			CurSelFunc = 0;
		public UILabel			LabelPage;
		public UIImageButton	LeftBtn;
		public UIImageButton	RightBtn;
		
		public void Init(XActionIcon[] UIList)
		{
			if(isInit)
				return ;
			isInit	= true;
			for(int i = 0; i < XStrengthenWindow.MAX_ITEM_SEL_NUM; i++)
			{
				mActionList[i] = new XBagActionIcon();
				mActionList[i].SetUIIcon(UIList[i]);
				mActionList[i].IsCanDBClick	= true;
				mActionList[i].IsCanToolTip	= true;
				mActionList[i].IsCanPopMenu	= true;
				mActionList[i].IsCanDrag	= false;
				mActionList[i].IsCanDrop	= false;
			}
		}
		
		public void UpdateContainerHandle(EEvent evt, params object[] args)
		{
			Show(CurSelFunc);
		}
		
		public void LeftBtnHandle(EEvent evt, params object[] args)
		{
			if(CurSelPage == 0)
				return ;
			
			CurSelPage -= 1;
			
			Show(CurSelFunc);
		}
		
		public void RightBtnHandle(EEvent evt, params object[] args)
		{
			if(CurSelPage >= PageNum - 1)
				return ;
			
			CurSelPage += 1;
			
			Show(CurSelFunc);
		}
		
		//其他类型，宝石，符印，灵丹，靠子类来判断
		private void GetTypeItem(ushort type)
		{
			mAllTypeItemList.Clear();
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
				
				if(cfgItem.TypeTag == type)
					mAllTypeItemList.Add(LogicItem.ItemIndex);
			}
		}
		
		private void ShowItem()
		{
			//清空所有按钮数据
			for(int i = 0;i < XStrengthenWindow.MAX_ITEM_SEL_NUM;i++)
			{
				mActionList[i].ResetUIAndLogic();
			}
			
			PageNum = (uint)(mAllTypeItemList.Count / (XStrengthenWindow.MAX_ITEM_SEL_NUM + 1) + 1);
			LabelPage.text = (CurSelPage + 1).ToString() + "/" + PageNum.ToString();
			
			if(CurSelPage == 0)
			{
				LeftBtn.isEnabled	= false;
				LeftBtn.UpdateImage();
			}
			else
			{
				LeftBtn.isEnabled	= true;
				LeftBtn.UpdateImage();
			}
			
			if(CurSelPage < PageNum - 1)
			{
				RightBtn.isEnabled	= true;
				RightBtn.UpdateImage();
			}
			else
			{
				RightBtn.isEnabled	= false;
				RightBtn.UpdateImage();
			}			
			
			int Base = (int)CurSelPage * (int)XStrengthenWindow.MAX_ITEM_SEL_NUM;
			if(Base >= mAllTypeItemList.Count)
				return ;
			
			for(int i = 0;i < XStrengthenWindow.MAX_ITEM_SEL_NUM && (Base + i) < mAllTypeItemList.Count; i++)
			{
				int tempIndex = Base + i;
				ushort itemIndex = (ushort)mAllTypeItemList[tempIndex];
				mActionList[i].SetLogicData(ActionIcon_Type.ActionIcon_Bag,(uint)itemIndex);
				
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)itemIndex);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				mActionList[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,LogicItem.Color,LogicItem.ItemCount,true,LogicItem.mEquipAttr.mStrengthenLevel);
			}
		}
		
		//index 标示第几个标签
		public void Show(uint index)
		{
			if(index == 0)
				GetTypeItem(1);
			else if(index == 1)
				GetTypeItem(10);
			
			CurSelFunc	= index;
			
			
			ShowItem();
		}
	}
	
	ItemSelContainer Container	= new ItemSelContainer();
	
	LeftOperObjectArray	LeftOper = new LeftOperObjectArray();
	
	public XCharacter CurSelObj
	{
		get
		{
			return LeftOper.GetCurCharacter();
		}
	}
	
	public XStrengthenActionIcon[] 	LogicStrengthenIcon = new XStrengthenActionIcon[XStrengthenWindow.SHOW_ICON_NUM];
	public XStrengthenActionIcon 	LogicOperIcon = new XStrengthenActionIcon();
	private bool	IsInit = false;
	
	public XUIStrengthenWindow()
	{		
//		XEventManager.SP.AddHandler(OnItemUpdate, EEvent.Strengthen_Item_Update);
//		XEventManager.SP.AddHandler(OnUpdateIndex, EEvent.Strengthen_UpdateIndex);
		XEventManager.SP.AddHandler(LeftOper.OnItemClick, EEvent.Strengthen_ItemClick);
		XEventManager.SP.AddHandler(LeftOper.RightOper.mOperStrengthen.OnStartStrengthen, EEvent.Strengthen_StartShtrengthen);
		XEventManager.SP.AddHandler(LeftOper.RightOper.mOperStrengthen.OnDirectStrengthen, EEvent.Strengthen_StartDirectStr);
		XEventManager.SP.AddHandler(LeftOper.OnUpdateItem, EEvent.Strengthen_UpdateUI);
		XEventManager.SP.AddHandler(LeftOper.OnReflash, EEvent.CharInfo_AddPet);
		XEventManager.SP.AddHandler(LeftOper.RightOper.mOperStrengthen.Reflash, EEvent.Strengthen_UpdateData);
		
//		XEventManager.SP.AddHandler(OnShowBagUI, EEvent.Strengthen_ShowBag);
//		XEventManager.SP.AddHandler(OnShowEquipUI, EEvent.Strengthen_ShowEquip);
//		XEventManager.SP.AddHandler(OnShowInlay, EEvent.Strengthen_ShowInlay);
//		XEventManager.SP.AddHandler(OnShowStrengthen, EEvent.Strengthen_ShowStrengthen);
//		
		XEventManager.SP.AddHandler(LeftOper.RightOper.mOperInlay.OnGemInlay, EEvent.Strengthen_InlayBtn);
		XEventManager.SP.AddHandler(LeftOper.RightOper.mOperInlay.OnGemRemoveAll, EEvent.Strengthen_InlayRemoveAll);
		
		XEventManager.SP.AddHandler(Container.LeftBtnHandle, EEvent.Strengthen_LeftPageBtn);
		XEventManager.SP.AddHandler(Container.RightBtnHandle, EEvent.Strengthen_RightPageBtn);
		
	}
	
		
	public override void OnShow()
	{
		if(!IsInit)
		{
			IsInit	= true;
			for(int i = 0; i < XStrengthenWindow.SHOW_ICON_NUM; i++)
			{
				LogicStrengthenIcon[i] = new XStrengthenActionIcon();
				LogicUI.StrengthenItemList[i].Init();
				LogicStrengthenIcon[i].SetUIIcon(LogicUI.StrengthenItemList[i].ActionIcon);
			}
			
			LogicOperIcon.SetUIIcon(LogicUI.Inlay.OperItemIcon);
			LeftOper.Init(LogicUI,LogicStrengthenIcon);
			
			LeftOper.RightOper.Container	= Container;
			Container.LeftBtn	= LogicUI.LeftBtn;
			Container.RightBtn	= LogicUI.RightBtn;
			Container.LabelPage	= LogicUI.LabelPage;
			Container.Init(LogicUI.ItemSelIconList);
		}
		
		LeftOper.Show();
		
	}
	
	public override void OnHide()
	{
		//CurSelItemGUID	= 0;
	}
//	public void OnUpdateIndex(EEvent evt, params object[] args)
//	{
//		int index = (int)args[0];
//		UInt64 selItemGUID	= 0;
//		
//		if(index >= CurShowItemNum)
//		{
//			//清空
//			LogicUI.ClearStrengthenInfo();
//			return ;
//		}
//		if(CurShowItemNum <= XStrengthenWindow.SHOW_ICON_NUM)
//		{
//			selItemGUID = mAllEquip[index];					
//		}
//		else
//		{
//			selItemGUID	= mAllEquip[CurShowItemNum - XStrengthenWindow.SHOW_ICON_NUM + index];
//		}
//		
//		CurSelItemGUID	= selItemGUID;
//		
//		_UpdateIndex(CurSelItemGUID);
//		
//		// 新手引导
//		if ( OP_ITEM.OP_ITEM_STRENGTHEN ==	opItem )
//		{
//			Vector3 showPos = new Vector3(Mathf.Round(LogicUI.BtnStrengthen.transform.localPosition.x - 140),
//				Mathf.Round(LogicUI.BtnStrengthen.transform.localPosition.y + 8),
//				Mathf.Round(LogicUI.BtnStrengthen.transform.localPosition.z));
//			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_Strength_Click, 2, showPos, 
//				LogicUI.BtnStrengthen.transform.parent.gameObject);
//		}
//	}	

//	
//	private void _UpdateIndex(UInt64 guid)
//	{
//		//更新UI
//		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(guid);	
//		if(item == null || !item.isEquip() || !item.IsCanStrengthen())
//		{
//			//XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(60));
//			LogicUI.ClearAttrLabel();
//			LogicUI.ClearStrengthenInfo();
//			return ;
//		}
//		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
//		if(cfgItem == null)
//			return ;
//		EItemBoxType boxType;
//		ushort index;
//		XItemManager.GetContainerType(item.ItemIndex,out boxType,out index);
//		
//		LogicUI.LogicStrengthenIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,item.Color,1);	
//		LogicUI.LogicStrengthenIcon.SetLogicData((ActionIcon_Type)boxType,(uint)index);
//		LogicUI.StrengthenItemName.text	= cfgItem.Name;
//		LogicUI.StrengthenItemName.text += "\n";
//		LogicUI.StrengthenItemName.text	+= Convert.ToString(item.mEquipAttr.mStrengthenLevel);
//		LogicUI.StrengthenItemName.text	+= "级强化";
//		
//		//显示材料
//		XCfgStrengthen Nextcfg = XCfgStrengthenMgr.SP.GetConfig((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel+1));
//		if(Nextcfg == null)
//			return ;
//		float curAttrRate = 0f;
//		XCfgStrengthen curCfg = XCfgStrengthenMgr.SP.GetConfig((uint)item.Color,(byte)(item.mEquipAttr.mStrengthenLevel));
//		if(curCfg == null)
//			curAttrRate = 1.0f;
//		else
//			curAttrRate = curCfg.AttrRate;
//		
//		if(opItem == OP_ITEM.OP_ITEM_STRENGTHEN)
//		{
//			if(item == null || !item.isEquip() || !item.IsCanStrengthen())
//				return ;
//			
//			//基本属性
//			XCfgAttrID attrID = XCfgAttrIDMgr.SP.GetConfig(cfgItem.BaseAttrType);
//			if(attrID == null)
//				return ;
//			
//			int afterAttrValue = (int)(cfgItem.BaseAttrValue * Nextcfg.AttrRate);
//			int deltaAttrValue = afterAttrValue - (int)(cfgItem.BaseAttrValue * curCfg.AttrRate);
//			
//			
//			LogicUI.AttrLabelL[0].text	= attrID.EffectDes;			
//			LogicUI.AttrLabelL[0].text	+= Convert.ToString((int)(cfgItem.BaseAttrValue * curAttrRate));
//
//			LogicUI.AttrLabelR[0].text	= "+";
//			LogicUI.AttrLabelR[0].text	+= Convert.ToString(deltaAttrValue);
//			
//			//魔法属性
//			if(item.IsHasDynamicMagicAttr())
//			{
//				for(int i = 0; i < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
//				{
//					if(item.mEquipAttr.mAttrID[i] > 0)
//					{
//						XCfgAttrID tempAttrID = XCfgAttrIDMgr.SP.GetConfig(item.mEquipAttr.mAttrID[i]);
//						if(tempAttrID == null)
//							continue;
//						
//						LogicUI.AttrLabelL[i+1].text	= tempAttrID.EffectDes;						
//						LogicUI.AttrLabelL[i+1].text	+= Convert.ToString((int)(item.mEquipAttr.mAttrValue[i]*curCfg.AttrRate));								
//						
//						int afterMagicAttrValue = (int)(item.mEquipAttr.mAttrValue[i] * Nextcfg.AttrRate);
//						int deltaMagicAttrValue = afterMagicAttrValue - (int)(item.mEquipAttr.mAttrValue[i] *curCfg.AttrRate);
//	
//						LogicUI.AttrLabelR[i+1].text	= "+";
//						LogicUI.AttrLabelR[i+1].text	+= Convert.ToString(deltaMagicAttrValue);
//					}
//				}
//			}
//			else
//			{
//				for(int i = 0; i < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM;i++)
//				{
//					if(cfgItem.MagicAttrType[i] > 0)
//					{
//						XCfgAttrID tempAttrID = XCfgAttrIDMgr.SP.GetConfig(cfgItem.MagicAttrType[i]);
//						if(tempAttrID == null)
//							continue;
//						
//						LogicUI.AttrLabelL[i+1].text	= tempAttrID.EffectDes;						
//						LogicUI.AttrLabelL[i+1].text	+= Convert.ToString((int)(cfgItem.MagicAttrValue[i]*curCfg.AttrRate));
//						
//						int afterMagicAttrValue = (int)(cfgItem.MagicAttrValue[i] * Nextcfg.AttrRate);
//						int deltaMagicAttrValue = afterMagicAttrValue - (int)(cfgItem.MagicAttrValue[i] *curCfg.AttrRate);
//	
//						LogicUI.AttrLabelR[i+1].text	= "+";
//						LogicUI.AttrLabelR[i+1].text	+= Convert.ToString(deltaMagicAttrValue);
//					}
//				}
//			}
//			
//			bool isCanStrengthen = true;
//			for(int i = 0; i < XStrengthenWindow.MAX_MATERIAL_NUM; i++)
//			{
//				LogicUI.LogicMatIcon[i].SetVisible(false);
//				uint materialID = curCfg.MaterialID[i];
//				XCfgItem tempCfg = XCfgItemMgr.SP.GetConfig(materialID);
//				if(tempCfg == null)
//					continue;
//				
//				LogicUI.LogicMatIcon[i].SetVisible(true);
//				
//				LogicUI.LogicMatIcon[i].SetSprite(tempCfg.IconAtlasID,tempCfg.IconID,(EItem_Quality)tempCfg.QualityLevel,1);
//				LogicUI.LogicMatIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,(uint)materialID);
//				
//				int bagMaterialNum = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(materialID);
//				LogicUI.materialNum[i].text	= "";
//				if(bagMaterialNum < curCfg.MaterialNum[i])
//				{
//					isCanStrengthen	= false;
//					LogicUI.materialNum[i].text += "[color=ff0000]";
//				}
//			
//				LogicUI.materialNum[i].text += Convert.ToString(bagMaterialNum);
//				LogicUI.materialNum[i].text += "/";
//				LogicUI.materialNum[i].text += Convert.ToString(curCfg.MaterialNum[i]);
//				
//				if(curCfg.GameMoney == 0)
//				{
//					LogicUI.MoneySprite.gameObject.SetActive(false);
//					LogicUI.Money.text	= "";
//				}
//				else
//				{
//					LogicUI.MoneySprite.gameObject.SetActive(true);
//					LogicUI.Money.text	= Convert.ToString(curCfg.GameMoney);
//					
//					long GameMoney = XLogicWorld.SP.MainPlayer.GameMoney;					
//					if(GameMoney < (long)curCfg.GameMoney)
//					{
//						isCanStrengthen	= false;
//					}
//				}
//				
//				//设置按钮字体状态
//				LogicUI.BtnStrengthen.isEnabled	= isCanStrengthen;
//				LogicUI.BtnStrengthen.UpdateImage();
//				
//				long RealMoney = XLogicWorld.SP.MainPlayer.RealMoney;
//				LogicUI.BtnDirectStrengthen.isEnabled = (RealMoney >= (long)curCfg.RealMoney);
//				LogicUI.BtnDirectStrengthen.UpdateImage();
//			}
//		}
//		else// if(op == OP_ITEM.OP_ITEM_INLAY)			
//		{	
//			bool isHasGem = false;
//			for(int i = 0; i < 3; i++)
//			{
//				LogicUI.InlayGemIcon[i].Label		= LogicUI.FreeGemLabel[i];
//				LogicUI.InlayGemIcon[i].Btn			= LogicUI.mUIInlayBtn[i];
//
//				
//				if(i < curCfg.SlotNum)
//				{
//					LogicUI.UnLockSprite[i].gameObject.SetActive(false);
//					LogicUI.mUIInlayBtn[i].gameObject.SetActive(true);
//					if(item.mEquipAttr.mGemID[i] > 0)
//					{
//						XCfgItem cfgGemItem = XCfgItemMgr.SP.GetConfig(item.mEquipAttr.mGemID[i]);
//						if(cfgItem == null)
//							continue;
//						XCfgAttrID cfgGemAttr = XCfgAttrIDMgr.SP.GetConfig(cfgGemItem.GemAttrType);
//						if(cfgGemAttr == null)
//							continue;
//						
//						isHasGem	= true;
//						
//						//显示上部宝石图标
//						XUIDynamicAtlas.SP.SetSprite(LogicUI.mGemSprite[i],(int)cfgGemItem.IconAtlasID,cfgGemItem.IconID);
//						LogicUI.mGemLabel[i].text	= XGameColorDefine.Quality_Color[cfgGemItem.QualityLevel];
//						LogicUI.mGemLabel[i].text	+= cfgGemItem.Name;
//						LogicUI.mGemLabel[i].text	+= "  ";
//						LogicUI.mGemLabel[i].text	+= "[color=00ff00]" + cfgGemAttr.EffectDes;
//						LogicUI.mGemLabel[i].text	+= Convert.ToString(cfgGemItem.GemAttrValue);
//						
//						//显示下部宝石Icon
//						LogicUI.InlayGemIcon[i].SetSprite(cfgGemItem.IconAtlasID,cfgGemItem.IconID,(EItem_Quality)cfgGemItem.QualityLevel,1);
//						LogicUI.InlayGemIcon[i].IsHasItem	= true;
//						LogicUI.InlayGemIcon[i].IsNewSet	= false;
//						LogicUI.InlayGemIcon[i].DataID		= item.mEquipAttr.mGemID[i];
//						
//						LogicUI.FreeGemLabel[i].gameObject.SetActive(false);
//						
//						LogicUI.mUIInlayBtn[i].isEnabled = true;
//						LogicUI.mUIInlayBtn[i].UpdateImage();
//						UILabel label = LogicUI.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
//						if(label == null)
//							continue;
//						label.text	= "摘除";
//					}
//					else
//					{		
//						LogicUI.mGemSprite[i].atlas			= LogicUI.mUIComAtlas;
//						LogicUI.mGemSprite[i].spriteName	= LogicUI.mGemBKName;
//						LogicUI.mGemLabel[i].text			= "未镶嵌宝石";
//					
//						LogicUI.InlayGemIcon[i].Reset();
//						LogicUI.InlayGemIcon[i].IsHasItem	= false;
//						LogicUI.InlayGemIcon[i].IsNewSet	= false;
//						LogicUI.InlayGemIcon[i].DataID		= 0;
//						LogicUI.FreeGemLabel[i].gameObject.SetActive(true);
//						
//						LogicUI.mUIInlayBtn[i].isEnabled = false;
//						LogicUI.mUIInlayBtn[i].UpdateImage();
//						UILabel label = LogicUI.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
//						if(label == null)
//							continue;
//						label.text	= "镶嵌";
//					}
//				}
//				else
//				{
//					LogicUI.mGemSprite[i].atlas			= LogicUI.mUIComAtlas;
//					LogicUI.mGemSprite[i].spriteName	= LogicUI.mGemBKName;
//					LogicUI.mGemLabel[i].text			= "强化后开启";
//					
//					LogicUI.InlayGemIcon[i].Reset();
//					LogicUI.InlayGemIcon[i].IsHasItem	= false;
//					LogicUI.InlayGemIcon[i].DataID		= 0;
//					LogicUI.InlayGemIcon[i].IsNewSet	= false;
//					
//					LogicUI.mUIInlayBtn[i].gameObject.SetActive(false);
//					LogicUI.UnLockSprite[i].gameObject.SetActive(true);
//					
//					LogicUI.FreeGemLabel[i].gameObject.SetActive(false);
//					
////					UILabel label = LogicUI.mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
////					if(label == null)
////						continue;
////					label.text	= "镶嵌";
//				}
//				
//			}
//			
//			for(int i = 0; i < 3;i++)
//			{			
//				LogicUI.InlayGemIcon[i].NeedStrengthenItem	= item;
//			}
//			
//			if(isHasGem)
//			{
//				LogicUI.mUIInlayBtn[3].isEnabled	= true;
//				LogicUI.mUIInlayBtn[3].UpdateImage();
//			}
//			else
//			{
//				LogicUI.mUIInlayBtn[3].isEnabled	= false;
//				LogicUI.mUIInlayBtn[3].UpdateImage();
//			}
//				
//		}
//	}	
//	private void _ShowContainer(EItemBoxType type)
//	{		
//		LogicUI.ClearStrengthenInfo();
//		LogicUI.StrItemMgr.ClearState();
//		LogicUI.ClearStrengthenItem();
//		LogicUI.ClearInlayInfo();
//		LogicUI.ShowContainerBtnState(0);
//		ShowOpUI(opItem);
//		//清空Icon数据
//		for(int i = 0;i < XStrengthenWindow.SHOW_ICON_NUM;i++)
//		{
//			LogicUI.StrItemMgr.ItemList[i].mStrengthenIcon.SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,0);
//			LogicUI.StrItemMgr.ItemName[i].text = "";
//		}
//		
//		LogicUI.PlayerName.text	= XLogicWorld.SP.MainPlayer.Name;
//		
//		XLogicWorld.SP.MainPlayer.ItemManager.GetAllEquip(type,mAllEquip);
//		if(mAllEquip.Count == 0)
//			return ;
//		
//		if(mAllEquip.Count <= XStrengthenWindow.SHOW_ICON_NUM)
//		{
//			LogicUI.ItemUp.gameObject.SetActive(false);
//			LogicUI.ItemDown.gameObject.SetActive(false);
//		}
//		else
//		{
//			LogicUI.ItemUp.gameObject.SetActive(true);
//			LogicUI.ItemDown.gameObject.SetActive(true);
//		}
//		
//		for(int i =0; i < mAllEquip.Count && i < XStrengthenWindow.SHOW_ICON_NUM; i++)
//		{
//			XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(mAllEquip[i]);
//			if(tempItem == null || tempItem.IsEmpty())				
//				continue;
//				
//			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
//			if(cfgItem == null)
//				continue;
//	
//			LogicUI.StrItemMgr.ItemList[i].mStrengthenIcon.SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,(uint)tempItem.DataID);
//			LogicUI.StrItemMgr.ItemName[i].text = cfgItem.Name;
//			LogicUI.StrItemMgr.mActionIconList[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount);
//			CurShowItemNum++;
//		}
//				
////		// 新手引导
////		if ( mAllEquip.Count > 0 )
////		{
////			Vector3 showPos = new Vector3(Mathf.Round(LogicUI.StrItemMgr.ItemList[0].mStrengthenIcon.mUIIcon.transform.localPosition.x - 130),
////				Mathf.Round(LogicUI.StrItemMgr.ItemList[0].mStrengthenIcon.mUIIcon.transform.localPosition.y + 12),
////				Mathf.Round(LogicUI.StrItemMgr.ItemList[0].mStrengthenIcon.mUIIcon.transform.localPosition.z));
////			XNewPlayerGuideManager.SP.handleShowGuideExt(16, 1, showPos, 
////				LogicUI.StrItemMgr.ItemList[0].mStrengthenIcon.mUIIcon.transform.parent.gameObject);
////		}
//			
//		_UpdateIndex(CurSelItemGUID);
//	}
//	
//	private void ShowOpUI(OP_ITEM opItem)
//	{		
//		this.opItem = opItem;
//		if(opItem == OP_ITEM.OP_ITEM_INLAY)
//		{
//			LogicUI.ShowInlay();
//		}
//		else
//		{
//			LogicUI.ShowStrengthen();
//		}
//	}
}
