using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public class XCareerInfo
{
	public XCareerInfo(byte careerId, byte lvl, ushort exp)
	{
		this.CareerId = careerId;
		this.Level = lvl;
		this.Exp = exp;
	}
	public byte CareerId;
	public byte Level;
	public ushort Exp;
}

public class XProductManager : XSingleton<XProductManager>
{
	private uint m_nStrength;
	private List<XCareerInfo> m_CareerInfo;

	private bool m_bIsInProduct;
	private int m_lastWeapon = -1;
	private XCfgGatherObject m_curCfgGatherObject;	// 当前试图采集的对象
	private XCfgFormula m_curCfgFormula;	// 当前试图制作的配方
	
	private float m_productRockon;
	private int m_productTime = 0;

	private float m_breatheRockon = 0f;
	private float m_breatheInterval = 0.01f;
	
	public XProductManager()
	{
		m_CareerInfo = new List<XCareerInfo>();
		for(int i=0; i<(int)EProductCareerType.eProductCareerType_Count; i++)
		{
			m_CareerInfo.Add(null);
		}
		m_nStrength = 0;
		m_bIsInProduct = false;
		m_productRockon = 0f;
		m_curCfgFormula = null;
		m_curCfgGatherObject = null;
	}
	
	public void Breathe()
	{
		if(!m_bIsInProduct || m_productRockon <= 0f)
			return;
		
		float fNow = Time.time;
		if(fNow - m_breatheRockon < m_breatheInterval) return;
		
		m_breatheRockon = fNow;
		float fd = fNow - m_productRockon;
		int nd = m_productTime - (int)fd;
		if(nd < 0 ) nd = 0;
		XEventManager.SP.SendEvent(EEvent.ToolTip_ReadTip_Progress, XStringManager.SP.GetString(1001) + XUtil.GetTimeStrByInt(nd, 0));
		XEventManager.SP.SendEvent(EEvent.ToolTip_ReadTip_Progress, fd, (float)m_productTime);
	}
	
	public XCareerInfo GetCareerInfo(byte btType)
	{
		if(0 > btType || btType >= m_CareerInfo.Count)
			return null;
		return m_CareerInfo[btType];
	}
	
	public void On_SC_ActiveCareer(SC_ActiveCareer msg)
	{
		uint ns = 0;
		if(msg.HasStrength) ns = msg.Strength;
		On_SC_SetStrength(ns);
		
		foreach(int data in msg.CareerInfoList)
		{
			byte id = (byte)(data >> 24);
			byte lvl = (byte)(data >> 16);
			ushort exp = (byte)(data);
			On_SC_LearnCareer(id, lvl, exp);
		}
		
		//gather_rec
		foreach(uint data in msg.GatherRecList)
		{
			On_SC_AddGatherRec(data);
		}
		
		// formula
		foreach(uint data in msg.FormulaList)
		{
			On_SC_AddFormula(data);
		}
	}
	
	public void On_SC_LearnCareer(byte btCareerId, byte btLevel, ushort exp)
	{
		XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(btCareerId, btLevel);
		if(null == cfg || cfg.Kind >= m_CareerInfo.Count)
			return;
		
		if(null != m_CareerInfo[cfg.Kind])
		{
			Log.Write(LogLevel.WARN, "XProductManager, server order client to learn a career which exists");
			return;
		}
		XCareerInfo careerInfo = new XCareerInfo(btCareerId, btLevel, exp);
		m_CareerInfo[cfg.Kind] = careerInfo;
		XEventManager.SP.SendEvent(EEvent.product_LearnCareer, cfg.Kind, careerInfo);
	}
	
	public void On_SC_UpgradeCareer(byte btType, byte btLevel)
	{
		if(btType >= m_CareerInfo.Count)
			return;
		
		if(null == m_CareerInfo[btType])
		{
			Log.Write(LogLevel.WARN, "XProductManager, server order client to upgrade a career which does not exist");
			return;
		}
		m_CareerInfo[btType].Level = btLevel;
		XEventManager.SP.SendEvent(EEvent.product_UpgradeCareer, btType, m_CareerInfo[btType]);
	}
	
