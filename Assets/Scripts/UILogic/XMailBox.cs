using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;


[AddComponentMenu("UILogic/XMailBox")]
public class XMailBox : XDefaultFrame
{
	
	public static readonly int MAX_MAIL_CNT_ONE_PAGE = 5;	
	
	public GameObject 		ButtonAllDelete;
	public GameObject		ButtonAllGet;
	public GameObject		ButtonLastPage;
	public GameObject 		ButtonNextPage;
	public GameObject		ButtonShowNotRead;
	public GameObject		ButtonShowAll;
	public GameObject		ButtonShowRead;
	
	public GameObject[]		MailItem = new GameObject[MAX_MAIL_CNT_ONE_PAGE];
	
	public XActionIcon[]	arrayItemIcon = new XActionIcon[MAX_MAIL_CNT_ONE_PAGE];
	public UIImageButton[]	arrayButtonBG = new UIImageButton[MAX_MAIL_CNT_ONE_PAGE];
	public UICheckbox[]		arrayCheckDel = new UICheckbox[MAX_MAIL_CNT_ONE_PAGE];
	public UIImageButton[]	arrayButtonDel = new UIImageButton[MAX_MAIL_CNT_ONE_PAGE];
	public UILabel[]		arrayTitle = new UILabel[MAX_MAIL_CNT_ONE_PAGE];
	public UILabel[]		arraySender = new UILabel[MAX_MAIL_CNT_ONE_PAGE];
	public UILabel[]		arrayLeftTime = new UILabel[MAX_MAIL_CNT_ONE_PAGE];
	public UILabel[]		arrayMailType = new UILabel[MAX_MAIL_CNT_ONE_PAGE];
	public UILabel[]		arrayDesc1 = new UILabel[MAX_MAIL_CNT_ONE_PAGE];
	public XBaseActionIcon[] 	arrayItemIconLogic = new XBaseActionIcon[MAX_MAIL_CNT_ONE_PAGE];
	
	public GameObject[]	MoneyItemObject = new GameObject[MAX_MAIL_CNT_ONE_PAGE];
	public UISprite[]	MoneySpriteObject = new UISprite[MAX_MAIL_CNT_ONE_PAGE];
	
	public UILabel			LabelPage;
	
	public int 				currentPage = 0;
	
	public int 				currentShowType = 1;
	
	public List<int>		listMailSelected = new List<int>();
	
	public int				readyToDeleteMailId = 0;
	
	public static bool isActive = false;
	
