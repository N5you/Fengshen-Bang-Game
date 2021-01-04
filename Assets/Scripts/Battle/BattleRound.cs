using System.Collections.Generic;
using XGame.Client.Packets;
using UnityEngine;

#region SkillEffectData
public class SkillEffectData_Base
{	
	public XSkill	CurSkill {get; set; }
	public virtual void DoEffect(bool isNeedAnim) {}	
}

public class SkillEffectData_ComDamage : SkillEffectData_Base
{	
	private struct TargetDamage
	{
		public XBattlePosition mPos;
		public uint mFlag;
		public int mDamageValue; 
	}
	
	public void SetMainTargetDamage(uint pos,uint flag,int damageValue)
	{
		mMainTargetPos			= XBattlePosition.Create(pos);
		
		TargetDamage	damage = new TargetDamage();
		damage.mPos				= mMainTargetPos;
		damage.mFlag			= flag;
		damage.mDamageValue		= damageValue;
		
		mAllTargetList.Add(damage);
	}
	
	public void AddOtherTargetDamage(uint pos,uint flag,int damageValue)
	{
		TargetDamage	damage = new TargetDamage();
		damage.mPos			= XBattlePosition.Create(pos);
		damage.mFlag		= flag;
		damage.mDamageValue	= damageValue;
		
		mAllTargetList.Add(damage);
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		
		for(int i = 0; i < mAllTargetList.Count; i++)
		{
			TargetDamage damage = mAllTargetList[i];
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(damage.mPos);
			if(obj == null)
				continue;
			int tempLastHP = obj.Hp;
			//int test = obj.Hp + damage.mDamageValue;
			//Log.Write(LogLevel.INFO,"curHP {0}  subHP {1}  resultHP {2} ",obj.Hp,damage.mDamageValue,test);			
			obj.Hp	+= damage.mDamageValue;
			string flyString = "";
			EAnimName animName = EAnimName.Fight;
			switch((ESkillAttackResult)damage.mFlag)
			{
			case ESkillAttackResult.eSAttackResult_Miss:		// 未击中
				flyString += XStringManager.SP.GetString(14);
				break;
				
			case ESkillAttackResult.eSAttackResult_Defence:		// 抵挡
				flyString += XStringManager.SP.GetString(17);
				animName = EAnimName.Deffence;
				break;
				
			case ESkillAttackResult.eSAttackResult_Dodge:		// 闪避
				flyString += XStringManager.SP.GetString(15);
				animName = EAnimName.Dodge;
				break;

			case ESkillAttackResult.eSAttackResult_Critical:	// 暴击
				flyString += XStringManager.SP.GetString(16);
				animName = EAnimName.Wound;
				break;				
			case ESkillAttackResult.eSAttackResult_Normal:		// 普通攻击
				animName = EAnimName.Wound;
				break;
			}

			//if(isNeedAnim && obj.Hp > 0 && animName != EAnimName.Fight && damage.mDamageValue < 0)
			if(isNeedAnim && obj.Hp > 0 && animName != EAnimName.Fight )
			{
				if(animName == EAnimName.Wound)
				{
					if(damage.mDamageValue < 0)
						obj._playAnimation(animName,1.0f,false);
				}
				else
				{
					obj._playAnimation(animName,1.0f,false);
				}
				obj._playAnimation(EAnimName.Fight,1.0f,true);
			}
			
			if(damage.mDamageValue != 0)
			{				
				if(damage.mDamageValue < 0)
				{
					flyString += "[color=FF3C1E]";
					flyString += damage.mDamageValue;
				}					
				else
				{
					flyString += "[color=00ff00]";
					flyString += "+";
					flyString += damage.mDamageValue;
				}
				
				int finalDamage = damage.mDamageValue;
				if(obj.Hp <= 0)
				{
					finalDamage = -tempLastHP;
					if(isNeedAnim)
						obj._playAnimation(EAnimName.Death,1.0f,false);
				}
				
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,damage.mPos.Group,finalDamage);
			}
			
			if(isNeedAnim)
			{
//				if(damage.mFlag == (uint)ESkillAttackResult.eSAttackResult_Miss)
//				{
//					CurSkill.Attacker.FlyString((EFlyStrType)damage.mFlag, flyString);
//				}
//				else
				obj.FlyString((EFlyStrType)damage.mFlag, flyString);
			}		
		}
	}
	
	private XBattlePosition		mMainTargetPos;
	private List<TargetDamage>	mAllTargetList	= new List<TargetDamage>();
}

