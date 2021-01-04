using System;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Packets;
class XTransPoint : XGameObject
{
	// 以静态传送门ID为索引的所有传送门对象列表
	private static SortedList<uint, XTransPoint> m_allTransPoint = new SortedList<uint, XTransPoint> ();
	
	public static XTransPoint GetByIndex(uint index)
	{
		if (!m_allTransPoint.ContainsKey (index))
			return null;
		return m_allTransPoint [index];
	}
	
	internal XCfgLevelEntry EntryInfo { get; private set; }
	// 进入区域标记, 用于在玩家手动关闭后不再自动显示
	public bool m_EnterFlag = false;

	public XTransPoint (ulong id) : base(id)
	{
		ObjectType = EObjectType.TransPoint;
		EntryInfo = null;
	}

	public override void Breathe()
	{
		base.Breathe ();
		this.checkTrigger ();
	}
	
	public override void Appear()
	{
		base.Appear ();
		if (!m_allTransPoint.ContainsKey (EntryInfo.Index))
			m_allTransPoint.Add (EntryInfo.Index, this);
	}

	public override void SetAppearData(object data)
	{
		XTransPointAppearInfo info = data as XTransPointAppearInfo;
		if (null == info)
			return;

		EntryInfo = XCfgLevelEntryMgr.SP.GetConfig (info.TransID);
		if (EntryInfo == null)
			return ;
		ModelId = EntryInfo.ModeID;
		Position = EntryInfo.Position;
	}

	public override float GetClickDistance()
	{
		float d = Radius () + XLogicWorld.SP.MainPlayer.Radius () + 0.5f;
		if (d > XUtil.DEFAULT_TRIGGER_DISTANCE) 
			d = XUtil.DEFAULT_TRIGGER_DISTANCE;
		return d;
	}
	
	public override void OnMouseUpAsButton(int mouseCode)
	{
		if (0 != mouseCode)
			return;
		bool bIsTrigger = XUtil.IsTrigger (Position, GetClickDistance ());
		if (!bIsTrigger) {
			XLogicWorld.SP.MainPlayer.AutoMoveTo (this);
		}
		else {
			trigger ();
		}
	}
	
	private void trigger()
	{
		//--4>: 这个逻辑稍微有点问题, 如果两个传送点距离太近会导致传送界面显示混乱, 不过理论上不该有这样的配置
		m_EnterFlag = true;
		XUTSelectSceneSE.TransPoint = this;
		XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eSelectSceneSE);
		XEventManager.SP.SendEvent (EEvent.SelectScene_AddScene, EntryInfo);
	}
	
	private void checkTrigger()
	{
		if (null == EntryInfo) {
			return;
		}
		//bool bIsTrigger = XUtil.IsTrigger(Position, GetClickDistance());
		bool bIsTrigger = XUtil.IsTrigger (Position, 3);
		if (!bIsTrigger && m_EnterFlag) {
			m_EnterFlag = false;
			//XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectScene);
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectSceneSE);
		}
		else if (bIsTrigger && !m_EnterFlag) {
				trigger ();
			}
	}

	public void OnChooseScene(int index, int hardLevel)
	{
		if (EntryInfo != null) {
			XLogicWorld.SP.SubSceneManager.CurSelLevel = (ECopySceneLevel)hardLevel;
			XLogicWorld.SP.NetManager.SendCommonArray<uint> ((int)CS_Protocol.eCS_SelectLevelEntry, EntryInfo.Index, (uint)index, (uint)hardLevel);
		}
	}

	public override void DisAppear()
	{
		base.DisAppear ();
		if (m_EnterFlag) {
			m_EnterFlag = false;
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectScene);
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectSceneSE);
		}
		if (m_allTransPoint.ContainsKey (EntryInfo.Index))
			m_allTransPoint.Remove (EntryInfo.Index);
	}
	
	public static void NotTrigger()
	{
		foreach (XTransPoint point in m_allTransPoint.Values) {
			point.m_EnterFlag = true;
		}
	}
}
