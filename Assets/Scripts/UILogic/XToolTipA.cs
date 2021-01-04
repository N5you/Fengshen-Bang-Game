using UnityEngine;
using System.Collections;

/* ToolTip样式A
 * 正文 + 右上角一个图标 + 背景图
 * 目前原点锁定在左上角
 */
[AddComponentMenu("UILogic/XToolTipA")]
public class XToolTipA : XUIBaseLogic
{
	public static uint ITEM_COUNT = 2;
	[System.Serializable]
	public class OneItemTipData
	{
		public XToolTipA	Parent;
		public GameObject 	root;
		public UILabel 		TipContent;
		public UILabel 		TipTitle;
		public XActionIcon	ItemIcon;
		public UISprite 	TipBackGround;
		public UILabel		CurEquip;
		
		public bool		IsSecondItem = false;
		
		public void SetTipContent(string str)
		{
			TipContent.text = str;
			if(CurEquip != null)
			{
				IsSecondItem	= true;
				CurEquip.gameObject.SetActive(true);
			}
			Parent.VerifyLayout();			
		}	
		
		public void SetTitle(string str)
		{
			TipTitle.text	= str;
		}
		
		public void SetTipSprite(uint uAtlasId, string strSprite,EItem_Quality quality,ushort strengthenLevel)
		{
			ItemIcon.SetSprite(uAtlasId,strSprite,quality,1,false,strengthenLevel);
		}		
	}

	public static Vector3 pos;
	public static bool updatePos = true;
	
	public OneItemTipData[] ItemList = new OneItemTipData[ITEM_COUNT];
	
	public override void Show()
	{
		// 在鼠标指针所在处显示
		if ( updatePos )
		{
			Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
			transform.position = new Vector3(vec.x, vec.y, transform.position.z);
		}
		else
		{
			transform.position = new Vector3(pos.x, pos.y, transform.position.z);
		}
		
		VerifyLayout();
		base.Show();
	}
	
	public override void Hide()
	{
		updatePos = true;
		base.Hide();
	}
	
	public void VerifyLayout()
	{
		for(int i = 0; i < ITEM_COUNT; i++)
		{
			// 校准TipBackGround大小			
			Vector3 vec = ItemList[i].TipBackGround.transform.localScale;
			vec.y = ItemList[i].TipContent.transform.localScale.y * ItemList[i].TipContent.relativeSize.y + 70.0f;
			if(vec.y < 70.0f) vec.y = 70.0f;
			
			ItemList[i].TipBackGround.transform.localScale = vec;
		}
		
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		if ( transform.position.y - XChatWindow.BottomYPos < ItemList[0].TipBackGround.transform.localScale.y )
		{
			y = XChatWindow.BottomYPos + ItemList[0].TipBackGround.transform.localScale.y;
		}
		
		float adjustPos = ItemList[0].TipBackGround.transform.localScale.x;
		if(ItemList[1].IsSecondItem)
		{				
			adjustPos = adjustPos * 2;
		}
		
		if ( XMissionTip.RightXpos - transform.position.x <  adjustPos)
		{
			x = XMissionTip.RightXpos - adjustPos;
		}
		transform.position = new Vector3(x, y, z);
		transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), 
			Mathf.RoundToInt(transform.localPosition.y), 
			Mathf.RoundToInt(transform.localPosition.z));
		
		
	}
	
	
}
