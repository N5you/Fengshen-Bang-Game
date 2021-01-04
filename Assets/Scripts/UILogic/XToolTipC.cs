using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XToolTipC")]
public class XToolTipC : XUIBaseLogic
{
	public UILabel TipContent 		= null;	
	public UISprite TipBackGround 	= null;
	
	public override void Show()
	{
		// 在鼠标指针所在处显示
		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		vec.z = transform.position.z;
		transform.position = vec;
		
		VerifyLayout();
		base.Show();
	}
	
	public void SetTipContent(string strContent)
	{
		TipContent.text = strContent;
		
		VerifyLayout();
	}
	
	private void VerifyLayout()
	{
		// 校准TipBackGround大小
		Vector3 vec = TipBackGround.transform.localScale;
		
		vec.y = TipContent.transform.localScale.y * TipContent.relativeSize.y + 4;
		vec.x = Mathf.RoundToInt(TipContent.transform.localScale.x * TipContent.relativeSize.x) + 15;
		TipBackGround.transform.localScale = vec;
	}
}


