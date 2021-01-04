using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

/*
 * 运行中技能, 只用于技能表现，修改的时候，要保证能不依赖其他，能单独使用
 */
public class XSkill
{
	public  XSkill()
	{
		Attacker	= null;
		mCurRunTime = 0;
		mSkillID	= 0;
		mSkillLevel	= 0;
		mIsActive	= false;
		SkillTotalTime = 0.0f;
	}
	
	// 高16位为技能ID, 低16位是技能等级
	public uint SkillID	{
		get
		{
			return mSkillID;
		}
		set
		{
			if(value > 0)
			{
				mSkillID	= value >> 16;
				mSkillLevel	= value & 0xffff;
				m_SkillDef  = SkillManager.SP.GetSkillDefine((ushort)mSkillID);
				m_SkillLevel = m_SkillDef.Levels[(byte)mSkillLevel];
				
				if(m_SkillLevel == null)
				{
					Log.Write(LogLevel.ERROR,"skill Level = null");
				}
			}
		}
	}
	
	public uint SkillLevelID
	{
		get
		{
			return mSkillLevel;
		}
	}
	
	public void GetEffectPos(XU3dEffect effect,Vector3 rot)
	{		
		if(m_SkillLevel.useRegion == ESkillUseRegion.eSUseRegion_Total)
		{
			Vector3 pos = BattleDisplayerMgr.GetFighterPos(TargetBattlePos.Group,5);
			effect.Position		= pos;
			effect.Direction	= Quaternion.Euler(rot).eulerAngles;
			return ;
		}
		else if(m_SkillLevel.useRegion == ESkillUseRegion.eSUseRegion_Column)
		{
			int ColumnNum = (int)TargetBattlePos.Position / 3;
			Vector3 pos = BattleDisplayerMgr.GetFighterPos(TargetBattlePos.Group,ColumnNum * 3 + 2);
			effect.Position		= pos;
			effect.Direction	= Quaternion.Euler(rot).eulerAngles;
			return ;
			
		}
		else if(m_SkillLevel.useRegion == ESkillUseRegion.eSUseRegion_Row)
		{
//			int tempPos = 0;
//			int left = (int)TargetBattlePos.Position % 3;
//			if(left == 0)
//				tempPos = 6;
//			else
//				tempPos = left + 3;
//			
//			Vector3 pos = BattleDisplayerMgr.GetFighterPos(TargetBattlePos.Group,tempPos);
//			effect.Position		= pos;
//			effect.Direction	= Quaternion.Euler(rot).eulerAngles;
//			return ;
		}
	}
	
	public void AddDamageEvent(int DamageNo,SkillDisplay_Cast_Event.MakeDamageHandle handle)
	{
		//6.伤害事件
		SkillDisplay_Cast_Event castEvent 	= GetSkillDamage(DamageNo,true) as SkillDisplay_Cast_Event;	
		if(castEvent != null)
			castEvent.damageHandle			+= handle;
	}
	
