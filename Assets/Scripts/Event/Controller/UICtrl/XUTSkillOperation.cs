using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

class XUTSkillOperation : XUICtrlTemplate<XSkillOperation>
{
	private bool m_bMainPlayerEntered = false;
	private List<XSkillOper> m_SkillOpers = new List<XSkillOper>();
	private List<ESkill_State>	m_SillState = new List<ESkill_State>();
	private List<object[]> m_SkillEvents = new List<object[]>();
	
	private static string[] DutyToBKSprite = {"","11001002","11001003","11001001"};
	private static int[] DutyToAtlasID = {0,1003,1001,1002};
	
	public XUTSkillOperation()
	{
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		XEventManager.SP.AddHandler(OnAddSkillOperConfig, EEvent.Skill_AddSkillOperConfig);
		XEventManager.SP.AddHandler(OnSkillPoint, EEvent.Skill_SkillPoint);
		XEventManager.SP.AddHandler(OnLearnSkill, EEvent.Skill_OnLearnSkill);
		XEventManager.SP.AddHandler(OnUpgradeSkill, EEvent.Skill_OnUpgradeSkill);
		XEventManager.SP.AddHandler(OnEquipSkill, EEvent.Skill_OnEquipSkill);
		XEventManager.SP.AddHandler(OnForgetSkill, EEvent.Skill_OnForgetSkill);
		XEventManager.SP.AddHandler(ImproveSkill, EEvent.Skill_ImproveSkill);
		XEventManager.SP.AddHandler(EquipSkill, EEvent.Skill_EquipSkill);
	}
	
	public override bool Show()
	{
		if(!m_bMainPlayerEntered)
			return true;
		
		return base.Show();	
	}
	
	public override void OnShow()
	{
		XMainPlayer playSelf = XLogicWorld.SP.MainPlayer;
		int playerClass = playSelf.DynGet(EShareAttr.esa_Class);
		if(playSelf != null && LogicUI != null)
			LogicUI.SetBKSprite(DutyToAtlasID[playerClass],DutyToBKSprite[playerClass]);
	}	
	
	public override void OnCreated(object arg)
	{
		base.OnCreated(arg);
		for(int i=0; i<m_SkillOpers.Count; i++)
		{
			LogicUI.OnAddSkillOperConfig(m_SkillOpers[i],m_SillState[i]);
		}
		LogicUI.OnSkillPoint(SkillManager.SP.m_uSkillPoint);
		m_SkillOpers.Clear();
		m_SillState.Clear();
		
		for(int i=0; i<m_SkillEvents.Count; i++)
		{
			EEvent evt = (EEvent)(m_SkillEvents[i][0]);
			object[] args = (object[])(m_SkillEvents[i][1]);
			switch(evt)
			{
			case EEvent.Skill_OnLearnSkill:
				LogicUI.OnInitSkill((ushort)(args[0]), (byte)(args[1]));
				break;
			case EEvent.Skill_OnUpgradeSkill:
				LogicUI.OnUpgradeSkill((ushort)(args[0]), (byte)(args[1]));
				break;
			case EEvent.Skill_OnForgetSkill:
				LogicUI.OnForgetSkill((ushort)(args[0]));
				break;
			case EEvent.Skill_OnEquipSkill:
				LogicUI.OnEquipSkill((ushort)(args[0]));
				break;
			}
		}
		m_SkillEvents.Clear();
		
		LogicUI.DoNewPlayerGuide(XNewPlayerGuideManager.GuideType.Guide_Skill_Select);
	}
	
	private void PushEvent(EEvent evt, object[] args)
	{
		object[] arr = new object[2];
		arr[0] = evt;
		arr[1] = args;
		m_SkillEvents.Add(arr);
	}
	
	private void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
		m_bMainPlayerEntered = true;
	}
	
	private void OnAddSkillOperConfig(EEvent evt, params object[] args)
	{
		XSkillOper skillOper = (XSkillOper)(args[0]);
		ESkill_State state = (ESkill_State)(args[1]);
		if(null == LogicUI)
		{
			m_SkillOpers.Add(skillOper);
			m_SillState.Add(state);
			return;
		}
		LogicUI.OnAddSkillOperConfig(skillOper,state);
	}
	
	private void OnSkillPoint(EEvent evt, params object[] args)
	{
		if(null == LogicUI) return;
		LogicUI.OnSkillPoint((uint)(args[0]));
	}
	
	private void OnLearnSkill(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			PushEvent(evt, args);
			return;
		}
		LogicUI.OnLearnSkill((ushort)(args[0]), (byte)(args[1]));
		LogicUI.ReflashUI();
	}
	
	private void OnUpgradeSkill(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			PushEvent(evt, args);
			return;
		}
		LogicUI.OnUpgradeSkill((ushort)(args[0]), (byte)(args[1]));
		
	}
	
	private void OnForgetSkill(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			PushEvent(evt, args);
			return;
		}
		LogicUI.OnForgetSkill((ushort)(args[0]));
	}
	
	private void OnEquipSkill(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			PushEvent(evt, args);
			return;
		}
		LogicUI.OnEquipSkill((ushort)(args[0]));
	}
	
	private void ImproveSkill(EEvent evt, params object[] args)
	{
		SkillManager.SP.ImproveSkill((ushort)(args[0]));
	}
	
	private void EquipSkill(EEvent evt, params object[] args)
	{
		SkillManager.SP.EquipSkill((ushort)(args[0]));
	}
}

