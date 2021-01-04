using System.Timers;
using XGame.Client.Packets;
using UnityEngine;
using resource;

public enum ECom_Effect
{
	ECom_Effect_Move	= 900051,
	//ECom_Effect_Morale	= 900052,
	ECom_Effect_Morale	= 900512,
	ECom_Effect_Lucky	= 900053,
	ECom_Effect_Strike = 900511,
	ECom_Effect_Mask_Effect = 900117,
}

public class XBattleNULL : XState<BattleAction>
{
	public XBattleNULL(EStateId id, BattleAction owner)
		: base((int)id, owner)
	{
	}
	public override void Breathe(float deltaTime)
	{
		
		
		Machine.TranslateToState((int)EStateId.esBattle_RunForward);
	}
}

public class XBattleMove : XState<BattleAction>
{
	public XBattleMove(EStateId id, BattleAction owner)
		: base((int)id, owner)
	{
	}	
	
	public void StartMove()
	{
		//瞬移到攻击目标前面		
		XBattleObject target = m_Owner.mCurSkill.MainTarget as XBattleObject;
		if(target == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMove state target not find !");
			return ;
		}
			
		
		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
		if(attack == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMove state attack not find !");
			return ;
		}
			

		XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Move);
		Effect.Position 	= attack.GetSkeleton(ESkeleton.eMainObject).position;
		Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
		
		
		float UnitDist = 3.0f;
		XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Unit_Dist);
		if(param != null)
			UnitDist	= param.Value;
		else
			Log.Write(LogLevel.WARN,"XBattleMove state GameParam_Define.Battle_Unit_Dist not find !");

		attack.Position		= target.Position + Quaternion.Euler( target.Direction) * (Vector3.forward * UnitDist);
		Machine.TranslateToState((int)EStateId.esBattle_Pre_UseSkill);
		
	}
	
	public override void Enter(params object[] args)
	{
		m_Owner.ActionBegin();
		
		if(m_Owner.IsMoraleAttack)
		{
			XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
			if(attack == null)
			{
				Log.Write(LogLevel.ERROR,"XBattleMove state skill id = {0}",m_Owner.mCurSkill.SkillID);
				return ;
			}
				
			XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Morale);
			Effect.Position 	= attack.GetSkeleton(ESkeleton.eCapsuleTop).position;
			Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
			
			float MoraleTime = 1.0f;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Morale_Time);
			if(param != null)
				MoraleTime	= param.Value;
			
			mMoraleTimeCalc.BeginTimeCalc(MoraleTime,true);			
			
		}
		
		XSkillDefine skillDefine = SkillManager.SP.GetSkillDefine((ushort)m_Owner.mCurSkill.SkillID);
		if(skillDefine == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMove state skill id = {0}",m_Owner.mCurSkill.SkillID);
			return ;
		}
			
		
		if(skillDefine.UseWay == ESkillUseWay.eSUseWay_CloseCombat)
		{
			float beforeMoveTime = 1.0f;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Attack_Before_Move);
			if(param != null)
				beforeMoveTime	= param.Value;
			
			mTimeCalc.BeginTimeCalc(beforeMoveTime,true);
		}
		else
		{
			Machine.TranslateToState((int)EStateId.esBattle_Pre_UseSkill);
		}
	}
	public override void Breathe(float deltaTime)
	{
		if(mMoraleTimeCalc.IsStart())
		{
			if(!mMoraleTimeCalc.CountTime(deltaTime))
				return ;
		}
		if(mTimeCalc.CountTime(deltaTime))
		{
			//时间到了
			StartMove();
		}
	}
	public override void Exit()
	{
		
	}
	
	private TimeCalc mTimeCalc	= new TimeCalc();
	private TimeCalc mMoraleTimeCalc = new TimeCalc();
}

public class XBattleMoveBack : XState<BattleAction>
{
	public XBattleMoveBack(EStateId id, BattleAction owner)
		: base((int)id, owner)
	{
	}	
	