public class SkillEffectData_AddState : SkillEffectData_Base
{	
	struct AddStateData
	{
		public AddStateData(uint StateID,uint StateLevel,uint TargetPos)
		{
			this.StateID = StateID;
			this.StateLevel	= StateLevel;
			this.TargetPos	= TargetPos;
		}
		public uint StateID;
		public uint StateLevel;
		public uint TargetPos;
	}
	
	List<AddStateData>	mAddStateList = new List<AddStateData>();
	
	public void AddState(uint stateID,uint stateLevel,uint BattlePos)
	{
		mAddStateList.Add(new AddStateData(stateID,stateLevel,BattlePos));
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		for(int i = 0; i < mAddStateList.Count; i++)
		{
			AddStateData data = mAddStateList[i];
			
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(XBattlePosition.Create(data.TargetPos));
			if(obj == null)
				continue;
				
			obj.BuffOper.AddBuff(data.StateID, (byte)data.StateLevel, 1, false, 0);
		}
	}
}

public class SkillEffectData_DisState : SkillEffectData_Base
{
	struct DisStateData
	{
		public DisStateData(uint StateID,uint TargetPos)
		{
			this.StateID = StateID;
			this.TargetPos	= TargetPos;
		}
		public uint StateID;
		public uint TargetPos;
	}
	
	List<DisStateData>	mDisStateList = new List<DisStateData>();
	
	public void AddState(uint stateID,uint BattlePos)
	{
		mDisStateList.Add(new DisStateData(stateID,BattlePos));
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		for(int i = 0; i < mDisStateList.Count; i++)
		{
			DisStateData data = mDisStateList[i];
			
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(XBattlePosition.Create(data.TargetPos));
			if(obj == null)
				continue;
				
			obj.BuffOper.DisperseBuff(data.StateID);
		}
	}
}

public class SkillEffectData_Revive : SkillEffectData_Base
{
	struct ReviveData
	{
		public uint mCurHP;
		public uint mCurAnger;
		public uint mTargetPos;
	}
	
	List<ReviveData>	mList = new List<ReviveData>();
	
	public void AddReviveData(uint curHP,uint curAnger,uint TargetPos)
	{
		ReviveData	data = new ReviveData();
		data.mCurHP		= curHP;
		data.mCurAnger	= curAnger;
		data.mTargetPos	= TargetPos;
		
		mList.Add(data);
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		for(int i = 0; i < mList.Count; i++)
		{
			ReviveData	data	= mList[i];
			XBattlePosition pos = XBattlePosition.Create(data.mTargetPos);
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(pos);
		
			obj.Hp	= (int)data.mCurHP;
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,pos.Group,obj.Hp);
		}		
	}
}

public class SkillEffectData_SuckBlood : SkillEffectData_Base
{
	private struct TargetSuckBlood
	{
		public XBattlePosition mPos;
		public uint mFlag;
		public int mDamageValue;
		public int mAddHP;
	}
	
	public void SetAttackPos(XBattlePosition pos)
	{
		mAttackPos	= pos;
	}
	
	public void SetMainTargetSuckBlood(uint pos,uint flag,int damageValue,int addHP)
	{
		mMainTargetPos			= XBattlePosition.Create(pos);
		
		TargetSuckBlood	damage = new TargetSuckBlood();
		damage.mPos				= mMainTargetPos;
		damage.mFlag			= flag;
		damage.mDamageValue		= damageValue;
		damage.mAddHP			= addHP;
		
		mAllTargetList.Add(damage);
	}
	
