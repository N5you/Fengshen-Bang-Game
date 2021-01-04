using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using System;

class XUTChatWindow : XUICtrlTemplate<XChatWindow>
{
    public XUTChatWindow()
    {
        
        XEventManager.SP.AddHandler(OrderChat, EEvent.Chat_OrderChat);
        XEventManager.SP.AddHandler(ToggleChat, EEvent.Chat_ToggleChat);
        XEventManager.SP.AddHandler(SendChatMsg, EEvent.Chat_SendChatMsg);
        XEventManager.SP.AddHandler(OnSetChatItemData, EEvent.Chat_SetChatItemData);
        XEventManager.SP.AddHandler(OnChatNotice, EEvent.Chat_Notice);
		XEventManager.SP.AddHandler(OnSendOpenPrivateChateReq, EEvent.Chat_OpenPrivate);
        XEventManager.SP.AddHandler(SetCommand, EEvent.Chat_SetChatData);
		XEventManager.SP.AddHandler(handleShowPlayerInfoReq, EEvent.Chat_ShowPlayerInfoReq);
		XEventManager.SP.AddHandler(handleShowPlayerInfo, EEvent.Chat_ShowPlayerInfo);
		
		RegEventAgent_CheckCreated(EEvent.Chat_HideBiaoQingSelUI, handleHideBiaoQingSelUI);
		RegEventAgent_CheckCreated(EEvent.Chat_OnChatMsg, OnChatMsg);
    }

    private void OnChatMsg(EEvent evt, params object[] args)
    {
        if (null == LogicUI)
            return;
        LogicUI.On_SC_Chat((SC_Chat)args[0]);
    }
	
	private void OnSendOpenPrivateChateReq(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		CS_Chat.Builder chatMsg = CS_Chat.CreateBuilder();
		chatMsg.SetChatType(EChatType.eChatPlayer_OpenPrivateChat);
		chatMsg.SetTargetName((string)args[0]);
		chatMsg.SetTargetId((ulong)args[1]);
		chatMsg.SetChatData("");
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Chat, chatMsg.Build());
	}

    private void OnChatNotice(EEvent evt, params object[] args)
    {
        if (null == LogicUI)
            return;

        SC_Chat.Builder chatMsg = SC_Chat.CreateBuilder();
        chatMsg.SetChatType(EChatType.eChatSystem_Tip);
        chatMsg.SetChatData((string)args[0]);
        LogicUI.On_SC_Chat(chatMsg.Build());
    }

    private void OnSetChatItemData(EEvent evt, params object[] args)
    {
        if (args.Length < 1)
            return;

        LogicUI.On_Set_ItemData((XItem)args[0]);
		LogicUI.OrderChat();
    }

    private void OrderChat(EEvent evt, params object[] args)
    {
        if (null == LogicUI) return;
        LogicUI.OrderChat();
    }

    private void ToggleChat(EEvent evt, params object[] args)
    {
        XHardWareGate.SP.LockKeyBoard = (bool)args[0];
    }

    private void SendChatMsg(EEvent evt, params object[] args)
    {
		// 四个参数分别为 需要发送的数据，发送的类型，目标玩家的名称，目标玩家的id
		if ( args.Length < 2 )
			return;
        string data = args[0] as string;
		
		if ( null == data || data.Trim().Length <= 0 )
		{
			XEventManager.SP.SendEvent(EEvent.Chat_Notice, XStringManager.SP.GetString(1045));
			return;
		}
		if ( data.Length >= Define.MAX_CHAT_DATA_LEN )
		{
			XEventManager.SP.SendEvent(EEvent.Chat_Notice, XStringManager.SP.GetString(1041));
			return;
		}
        if ( XClientGM.SP.DoCmd(data) )
       		return;
		
		CS_Chat.Builder builder = CS_Chat.CreateBuilder();
		builder.SetChatData(data);
        builder.SetChatType((EChatType)args[1]);
		
		if ( args.Length >= 3 )
		{
			string targetName = (string)args[2];
			if ( null != targetName && targetName.Length > 0 )
				builder.SetTargetName(targetName);
		}
		if ( args.Length >= 4 )
		{
			ulong targid = (ulong)args[3];
			if ( targid > 0 )
				builder.SetTargetId((ulong)targid);
		}
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Chat, builder.Build());
    }

    private void SetCommand(EEvent evt, params object[] args)
    {
        if (null == LogicUI) 
			return;
        LogicUI.InputChat.text = (string)args[0];
        LogicUI.OrderChat();
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGMWindow);
    }
	
	private void handleShowPlayerInfoReq(EEvent evt, params object[] args)
	{
		CS_String.Builder msg = CS_String .CreateBuilder();
		msg.SetData((string)args[0]);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_GetPlayerInfo, msg.Build());
	}
	
	private void handleShowPlayerInfo(EEvent evt, params object[] args)
	{
		SC_PlayerInfo msg = (SC_PlayerInfo)args[0];
	
		XMainPlayer player = new XMainPlayer(msg.BaseInfo.ObjectId);
		player.InitRoleInfo(msg);
		
		XUIRoleInformation.CurrPlayer = player;
		XUIRoleInformation.bShowCurrentUser = false;
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eRoleInformation);
	}
	
	private void handleHideBiaoQingSelUI(EEvent evt, params object[] args)
	{
		LogicUI.HideBiaoQingUI();
	}
}

