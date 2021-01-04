using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using System.Text.RegularExpressions;

[AddComponentMenu("UILogic/XLoginUI")]
public class XLoginUI : XUIBaseLogic
{
	public GameObject 		ButtonComfirm;
	//public GameObject 		ButtonCancel;
	public UIInput  		InputAccount;
	public UIInput 			InputPassword;

	public override bool Init()
	{
        base.Init();

		UIEventListener listenerComfirm = UIEventListener.Get(ButtonComfirm);
		//UIEventListener listenerCancel = UIEventListener.Get(ButtonCancel);
		listenerComfirm.onClick = OnClickComfirm;
		//listenerCancel.onClick = OnClickCancel;
		
		return true;
	}

	private void OnClickComfirm(GameObject obj)
	{
		XEventManager.SP.SendEvent(EEvent.Login_CheckAccount, InputAccount.text, InputPassword.text);
	}
	
	private void OnClickCancel(GameObject obj)
	{
		
	}
}