	public void AddOtherTargetSuckBlood(uint pos,uint flag,int damageValue,int addHP)
	{
		TargetSuckBlood	damage = new TargetSuckBlood();
		damage.mPos			= XBattlePosition.Create(pos);
		damage.mFlag		= flag;
		damage.mDamageValue	= damageValue;
		damage.mAddHP		= addHP;
		
		mAllTargetList.Add(damage);
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		
		for(int i = 0; i < mAllTargetList.Count; i++)
		{
			TargetSuckBlood damage = mAllTargetList[i];
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(damage.mPos);
			if(obj == null)
				continue;
			int tempLastHP = obj.Hp;
			obj.Hp	+= damage.mDamageValue;
			string flyString = "";
			EAnimName animName = EAnimName.Fight;
			switch((ESkillAttackResult)damage.mFlag)
			{
			case ESkillAttackResult.eSAttackResult_Miss:		// 未击中
				flyString += XStringManager.SP.GetString(14);
				break;
				
			case ESkillAttackResult.eSAttackResult_Defence:		// 抵挡
				flyString += XStringManager.SP.GetString(17);
				animName = EAnimName.Deffence;
				break;
				
			case ESkillAttackResult.eSAttackResult_Dodge:		// 闪避
				flyString += XStringManager.SP.GetString(15);
				animName = EAnimName.Dodge;
				break;

			case ESkillAttackResult.eSAttackResult_Critical:	// 暴击
				flyString += XStringManager.SP.GetString(16);
				animName = EAnimName.Wound;
				break;				
			case ESkillAttackResult.eSAttackResult_Normal:		// 普通攻击
				animName = EAnimName.Wound;
				break;
			}

			if(isNeedAnim && obj.Hp > 0 && animName != EAnimName.Fight)
			{
				if(animName == EAnimName.Wound)
				{
					if(damage.mDamageValue < 0)
						obj._playAnimation(animName,1.0f,false);
				}
				else
				{
					obj._playAnimation(animName,1.0f,false);
				}
				obj._playAnimation(EAnimName.Fight,1.0f,true);
			}
			
			if(damage.mDamageValue != 0)
			{				
				if(damage.mDamageValue < 0)
				{
					flyString += "[color=C42626]";
					flyString += damage.mDamageValue;
				}					
				else
				{
					flyString += "[color=00ff00]";
					flyString += "+";
					flyString += damage.mDamageValue;
				}
				
				int finalDamage = damage.mDamageValue;
				if(obj.Hp <= 0)
				{
					finalDamage = -tempLastHP;
					if(isNeedAnim)
						obj._playAnimation(EAnimName.Death,1.0f,false);
				}
				
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,damage.mPos.Group,finalDamage);
			}
			
			
			if(isNeedAnim)
			{
				if(damage.mFlag == (uint)ESkillAttackResult.eSAttackResult_Miss)
				{
					CurSkill.Attacker.FlyString((EFlyStrType)damage.mFlag, flyString);
				}
				else
					obj.FlyString((EFlyStrType)damage.mFlag, flyString);
			}		
			
			//addHP
			flyString += "[color=00ff00]";
			flyString += "+";
			flyString += damage.mAddHP;
			if(isNeedAnim)
				CurSkill.Attacker.FlyString((EFlyStrType)ESkillAttackResult.eSAttackResult_Normal, flyString);
			CurSkill.Attacker.Hp	+= damage.mAddHP;
			
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,mAttackPos.Group,damage.mAddHP);
		}
	}
	
	private XBattlePosition		mMainTargetPos;
	private XBattlePosition		mAttackPos;
	private List<TargetSuckBlood>	mAllTargetList	= new List<TargetSuckBlood>();
}

public class SkillEffectData_Summon : SkillEffectData_Base
{
	private static uint SummonEffectID = 900028;
	public XBattlePosition		MonsterPos;
	public uint					MonsterID;
	public uint					OwnerLevel;
	
	public override void DoEffect(bool isNeedAnim)
	{
		XBattleMonster summonMonster = BattleDisplayerMgr.SP.AddMonster(MonsterPos,MonsterID,OwnerLevel);
		if(summonMonster != null)
		{
			XU3dEffect Effect = new XU3dEffect(SummonEffectID);
			Effect.Position 	= summonMonster.GetSkeleton(ESkeleton.eMainObject).position;
			Effect.Direction	= Quaternion.Euler(summonMonster.Direction).eulerAngles;
			summonMonster.m_bIsSummon = true;
			
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_ReSet_Max_Blood,MonsterPos.Group,summonMonster.MaxHp);			
		}
	}
}

