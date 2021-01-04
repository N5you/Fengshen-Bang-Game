using System;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Packets;

public class XBuffDisplay
{
	private XBattlePosition m_BP = null;
	private List<int> hp;
	private List<int> u3dEffect;
	private List<Int64> buff;
	
	public XBuffDisplay(PB_OneTgtBuffEffect pb)
	{
		m_BP = XBattlePosition.Create(pb.TgtPos);
		hp = new List<int>();
		u3dEffect = new List<int>();
		buff = new List<long>();
		hp.AddRange(pb.HpList);
		u3dEffect.AddRange(pb.U3DEffectList);
		buff.AddRange(pb.BuffCtrlList);
	}
	
	public void Display()
	{
		XCharacter tgt = BattleDisplayerMgr.SP.GetBattleObject(m_BP);
		EFlyStrType lt = EFlyStrType.eFlyStrType_LittleStr1;
		for(int i=0; i<hp.Count; i+=2)
		{
			int nHp = hp[i]; // 要表现的血量变化
			if(nHp >= 0)
			{
				tgt.FlyString(lt, "[color=00FF00]+" + nHp);
			}
			else
			{
				tgt.FlyString(lt, "[color=ff1515]" + nHp);
			}
			lt++;
			if(lt > EFlyStrType.eFlyStrType_LittleStr6)
				lt = EFlyStrType.eFlyStrType_LittleStr1;
			tgt.Hp += hp[i+1];	// 真实的血量变化
		}
		for(int i=0; i<u3dEffect.Count; i++)
		{
			XU3dEffect effect = new XU3dEffect((uint)(u3dEffect[i]));
			tgt.AttachGo(ESkeleton.eMainObject, effect.m_gameObject);
		}
		for(int i=0; i<buff.Count; i++)
		{
			Int64 ld = buff[i];
			uint buffId = (uint)(ld >> 32);
			int userData = (int)(ld); 
			userData >>= 8;
			byte tmp = (byte)(ld);
			EBuffEffectDisplayType dt = (EBuffEffectDisplayType)(tmp);
			switch(dt)
			{
			case(EBuffEffectDisplayType.BUFF_DISPLAY_TYPE_EXTRA_BUFF):
				tgt.BuffOper.AddBuff(buffId, (byte)userData, 1, false, 0);
				break;
				
			case(EBuffEffectDisplayType.BUFF_DISPLAY_TYPE_DISPERSE_BUFF):
				tgt.BuffOper.DisperseBuff(buffId);
				break;
				
			case(EBuffEffectDisplayType.BUFF_DISPLAY_TYPE_REMOVE_BUFF):
				tgt.BuffOper.RemoveBuff(buffId);
				break;
				
			case(EBuffEffectDisplayType.BUFF_DISPLAY_TYPE_BUFF_COUNT):
				tgt.BuffOper.DecBuffCount(buffId, userData);
				break;
			}
		}
	}
}

// 一组buff效果
public class XBuffDisplayGroup
{
	private List<XBuffDisplay> m_eff;
	
	public XBuffDisplayGroup(PB_SameTimeBuffEffect pb)
	{
		m_eff = new List<XBuffDisplay>();
		
		for(int i=0; i<pb.TgtBuffEffectArrCount; i++)
		{
			PB_OneTgtBuffEffect ote = pb.GetTgtBuffEffectArr(i);
			m_eff.Add(new XBuffDisplay(ote));
		}
	}
	
	public void Display()
	{
		for(int i=0; i<m_eff.Count; i++)
		{
			m_eff[i].Display();
		}
	}
}

// buff效果综合
public class XBuffDisplayMulti
{
	private SortedList<EBuffEffectDisplayTime, XBuffDisplayGroup> m_mulEff;
	
	public XBuffDisplayMulti(IList<PB_SameTimeBuffEffect> pbArr)
	{
		m_mulEff = new SortedList<EBuffEffectDisplayTime, XBuffDisplayGroup>();

		foreach(PB_SameTimeBuffEffect pb in pbArr)
		{
			EBuffEffectDisplayTime dt = pb.DisplayTime;
			if(m_mulEff.ContainsKey(dt))
				continue;
			
			m_mulEff.Add(dt, new XBuffDisplayGroup(pb));
		}
	}
	
	public void Display(EBuffEffectDisplayTime dt)
	{
		if(!m_mulEff.ContainsKey(dt))
			return;
		
		m_mulEff[dt].Display();
	}
}









