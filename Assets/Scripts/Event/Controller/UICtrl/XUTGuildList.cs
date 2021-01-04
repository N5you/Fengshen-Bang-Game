using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class XUTGuildSynInfo
{
	public  STGuildSynInfo m_stGuildSynInfo = null;
	public  XGuildSynInfo	 m_UIInfo			   = null;
	public  uint  m_State;
	
	
	public void Init(XGuildSynInfo uiInfo)
	{ 
		m_UIInfo = uiInfo;
		UIEventListener ls 	= UIEventListener.Get(m_UIInfo.m_But);
		ls.onClick				+= OnClick;
		m_State				= (uint)EGuildConstant.eApplyState_Normal;
		// 设置按钮文字
		m_UIInfo.m_LabelButText.text = XStringManager.SP.GetString(715);
	}
	
	public void Release()
	{
		UIEventListener ls 	= UIEventListener.Get(m_UIInfo.m_But);
		ls.onClick				-= OnClick;
	}
	
	public void SetDataInfo(STGuildSynInfo stGuildSynInfo)
	{
		m_stGuildSynInfo 	= stGuildSynInfo;
		
		m_UIInfo.m_LabelMasterName.text 	= "";
		m_UIInfo.m_LabelGuildName.text 	= m_stGuildSynInfo.cGuildName;
		m_UIInfo.m_LabelGuildLvl.text 		= m_stGuildSynInfo.uLvl.ToString();
		m_UIInfo.m_LabelMasterName.text 	= m_stGuildSynInfo.cMasterName;
		m_UIInfo.m_LabelMemCount.text 	= m_stGuildSynInfo.uMemLen.ToString();
	}
	
	public void OnClick (GameObject go)
	{
		if(m_State == (uint)EGuildConstant.eApplyState_Normal)
		{
			XGuildManager.SP.RequestApplyJoinGuild(m_stGuildSynInfo.uGuildId);
			SetSate((uint)EGuildConstant.eApplyState_Apply);
		}
		else
		{
			XGuildManager.SP.RequestCancelApplyJoin(m_stGuildSynInfo.uGuildId);
			SetSate((uint)EGuildConstant.eApplyState_Normal);
		}
	}
	
	public void SetSate(uint uState)
	{
		if(m_State == uState) return;
		
		m_State = uState;
		
		if(m_State == (uint)EGuildConstant.eApplyState_Normal)
		{
			m_UIInfo.m_LabelButText.text = XStringManager.SP.GetString(715);
		}
		else
		{
			m_UIInfo.m_LabelButText.text = XStringManager.SP.GetString(716);
		}
	}
	
	public void Show()
	{
		if(m_UIInfo != null)
		{ 
			m_UIInfo.m_curRoot.SetActive(true);
		}
	}
	
	public void Hide()
	{
		if(m_UIInfo != null)
		{ 
			m_UIInfo.m_curRoot.SetActive(false);
		}
	}
}
	

class XUTGuildList: XUICtrlTemplate<XGuildList>
{	
	public XUTGuildList()
	{
		RegEventAgent_CheckCreated(EEvent.GuildList_Init, OnInit);
		RegEventAgent_CheckCreated(EEvent.GuildList_UpdateGuildSynInfo, OnUpdateGuildSynInfo);
		RegEventAgent_CheckCreated(EEvent.GuildList_UpdateSelfApplyState, OnUpdateAllApplyState);
	}
	
	
	private uint m_uCurPage		= 0;
	private SortedList<UInt64,STGuildSynInfo> 			m_GuildList  			= null;
	private  ArrayList 												m_GuildSynInfoList = new ArrayList();
	public   ArrayList 												m_SelfApplyList;
	private XUTGuildSynInfo[] m_utGuildInfoList 			= new XUTGuildSynInfo[(int)EGuildConstant.eMAX_GUILD_SHOW_COUNT];
	
	// 初始化 
	public void OnInit(EEvent evt, params object[] args)
	{
		for(int j = 0; j < (int)EGuildConstant.eMAX_GUILD_SHOW_COUNT; j++)
		{
			m_utGuildInfoList[j] = new XUTGuildSynInfo();
		}
		
		m_SelfApplyList 		= XGuildManager.SP.m_SelfApplyList;
		m_GuildList			= XGuildManager.SP.m_GuildSysList;
		
		for(int i = 0; i < (int)EGuildConstant.eMAX_GUILD_SHOW_COUNT; i++)
		{
			m_utGuildInfoList[i].Init(LogicUI.m_UIGuildList[i]);
		}
		
		UIEventListener ls		= UIEventListener.Get(LogicUI.m_ButCreate);
		ls.onClick					+= OnCreateGuild;
		
		UIEventListener ls2 	= UIEventListener.Get(LogicUI.m_ButPre);
		ls2.onClick					+= OnPrePage;
		
		UIEventListener ls3 	= UIEventListener.Get(LogicUI.m_ButNext);
		ls3.onClick					+= OnNextPage;
		
		HideAllGuildInfo();
	}
	// 显示
	public override void OnShow()
	{
		base.OnShow();
		
		if(XGuildManager.SP.CheckGuildDataState((uint)EGuildConstant.eGET_ALL_GUILD_INFO) == false)
		{
			XGuildManager.SP.RequestAllGuildSynInfo();
			return;
		}
		ShowCurPageGuildInfo();
	}
	
