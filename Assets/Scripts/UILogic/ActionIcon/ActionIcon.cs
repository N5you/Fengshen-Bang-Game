using UnityEngine;

//游戏中所有的actionIcon类型定义，前几项与EItemBoxType一致，免去转换
public enum ActionIcon_Type
{
	ActionIcon_ComDef	=-1,
	ActionIcon_Bag		= 0,
	ActionIcon_Equip	= 1,
	ActionIcon_Bank		= 2,
	ActionIcon_PetEquip = 3,
	ActionIcon_Strengthen=4,
	ActionIcon_Trade	= 5,
	ActionIcon_SubSceneAwardSel	= 6,
	ActionIcon_Show     = 7,
	ActionIcon_Shop		= 8,
	ActionIcon_ShopBuyBack = 9,	
	ActionIcon_Auction	= 10,
	ActionIcon_Mail		= 11,
	ActionIcon_SWShop 	= 12,// 声望
	ActionIcon_Sel		= 13,
	ActionIcon_Num,
}

public class XBaseActionIcon
{
	public XActionIcon 	mUIIcon;
	public ActionIcon_Type 	IconType;
	public int 				IconData;
	public int				ObjectIndex; //0 MainPlayer,1--20 Pet
	
	public bool	IsCanDrag 		{get;set;}
	public bool IsCanPopMenu 	{get;set;}
	public bool IsCanToolTip 	{get;set;}
	public bool IsCanDrop		{get;set;}
	public bool IsCanDBClick	{get;set;}
	
	public XBaseActionIcon()
	{
		mUIIcon	= null;
		IconType= ActionIcon_Type.ActionIcon_ComDef;
		IconData= 0;
		
		IsCanDrag	= true;
		IsCanPopMenu= true;
		IsCanToolTip= true;
		IsCanDrop	= true;
		IsCanDBClick = false;
	}	
	
	#region wrap UI Op
	public void Reset()
	{
		if(mUIIcon)
			mUIIcon.Reset();
	}
	
	public void ResetDragAlpha()
	{
		if(null!=mUIIcon)
		{
			mUIIcon.DragReset();
		}
	}
	
	public void ResetUIAndLogic()
	{
		Reset();
		IconType= ActionIcon_Type.ActionIcon_ComDef;
		IconData= 0;
	}
	
	public void EnableEffect(bool isEnable)
	{
		if(null!=mUIIcon)
		{
			mUIIcon.EnableEffect(isEnable);
		}
	}
		
	public void SetSprite(uint uAtlasId, string strSpriteName,EItem_Quality quality,ushort num)
	{
		SetSprite(uAtlasId,strSpriteName,quality,num,true);	
	}
	
	public void SetSprite(uint uAtlasId, string strSpriteName,EItem_Quality quality,ushort num,bool isAddCollider)
	{
		if(mUIIcon)
			mUIIcon.SetSprite(uAtlasId,strSpriteName,quality,num,isAddCollider,1);
	}
	
	public void SetSprite(uint uAtlasId, string strSpriteName,EItem_Quality quality,ushort num,bool isAddCollider,ushort strengthenLevel)
	{
		if(mUIIcon)
			mUIIcon.SetSprite(uAtlasId,strSpriteName,quality,num,isAddCollider,strengthenLevel);
	}
	
	public void ShowUnOpen(bool isShow)
	{
		if(mUIIcon)
			mUIIcon.ShowUnOpen(isShow);
	}
	
	public void SetVisible(bool isVisible)
	{
		if(mUIIcon)
			mUIIcon.gameObject.SetActive(isVisible);
	}
	
//	public void Hide()
//	{
//		
//	}
	
	#endregion
	
