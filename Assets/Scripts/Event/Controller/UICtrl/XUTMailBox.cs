using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

class XUTMailBox : XUICtrlTemplate<XMailBox>
{
	
	public XUTMailBox()
	{
		RegEventAgent_CheckCreated(EEvent.Mail_WriteMail, WriteMail);
		RegEventAgent_CheckCreated(EEvent.Mail_UpdateMailBox, UpdateAllMail);
		RegEventAgent_CheckCreated(EEvent.Mail_Oper, OperMail);
	}
	
	void WriteMail(EEvent evt, params object[] args)
	{
		LogicUI.UpdateAll();
	}
	
	void UpdateAllMail(EEvent evt, params object[] args)
	{
		int mailCnt = 0;
		foreach(  KeyValuePair<XMailManager.XMailInfo, XMailManager.XMailInfo> item in XMailManager.listMail  )
		{
			XMailManager.XMailInfo tinfo  = item.Value;
			if ( LogicUI.currentShowType == 0 )
				mailCnt++;
			else if ( LogicUI.currentShowType == 1 && tinfo.m_read == 1 )
				mailCnt++;
			else if ( LogicUI.currentShowType == 2 && tinfo.m_read == 2 )
				mailCnt++;
		}

		if (LogicUI.currentPage >= (int)Mathf.Ceil((float)mailCnt / (float)5))
		{
			LogicUI.currentPage--;
		}
		
		LogicUI.UpdateAll();
	}
	
	void OperMail(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		uint id = (uint)args[0];
		uint result = (uint)args[1];
		if ( 1u == result )
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(257));
			LogicUI.DeleteMail(id);
		}
		if ( 2u == result )
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1074));
		}
	}
}