public class SkillEffectData_Morale : SkillEffectData_Base
{
	public List<XBattlePosition>	PositionList = new List<XBattlePosition>();
	public override void DoEffect(bool isNeedAnim)
	{
		
	}
	
}

public class SkillEffectData_CRI : SkillEffectData_Base
{
	private struct TargetCRI
	{
		public XBattlePosition mPos;
		public uint mFlag;
		public int mDamageValue;
		public bool IsCRI;		
	}
	
	private XBattlePosition		mMainTargetPos;
	private XBattlePosition		mAttackPos;
	private List<TargetCRI>	mAllTargetList	= new List<TargetCRI>();
	
	public void SetAttackPos(XBattlePosition pos)
	{
		mAttackPos	= pos;
	}
	
	public void SetMainTargetCRI(uint pos,uint flag,int damageValue,bool isCRI)
	{
		mMainTargetPos			= XBattlePosition.Create(pos);
		
		TargetCRI	damage = new TargetCRI();
		damage.mPos				= mMainTargetPos;
		damage.mFlag			= flag;
		damage.mDamageValue		= damageValue;
		damage.IsCRI			= isCRI;
		
		mAllTargetList.Add(damage);
	}
	
	public void AddOtherTargetCRI(uint pos,uint flag,int damageValue,bool isCRI)
	{
		TargetCRI	damage = new TargetCRI();
		damage.mPos			= XBattlePosition.Create(pos);
		damage.mFlag		= flag;
		damage.mDamageValue	= damageValue;
		damage.IsCRI			= isCRI;
		
		mAllTargetList.Add(damage);
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		
		for(int i = 0; i < mAllTargetList.Count; i++)
		{
			TargetCRI damage = mAllTargetList[i];
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(damage.mPos);
			if(obj == null)
				continue;
			if(damage.IsCRI)
			{
				string flyString = "";
				flyString += XStringManager.SP.GetString(63);
				flyString += "[color=C42626]";
				flyString += damage.mDamageValue;
				
				obj.Hp	+= damage.mDamageValue;
				
				obj.FlyString(EFlyStrType.eFlyStrType_Crit, flyString);
				
				int finalDamage = damage.mDamageValue;
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,damage.mPos.Group,finalDamage);
			}
			else
			{
				int tempLastHP = obj.Hp;
				obj.Hp	+= damage.mDamageValue;
				string flyString = "";
				EAnimName animName = EAnimName.Fight;
				switch((ESkillAttackResult)damage.mFlag)
				{
				case ESkillAttackResult.eSAttackResult_Miss:		// 未击中
					flyString += XStringManager.SP.GetString(14);
					break;
					
				case ESkillAttackResult.eSAttackResult_Defence:		// 抵挡
					flyString += XStringManager.SP.GetString(17);
					animName = EAnimName.Deffence;
					break;
					
				case ESkillAttackResult.eSAttackResult_Dodge:		// 闪避
					flyString += XStringManager.SP.GetString(15);
					animName = EAnimName.Dodge;
					break;
	
				case ESkillAttackResult.eSAttackResult_Critical:	// 暴击
					flyString += XStringManager.SP.GetString(16);
					animName = EAnimName.Wound;
					break;				
				case ESkillAttackResult.eSAttackResult_Normal:		// 普通攻击
					animName = EAnimName.Wound;
					break;
				}
	
				if(obj.Hp > 0 && animName != EAnimName.Fight)
				{
					if(animName == EAnimName.Wound)
					{
						if(damage.mDamageValue < 0)
							obj._playAnimation(animName,1.0f,false);
					}
					else
					{
						obj._playAnimation(animName,1.0f,false);
					}
					obj._playAnimation(EAnimName.Fight,1.0f,true);
				}
				
				if(damage.mDamageValue != 0)
				{				
					if(damage.mDamageValue < 0)
					{
						flyString += "[color=C42626]";
						flyString += damage.mDamageValue;
					}					
					else
					{
						flyString += "[color=00ff00]";
						flyString += "+";
						flyString += damage.mDamageValue;
					}
					
					int finalDamage = damage.mDamageValue;
					if(obj.Hp <= 0)
					{
						finalDamage = -tempLastHP;
						if(isNeedAnim)
							obj._playAnimation(EAnimName.Death,1.0f,false);
					}
					
					XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,damage.mPos.Group,finalDamage);
				}
					
				if(damage.mFlag == (uint)ESkillAttackResult.eSAttackResult_Miss)
				{
					CurSkill.Attacker.FlyString((EFlyStrType)damage.mFlag, flyString);
				}
				else
					obj.FlyString((EFlyStrType)damage.mFlag, flyString);
			}
		}
	}
}

