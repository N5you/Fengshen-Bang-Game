using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUTGuildMemInfo
{
	public  STGuildMember m_stGuildMember = null;
	public  XMemInfo		  m_UIInfo			   	 = null;
	
	public void Init(XMemInfo uiInfo)
	{ 
		m_UIInfo			  	    = uiInfo;
		UIEventListener ls 		= UIEventListener.Get(m_UIInfo.m_ButRoot);
		ls.onClick					+= OnClick;
	}
	
	public void SetDataInfo(STGuildMember stGuildMember)
	{
		m_stGuildMember 	= stGuildMember;
		
		m_UIInfo.m_LabelMemName.text 	= m_stGuildMember.cName;
		m_UIInfo.m_LabelRank.text 			= m_stGuildMember.uRank.ToString();
		m_UIInfo.m_LabelCurContr.text 		= m_stGuildMember.uCurContr.ToString();
		m_UIInfo.m_LabelTotalContr.text 	= m_stGuildMember.uTotalContr.ToString();
		//m_UIInfo.m_LabelPos.text				= m_stGuildMember.cPos.ToString();
		m_UIInfo.m_LabelLeftTime.text		= "";//m_stGuildMember.uLeftTime.ToString();
		if(m_stGuildMember.cPos == (byte)EGuildConstant.GJ_NORMAL)
		{
			m_UIInfo.m_LabelPos.text = XStringManager.SP.GetString(720);
		}
		else if(m_stGuildMember.cPos == (byte)EGuildConstant.GJ_BRAINMAN)
		{
			m_UIInfo.m_LabelPos.text = XStringManager.SP.GetString(719);
		}
		else if(m_stGuildMember.cPos == (byte)EGuildConstant.GJ_HEADER)
		{
			m_UIInfo.m_LabelPos.text = XStringManager.SP.GetString(718);
		}
	}
	
	public void OnClick(GameObject go)
	{
		XUIPopMenu.isOper = true;
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePopMenu );
		XEventManager.SP.SendEvent(EEvent.PopMenu_GuildKickMem, m_stGuildMember.uPlayerId);	
	}
	
	public void Show()
	{
		m_UIInfo.m_curRoot.SetActive(true);
	}
	
	public void Hide()
	{
		m_UIInfo.m_curRoot.SetActive(false);
	}
}

public class XUTApplyInfo
{
	public  STApply			  m_stGuildApply		 = null;
	public  XApplyInfo		  m_UIInfo			   	 = null;
	
	public void Init(XApplyInfo uiInfo)
	{ 
		m_UIInfo	 	= uiInfo;
		UIEventListener ls 	= UIEventListener.Get(m_UIInfo.m_But);
		ls.onClick				+= OnClick;
	}
	
	public void SetDataInfo(STApply	stGuildApply)
	{
		m_stGuildApply	 	= stGuildApply;
		
		m_UIInfo.m_LabelApplyName.text 	= m_stGuildApply.cName;
		m_UIInfo.m_LabelRank.text 			= m_stGuildApply.uRank.ToString();
		m_UIInfo.m_LabelLvl.text 				= m_stGuildApply.uLvl.ToString();
		m_UIInfo.m_LabelCombat.text 		= m_stGuildApply.uCombat.ToString();
		m_UIInfo.m_LabelApplyTime.text		= "";//m_stGuildApply.uApplyTime.ToString();
	}
	
	public void OnClick(GameObject go)
	{
		XGuildManager.SP.RequestHandleApply(m_stGuildApply.uPlayerId, (uint)EGuildConstant.eGUILD_AGREE_JOIN);
	}
	
	public void Show()
	{
		m_UIInfo.m_curRoot.SetActive(true);
	}
	
	public void Hide()
	{
		m_UIInfo.m_curRoot.SetActive(false);
	}
	
}


class XUTGuildInfo: XUICtrlTemplate<XGuildInfo>
{	
	private 		SortedList<UInt64, STGuildMember> 		m_MemberList 	     = null;
	private 		SortedList<UInt64, STApply> 					m_ApplyList	  	     = null;
	
	private		uint 														m_curMemPage 	 = 0;
	private		uint 														m_curApplyPage 	 = 0;
	
	private  		ArrayList 												m_arrayGuildMemInfo   	= new ArrayList();
	private  		ArrayList 												m_arrayApplyInfo		  	= new ArrayList();
	
	private		XUTGuildMemInfo[]								m_utGuildMemInfo  		= new XUTGuildMemInfo[(int)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT];
	private		XUTApplyInfo[]										m_utApplyInfo		  		= new XUTApplyInfo[(int)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT];
	
