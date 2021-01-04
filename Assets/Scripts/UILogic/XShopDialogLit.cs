using UnityEngine;
using System.Collections;

public class XShopDialogLit : MonoBehaviour {
	public GameObject m_totalPrice = null;
	public GameObject m_singlePrice = null;
	public GameObject m_MaxBtn = null;
	public XActionIcon m_itemIcon = null;
	public GameObject m_itemPriceIcon = null;
	public GameObject m_yesBtn = null;
	public GameObject m_noBtn = null;
	public GameObject m_itemName = null;
	
	private XShopDialog m_owner = null;
	private uint m_itemID = 0;
	private uint m_npcID = 0;
	
	public XShopDialog owner{
		set{ m_owner = value; }
	}
	
	// Use this for initialization
	void Start () {
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void showBuyItem(uint itemID,uint npcID)
	{
		gameObject.SetActive(true);
	}
	
	
}