public class SkillEffectData_MultiAttack : SkillEffectData_Base
{
	private struct TargetMultiDamage
	{
		public XBattlePosition mPos;
		public uint mFlag;
		public List<int>	DamageValueList ;
		public int mCurNo ;
	}
	private List<TargetMultiDamage>	mAllTargetList	= new List<TargetMultiDamage>();
//	public XBattlePosition targetPos;
//	public uint Flag;
//	public List<int>	DamageList = new List<int>();	
//	
//	private int mCurNo = 0;
	
	public void SetTargetDamage(uint pos,uint flag,IList<int> damageArray)
	{
		TargetMultiDamage	damage = new TargetMultiDamage();
		damage.DamageValueList = new List<int>();	
		damage.mCurNo = 0;
		damage.mPos				= XBattlePosition.Create(pos);
		damage.mFlag			    = flag;
		
		foreach(int damageValue in damageArray)
		{
			damage.DamageValueList.Add(damageValue);
		}
		mAllTargetList.Add(damage);
	}
	
	public override void DoEffect(bool isNeedAnim)
	{
		
	}
	
	public void HandleMultiDamage(int num,bool isNeedAnim)
	{
		for(int i = 0; i < mAllTargetList.Count; i++)
		{
			TargetMultiDamage damage = mAllTargetList[i];
			XBattleObject obj = BattleDisplayerMgr.SP.GetBattleObject(damage.mPos);
			if(obj == null)
				continue;
			int tempLastHP = obj.Hp;
			obj.Hp	+= damage.DamageValueList[damage.mCurNo];
			string flyString = "";
			EAnimName animName = EAnimName.Fight;
			switch((ESkillAttackResult)damage.mFlag)
			{
			case ESkillAttackResult.eSAttackResult_Miss:		// 未击中
				flyString += XStringManager.SP.GetString(14);
				break;
				
			case ESkillAttackResult.eSAttackResult_Defence:		// 抵挡
				flyString += XStringManager.SP.GetString(17);
				animName = EAnimName.Deffence;
				break;
				
			case ESkillAttackResult.eSAttackResult_Dodge:		// 闪避
				flyString += XStringManager.SP.GetString(15);
				animName = EAnimName.Dodge;
				break;
	
			case ESkillAttackResult.eSAttackResult_Critical:	// 暴击
				flyString += XStringManager.SP.GetString(16);
				animName = EAnimName.Wound;
				break;				
			case ESkillAttackResult.eSAttackResult_Normal:		// 普通攻击
				animName = EAnimName.Wound;
				break;
			}
	
			if(obj.Hp > 0 && animName != EAnimName.Fight && damage.DamageValueList[damage.mCurNo] < 0)
			{
				obj._playAnimation(animName,1.0f,false);
				obj._playAnimation(EAnimName.Fight,1.0f,true);
			}
			
			//播放受击特效
			XSkillDefine SkillDef  = SkillManager.SP.GetSkillDefine((ushort)CurSkill.SkillID);
			XSkillLevelDefine SkillLevel = SkillDef.Levels[(byte)CurSkill.SkillLevelID];		
			
			Vector3 selfRot;
			Transform attachPos;		
			XU3dEffect Effect = new XU3dEffect((uint)SkillLevel.HitEffectID[damage.mCurNo]);	
			
			attachPos 	= obj.GetSkeleton(SkillLevel.HitEffectBind[damage.mCurNo]);
			selfRot		= obj.Direction;
	
			Effect.Position 	= attachPos.position;
			Effect.Direction	= Quaternion.Euler(selfRot).eulerAngles;	
			
			if(damage.DamageValueList[damage.mCurNo] != 0)
			{				
				if(damage.DamageValueList[damage.mCurNo] < 0)
				{
					flyString += "[color=C42626]";
					flyString += damage.DamageValueList[damage.mCurNo];
				}					
				else
				{
					flyString += "[color=00ff00]";
					flyString += "+";
					flyString += damage.DamageValueList[damage.mCurNo];
				}
				
				int finalDamage = damage.DamageValueList[damage.mCurNo];
				if(obj.Hp <= 0)
				{
					finalDamage = -tempLastHP;
					if(isNeedAnim)
						obj._playAnimation(EAnimName.Death,1.0f,false);
				}
				
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Sub_Blood,damage.mPos.Group,finalDamage);
			}
				
			if(damage.mFlag == (uint)ESkillAttackResult.eSAttackResult_Miss)
			{
				CurSkill.Attacker.FlyString((EFlyStrType)damage.mFlag, flyString);
			}
			else
				obj.FlyString((EFlyStrType)damage.mFlag, flyString);
			
			damage.mCurNo++;
		}
	}
}