	private		uint														m_SelectPage 		  		= 0;
	
	public XUTGuildInfo()
	{
		RegEventAgent_CheckCreated(EEvent.GuildInfo_Init, OnInit);
		RegEventAgent_CheckCreated(EEvent.GuildInfo_UpdateMemInfo, OnUpdateMemInfo);
		RegEventAgent_CheckCreated(EEvent.GuildInfo_UpdateApplyInfo, OnUpdateApplyInfo);
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		ShowMemInfo();
	}
	
	public void OnInit(EEvent evt, params object[] args)
	{
		m_MemberList 		= XGuildManager.SP.m_MemberList;
		m_ApplyList			= XGuildManager.SP.m_ApplyList;
		
		for(int i = 0; i < (int)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT; i++)
		{
			m_utGuildMemInfo[i] = new XUTGuildMemInfo();
		}
		
		for(int i = 0; i < (int)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT; i++)
		{
			m_utApplyInfo[i]	 = new XUTApplyInfo();
		}
		
		for(int i = 0; i < (int)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT; i++)
		{
			m_utGuildMemInfo[i].Init(LogicUI.m_MemInfoList[i]);
		}
		
		for(int i = 0; i < (int)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT; i++)
		{
			m_utApplyInfo[i].Init(LogicUI.m_ApplyInfoList[i]);
		}
		
		LogicUI.m_SpritePage.mModify	= OnSelectModify;
		
		UIEventListener ls 		= UIEventListener.Get(LogicUI.m_ButPre);
		ls.onClick					+= OnPrePage;
		
		UIEventListener ls2	 	= UIEventListener.Get(LogicUI.m_ButNext);
		ls2.onClick					+= OnNextPage;
		
		HideAllMemInfo();
	}
		
	public void OnSelectModify(int index)
	{
		m_SelectPage = (uint)index;
		if(index == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_MEM)
		{
			ShowMemInfo();
		}
		else if(index == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_APPLY)
		{
			ShowApplyInfo();
		}
	}
	
	 // 比较参数
	 private class XCompareMemInfo : System.Collections.IComparer
	 {
            public int Compare(object x, object y)
            {
                return (int)(((STGuildMember)x).uPlayerId - ((STGuildMember)y).uPlayerId);
            }
      }
	
	 // 比较参数
	 private class XCompareApplyInfo : System.Collections.IComparer
	 {
            public int Compare(object x, object y)
            {
                return (int)(((STApply)x).uPlayerId - ((STApply)y).uPlayerId);
            }
      }
	
	public void HideAllMemInfo()
	{
		for(int i = 0; i < (uint)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT; i++)
		{
			m_utGuildMemInfo[i].Hide();
		}
	}
	
	// 加载数据 排序
	public void LoadMemInfo()
	{
		m_arrayGuildMemInfo.Clear();
		
		if(m_MemberList == null) return;
		
		STGuildMember  stGuildMember = null;
		foreach(KeyValuePair<UInt64, STGuildMember> kvpItem in m_MemberList)
		{
			stGuildMember = kvpItem.Value;
		
			m_arrayGuildMemInfo.Add(stGuildMember);
		}
		XCompareMemInfo	 compareMemInfo = new XCompareMemInfo();
		m_arrayGuildMemInfo.Sort(compareMemInfo);
	}
	
	// 显示当前页
	public void ShowCurPageMemInfo()
	{
		HideAllMemInfo();
		
		uint beginIndex = (uint)m_curMemPage * (uint)EGuildConstant.eGUILDINFO_SELECT_PAGE_MEM;
		uint endIndex    = beginIndex + (uint)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT;
		if(endIndex >= m_arrayGuildMemInfo.Count)
			endIndex = (uint)m_arrayGuildMemInfo.Count;
		
		if(beginIndex > endIndex) return; 
		
		if(m_arrayGuildMemInfo.Count < beginIndex)
		{
			return;
		}
		
		for(uint curIndex = beginIndex, uiIndex = 0; curIndex < endIndex; curIndex++, uiIndex++)
		{
			STGuildMember  stGuildMember = m_arrayGuildMemInfo[(int)curIndex] as STGuildMember;
			m_utGuildMemInfo[uiIndex].Show();
			m_utGuildMemInfo[uiIndex].SetDataInfo(stGuildMember);
		}
		
		LogicUI.m_LabelCurPage.text = (m_curMemPage + 1).ToString();
	}
	
	public void HideAllApplyInfo()
	{
		for(int i = 0; i < (uint)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT; i++)
		{
			m_utApplyInfo[i].Hide();
		}
	}
	