	#region virtual Func
	public virtual void ClickIcon(GameObject _go)
	{
		if (UICamera.currentTouchID == -1)
		{			
			if(mUIIcon.IsChange())
			{
				XUIPopMenu.isOper = true;
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePopMenu );
				XEventManager.SP.SendEvent(EEvent.PopMenu_Data,EUIPanel.ePopMenu,IconType,IconData );
			}
			else
			{
				XEventManager.SP.SendEvent(EEvent.Bag_ItemSpace,IconType,IconData);
			}
		}
	}
	
	public virtual void DbClickIcon(GameObject _go)
	{
	}
	
	public virtual void HoverIcon(GameObject go, bool state)
	{
		mUIIcon.HoverShow(state);
		if(state && IsCanToolTip)
		{
			if(IconType == ActionIcon_Type.ActionIcon_Bag || IconType == ActionIcon_Type.ActionIcon_Equip 
				|| IconType == ActionIcon_Type.ActionIcon_Auction || IconType == ActionIcon_Type.ActionIcon_PetEquip)
				XItemTip.ShowXItemTip((EItemBoxType)IconType,(uint)IconData,ObjectIndex);
			else if(IconType == ActionIcon_Type.ActionIcon_ShopBuyBack)
				XItemTip.ShowShopItemTip((uint)IconData);
			else if(IconType == ActionIcon_Type.ActionIcon_Mail )
				XItemTip.ShowMailItemTip((uint)IconData);
			else
				XItemTip.ShowItemTip(0,new XItem((uint)IconData));
		}			
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipA);
		}
	}
	
 	public virtual void DropIcon(GameObject go, GameObject draggedObject)
	{
		
	}
	public virtual void OnStartDragIcon(GameObject go, Vector2 delta)
	{
		if(mUIIcon != null)
			mUIIcon.DragShow();
	}
		
	public void SetUIIcon(XActionIcon icon)
	{
		mUIIcon	= icon;
		Init();
	}	
		
	public void SetLogicData(ActionIcon_Type type , int index)
	{
		IconType	= type;
		IconData	= index;
		
		ObjectIndex	= 0;
	}
	
	public void SetLogicData(ActionIcon_Type type , uint index)
	{
		SetLogicData(type,(int)index );
	}
	
	public void SetLogicDataAndIcon( XActionIcon  uiIcon, ActionIcon_Type type , int index,uint itemID, int iCount = 0)
	{
		SetUIIcon(uiIcon);
		IconType	= type;
		IconData	= index;
		ObjectIndex	= 0;
		
		if(null!=mUIIcon)
		{
			mUIIcon.SetSpriteByID(itemID);
			if(iCount > 0)
				mUIIcon.IconNum.text = iCount.ToString();
		}
	}
	
	public void SetLogicData(ActionIcon_Type type , int index,int OIndex)
	{
		IconType	= type;
		IconData	= index;
		ObjectIndex = OIndex;
	}
	
	public void Init()
	{
		if(mUIIcon == null)
			return ;
		UIEventListener Listener1 = UIEventListener.Get(mUIIcon.gameObject);
		Listener1.onClick 	= OnClickIcon;
		//Listener1.onHover	= OnHoverIcon;
		Listener1.onToolTip = OnToolTipIcon;
		Listener1.onDrop	= OnDropIcon;
		Listener1.onDrag	= OnDragIcon;
		Listener1.onDoubleClick = OnDbClickIcon;
		
	}	
	
	public void OnDbClickIcon(GameObject _go)
	{
		if ( !IsCanDBClick )
			return;
		
		XDragMgr.SP.IconType	= IconType;
		XDragMgr.SP.IconData	= (ushort)IconData;
		
		DbClickIcon(_go);
	}
	
	public void OnClickIcon(GameObject _go)
	{
		if(!IsCanPopMenu)
			return ;
		
		ClickIcon(_go);
	}
	
	public void OnToolTipIcon(GameObject go, bool state)
	{
		if(!IsCanToolTip)
			return;
		
		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		if(go.GetComponent<Collider>() != null)
		{
			Vector2 scale = new Vector2(mUIIcon.ItemIcon.transform.localScale.x,mUIIcon.ItemIcon.transform.localScale.y);
			Vector2 relativeSize = new Vector2(mUIIcon.ItemIcon.relativeSize.x * scale.x,mUIIcon.ItemIcon.relativeSize.y * scale.y);
			vec = mUIIcon.ItemIcon.cachedTransform.position;
			vec	= new Vector3(vec.x + relativeSize.x / 2,vec.y + relativeSize.y / 2,vec.z);
		}

		XToolTipA.pos	= vec;
		
		HoverIcon(go,state);
	}
	
	public void OnDropIcon(GameObject go, GameObject draggedObject)
	{
		if(!IsCanDrop)
			return ;
		
		if (XDragMgr.SP.IsDraging)
		{
			XDragMgr.SP.IsDraging = false;			
			XEventManager.SP.SendEvent(EEvent.Cursor_ClearIcon,EUIPanel.eCursor);
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eCursor);

			DropIcon(go,draggedObject);
			
			if(!UICamera.currentTouch.DropIsDeal)
			{
				//XEventManager.SP.SendEvent(EEvent.PopMenu_Drop, EUIPanel.ePopMenu);
				XUIPopMenu popMenu = XUIManager.SP.GetUIControl(EUIPanel.ePopMenu) as XUIPopMenu;
				if(popMenu == null)
					return ;
				popMenu.OnGetPopMenuData(EEvent.PopMenu_Data,EUIPanel.ePopMenu,IconType,IconData );
				popMenu.OnPopMenuDrop(EEvent.PopMenu_Drop,null);
				
			}
		}
	}
	
	public void OnDragIcon(GameObject go, Vector2 delta)
	{
		if(!IsCanDrag)
			return ;
		
		if (UICamera.currentTouchID > -2)
		{
			if (!XDragMgr.SP.IsDraging)
			{
				XDragMgr.SP.IsDraging  	= true;
				XDragMgr.SP.IconType	= IconType;
				XDragMgr.SP.IconData	= (ushort)IconData;
				if(mUIIcon.IsChange())
				{
					XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eCursor);
					XEventManager.SP.SendEvent(EEvent.Cursor_UpdateIcon,EUIPanel.eCursor,mUIIcon);
					OnStartDragIcon(go,delta);
				}
			}
		}
	}
	#endregion 
}

