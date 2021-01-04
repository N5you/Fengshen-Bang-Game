

using System;
using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;


[AddComponentMenu("UILogic/XVip")]

[System.Serializable]
public class XVipAwardItem
{
	public GameObject m_Root;
	public UILabel m_LabelName = null;
	public XActionIcon m_itemActionIcon = null;
	
	public uint uItemId = 0;
	
	public void ShowInfo(uint uItemId, uint count)
	{
		XCfgItem itemBase = XCfgItemMgr.SP.GetConfig(uItemId);
		if(itemBase == null)
			return;
		
		m_itemActionIcon.SetSpriteByID(uItemId);
		if(count > 0)
			m_itemActionIcon.IconNum.text = count.ToString();
		
		m_LabelName.text = itemBase.Name;
	}
	
	public void ShowInfo(EVipConst eMoney, string name, UIAtlas  atlas)
	{
		if(eMoney == EVipConst.eVIP_AWARD_Money)
		{
			m_itemActionIcon.SetSprite(atlas, Define.Money_Icon_Id);
		}
		else
		{
			m_itemActionIcon.SetSprite(atlas, Define.RealMoney_Icon_Id);
		}
		
		m_LabelName.text = name;
	}
	
	public void Show()
	{
		m_Root.SetActive(true);
	}
	
	public void Hide()
	{
		m_Root.SetActive(false);
	}
	
}



public class XVip: XDefaultFrame
{
	public GameObject   m_RootSprite;
	public UISpriteGroup m_SpriteGroup;
	
	
	// 所有当前级vip开放项目根
	public UIGrid			m_TeQuanGrid;
	// 模板vip项
	public GameObject	m_TempItem;
	
	// 再充值 多少 就升至 vip n
	public UILabel			m_LabelTip1;
	// 充值多少 到vip n
	public UILabel			m_LabelTip2;
	
	public UILabel 		m_LabelCurLvl;
	public UILabel			m_LabelNextLvl;
	
	public UILabel			m_LabelAwardLvl;
	
	// 进度条
	public UISlider		m_Slider;
	// 上面 充值根 
	public GameObject m_RootVipCharge;
	public GameObject m_ButRecharge;
	public GameObject m_ButGotAward;
	public GameObject m_ButMoreAndBack;
	
	public UIAtlas		m_Atlas = null;
	
	public XVipAwardItem [] m_xVipAwardItem = new XVipAwardItem[(int)EVipConst.eMAX_VIP_AWARD_ITEM];
	
	private ArrayList m_arrayItemObject = new ArrayList();
	
	private  uint m_uShowVipLvl = 0;
	private  bool m_bShowCur = true;
	
