using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

public enum ESkillUseObject				// 技能作用对象
{
	eSUseObject_Single = 0,				// 单位
	eSUseObject_Region,					// 区域
}

public enum EBattleSkillType
{
	Battle_Skill_Normal,				//普攻
	Battle_SKill_Treat,					//治疗
	Battle_Skill_Special,				//绝招
	Battle_Skill_Num
}


public class XSkillLevelDefine
{
	// 文本
	public string TextLearn;			// 学习文本
	public string TextEffect;			// 效果文本
	public string TextUpgrade;			// 升级文本
	
	// 状态
	public byte Level;					// 技能等级
	public uint	LearnLevel;				// 技能学习等级
	public ESkillUseRegion useRegion;	// 作用区域
	public uint AnglerValue;
	
	public uint AttackCount;
	public float[] AttackTime = new float[3];
	public ESkeleton[] HitEffectBind = new ESkeleton[3];
	public int[] HitEffectID = new int[3];
}

public class XSkillDefine
{
	// 文本
	public string Name;					// 技能名称
	
	// 状态
	public ushort ID;					// 技能ID
	public ESkillFuncType FuncType;		// 技能功能类型
	public int LevelLimit;				// 技能等级上限
	public ESkillAttackTarget	AttackTergetType;	//攻击目标类型(1前排2后排3自身)
	
	// 条件
	public byte	Class;					// 学习职业
	
	// 攻击
	public float 	SkillTime;			// 技能时间
	public ESkillUseWay UseWay;			// 技能作用方式
	public EAnimName AttackAnim;		// 施展动画
	public float AttackAnimSpeed;		// 施展动画速率
	public float AttackEffectDelay;		// 起手特效释放时间点
	public int AttackEffectID;			// 起手特效ID
	public ESkeleton AttackEffectBind;	// 起手特效绑定点
	public uint IsFollowBone;			// 是否跟随骨骼运动
	public int AttackEffectNum;			// 起手特效数量
	public float AttackEffectLife;		// 起手特效生存周期
	public float TranslateDelay;		// 技能效果开始传递延时
	public ESkillUseObject UseObject;	// 技能作用对象
	
	public int BulletID;				// 子弹ID
	public ESkeleton BulletSrcBind;		// 子弹发射源绑定点
	public ESkeleton BulletTgtBind;		// 子弹发射目标绑定点
	public float BulletRate;			// 子弹缩放比例
	public float BulletVelocity;		// 子弹飞行速率
	public float BulletFlyTrack;		// 子弹飞行轨迹
	public int RegionEffectID;			// 区域特效ID
	public float DefeatDelay;			// 生效延时
	public int HitEffectID;				// 被击特效ID
	public ESkeleton HitEffectBind;		// 被击特效绑定点
	public int HitEffectNum;			// 被击特效数量
	public int AttackShockID;			// 攻击时震屏特效索引
	public float AttackShockDelay;		// 攻击震屏特效延时
	public int HitShockID;				// 被击时震屏特效索引
	
	public SortedList<byte, XSkillLevelDefine> Levels = new SortedList<byte, XSkillLevelDefine>();
	
	public int CanLearn(byte byteLevel)
	{
		if(null == XLogicWorld.SP.MainPlayer) 
			return -1;
		
//		if(Class > 0 && XLogicWorld.SP.MainPlayer.DynGet(EShareAttr.esa_Class) != Class)
//			return -1;
		
		if(!Levels.ContainsKey(byteLevel)) 
			return -1;
		
		XSkillLevelDefine level = Levels[byteLevel];
		if(null == level)
			return -1;
		
		if(level.LearnLevel > XLogicWorld.SP.MainPlayer.Level)
			return 1;
		
		return 0;
	}
	
	public uint GetAnger(byte level)
	{
		if(null == XLogicWorld.SP.MainPlayer) 
			return 0;
		
		if(!Levels.ContainsKey(level)) 
			return 0;
		
		XSkillLevelDefine levelDefine = Levels[level];
		if(null == levelDefine)
			return 0;
		
		return levelDefine.AnglerValue;
	}
	
	public uint GetLearnLevel(byte level)
	{
		if(null == XLogicWorld.SP.MainPlayer) 
			return 0;
		
		if(!Levels.ContainsKey(level)) 
			return 0;
		
		XSkillLevelDefine levelDefine = Levels[level];
		if(null == levelDefine)
			return 0;
		
		return levelDefine.LearnLevel;
	}
}



public class XSkillOper
{
	public ushort ID;			// 技能ID
	public byte Level;			// 技能等级
	public byte Class;			// 职业
	public ushort PreID;		// 前置技能ID
	public byte PreLevel;		// 前置技能等级
	public float PosX;
	public float PosY;
	public uint ClassLevel;		// 学习阶段
	public uint SkillPoint;		// 技能点消耗
	public byte FieldID;		// 技能栏ID
	public uint AtlasID;		// 图集ID
	public string SpriteName;	// 精灵名称
	public string NotLearnSpriteName; 
	
	public int CanLearn()
	{
		if(null == XLogicWorld.SP.MainPlayer) 
			return -1;
		
		if(SkillManager.SP.m_uSkillPoint < SkillPoint)
			return 1;
		
		if(SkillManager.SP.GetActiveSkill(PreID) < PreLevel)
			return 2;
		
		if(XLogicWorld.SP.MainPlayer.DynGet(EShareAttr.esa_Class) != Class)
			return -1;
		
		XSkillDefine SkillDef  = SkillManager.SP.GetSkillDefine((ushort)ID);
		if(null == SkillDef)
			return -1;
		XSkillLevelDefine SkillLevel = SkillDef.Levels[Level];	
		if(null == SkillLevel)
			return -1;
		if(SkillLevel.LearnLevel > XLogicWorld.SP.MainPlayer.Level)
			return -1;
		return 0;
	}
}