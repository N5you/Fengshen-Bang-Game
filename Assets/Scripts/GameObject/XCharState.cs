using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public abstract class XCharStateBase : XState<XCharacter>
{
	public XCharStateBase(EStateId id, XCharacter owner)
		: base((int)id, owner)
	{
	}
	
	public abstract void OnAppear();
}


public class XCharStateIdle : XCharStateBase
{
	private bool bIsInProduct = false;
	private EAnimName curAnim = EAnimName.Idle;

	public XCharStateIdle(XCharacter owner)
		: base(EStateId.esIdle, owner)
	{
		
	}
	
	public override void Enter(params object[] args)
	{
		bool bIsPush = false;
		if(null != args && args.Length > 0 && null != args[0] && (args[0] is bool) && (bool)(args[0]))
			bIsPush = true;
		m_Owner._playAnimation(EAnimName.Idle, 1f, bIsPush);
	}

	public override bool OnEvent(int evt, params object[] args)
	{
		EStateEvent ESE = (EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esMoveTo):
			Machine.TranslateToState((int)EStateId.esMove, args);
			return true;
			
		case(EStateEvent.esRotateTo):
			Machine.RotateTo((int)EStateId.esMove, args);
			return true;
			
		case(EStateEvent.esJump):
			Machine.TranslateToState((int)EStateId.esMove, args);
			return true;
			
		case(EStateEvent.esSit):
			Machine.TranslateToState((int)EStateId.esSit);
			return true;
			
		case(EStateEvent.esStartFight):
			Machine.TranslateToState((int)EStateId.esFight);
			return true;
			
		case(EStateEvent.esStartProduct):
			{
				if(bIsInProduct) return false;
				curAnim = (EAnimName)(args[0]);
				m_Owner._playAnimation(curAnim, 1f, false); 
				bIsInProduct = true;
				return true;
			}
			
		case(EStateEvent.esQuitProduct):
			{
				if(!bIsInProduct) return false;
				curAnim = EAnimName.Idle;
				m_Owner._playAnimation(curAnim, 1f, false);
				bIsInProduct = false;
				return true;
			}
			
		case(EStateEvent.esLevelLoaded):
			{
				m_Owner.PushOnTerrain();
				return true;
			}
		}
		return false;
	}
	
	public override void OnAppear()
	{
		m_Owner._playAnimation(curAnim, 1f, false);
	}
	
	public override void Exit ()
	{
		bIsInProduct = false;
	}
}


public class XCharStateSit : XCharStateBase
{
	private XU3dEffect effect = null;
	private XU3dEffect effectMeditationLive = null;

	public XCharStateSit(XCharacter owner)
		: base(EStateId.esSit, owner)
	{
		
	}
	
	public override void Enter(params object[] args)
	{
		m_Owner._playAnimation(EAnimName.Sit, 1f, false);
		effect = new XU3dEffect(900420);
		effectMeditationLive = new XU3dEffect(900061);
		
		m_Owner.AttachGo(ESkeleton.eMainObject,effect.m_gameObject);
		m_Owner.AttachGo(ESkeleton.eCapsuleTop,effectMeditationLive.m_gameObject);
		m_Owner.OnEnterSit();
	}
	
	public override bool OnEvent(int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		
		switch(ESE)
		{
		case(EStateEvent.esMoveTo):
			Machine.TranslateToState((int)EStateId.esMove, args);
			return true;
			
		case(EStateEvent.esJump):
			Machine.TranslateToState((int)EStateId.esMove, args);
			return true;
			
		case(EStateEvent.esCancelSit):
			Machine.TranslateToState((int)EStateId.esIdle);
			return true;
		case(EStateEvent.esLevelLoaded):
			m_Owner.PushOnTerrain();
			return true;
		}
		return false;
	}
	
	public override void OnAppear()
	{
		m_Owner._playAnimation(EAnimName.Sit, 1f, false);
	}

	public override void Exit ()
	{
		destroyEffect();
		m_Owner.OnExitSit();
	}

	private void destroyEffect()
	{
		if(null != effect)
		{
			effect.Destroy();
			effect = null;
		}
		if(null !=  effectMeditationLive)
		{
			effectMeditationLive.Destroy();
			effectMeditationLive = null;
			
		}
	}
}


