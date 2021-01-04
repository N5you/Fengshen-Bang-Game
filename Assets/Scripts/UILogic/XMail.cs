using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;

[AddComponentMenu("UILogic/XMail")]
public class XMail : XDefaultFrame
{
	public GameObject	ButtonReply;
	public GameObject	ButtonDelete;
	
	public UILabel 		LabelTitle;
	public UILabel		LabelName;
	public UILabel		LabelTime;
	public UILabel		LabelContent;
	
	public XActionIcon[] arrayItemIcon = new XActionIcon[6];
	public XBaseActionIcon[]	arrayItemIconLogic = new XBaseActionIcon[6];
	
	public GameObject MoneyParent;
	public UIImageButton MoneyObject;
	
	public GameObject ItemParent;
	
	private XMailManager.XMailInfo m_info;
	
	public override bool Init()
	{
        base.Init();

		UIEventListener.Get(ButtonReply).onClick = OnClickReply ;
		UIEventListener.Get(ButtonDelete).onClick = OnClickDelete ;
		
		for (int i = 0 ; i < 6 ; i++)
		{
			arrayItemIconLogic[i] = new XBaseActionIcon();
			arrayItemIconLogic[i].SetUIIcon(arrayItemIcon[i]);
			arrayItemIconLogic[i].IsCanPopMenu = false;
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eMail);
	}
	
	private void OnClickReply(GameObject obj)
	{
		uint spaceNum = (uint)XLogicWorld.SP.MainPlayer.ItemManager.GetFullSpaceCount(EItemBoxType.Bag) - XLogicWorld.SP.MainPlayer.ItemManager.GetCurItemSpaceCount(EItemBoxType.Bag);
		MoneyParent.SetActive(false);
		
		if (m_info.listItems.Count == 0)
		{
			return;
		}
		
		if (m_info.listItems.Count <= spaceNum)
		{
			XMailManager.XMailInfo info = new XMailManager.XMailInfo(m_info.m_id);
			if ( !XMailManager.listMail.ContainsKey(info) )
			{
				return;
			}
			XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
			
			tmpinfo.listItems.Clear();
			// MAIL GET_ITEM
	        CS_MailGetItem.Builder builder = CS_MailGetItem.CreateBuilder();
	        builder.MailId = (uint)tmpinfo.m_id;
	        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_MailGetItem, builder.Build());
			XEventManager.SP.SendEvent(EEvent.Mail_UpdateMailBox);
			for (int i = 0 ; i < 6 ; i++)
			{
				arrayItemIconLogic[i].ResetUIAndLogic();
			}
			// delete type 
			if (m_info.m_deleteType == 2)
			{
				XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eMail);
			}
			return;
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1074));
		}
	}
	
	private void OnClickDelete(GameObject obj)
	{		
		XMailManager.XMailInfo info = new XMailManager.XMailInfo(m_info.m_id);
		if ( !XMailManager.listMail.ContainsKey(info) )
		{
			return;
		}
		XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
		if (tmpinfo.listItems.Count > 0 || tmpinfo.m_money > 0)
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(MsgDeleteMail);
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString(263));
		}
		else
		{
			MsgDeleteMail(obj);
		}
	}
	
	private void MsgDeleteMail(GameObject obj)
	{
		XMailManager.XMailInfo info = new XMailManager.XMailInfo(m_info.m_id);
		if ( !XMailManager.listMail.ContainsKey(info) )
		{
			return;
		}
		XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
		XMailManager.listMail.Remove(info);
		
		// MAIL DELETE
		XLogicWorld.SP.NetManager.SendCommonArray<uint>((int)CS_Protocol.eCS_MailDeleteMail, m_info.m_id);
		
		XEventManager.SP.SendEvent(EEvent.Mail_UpdateMailBox);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eMail);
		
	}
	
	public void UpdateMailDetail(XMailManager.XMailInfo info)
	{
		m_info = info;
		
		LabelTitle.text = info.m_title;
		LabelName.text = info.m_sender;
		if ( info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_AUCTION_SUCCESS )
		{
			if ( info.listItems.Count > 0 )
			{
				XItem item = info.listItems[0];
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				string showItem = "[color=00FFBB]" + string.Format(XStringManager.SP.GetString(1066), cfgItem.Name, item.ItemCount);
				string showMoney = "[color=00FFBB]" +  string.Format(XStringManager.SP.GetString(1067), info.m_money);
				LabelContent.text = "[color=A1C6ED]" + info.m_content + "\r\n" + showItem + "\r\n" + showMoney;
			}
		}
		else
		{
			LabelContent.text = "[color=A1C6ED]" + info.m_content;
		}
		//time
		DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(info.m_time);
		LabelTime.text  = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();
		for (int i = 0 ; i < 6 ; i++)
		{
			arrayItemIconLogic[i].ResetUIAndLogic();
		}
		if ( info.m_mailType != (uint)MAIL_TYPE.MAIL_TYPE_AUCTION_SUCCESS )
		{
			ItemParent.SetActive(true);
			MoneyParent.SetActive(false);
			for (int i = 0 ; i < info.listItems.Count ; i++)
			{
				arrayItemIconLogic[i].Reset();
				XItem item = info.listItems[i];
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				if(cfgItem == null)
				{
					continue;
				}
				uint dataid = (uint)((info.m_id << 16) | i);
				arrayItemIcon[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID, item.Color, item.ItemCount);
				arrayItemIconLogic[i].SetLogicData(ActionIcon_Type.ActionIcon_Mail, dataid);			
			}
		}  
		else
		{
			ItemParent.SetActive(false);
			MoneyParent.SetActive(true);
			string tipMoney = string.Format(XStringManager.SP.GetString(1049), info.m_money);
			MoneyObject.SetTipString(tipMoney);
		}
	}
	
}
