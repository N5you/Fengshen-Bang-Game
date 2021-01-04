using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

public class XBuffOper
{	
	private class BuffModelCmp : IComparer<XBuff>
	{
		public int Compare(XBuff x, XBuff y)
		{
			if(x.CfgBuffBase.AffectPriority != y.CfgBuffBase.AffectPriority)
				return y.CfgBuffBase.AffectPriority - x.CfgBuffBase.AffectPriority;
			return (int)(x.BuffId - y.BuffId);
		}
	}
	
	public SortedList<uint, XBuff> m_ExistBuff { get; private set;}
	private SortedList<XBuff, uint> m_ModelBuff;
	private XBuff m_curModelBuff = null;

	private XCharacter Owner;
	
	public XBuffOper(XCharacter owner)
	{
		m_ExistBuff = new SortedList<uint, XBuff>();
		Owner = owner;
		m_ModelBuff = new SortedList<XBuff, uint>(new BuffModelCmp());
	}
	
	public void Breathe()
	{
		foreach(XBuff buff in m_ExistBuff.Values)
		{
			buff.Breathe();
		}
	}
	
	public void AddBuff(uint nBuffId, byte btBuffLevel, byte btLayer, bool bAttach, int nRemainLife)
	{
		if(m_ExistBuff.ContainsKey(nBuffId))
		{
			m_ExistBuff[nBuffId].Overlay(btLayer);
			return;
		}
		XBuff buff = new XBuff(nBuffId, btBuffLevel, btLayer, Owner);
		if(!buff.Init(bAttach, nRemainLife))
		{
			Log.Write(LogLevel.WARN, "fail to init buff with id : {0}, level : {1}", nBuffId, btBuffLevel);
			return;
		}
		m_ExistBuff.Add(nBuffId, buff);
		buff.OnAdd(bAttach);
		addModelBuff(buff);
	}
	
	public void SetBuffLayer(uint nBuffId, byte btLayer)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
		{
			Log.Write(LogLevel.WARN, "try to set layer of a buff which does not exists: {0}", nBuffId);
			return;
		}
		m_ExistBuff[nBuffId].SetLayer(btLayer);
	}
	
	// nCount为剩余的
	public void SetBuffCount(uint nBuffId, int nCount)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
		{
			Log.Write(LogLevel.WARN, "try to set count of a buff which does not exists: {0}", nBuffId);
			return;
		}
		m_ExistBuff[nBuffId].SetCount(nCount);
	}

	// nDecCount为消耗的
	public void DecBuffCount(uint nBuffId, int nDecCount)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
		{
			Log.Write(LogLevel.WARN, "try to set count of a buff which does not exists: {0}", nBuffId);
			return;
		}
		m_ExistBuff[nBuffId].DecCount(nDecCount);
	}
		
	public void RemoveBuff(uint nBuffId)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
		{
			Log.Write(LogLevel.WARN, "Try to remove a buff which doesnot exists: {0}", nBuffId);
			return;
		}
		XBuff buff = m_ExistBuff[nBuffId];
		buff.OnRemove();
		delModelBuff(buff);
		m_ExistBuff.Remove(nBuffId);
	}
	
	public void DisperseBuff(uint nBuffId)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
		{
			Log.Write(LogLevel.WARN, "Try to disperse a buff which doesnot exits: {0}", nBuffId);
			return;
		}
		m_ExistBuff[nBuffId].Disperse();
	}
	
	public XBuff GetBuff(uint nBuffId)
	{
		if(!m_ExistBuff.ContainsKey(nBuffId))
			return null;
		return m_ExistBuff[nBuffId];
	}
	
	public void Appear()
	{
		foreach(XBuff buff in m_ExistBuff.Values)
		{
			buff.Appear();
		}
	}
	
	public void Disappear()
	{
		foreach(XBuff buff in m_ExistBuff.Values)
		{
			buff.Disappear();
		}
	}
		
	public void Clear()
	{
		Disappear();
		m_ExistBuff.Clear();
	}
	
	public void OnDead()
	{
		List<uint> removed = new List<uint>();
		foreach(XBuff buff in m_ExistBuff.Values)
		{
			if(buff.IsDeadDisappear)
				removed.Add(buff.BuffId);
		}
		for(int i=0; i<removed.Count; i++)
		{
			RemoveBuff(removed[i]);
		}
		
	}
	
	private void AddBuffChangeModel(XBuff buff)
	{
		if(null == buff || null == buff.CfgBuffBase || 0 == buff.CfgBuffLevel.ModelId)
			return;
		if(m_ModelBuff.ContainsKey(buff))
		{
			Log.Write(LogLevel.WARN, "XBuffOper, logic error try push buffmodel which exists {0}", buff.BuffId);
			return;
		}
		m_ModelBuff.Add(buff, 0);
		
		if(m_curModelBuff == m_ModelBuff.Keys[0])
			return;
		
		m_curModelBuff = m_ModelBuff.Keys[0];
		Owner.SetModel(EModelCtrlType.eModelCtrl_ByBuff, m_curModelBuff.CfgBuffLevel.ModelId);
	}
	
	private void DelBuffChangeModel(XBuff buff)
	{
		if(!m_ModelBuff.ContainsKey(buff))
		{
			Log.Write(LogLevel.WARN, "XBuffOper, logic error, try to del buffmodel which does not exists {0}", buff.BuffId);
			return;
		}
		m_ModelBuff.Remove(buff);
		
		if(m_curModelBuff != buff)
			return;
		
		if(m_ModelBuff.Count > 0)
		{
			m_curModelBuff = m_ModelBuff.Keys[0];
			Owner.SetModel(EModelCtrlType.eModelCtrl_ByBuff, m_curModelBuff.CfgBuffLevel.ModelId);
		}
		else
		{
			m_curModelBuff = null;
			Owner.SetModel(EModelCtrlType.eModelCtrl_ByBuff, 0);
		}
	}
	
	private void addModelBuff(XBuff buff)
	{
		if(null == buff || null == buff.CfgBuffBase)
			return;
		if(buff.CfgBuffLevel.ModelId != 0)
		{
			AddBuffChangeModel(buff);
		}
		if(buff.CfgBuffLevel.UColor.Length >= 6)
		{
			Color effectColor = Color.white;
			string ColorStr = buff.CfgBuffLevel.UColor;
			int colorR = System.Convert.ToInt32(ColorStr.Substring(0,2),16);
			int colorG = System.Convert.ToInt32(ColorStr.Substring(2,2),16);
			int colorB = System.Convert.ToInt32(ColorStr.Substring(4,2),16);
			effectColor.r = (float)colorR/256.0f;
			effectColor.g = (float)colorG/256.0f;
			effectColor.b = (float)colorB/256.0f;
			Owner.MatColor = effectColor;
		}
		if(buff.CfgBuffLevel.Usize != (float)1.0 && buff.CfgBuffLevel.Usize != (float)0.0)
		{
			Owner.Scale = buff.CfgBuffLevel.Usize;
		}
	}
	
	private void delModelBuff(XBuff buff)
	{
		if(null == buff || null == buff.CfgBuffBase)
			return;
		
		if(buff.CfgBuffLevel.ModelId != 0)
		{
			DelBuffChangeModel(buff);
		}
		if(buff.CfgBuffLevel.UColor.Length >= 6)
		{
			Owner.MatColor = Color.white;
		}
		if(buff.CfgBuffLevel.Usize != (float)1.0 && buff.CfgBuffLevel.Usize != (float)0.0)
		{
			Owner.Scale = (float)1.0;
		}
	}
}