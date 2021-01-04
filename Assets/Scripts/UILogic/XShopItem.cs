using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

public class XShopItem : MonoBehaviour {
	
	public GameObject m_itemName = null;
	
	public GameObject m_itemPrice = null;
	
	public GameObject m_itemPriceIcon = null;
	
	public GameObject m_itemBackGround = null;
	
	public GameObject m_iShengWangLvl	= null;
	
	public XActionIcon m_itemActionIcon = null;
	
	private XShopActionIcon m_itemLogic = new XShopActionIcon();
	
	private XShopBackActionIcon m_itemBuyBackLogic = new XShopBackActionIcon();
	
	// Use this for initialization
	void Start () {
		m_itemLogic.IsCanPopMenu = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public bool setBuyItemLogic( uint npcID, uint itemID )
	{
		//m_itemLogic.SetUIIcon(m_itemActionIcon);
		
		XCfgItem itemBase = XCfgItemMgr.SP.GetConfig(itemID);
		if(itemBase == null)
			return false;
		XShopItemMgr.ShopItemPriceMsg priceMsg;
		XShopItemMgr.SP.resolveShopItemPriceMsg(npcID,itemID,out priceMsg );
		
		//price icon change
		m_itemPriceIcon.GetComponent<UISprite>().spriteName = XShopItemMgr.SP.resolvePriceTypeUISpriteName(priceMsg.m_priceType);
		//price number
		m_itemPrice.GetComponent<UILabel>().text = priceMsg.m_price.ToString();
		//item Name
		m_itemName.GetComponent<UILabel>().text = XGameColorDefine.Quality_Color[itemBase.QualityLevel]+itemBase.Name;
		
		m_itemLogic.SetLogicDataAndIcon(m_itemActionIcon, ActionIcon_Type.ActionIcon_Shop,(int)itemID,itemID );
		return true;
	}
	
	public void setBuyBackItemLogic(int index, int iCount)
	{
		m_itemBuyBackLogic.SetUIIcon(m_itemActionIcon);
		
		uint itemID = XShopItemMgr.SP.getBuyBackItem(index).DataID;
		// 设置icon图片
		m_itemBuyBackLogic.SetLogicDataAndIcon(m_itemActionIcon, ActionIcon_Type.ActionIcon_ShopBuyBack, index, itemID, iCount);
		
		XCfgItem itemBase = XCfgItemMgr.SP.GetConfig(itemID);
		if(itemBase == null)
			return;
		
		XShopItemMgr.ShopItemPriceMsg priceMsg;
		XShopItemMgr.SP.resolveShopItemBackPriceMsg(itemID, out priceMsg);
		//price icon change
		m_itemPriceIcon.GetComponent<UISprite>().spriteName = XShopItemMgr.SP.resolvePriceTypeUISpriteName(priceMsg.m_priceType);
		//price number
		m_itemPrice.GetComponent<UILabel>().text = priceMsg.m_price.ToString();
		//item Name
		m_itemName.GetComponent<UILabel>().text = XGameColorDefine.Quality_Color[itemBase.QualityLevel]+itemBase.Name;
	}
	
	public void setShengWangItemLogic(uint iItemId)
	{
		//m_itemLogic.SetUIIcon(m_itemActionIcon);
		
		XCfgItem itemBase = XCfgItemMgr.SP.GetConfig(iItemId);
		if(itemBase == null)
			return;
		
		m_itemLogic.SetLogicDataAndIcon(m_itemActionIcon, ActionIcon_Type.ActionIcon_SWShop, (int)iItemId, iItemId);
		
		XShopItemMgr.ShopItemPriceMsg priceMsg;
		XShopItemMgr.SP.resolveShengWangItemPriceMsg( iItemId, out priceMsg );
		
		//price icon change
		m_itemPriceIcon.GetComponent<UISprite>().spriteName = XShopItemMgr.SP.resolvePriceTypeUISpriteName(priceMsg.m_priceType);
		//price number
		m_itemPrice.GetComponent<UILabel>().text = priceMsg.m_price.ToString();
		//item Name
		m_itemName.GetComponent<UILabel>().text = XGameColorDefine.Quality_Color[itemBase.QualityLevel]+itemBase.Name;
		m_iShengWangLvl.GetComponent<UILabel>().text = "LVL" + priceMsg.m_uLvl.ToString();
	}
	
}
