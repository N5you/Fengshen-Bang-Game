using System;
using UnityEngine;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

//物品相关的消息包接收和发送处理
public class XItemPacketProcess : XSingleton<XItemPacketProcess>
{	
	public XItemPacketProcess()
	{
		IsOpenedRole	= false;
		IsOpenedBag		= false;
		IsOpenedBank	= false;
	}
	public bool	IsOpenedRole	{get;set;}
	public bool	IsOpenedBag 	{get;set;}
	public bool	IsOpenedBank 	{get;set;}	
	
	#region Receive Process
	public void On_SC_ItemSpace(SC_ItemSpace msg)
    {
		int Length = msg.ItemSpaceCount;
		int[] array = new int[Length];
        for (int i = 0; i < Length; ++i)
        {
			array[i] =(int) msg.GetItemSpace(i);
        }
		
		XItemSpaceMgr.SP.Init(array);
		
		//刷新界面		
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);		
    }
	
	public void On_SC_ItemCombine(SC_ItemCombine msg)
	{
		//删除物品
		for(int i = 0;i < msg.ResultListCount;i++)
		{
			SC_ItemCombine.Types.Int_SingleResult res = msg.GetResultList(i);
			XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)res.ItemIndex,res.ItemNum);
		}
		//添加物品
		XLogicWorld.SP.MainPlayer.ItemManager.AddItem((ushort)msg.CombineItemIndex,msg.CombineItemDataID,1);
	}
	
	public void On_SC_ItemDrop(SC_ItemDrop msg)
	{
		uint index = msg.ItemIndex;
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(index);
		XCfgItem CfgDelItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(CfgDelItem == null)
			return ;
		string DelTemp = XGameColorDefine.Quality_Color[(int)item.Color] + CfgDelItem.Name;
		string Delcon = "[color=ffffff]";
		Delcon += string.Format(XStringManager.SP.GetString(57),DelTemp);
		XEventManager.SP.SendEvent(EEvent.Chat_Notice,Delcon);
		
		XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)index);
		
		//刷新界面
		XEventManager.SP.SendEvent(EEvent.charInfo_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		XEventManager.SP.SendEvent(EEvent.Bank_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Strengthen_Item_Update,EUIPanel.eStrengthenWindow);
	}
	
	public void On_SC_ItemUse(SC_UInt msg)
	{
		uint index = msg.Data;
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(index);
		XCfgItem CfgDelItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(CfgDelItem == null)
			return ;
		string DelTemp = XGameColorDefine.Quality_Color[(int)item.Color] + CfgDelItem.Name;
		string Delcon = "[color=ffffff]";
		Delcon += string.Format(XStringManager.SP.GetString(504),DelTemp);
		XEventManager.SP.SendEvent(EEvent.Chat_Notice,Delcon);
		
		XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)index);
		
		//刷新界面
		XEventManager.SP.SendEvent(EEvent.charInfo_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		XEventManager.SP.SendEvent(EEvent.Bank_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Strengthen_Item_Update,EUIPanel.eStrengthenWindow);
	}
	
	
	
	public void On_SC_ItemList(SC_ItemList msg)
    {
		for (int i = 0; i < msg.ItemListCount; ++i)
        {
			PB_ItemInfo pbInfo = msg.GetItemList(i); 
			XItem temp = new XItem();
			temp.InitFromPB(pbInfo);
			EItemBoxType type;
			ushort index;
			XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
			XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);
        }
		
		//刷新界面
		XEventManager.SP.SendEvent(EEvent.charInfo_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		XEventManager.SP.SendEvent(EEvent.Bank_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Strengthen_Item_Update,EUIPanel.eStrengthenWindow);
    }
	
	public void On_SC_ItemUpdate(SC_ItemUpdate msg)
	{
		XItem item = new XItem();
		item.InitFromPB( msg.ItemMsg );
		EItemBoxType type;
		
		ushort index;
		XItemManager.GetContainerType((ushort)msg.ItemMsg.Position,out type,out index);
		XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,item);
		
		XItem TargetItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((ushort)msg.ItemMsg.Position);
		if(!TargetItem.IsEmpty())
		{
			XItemManager.FlyItemAnimation(TargetItem);
			XEquipGetMgr.SP.GetNewEquip(TargetItem);
		}
		
		//刷新界面
		XEventManager.SP.SendEvent(EEvent.charInfo_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		XEventManager.SP.SendEvent(EEvent.Bank_Update,EUIPanel.eRoleInformation);
		XEventManager.SP.SendEvent(EEvent.Strengthen_Item_Update,EUIPanel.eStrengthenWindow);
	}
	
	public void On_SC_ItemComStrengthen(SC_ItemComStrengthen msg)
	{
		for(int i = 0;i < msg.ResultListCount;i++)
		{
			SC_ItemComStrengthen.Types.Int_SingleResult res = msg.GetResultList(i);
			XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)res.ItemIndex,res.ItemNum);
		}
		
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(msg.StrengthenItemIndex);
		if(item == null)
			return ;
		
		XCfgStrengthen  cfg = XItemManager.GetStrengthenCfg((uint)item.mEquipAttr.mColor,(byte)item.mEquipAttr.mStrengthenLevel);
		if(cfg == null)
			return ;
		
		XLogicWorld.SP.MainPlayer.GameMoney	-= cfg.GameMoney;
		
		if(msg.IsSuccess > 0)
		{
			string suStr = XStringManager.SP.GetString(151);
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCenterTip);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,suStr);
			item.mEquipAttr.mStrengthenLevel	= cfg.AfterLevel;
		}
		else
		{			
			string suStr = XStringManager.SP.GetString(152);
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCenterTip);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,suStr);
			item.mEquipAttr.mStrengthenLevel	= cfg.FailLevel;
			
		}
		
		//刷新界面
		//刷新显示效果
		EItemBoxType type;
		ushort index;
		XItemManager.GetContainerType(item.ItemIndex,out type,out index);
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		//武器的话
		if(cfgItem.EquipPos == (ushort)EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON)
		{
			if(type == EItemBoxType.Equip)
			{
				XLogicWorld.SP.MainPlayer.WeaponItemID	= cfgItem.Index;
			}
		}
		else if(cfgItem.EquipPos == (ushort)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR)
		{
			XCfgStrengthen  newCfg = XItemManager.GetStrengthenCfg((uint)item.mEquipAttr.mColor,(byte)item.mEquipAttr.mStrengthenLevel);
			if(newCfg == null)
				return ;
			
			//如果装备穿着的话
			if(type == EItemBoxType.Equip)
			{					
				XLogicWorld.SP.MainPlayer.ArmourItemID	= cfgItem.Index;
			}					
		}
		
		//刷新强化显示界面
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateData);
		XEventManager.SP.SendEvent(EEvent.Attr_Changed);
	}
	
	public void On_SC_ItemDecompose(SC_ItemDecompose msg)
	{
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(msg.ItemIndex);
		if(item == null)
			return ;
		
		XCfgItem CfgDelItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		string DelTemp = XGameColorDefine.Quality_Color[(int)item.Color] + CfgDelItem.Name;
		string Delcon = "[color=ffffff]";
		Delcon += string.Format(XStringManager.SP.GetString(56),DelTemp);
		XEventManager.SP.SendEvent(EEvent.Chat_Notice,Delcon);
		
		XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)msg.ItemIndex);
		
		for(int i = 0; i < msg.ResultListCount;i++)
		{
			SC_ItemDecompose.Types.Int_SingleResult res = msg.GetResultList(i);
			XItem TempItem = XLogicWorld.SP.MainPlayer.ItemManager.AddItem((ushort)res.ItemIndex,res.ItemData,(ushort)res.ItemNum);
			if(TempItem != null)
			{
				XCfgItem CfgItem = XCfgItemMgr.SP.GetConfig(TempItem.DataID);
				string temp = XGameColorDefine.Quality_Color[(int)TempItem.Color] + CfgItem.Name;
				string con = "[color=ffffff]";
				con += string.Format(XStringManager.SP.GetString(55),temp);
				XEventManager.SP.SendEvent(EEvent.Chat_Notice,con);
			}
			
		}	
	}
	
	
	public void On_SC_ItemMoneyStrengthen(SC_ItemMoneyStrengthen msg)
	{
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(msg.StrengthenItemIndex);
		if(item == null)
			return ;
		
		XCfgStrengthen  cfg = XItemManager.GetStrengthenCfg((uint)item.mEquipAttr.mColor,(byte)item.mEquipAttr.mStrengthenLevel);
		if(cfg == null)
			return ;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		EItemBoxType type;
		ushort index;
		XItemManager.GetContainerType(item.ItemIndex,out type,out index);
		
		//服务器自己会单独同步
		//XLogicWorld.SP.MainPlayer.RealMoney	-= cfg.RealMoney;
		if(msg.IsSuccess > 0)
		{
			string suStr = XStringManager.SP.GetString(151);
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCenterTip);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,suStr);
			item.mEquipAttr.mStrengthenLevel	= cfg.AfterLevel;
		}
		else
		{
			string suStr = XStringManager.SP.GetString(152);
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCenterTip);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,suStr);
			item.mEquipAttr.mStrengthenLevel	= cfg.FailLevel;
		}
		
		//武器的话
		if(cfgItem.EquipPos == (ushort)EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON)
		{
			if(type == EItemBoxType.Equip)
			{
				XLogicWorld.SP.MainPlayer.WeaponItemID	= cfgItem.Index;
			}
		}
		else if(cfgItem.EquipPos == (ushort)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR)
		{
			XCfgStrengthen  newCfg = XItemManager.GetStrengthenCfg((uint)item.mEquipAttr.mColor,(byte)item.mEquipAttr.mStrengthenLevel);
			if(newCfg == null)
				return ;
			
			//如果装备穿着的话
			if(type == EItemBoxType.Equip)
			{					
				XLogicWorld.SP.MainPlayer.ArmourItemID	= cfgItem.Index;
			}					
		}
		
		//刷新界面
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateData);
		XEventManager.SP.SendEvent(EEvent.Attr_Changed);
	}
	
	public void On_SC_ItemGemInlay(SC_ItemGemInlay msg)
	{
		ushort itemIndex 	= (ushort)msg.StrengthenItemIndex;
		ushort gemIndex		= (ushort)msg.GemIndex;
		byte	index		= (byte)msg.Index;
		
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(itemIndex);
		if(item == null)
			return ;
		XItem GemItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(gemIndex);
		if(GemItem == null)
			return ;
		item.AddGem(index,GemItem.DataID);
		XLogicWorld.SP.MainPlayer.ItemManager.DelItem(gemIndex,1);
		
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateUI);
	}
	
	public void On_SC_ItemGemRemove(SC_ItemGemRemove msg)
	{
		ushort itemIndex = (ushort)msg.StrengthenItemIndex;
		uint GemID	 = msg.GemID;
		ushort addGemIndex = (ushort)msg.AddGemIndex;
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(itemIndex);
		if(item == null)
			return ;
		
		XItem GemItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(addGemIndex);
		if(GemItem == null)
			return ;
		
		if(GemItem.IsEmpty())
		{
			XLogicWorld.SP.MainPlayer.ItemManager.AddItem(addGemIndex,GemID,1);
		}
		else
		{
			GemItem.ItemCount += 1;
		}			
		
		item.RemoveGem(GemID);
		
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateUI);
		
	}
	
	public void On_SC_ItemGemRemoveAll(SC_ItemGemRemoveAll msg)
	{
		ushort itemIndex = (ushort)msg.StrengthenItemIndex;
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(itemIndex);
		if(item == null)
			return ;
		
		for(int i = 0; i < msg.AddGemIDCount; i++)
		{
			XItem GemItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(msg.GetAddGemIndex(i));
			if(GemItem == null)
				return ;
			
			if(GemItem.IsEmpty())
			{
				XLogicWorld.SP.MainPlayer.ItemManager.AddItem((ushort)msg.GetAddGemIndex(i),msg.GetAddGemID(i),1);
			}
			else
			{
				GemItem.ItemCount += 1;
			}
		}
		
		item.RemoveGemAll();
		
		XEventManager.SP.SendEvent(EEvent.Strengthen_UpdateUI);
		
	}
	
	public void On_SC_ItemSeal(SC_ItemList msg)
	{
		for (int i = 0; i < msg.ItemListCount; ++i)
        {
			PB_ItemInfo pbInfo = msg.GetItemList(i); 
			XItem temp = new XItem();
			temp.InitFromPB(pbInfo);
			EItemBoxType type;
			ushort index;
			XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
			XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);
        }
		
		if(msg.ItemListCount > 0)
			XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(62));
		
		//刷新界面		
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
	}
	
	public void On_SC_ItemSplit(SC_ItemSplit msg)
	{
		ushort srcItemIndex = (ushort)msg.SrcItemIndex;
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(srcItemIndex);
		item.ItemCount	= (ushort)msg.SrcItemCount;
		PB_ItemInfo pbInfo	= msg.Info;
		XItem temp = new XItem();
		temp.InitFromPB(pbInfo);
		EItemBoxType type;
		ushort index;
		XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
		XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);	
		
		XMouseStateMgr.SP.mouseState	= EMouseState.EMouseState_None;
		CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
		XEventManager.SP.SendEvent(EEvent.Cursor_ClearIcon);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eCursor);
		XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,srcItemIndex);
		XEventManager.SP.SendEvent(EEvent.Bag_ItemUpdate,(ushort)pbInfo.Position);
		XEventManager.SP.SendEvent(EEvent.Bag_UpdateNum);
	}
	
	public void On_SC_ItemArrange(SC_ItemList msg)
	{
		XLogicWorld.SP.MainPlayer.ItemManager.ClearAllItemData(EItemBoxType.Bag);
		
		for (int i = 0; i < msg.ItemListCount; ++i)
        {
			PB_ItemInfo pbInfo = msg.GetItemList(i); 
			XItem temp = new XItem();
			temp.InitFromPB(pbInfo);
			EItemBoxType type;
			ushort index;
			XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
			XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);
        }
		
		//刷新界面		
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		
	}
	
	public void On_SC_ItemShow(PB_ItemInfo msg)
	{
		XItem item = new XItem();
		item.InitFromPB(msg);
		XItemTip.SetNeedUpdateTipPos(false);
		XItemTip.ShowItemTip(0,item);
	}
	
	#endregion
	
	#region Send Process
	public void SendItemCombine(ushort itemIndex)
	{
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,itemIndex);
		CS_ItemCombine.Builder builder = CS_ItemCombine.CreateBuilder();
		builder.SetItemIndex((uint)realIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemCombine, builder.Build());
	}
	
	public void SendItemDecompose(ushort itemIndex)
	{
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,itemIndex);
		CS_ItemDecompose.Builder builder = CS_ItemDecompose.CreateBuilder();
		builder.SetItemIndex((uint)realIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemDecompose, builder.Build());
	}
	
	
	public void SendItemDrop(ushort itemIndex)
	{
		ushort realIndex = XItemManager.GetItemIndex(EItemBoxType.Bag,itemIndex);
		CS_ItemDrop.Builder	builder = CS_ItemDrop.CreateBuilder();
		builder.SetItemIndex((uint)realIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemDrop, builder.Build());
	}
	
	public void SendItemComStrengthen(ushort itemIndex)
	{
		CS_ItemComStrengthen.Builder builder = CS_ItemComStrengthen.CreateBuilder();
		builder.SetItemIndex((uint)itemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemComStrengthen, builder.Build());
	}
	
	public void SendItemMoneyStrengthen(ushort itemIndex)
	{
		CS_ItemMoneyStrengthen.Builder builder = CS_ItemMoneyStrengthen.CreateBuilder();
		builder.SetItemIndex(itemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemMoneyStrengthen, builder.Build());
	}
	
	public void SendItemGemInlay(ushort itemIndex,ushort gemIndex,uint index)
	{
		CS_ItemGemInlay.Builder builder = CS_ItemGemInlay.CreateBuilder();
		builder.SetStrengthenItemIndex((uint)itemIndex);
		builder.SetGemIndex(gemIndex);
		builder.SetIndex(index);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemGemInlay, builder.Build());
	}
	
	public void SendItemGemRemove(ushort itemIndex,uint gemID)
	{
		CS_ItemGemRemove.Builder builder = CS_ItemGemRemove.CreateBuilder();
		builder.SetStrengthenItemIndex((uint)itemIndex);
		builder.SetGemID(gemID);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemGemRemove, builder.Build());
	}
	
	public void SendItemGemRemoveAll(ushort itemIndex)
	{
		CS_ItemGemRemoveAll.Builder builder = CS_ItemGemRemoveAll.CreateBuilder();
		builder.SetStrengthenItemIndex((uint)itemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemGemRemoveAll, builder.Build());
	}
	
	public void SendUseItem(ushort itemIndex)
	{
		CS_UseItem.Builder builder = CS_UseItem.CreateBuilder();
		builder.SetItemIndex((uint)itemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_UseItem, builder.Build());
	}
	
	public void SendItemSeal(ushort itemIndex)
	{
		CS_UInt.Builder	builder = CS_UInt.CreateBuilder();
		builder.SetData(itemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemSeal, builder.Build());
	}
	
	public void SendItemSplit(ushort srcItemIndex,int splitCount,ushort targetItemIndex)
	{
		CS_ItemSplit.Builder builder = CS_ItemSplit.CreateBuilder();
		builder.SetItemIndex((uint)srcItemIndex);
		builder.SetItemCount((uint)splitCount);
		builder.SetTargetIndex(targetItemIndex);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemSplit, builder.Build());
	}
	
	public void SendItemArrange()
	{
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemArrange, builder.Build());
	}
	#endregion

}
