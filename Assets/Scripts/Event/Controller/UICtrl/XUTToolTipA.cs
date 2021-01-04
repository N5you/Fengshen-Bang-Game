using UnityEngine;
using System.Collections;

class XUTToolTipA : XUICtrlTemplate<XToolTipA>
{
	private object[] args;
	
	public XUTToolTipA()
	{
		RegEventAgent_CheckCreated(EEvent.ToolTip_A, OnToolTip);
	}
	
	public void OnToolTip(EEvent evt, params object[] args)
	{
		int ItemIndex = (int)(args[0]);
		if(ItemIndex >= XToolTipA.ITEM_COUNT)
			return ;
		
		LogicUI.ItemList[ItemIndex].SetTitle((string)(args[1]));
		LogicUI.ItemList[ItemIndex].SetTipContent((string)(args[2]));
		bool isHasIcon = (bool)args[3];
		if(!isHasIcon)
		{
			LogicUI.ItemList[ItemIndex].ItemIcon.gameObject.SetActive(false);
			return ;
		}
		uint atlasID = (uint)args[4];
		string spriteName = (string)args[5];
		EItem_Quality	quality = (EItem_Quality)args[6];
		ushort strengthenLevel  = (ushort)args[7];
		LogicUI.ItemList[ItemIndex].ItemIcon.gameObject.SetActive(true);
		LogicUI.ItemList[ItemIndex].SetTipSprite(atlasID,spriteName,quality,strengthenLevel);
		
		if(ItemIndex == 0)
		{
			LogicUI.ItemList[1].root.gameObject.SetActive(false);
		}
		else
			LogicUI.ItemList[1].root.gameObject.SetActive(true);
	}
	
	public override void Breathe()
	{
		if(LogicUI == null || LogicUI.gameObject.activeSelf == false || !XToolTipA.updatePos )
			return ;

		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		
		float x = vec.x+10.5f;
		float y = vec.y - 12.5f;
		float z = LogicUI.transform.position.z;
		if ( y - XChatWindow.BottomYPos < LogicUI.ItemList[0].TipBackGround.transform.localScale.y )
		{
			y = XChatWindow.BottomYPos + LogicUI.ItemList[0].TipBackGround.transform.localScale.y;
		}
		
		float adjustPos = LogicUI.ItemList[0].TipBackGround.transform.localScale.x;
		if(LogicUI.ItemList[1].IsSecondItem)
		{				
			adjustPos = adjustPos * 2;
		}
		
		if ( XMissionTip.RightXpos - x < adjustPos )
		{
			x = XMissionTip.RightXpos - adjustPos;
		}
		LogicUI.transform.position = new Vector3(x, y, z);
		LogicUI.transform.localPosition = new Vector3(Mathf.RoundToInt(LogicUI.transform.localPosition.x), 
			Mathf.RoundToInt(LogicUI.transform.localPosition.y), 
			Mathf.RoundToInt(LogicUI.transform.localPosition.z));
	}
	
	
}

