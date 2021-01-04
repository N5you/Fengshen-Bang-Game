using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGuildList")]

[System.Serializable]
public class XGuildSynInfo
{
	public  GameObject m_curRoot;
	public  UILabel m_LabelRank;
	public  UILabel m_LabelGuildName;
	public  UILabel m_LabelGuildLvl;
	public  UILabel m_LabelMasterName;
	public  UILabel m_LabelMemCount;
	public  GameObject m_But;
	public  UILabel m_LabelButText;
}


public class XGuildList : XDefaultFrame
{
	public UILabel m_LabelGuildName;
	public UILabel m_LabelMasterName;
	public UILabel m_LabelPage;
	
	public UILabel m_LabelMainLvl;
	public UILabel m_LabelShenSLvl;
	public UILabel m_LabelWuXLvl;
	
	public GameObject m_ButSearch;
	public GameObject m_ButPre;
	public GameObject m_ButNext;
	public GameObject m_ButCreate;

	public XGuildSynInfo[] m_UIGuildList = new XGuildSynInfo[(int)EGuildConstant.eMAX_GUILD_SHOW_COUNT];
	
	public override bool Init()
	{
		base.Init();
		
		XEventManager.SP.SendEvent(EEvent.GuildList_Init);
		
		UIEventListener lsExit = UIEventListener.Get(ButtonExit.gameObject);
		lsExit.onClick	= ClickExit;
		return true;
	}
	
	public  void ClickExit (GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGuildList);
	}
	
	public override void Show()
	{
		base.Show();
		
//		if(XGuildManager.SP.CheckGuildDataState() == false)
//		{
//			XGuildManager.SP.RequestAllGuildSynInfo();
//			return;
//		}
	}
}