public class XBagActionIcon : XBaseActionIcon
{	
	public XBagActionIcon()
	{		
		IsCanDBClick = true;
	}
	
	public override void OnStartDragIcon(GameObject go, Vector2 delta)
	{
		if(XUIBagWindow.CurSelItemType != 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eCursor);
			XDragMgr.SP.IsDraging  	= false;
			XDragMgr.SP.IconType	= ActionIcon_Type.ActionIcon_ComDef;
			XDragMgr.SP.IconData	= 0;
		}
		else
		{
			if(mUIIcon != null)
				mUIIcon.DragShow();
		}
	}
	
	public override void ClickIcon(GameObject _go)
	{
		base.ClickIcon(_go);

		XMouseStateMgr.SP.Click((ushort)IconData);
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			if(mUIIcon.IsChange())
			{
				XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)IconType,(short)IconData);
				if(!item.IsEmpty())
					XEventManager.SP.SendEvent(EEvent.Chat_SetChatItemData,item);
			}			
		}
	}
	
	public override void DbClickIcon(GameObject _go)
	{
		base.DbClickIcon(_go);
		if ( XAuctionUI.AuctionShow )
		{
			 XEventManager.SP.SendEvent(EEvent.auction_DragPublish, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, XDragMgr.SP.IconData);
		}
		
		//直接使用物品
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)IconType, (short)IconData);
		
		if(item == null || item.IsEmpty())
			return ;
		
		XItemManager.UseItem(item);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePopMenu);
	}
	
	public override void DropIcon(GameObject go, GameObject draggedObject)
	{	
        if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bag)
			XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
        else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Equip || XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_PetEquip )
            XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
		else if(XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Shop || XDragMgr.SP.IconType ==  ActionIcon_Type.ActionIcon_SWShop)
		{
			XShopItemMgr.SP.requestBuyItemToBagIndex(XDragMgr.SP.IconData,IconData,  XDragMgr.SP.IconType);
			
			if(null!=XDragMgr.SP.SourceShopItem)
			{
				XDragMgr.SP.SourceShopItem.ResetDragAlpha();
				XDragMgr.SP.SourceShopItem = null;
			}
		}else if(XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_ShopBuyBack)
		{
			XShopItemMgr.SP.requestBuyBackItem(XDragMgr.SP.IconData,IconData );
			if(null!=XDragMgr.SP.SourceShopItem)
			{
				XDragMgr.SP.SourceShopItem.ResetDragAlpha();
				XDragMgr.SP.SourceShopItem = null;
			}
		}
		
		if(XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bag || 
			XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Equip ||
			XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_PetEquip|| 
			XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bank)
		{
			XItemManager.CharSwapItem((EItemBoxType)XDragMgr.SP.IconType,(ushort) XDragMgr.SP.IconData, (EItemBoxType)IconType, (ushort)IconData);			
		}
		
		if(XDragMgr.SP.IconType != IconType || XDragMgr.SP.IconData != IconData)
		{
			UICamera.currentTouch.DropIsDeal = true;
		}
	}	
}