	public void InitSkillDisplay()
	{
		if(m_SkillDef == null)
			return ;
		
		SkillTotalTime = m_SkillDef.SkillTime;
		
		//1.攻击特效
		SkillDisplay_Effect effectAttack = new SkillDisplay_Effect(this);
		effectAttack.DoTime		= m_SkillDef.AttackEffectDelay;
		effectAttack.BindID		= m_SkillDef.AttackEffectBind;
		effectAttack.EffectID	= (uint)m_SkillDef.AttackEffectID;
		effectAttack.mTarget	= Attacker;
		effectAttack.IsFollowBone	= m_SkillDef.IsFollowBone;
		if(m_SkillDef.AttackEffectLife > 0)
		{
			effectAttack.IsNeedForceOver	= true;
			effectAttack.TotalLifeTime		= m_SkillDef.AttackEffectLife;
		}
		mDisplayList.Add(effectAttack);	
		
		
		//2.子弹特效
		float bulletFlyTime = 0;
		if(m_SkillDef.UseWay == ESkillUseWay.eSUseWay_LongBullet)
		{
			SkillDisplay_Bullet effectBullet = new SkillDisplay_Bullet(this);
			effectBullet.DoTime			= m_SkillDef.TranslateDelay;
			effectBullet.SrcBindID		= m_SkillDef.BulletSrcBind;
			effectBullet.TargetBindID 	= m_SkillDef.BulletTgtBind;
			effectBullet.BulletID		= (uint)m_SkillDef.BulletID;
			effectBullet.BulletRate		= m_SkillDef.BulletRate;
			effectBullet.BulletVelocity	= m_SkillDef.BulletVelocity;
			effectBullet.BulletFlyTrack	= m_SkillDef.BulletFlyTrack;
			mDisplayList.Add(effectBullet);
			effectBullet.Init();
			bulletFlyTime				= effectBullet.BulletFlyTime;
		}
		
		//3.命中
		float HitTime = 0;
		if(m_SkillDef.UseWay == ESkillUseWay.eSUseWay_CloseCombat || m_SkillDef.UseWay == ESkillUseWay.eSUseWay_LongDirect)
		{
			float externTime = 0;
			if(m_SkillDef.UseObject == ESkillUseObject.eSUseObject_Region)
			{
				//区域特效
				SkillDisplay_Effect effectArea = new SkillDisplay_Effect(this);
				effectArea.DoTime		= m_SkillDef.TranslateDelay;  
				effectArea.BindID		= ESkeleton.eMainObject;
				effectArea.EffectID		= (uint)m_SkillDef.RegionEffectID;
				effectArea.mTarget		= MainTarget;
				effectArea.IsAreaEffect	= true;
				mDisplayList.Add(effectArea);
				externTime				= m_SkillDef.DefeatDelay;
			}
			
			//命中特效
			
			for(int m = 0; m < mTargetList.Count; m++)
			{
				SkillDisplay_Effect effectHit = new SkillDisplay_Effect(this);
				HitTime					= externTime + m_SkillDef.TranslateDelay;
				effectHit.DoTime		= HitTime;				
				effectHit.mTarget		= mTargetList[m];
				effectHit.BindID		= m_SkillDef.HitEffectBind;
				effectHit.EffectID		= (uint)m_SkillDef.HitEffectID;
				
				mDisplayList.Add(effectHit);
			}		
			
			//命中伤害
			if(IsMultiAttack)
			{
				float tempTime = 0;
				for(int j = 0; j < m_SkillLevel.AttackCount; j++)
				{
					tempTime	+= m_SkillLevel.AttackTime[j];
					SkillDisplay_Base damageEvent 	= GetSkillDamage(j+2,false);
					damageEvent.DoTime				= tempTime;					
				}
			}
			else
			{
				SkillDisplay_Base damageEvent 	= GetSkillDamage(1,true);
				damageEvent.DoTime				= HitTime;
			}
		}
		else if(m_SkillDef.UseWay == ESkillUseWay.eSUseWay_LongBullet)
		{
			for(int m = 0; m < mTargetList.Count; m++)
			{
				SkillDisplay_Effect effectBulletHit = new SkillDisplay_Effect(this);
				effectBulletHit.DoTime		= m_SkillDef.TranslateDelay + bulletFlyTime;
				effectBulletHit.BindID		= m_SkillDef.HitEffectBind;
				effectBulletHit.EffectID	= (uint)m_SkillDef.HitEffectID;
				effectBulletHit.mTarget		= mTargetList[m];
				mDisplayList.Add(effectBulletHit);
			}
			
						//命中伤害
			SkillDisplay_Base damageEvent 	= GetSkillDamage(1,true);
			damageEvent.DoTime				= m_SkillDef.TranslateDelay + bulletFlyTime;
		}
		
		//4.攻击震屏特效
		if(m_SkillDef.AttackShockID > 0)
		{
			SkillDisplay_Effect EffectAttackShake	= new SkillDisplay_Effect(this);
			EffectAttackShake.DoTime	= m_SkillDef.AttackShockDelay;
			EffectAttackShake.EffectID	= (uint)m_SkillDef.AttackShockID;
			mDisplayList.Add(EffectAttackShake);
		}
		
		//5.命中震屏特效
		if(m_SkillDef.HitShockID > 0)
		{
			SkillDisplay_Effect EffectHitShake	= new SkillDisplay_Effect(this);
			EffectHitShake.DoTime	= HitTime;
			EffectHitShake.EffectID	= (uint)m_SkillDef.HitShockID;
			mDisplayList.Add(EffectHitShake);
		}
	}
	
	public void InitBuffDisplayMulti(IList<PB_SameTimeBuffEffect> mul)
	{
		m_BuffDisplayMulti = new XBuffDisplayMulti(mul);
	}
	
	public void Start()
	{
		mIsActive	= true;
		mCurRunTime	= 0;
		if(m_SkillDef != null && Attacker != null)
		{			
			Attacker._playAnimation(m_SkillDef.AttackAnim,m_SkillDef.AttackAnimSpeed,false);
			Attacker._playAnimation(EAnimName.Fight,1.0f,true);
			
			
		}
	}
	
	public void Breath()
	{
		if(!mIsActive)
			return ;
		
		for(int i = 0; i < mDisplayList.Count; i++)
		{
			SkillDisplay_Base Display = mDisplayList[i];
			if(Display != null)
				Display.UpdateSkillDisplay(mCurRunTime,Time.deltaTime);
		}
		
		mCurRunTime += Time.deltaTime;
	}
	
	public bool IsEnd()
	{
		return mCurRunTime >= SkillTotalTime;
	}
	
	//设想允许出现多次伤害，参数是第几次伤害，通知逻辑上层
	private SkillDisplay_Base GetSkillDamage(int num,bool isCreate)
	{
		SkillDisplay_Cast_Event damage = null;
		if(mDamageList.Count < num)
		{
			if(isCreate)
			{
				damage		= new SkillDisplay_Cast_Event(this);
				damage.Num	= (uint)num;
				damage.IsNeedAnim	= true;
				
				
				mDamageList.Add(damage);
				mDisplayList.Add(damage);
			}
		}
		
		return mDamageList[num - 1];
	}
	
	public void ClearDamageList()
	{
		for(int i = 0; i < mDamageList.Count; i++)
		{
			SkillDisplay_Base effect = mDamageList[i];
			if(effect != null)
			{
				mDisplayList.Remove(effect);
				mDamageList.Remove(effect);				
			}
		}
	}
	
	public bool				IsLucky {get; set;}
	public bool				IsMultiAttack	= false;
	private XSkillDefine 	m_SkillDef = null;
	private XSkillLevelDefine m_SkillLevel = null;
	public  XCharacter 			Attacker 		{get; set;}
	public  XCharacter 			MainTarget		{get; set;}
	public  List<XCharacter>	mTargetList	= new List<XCharacter>();
	private float			mCurRunTime;
	private uint 			mSkillID;
	private uint 			mSkillLevel;
	private bool			mIsActive;
	public	float			SkillTotalTime {get;private set;}	
	public  XBattlePosition	TargetBattlePos	{get;set;}
	private List<SkillDisplay_Base>	mDisplayList = new List<SkillDisplay_Base>();
	private List<SkillDisplay_Base>	mDamageList = new List<SkillDisplay_Base>();
	public XBuffDisplayMulti m_BuffDisplayMulti = null;
}