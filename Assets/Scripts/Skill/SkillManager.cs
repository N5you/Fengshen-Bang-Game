using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Packets;


public enum ESkill_State
{
	ESkill_State_Not_Learn,
	ESkill_State_Can_Learn,
	ESkill_State_Learn,
}

class SkillManager
{
	private TextAsset TextSkillOperConfig = null;
	private SortedList<ushort, XSkillDefine> m_Skills;		// 技能定义
	public SortedList<uint, XSkillOper> m_SkillOpers;		// 技能Oper定义
	private SortedList<ushort, byte> m_ActiveSkills;		// 已学习的技能
	
	public uint skillPoint = 0;								// 技能点数
	public uint m_uSkillPoint 
	{ 
		get
		{ 
			return skillPoint;
		}
		set
		{
			skillPoint = value;
			XDailyPlaySignMgr.SP.CheckNeedShowSkill();
		} 
	}
	public ushort m_uSkillEquip { get; private set; }
	
	public static SkillManager SP { get; private set; }	
	
	public SkillManager()
	{
		SP = this;
		m_Skills = new SortedList<ushort, XSkillDefine>();
		m_SkillOpers = new SortedList<uint, XSkillOper>();
		m_ActiveSkills = new SortedList<ushort, byte>();
		m_uSkillPoint = m_uSkillEquip = 0;
		
		XEventManager.SP.AddHandler(OnDBFileLoad, EEvent.DBFileLoadReady);
		
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		
		
	}
	
