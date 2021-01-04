using System;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

public class XBuff
{
	public uint BuffId { get; private set; }
	public byte BuffLevel { get; private set; }
	public int RemainLife;
	private float m_rockon; 
	private float m_addRockon;
	
	internal XCfgBuffBase CfgBuffBase { get; private set; }
	internal XCfgBuffLevel CfgBuffLevel { get; private set; }
	
	public byte BuffLayer { get; private set; }
	public XCharacter Owner { get; private set; }
	
	private List<XU3dEffect> m_EffectOnAdd;
	private List<XU3dEffect> m_EffectOnAttach;
	private List<XU3dEffect> m_EffectOnDisperse;
	private List<XU3dEffect> m_EffectOnRemove;
	
	public XBuff(uint nBuffId, byte btBuffLevel, byte btBuffLayer, XCharacter owner)
	{
		BuffId = nBuffId;
		BuffLevel = btBuffLevel;
		BuffLayer = btBuffLayer;
		
		Owner = owner;
		m_EffectOnAdd = new List<XU3dEffect>();
		m_EffectOnAttach = new List<XU3dEffect>();
		m_EffectOnDisperse = new List<XU3dEffect>();
		m_EffectOnRemove = new List<XU3dEffect>();
	}
	
	public bool Init(bool bAttach, int nRemainLife)
	{
		CfgBuffBase = XCfgBuffBaseMgr.SP.GetConfig(BuffId);
		CfgBuffLevel = XCfgBuffLevelMgr.SP.GetConfig(BuffId, BuffLevel);
		bool bRet = null != CfgBuffBase && null != CfgBuffLevel;
		if(bRet)
		{
			if(bAttach)	RemainLife = nRemainLife;
			else RemainLife = getLifeValue();
			m_rockon = Time.time;
		}
		return bRet;
	}
	
	public void Breathe()
	{
		if(0 == m_addRockon)
			return;
		
		if(Time.time - m_addRockon > CfgBuffBase.EffectDelayOnAttach)
		{
			if(Owner.IsAppear)
			{
				DisplayAttachEffect();
			}
			m_addRockon = 0f;
		}
	}
	
	public void OnAdd(bool bAttach)
	{
		m_addRockon = 0f;
		if(Owner.IsAppear)
		{
			if(!bAttach && CfgBuffBase.EffectOnAdd > 0)
			{
				for(int i=0; i<CfgBuffBase.EffectNumOnAdd; i++)
				{
					XU3dEffect effect = new XU3dEffect(CfgBuffBase.EffectOnAdd);
					Owner.AttachGo((ESkeleton)(CfgBuffBase.EffectSkeOnAdd), effect.m_gameObject, CfgBuffBase.EffectPosOnAdd, Vector3.zero);
					m_EffectOnAdd.Add(effect);
				}
				m_addRockon = Time.time;
			}
			else
			{
				DisplayAttachEffect();
			}
		}
		
		if(Owner == XLogicWorld.SP.MainPlayer)
		{
			XEventManager.SP.SendEvent(EEvent.buff_OnAddBuff, Owner, this);
		}
		if(!bAttach && Owner.Visible) DisplayHint();
	}
	
	public void DisplayHint()
	{
		/*if((byte)EBuffType.eBuffType_Buff == CfgBuffBase.BuffType)
		{
			if(Owner == XLogicWorld.SP.MainPlayer)
			{
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, "[color=00FF00][" + CfgBuffBase.Name + "]", 1.5f);
			}
			else // 方便测试, 之后删除
			{
				Owner.FlyStringHalf(EObjectHalfHintType.eHalfHint_Up, "[color=00FF00][" + CfgBuffBase.Name + "]");
			}
		}
		else
		{
			if(Owner == XLogicWorld.SP.MainPlayer)
			{
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Down, "[color=acc33e][" + CfgBuffBase.Name + "]", 1.5f);
			}
			else
			{
				Owner.FlyStringHalf(EObjectHalfHintType.eHalfHint_Up, "[color=00FF00][" + CfgBuffBase.Name + "]");
			}
		}*/
	}
	
	public void DisplayAttachEffect()
	{
		if(CfgBuffBase.EffectOnAttach > 0)
		{
			for(int i=0; i<CfgBuffBase.EffectNumOnAttach; i++)
			{
				XU3dEffect effect = new XU3dEffect(CfgBuffBase.EffectOnAttach);
				Owner.AttachGo((ESkeleton)(CfgBuffBase.EffectSkeOnAttach), effect.m_gameObject, CfgBuffBase.EffectPosOnAttach, Vector3.zero);
				m_EffectOnAttach.Add(effect);
			}
		}
	}
		
	private int getLifeValue()
	{
		if((int)EBuffLifeOverlayType.eBuffLifeOverlayType_Add == CfgBuffBase.LifeOverlayType)
			return (int)CfgBuffBase.LifeValue * BuffLayer;
		return (int)CfgBuffBase.LifeValue;
	}
	
	public void Overlay(byte btLayer)
	{
		byte tLayer = (byte)(BuffLayer + btLayer);
		if(tLayer > CfgBuffBase.MaxOverLayNum)
			tLayer = CfgBuffBase.MaxOverLayNum;
		SetLayer(tLayer);
	}
	
