using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XZhuangBeiTiShi")]
public class XZhuangBeiTiShi : XDefaultFrame
{
	public XActionIcon  	ActionIcon;
	public UILabel			LabelName;
	public UIImageButton	EquipBtn;
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener ls = UIEventListener.Get(ButtonExit.gameObject);
		ls.onClick	= ClickExit;
		
		UIEventListener Euqipls = UIEventListener.Get(EquipBtn.gameObject);
		Euqipls.onClick	= ClickEquip;
		
		return true;
	}
	
	public void ClickExit(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_MuliHide,PanelKey);
	}
	
	public void ClickEquip(GameObject go)
	{
		XUTZhuangBeiTiShi ctrl = XUIManager.SP.GetMuliUIObject(PanelKey) as XUTZhuangBeiTiShi;
		if(ctrl == null)
			return ;
		
		XCharacter newCh = XEquipGetMgr.SP.IsNeedHint(ctrl.LogicItem);
		if(newCh == null)
		{
			XEventManager.SP.SendEvent(EEvent.UI_MuliHide,PanelKey);
			return ;
		}
		
		ctrl.Char	= newCh;
		//Equip
		ushort realSrcIndex = ctrl.LogicItem.ItemIndex;
		EItemBoxType srcType;
		ushort srcIndex;
		XItemManager.GetContainerType((ushort)ctrl.LogicItem.ItemIndex,out srcType,out srcIndex);
		
		XCfgItem srcCfgItem = XCfgItemMgr.SP.GetConfig(ctrl.LogicItem.DataID);
		if(srcCfgItem == null)
			return ;					
			
		int equipPos = ctrl.Char.ItemManager.GetEquipPos((EQUIP_SLOT_TYPE)srcCfgItem.EquipPos);
		int itemIndex = XItemManager.GetRealItemIndex(ctrl.Char,equipPos);
				
		EItemBoxType targetType;
		ushort targetIndex;
		XItemManager.GetContainerType((ushort)itemIndex,out targetType,out targetIndex);
		XCharacter oldCh = XUIRoleInformation.CurRoInfoSelChar;
		XUIRoleInformation.CurRoInfoSelChar	= ctrl.Char;
		XItemManager.CharSwapItem(srcType,(ushort)srcIndex,targetType,targetIndex);
		XUIRoleInformation.CurRoInfoSelChar	= oldCh;
		
		XEventManager.SP.SendEvent(EEvent.UI_MuliHide,PanelKey);
		
	}
}