public class XCharStateDead : XCharStateBase
{
	public XCharStateDead(XCharacter owner)
		: base(EStateId.esDead, owner)
	{
	}
	
	public override void Enter (params object[] args)
	{
		m_Owner._playAnimation(EAnimName.Death, 1.0f, false);
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esHp):
			if((int)(args[0]) > 0)
				Machine.TranslateToState((int)EStateId.esFight);
			return true;
		}
		return false;
	}
	
	public override void OnAppear()
	{
		m_Owner._playAnimation(EAnimName.Death, 1.0f, false);
	}
}


public class XCharStateFight : XCharStateBase
{
	public XCharStateFight(XCharacter owner)
		: base(EStateId.esFight, owner)
	{
		
	}
	
	public override void Enter (params object[] args)
	{
		m_Owner._playAnimation(EAnimName.Fight, 1.0f, false);
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esHp):
			if((int)(args[0]) <= 0)
				Machine.TranslateToState((int)EStateId.esDead);
			return true;
			
		case(EStateEvent.esQuitFight):
			Machine.TranslateToState((int)EStateId.esIdle);
			return true;
		case EStateEvent.esMoveTo:
			Machine.TranslateToState((int)EStateId.esMove,args);
			return true;
		}
		return false;
	}
	
	public override void OnAppear()
	{
		m_Owner._playAnimation(EAnimName.Fight, 1.0f, false);
	}
}


public class XCharStateMove : XCharStateBase
{
	// 跳跃
	private static readonly float JUMP_INITIAL_VELOCITY = 4.8f; // 起跳初速度单位米/s
	public delegate void OnJumpDone();
	private OnJumpDone onJumpDone = null;
	private float m_JumpX = 0.0f;		// 起跳点x坐标
	private float m_JumpZ = 0.0f;		// 起跳点z坐标
	private float m_JumpHeight = 0.0f;	// 起跳点的高度
	private float m_JumpTime = 0.0f;	// 距离起跳的时间
	private bool m_IsInJump = false;	// 是否在跳跃状态(在跳跃状态不一定执行跳跃逻辑)
	private bool m_IsJump = false;		// 是否在跳跃过程中(是否正在执行跳跃逻辑)
	private bool m_IsWaitJump = false;	// 是否在等待执行跳跃逻辑(逻辑地图未生成之前不可执行跳跃逻辑)
	private bool m_IsDrop = false;		// 是否在降落过程中
	
	// XZ移动
	public delegate void OnMoveDone();
	private OnMoveDone onMoveDone = null;		// 水平移动完成以后回调
	private bool bIsInMove = false;				// 是否在水平移动状态(在水平移动状态不一定执行移动逻辑)
	private bool bIsMoving = false;				// 标记是否在水平移动(是否正在执行水平移动逻辑)
	private EProtoMoveType moveType;			// 移动类型: 线段移动/滑行/原地踏步
	private Vector3 tgtPos = Vector3.zero;		// 水平移动目标点
	private float mSpeed;						// 真实的运动速度(可以和Owner的Speed不一样, 在滑行时会衰减)
	
	// 转向
	private static readonly float ROTATE_SPEED_PER_SECOND = 540.0f;	// 转向速度
	private float yAngleDur = 0.0f;				// 每次移动的总转向
	private bool bIsRotating = false;			// 标记是否在转向
	
	private bool m_IsIdlePush = false;			// 切换到Idle状态时, Idle动画是否Push切换
	
	private EAnimName	mMoveAnim	= EAnimName.Run;
	
	public XCharStateMove(XCharacter owner)
		: base(EStateId.esMove, owner)
	{
		
	}
	
	private void initRotateInfo(float tgtY)
	{
		bIsRotating = false;
		yAngleDur = (tgtY - m_Owner.Direction.y) % 360.0f;
		if(yAngleDur > 180.0f) yAngleDur -= 360.0f;
		else if(yAngleDur < -180.0f) yAngleDur += 360.0f;
		if(0.0f != yAngleDur) bIsRotating = true;		
	}
	