	public void SetLayer(byte btLayer)
	{
		if(btLayer > BuffLayer)
		{
			if((int)EBuffLifeOverlayType.eBuffLifeOverlayType_Add == CfgBuffBase.LifeOverlayType)
			{
				RemainLife += (int)CfgBuffBase.LifeValue * (btLayer - BuffLayer);
			}
			else if((int)EBuffLifeOverlayType.eBuffLifeOverlayType_Reset == CfgBuffBase.LifeOverlayType)
			{
				m_rockon = Time.time; 
				RemainLife = getLifeValue();
			}
		}
		else
		{
			if((int)EBuffLifeOverlayType.eBuffLifeOverlayType_Add == CfgBuffBase.LifeOverlayType)
			{
				RemainLife -= (int)CfgBuffBase.LifeValue * (btLayer - BuffLayer);
			}
			
			if(Owner.IsAppear)
			{
				// 播放驱散特效
				for(int i=0; i<CfgBuffBase.EffectNumOnDisperse; i++)
				{
					XU3dEffect effect = new XU3dEffect(CfgBuffBase.EffectOnDisperse);
					Owner.AttachGo((ESkeleton)(CfgBuffBase.EffectSkeOnDisperse), effect.m_gameObject, CfgBuffBase.EffectPosOnAttach, Vector3.zero);
					m_EffectOnDisperse.Add(effect);
				}
			}
			if(RemainLife <= 0)
			{
				Owner.BuffOper.RemoveBuff(BuffId);
				return;
			}
		}
		
		BuffLayer = btLayer;
		if(Owner == XLogicWorld.SP.MainPlayer)
		{
			XEventManager.SP.SendEvent(EEvent.buff_OnSetLayer, Owner, this);
		}
	}

	public void SetCount(int nCount)
	{
		RemainLife = nCount;
		if(RemainLife <= 0)
			Owner.BuffOper.RemoveBuff(BuffId);
	}
	
	public void DecCount(int nDecCount)
	{
		RemainLife -= nDecCount;
		if(RemainLife <= 0)
			Owner.BuffOper.RemoveBuff(BuffId);
	}
	
	public void Disperse()
	{
		if(BuffLayer <= CfgBuffBase.DisperseNum)
		{
			Owner.BuffOper.RemoveBuff(BuffId);
			return;
		}
		else
		{
			SetLayer((byte)(BuffLayer - (byte)CfgBuffBase.DisperseNum));
		}
	}
	
	public void OnRemove()
	{
		if(Owner.IsAppear)
		{
			// 播放删除特效
			for(int i=0; i<CfgBuffBase.EffectNumOnRemove; i++)
			{
				XU3dEffect effect = new XU3dEffect(CfgBuffBase.EffectOnRemove);
				Owner.AttachGo((ESkeleton)(CfgBuffBase.EffectSkeOnRemove), effect.m_gameObject, CfgBuffBase.EffectPosOnAttach, Vector3.zero);
				m_EffectOnRemove.Add(effect);
			}
		}

		if(Owner == XLogicWorld.SP.MainPlayer)
		{
			XEventManager.SP.SendEvent(EEvent.buff_OnRemoveBuff, Owner, this);	
		}
		
		if(CfgBuffLevel.ModelId > 0)
		{
			Owner.SetModel(EModelCtrlType.eModelCtrl_ByBuff, 0);
		}

		destroyEffectArr(ref m_EffectOnAdd);
		destroyEffectArr(ref m_EffectOnAttach);
		destroyEffectArr(ref m_EffectOnDisperse);
	}
	
	public bool IsDeadDisappear
	{
		get
		{
			return ((byte)EBuffDisappearType.eBuffDisappearType_OnDead & CfgBuffBase.DisappearType) > 0;
		}
	}
	
	public void Appear()
	{
		DisplayAttachEffect();
	}
	
	public void Disappear()
	{
		destroyEffectArr(ref m_EffectOnAdd);
		destroyEffectArr(ref m_EffectOnAttach);
		destroyEffectArr(ref m_EffectOnDisperse);
		destroyEffectArr(ref m_EffectOnRemove);
	}
			
	private void destroyEffectArr(ref List<XU3dEffect> effectArr)
	{
		foreach(XU3dEffect effect in effectArr)
			effect.Destroy();
		effectArr.Clear();
	}
	
	public string GetBuffDiscription()
	{
		string str = "" + CfgBuffBase.Name + " " + XStringManager.SP.GetString(2) + BuffLevel + "\n";
		//str += CfgBuffBase.Name + "\n";
		if((int)EBuffLifeType.eBuffLifeType_Count == CfgBuffBase.LifeType)
		{
			str += XStringManager.SP.GetString(1002);
			str += "" + RemainLife + "/" + getLifeValue();
		}
		else
		{
			str += XStringManager.SP.GetString(1001);
			if((int)EBuffLifeType.eBuffLifeType_OfflineTime != CfgBuffBase.LifeType)
			{
				float now = Time.time;
				RemainLife -= (int)(now - m_rockon);
				m_rockon = now;
			}
			if(RemainLife <= 0) str += "0s";
			else str += XUtil.GetTimeStrByInt(RemainLife, 2);
		}
		str += "\n" + CfgBuffLevel.BuffDiscription.Replace("\\n", "\n");
		return str;
	}
}