public class XShopActionIcon : XBaseActionIcon
{	
	public XShopActionIcon()
	{
		
	}
	
	public override void ClickIcon(GameObject _go)
	{
		base.ClickIcon(_go);
		XMouseStateMgr.SP.Click((ushort)IconData);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePopMenu );
		XEventManager.SP.SendEvent(EEvent.PopMenu_BuyItem,(uint)(IconData) , IconType);
	}
	
	public override void DropIcon(GameObject go, GameObject draggedObject)
	{	
		restDragAlpha();
		
        if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Shop)
            XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
        else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_ShopBuyBack)
            XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, XDragMgr.SP.IconData);		
		
	}
	
	public override void OnStartDragIcon(GameObject go, Vector2 delta)
	{
		base.OnStartDragIcon(go,delta);
		
		XDragMgr.SP.IconType = IconType;
		XDragMgr.SP.IconData = (uint)IconData;
		XDragMgr.SP.SourceShopItem = this;
	}
	
	private void restDragAlpha()
	{
		if(null!=mUIIcon)
		{
			mUIIcon.DefaultAlpha = 1.0f;
			mUIIcon.DragReset();
		}
	}
	
}

public class XShopBackActionIcon : XBaseActionIcon
{	
	public XShopBackActionIcon()
	{
		
	}
	
	public override void ClickIcon(GameObject _go)
	{
		base.ClickIcon(_go);
		XMouseStateMgr.SP.Click((ushort)IconData);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePopMenu );
		XEventManager.SP.SendEvent(EEvent.PopMenu_BuyBackItem,(uint)IconData );
		
	}
	
	public override void DropIcon(GameObject go, GameObject draggedObject)
	{	
		restDragAlpha();
		
        if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Shop)
            XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
        else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_ShopBuyBack)
            XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, XDragMgr.SP.IconData);
	}
	
	public override void OnStartDragIcon(GameObject go, Vector2 delta)
	{
		base.OnStartDragIcon(go,delta);
		
		XDragMgr.SP.IconType = IconType;
		XDragMgr.SP.IconData = (uint)IconData;
		XDragMgr.SP.SourceShopItem = this;
	}
	
	private void restDragAlpha()
	{
		if(null!=mUIIcon)
		{
			mUIIcon.DefaultAlpha = 1.0f;
			mUIIcon.DragReset();
		}
	}
	
}


