using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public enum DailyPlayKey
{
	DailyPlay_HuaLing				= 1, 	// 化灵
	DailyPlay_ZhanYaoLu				= 2,	// 斩妖录
	DailyPlay_ZuoQi					= 3,	// 坐骑
	DailyPlay_Food					= 4,	// 食物
	DailyPlay_BangPaiQiFu			= 5,	// 帮派祈福
	DailyPlay_BangPaiLianGong		= 6,	// 帮派练功
	DailyPlay_Skill					= 7,	// 升级技能
	DailyPlay_ShanHe				= 8,	// 挑战山河
	DailyPlay_XianDaoHui			= 9,	// 调整仙道会
}

public struct SItemDailyPlay : IComparable<SItemDailyPlay>
{
    public uint key;
    public uint sortkey;
    public string text;

    public SItemDailyPlay(uint id, string t, uint skey)
    {
        key = id;
        text = t;
		sortkey = skey;
    }

    public void Clear()
    {
        key= 0;
    }

    public int CompareTo(SItemDailyPlay other)
    {
		if ( sortkey == other.sortkey )
        {
			return 0;
        }
        if (sortkey < other.sortkey)
        {
            return -1;
        }
		return 1;
    }

    public override bool Equals(System.Object obj)
    {
        if ( null == obj || GetType() != obj.GetType() )
        {
            return false;
        }
        SItemDailyPlay other = (SItemDailyPlay)obj;
        return key == other.key;
    }
}

public class XDailyPlaySignMgr : XSingleton<XDailyPlaySignMgr>
{	
	//FEBF02
	public static string FORMAT_PLAYSIGN_STRING	= "[link=1][linkdata=T(5)D({0})][linkcolor=00FF00]{1}[link=0]";
	
	public XDailyPlaySignMgr()
	{
		m_unlocks = new SortedList<uint, EFeatureID>();
		
		m_unlocks[(uint)DailyPlayKey.DailyPlay_BangPaiLianGong] = EFeatureID.EFeatureID_Family;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_BangPaiQiFu] = EFeatureID.EFeatureID_Family;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_Food] = EFeatureID.EFeatureID_Bag;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_HuaLing] = EFeatureID.EFeatureID_hualing;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_ShanHe] = EFeatureID.EFeatureID_ShanHeTu;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_Skill] = EFeatureID.EFeatureID_Skill;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_XianDaoHui] = EFeatureID.EFeatureID_XianDaoHui;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_ZhanYaoLu] = EFeatureID.EFeatureID_KillMonster;
		m_unlocks[(uint)DailyPlayKey.DailyPlay_ZuoQi] = EFeatureID.EFeatureID_FaBao;
		
		m_allShowData = new SortedList<SItemDailyPlay, SItemDailyPlay>();
		
	}
	
	public void Init()
	{
	}
	
	public void ShowUI()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eDailyPlaySign);
	}
	
	public void DoHandleShowPlaySign(DailyPlayKey key, int leftCount, int maxCount)
	{
		XCfgDailyPlaySign config = XCfgDailyPlaySignMgr.SP.GetConfig((uint)key);
		if ( null == config )
			return;
		
		string temp = string.Format(config.Text, leftCount, maxCount);
		
		string text = string.Format(FORMAT_PLAYSIGN_STRING, (uint)key, temp);
		
		SItemDailyPlay t = new SItemDailyPlay((uint)key, text, config.SortKey);
		
		if ( leftCount <= 0 && m_allShowData.ContainsKey(t) )
		{
			m_allShowData.Remove(t);
		}
		else
		{
			m_allShowData[t] = t;
		}
		
		RefreshUI();
	}
	
	public void CheckNeedShowSkill()
	{
		uint key = (uint)DailyPlayKey.DailyPlay_Skill;
		XCfgDailyPlaySign config = XCfgDailyPlaySignMgr.SP.GetConfig(key);
		if ( null == config )
			return;
		
		bool needShow = false;
		foreach(KeyValuePair<uint, XSkillOper> item in SkillManager.SP.m_SkillOpers)
		{
			if ( ESkill_State.ESkill_State_Can_Learn == SkillManager.SP.GetSkillState(item.Value.ID) )
			{
				needShow = true;
				break;
			}
		}
		
		string text = string.Format(FORMAT_PLAYSIGN_STRING, key, config.Text);
		
		SItemDailyPlay t = new SItemDailyPlay(key, text, config.SortKey);
		if ( needShow )
		{
			if ( !m_allShowData.ContainsKey(t) )
				m_allShowData[t] = t;
		}
		else
		{
			if ( m_allShowData.ContainsKey(t) )
				m_allShowData.Remove(t);
		}
		
		RefreshUI();
	}
	
	public void RefreshUI()
	{
		XEventManager.SP.SendEvent(EEvent.DailyPlaySign_RefreshUI);
	}
	
	public void UpdateZhanYaoLeftTime(string timevalue, bool showtime, int leftCount)
	{
		uint key = (uint)DailyPlayKey.DailyPlay_ZhanYaoLu;
		XCfgDailyPlaySign config = XCfgDailyPlaySignMgr.SP.GetConfig(key);
		string temp = string.Format(config.Text, leftCount, 0);
		SItemDailyPlay t = new SItemDailyPlay(key, temp, config.SortKey);
		if ( !m_allShowData.ContainsValue(t) )
			return;
		
		if ( showtime )
		{
			temp = temp + " " + timevalue;
		}
		t.text = string.Format(FORMAT_PLAYSIGN_STRING, (uint)key, temp);

		
		m_allShowData[t] = t;
		XEventManager.SP.SendEvent(EEvent.DailyPlaySign_UpdateText, key, t.text);
	}
	
	public void HandleClickLink(uint key)
	{
		switch ( (DailyPlayKey)key )
		{
		case DailyPlayKey.DailyPlay_BangPaiLianGong:
			
			break;
		case DailyPlayKey.DailyPlay_BangPaiQiFu:
			
			break;
		case DailyPlayKey.DailyPlay_Food:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eBagWindow);
			XEventManager.SP.SendEvent(EEvent.Bag_ShowEffect, EItem_Type.EITEM_TYPE_GOODS, EITEM_GOODS_SUB_TYPE.EITEM_GOODS_SUB_TYPE_JY, 900049u);
			
			break;
		case DailyPlayKey.DailyPlay_HuaLing:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eRoleInformation);
			XEventManager.SP.SendEvent(EEvent.CharInfo_OpenHuaLing);
			
			break;
		case DailyPlayKey.DailyPlay_ShanHe:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eShanHT);
			
			break;
		case DailyPlayKey.DailyPlay_Skill:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eSkillOpertation);
			
			break;
		case DailyPlayKey.DailyPlay_XianDaoHui:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eXianDH);
			
			break;
		case DailyPlayKey.DailyPlay_ZhanYaoLu:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eZhanYaoLu);
			
			break;
		case DailyPlayKey.DailyPlay_ZuoQi:
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eFaBao);
			
			break;
		default:
			break;
		}
	}
	
	public bool CheckCanShow(uint key)
	{
		if ( !m_unlocks.ContainsKey(key) )
			return false;
		return FeatureDataUnLockMgr.SP.IsUnLock((int)m_unlocks[key]);
	}
	
	public void NewFunctionOpen()
	{
		RefreshUI();
	}
	
	public SortedList<SItemDailyPlay, SItemDailyPlay> m_allShowData;
	
	public SortedList<uint, EFeatureID> m_unlocks;
}