	public XVip()
	{
		
	}
	
	
	public override bool Init()
	{
		base.Init();
		//XEventManager.SP.SendEvent(EEvent.GuildCreate_Init);
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		m_SpriteGroup.mModify	= OnSelectModify;
		
		UIEventListener listenMore = UIEventListener.Get(m_ButMoreAndBack);
		listenMore.onClick += OnClickMoreAndBack;

		UIEventListener listenAward = UIEventListener.Get(m_ButGotAward);
		listenAward.onClick += OnClickGotAward;
		return true;
	}


	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eVip);
	}
	
	public override void Show()
	{
		base.Show();
		
		m_bShowCur = true;
		m_uShowVipLvl = XLogicWorld.SP.MainPlayer.VIPLevel;
		m_ButMoreAndBack.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(726);
		ShowInfo();
	}
	
	public void OnSelectModify(int index)
	{
		m_uShowVipLvl = (uint)index + 1;
		
		ShowInfo();
	}
		
	public void OnClickMoreAndBack(GameObject go)
	{
		if(m_bShowCur)
		{
			// 更多vip
			m_bShowCur = false;
			m_uShowVipLvl = 1;
			m_ButMoreAndBack.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(728);
		}
		else
		{
			// 当前vip
			m_bShowCur = true;
			m_uShowVipLvl = XLogicWorld.SP.MainPlayer.VIPLevel;
			m_ButMoreAndBack.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(726);
		}
		ShowInfo();
	}
	
	public void OnClickGotAward(GameObject go)
	{
		
		XVipManager.SP.OnRequestVipInfo(m_uShowVipLvl);
	}
	
	public void HideAllIcon()
	{
		for(int i = 0; i < (int)EVipConst.eMAX_VIP_AWARD_ITEM; i++)
		{
			m_xVipAwardItem[i].Hide();
		}
	}
	
	public void ShowInfo()
	{
		HideAllIcon();
		m_ButGotAward.SetActive(false);
		m_LabelAwardLvl.text = "";
		
		if(m_bShowCur)
		{
			if(m_uShowVipLvl < (uint)EVipConst.eMAX_VIP_LVL)
			{
				m_RootVipCharge.SetActive(true);
				m_Slider.sliderValue = (float)GetVipLvlMoney(m_uShowVipLvl) / (float)GetVipLvlMoney(m_uShowVipLvl + 1);
				m_LabelTip1.text = string.Format(XStringManager.SP.GetString(724), GetNextLvlMoney(), m_uShowVipLvl + 1);
			}
			else
			{
				m_RootVipCharge.SetActive(false);
			}
			
			// 当前vip
			m_RootSprite.SetActive(false);
			
			
			
		}
		else
		{
			// 更多vip
			m_RootSprite.SetActive(true);
			m_RootVipCharge.SetActive(false);
		}
		
		foreach(GameObject info in m_arrayItemObject)
		{
			NGUITools.Destroy(info);
		}
		m_arrayItemObject.Clear();
		
		if(m_uShowVipLvl < (uint)EVipConst.eMAX_VIP_LVL)
		{
			m_LabelTip2.text = string.Format(XStringManager.SP.GetString(725), GetVipLvlMoney(m_uShowVipLvl + 1), m_uShowVipLvl + 1);
			m_LabelCurLvl.text = "VIP" + m_uShowVipLvl.ToString();
			m_LabelNextLvl.text = "VIP" + (m_uShowVipLvl + 1).ToString();
		}
		else
		{
			m_LabelCurLvl.text = "VIP" + m_uShowVipLvl.ToString();
			m_LabelTip2.text = "";
			m_LabelNextLvl.text = "";
		}
		
		XCfgVipShow xCfgVipShow = null;
		SortedList<uint, XCfgVipShow> cfgVipShowList = XCfgVipShowMgr.SP.GetGroup(m_uShowVipLvl);
		if(cfgVipShowList != null)
		{
			GameObject tempGroup = null;
			foreach(KeyValuePair<uint, XCfgVipShow> kvpItem in  cfgVipShowList )
			{
				xCfgVipShow = kvpItem.Value;
			
				tempGroup = XUtil.Instantiate(m_TempItem, m_TeQuanGrid.gameObject.transform, Vector3.zero, Vector3.zero);	
				tempGroup.SetActive(true);
				m_arrayItemObject.Add (tempGroup);
			
				//UILabel label1 = tempGroup.GetComponentInChildren<UILabel>(true);
				UILabel label1 = tempGroup.transform.Find("Label1").GetComponent<UILabel>();
				UILabel label2 = tempGroup.transform.Find("Label2").GetComponent<UILabel>();			
				UILabel label3 = tempGroup.transform.Find("Label3").GetComponent<UILabel>();
				label1.text = xCfgVipShow.ItemDesc;
				label2.text = xCfgVipShow.CurDesc;
				label3.text = xCfgVipShow.NextDesc;
			}
		
			m_TeQuanGrid.repositionNow	= true;
		}
		
		
		
		XCfgVipAward cfgVipAward = XCfgVipAwardMgr.SP.GetConfig(m_uShowVipLvl);
		if(cfgVipAward != null)
		{
			m_LabelAwardLvl.text = "VIP" + m_uShowVipLvl.ToString() + XStringManager.SP.GetString(729);
			if(XVipManager.SP.CanGotVipAward(m_uShowVipLvl))
			{
				m_ButGotAward.SetActive(true);
			}
			else
			{
				m_ButGotAward.SetActive(false);
			}
			
			int index = 0;
			if(cfgVipAward.Money != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(EVipConst.eVIP_AWARD_Money, XStringManager.SP.GetString(421) + cfgVipAward.Money.ToString(), m_Atlas);
				index++;
			}
			
			if(cfgVipAward.RealMoney != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(EVipConst.eVIP_AWARD_RealMoney, XStringManager.SP.GetString(727) + cfgVipAward.Money.ToString(), m_Atlas);
				index++;
			}
			
			if(cfgVipAward.Item1Id != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item1Id,  cfgVipAward.Item1Count);
				index++;
			}
			
			if(cfgVipAward.Item2Id != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item2Id,  cfgVipAward.Item2Count);
				index++;
			}
			
			if(cfgVipAward.Item3Id != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item3Id,  cfgVipAward.Item3Count);
				index++;
			}
			
			if(cfgVipAward.Item4Id != 0)
			{
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item4Id,  cfgVipAward.Item4Count);
				index++;
			}
			
			if(cfgVipAward.Item5Id != 0)
			{
				if(index >= (int)EVipConst.eMAX_VIP_AWARD_ITEM)
					return;
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item5Id,  cfgVipAward.Item5Count);
				index++;
			}
			
			if(cfgVipAward.Item6Id != 0)
			{
				if(index >= (int)EVipConst.eMAX_VIP_AWARD_ITEM)
					return;
				m_xVipAwardItem[index].Show();
				m_xVipAwardItem[index].ShowInfo(cfgVipAward.Item6Id,  cfgVipAward.Item6Count);
			}
		}
	}
	
	public uint GetVipLvlMoney(uint uLvl)
	{
		XCfgVipAward xcfgVipAward = XCfgVipAwardMgr.SP.GetConfig(uLvl);
		
		if(xcfgVipAward != null)
			return xcfgVipAward.Recharge;
		
		return 0;
	}
	
	public UInt64 GetNextLvlMoney()
	{
		return GetVipLvlMoney(XLogicWorld.SP.MainPlayer.VIPLevel + 1) - XLogicWorld.SP.MainPlayer.Recharge;
	}
	
	
	public void UpdateInfo()
	{
		if(m_bShowCur)
		{
			m_uShowVipLvl = XLogicWorld.SP.MainPlayer.VIPLevel;
			m_ButMoreAndBack.GetComponentInChildren<UILabel>().text = XStringManager.SP.GetString(726);
		}
		
		ShowInfo();
	}
}