public class XRoleActionIcon : XBaseActionIcon
{
	public override void DropIcon(GameObject go, GameObject draggedObject)
	{
		//通知源对象图标变化
        if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bag)
            XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
        else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Equip || XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_PetEquip)
            XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
				
		//通知目标交换物品		
		XItemManager.CharSwapItem((EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData, (EItemBoxType)IconType, (ushort)IconData);
		
		UICamera.currentTouch.DropIsDeal	= true;
	}
	
	public override void ClickIcon(GameObject _go)
	{
		base.ClickIcon(_go);
		int equipPos = IconData % (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
		if(equipPos >= (int)EQUIP_POS.EQUIP_POS_FUYIN_START &&
			equipPos <= (int)EQUIP_POS.EQUIP_POS_FUYIN_END)
		{
			XUIRoleInformation roleInfo = XUIManager.SP.GetUIControl(EUIPanel.eRoleInformation) as XUIRoleInformation;
			XCharacter obj = roleInfo.GetCurSelObject();
			
			if(obj.Level < GlobalU3dDefine.FuYinOpenLevel[equipPos - (int)EQUIP_POS.EQUIP_POS_FUYIN_START])
				return ;
			XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)IconType,(short)IconData);
			if(item.IsEmpty())
			{
				XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eBagWindow);
			}
		}
	}
	
	public override void DbClickIcon(GameObject _go)
	{
		base.DbClickIcon(_go);
		
		//直接使用物品
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)IconType, (short)IconData);
		if(item.IsEmpty())
			return ;
		
		XItemManager.UseItem(item);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePopMenu);
		
	}
}

public class XAuctionSellIcon : XBaseActionIcon
{
    public XAuctionSellIcon()
    {
        IsCanDrag = false;
        IsCanPopMenu = false;
        IsCanDrop = true;
		IsCanDBClick = true;
    }

	public override void DropIcon(GameObject go, GameObject draggedObject)
	{
        if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bag ||
			XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Auction )
        {
            XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
            XEventManager.SP.SendEvent(EEvent.auction_DragPublish, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, XDragMgr.SP.IconData);
		}
        else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Equip || XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_PetEquip)
        {
            XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, XDragMgr.SP.IconData);
        }
	}
	
	public override void OnStartDragIcon(GameObject go, Vector2 delta)
	{
		base.OnStartDragIcon(go,delta);
		
		XDragMgr.SP.IconType = IconType;
		XDragMgr.SP.IconData = (uint)IconData;
	}
	
	public override void DbClickIcon(GameObject _go)
	{
		base.DbClickIcon(_go);
		if ( XAuctionUI.AuctionShow )
		{
			  XEventManager.SP.SendEvent(EEvent.auction_DeleteOnSoldItem);
		}
	}
}

public class XStrengthenActionIcon : XBaseActionIcon
{
	enum EINLAY_ERROR
	{
		EINLAY_ERROR_NONE,
		EINLAY_ERROR_NOT_GEM,
		EINLAY_ERROR_HAS_SAME,
		EINLAY_ERROR_LEVEL,
		EINLAY_ERROR_SLOT_NOT_OPEN,
		EINLAY_ERROR_HAS_GEM,
		EINLAY_ERROR_OTHER,
	}
	
	public XStrengthenActionIcon()
	{
		IsCanDrag	= false;
		IsCanPopMenu= false;
		IsCanDrop	= true;
	}
	
	public bool IsHasItem = false;
	public bool IsNewSet = false;
	public bool IsLock = true;
	public uint DataID	= 0;
	public uint ItemIndex = 0;
	public UILabel	Label;
	public UIImageButton	Btn;
	
	//想要强化的物品的装备使用等级
	public XItem NeedStrengthenItem {get;set;}
	