	public void On_SC_ForgetCareer(byte btType)
	{
		if(btType >= m_CareerInfo.Count)
			return;
		
		if(null == m_CareerInfo[btType])
		{
			Log.Write(LogLevel.WARN, "XProductManager, server order client to forget a career which does not exist");
			return;
		}
		m_CareerInfo[btType] = null;
		XEventManager.SP.SendEvent(EEvent.product_ForgetCareer, btType);
	}
	
	public void On_SC_SetStrength(uint nStrength)
	{
		m_nStrength = nStrength;
		XEventManager.SP.SendEvent(EEvent.product_Strength, m_nStrength, 200);
	}
	
	public void On_SC_SetExp(byte btType, ushort wExp)
	{
		XCareerInfo info = GetCareerInfo(btType);
		int addExp = wExp - info.Exp;
		if( null == info ) 
			return;
		info.Exp = wExp;
		
		if ( addExp > 0 )
		{
			XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format(XStringManager.SP.GetString(1030), cfg.Name, addExp));
		}
		
		XEventManager.SP.SendEvent(EEvent.product_Exp, btType, info);
	}
	
	public void On_SC_AddGatherRec(uint nGatherId)
	{
		XEventManager.SP.SendEvent(EEvent.product_AddGatherRec, nGatherId);
	}
	
	public void On_SC_AddFormula(uint nFormulaId)
	{
		XEventManager.SP.SendEvent(EEvent.product_AddFormula, nFormulaId);
	}
	
	public void On_SC_GatherOnceOver()
	{
		if(!m_bIsInProduct) return;
		m_productRockon = Time.time;		
	}
	
	public void On_SC_ProduceOnceOver(SC_ProduceOnce msg)
	{
		//if(!m_bIsInProduct) return;
		//m_productRockon = Time.time;
		
		// 客户端自己扣除物品
		foreach(SC_Int mi in msg.DelItemList)
		{
			XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)mi.Uid, (uint)mi.Data);
		}
	}
	
	public void On_SC_ProductError(int data)
	{
		if(!m_bIsInProduct) 
			return;
		EProductCareerType pcType = (EProductCareerType)(data >> 16);
		short nResult = (short)(data);
		switch(pcType)
		{
			// 采集
			case EProductCareerType.eProductCareerType_Gather:
			{
				XCfgProductCareer cfgProductCareer = XCfgProductCareerMgr.SP.GetConfig(m_curCfgGatherObject.NeedCareer, 1);
				if ( null == cfgProductCareer )
					return;
				switch(nResult)
				{
					case -1:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1017), cfgProductCareer.Name));
						
						break;
					case -2:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1018), cfgProductCareer.Name));
				
						break;
					case -3:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1019), cfgProductCareer.Name));
				
						break;
					case -4:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1021), cfgProductCareer.Name));
				
						break;
					case -5:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1020), cfgProductCareer.Name));
					break;
				}
			}
			
			// 制造
				break;
			case EProductCareerType.eProductCareerType_Produce:
			{
				XCfgProductCareer cfgProductCareer = XCfgProductCareerMgr.SP.GetConfig(m_curCfgGatherObject.NeedCareer, 1);
				if ( null == cfgProductCareer )
					return;
			
				switch(nResult)
				{
					case -1:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1017), cfgProductCareer.Name));
						break;
					case -2:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1018), cfgProductCareer.Name));
				
						break;
					case -3:
				
						break;
					case -4:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1027));
				
						break;
					case -5:
						XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
							string.Format(XStringManager.SP.GetString(1020), cfgProductCareer.Name));
					break;
				}
				m_curCfgFormula = null;
			}
			
			// 默认
				break;
			default:
				break;	
		}
		m_bIsInProduct = false;
		m_curCfgFormula = null;
		m_curCfgGatherObject = null;
		updateWeapon();
		XLogicWorld.SP.MainPlayer.QuitProduct();
	}
	
	public void On_SC_StartProduct(Int64 data)
	{
		if(!m_bIsInProduct) 
			return;
		EProductCareerType pcType = (EProductCareerType)(data >> 32);
		switch(pcType)
		{
			case EProductCareerType.eProductCareerType_Gather:
			{
				XCfgProductCareer cfgProductCareer = XCfgProductCareerMgr.SP.GetConfig(m_curCfgGatherObject.NeedCareer, 1);
				string str = "";
				if(null != cfgProductCareer) str = cfgProductCareer.Name;
				DisplayReadTip(string.Format(XStringManager.SP.GetString(1023), str, m_curCfgGatherObject.Name)
					, (int)(m_curCfgGatherObject.CostTime));
			}
			break;
			
			case EProductCareerType.eProductCareerType_Produce:
			{
				XCfgItem itemCfg = XCfgItemMgr.SP.GetConfig(m_curCfgFormula.OutputItemId);
				DisplayReadTip(string.Format(XStringManager.SP.GetString(1026), (null != itemCfg) ? itemCfg.Name : m_curCfgFormula.Name), (int)m_curCfgFormula.CostTime);
			}
			break;
		}
	}
	
	public void On_SC_EndProduct()
	{
		if(!m_bIsInProduct) 
			return;
		
		updateWeapon();
		m_bIsInProduct = false;
		m_curCfgFormula = null;
		m_curCfgGatherObject = null;
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eReadTip);
		m_productRockon = 0f;
		XLogicWorld.SP.MainPlayer.QuitProduct();
	}
	
	public void ApplyLearnCareer(byte btCareerId)
	{
		XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(btCareerId, 1);
		if(null == cfg || null != GetCareerInfo(cfg.Kind))
			return;
		
		if(XLogicWorld.SP.MainPlayer.Level < cfg.NeedPlayerLevel)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format(XStringManager.SP.GetString(1016), cfg.NeedPlayerLevel));
			return;
		}
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(btCareerId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyLearnCareer, builder.Build());
	}
	
	public void ApplyUpgradeCareer(byte btType)
	{
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(btType);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyUpgradeCareer, builder.Build());
	}
	
	public void ApplyForgetCareer(byte btType)
	{
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(btType);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyForgetCareer, builder.Build());
	}
	
	internal void DisplayReadTip(string str, int costTime)
	{
		// 开始
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eReadTip);
		XEventManager.SP.SendEvent(EEvent.ToolTip_ReadTip_Discription, str);
		m_productTime = costTime;
		m_productRockon = Time.time;
		XEventManager.SP.SendEvent(EEvent.ToolTip_ReadTip_Progress, XStringManager.SP.GetString(1001) + XUtil.GetTimeStrByInt(m_productTime, 0));
		XEventManager.SP.SendEvent(EEvent.ToolTip_ReadTip_Progress, 0f, (float)m_productTime);
	}
	
	internal void ApplyGatherObjectByMessageBox(GameObject go)
	{
		ApplyGatherObject(m_curCfgGatherObject, false);
	}
	
	internal void ApplyGatherObject(XCfgGatherObject cfgGatherObject, bool b) // b->是否验证背包格子数量少于3个
	{
		if( m_bIsInProduct ) 
			return;
		m_curCfgGatherObject = cfgGatherObject;
		
		XCfgProductCareer cfgProductCareer = XCfgProductCareerMgr.SP.GetConfig(cfgGatherObject.NeedCareer, 1);
		if(null == cfgProductCareer) return;
		
		// 需要专业
		XCareerInfo careerInfo = GetCareerInfo((byte)EProductCareerType.eProductCareerType_Gather);
		if(null == careerInfo || careerInfo.CareerId != cfgGatherObject.NeedCareer)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
				string.Format(XStringManager.SP.GetString(1017), cfgProductCareer.Name));
			return;
		}
		// 需要体力
		if(m_nStrength < cfgGatherObject.CostStrength)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
				string.Format(XStringManager.SP.GetString(1018), cfgProductCareer.Name));
			return;
		}
		// 需要熟练度
		if(careerInfo.Exp < cfgGatherObject.NeedExp)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
				string.Format(XStringManager.SP.GetString(1019), cfgProductCareer.Name));
			return;
		}
		
		//检测背包状态
		int nSpace = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		if(nSpace <= 0)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
				string.Format(XStringManager.SP.GetString(1021), cfgProductCareer.Name));
			return;
		}
		
		if(b && nSpace <= 2)
		{
			UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate(ApplyGatherObjectByMessageBox);
			XEventManager.SP.SendEvent(EEvent.MessageBox, funcOK, null, string.Format(XStringManager.SP.GetString(1022), cfgProductCareer.Name));
			return;
		}
		
		// 客户端先进入采集状态:
		if(!XLogicWorld.SP.MainPlayer.StartProduct((EAnimName)cfgProductCareer.AnimName))
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, 
				string.Format(XStringManager.SP.GetString(1020), cfgProductCareer.Name));
			return;
		}
		
		CS_Int64.Builder builder = CS_Int64.CreateBuilder();
		Int64 data = (Int64)(EProductCareerType.eProductCareerType_Gather);
		data <<= 56; data |= cfgGatherObject.ID;
		builder.SetData(data);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyProduct, builder.Build());
		
		uint tool = 0u;
		if ( 3 == cfgProductCareer.ID )
			tool = 087004u;
		else if ( 2 == cfgProductCareer.ID )
			tool = 087005u;
		
		if ( tool != XLogicWorld.SP.MainPlayer.WeaponModelId )
		{
			m_lastWeapon = (int)XLogicWorld.SP.MainPlayer.WeaponModelId;
			XLogicWorld.SP.MainPlayer.WeaponModelId = tool;
		}
		
		m_bIsInProduct = true;
	}
	
	internal void ApplyProduceFormula(XCfgFormula cfgFormula, int nExpectNum, out int nMaxNum)
	{
		if(m_bIsInProduct) 
		{
			nMaxNum = -1;
			return;
		}

		// 体力检测
		if(m_nStrength < cfgFormula.CostStrength)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1024));
			nMaxNum = -1;
			return;
		}
		
		// 物品检测
		nMaxNum = 0;
		for(int i=0; i<cfgFormula.NeedItemId.Length; i++)
		{
			uint itemId = cfgFormula.NeedItemId[i];
			ushort itemNum = cfgFormula.NeedItemNum[i];
			if(0 == itemId || 0 == itemNum)
				continue;
			
			ushort itemRealNum = (ushort)(XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(itemId));
			int n = (int)(itemRealNum / itemNum);
			if(0 == i) 
			{
				nMaxNum = n;
			}
			else if(n < nMaxNum)
			{
				nMaxNum = n;
			}
			if(0 == nMaxNum)
			{
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1025));
				return;
			}
		}
		if(nExpectNum > 0 && nExpectNum > nMaxNum)
		{
			nExpectNum = nMaxNum;
		}
		
		// 检测背包状态
		int spaceNum = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		if(spaceNum <= 0)
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1029));
			return;
		}
		
		// 客户端先进入制造状态
		if(!XLogicWorld.SP.MainPlayer.StartProduct(EAnimName.Idle))
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1028));
			return;
		}		

		m_bIsInProduct = true;
		m_curCfgFormula = cfgFormula;
		
		CS_Int64.Builder builder = CS_Int64.CreateBuilder();
		Int64 data = (Int64)(EProductCareerType.eProductCareerType_Produce);
		data <<= 56;
		Int64 tmp = (Int64)nExpectNum; tmp <<= 32;
		data |= tmp; data |= cfgFormula.ID;
		builder.SetData(data);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyProduct, builder.Build());
	}
	
	private void updateWeapon()
	{
		uint curWeapon = XLogicWorld.SP.MainPlayer.WeaponModelId;
		if ( curWeapon != m_lastWeapon && m_lastWeapon >= 0 )
			XLogicWorld.SP.MainPlayer.WeaponModelId = (uint)m_lastWeapon;
		m_lastWeapon = -1;
	}
	
	public static string GetCareerName(byte CareerID,byte level)
	{
		XCfgProductCareer career = XCfgProductCareerMgr.SP.GetConfig(CareerID, level);
		if(career == null)
			return "";
		
		return career.Name;
	}
}