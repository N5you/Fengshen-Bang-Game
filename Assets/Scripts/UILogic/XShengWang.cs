using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XShengWang")]
public class XShengWang : XDefaultFrame
{
	// ui
	public UISpriteGroup SpriteGroup;
	public GameObject		m_RootItemPanel;
	public UIGrid				m_ItemGroupList;
	public GameObject		m_SelfShW;
	//public XShopItem[]    m_OneGroupShopItem;	
	public GameObject		TemplateGo;
	
	public static readonly int MAX_GROUP_ITEM_NUM = 3;	
	
	// DATA
	private ArrayList m_GameGroupList = new ArrayList();
	private uint m_iShwLvl;
	private uint m_iShwValue;
	
	// date
	// 当前可显示的所有数据(下一级之前的所有数据)
	private ArrayList m_CurrentBuyItemList = new ArrayList();
	
	// 比较参数
	 private class XCfgShengWangItemCompare : System.Collections.IComparer
	 {
            public int Compare(object x, object y)
            {
                return (int)(((XCfgShengWangItem)x).Sequencs - ((XCfgShengWangItem)y).Sequencs);
            }
      }
	
	public override bool Init()
	{
		base.Init();
		
		SpriteGroup.mModify	= OnSelectModify;
		
		UIEventListener ls = UIEventListener.Get(m_RootItemPanel);
		ls.onClick	= ClickGO;
		return true;
	}
	
	public void OnSelectModify(int index)
	{
		if(index == 1)
		{
			m_RootItemPanel.SetActive(false);
		}
		else
		{
			m_RootItemPanel.SetActive(true);
		}
	}
	
	public  void ClickGO (GameObject go)
	{
		//OnInitShengWang();
	}
	
	public override void Show()
	{
		base.Show();
		
		OnInitShengWang();
	}
	
	//
	public void OnInitShengWang()
	{
		LoadItemInfo();
		ShowAllItemInfo();
	}
	
	// 
	public void LoadItemInfo()
	{
		m_CurrentBuyItemList.Clear();
		
		m_iShwValue =  XLogicWorld.SP.GetMainPlayer().ShengWangValue;
		m_iShwLvl = XLogicWorld.SP.GetMainPlayer().GetShengWangLvl();
		XCfgShengWangItem cfgShengWangItem = null;
		SortedList<uint, XCfgShengWangItem>  ItemTable = XCfgShengWangItemMgr.SP.ItemTable;;
		if(ItemTable == null) return;
		foreach(KeyValuePair<uint, XCfgShengWangItem> kvpItem in ItemTable)
		{
			cfgShengWangItem = kvpItem.Value;
			if(m_iShwLvl > cfgShengWangItem.PrestigeLvShow)
			{
				m_CurrentBuyItemList.Add(cfgShengWangItem);
			}
		}
		XCfgShengWangItemCompare	   cfgShengWangItemCompare= new XCfgShengWangItemCompare();
		m_CurrentBuyItemList.Sort(cfgShengWangItemCompare);
	}
	
	// 
	public void ShowAllItemInfo()
	{
		m_SelfShW.GetComponent<UILabel>().text = m_iShwValue.ToString() + "(LVL" + m_iShwLvl.ToString() + ")";
		HideAllItem();
		foreach(GameObject info in m_GameGroupList)
		{
			NGUITools.Destroy(info);
		}
		m_GameGroupList.Clear();
		
		uint uCount = 0;
		GameObject oneGroupShopItem = null;
		GameObject tempGroup = null;
		XShopItem shopItem = null;
		
		foreach(XCfgShengWangItem cfgShengWangItem in m_CurrentBuyItemList)
		{
			if(uCount % MAX_GROUP_ITEM_NUM == 0)
			{	
				tempGroup = XUtil.Instantiate(TemplateGo,m_ItemGroupList.gameObject.transform,Vector3.zero,Vector3.zero);	
				tempGroup.SetActive(true);
				m_GameGroupList.Add (tempGroup);
			}
			XShopItem[] shopItemArray = tempGroup.GetComponentsInChildren<XShopItem>(true);
			if(uCount % MAX_GROUP_ITEM_NUM >= shopItemArray.Length)
			{
				Log.Write(LogLevel.ERROR,"ShowAllItemInfo too Long");
				continue;
			}
			shopItem = shopItemArray[uCount % MAX_GROUP_ITEM_NUM];
			
			shopItem.setShengWangItemLogic(cfgShengWangItem.ItemID);
			uCount++;
		}
		
		m_ItemGroupList.repositionNow	= true;
		
	}
	
	public void HideAllItem()
	{
		//TemplateGo.SetActive(false);
		/*XShopItem shopItem = null;
		foreach(GameObject info in m_GameGroupList)
		{
			for(int i = 0; i < 3; i++)
			{
				shopItem = info.GetComponentsInChildren<XShopItem>()[i];
				shopItem.gameObject.SetActive(false);
			}
		}*/
	}
	
	
}
