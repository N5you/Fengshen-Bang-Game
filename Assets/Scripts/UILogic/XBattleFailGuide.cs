
using System;
using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;


[AddComponentMenu("UILogic/XBattleFailGuide")]


[System.Serializable]
public class XBattleFailGuideBut
{
	public UIImageButton 	m_But = null;
	public int					m_iLink;
	
	public void OnInit()
	{
		m_iLink = -1;
		UIEventListener listen = UIEventListener.Get(m_But.gameObject);
		listen.onClick += OnClick;
	}
	
	public void SetLink(int iLink)
	{
		m_iLink = iLink;
	}
	
	public void OnClick(GameObject go)
	{		
		if(m_iLink == -1)
			return;
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (EUIPanel)m_iLink);
	}
	
}



public class XBattleFailGuide: XUIBaseLogic
{
	public enum BattleFailGuide
	{
		eMax_Button = 6,
	}
	
	public UIImageButton m_ButRebroad = null;
	public UIImageButton m_ButOK = null;
	public UILabel m_Label = null;
	public XBattleFailGuideBut [] m_BattleFailGuideBut = new XBattleFailGuideBut[(int)BattleFailGuide.eMax_Button];
	
	private ArrayList m_UnLockList = new ArrayList();
	
	
	public XBattleFailGuide()
	{
		
	}
	
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener listenExit = UIEventListener.Get(m_ButOK.gameObject);
		listenExit.onClick += Exit;
		
		UIEventListener listen = UIEventListener.Get(m_ButRebroad.gameObject);
		listen.onClick += Replayer;
		
		for(int i = 0; i < (int)BattleFailGuide.eMax_Button; i++)
		{
			//m_BattleFailGuideBut[i] = new XBattleFailGuideBut();
			m_BattleFailGuideBut[i].OnInit();
		}
		
		return true;
	}


	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.Fight_LeaveSce);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eBattleFailGuide);
	}
	
	public void Replayer(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eBattleFailGuide);
		XEventManager.SP.SendEvent(EEvent.Show_Fight_Replay);
	}
	
	public override void Show()
	{
		base.Show();
		ShowAllInfo();
	}
	
	public void LoadUnlockData()
	{
		 XCfgBattleFailGuide cfgBattleFailGuide = null;
		SortedList<int, XCfgBattleFailGuide>  ItemTable = XCfgBattleFailGuideMgr.SP.ItemTable;;
		if(ItemTable == null) return;
		foreach(KeyValuePair<int, XCfgBattleFailGuide> kvpItem in ItemTable)
		{
			cfgBattleFailGuide = kvpItem.Value;
			if(FeatureDataUnLockMgr.SP.IsUnLock(cfgBattleFailGuide.Unlock))
			{
				m_UnLockList.Add(cfgBattleFailGuide);
			}
		}
	}
	
	public void HideAllInfo()
	{
		for(int i = 0; i < (int)BattleFailGuide.eMax_Button; i++)
		{
			m_BattleFailGuideBut[i].m_But.Hide();
		}
		m_Label.text = "";
	}
	
	
	public void ShowAllInfo()
	{	
		HideAllInfo();
		LoadUnlockData();
		
		if(m_UnLockList.Count == 0) 
		{
			//XEventManager.SP.SendEvent(EEvent.Fight_LeaveSce);
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eBattleFailGuide);
			return; 
		}
		
		XCfgBattleFailGuide cfgBattleFailGuide = null;
		
		System.Random ran=new System.Random(); 
		int index = ran.Next(0, m_UnLockList.Count);
		
		cfgBattleFailGuide = m_UnLockList[index] as XCfgBattleFailGuide;
		
		if(cfgBattleFailGuide != null)
		{
			m_Label.text = string.Format(XStringManager.SP.GetString(732), cfgBattleFailGuide.Name);
		
			int iCurIndex = (int)BattleFailGuide.eMax_Button - 1;
			
			FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig((uint)cfgBattleFailGuide.Unlock);
			if(unLock1 != null)
			{
				if(FeatureDataUnLockMgr.SP.IsUnLock(cfgBattleFailGuide.Unlock) == true)
				{
					XUIDynamicAtlas.SP.SetSprite(m_BattleFailGuideBut[iCurIndex].m_But.target, (int)unLock1.AtlasID, unLock1.IconID_Com, true, null);
					m_BattleFailGuideBut[iCurIndex].m_But.normalSprite		= unLock1.IconID_Com;
					m_BattleFailGuideBut[iCurIndex].m_But.hoverSprite		= unLock1.IconID_Hover;
					m_BattleFailGuideBut[iCurIndex].m_But.pressedSprite	= unLock1.IconID_Pressed;
					m_BattleFailGuideBut[iCurIndex].m_But.disabledSprite	= unLock1.IconID_Com;
					
					m_BattleFailGuideBut[iCurIndex].m_But.Show();
					m_BattleFailGuideBut[iCurIndex].SetLink(cfgBattleFailGuide.LinkUI);
				}
			}
		}
		
		for(int i = 0; i < (int)BattleFailGuide.eMax_Button - 1; i++)
		{
			cfgBattleFailGuide = XCfgBattleFailGuideMgr.SP.GetConfig( i + 1 );
			if(cfgBattleFailGuide == null)
				continue;
			
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)cfgBattleFailGuide.Unlock);
			if(unLock == null)
				continue;
			
			if(FeatureDataUnLockMgr.SP.IsUnLock(cfgBattleFailGuide.Unlock) == true)
			{
				XUIDynamicAtlas.SP.SetSprite(m_BattleFailGuideBut[i].m_But.target, (int)unLock.AtlasID, unLock.IconID_Com, true, null);
				m_BattleFailGuideBut[i].m_But.normalSprite		= unLock.IconID_Com;
				m_BattleFailGuideBut[i].m_But.hoverSprite		= unLock.IconID_Hover;
				m_BattleFailGuideBut[i].m_But.pressedSprite	= unLock.IconID_Pressed;
				m_BattleFailGuideBut[i].m_But.disabledSprite		= unLock.IconID_Com;
				
				m_BattleFailGuideBut[i].m_But.Show();
				m_BattleFailGuideBut[i].SetLink(cfgBattleFailGuide.LinkUI);
			}
			// disabled state
			//m_BattleFailGuideBut[i].m_But.isEnabled = false;
			//m_BattleFailGuideBut[i].m_But.UpdateImage();
		}
	}
	
}