#endregion

//战斗单人行为
#region BattleAction
public class BattleActionBase
{
	public virtual void Display()	{}
}

public class BattleAction : BattleActionBase
{
	public BattleAction(BattleRound round)
	{
		CurBattleRound	= round;
		mCurSkill		= new XSkill();
		initBehaviourSM();
	}
	
	private void initBehaviourSM()
	{
		m_BehaviourSM = new XStateMachince(new XBattleNULL(EStateId.esBattle_Null,this));
		m_BehaviourSM.RegState(new XBattleMove(EStateId.esBattle_Move,this));
		m_BehaviourSM.RegState(new XBattle_BeforeUseSkill(EStateId.esBattle_Pre_UseSkill,this));
		m_BehaviourSM.RegState(new XBattle_UseSkill(EStateId.esBattle_Use_Skill,this));
		m_BehaviourSM.RegState(new XBattle_AfterUseSkill(EStateId.esBattle_After_UseSkill,this));
		m_BehaviourSM.RegState(new XBattleMoveBack(EStateId.esBattle_Move_Back,this));
		m_BehaviourSM.RegState(new XBattle_RunForward(EStateId.esBattle_RunForward,this));
		m_BehaviourSM.RegState(new XBattle_CutScene(EStateId.esBattle_Cut_Scene,this));
	}
	
	public bool IsWillBeStriked = false;
	public bool IsStriker = false;
	
	public struct DotEffect
	{
		public XBattlePosition pos;
		public uint CurHP;
	}
	
	public DotEffect AddDotEffect()
	{
		DotEffect effect = new DotEffect();
		mDotEffectList.Add(effect);
		return effect;
	}
	
	public void ActionBegin()
	{	
		// 表现行动开始时所有的buff效果
		m_BuffDisplayMulti.Display(EBuffEffectDisplayTime.BUFF_DISPLAY_TIME_BEFORE_ACTION);
		//autoHP
		XBattleObject Attacker = BattleDisplayerMgr.SP.GetBattleObject(AttackBattlePos);
		
		XCfgSkillBase skillBase = XCfgSkillBaseMgr.SP.GetConfig((ushort)mCurSkill.SkillID );
		if(skillBase != null && 0!=skillBase.ShowCutSceneAnimation)
		{
			m_BehaviourSM.TranslateToState( (int)EStateId.esBattle_Cut_Scene,skillBase.ShowCutSceneAnimation );
		}
		
		if(Attacker != null)
		{
			Attacker.Hp += AutoHP;
		}
		
		//DOT
		for(int i = 0; i < mDotEffectList.Count; i++)
		{
			//XBattleObject battleObj = BattleDisplayerMgr.SP.GetBattleObject(mDotEffectList[i].pos);
			//if(battleObj != null)
			//	battleObj.AddState();
		}
	}
	