	public void MoveBack()
	{
		//瞬移到回
		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
		if(attack == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMoveBack state attack not find ");
			return ;
		}
		
		XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Move);
		Effect.Position 	= attack.GetSkeleton(ESkeleton.eMainObject).position;
		Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
		
		attack.Position	 = BattleDisplayerMgr.GetFighterPos(m_Owner.AttackBattlePos.Group,(int)m_Owner.AttackBattlePos.Position);
		//m_Owner.CurBattleRound.AddBattleActionID();
		m_Owner.ActionEnd();
		
	}
	
	public override void Enter(params object[] args)
	{
		XSkillDefine skillDefine = SkillManager.SP.GetSkillDefine((ushort)m_Owner.mCurSkill.SkillID);
		if(skillDefine == null)
			return ;
		
		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
		if(attack == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMoveBack state attack not find ");
			return ;
		}
			
		
		if(skillDefine.UseWay == ESkillUseWay.eSUseWay_CloseCombat)
		{
			mTimeCalc.BeginTimeCalc(0,true);
		}
		else
		{
			//m_Owner.CurBattleRound.AddBattleActionID();
			m_Owner.ActionEnd();
		}
	}
	public override void Breathe(float deltaTime)
	{
		if(mTimeCalc.CountTime(deltaTime))
		{
			//时间到了
			MoveBack();
		}
	}
	public override void Exit()
	{
		
	}
	
	private TimeCalc mTimeCalc	= new TimeCalc();
}

public class XBattle_StrikedRunBack : XState<BattleAction_RunBack>
{
	public XBattle_StrikedRunBack(EStateId id, BattleAction_RunBack owner)
	: base((int)id, owner)
	{
	}
	
	public override void Enter(params object[] args)
	{
//		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
//		if(attack == null)
//		{
//			Log.Write(LogLevel.ERROR,"XBattleMoveBack state attack not find ");
//			return ;
//		}
//		
//		attack._playAnimation(EAnimName.Fight,1.0f,false);
		mTimeCalc.BeginTimeCalc(0,true);
	}
	
	public override void Breathe(float deltaTime)
	{
		if(mTimeCalc.CountTime(deltaTime))
		{
			//时间到了
			MoveBack();
		}
	}
	
	public void MoveBack()
	{
		//瞬移到回
		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
		if(attack == null)
		{
			Log.Write(LogLevel.ERROR,"XBattleMoveBack state attack not find ");
			return ;
		}
			
		
		XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Move);
		Effect.Position 	= attack.GetSkeleton(ESkeleton.eMainObject).position;
		Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
		
		attack.Position	 = BattleDisplayerMgr.GetFighterPos(m_Owner.AttackBattlePos.Group,(int)m_Owner.AttackBattlePos.Position);		
		m_Owner.ActionEnd();
	}
	
	private TimeCalc mTimeCalc	= new TimeCalc();
}

public class XBattle_BeforeUseSkill : XState<BattleAction>
{
	public XBattle_BeforeUseSkill(EStateId id, BattleAction owner)
	: base((int)id, owner)
	{
	}
	
	public override void Enter(params object[] args)
	{
		XSkillDefine skillDefine = SkillManager.SP.GetSkillDefine((ushort)m_Owner.mCurSkill.SkillID);
		if(skillDefine == null)
		{
			//Log.Write(LogLevel.WARN,"XBattle_BeforeUseSkill state skill ID is {0}",m_Owner.mCurSkill.SkillID);
			return ;
		}	
		
		XCharacter attack = m_Owner.mCurSkill.Attacker;
		
		if(m_Owner.mCurSkill.IsLucky)
		{			
			XU3dEffect luckEffect 	= new XU3dEffect((uint)ECom_Effect.ECom_Effect_Lucky);
			luckEffect.Position 	= attack.GetSkeleton(ESkeleton.eWaist).position;
			luckEffect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
			
			float LuckyTime = 0.0f;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Lucky_Time);
			if(param != null)
				LuckyTime	= param.Value;
			
			mLuckyTimeCalc.BeginTimeCalc(LuckyTime,true);
		}