	public bool Init()
	{		
//		TextAsset text = StaticResourceManager.SP.TextSkillBaseConfig;
//		if(null == text)
//		{
//			Log.Write(LogLevel.ERROR, "[ERROR] SkillManager, 没有SkillBase配置");
//			return false;
//		}
//		
//		TabFile tabFile = new TabFile(text.name, text.text);
//		while(tabFile.Next())
//		{
//			ushort id = (ushort)tabFile.GetInt32("SkillID");
//			if(m_Skills.ContainsKey(id))
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillBase表ID配重复了, 后者会被忽略, id为{0}", id);
//				continue;
//			}
//			XSkillDefine skillDef = new XSkillDefine();
//			skillDef.ID = id;
//			skillDef.Name = tabFile.GetString("Name");
//			skillDef.FuncType = (ESkillFuncType)tabFile.GetInt32("FuncType");
//			skillDef.AttackTergetType	= (ESkillAttackTarget)tabFile.GetInt32("AttackTarget");
//			skillDef.LevelLimit = tabFile.GetInt32("LevelLimit");
//			skillDef.Class = Convert.ToByte(tabFile.GetInt32("Class"));
//			skillDef.SkillTime	= tabFile.GetFloat("SkillTime");
//			skillDef.UseWay = (ESkillUseWay)tabFile.GetInt32("UseWay");
//			skillDef.AttackAnim = (EAnimName)tabFile.GetInt32("AttackAnim");
//			skillDef.AttackAnimSpeed = tabFile.GetFloat("AttackAnimSpeed");
//			skillDef.AttackEffectDelay = tabFile.GetFloat("AttackEffectDelay");
//			skillDef.AttackEffectID = tabFile.GetInt32("AttackEffectID");
//			skillDef.AttackEffectBind = (ESkeleton)tabFile.GetInt32("AttackEffectBind");
//			skillDef.IsFollowBone		= tabFile.GetUInt32("IsFollowBone");
//			skillDef.AttackEffectNum = tabFile.GetInt32("AttackEffectNum");
//			skillDef.AttackEffectLife = tabFile.GetFloat("AttackEffectLife");
//			skillDef.TranslateDelay = tabFile.GetFloat("TranslateDelay");
//			skillDef.UseObject = (ESkillUseObject)tabFile.GetInt32("UseObject");
//			skillDef.BulletID = tabFile.GetInt32("BulletID");
//			
//			//策划需求增加一个子弹ID的检测
//            if (ESkillUseWay.eSUseWay_LongBullet == skillDef.UseWay && 0 == skillDef.BulletID)
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, {0} 为远程子弹的技能的子弹ID配置成了0", skillDef.ID);
//			}
//			skillDef.BulletSrcBind = (ESkeleton)tabFile.GetInt32("BulletSrcBind");
//			skillDef.BulletTgtBind = (ESkeleton)tabFile.GetInt32("BulletTgtBind");
//			skillDef.BulletRate = tabFile.GetFloat("BulletRate");
//			skillDef.BulletVelocity = tabFile.GetFloat("BulletVelocity");
//			skillDef.BulletFlyTrack = tabFile.GetFloat("BulletFlyTrack");
//			skillDef.RegionEffectID = tabFile.GetInt32("RegionEffectID");
//			skillDef.DefeatDelay = tabFile.GetFloat("DefeatDelay");
//			skillDef.HitEffectID = tabFile.GetInt32("HitEffectID");
//			skillDef.HitEffectBind = (ESkeleton)tabFile.GetInt32("HitEffectBind");
//			skillDef.HitEffectNum = tabFile.GetInt32("HitEffectNum");
//			skillDef.AttackShockID = tabFile.GetInt32("AttackShockID");
//			skillDef.AttackShockDelay = tabFile.GetFloat("AttackShockDelay");
//			skillDef.HitShockID = tabFile.GetInt32("HitShockID");
//			m_Skills.Add(skillDef.ID, skillDef);
//		}
//		
//		text = StaticResourceManager.SP.TextSkillLevelConfig;
//		if(null == text)
//		{
//			Log.Write(LogLevel.ERROR, "[ERROR] SkillManager, 没有SkillLevel配置");
//			return false;
//		}
//		
//		tabFile = new TabFile(text.name, text.text);
//		while(tabFile.Next())
//		{
//			ushort id = (ushort)tabFile.GetUInt32("SkillID");
//			if(!m_Skills.ContainsKey(id))
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillLevel中配置了一个不存在的SkillID: {0}", id);
//				continue;
//			}
//			XSkillDefine skillDef = m_Skills[id];
//			
//			byte byteLevel = (byte)tabFile.GetInt32("SkillLevel");
//			if(byteLevel > skillDef.LevelLimit) continue;
//			if(byteLevel != skillDef.Levels.Count + 1)
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, id: {0}, level 不是从1开始递增的", id);
//				continue;
//			}
//			
//			XSkillLevelDefine pSLD = new XSkillLevelDefine();
//			pSLD.Level = byteLevel;
//			pSLD.LearnLevel = tabFile.GetUInt32("LearnLevel");
//			pSLD.TextLearn = tabFile.GetString("TextLearn");
//			pSLD.TextEffect = tabFile.GetString("TextEffect");
//			pSLD.TextUpgrade = tabFile.GetString("TextUpgrade");
//			pSLD.AnglerValue	= tabFile.GetUInt32("Kill");
//			pSLD.useRegion	= (ESkillUseRegion)tabFile.GetUInt32("UseRegion");
//			
//			pSLD.AttackCount		= tabFile.GetUInt32("AttackCount");
//			pSLD.AttackTime[0]		= tabFile.GetFloat("AttackTime_3_0");
//			pSLD.HitEffectBind[0]	= (ESkeleton)tabFile.GetInt32("AttackEffectBind_3_0");
//			pSLD.HitEffectID[0]		= tabFile.GetInt32("AttackEffect_3_0");
//			
//			pSLD.AttackTime[1]		= tabFile.GetFloat("AttackTime_3_1");
//			pSLD.HitEffectBind[1]	= (ESkeleton)tabFile.GetInt32("AttackEffectBind_3_1");
//			pSLD.HitEffectID[1]		= tabFile.GetInt32("AttackEffect_3_1");
//			
//			pSLD.AttackTime[2]		= tabFile.GetFloat("AttackTime_3_2");
//			pSLD.HitEffectBind[2]	= (ESkeleton)tabFile.GetInt32("AttackEffectBind_3_2");
//			pSLD.HitEffectID[2]		= tabFile.GetInt32("AttackEffect_3_2");		
//			
//			skillDef.Levels.Add(pSLD.Level, pSLD);
//		}		
//		
//		TextSkillOperConfig = StaticResourceManager.SP.TextSkillOperConfig;
		
		SortedList<ushort, XCfgSkillBase> skillBaseList = XCfgSkillBaseMgr.SP.ItemTable;
		foreach(KeyValuePair<ushort,XCfgSkillBase> skillBase in skillBaseList)
		{
			XCfgSkillBase cfgSkillBase = skillBase.Value;
			ushort id = cfgSkillBase.SkillID;
			if(m_Skills.ContainsKey(id))
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillBase表ID配重复了, 后者会被忽略, id为{0}", id);
				continue;
			}
			XSkillDefine skillDef = new XSkillDefine();
			skillDef.ID = id;
			skillDef.Name = cfgSkillBase.Name;
			skillDef.FuncType = (ESkillFuncType)cfgSkillBase.FuncType;
			skillDef.AttackTergetType	= (ESkillAttackTarget)cfgSkillBase.AttackTarget;
			skillDef.LevelLimit = cfgSkillBase.LevelLimit;
			skillDef.Class = Convert.ToByte(cfgSkillBase.Class);
			skillDef.SkillTime	= cfgSkillBase.SkillTime;
			skillDef.UseWay = (ESkillUseWay)cfgSkillBase.UseWay;
			skillDef.AttackAnim = (EAnimName)cfgSkillBase.AttackAnim;
			skillDef.AttackAnimSpeed = cfgSkillBase.AttackAnimSpeed;
			skillDef.AttackEffectDelay = cfgSkillBase.AttackEffectDelay;
			skillDef.AttackEffectID = cfgSkillBase.AttackEffectID;
			skillDef.AttackEffectBind = (ESkeleton)cfgSkillBase.AttackEffectBind;
			skillDef.IsFollowBone		= (uint)cfgSkillBase.IsFollowBone;
			skillDef.AttackEffectNum = cfgSkillBase.AttackEffectNum;
			skillDef.AttackEffectLife = cfgSkillBase.AttackEffectLife;
			skillDef.TranslateDelay = cfgSkillBase.TranslateDelay;
			skillDef.UseObject = (ESkillUseObject)cfgSkillBase.UseObject;
			skillDef.BulletID = cfgSkillBase.BulletID;
			
			//策划需求增加一个子弹ID的检测
            if (ESkillUseWay.eSUseWay_LongBullet == skillDef.UseWay && 0 == skillDef.BulletID)
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, {0} 为远程子弹的技能的子弹ID配置成了0", skillDef.ID);
			}
			skillDef.BulletSrcBind = (ESkeleton)cfgSkillBase.BulletSrcBind;
			skillDef.BulletTgtBind = (ESkeleton)cfgSkillBase.BulletTgtBind;
			skillDef.BulletRate = cfgSkillBase.BulletRate;
			skillDef.BulletVelocity = cfgSkillBase.BulletVelocity;
			skillDef.BulletFlyTrack = cfgSkillBase.BulletFlyTrack;
			skillDef.RegionEffectID = cfgSkillBase.RegionEffectID;
			skillDef.DefeatDelay = cfgSkillBase.DefeatDelay;
			skillDef.HitEffectID = cfgSkillBase.HitEffectID;
			skillDef.HitEffectBind = (ESkeleton)cfgSkillBase.HitEffectBind;
			skillDef.HitEffectNum = cfgSkillBase.HitEffectNum;
			skillDef.AttackShockID = cfgSkillBase.AttackShockID;
			skillDef.AttackShockDelay = cfgSkillBase.AttackShockDelay;
			skillDef.HitShockID = cfgSkillBase.HitShockID;
			m_Skills.Add(skillDef.ID, skillDef);
		}
		
		List<XCfgSkillLevel> levelList = XCfgSkillLevelMgr.SP.ItemTable;
		foreach(XCfgSkillLevel cfgLevel in levelList)
		{
			ushort id = cfgLevel.SkillID;
			if(!m_Skills.ContainsKey(id))
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillLevel中配置了一个不存在的SkillID: {0}", id);
				continue;
			}
			XSkillDefine skillDef = m_Skills[id];
			
			byte byteLevel = (byte)cfgLevel.SkillLevel;
			if(byteLevel > skillDef.LevelLimit) continue;
			if(byteLevel != skillDef.Levels.Count + 1)
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, id: {0}, level 不是从1开始递增的", id);
				continue;
			}
			
			XSkillLevelDefine pSLD = new XSkillLevelDefine();
			pSLD.Level = byteLevel;
			pSLD.LearnLevel 		= (uint)cfgLevel.LearnLevel;
			pSLD.TextLearn 			= cfgLevel.TextLearn;
			pSLD.TextEffect 		= cfgLevel.TextEffect;
			pSLD.TextUpgrade 		= cfgLevel.TextUpgrade;
			pSLD.AnglerValue		= (uint)cfgLevel.Kill;
			pSLD.useRegion			= (ESkillUseRegion)cfgLevel.UseRegion;
			
			pSLD.AttackCount		= (uint)cfgLevel.AttackCount;
			pSLD.AttackTime[0]		= cfgLevel.AttackTime[0];
			pSLD.HitEffectBind[0]	= (ESkeleton)cfgLevel.AttackEffectBind[0];
			pSLD.HitEffectID[0]		= cfgLevel.AttackEffect[0];
			
			pSLD.AttackTime[1]		= cfgLevel.AttackTime[1];
			pSLD.HitEffectBind[1]	= (ESkeleton)cfgLevel.AttackEffectBind[1];
			pSLD.HitEffectID[1]		= cfgLevel.AttackEffect[1];
			
			pSLD.AttackTime[2]		= cfgLevel.AttackTime[2];
			pSLD.HitEffectBind[2]	= (ESkeleton)cfgLevel.AttackEffectBind[2];
			pSLD.HitEffectID[2]		= cfgLevel.AttackEffect[2];		
			
			skillDef.Levels.Add(pSLD.Level, pSLD);
		}
		return true;
	}
	
	public ESkill_State GetSkillState(ushort skillID)
	{
		if(!m_Skills.ContainsKey(skillID))
			return ESkill_State.ESkill_State_Not_Learn;
		
		if(!m_ActiveSkills.ContainsKey(skillID))
		{
			XSkillDefine define = GetSkillDefine(skillID);
			if(define == null)
				return ESkill_State.ESkill_State_Not_Learn;
			
			XSkillOper oper = GetSkillOper(skillID,1);
			if(oper == null)
				return ESkill_State.ESkill_State_Not_Learn;
			
			if(oper.CanLearn() == 0)
				return ESkill_State.ESkill_State_Can_Learn;
			else
				return ESkill_State.ESkill_State_Not_Learn;
		}
		
		return ESkill_State.ESkill_State_Learn;	
	}
	
	public void OnDBFileLoad(EEvent evt, params object[] args)
	{
		Init();
	}
	
	public void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