	// 加载数据 排序
	public void LoadApplyInfo()
	{
		m_arrayApplyInfo.Clear();
		
		if(m_ApplyList == null) return;
		
		STApply  stApply = null;
		foreach(KeyValuePair<UInt64, STApply> kvpItem in m_ApplyList)
		{
			stApply = kvpItem.Value;
		
			m_arrayApplyInfo.Add(stApply);
		}
		XCompareApplyInfo	 compareApplyInfo = new XCompareApplyInfo();
		m_arrayApplyInfo.Sort(compareApplyInfo);
	}
	
	// 显示当前页
	public void ShowCurPageApplyInfo()
	{
		HideAllApplyInfo();
		
		uint beginIndex = (uint)m_curApplyPage * (uint)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT;
		uint endIndex    = beginIndex + (uint)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT;
		if(endIndex >= m_arrayApplyInfo.Count)
			endIndex = (uint)m_arrayApplyInfo.Count;
		
		if(beginIndex > endIndex) return; 
		
		if(m_arrayApplyInfo.Count < beginIndex)
		{
			return;
		}
		
		for(uint curIndex = beginIndex, uiIndex = 0; curIndex < endIndex; curIndex++, uiIndex++)
		{
			STApply  stApply = m_arrayApplyInfo[(int)curIndex] as STApply;
			m_utApplyInfo[uiIndex].Show();
			m_utApplyInfo[uiIndex].SetDataInfo(stApply);
		}
		
		LogicUI.m_LabelCurPage.text = (m_curApplyPage+ 1).ToString();
	}
	
	public uint GetMemMaxPageCount()
	{
		return  (uint)m_arrayGuildMemInfo.Count / (uint)EGuildConstant.eMAX_GUILDMEM_SHOW_COUNT + 1;
	}
	
	public uint GetApplyMaxPageCount()
	{
		return  (uint)m_arrayApplyInfo.Count / (uint)EGuildConstant.eMAX_GUILDAPPLY_SHOW_COUNT + 1;
	}
	
	public void OnPrePage(GameObject go)
	{
		if(m_SelectPage == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_MEM)
		{
			if(m_curMemPage <= 0)
			{
				m_curMemPage = 0;
				return;
			}
		
			m_curMemPage--;
			ShowCurPageMemInfo();
		}
		else if(m_SelectPage == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_APPLY)
		{
			if(m_curApplyPage <= 0)
			{
				m_curApplyPage = 0;
				return;
			}
		
			m_curApplyPage--;
			
			ShowCurPageApplyInfo();
		}
		
	}
	
	public void OnNextPage(GameObject go)
	{
		if(m_SelectPage == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_MEM)
		{
			if(m_curMemPage <= 0)
			m_curMemPage = 0;
		
			if(m_curMemPage + 1 >= GetMemMaxPageCount()) 
			{
				return;
			}
			m_curMemPage++;
			
			ShowCurPageMemInfo();
		}
		else if(m_SelectPage == (int)EGuildConstant.eGUILDINFO_SELECT_PAGE_APPLY)
		{
			if(m_curApplyPage <= 0)
			m_curApplyPage = 0;
		
			if(m_curApplyPage + 1 >= GetApplyMaxPageCount()) 
			{
				return;
			}
			m_curApplyPage++;
			ShowCurPageApplyInfo();
		}
	}
	
	public void ShowMemInfo()
	{ 
		HideAllMemInfo();
		LogicUI.m_GuildApplyRoot.SetActive(false);
		LogicUI.m_GuildMemRoot.SetActive(true);
		if(XGuildManager.SP.CheckGuildDataState((uint)EGuildConstant.eGET_MEM_GUILD_INFO) == false)
		{
			XGuildManager.SP.RequestGuildAllMemInfo();
			return;
		}
		
		ShowCurPageMemInfo();
	}
	
	public void OnUpdateMemInfo(EEvent evt, params object[] args)
	{
		LoadMemInfo();
		ShowCurPageMemInfo();
	}
	
	public void ShowApplyInfo()
	{
		HideAllApplyInfo();
		LogicUI.m_GuildApplyRoot.SetActive(true);
		LogicUI.m_GuildMemRoot.SetActive(false);
		
		if(XGuildManager.SP.CheckGuildDataState((uint)EGuildConstant.eGET_APY_GUILD_INFO) == false)
		{
			XGuildManager.SP.RequestGuildAllApplyInfo();
			return;
		}
		
		ShowCurPageApplyInfo();
	}
	
	public void OnUpdateApplyInfo(EEvent evt, params object[] args)
	{
		LoadApplyInfo();
		ShowCurPageApplyInfo();
	}
}	
	
	
	