		//绝技
		if(skillDefine.FuncType == ESkillFuncType.eSFuncType_Unique)
		{
			AddMaskEffect();
			mMaskTimeCalc.BeginTimeCalc(MaskEffect.TotalChangeTime,true);
		}
		else
		{
			Machine.TranslateToState((int)EStateId.esBattle_Use_Skill);
		}
	}
	
	public override void Breathe(float deltaTime)
	{
		if(mLuckyTimeCalc.IsStart())
		{
			if(!mLuckyTimeCalc.CountTime(deltaTime))
				return ;
		}		
		
		if(mMaskTimeCalc.CountTime(deltaTime))
		{
			XCharacter attack = m_Owner.mCurSkill.Attacker;
			XU3dEffect luckEffect 	= new XU3dEffect((uint)ECom_Effect.ECom_Effect_Mask_Effect);
			luckEffect.Position 	= attack.GetSkeleton(ESkeleton.eWaist).position;
			luckEffect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
			
			Machine.TranslateToState((int)EStateId.esBattle_Use_Skill);
		}
	}
	
	private void AddMaskEffect()
	{
		Camera mainCamera = Camera.main;
		if(null == mainCamera) return;
		MaskEffect effect = mainCamera.gameObject.GetComponent<MaskEffect>() as MaskEffect;
		if(effect == null)
		{
			effect = mainCamera.gameObject.AddComponent<MaskEffect>();
			effect.ResMaterial	= XResourceManager.GetResource(XResourceMaterial.ResTypeName,MaskEffect.MaskMaterialID) as XResourceMaterial;
			if(effect.ResMaterial == null)
			{
				Log.Write(LogLevel.ERROR,"cant find Material ID {0}",MaskEffect.MaskMaterialID);
				return ;
			}

			if(effect.ResMaterial.IsLoadDone())
			{
				effect.LoadMaterial();
			}
			else
			{
				XResourceManager.StartLoadResource(XResourceMaterial.ResTypeName,MaskEffect.MaskMaterialID);
				effect.ResMaterial.ResLoadEvent	+= effect.ResMaterial.LoadCompleted;
				effect.ResMaterial.ResLoadEvent	+= effect.LoadCompleted;
			}
		}
		else
		{
			effect.enabled	= true;
		}
	}
	
	private TimeCalc 	mMaskTimeCalc = new TimeCalc();
	private TimeCalc	mLuckyTimeCalc = new TimeCalc();	
}

public class XBattle_UseSkill : XState<BattleAction>
{
	public XBattle_UseSkill(EStateId id, BattleAction owner)
	: base((int)id, owner)
	{
	}
	
	public override void Enter(params object[] args)
	{
		if(m_Owner != null && m_Owner.mCurSkill != null)
		{
			m_Owner.mCurSkill.Start();
			
			if(m_Owner.IsStriker)
			{	
				XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
				if(attack == null)
				{
					Log.Write(LogLevel.ERROR,"XBattleMove state skill id = {0}",m_Owner.mCurSkill.SkillID);
					return ;
				}
				XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Strike);
				Effect.Position 	= new Vector3(attack.GetSkeleton(ESkeleton.eCapsuleTop).position.x + 1,attack.GetSkeleton(ESkeleton.eCapsuleTop).position.y,attack.GetSkeleton(ESkeleton.eCapsuleTop).position.z - 1);
				Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;
				
				float StrikeTime = 1.0f;
				XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Morale_Time);
				if(param != null)
					StrikeTime	= param.Value;
				
				mStrikeTimeCalc.BeginTimeCalc(StrikeTime,true);
				
			}
				
