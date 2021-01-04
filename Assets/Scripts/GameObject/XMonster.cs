using UnityEngine;
using System.Collections;
using Google.ProtocolBuffers;
using XGame.Client.Packets;

public class XMonster : XCharacter
{
	// 需要改成根据配置来配置    
    private static readonly float MONSTER_SEE_DISTANCE = 8.0f;
    private static readonly float MONSTER_ATTACK_DISTANCE = 2.0f;
	private static readonly uint  MonsterSelectEffect = 900023;
	private bool m_bBeAttacker;
	private XMonsterAppearInfo mAppearInfo;	
	private bool m_IsSendAttackMsg = false;
	//6秒随机移动一次
	private float mRandomMoveDeltaTime = 5.0f;
	private uint RandDist = 4;
	
	private uint AngaryEffectID = 900011;
	
	private XCfgMonsterGroup 	mCfgGroup;
	private XCfgMonsterBase		mCfgBase;
	private Vector3 	mOrignPos;
	private TimeCalc	mTimer = new TimeCalc();

	public XMonster(ulong id) : base(id)
	{
		ObjectType = EObjectType.Monster;
		mCfgGroup	= null;
		mCfgBase	= null;
		
		if(mAppearInfo != null)
		{
			mCfgBase	= XCfgMonsterBaseMgr.SP.GetConfig(mAppearInfo.MonsterBaseID);
			mCfgGroup	= XCfgMonsterGroupMgr.SP.GetConfig(mAppearInfo.MonsterGroupID);
			
			if(mCfgGroup != null)
				Speed = mCfgGroup.MoveSpeed;
		}
			
		
		m_bBeAttacker = false;
	}
	
	public override EHeadBoard_Type GetHeadBoardType() { return EHeadBoard_Type.EHeadBoard_Type_Monster; }

    public override void SetAppearData(object data)
	{
        mAppearInfo = data as XMonsterAppearInfo;
        if (null == mAppearInfo) return;
		Direction 	= mAppearInfo.Direction;
		Position 	= mAppearInfo.Position;	
		mOrignPos	= Position;
		if(mAppearInfo.MonsterBaseID > 0)
		{
			mCfgBase = XCfgMonsterBaseMgr.SP.GetConfig(mAppearInfo.MonsterBaseID);
			if(mCfgBase == null)
				return ;
			Name = mCfgBase.Name;
	        	Title = mCfgBase.Title;
			if(mCfgBase.Title == "0" || mCfgBase.Title == "")
			{
				NickName = "";
			}else
			{
				NickName = mCfgBase.Title;
			}
			SetModel(EModelCtrlType.eModelCtrl_Original, mCfgBase.ModelId);
			Level = (int)mCfgBase.Level;
	        Hp = MaxHp = mCfgBase.MaxHp;
			Scale = mCfgBase.Zoom;
		}
		else if(mAppearInfo.MonsterGroupID > 0)
		{
			mCfgGroup = XCfgMonsterGroupMgr.SP.GetConfig(mAppearInfo.MonsterGroupID);
			if(mCfgGroup == null)
				return ;
			Name 	= mCfgGroup.Name;
			Title 	= mCfgGroup.Title;
			if(mCfgGroup.Title == "0" || mCfgGroup.Title == "")
			{
				NickName = "";
			}else
			{
				NickName = mCfgGroup.Title;
			}
			ModelId = mCfgGroup.ModelId;
			Speed 	= mCfgGroup.MoveSpeed;
			Scale = mCfgGroup.Zoom;
		}
	}
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		m_ObjectModel.AddBehaviourListener(EBehaviourType.e_BehaviourType_Monster,this);
	}
	
	private void RandomMove()
	{
		if(!m_bBeAttacker)
		{
			if(!mTimer.IsStart())
			{
				mTimer.BeginTimeCalc(mRandomMoveDeltaTime,false);
				RealRandomMove();
				
				
			}
			else
			{
				if(mTimer.CountTime(Time.deltaTime))
				{
					RealRandomMove();
				}
			}
		}
		else
		{
			
		}
	}
	
	private void RealRandomMove()
	{
		//random Target
		Vector2 randomPos = Random.insideUnitCircle * RandDist;
		
		
		Vector3 targetPos = new Vector3(mOrignPos.x + randomPos.x,0,mOrignPos.z + randomPos.y);
		SegmentMoveTo(targetPos, Speed, OnMoveDone,EAnimName.Walk);
	}
	
	public  void OnMoveDone()
	{
		StopMove();
	}

    public override void Breathe()
    {		
        base.Breathe();
		
		RandomMove();
		
		if(m_bBeAttacker)
			SegmentMoveTo(XLogicWorld.SP.MainPlayer.Position, Speed, null,EAnimName.Run);
		
		

		float dist = XUtil.CalcDistanceXZ(Position, XLogicWorld.SP.MainPlayer.Position);
		float CanSeeDist = MONSTER_SEE_DISTANCE;
		if(mCfgGroup != null)
			CanSeeDist	= mCfgGroup.SeeRadius;

       	if (dist <= CanSeeDist)
        {
			if(!m_bBeAttacker)
			{
				XU3dEffect AngaryEffect = new XU3dEffect(AngaryEffectID);
				AttachGo(ESkeleton.eCapsuleTop,AngaryEffect.m_gameObject);
			}
			
            m_bBeAttacker = true;
			if(mCfgGroup != null)
				Speed = mCfgGroup.RunSpeed;
			
        }
		
		float attackDist = MONSTER_ATTACK_DISTANCE;
		if(mCfgGroup != null)
			attackDist = mCfgGroup.AttackRadius;
		if(dist <= attackDist && !m_IsSendAttackMsg)
		{			
			m_IsSendAttackMsg	= true;
			XLogicWorld.SP.SubSceneManager.EnterFightScene();
		}
		
		if(m_bBeAttacker)
		{
			float Far = XUtil.CalcDistanceXZ(Position,mOrignPos);			
			if(Far >= mCfgGroup.FollowRadius)
			{
				StopMove();
				XU3dEffect effect = new XU3dEffect(XMainPlayerStatePreEnterScene.TransEffect);
				effect.Position	= Position;
				
				Position		= mOrignPos;
				if(mCfgGroup != null)
					Speed			= mCfgGroup.MoveSpeed;
				m_bBeAttacker	= false;
				
			}
		}
    }
	
	public override void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{		
		SendModelEvent(EModelEvent.evtSelect,MonsterSelectEffect,true);
	}
}

