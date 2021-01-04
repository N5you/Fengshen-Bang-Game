using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

enum  MAIL_TYPE
{
	MAIL_TYPE_PLAYER			= 1,		// 玩家邮件
	MAIL_TYPE_AUCTION			= 2,		// 拍卖
	MAIL_TYPE_AUCTION_SUCCESS	= 3,		// 拍卖系统邮件,拍卖成功
	MAIL_TYPE_AUCTION_OFFLINE	= 4,		// 拍卖系统邮件，流拍
	MAIL_TYPE_GM				= 5,		// GM邮件
	MAIL_TYPE_ARENA				= 6,		// 竞技场邮件
	MAIL_TYPE_OTHER				= 7,		// 其他邮件
}



public class XMailManager
{
	public class XMailInfo
	{
		public uint m_id;				
		public string m_title;			
		public string m_sender;			
		public string m_content;								
		public uint m_time;		
		public uint m_read;
		public uint m_mailType;
		public uint m_deleteType;
		public uint m_money;
		
		public List<XItem> listItems = new List<XItem>();
		
		public XMailInfo(uint id)
		{
			m_id = id;
		}
		
		public XMailInfo(uint id, string title, string sender, string content, uint time, 
			uint read, uint mailType, uint deleteType, uint money)
		{
			m_id = id;
			m_title = title;
			m_sender = sender;
			m_content = content;
			m_time = time;
			m_read = read;
			m_mailType = mailType;
			m_deleteType = deleteType;
			m_money = money;
		}
		
		public void AddItem(XItem item)
		{
			listItems.Add(item);
		}
		
		public override bool Equals(System.Object obj)
    	{
        	if (null == obj || GetType() != obj.GetType())
        	{
            	return false;
        	}
        	XMailInfo other = (XMailInfo)obj;
        	return m_id == other.m_id;
    	}
	}
	
	public class MailFind: IComparer<XMailInfo>
	{
    	public int Compare(XMailInfo x, XMailInfo y)
    	{
			return (int)(x.m_id - y.m_id);
    	}
	}
	
	public class MailSort: IComparer<XMailInfo>
	{
    	public int Compare(XMailInfo x, XMailInfo y)
    	{
			if ( x.m_id == y.m_id )
				return 0;
        	if (x.m_read < y.m_read)
            	return -1;
			else if ( x.m_read > y.m_read )
				return 1;
			else if ( x.m_time > y.m_time )
				return -1 ;
			else
				return 1;
    	}
	}
	
	public static SortedList<XMailInfo, XMailInfo> listMail = new SortedList<XMailInfo, XMailInfo>(new MailFind());
	
	public static List<XMailInfo> list2delete = new List<XMailInfo>();

    public XMailManager()
    {

    }

    ~XMailManager()
    {
		
    }
	
	public static void HandleMailComeIn()
	{
		if ( XMailBox.isActive )
		{
			XEventManager.SP.SendEvent(EEvent.Mail_WriteMail);
		}
		else
		{
			int unreadCount = 0;
			foreach(  KeyValuePair<XMailManager.XMailInfo, XMailManager.XMailInfo> item in XMailManager.listMail  )
			{
				if ( item.Value.m_read == 1 )
					unreadCount++;
			}
			if ( unreadCount > 0 )
			{
				XEventManager.SP.SendEvent(EEvent.Mail_Tip, unreadCount);
			}
		}
	}
	
	public XItem GetItem(uint dataIndex)
	{
		uint id = ( dataIndex >> 16 );
		int index = (int)( dataIndex & 0x0000FFFF );
		XMailInfo tinfo = new XMailInfo(id);
		XMailInfo info;
		if ( listMail.TryGetValue(tinfo, out info) )
		{
			if ( index >= 0 && index < info.listItems.Count )
			{
				return info.listItems[index];
			}
		}
		return null;
	}
}