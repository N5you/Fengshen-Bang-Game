
using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTZhuangBeiTiShi : XUICtrlTemplate<XZhuangBeiTiShi>
{
	XBagActionIcon	ItemIcon = new XBagActionIcon();
	public XCharacter		Char;
	public XItem			LogicItem;
	
	public override EUIPanel GetPanelType() {return EUIPanel.eZhuangBeiTiShi; }
	
	public void SetItemData(XCharacter ch,XItem item)
	{
		Char 		= ch;
		LogicItem	= item;	
	}
	
	public override void OnShow()
	{
		ItemIcon.SetUIIcon(LogicUI.ActionIcon);
		
		ItemIcon.SetLogicData(ActionIcon_Type.ActionIcon_Bag,LogicItem.ItemIndex);
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
		if(cfgItem == null)
		{
			Log.Write(LogLevel.WARN,"not find Item Config ,DataID is {0}",LogicItem.DataID);
			return ;
		}
		
		ItemIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,LogicItem.Color,1,true,1);
		ItemIcon.IsCanToolTip	= true;
		
		LogicUI.LabelName.text = XGameColorDefine.Quality_Color[(int)LogicItem.Color] + cfgItem.Name;
	}
	
}