	public override void DropIcon(GameObject go, GameObject draggedObject)
	{
		EINLAY_ERROR error = IsCanPutIn(XDragMgr.SP.IconType);
		if(error != EINLAY_ERROR.EINLAY_ERROR_NONE)
		{
			if(error == EINLAY_ERROR.EINLAY_ERROR_HAS_SAME)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,28);
			}
			else if(error == EINLAY_ERROR.EINLAY_ERROR_LEVEL)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,29);
			}
			else if(error == EINLAY_ERROR.EINLAY_ERROR_NOT_GEM)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,30);
			}
			else if(error == EINLAY_ERROR.EINLAY_ERROR_SLOT_NOT_OPEN)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,31);
			}
			else if(error == EINLAY_ERROR.EINLAY_ERROR_HAS_GEM)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,32);
			}
			
			if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Bag)
				XEventManager.SP.SendEvent(EEvent.Bag_SetSprite, EUIPanel.eBagWindow, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
			else if (XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_Equip || XDragMgr.SP.IconType == ActionIcon_Type.ActionIcon_PetEquip )
            	XEventManager.SP.SendEvent(EEvent.charInfo_SetSprite, EUIPanel.eRoleInformation, (EItemBoxType)XDragMgr.SP.IconType, (ushort)XDragMgr.SP.IconData);
				
			return ;
		}
		
		//刷新图片
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)XDragMgr.SP.IconType,(short)XDragMgr.SP.IconData);
		if(item == null)
			return ;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		
		SetLogicData(ActionIcon_Type.ActionIcon_Strengthen,item.DataID);
		
		//mUIIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,item.ItemCount);
		mUIIcon.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,1);
		
		UICamera.currentTouch.DropIsDeal	= true;
		

		ItemIndex	= item.ItemIndex;
		DataID		= item.DataID;
		IsNewSet	= true;
		
		if(Label != null)
		{
			Label.gameObject.SetActive(false);
		}
		
		if(Btn != null)
		{
			Btn.isEnabled	= true;
			Btn.UpdateImage();
		}
		
		Vector3 showPos = new Vector3(Mathf.Round(Btn.transform.localPosition.x),
			Mathf.Round(Btn.transform.localPosition.y - 57),
			Mathf.Round(Btn.transform.localPosition.z));
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Drag);
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click, 3, showPos, 
				Btn.transform.parent.gameObject);
	}
	
	private EINLAY_ERROR IsCanPutIn(ActionIcon_Type type)
	{
		if(NeedStrengthenItem == null)
			return EINLAY_ERROR.EINLAY_ERROR_OTHER;
		
		bool isFromBag = type == ActionIcon_Type.ActionIcon_Bag;
		if(!isFromBag)
			return EINLAY_ERROR.EINLAY_ERROR_OTHER;
		
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)XDragMgr.SP.IconType,(short)XDragMgr.SP.IconData);
		if(item == null)
			return EINLAY_ERROR.EINLAY_ERROR_OTHER;
		
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return EINLAY_ERROR.EINLAY_ERROR_OTHER;
		
		if(IsLock)
			return EINLAY_ERROR.EINLAY_ERROR_SLOT_NOT_OPEN;
		
		if(IsHasItem)
			return EINLAY_ERROR.EINLAY_ERROR_HAS_GEM;
		
		//不是宝石
		if(cfgItem.GemType == 0)
			return EINLAY_ERROR.EINLAY_ERROR_NOT_GEM;
		
		XCfgItem strengthenCfgItem = XCfgItemMgr.SP.GetConfig(NeedStrengthenItem.DataID);
		if(strengthenCfgItem.ItemLevel < cfgItem.RequireEquipUseLevel)
			return EINLAY_ERROR.EINLAY_ERROR_LEVEL;
		
		//是否已经装备过了同种类型的宝石
		if(!NeedStrengthenItem.IsInlayGem(item.DataID))
			return EINLAY_ERROR.EINLAY_ERROR_HAS_SAME;
		
		return EINLAY_ERROR.EINLAY_ERROR_NONE;		
	}
}

public class XItemSelActionIcon : XBaseActionIcon
{
	
}
