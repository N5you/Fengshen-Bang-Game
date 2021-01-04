using UnityEngine;
using XGame.Client.Packets;

class XUTMail : XUICtrlTemplate<XMail>
{
	public XUTMail()
	{
		//XEventManager.SP.AddHandler(ReplyMail, EEvent.Mail_Reply);
		//XEventManager.SP.AddHandler(DeleteMail, EEvent.Mail_Delete);
		RegEventAgent_CheckCreated(EEvent.Mail_UpdateMailDetail, UpdateMailDetail);
	}
	
	void ReplyMail(EEvent evt, params object[] args)
	{
		
	}
	
	void DeleteMail(EEvent evt, params object[] args)
	{
		
	}
	
	void UpdateMailDetail(EEvent evt, params object[] args)
	{
		int id = (int)args[0];
		
		XMailManager.XMailInfo info = new XMailManager.XMailInfo((uint)id);
		if ( !XMailManager.listMail.ContainsKey(info) )
		{
			return;
		}
		
		XMailManager.XMailInfo tmpInfo = XMailManager.listMail[info];
		LogicUI.UpdateMailDetail(tmpInfo);
   		if (tmpInfo.m_read == 1)
		{
			// MAIL READ
	   		CS_MailReadFlag.Builder builder = CS_MailReadFlag.CreateBuilder();
	    	builder.MailId = (uint)id;
	        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_MailReadFlag, builder.Build());
			tmpInfo.m_read = 2;		
			XEventManager.SP.SendEvent(EEvent.Mail_WriteMail);
		}
	}
}
