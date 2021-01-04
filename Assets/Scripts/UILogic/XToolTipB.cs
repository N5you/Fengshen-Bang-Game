using UnityEngine;
using System.Collections;

/* ToolTip样式A
 * 正文 + 背景图
 * 目前原点锁定在左上角
 */

[AddComponentMenu("UILogic/XToolTipB")]
public class XToolTipB : XUIBaseLogic
{
	public UILabel TipContent 		= null;
	public UILabel	TipName			= null;
	public UILabel	TipLevel 		= null;
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
	
	public void SetTipContent(string strName,string strLevel,string strContent)
	{
		TipName.text	= strName;
		TipLevel.text	= strLevel;
		TipContent.text = strContent;
		VerifyLayout();
	}
	
	private void VerifyLayout()
	{
		// 校准TipBackGround大小
		Vector3 vec = TipBackGround.transform.localScale;
		vec.y = TipContent.transform.localScale.y * TipContent.relativeSize.y + 35;
		//vec.x = Mathf.RoundToInt(TipContent.transform.localScale.x * TipContent.relativeSize.x);
		TipBackGround.transform.localScale = vec;
	}
}

