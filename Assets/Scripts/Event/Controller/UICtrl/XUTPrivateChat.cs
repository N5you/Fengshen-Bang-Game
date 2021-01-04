using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using System;

class XUTPrivateChat : XUICtrlTemplate<XPrivateChatUI>
{
	public XUTPrivateChat()
	{
		RegEventAgent_CheckCreated(EEvent.Chat_SetPrivateChatData, OnSetPrivateChatData);
		RegEventAgent_CheckCreated(EEvent.Chat_PrivateUserChange, OnUserChange);
		RegEventAgent_CheckCreated(EEvent.Chat_OpenPrivateUI, OnOpernPrivateChatUI);
		RegEventAgent_CheckCreated(EEvent.Chat_HideBiaoQingSelUI, handleHideBiaoQingSelUI);
	}
			
	private void OnSetPrivateChatData(EEvent evt, params object[] args)
    {
		if ( args.Length < 6 )
			return;
    	LogicUI.ON_SC_SetChatData((string)args[0], (ulong)args[1],(int)args[2], (int)args[3], (string)args[4], (int)args[5]);
	}
	
	private void OnUserChange(EEvent evt, params object[] args)
	{
		if ( args.Length < 1 )
			return;
		
		LogicUI.On_UserChange((string)args[0]);
	}
	
	private void OnOpernPrivateChatUI(EEvent evt, params object[] args)
	{
		if ( args.Length < 4 )
			return;
		
		LogicUI.On_SC_OpenPrivateChat((string)args[0], (ulong)args[1],(int)args[2], (int)args[3]);
	}
	
	private void handleHideBiaoQingSelUI(EEvent evt, params object[] args)
	{
		LogicUI.HideBiaoQingUI();
	}
}
