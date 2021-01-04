
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGuildInfo")]



[System.Serializable]
public class XMemInfo
{
	public  GameObject m_curRoot;
	public  GameObject m_ButRoot;
	public  UILabel m_LabelMemName;
	public  UILabel m_LabelPos;
	public  UILabel m_LabelRank;
	public  UILabel m_LabelCurContr;
	public  UILabel m_LabelTotalContr;
	public  UILabel m_LabelLeftTime;
}

[System.Serializable]
public class XApplyInfo
{
	public  GameObject m_curRoot;
	public  UILabel m_LabelApplyName;
	public  UILabel m_LabelApplyTime;
	public  UILabel m_LabelRank;
	public  UILabel m_LabelLvl;
	public  UILabel m_LabelCombat;
	public  GameObject m_But;
}

public class XGuildInfo: XDefaultFrame
{
	public UISpriteGroup m_SpritePage;
	public GameObject  m_GuildMemRoot;
	public GameObject  m_GuildApplyRoot;
	
	public UILabel 		m_LabelCurPage;
	
	
	public GameObject m_ButPre;
	public GameObject m_ButNext;
	// 	
	public XMemInfo[] m_MemInfoList;
	public XApplyInfo [] m_ApplyInfoList;
	
	public override bool Init()
	{
		base.Init();
		
		XEventManager.SP.SendEvent(EEvent.GuildInfo_Init);
		
		UIEventListener lsExit = UIEventListener.Get(ButtonExit.gameObject);
		lsExit.onClick	= ClickExit;
		return true;
	}
	
	public  void ClickExit (GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGuildInfo);
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