	// 隐藏所有项
	public void HideAllGuildInfo()
	{
		for(int i = 0; i < (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT; i++)
		{
			m_utGuildInfoList[i].Hide();
		}
	}
	
	// 比较参数
	 private class XCompareGuildInfo : System.Collections.IComparer
	 {
            public int Compare(object x, object y)
            {
                return (int)(((STGuildSynInfo)x).uLvl - ((STGuildSynInfo)y).uLvl);
            }
      }
	
	// 加载数据 排序
	public void LoadGuildInfo()
	{
		m_GuildSynInfoList.Clear();
		
		if(m_GuildList == null) return;
		
		STGuildSynInfo  stGuildSynInfo = null;
		foreach(KeyValuePair<UInt64, STGuildSynInfo> kvpItem in m_GuildList)
		{
			stGuildSynInfo = kvpItem.Value;
		
			m_GuildSynInfoList.Add(stGuildSynInfo);
		}
		XCompareGuildInfo	 compareGuildInfo = new XCompareGuildInfo();
		m_GuildSynInfoList.Sort(compareGuildInfo);
	}
	
	// 显示当前页
	public void ShowCurPageGuildInfo()
	{
		HideAllGuildInfo();
		
		if(m_GuildSynInfoList.Count == 0) return;
		
		uint beginIndex = (uint)m_uCurPage * (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT;
		uint endIndex    = beginIndex + (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT;
		if(endIndex >= m_GuildSynInfoList.Count)
			endIndex = (uint)m_GuildSynInfoList.Count;
		
		if(beginIndex > endIndex) return; 
		
		if(m_GuildSynInfoList.Count < beginIndex)
		{
			return;
		}
		
		for(uint curIndex = beginIndex, uiIndex = 0; curIndex < endIndex; curIndex++, uiIndex++)
		{
			STGuildSynInfo  stGuildSynInfo = m_GuildSynInfoList[(int)curIndex] as STGuildSynInfo;
			m_utGuildInfoList[uiIndex].Show();
			m_utGuildInfoList[uiIndex].SetDataInfo(stGuildSynInfo);
			
			if(HasApplyId(stGuildSynInfo.uGuildId))
			{
				m_utGuildInfoList[uiIndex].SetSate((uint)EGuildConstant.eApplyState_Apply);
			}
			else
			{
				m_utGuildInfoList[uiIndex].SetSate((uint)EGuildConstant.eApplyState_Normal);
			}
		}
		
		LogicUI.m_LabelPage.text = (m_uCurPage + 1).ToString();
	}
	
	public void OnPrePage(GameObject go)
	{
		if(m_uCurPage <= 0)
		{
			m_uCurPage = 0;
			return;
		}
		
		m_uCurPage--;
		ShowCurPageGuildInfo();
	}
	
	public void OnNextPage(GameObject go)
	{
		if(m_uCurPage <= 0)
			m_uCurPage = 0;
		
		if(m_uCurPage + 1 >= GetMaxPageCount()) 
		{
			return;
		}
		m_uCurPage++;
		ShowCurPageGuildInfo();
	}
	
	public uint GetMaxPageCount()
	{
		return  (uint)m_GuildSynInfoList.Count / (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT + 1;
	}
	
	
	public void OnUpdateGuildSynInfo(EEvent evt, params object[] args)
	{
		LoadGuildInfo();
		ShowCurPageGuildInfo();
	}
	
	//public void OnUpdateSeflApplyState()
	//{
		//ShowApplyState();
	//}
	
	public void OnUpdateAllApplyState(EEvent evt, params object[] args)
	{
		uint beginIndex = (uint)m_uCurPage * (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT;
		uint endIndex    = beginIndex + (uint)EGuildConstant.eMAX_GUILD_SHOW_COUNT;
		if(endIndex >= m_GuildSynInfoList.Count)
			endIndex = (uint)m_GuildSynInfoList.Count - 1;
		
		uint curIndex = beginIndex;
		for(uint i = 0; curIndex < endIndex; i++, curIndex++)
		{
			STGuildSynInfo  stGuildSynInfo = m_GuildSynInfoList[(int)curIndex] as STGuildSynInfo;
			if(HasApplyId(stGuildSynInfo.uGuildId))
			{
				m_utGuildInfoList[i].SetSate((uint)EGuildConstant.eApplyState_Apply);
			}
			else
			{
				m_utGuildInfoList[i].SetSate((uint)EGuildConstant.eApplyState_Normal);
			}
		}
	}
	
	public bool HasApplyId(UInt64 uGuildId)
	{
		return m_SelfApplyList.Contains(uGuildId);
	}
	
	public void OnCreateGuild(GameObject obj)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eGuildCreate);
	}
	
}	
	