			//only not striker need check fire soul value
			if( false ==m_Owner.IsStriker)
			{
				BattleDisplayerMgr.SP.FireSoulValue(m_Owner.AttackBattlePos );
			}
		}
	}
	
	public override void Breathe(float deltaTime)
	{
		if(mStrikeTimeCalc.IsStart())
		{
			if(!mStrikeTimeCalc.CountTime(deltaTime))
				return ;
		}
		
		if(m_Owner == null || m_Owner.mCurSkill == null)
			return ;
		
		
		m_Owner.mCurSkill.Breath();
		
		if(m_Owner.mCurSkill.IsEnd())
			Machine.TranslateToState((int)EStateId.esBattle_After_UseSkill);
	}
	
	public override void Exit()
	{
		
	}
	private TimeCalc mStrikeTimeCalc 	= new TimeCalc();
}

public class XBattle_AfterUseSkill : XState<BattleAction>
{
	public XBattle_AfterUseSkill(EStateId id, BattleAction owner)
	: base((int)id, owner)
	{
	}
	
	public override void Enter(params object[] args)
	{
		XSkillDefine skillDefine = SkillManager.SP.GetSkillDefine((ushort)m_Owner.mCurSkill.SkillID);
		if(skillDefine == null)
		{
			Log.Write(LogLevel.ERROR,"XBattle_AfterUseSkill state skill ID is {0}",m_Owner.mCurSkill.SkillID);
			return ;
		}
			
		
		//绝技
		if(skillDefine.FuncType == ESkillFuncType.eSFuncType_Unique)
		{
			DisMaskEffect();
		}
		
		if(!m_Owner.IsWillBeStriked)
			Machine.TranslateToState((int)EStateId.esBattle_Move_Back);
		else
			m_Owner.ActionEnd();
	}
	
	public override void Breathe(float deltaTime)
	{
		
	}
	
	private void DisMaskEffect()
	{
		Camera mainCamera = Camera.main;
		if(null == mainCamera) return;
		MaskEffect effect = mainCamera.gameObject.GetComponent<MaskEffect>() as MaskEffect;
		if(effect == null)
		{
			//MaskEffect newEffect = mainCamera.gameObject.AddComponent<MaskEffect>() as MaskEffect;
		}
		else
		{
			effect.enabled	= false;
		}
	}
}

public class XBattle_RunForward : XState<BattleAction>
{
	private float BATTLE_MOVE_SPEED = 100.0f;
	public XBattle_RunForward(EStateId id, BattleAction owner)
	: base((int)id, owner)
	{
		IsEndMove	= false;
	}
	
	public void OnMoveDone()
	{
		IsEndMove	= true;
		if(m_Owner.mCurSkill.Attacker == null)
			m_Owner.mCurSkill.Attacker	= BattleDisplayerMgr.SP.GetBattleObject(m_Owner.AttackBattlePos);
		if(m_Owner.mCurSkill.Attacker == null)
			return ;
		
		m_Owner.mCurSkill.Attacker.Direction	= ObjDir;
		m_Owner.mCurSkill.Attacker.StartFight();
	}
	
	public void StartMove()
	{	
		//瞬移到攻击目标前面
		if(m_Owner.mCurSkill.MainTarget == null)
			m_Owner.mCurSkill.MainTarget	= BattleDisplayerMgr.SP.GetBattleObject(m_Owner.MainTargetPos);
		
		XBattleObject target = m_Owner.mCurSkill.MainTarget as XBattleObject;		
		if(target == null)
		{			
			Log.Write(LogLevel.WARN,"XBattleMove state target not find !");
			IsEndMove	= true;
			return ;
		}
			
		
		XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
		if(attack == null)
		{
			Log.Write(LogLevel.WARN,"XBattleMove state attack not find !");
			IsEndMove	= true;
			return ;
		}
		
		float UnitDist = 3.0f;
		XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Unit_Dist);
		if(param != null)
			UnitDist	= param.Value;
		else
			Log.Write(LogLevel.WARN,"XBattleMove state GameParam_Define.Battle_Unit_Dist not find !");
		
		Vector3 targetPos = target.Position + Quaternion.Euler( target.Direction) * (Vector3.forward * UnitDist);

