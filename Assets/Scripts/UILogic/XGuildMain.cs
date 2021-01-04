
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGuildMain")]
public class XGuildMain : XDefaultFrame
{
	// 帮会信息========================================================
	// 帮派管理
	public GameObject	m_ButGuildMgr;
	// 所有帮会列表
	public GameObject m_ButAllGuild;
	// 帮会名字
	public GameObject	m_LabelGuildName;
	// 帮会等级
	public GameObject	m_LabelGuildLvl;
	// 帮会排名
	public GameObject	m_LabelGuildRank;
	// 帮主名字
	public GameObject m_LabelMasterName;
	// 帮派人数
	public GameObject m_LabelMemQuantity;
	// 帮派经验
	public GameObject m_LabelGuildExp;
	// 帮派金钱
	public GameObject m_LabelGuildMoney;
	// 帮派公告
	public GameObject m_LabelAnno;
	// 解散倒计时
	public GameObject m_LabelDisTime;
	// 场景===========================================================
	// 主殿
	public GameObject	m_ButMain;
	public GameObject	m_ButMainJia;
	public GameObject	m_LabelMainLvl;
	public GameObject	m_LabelMainTime;
	// 学武
	public GameObject	m_ButXueW;
	public GameObject	m_ButXueWJia;
	public GameObject	m_LabelXueWLvl;
	public GameObject	m_LabelXueWTime;	
	// 演武
	public GameObject	m_ButYanW;
	public GameObject	m_ButYanWJia;
	public GameObject	m_LabelYanWLvl;
	public GameObject	m_LabelYanWTime;		
	// 祈愿
	public GameObject	m_ButQiY;
	public GameObject	m_ButQiYJia;
	public GameObject	m_LabelQiYLvl;
	public GameObject	m_LabelQiYTime;	
	// 神树
	public GameObject	m_ButShenS;
	public GameObject	m_ButShenSJia;
	public GameObject	m_LabelShenSLvl;
	public GameObject	m_LabelShenSTime;	
	
	public override bool Init()
	{
		base.Init();
		
		XEventManager.SP.SendEvent(EEvent.GuildMain_Init);
		
		UIEventListener lsExit = UIEventListener.Get(ButtonExit.gameObject);
		lsExit.onClick	= ClickExit;
		
		return true;
	}
	
	public  void ClickExit (GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGuildMain);
	}
	
	public void ShowGuildBaseInfo(STGuildBaseInfo	 stGuildBaseInfo, STGuildConstInfo[] stGuildConstInfo, byte[] inter)
	{
		ResetInfo();
		
		m_LabelGuildName.GetComponent<UILabel>().text 		= stGuildBaseInfo.cGuildName;
		m_LabelGuildLvl.GetComponent<UILabel>().text 			= stGuildBaseInfo.uLvl.ToString() + XStringManager.SP.GetString(405);
		m_LabelGuildRank.GetComponent<UILabel>().text 		= "";
		m_LabelMasterName.GetComponent<UILabel>().text 	= stGuildBaseInfo.cMasterName;
		m_LabelMemQuantity.GetComponent<UILabel>().text 	= stGuildBaseInfo.uMemCount.ToString();
		m_LabelGuildExp.GetComponent<UILabel>().text 			= stGuildBaseInfo.uExp.ToString();
		m_LabelGuildMoney.GetComponent<UILabel>().text 		= stGuildBaseInfo.uMoney.ToString();
		m_LabelAnno.GetComponent<UILabel>().text 				= stGuildBaseInfo.cAnno2;
		
	}
	
	public void ResetInfo()
	{
		m_LabelGuildName.GetComponent<UILabel>().text			= "";
		m_LabelGuildLvl.GetComponent<UILabel>().text				= "";
		m_LabelGuildRank.GetComponent<UILabel>().text				= "";
		m_LabelMasterName.GetComponent<UILabel>().text			= "";
		m_LabelMemQuantity.GetComponent<UILabel>().text		= "";
		m_LabelGuildExp.GetComponent<UILabel>().text				= "";
		m_LabelGuildMoney.GetComponent<UILabel>().text			= "";
		m_LabelAnno.GetComponent<UILabel>().text					= "";
		m_LabelDisTime.GetComponent<UILabel>().text				= "";
		
		
		m_LabelMainLvl.GetComponent<UILabel>().text				= "";
	 	m_LabelMainTime.GetComponent<UILabel>().text				= "";
		m_LabelXueWLvl.GetComponent<UILabel>().text				= "";
	 	m_LabelXueWTime.GetComponent<UILabel>().text			= "";
		m_LabelYanWLvl.GetComponent<UILabel>().text				= "";
	 	m_LabelYanWTime.GetComponent<UILabel>().text			= "";		
		m_LabelQiYLvl.GetComponent<UILabel>().text					= "";
	 	m_LabelQiYTime.GetComponent<UILabel>().text				= "";	
		m_LabelShenSLvl.GetComponent<UILabel>().text				= "";
	 	m_LabelShenSTime.GetComponent<UILabel>().text			= "";	
	}
	
	
}