	public void ActionEnd()
	{
		// 表现行动结束后的所有buff效果
		m_BuffDisplayMulti.Display(EBuffEffectDisplayTime.BUFF_DISPLAY_TIME_AFTER_ACTION);
		if(CurBattleRound != null)
			CurBattleRound.AddBattleActionID();
	}
	
	public override void Display()
	{
		m_BehaviourSM.Breathe();
	}
	
	public void InitBuffDisplayMulti(IList<PB_SameTimeBuffEffect> mul)
	{
		m_BuffDisplayMulti = new XBuffDisplayMulti(mul);
	}
	
	public SkillEffectData_Base AddSkillEffectData(ECOM_EFFECT_TYPE effectDataType)
	{
		SkillEffectData_Base baseSkillEfectData = null;
		switch(effectDataType)
		{
			case ECOM_EFFECT_TYPE.COM_EFFECT_COM_DAMAGE:
			{
				baseSkillEfectData	= new SkillEffectData_ComDamage();
			}			
			break;	
			case ECOM_EFFECT_TYPE.COM_EFFECT_ADD_STATE:
			{
				baseSkillEfectData	= new SkillEffectData_AddState();
			}
			break;
			case ECOM_EFFECT_TYPE.COM_EFFECT_DEL_STATE:
			{
				baseSkillEfectData	= new SkillEffectData_DisState();
			}
			break;
			case ECOM_EFFECT_TYPE.COM_EFFECT_REVIVE:
			{
				baseSkillEfectData	= new SkillEffectData_Revive();
			}
			break;
			case ECOM_EFFECT_TYPE.COM_EFFECT_SuckBlood:
			{
				baseSkillEfectData	= new SkillEffectData_SuckBlood();
			}
			break;
		case ECOM_EFFECT_TYPE.COM_EFFECT_SUMMON:
			{
				baseSkillEfectData	= new SkillEffectData_Summon();
			}
			break;
		case ECOM_EFFECT_TYPE.COM_EFFECT_MORALE:
			{
				baseSkillEfectData	= new SkillEffectData_Morale();
			} 
			break;
		case ECOM_EFFECT_TYPE.COM_EFFECT_CRI:
			{
				baseSkillEfectData	= new SkillEffectData_CRI();
			}
			break;
		case ECOM_EFFECT_TYPE.COM_EFFECT_MultiAttack:
			{
				baseSkillEfectData	= new SkillEffectData_MultiAttack();
			}
			break;
		default:
			break;
		}
		
		mEffectData.Add(baseSkillEfectData);
		
		return baseSkillEfectData;
	}
	
	public void Action_DamageHandle(int num,bool isNeedAnim)
	{
		for(int i = 0; i < mEffectData.Count; i++)
		{
			mEffectData[i].DoEffect(isNeedAnim);
		}
		mCurSkill.m_BuffDisplayMulti.Display(EBuffEffectDisplayTime.BUFF_DISPLAY_TIME_SKILL_ATTACK);
	}
	
	public BattleRound		CurBattleRound	{get;	set;}
	public XBattlePosition	AttackBattlePos {get; 	set;} 
	public XBattlePosition	MainTargetPos	{get;   set;}
	public uint				ActionRound 	{get; 	set;}
	public int				AutoHP 			{get; 	set;}
	public bool				IsBeStriked 	{get; 	set;}
	public bool				IsMoraleAttack 	{get;	set;}
	
	private List<DotEffect>	mDotEffectList = new List<DotEffect>();
	public XSkill	mCurSkill	{get;set;}	
	private XStateMachince m_BehaviourSM;
	private List<SkillEffectData_Base>	mEffectData = new List<SkillEffectData_Base>();
	private XBuffDisplayMulti m_BuffDisplayMulti = null;
}

public class BattleAction_RunBack : BattleActionBase
{
	public BattleAction_RunBack(BattleRound round)
	{
		CurBattleRound	= round;
		initBehaviourSM();
	}
	
	private void initBehaviourSM()
	{
		m_BehaviourSM = new XStateMachince(new XBattle_StrikedRunBack(EStateId.esBattle_StrikedRunBack,this));
	}
	
	public override void Display()
	{
		m_BehaviourSM.Breathe();
	}
	
	public void ActionEnd()
	{
		if(CurBattleRound != null)
			CurBattleRound.AddBattleActionID();
	}
	