	public override bool Init()
	{
        base.Init();

		UIEventListener.Get(ButtonAllDelete).onClick = OnClickAllDelete ;
		UIEventListener.Get(ButtonAllGet).onClick = OnClickAllGet ;
		UIEventListener.Get(ButtonLastPage).onClick = OnClickLastPage ;
		UIEventListener.Get(ButtonNextPage).onClick = OnClickNextPage ;
		UIEventListener.Get(ButtonShowNotRead).onClick += OnClickShowNotRead ;
		UIEventListener.Get(ButtonShowAll).onClick += OnClickShowAll ;
		UIEventListener.Get(ButtonShowRead).onClick += OnClickShowRead ;
		
		for (int i = 0 ; i < MAX_MAIL_CNT_ONE_PAGE ; i++)
		{
			UIEventListener.Get(arrayButtonBG[i].gameObject).onClick = OnClickOpenMail;
		}
		
		for (int i = 0 ; i < MAX_MAIL_CNT_ONE_PAGE ; i++)
		{
			UIEventListener.Get(arrayCheckDel[i].gameObject).onClick = OnClickReadyDelete;
		}
		
		for (int i = 0 ; i < MAX_MAIL_CNT_ONE_PAGE ; i++)
		{
			UIEventListener.Get(arrayButtonDel[i].gameObject).onClick = OnClickDeleteMail;
		}
		
		for (int i = 0 ; i < MAX_MAIL_CNT_ONE_PAGE ; i++)
		{
			arrayItemIconLogic[i] = new XBaseActionIcon();
			arrayItemIconLogic[i].SetUIIcon(arrayItemIcon[i]);
			arrayItemIconLogic[i].IsCanPopMenu = false;
		}
		currentShowType = 1;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eMailBox);
	}
	
	public override void Show()
	{
		isActive = true;
		base.Show();
	}
	
	public override void Hide()
	{
		isActive = false;
		base.Hide();
	}
	
	
	private void OnClickDeleteMail(GameObject obj)
	{
		UIImageButton btn = obj.GetComponentInChildren<UIImageButton >(); 
		readyToDeleteMailId = btn.userdata;
		
		XMailManager.XMailInfo info = new XMailManager.XMailInfo((uint)readyToDeleteMailId);
		if ( !XMailManager.listMail.ContainsKey(info) )
		{
			return;
		}
		XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
		if (tmpinfo.listItems.Count > 0 || tmpinfo.m_money > 0)
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(MsgBoxDeleteMail);
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString(263));
		}
		else
		{
			DeleteMail((uint)readyToDeleteMailId);
		}
		return;
	}
	
	private void MsgBoxDeleteMail(GameObject go)
	{
		DeleteMail((uint)readyToDeleteMailId);
	}
	
	public void DeleteMail(uint id)
	{
		XMailManager.XMailInfo info = new XMailManager.XMailInfo(id);
		if ( !XMailManager.listMail.ContainsKey(info) )
		{
			return;
		}
		XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
		XMailManager.listMail.Remove(tmpinfo);
		// MAIL DELETE
		uint[] arrDelId = new uint[1];
		arrDelId[0] = (uint)id;
		XLogicWorld.SP.NetManager.SendCommonArray<uint>((int)CS_Protocol.eCS_MailDeleteMail, arrDelId);
		readyToDeleteMailId = 0;
		
		int mailCnt = 0;
		foreach(  KeyValuePair<XMailManager.XMailInfo, XMailManager.XMailInfo> item in XMailManager.listMail  )
		{
			XMailManager.XMailInfo tinfo  = item.Value;
			if ( currentShowType == 0 )
				mailCnt++;
			else if ( currentShowType == 1 && tinfo.m_read == 1 )
				mailCnt++;
			else if ( currentShowType == 2 && tinfo.m_read == 2 )
				mailCnt++;
		}
		
		if (currentPage >= (int)Mathf.Ceil((float)mailCnt / (float)MAX_MAIL_CNT_ONE_PAGE))
		{
			currentPage--;
		}
		
		UpdateAll();
	}
	
	private void OnClickAllDelete(GameObject obj)
	{
		int mailItemCount = 0;
		foreach (int id in listMailSelected)
		{
			XMailManager.XMailInfo info = new XMailManager.XMailInfo((uint)id);
			if ( !XMailManager.listMail.ContainsKey(info) )
			{
				continue;
			}
			XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
			if ( tmpinfo.listItems.Count > 0 || tmpinfo.m_money > 0u )
				mailItemCount++;
		}
		
		if ( mailItemCount > 0 )
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(MsgDeleteAllMail);
			string showstr = string.Format(XStringManager.SP.GetString(73), mailItemCount);
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, showstr);		
		}
	}
	
	private void MsgDeleteAllMail(GameObject obj)
	{
		uint[] arrDelId = new uint[listMailSelected.Count];
		
		int index = 0;
		foreach (int id in listMailSelected)
		{
			arrDelId[index] = (uint)id;
			
			XMailManager.XMailInfo info = new XMailManager.XMailInfo((uint)id);
			if ( !XMailManager.listMail.ContainsKey(info) )
			{
				continue;
			}
			XMailManager.XMailInfo tmpinfo  = XMailManager.listMail[info];
			XMailManager.listMail.Remove(tmpinfo);
			index++;
		}
		currentPage = 0;
		listMailSelected.Clear();
		UpdateAll();
		
		// MAIL DELETE
		XLogicWorld.SP.NetManager.SendCommonArray<uint>((int)CS_Protocol.eCS_MailDeleteMail, arrDelId);
	}
	
	private void OnClickAllGet(GameObject obj)
	{	
		// MAIL GET_ITEM
	   	CS_MailGetItem.Builder builder = CS_MailGetItem.CreateBuilder();
	    builder.MailId = 0;
	    XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_MailGetItem, builder.Build());
		
		//UpdateAll();
	}
	
	private void OnClickOpenMail(GameObject obj)
	{
		UIImageButton btn = obj.GetComponentInChildren<UIImageButton >(); 
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eMail);
		XEventManager.SP.SendEvent(EEvent.Mail_UpdateMailDetail, btn.userdata);
	}
	
	private void OnClickReadyDelete(GameObject obj)
	{
		UICheckbox check = obj.GetComponentInChildren<UICheckbox>();
		if (check.isChecked)
		{
			listMailSelected.Add(check.userdata);
		}
		else
		{
			listMailSelected.Remove(check.userdata);
		}
	}
	
	private void OnClickLastPage(GameObject obj)
	{
		currentPage--;
		
		if (currentPage <= 0)
		{
			currentPage = 0;
		}
		
		UpdateAll();
	}
	
	private void OnClickNextPage(GameObject obj)
	{
		currentPage++;
		
		int mailCnt = 0;
		foreach(  KeyValuePair<XMailManager.XMailInfo, XMailManager.XMailInfo> item in XMailManager.listMail  )
		{
			XMailManager.XMailInfo tinfo  = item.Value;
			if ( currentShowType == 0 )
				mailCnt++;
			else if ( currentShowType == 1 && tinfo.m_read == 1 )
				mailCnt++;
			else if ( currentShowType == 2 && tinfo.m_read == 2 )
				mailCnt++;
		}
		
//		List<XMailManager.XMailInfo> list = new List<XMailManager.XMailInfo>();
//		
//		if (currentShowType == 0)
//		{
//			list = XMailManager.listMail;
//		}
//		else if (currentShowType == 1)
//		{
//			foreach (XMailManager.XMailInfo info in XMailManager.listMail)
//			{
//				if (info.m_read == 1)
//				{
//					list.Add(info);
//				}
//			}
//		}
//		else if (currentShowType == 2)
//		{
//			foreach (XMailManager.XMailInfo info in XMailManager.listMail)
//			{
//				if (info.m_read == 2)
//				{
//					list.Add(info);
//				}
//			}
//		}
//		
//		int mailCnt = list.Count;
		
		if (currentPage >= (int)Mathf.Ceil((float)mailCnt / (float)MAX_MAIL_CNT_ONE_PAGE))
		{
			currentPage--;
		}
		
		UpdateAll();
	}
	
	private void OnClickShowAll(GameObject obj)
	{
		listMailSelected.Clear();
		currentPage = 0;
		currentShowType = 0;
		UpdateAll();
	}
	
	private void OnClickShowNotRead(GameObject obj)
	{
		listMailSelected.Clear();
		currentPage = 0;
		currentShowType = 1;
		UpdateAll();
	}
	
	private void OnClickShowRead(GameObject obj)
	{
		listMailSelected.Clear();
		currentPage = 0;
		currentShowType = 2;
		UpdateAll();
	}
	
	public void ClearAll()
	{
		for (int i = 0 ; i < MAX_MAIL_CNT_ONE_PAGE ; i++)
		{
			arrayCheckDel[i].isChecked = false;
			MailItem[i].SetActive(false);
			arrayItemIconLogic[i].ResetUIAndLogic();
		}
		
	}
	
	public void UpdateAll()
	{
		ClearAll();
		
		System.Collections.Generic.SortedList<XMailManager.XMailInfo, XMailManager.XMailInfo> list = new System.Collections.Generic.SortedList<XMailManager.XMailInfo, XMailManager.XMailInfo>(new XMailManager.MailSort());
		foreach(  KeyValuePair<XMailManager.XMailInfo, XMailManager.XMailInfo> item in XMailManager.listMail  )
		{
			XMailManager.XMailInfo tinfo  = item.Value;
			if ( currentShowType == 0 )
				list[tinfo] = tinfo;
			else if ( currentShowType == 1 && tinfo.m_read == 1 )
				list[tinfo] = tinfo;
			else if ( currentShowType == 2 && tinfo.m_read == 2 )
				list[tinfo] = tinfo;
		}
		
//		List<XMailManager.XMailInfo> list = new List<XMailManager.XMailInfo>();
//		
//		if (currentShowType == 0)
//		{
//			list = XMailManager.listMail;
//		}
//		else if (currentShowType == 1)
//		{
//			foreach (XMailManager.XMailInfo info in XMailManager.listMail)
//			{
//				if (info.m_read == 1)
//				{
//					list.Add(info);
//				}
//			}
//		}
//		else if (currentShowType == 2)
//		{
//			foreach (XMailManager.XMailInfo info in XMailManager.listMail)
//			{
//				if (info.m_read == 2)
//				{
//					list.Add(info);
//				}
//			}			
//		}

		int mailCnt = list.Count;
		
		int totalPage = (int)Mathf.Ceil((float)mailCnt / (float)MAX_MAIL_CNT_ONE_PAGE);
		LabelPage.text = (currentPage+1).ToString() + "/" + totalPage.ToString();
		if (mailCnt == 0)
		{
			LabelPage.text = "0/0";
		}
		
		if ( currentPage < 0 )
		{
			currentPage = 0;
		}
		
		for (int i = currentPage * MAX_MAIL_CNT_ONE_PAGE, index = 0 ; i < currentPage * MAX_MAIL_CNT_ONE_PAGE + 5 ; i++ ) 
		{
			if (i >= list.Count)
			{
				break;
			}
			
			XMailManager.XMailInfo info = list.Values[i];;
			if (info == null)
			{
				break;
			}
			
			if (listMailSelected.Contains((int)info.m_id))
			{
				arrayCheckDel[index].isChecked = true;
			}
			
			MailItem[index].SetActive(true);
			arrayTitle[index].text = info.m_title;
			arraySender[index].text = info.m_sender;
			//arrayLeftTime[index].text = info.m_time.ToString();
			arrayButtonBG[index].userdata = (int)info.m_id;
			arrayCheckDel[index].userdata = (int)info.m_id;
			arrayButtonDel[index].userdata = (int)info.m_id;
			
			//time
			TimeSpan ts = DateTime.Now - new DateTime(1970,1,1,8,0,0);
			uint time = (uint)(ts.TotalMilliseconds / 1000);
			uint timeoffset = 30 * 24 * 3600 - (time - info.m_time);
			if (timeoffset < 60)
			{
				arrayLeftTime[index].text = timeoffset.ToString() + XStringManager.SP.GetString(75);
			}
			else if (timeoffset >= 60 && timeoffset < 3600)
			{
				uint leftMin = timeoffset / 60;
				if ( (timeoffset % 60) > 0u )
					leftMin++;
				arrayLeftTime[index].text = leftMin.ToString() + XStringManager.SP.GetString(76);
			}
			else if (timeoffset >= 3600 && timeoffset < 3600 * 24)
			{
				uint leftHour = timeoffset / 3600;
				if ( (leftHour % 3600) > 0u )
					leftHour++;
				arrayLeftTime[index].text = leftHour.ToString() + XStringManager.SP.GetString(77);
			}
			else 
			{
				uint leftDay = timeoffset / (3600 * 24);
				if ( (leftDay % (3600 * 24)) > 0u )
					leftDay++;
				arrayLeftTime[index].text = leftDay.ToString() + XStringManager.SP.GetString(78);
			}
			
			if ( info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_AUCTION_SUCCESS )
			{
				MoneyItemObject[index].SetActive(true);
				arrayItemIcon[index].gameObject.SetActive(false);
				
				if ( info.m_read == 2 )
				{
					MoneySpriteObject[index].alpha = 0.4f;
				}
				else
				{
					MoneySpriteObject[index].alpha = 1f;
				}
			}
			else if ( info.listItems.Count > 0 )
			{
				arrayItemIcon[index].gameObject.SetActive(true);
				MoneyItemObject[index].SetActive(false);
				XItem item = info.listItems[0];
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				if(cfgItem == null)
					continue;
				
				arrayItemIcon[index].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID, item.Color, item.ItemCount);
				if ( info.m_read == 2 )
				{
					arrayItemIcon[index].ItemIcon.alpha = 0.4f;
					arrayItemIcon[index].IconBK.gameObject.SetActive(false);
				}
				uint dataid = (info.m_id << 16);
				arrayItemIconLogic[index].SetLogicData(ActionIcon_Type.ActionIcon_Mail, dataid);
			}
			else
			{
				arrayItemIcon[index].gameObject.SetActive(true);
				MoneyItemObject[index].SetActive(false);
				if (info.m_read == 1)
					arrayItemIcon[index].IconBK.spriteName = "11000108";
				else if (info.m_read == 2)
					arrayItemIcon[index].IconBK.spriteName = "11000109";
				arrayItemIcon[index].IconBK.ResetSize();
			}
			
			Color color;
			if (info.m_read == 1)
			{
				color = new Color(161.0f/255.0f, 198.0f/255.0f, 237.0f/255.0f, 1.0f);
			}
			else
			{
				color = new Color(118.0f/255.0f, 120.0f/255.0f, 117.0f/255.0f, 1.0f);
			}
			
			arrayTitle[index].color = color;
			arraySender[index].color = color;
			arrayLeftTime[index].color = color;
			arrayDesc1[index].color = color;
			arrayMailType[index].color = color;
			if (info.m_read == 1)
			{
				arrayMailType[index].color = new Color(1.0f, 84.0f/255.0f, 0.0f, 1.0f);
				arrayLeftTime[index].color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
			}
			
			//邮件类型
			if ( (info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_AUCTION) ||
				(info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_AUCTION_SUCCESS) ||
				(info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_AUCTION_OFFLINE) )
			{
				arrayMailType[index].text = XStringManager.SP.GetString(79);
			}
			else if ( info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_GM )
			{
				arrayMailType[index].text = XStringManager.SP.GetString(80);
			}
			else if ( info.m_mailType == (uint)MAIL_TYPE.MAIL_TYPE_ARENA )
			{
				arrayMailType[index].text = XStringManager.SP.GetString(81);
			}
			
			index++;
		}
	}
}

