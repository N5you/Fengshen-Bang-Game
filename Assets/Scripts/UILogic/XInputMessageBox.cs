using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XInputMessageBox")]
public class XInputMessageBox : XMessageBox
{	
	public UIInput InputContent = null;
	public static string Content;
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener listen = UIEventListener.Get(ButtonConfirm.gameObject);
		listen.onClick += OnInputConfirm;
		
		return true;
		
	}
	
	private void OnInputConfirm(GameObject go)
	{
		Content	= InputContent.text;
	}
	
}