	public BattleRound		CurBattleRound	{get;	set;}
	public XBattlePosition	AttackBattlePos {get; 	set;} 
	private XStateMachince m_BehaviourSM;
	
}

#endregion


#region BattleRound
public struct BattleMoraleData
{
	public uint mPos;
	public bool	mIsFull;
}

//战斗中一回合
public class BattleRound
{	
	public BattleRound(BattleDisplayerMgr displayerMgr)
	{
		DisplayMgr		= displayerMgr;
		mCurActionCount	= 0;
		mCurState		= Round_State.Round_State_Begin;
		mStateFuncList[(int)Round_State.Round_State_Begin]		= new RoundStateFunc(BattleRound_Start);
		mStateFuncList[(int)Round_State.Round_State_Process]	= new RoundStateFunc(BattleRound_Process);
		mStateFuncList[(int)Round_State.Round_State_End]		= new RoundStateFunc(BattleRound_End);		
	}
	
	public enum Round_State
	{
		Round_State_Begin,
		Round_State_Process,
		Round_State_End,
		Round_State_Num,
	}
	
	public void Display()
	{
		if(mCurState < Round_State.Round_State_Num)
		{
			mStateFuncList[(int)mCurState]();
		}
	}
	
	public void InitBuffDisplayMulti(IList<PB_SameTimeBuffEffect> mul)
	{
		m_BuffDisplayMulti = new XBuffDisplayMulti(mul);
	}
	
	public BattleAction AddBattleAction()
	{
		BattleAction ba = new BattleAction(this);
		mBattleActionList.Add(ba);
		return ba;
	}
	
	public BattleAction_RunBack AddBattleRunBack()
	{
		BattleAction_RunBack ba = new BattleAction_RunBack(this);
		mBattleActionList.Add(ba);
		return ba;
	}
	
	public void AddMoraleData(uint pos,bool isFull)
	{
		BattleMoraleData data = new BattleMoraleData();
		data.mPos		= pos;
		data.mIsFull 	= isFull;
		
		mMoraleList.Add(data);		
	}
	
	public void AddBattleActionID()
	{
		mCurActionCount++;
		if(mCurActionCount >= mBattleActionList.Count)
			BattleRound_End();
	}
	
	private void BattleRound_Start()
	{		
		for(int i = 0; i < mMoraleList.Count; i++)
		{
			XBattlePosition pos = XBattlePosition.Create(mMoraleList[i].mPos);
			XBattleObject obj = DisplayMgr.GetBattleObject(pos);
			if(obj != null)
				obj.IsFullMorale = mMoraleList[i].mIsFull;
		}
		
		mCurState	= Round_State.Round_State_Process;
		
		// 表现回合开始瞬间的所有buff效果
		m_BuffDisplayMulti.Display(EBuffEffectDisplayTime.BUFF_DISPLAY_TIME_BEFORE_ROUND);
	}
	
	private void BattleRound_End()
	{
		if(DisplayMgr != null)
			DisplayMgr.AddBattleRoundID();
		
		if(DisplayMgr.IsEnd())
		{
			XEventManager.SP.SendEvent(EEvent.Show_Fight_Result);
		}
		
		
	}
	
	private void BattleRound_Process()
	{
		if(mCurActionCount < mBattleActionList.Count)
		{
			mBattleActionList[mCurActionCount].Display();
		}
	}
	
	public BattleActionBase	GetBattleAcion(int i)
	{
		return mBattleActionList[i];
	}
	
	public int GetActionNum()
	{
		return mBattleActionList.Count;
	}
	
	private delegate void RoundStateFunc();
	RoundStateFunc[] mStateFuncList = new RoundStateFunc[(int)Round_State.Round_State_Num];
	
	Round_State	mCurState;
	int mCurActionCount;
	public BattleDisplayerMgr	DisplayMgr {get;set;}
	List<BattleActionBase> 	mBattleActionList	= new List<BattleActionBase>();
	List<BattleMoraleData> 	mMoraleList		= new List<BattleMoraleData>();
	
	private XBuffDisplayMulti m_BuffDisplayMulti = null;
}
#endregion
