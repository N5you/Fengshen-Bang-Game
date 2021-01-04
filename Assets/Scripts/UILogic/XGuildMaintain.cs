using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGuildMaintain")]
public class XGuildMaintain : XDefaultFrame
{
	public GameObject	m_ButModify;
	public GameObject m_ButDisband;
	public GameObject m_ZhuanR;
	
	public UISpriteGroup m_SpritePage;
	
	public UILabel m_LabelMemName = null;
	public GameObject m_LabelAnno;
	public int m_selectPage = (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2;
	
	
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener ls = UIEventListener.Get(m_ButModify);
		ls.onClick	= OnModify;
		
		UIEventListener ls2 = UIEventListener.Get(m_ButDisband);
		ls2.onClick	= OnDisband;
		
		UIEventListener ls3 = UIEventListener.Get(m_ZhuanR);
		ls3.onClick	= OnZhuanR;
		
		UIEventListener lsExit = UIEventListener.Get(ButtonExit.gameObject);
		lsExit.onClick	= ClickExit;
		
		m_SpritePage.mModify	= OnSelectModify;
		return true;
	}
	
	public  void ClickExit (GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGuildMaintain);
	}
	
	public void OnModify(GameObject go)
	{
		if(m_selectPage == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1)
		{
			//XGuildManager.SP.RequestBroadAnno((uint)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1, m_LabelAnno.GetComponent<UILabel>().text);
			XGuildManager.SP.m_stGuildBaseInfo.cAnno = m_LabelAnno.GetComponent<UILabel>().text;
			XGuildManager.SP.RequestBroadAnno((uint)m_selectPage, XGuildManager.SP.m_stGuildBaseInfo.cAnno);
		}
		else if(m_selectPage == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2)
		{
			//XGuildManager.SP.RequestBroadAnno((uint)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2, m_LabelAnno.GetComponent<UILabel>().text);
			XGuildManager.SP.m_stGuildBaseInfo.cAnno2 = m_LabelAnno.GetComponent<UILabel>().text;
			XGuildManager.SP.RequestBroadAnno((uint)m_selectPage, XGuildManager.SP.m_stGuildBaseInfo.cAnno2);
		}
	}
	
	public void OnDisband(GameObject go)
	{
		XGuildManager.SP.RequestDisband();
	}
	
	public void OnZhuanR(GameObject go)
	{
		if(m_LabelMemName == null) return;
		UInt64 uPlayerId = XGuildManager.SP.GetGuildMemId(m_LabelMemName.text);
		if(uPlayerId == 0) return;
		
		XGuildManager.SP.RequestTran(uPlayerId);
	}

	public void OnSelectModify(int index)
	{
		if(index == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1)
		{
			m_selectPage = (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1;
			m_LabelAnno.GetComponent<UILabel>().text = XGuildManager.SP.m_stGuildBaseInfo.cAnno;
		}
		else if(index == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2)
		{
			m_LabelAnno.GetComponent<UILabel>().text = XGuildManager.SP.m_stGuildBaseInfo.cAnno2;
			m_selectPage = (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2;
		}
	}
	
	public void UpdateAnno()
	{
		if(m_selectPage == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1)
		{
			//m_selectPage = (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1;
			m_LabelAnno.GetComponent<UILabel>().text = XGuildManager.SP.m_stGuildBaseInfo.cAnno;
		}
		else if(m_selectPage == (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2)
		{
			m_LabelAnno.GetComponent<UILabel>().text = XGuildManager.SP.m_stGuildBaseInfo.cAnno2;
			//m_selectPage = (int)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2;
		}
	}
	
}