//		TextAsset text = TextSkillOperConfig;
//		if(null == text)
//		{
//			Log.Write(LogLevel.ERROR, "[ERROR] SkillManager, 没有找到SkillOper配置");
//			return;
//		}
//		TabFile tabFile = new TabFile(text.name, text.text);
//		while(tabFile.Next())
//		{
//			byte byteClass = tabFile.Get<byte>("Class");
//			if(byteClass != XLogicWorld.SP.MainPlayer.DynGet(EShareAttr.esa_Class))	// 只需加载与主角职业相同的配置
//				continue;
//			
//			ushort id = tabFile.Get<ushort>("SkillID");
//			if(!m_Skills.ContainsKey(id))
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个找不到的技能ID: {0}", id);
//				continue;
//			}
//			byte level = tabFile.Get<byte>("SkillLevel");
//			XSkillDefine skillDef = m_Skills[id];
//			if(!skillDef.Levels.ContainsKey(level))
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个找不到的技能等级: {0}, {1}", id, level);
//				continue;
//			}
//			
//			ushort wPreID = tabFile.Get<ushort>("PreID");
//			byte wPreLevel = tabFile.Get<byte>("PreLevel");
//			if(0 != wPreID)
//			{
//				if(!m_Skills.ContainsKey(wPreID))
//				{
//					Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个不存在的前置技能: {0}", wPreID);
//					continue;
//				}
//				if(!m_Skills[wPreID].Levels.ContainsKey(wPreLevel))
//				{
//					Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个不存在的前置技能: {0}, {1}", wPreID, wPreLevel);
//					continue;
//				}
//			}
//			
//			uint key = id;
//			key <<= 16;
//			key |= level;
//			if(m_SkillOpers.ContainsKey(key))
//			{
//				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了重复数据: {0}, {1}", id, level);
//				continue;
//			}
//			
//			string notLearnSpriteName = tabFile.Get<string>("NotActiveSprite");
//
//			XSkillOper skillOper = new XSkillOper();
//			skillOper.ID = id;
//			skillOper.Level = level;
//			skillOper.PreID = wPreID;
//			skillOper.PreLevel = wPreLevel;
//			skillOper.Class = byteClass;
//			skillOper.PosX = tabFile.Get<float>("PosX");
//			skillOper.PosY = tabFile.Get<float>("PosY");
//			skillOper.ClassLevel = tabFile.Get<uint>("ClassLevel");
//			skillOper.SkillPoint = tabFile.Get<uint>("SkillPoint");
//			skillOper.FieldID = tabFile.Get<byte>("FieldID");
//			string[] str = tabFile.GetString("StarSprite").Split(new char[]{'.'});
//			skillOper.AtlasID = Convert.ToUInt32(str[0]);
//			skillOper.SpriteName = str[1];
//			skillOper.NotLearnSpriteName	= notLearnSpriteName;
//			
//			m_SkillOpers.Add(key, skillOper);
//			XEventManager.SP.SendEvent(EEvent.Skill_AddSkillOperConfig, skillOper,GetSkillState(id));
//		}
		
		List<XCfgSkillOper> opList = XCfgSkillOperMgr.SP.ItemTable;
		foreach(XCfgSkillOper temp in opList)
		{
			byte byteClass = temp.Class;
			if(byteClass != XLogicWorld.SP.MainPlayer.DynGet(EShareAttr.esa_Class))	// 只需加载与主角职业相同的配置
				continue;
			
			ushort id = temp.SkillID;
			if(!m_Skills.ContainsKey(id))
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个找不到的技能ID: {0}", id);
				continue;
			}
			byte level = temp.SkillLevel;
			XSkillDefine skillDef = m_Skills[id];
			if(!skillDef.Levels.ContainsKey(level))
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个找不到的技能等级: {0}, {1}", id, level);
				continue;
			}
			
			ushort wPreID = temp.PreID;
			byte wPreLevel = temp.PreLevel;
			if(0 != wPreID)
			{
				if(!m_Skills.ContainsKey(wPreID))
				{
					Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个不存在的前置技能: {0}", wPreID);
					continue;
				}
				if(!m_Skills[wPreID].Levels.ContainsKey(wPreLevel))
				{
					Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了一个不存在的前置技能: {0}, {1}", wPreID, wPreLevel);
					continue;
				}
			}
			
			uint key = id;
			key <<= 16;
			key |= level;
			if(m_SkillOpers.ContainsKey(key))
			{
				Log.Write(LogLevel.WARN, "[WARN] SkillManager, SkillOper中配置了重复数据: {0}, {1}", id, level);
				continue;
			}
			
			string notLearnSpriteName = temp.NotActiveSprite;

			XSkillOper skillOper = new XSkillOper();
			skillOper.ID = id;
			skillOper.Level = level;
			skillOper.PreID = wPreID;
			skillOper.PreLevel = wPreLevel;
			skillOper.Class = byteClass;
			skillOper.PosX = temp.PosX;
			skillOper.PosY = temp.PosY;
			skillOper.ClassLevel = temp.ClassLevel;
			skillOper.SkillPoint = temp.SkillPoint;
			skillOper.FieldID = temp.FieldID;
			string[] str = temp.StarSprite.Split(new char[]{'.'});
			skillOper.AtlasID = Convert.ToUInt32(str[0]);
			skillOper.SpriteName = str[1];
			skillOper.NotLearnSpriteName	= notLearnSpriteName;
			
			XCfgSkillBase baseConfig = XCfgSkillBaseMgr.SP.GetConfig(id);
			if ( null != baseConfig && baseConfig.FuncType == 4 )
			{
				m_ActiveSkills.Add(id, (byte)baseConfig.LevelLimit);
			}
			
			m_SkillOpers.Add(key, skillOper);
			XEventManager.SP.SendEvent(EEvent.Skill_AddSkillOperConfig, skillOper,GetSkillState(id));
		}
		
		XDailyPlaySignMgr.SP.CheckNeedShowSkill();
	}
	
	public XSkillDefine GetSkillDefine(ushort id)
	{
		if(m_Skills.ContainsKey(id))
			return m_Skills[id];
		return null;
	}
	
	public XSkillOper GetSkillOper(ushort id, byte level)
	{
		uint key = id; key <<= 16; key |= level;
		if(m_SkillOpers.ContainsKey(key))
			return m_SkillOpers[key];
		return null;
	}
	
	public byte GetActiveSkill(ushort id)
	{
		if(!m_ActiveSkills.ContainsKey(id)) return 0;
		return m_ActiveSkills[id];
	}
	
	public void ImproveSkill(ushort skillID)
	{
		byte level = GetActiveSkill(skillID);
		if(0 == level) LearnSkill(skillID, 1);
		else UpgradeSkill(skillID, 1);
	}
	
	public void LearnSkill(ushort skillID, byte level)
	{
		if(m_ActiveSkills.ContainsKey(skillID))
			return;

		if(!m_Skills.ContainsKey(skillID))
			return;

		XSkillDefine skillDef = m_Skills[skillID];
		uint decSkillPoint = 0;
		for(byte i=1; i<=level; i++)
		{
			switch(skillDef.CanLearn(i))
			{
			case -1:
				return;
			case 1:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(11);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,con);
				return;
			}
			}
			
			uint key = skillID; key <<= 16; key |= i;
			if(!m_SkillOpers.ContainsKey(key))
				return;
			XSkillOper skillOper = m_SkillOpers[key];
			switch(skillOper.CanLearn())
			{
			case -1:
				return;
			case 1:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(12);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, con);
			}
				return;
			case 2:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(13);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, con);
				return;
			}
			}
			decSkillPoint += skillOper.SkillPoint;
		}
		
		if(m_uSkillPoint < decSkillPoint)
		{
			string con = "[color=ff1515]" + XStringManager.SP.GetString(12);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,con);
			return;
		}
		
		uint unit = Convert.ToUInt32(skillID);
		unit <<= 16;
		unit |= level;
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(unit);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_LearnSkill, builder.Build());
	}
	
	public void UpgradeSkill(ushort skillID, byte level)
	{
		if(!m_ActiveSkills.ContainsKey(skillID))
			return;
		
		if(!m_Skills.ContainsKey(skillID))
			return;

		XSkillDefine skillDef = m_Skills[skillID];
		uint decSkillPoint = 0;
		//已经满级	
		byte nowLevel = GetActiveSkill(skillID);
		if((int)nowLevel >= skillDef.LevelLimit)
			return;
	
		for(byte i=1; i<=level; i++)
		{
			switch(skillDef.CanLearn((byte)(m_ActiveSkills[skillID] + i)))
			{
			case -1:
				return;
			case 1:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(11);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,con);
				return;
			}
			}
			
			uint key = skillID; key <<= 16; key |= (byte)(m_ActiveSkills[skillID] + i);
			if(!m_SkillOpers.ContainsKey(key))
				return;

			XSkillOper skillOper = m_SkillOpers[key];
			switch(skillOper.CanLearn())
			{
			case -1:
				return;
			case 1:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(12);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, con);
				return;
			}
			case 2:
			{
				string con = "[color=ff1515]" + XStringManager.SP.GetString(13);
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,con);
				return;
			}
			}
			decSkillPoint += skillOper.SkillPoint;
		}
		
		if(m_uSkillPoint < decSkillPoint)
		{
			string con = "[color=ff1515]" + XStringManager.SP.GetString(12);
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up,con);
			return;
		}

		uint unit = Convert.ToUInt32(skillID);
		unit <<= 16;
		unit |= level;
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(unit);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_UpgradeSkill, builder.Build());
	}
	
	public void ForgetSkill(ushort skillID)
	{
		if(!m_ActiveSkills.ContainsKey(skillID))
			return;
		
		uint unit = Convert.ToUInt32(skillID);
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(unit);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ForgetSkill, builder.Build());
	}
	
	public void EquipSkill(ushort skillID)
	{
		if(m_uSkillEquip == skillID || !m_ActiveSkills.ContainsKey(skillID))
			return;
		
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(skillID);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_EquipSkill, builder.Build());
	}
	
	public void On_SC_ActiveSkill(SC_UIntArr msg)
	{
		if(msg.HasUid)		// 提取装备技能
		{
			m_uSkillEquip = (ushort)msg.Uid;
			XEventManager.SP.SendEvent(EEvent.Skill_OnEquipSkill, m_uSkillEquip);
		}
		
		m_uSkillPoint = msg.GetData(0); 	// 提取技能点
		XEventManager.SP.SendEvent(EEvent.Skill_SkillPoint, m_uSkillPoint);
		if(msg.DataCount > 1)
		{
			for(int i=1; i<msg.DataCount; i++)		// 提取已学习技能id&level
			{
				uint unit = msg.GetData(i);
				ushort wSkillID = (ushort)(unit >> 16);
				byte byteLevel = (byte)unit;
				if(m_ActiveSkills.ContainsKey(wSkillID))
				{
					Log.Write(LogLevel.WARN, "初始技能信息时发过来了重复技能:{0}", wSkillID);
					continue;
				}
				m_ActiveSkills.Add(wSkillID, byteLevel);
				XEventManager.SP.SendEvent(EEvent.Skill_OnLearnSkill, wSkillID, byteLevel);
			}
		}
	}
	
	public void On_SC_LearnSkill(SC_UInt msg)
	{
		uint unit = msg.Data;
		ushort wSkillID = (ushort)(unit >> 16);
		byte byteLevel = (byte)unit;
		Log.Write("学习技能: {0} - {1}", wSkillID, byteLevel);
		if(m_ActiveSkills.ContainsKey(wSkillID))
		{
			Log.Write(LogLevel.WARN, "客户端还能比服务器先学习技能? : {0}", wSkillID);
			return;
		}
		
		//减技能点
		XSkillOper oper = GetSkillOper(wSkillID,byteLevel);
		if(oper == null)
			return ;
		m_uSkillPoint	-= oper.SkillPoint;
		XEventManager.SP.SendEvent(EEvent.Skill_SkillPoint,m_uSkillPoint);
		
		m_ActiveSkills.Add(wSkillID, byteLevel);
		XEventManager.SP.SendEvent(EEvent.Skill_OnLearnSkill, wSkillID, byteLevel);
		
		XSkillDefine skillDef = m_Skills[wSkillID];
		string con = "[color=00ff00]" + XStringManager.SP.GetString(9);
		XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format(con, skillDef.Name, byteLevel));
	}
	
	public void On_SC_UpgradeSkill(SC_UInt msg)
	{
		uint unit = msg.Data;
		ushort wSkillID = (ushort)(unit >> 16);
		byte byteLevel = (byte)unit;
		if(!m_ActiveSkills.ContainsKey(wSkillID))
		{
			Log.Write(LogLevel.WARN, "服务器命令客户端升级一个它没有的技能 : {0}", wSkillID);
			return;
		}
		m_ActiveSkills[wSkillID] += byteLevel;
		XEventManager.SP.SendEvent(EEvent.Skill_OnUpgradeSkill, wSkillID, byteLevel);
		
		//减技能点
		XSkillOper oper = GetSkillOper(wSkillID,m_ActiveSkills[wSkillID]);
		if(oper == null)
			return ;
		m_uSkillPoint	-= oper.SkillPoint;
		XEventManager.SP.SendEvent(EEvent.Skill_SkillPoint,m_uSkillPoint);
		
		XSkillDefine skillDef = m_Skills[wSkillID];
		string con = "[color=00ff00]" + XStringManager.SP.GetString(10);
		XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format(con, skillDef.Name, m_ActiveSkills[wSkillID]));
	}
	
	public void On_SC_ForgetSkill(SC_UInt msg)
	{
		ushort wSkillID = (ushort)msg.Data;
		if(!m_ActiveSkills.ContainsKey(wSkillID))
		{
			Log.Write(LogLevel.WARN, "服务器命令客户端删除一个它没有的技能 : {0}", wSkillID);
			return;
		}
		
		XEventManager.SP.SendEvent(EEvent.Skill_OnForgetSkill, wSkillID);
		
		// 如果忘却的是已经装备的技能
		if(wSkillID == m_uSkillEquip)
			m_uSkillEquip = 0;
		
		XEventManager.SP.SendEvent(EEvent.Skill_OnEquipSkill, m_uSkillEquip);
		m_ActiveSkills.Remove(wSkillID);
	}
	
	public void On_SC_SkillPoint(SC_UInt msg)
	{
		uint oldSkillPoint = m_uSkillPoint;
		m_uSkillPoint = msg.Data;
		XEventManager.SP.SendEvent(EEvent.Skill_SkillPoint, m_uSkillPoint);
		uint deltaValue = m_uSkillPoint - oldSkillPoint;
		if(deltaValue > 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,532,deltaValue);
		}
		
	}
	
	public void On_SC_EquipedSkill(SC_UInt msg)
	{
		m_uSkillEquip = (ushort)msg.Data;
		XEventManager.SP.SendEvent(EEvent.Skill_OnEquipSkill, m_uSkillEquip);
		
		
	}
}