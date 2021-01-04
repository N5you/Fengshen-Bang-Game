using System.Collections.Generic;
using UnityEngine;

public enum SkillDisplay_Event
{
	SkillDisplay_Event_None,
	SkillDisplay_Event_Effect,
	SkillDisplay_Event_Bullet,
	SkillDisplay_Event_Sound,
	SkillDisplay_Event_ID,
	SkillDisplay_Event_Shake,
	SkillDisplay_Event_Num,
	
}
public class SkillDisplay_Base 
{	
	public SkillDisplay_Base(XSkill skill)
	{
		CurSkill	= skill;
		SelfType	= SkillDisplay_Event.SkillDisplay_Event_None;
	}
	
	public SkillDisplay_Event SelfType {get; protected set;}

	public float	DoTime {get;set;}
	public XSkill 	CurSkill {get; set;}
	protected bool 	IsPlayed = false;
	
	
	public virtual void Init() {}
	public virtual bool CreateSkillDisplay() { return false; }
	public virtual void DestroySkillDisplay() {}
	public virtual void UpdateSkillDisplay(float runTime,float deltaTime)
	{
		if(IsPlayed)
			return ;
		
		float nowTime = runTime + deltaTime;
		if(runTime <= DoTime && nowTime > DoTime)
			CreateSkillDisplay();
		
	}
}

public class SkillDisplay_Effect : SkillDisplay_Base
{
	public SkillDisplay_Effect(XSkill skill) : base(skill)
	{
		EffectID		= 0;
		TotalLifeTime	= 0;
		IsNeedForceOver	= false;
		IsFollowBone	= 0;
		IsAreaEffect	= false;
		mTarget			= null;
		SelfType		= SkillDisplay_Event.SkillDisplay_Event_Effect;
	}
	
	public override bool CreateSkillDisplay()
	{
		if(EffectID == 0)
			return false;
		
		Vector3 selfRot;
		Transform attachPos;
		
		mEffect = new XU3dEffect(EffectID);
		
		if(mTarget == null)
			return false;
		
		attachPos 	= mTarget.GetSkeleton(BindID);
		selfRot		= mTarget.Direction;	
		
		if(IsFollowBone > 0)
		{
			mEffect.Parent 	= attachPos;
		}
		else
		{
			mEffect.Position 	= attachPos.position;
			mEffect.Direction	= Quaternion.Euler(selfRot).eulerAngles;
		}	
		
		if(IsAreaEffect)
			CurSkill.GetEffectPos(mEffect,selfRot);
		
		if(IsNeedForceOver)
			mTimeCalc.BeginTimeCalc(TotalLifeTime,true);

		return true;
	}
	
	public override void UpdateSkillDisplay(float runTime,float deltaTime)
	{		
		float nowTime = runTime + deltaTime;
		if(runTime <= DoTime && nowTime > DoTime)
			CreateSkillDisplay();

		if(IsNeedForceOver && mEffect != null)
		{
			if(mTimeCalc.CountTime(deltaTime))
				mEffect.Destroy();
		}
	}
	
	public override void DestroySkillDisplay()
	{
		
	}
	
	public uint EffectID 		{get;set;}
	public uint IsFollowBone	{get;set;}
	
	public ESkeleton	BindID;	
	public bool			IsAreaEffect;
	private XU3dEffect 	mEffect;
	public XCharacter	mTarget;

	public bool			IsNeedForceOver {get; set;}
	public float 		TotalLifeTime {get; set;}
	private	TimeCalc	mTimeCalc	= new TimeCalc();
	
}

public class SkillDisplay_Bullet: SkillDisplay_Base
{
	public SkillDisplay_Bullet(XSkill skill) : base(skill)
	{
		SelfType	= SkillDisplay_Event.SkillDisplay_Event_Bullet;
	}
	
	public override void Init()
	{
		if(CurSkill.Attacker == null || CurSkill.MainTarget == null)
			return ;
		
		Transform srcTran = CurSkill.Attacker.GetSkeleton(SrcBindID);
		Transform tgtTran = CurSkill.MainTarget.GetSkeleton(TargetBindID);

		if(null == srcTran || null == tgtTran)
			return ;
		
		float dist = Vector3.Distance(tgtTran.position, srcTran.position);
		if(BulletVelocity > 0)
		{
			BulletFlyTime	= dist / BulletVelocity;
		}
	}
	
	public override bool CreateSkillDisplay()
	{	
		Transform srcTran = CurSkill.Attacker.GetSkeleton(SrcBindID);
		Transform tgtTran = CurSkill.MainTarget.GetSkeleton(TargetBindID);
		if(null == srcTran || null == tgtTran)
			return false;
		
		mEffect = new XU3dEffect(BulletID);
		mEffect.Position 	= srcTran.position;
		//mEffect.Direction	= srcTran.rotation.eulerAngles;
		mEffect.Scale		*= BulletRate;		
		mEffect.FlyFromTo(srcTran,tgtTran,BulletVelocity,BulletFlyTrack);
		
	
		
		IsPlayed			= true;
		return true;
	}
	
	private void Defeat()
	{
		
	}
	public uint		BulletID {get;set;}
	public ESkeleton SrcBindID {get;set;}
	public ESkeleton TargetBindID {get;set;} 
	public float BulletRate {get; set;}
	public float BulletVelocity {get;set;}
	public float BulletFlyTrack {get; set;}
	public float BulletFlyTime {get; private set;}
	private XU3dEffect mEffect;
}

public class SkillDisplay_Shake: SkillDisplay_Base
{
	public SkillDisplay_Shake(XSkill skill):base(skill)
	{
		CurSkill	= skill;
		SelfType	= SkillDisplay_Event.SkillDisplay_Event_Shake;
	}
	
	public override bool CreateSkillDisplay()
	{		
		IsPlayed			= true;
		return true;
	}
	
	public int EffectID {get;set;}	
}

public class SkillDisplay_Cast_Event : SkillDisplay_Base
{	
	public delegate void MakeDamageHandle(int num,bool isNeedAnim);
	public event MakeDamageHandle damageHandle;
	
	public SkillDisplay_Cast_Event(XSkill skill):base(skill)
	{
		CurSkill	= skill;
		SelfType	= SkillDisplay_Event.SkillDisplay_Event_ID;
	}
	
	public override bool CreateSkillDisplay()
	{		
		if(damageHandle != null)
			damageHandle((int)Num,IsNeedAnim);
		
		IsPlayed			= true;
		return true;
	}
	
	public uint Num {get;set;}
	public bool IsNeedAnim {get;set;}
	
}