		//attack.Position		= target.Position + Quaternion.Euler( target.Direction) * (Vector3.forward * UnitDist);
		//Machine.TranslateToState((int)EStateId.esBattle_Pre_UseSkill);
		ObjDir	= attack.Direction;
		attack.SegmentMoveTo(targetPos, BATTLE_MOVE_SPEED, OnMoveDone,EAnimName.Fight);
		
	}
	
	public override void Enter(params object[] args)
	{
		m_Owner.ActionBegin();	

//		
//		attack._playAnimation(EAnimName.Fight,1.0f,false);
		
		if(m_Owner.IsMoraleAttack)
		{	
			XBattleObject attack = m_Owner.CurBattleRound.DisplayMgr.GetBattleObject(m_Owner.AttackBattlePos);
			if(attack == null)
			{
				Log.Write(LogLevel.ERROR,"XBattleMove state skill id = {0}",m_Owner.mCurSkill.SkillID);
				return ;
			}
			XU3dEffect Effect = new XU3dEffect((uint)ECom_Effect.ECom_Effect_Morale);
			//Effect.Position 	= attack.GetSkeleton(ESkeleton.eCapsuleTop).position;
			Effect.Position 	= new Vector3(attack.GetSkeleton(ESkeleton.eCapsuleTop).position.x + 1,attack.GetSkeleton(ESkeleton.eCapsuleTop).position.y,attack.GetSkeleton(ESkeleton.eCapsuleTop).position.z - 1);
			Effect.Direction	= Quaternion.Euler(attack.Direction).eulerAngles;

			
			float MoraleTime = 1.0f;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Morale_Time);
			if(param != null)
				MoraleTime	= param.Value;
			
			mMoraleTimeCalc.BeginTimeCalc(MoraleTime,true);
			
		}
		
		XSkillDefine skillDefine = SkillManager.SP.GetSkillDefine((ushort)m_Owner.mCurSkill.SkillID);
		if(skillDefine == null)
		{
			Log.Write(LogLevel.WARN,"XBattleMove state skill id = {0}",m_Owner.mCurSkill.SkillID);
			IsEndMove	= true;
			m_Owner.ActionEnd();
			return ;
		}
			
		
		if(skillDefine.UseWay == ESkillUseWay.eSUseWay_CloseCombat)
		{
			float beforeMoveTime = 1.0f;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Attack_Before_Move);
			if(param != null)
				beforeMoveTime	= param.Value;
			
			mTimeCalc.BeginTimeCalc(beforeMoveTime,true);
		}
		else
		{
			Machine.TranslateToState((int)EStateId.esBattle_Pre_UseSkill);
		}
	}
	public override void Breathe(float deltaTime)
	{
		if(mMoraleTimeCalc.IsStart())
		{
			if(!mMoraleTimeCalc.CountTime(deltaTime))
				return ;
		}
		if(mTimeCalc.CountTime(deltaTime))
		{
			//时间到了
			StartMove();
		}		

		if(IsEndMove)
			Machine.TranslateToState((int)EStateId.esBattle_Pre_UseSkill);

	}
	public override void Exit()
	{
		
	}
	
	private TimeCalc mTimeCalc			= new TimeCalc();
	private TimeCalc mMoraleTimeCalc 	= new TimeCalc();
	bool	IsEndMove;
	Vector3	ObjDir	= Vector3.zero;
}

public class XBattle_CutScene : XState<BattleAction>
{
	public XBattle_CutScene(EStateId id, BattleAction owner)
	: base((int)id, owner)
	{
	}
	
	public override void Enter(params object[] args)
	{
		XCutSceneMgr.SP.m_BatTargetPosList.Clear();
		
		XCutSceneMgr.SP.m_curBattleAction = m_Owner;
		
		XCutSceneMgr.SP.addBattleCutScene( (int)args[0] );
		
		if( false == m_Owner.IsStriker)
		{
			BattleDisplayerMgr.SP.FireSoulValue(m_Owner.AttackBattlePos );
		}
	}
	
	public override void Breathe(float deltaTime)
	{
		
	}
	
	public override void Exit()
	{
		
	}
}