	public void rotateTo(params object[] args)
	{
		tgtPos = (Vector3)(args[0]);
		Vector3 tmpPos = tgtPos; 
		tmpPos.y = m_Owner.Position.y;
		Vector3 tgtDir = Quaternion.LookRotation(tmpPos * 10000f - m_Owner.Position * 10000f).eulerAngles;

		
		float yDur = (tgtDir.y - m_Owner.Direction.y) % 360.0f;
		if(yDur > 180.0f) 
			yDur -= 360.0f;
		else if(yDur < -180.0f) 
			yDur += 360.0f;
		if(0.0f == yDur)
			return;
		
		m_Owner.Direction += new Vector3(0f, yDur, 0f);
		
		CS_StartMove.Builder builder = CS_StartMove.CreateBuilder();
       	builder.SetFromX(m_Owner.Position.x);
        builder.SetFromZ(m_Owner.Position.z);
		builder.SetMoveType((int)EProtoMoveType.eMoveType_Slide);
       	builder.SetToX(m_Owner.Position.x);
       	builder.SetToZ(m_Owner.Position.z);
       	builder.SetDir(m_Owner.Direction.y);
       	XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StartMove, builder.Build());
		
		CS_StopMove.Builder builderStop = CS_StopMove.CreateBuilder();
		builderStop.SetPosX(m_Owner.Position.x);
		builderStop.SetPosZ(m_Owner.Position.z);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StopMove, builderStop.Build());
	}
	
	private bool MoveTo(bool isEnter, params object[] args)
	{
		m_IsIdlePush = false;
		onMoveDone = null;		
		bIsMoving = false;
		float speed = m_Owner.Speed;
		moveType = (EProtoMoveType)(args[0]);
		switch(moveType)
		{
		case(EProtoMoveType.eMoveType_Segment):
		{
			tgtPos = (Vector3)(args[1]);
			speed = mSpeed = (float)(args[2]);
			onMoveDone = (OnMoveDone)(args[3]);
			mMoveAnim	= (EAnimName)(args[4]);
			Vector3 tmpPos = tgtPos; tmpPos.y = m_Owner.Position.y;
			Vector3 tgtDir = Quaternion.LookRotation(tmpPos * 10000f - m_Owner.Position * 10000f).eulerAngles;
			initRotateInfo(tgtDir.y);
			bIsMoving = true;
		}	
			break;
			
		case(EProtoMoveType.eMoveType_Slide):
		{
			tgtPos = (Vector3)(args[1]);
			float dir = (float)(args[2]);
			initRotateInfo(dir);
			speed = (float)(args[3]);
			onMoveDone = (OnMoveDone)(args[4]);
			// 根据角色朝向和移动方向计算移动速度(会衰减)
			Vector3 tmpPos = tgtPos; tmpPos.y = m_Owner.Position.y;
			Vector3 tgtDir = Quaternion.LookRotation(tmpPos * 10000f - m_Owner.Position * 10000f).eulerAngles;
			float dy = Mathf.Abs(tgtDir.y - dir) % 360f;
			if(dy > 180f) dy = 360f - dy;
			mSpeed = speed * Mathf.Cos(dy / 180f * Mathf.PI);
			bIsMoving = true;
		}	
			break;
			
		case(EProtoMoveType.eMoveType_Step):
		{
			speed = mSpeed = (float)(args[1]);
			initRotateInfo((float)(args[2]));
			bIsMoving = true;
		}
			break;
			
		default:
			break;
		}
		if(bIsMoving && isEnter)
		{
			m_Owner._playAnimation(mMoveAnim, speed / XCharacter.DEFAULT_RUN_SPEED, false);
		}
		bIsInMove = true;
		return true;
	}
	
	private bool Jump(params object[] args)
	{
		if(m_IsInJump) return false;
		m_JumpX = (float)(args[0]);
		m_JumpZ = (float)(args[1]);
		m_JumpTime = (float)(args[2]);
		onJumpDone = (OnJumpDone)(args[3]);
		m_IsInJump = m_IsJump = true;
		
		if(XLogicWorld.SP.SceneManager.IsLoading)
		{
			m_IsWaitJump = true;
		}
		else
		{
			m_IsWaitJump = false;
			StartJump();
		}
		return true;
	}
	
	private bool StartJump()
	{
		m_IsIdlePush = false;
		m_IsJump = false;
		m_JumpHeight = XLogicWorld.SP.SceneManager.GetExactHeightAt(m_JumpX, m_JumpZ);
		if(m_JumpTime > 0f)
		{
			float expectHeight = m_JumpHeight + JUMP_INITIAL_VELOCITY * m_JumpTime + Define.GRAVITATIONAL_ACCELERATION * m_JumpTime * m_JumpTime / 2f;
			float nowHeight = XLogicWorld.SP.SceneManager.GetExactHeightAt(m_Owner.Position.x, m_Owner.Position.z);
			if(expectHeight > nowHeight)
			{
				m_IsJump = true;
				m_IsDrop = (JUMP_INITIAL_VELOCITY + m_JumpTime * Define.GRAVITATIONAL_ACCELERATION) <= 0f;
				Vector3 posNow = m_Owner.Position;
				posNow.y = expectHeight;
				m_Owner.Position = posNow;
				m_Owner._playAnimation(EAnimName.Drop, 1.0f, false);
			}
		}
		else
		{
			m_IsJump = true;
			m_IsDrop = false;
			m_Owner._playAnimation(EAnimName.Jump, 1.0f, false);
			m_Owner._playAnimation(EAnimName.Drop, 1.0f, true);
		}
		return m_IsJump;
	}
	
	public override void Enter (params object[] args)
	{
		if(args[0] is EProtoMoveType)
		{
			MoveTo(true, args);
		}
		else
		{
			Jump(args);
		}
	}
	
	public override void Breathe (float deltaTime)
	{
		if(!m_IsJump && !bIsMoving)
		{
			return;
		}
		
		// 转向
		if(bIsRotating)
		{
			float ab = (yAngleDur > 0f) ? 1f : -1f;
			float yAngle = ROTATE_SPEED_PER_SECOND * deltaTime * ab;
			if(Mathf.Abs(yAngleDur) < Mathf.Abs(yAngle))
			{
				yAngle = yAngleDur;
				bIsRotating = false;
			}
			yAngleDur -= yAngle;
			m_Owner.Direction += new Vector3(0f, yAngle, 0f);
		}
		
		Vector3 motion = Vector3.zero;
		bool bIsMoveOverAtThisFrame = false;
		if(bIsMoving)
		{
			switch(moveType)
			{
			case(EProtoMoveType.eMoveType_Segment):
			case(EProtoMoveType.eMoveType_Slide):
			{
				float fDis = mSpeed * deltaTime;				// 本次移动的长度
				motion = tgtPos - m_Owner.Position;				// 移动方向
				motion.y = 0f;		// 只在xz平面移动
			
				if(fDis >= motion.sqrMagnitude)
				{
					bIsMoving = false;
					bIsMoveOverAtThisFrame = true;
				}
				else
				{
					motion = motion.normalized * fDis;
				}
			}
				break;
				
			case(EProtoMoveType.eMoveType_Step):
				break;
			}
		}
		
		bool bIsJumpOverAtThisFrame = false;
		Vector3 nextPos = m_Owner.Position + motion;
		bool bByJump = false;		// 标记是否被Jump流程处理过, 如果没有, 需要将坐标点拉到地表
		if(m_IsJump)
		{
			m_JumpTime += deltaTime;
			if(!m_IsWaitJump)
			{
				bByJump = true;
				if(!m_IsDrop) 
					m_IsDrop = (JUMP_INITIAL_VELOCITY + m_JumpTime * Define.GRAVITATIONAL_ACCELERATION) <= 0f;
				float expectHeight = m_JumpHeight + JUMP_INITIAL_VELOCITY * m_JumpTime + Define.GRAVITATIONAL_ACCELERATION * m_JumpTime * m_JumpTime / 2f;
				motion.y = expectHeight - nextPos.y;
				float nowHeight = XLogicWorld.SP.SceneManager.GetExactHeightAt(nextPos.x, nextPos.z);
				// 预测是否会着地
				if(expectHeight <= nowHeight)
				{
					motion.y = nowHeight - nextPos.y;
					m_IsJump = false;
					bIsJumpOverAtThisFrame = true;
					if(bIsMoving)
					{
						if(m_IsDrop)
						{
							m_Owner._playAnimation(EAnimName.DropRun, 1.0f, false);
							m_Owner._playAnimation(EAnimName.Run, m_Owner.Speed / XCharacter.DEFAULT_RUN_SPEED / m_Owner.Scale, true);
						}
						else
						{
							m_Owner._playAnimation(EAnimName.Run, m_Owner.Speed / XCharacter.DEFAULT_RUN_SPEED / m_Owner.Scale, false);
						}
					}
					else
					{
						if(m_IsDrop) 
						{
							m_Owner._playAnimation(EAnimName.DropStand, 1.0f, false);
							m_Owner._playAnimation(EAnimName.Idle, 1.0f, true);
							m_IsIdlePush = true;
						}
						else
						{
							m_Owner._playAnimation(EAnimName.Idle, 1.0f, false);
							m_IsIdlePush = true;
						}
					}
				}
			}
		}
		
		// 根据以上计算出来的motion, 结合地形高度, 设置坐标
		nextPos = m_Owner.Position + motion;
		if(!bByJump) XLogicWorld.SP.SceneManager.FixExactHeight(ref nextPos);
		m_Owner.Position = nextPos;
		
		if(bIsMoveOverAtThisFrame)
		{
			if(null != onMoveDone) 
			{
				OnMoveDone tmpCB = onMoveDone;
				onMoveDone = null;
				tmpCB(); // 有可能会执行下一个运动的命令
			}
		}
		
		if(bIsJumpOverAtThisFrame)
		{
			if(null != onJumpDone)
			{
				OnJumpDone tmpCB = onJumpDone;
				onJumpDone = null;
				tmpCB();
			}
		}
		
		// 移动状态不能主动退出, 等退出消息, 防止出现角色状态误差
		/*if(!bIsMoving && !m_IsJump)
		{
			Machine.TranslateToState((int)EStateId.esIdle, m_IsIdlePush);
		}*/
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esMoveTo):
			MoveTo(false, args);
			return true;
			
		case(EStateEvent.esStopMove):
			bIsMoving = bIsInMove = false;
			onMoveDone = null;
			if(!m_IsInJump) 
				Machine.TranslateToState((int)EStateId.esIdle, m_IsIdlePush);
			return true;
			
		case(EStateEvent.esSpeed):
			if(bIsMoving)
			{
				float nowSpeed = (float)(args[0]);
				float oldSpeed = (float)(args[1]);
				mSpeed *= nowSpeed / oldSpeed;			
			}
			return true;
			
		case(EStateEvent.esJump):
			return Jump(args);
			
		case(EStateEvent.esJumpOver):
			m_IsInJump = false;
			if(m_IsJump && !m_IsWaitJump) // 拉到地表
			{
				m_IsJump = false;
				m_Owner.PushOnTerrain();
			}
			if(!bIsInMove)
				Machine.TranslateToState((int)EStateId.esIdle, m_IsIdlePush);
			return true;
			
		case(EStateEvent.esStartFight):
			Machine.TranslateToState((int)EStateId.esFight);
			return true;
			
		case(EStateEvent.esLevelLoaded):
			if(!m_IsInJump) // 拉到地面
			{
				m_Owner.PushOnTerrain();
			}			
			if(m_IsInJump && m_IsJump && m_IsWaitJump)
			{
				m_IsWaitJump = false;
				StartJump();
			}
			return true;
		}
		return false;
	}
	
	public override void Exit ()
	{
		if(m_IsJump)
		{
			m_Owner.PushOnTerrain();
		}
		m_IsJump = false;
		bIsMoving = false;
		onMoveDone = null;
	}
	
	public override void OnAppear()
	{
		if(m_IsJump)
		{
			m_Owner._playAnimation(EAnimName.Drop, 1.0f, false);
		}
		else if(bIsMoving)
		{
			m_Owner._playAnimation(EAnimName.Run, m_Owner.Speed / XCharacter.DEFAULT_RUN_SPEED / m_Owner.Scale, false);
		}
		else
		{
			m_Owner._playAnimation(EAnimName.Idle, 1.0f, false);
		}
	}
}

