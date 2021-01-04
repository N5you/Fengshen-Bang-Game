using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

// 包括SegmentMove/Slidemove/StepMove
class XMainPlayerLineMove : XSingleton<XMainPlayerLineMove>
{
	public delegate void OnLineMoveDone();
	private XMainPlayer mainPlayer = null;
	private EProtoMoveType moveType;
	private bool bIsMoving = false;
	private OnLineMoveDone onLineMoveDone = null;
	private float fBreatheRockon = 0f;
	
	public XMainPlayerLineMove()
	{
		mainPlayer = XLogicWorld.SP.MainPlayer;
	}
	
	public void Breathe()
	{
		float fNow = Time.time;
		if(!bIsMoving || fNow - fBreatheRockon < 0.3f)
			return;
		fBreatheRockon = fNow;
		switch(moveType)
		{
		case(EProtoMoveType.eMoveType_Segment):
		case(EProtoMoveType.eMoveType_Slide):
		{
			// 向服务器报告当前坐标, 用于校准
			CS_SetPosition.Builder builder = CS_SetPosition.CreateBuilder();
			builder.SetPosX(mainPlayer.Position.x);
			builder.SetPosZ(mainPlayer.Position.z);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SetPosition, builder.Build());
		}
			break;
		}
	}
	
	// moveType: segment
	public bool SegmentMoveTo(Vector3 toPos, float speed, OnLineMoveDone cb,EAnimName anim)
	{
		bIsMoving = false;
		onLineMoveDone = cb;
		moveType = EProtoMoveType.eMoveType_Segment;
		if(mainPlayer.SegmentMoveTo(toPos, speed, onMoveDone,anim))
		{
			Vector3 fromPos = mainPlayer.Position;
			CS_StartMove.Builder builder = CS_StartMove.CreateBuilder();
        	builder.SetFromX(fromPos.x);
        	builder.SetFromZ(fromPos.z);
			builder.SetMoveType((int)moveType);
        	builder.SetToX(toPos.x);
        	builder.SetToZ(toPos.z);
        	XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StartMove, builder.Build());
			bIsMoving = true;
		}
		return bIsMoving;
	}
	
	// moveType: slide
	public bool SlideMoveTo(Vector3 toPos, float dir, float speed, OnLineMoveDone cb)
	{
		bIsMoving = false;
		onLineMoveDone = cb;
		moveType = EProtoMoveType.eMoveType_Slide;
		if(mainPlayer.SlideMoveTo(toPos, dir, speed, onMoveDone))
		{
			Vector3 fromPos = mainPlayer.Position;
			CS_StartMove.Builder builder = CS_StartMove.CreateBuilder();
        	builder.SetFromX(fromPos.x);
        	builder.SetFromZ(fromPos.z);
			builder.SetMoveType((int)moveType);
        	builder.SetToX(toPos.x);
        	builder.SetToZ(toPos.z);
        	builder.SetDir(dir);
        	XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StartMove, builder.Build());			
			bIsMoving = true;
		}
		return bIsMoving;
	}
	
	// stepMove
	public bool StepMove(float speed, float dir)
	{
		bIsMoving = false;
		onLineMoveDone = null;
		moveType = EProtoMoveType.eMoveType_Step;
		if(mainPlayer.StepMove(speed, dir))
		{
			Vector3 fromPos = mainPlayer.Position;
			CS_StartMove.Builder builder = CS_StartMove.CreateBuilder();
        	builder.SetFromX(fromPos.x);
        	builder.SetFromZ(fromPos.z);
        	builder.SetMoveType((int)moveType);
			builder.SetDir(dir);
        	XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StartMove, builder.Build());
			bIsMoving = true;
		}
		return bIsMoving;
	}
	
	private void onMoveDone()
	{
		if(!bIsMoving) return;
		
		bIsMoving = false;
		if(null != onLineMoveDone)
		{
			OnLineMoveDone tmpCB = onLineMoveDone;
			onLineMoveDone = null;
			tmpCB();	// 有可能会开启下一个直线运动
		}
	}

	// 客户端退出移动状态(bTellServer: 是否告诉服务器停止移动的消息, 如果服务器命令客户端停止移动, 则不需要发送)
	public void Stop(bool bTellServer)
	{
		bIsMoving = false;
		onLineMoveDone = null;
		mainPlayer.StopMove();
		if(bTellServer)
		{
			CS_StopMove.Builder builder = CS_StopMove.CreateBuilder();
			builder.SetPosX(mainPlayer.Position.x);
			builder.SetPosZ(mainPlayer.Position.z);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_StopMove, builder.Build());
		}
	}
}

// 主角自动寻路
class XMainPlayerAutoMove : XSingleton<XMainPlayerAutoMove>
{
	public delegate void OnAutoMoveDone();
	
	private XMainPlayer mainPlayer = null;
	private ArrayList posList = null;
	private XMainPlayerLineMove lineMove = null;
	private OnAutoMoveDone onAutoMoveDone = null;
	private bool isMoving = false;

	public XMainPlayerAutoMove()
	{
		mainPlayer = XLogicWorld.SP.MainPlayer;
		lineMove = XMainPlayerLineMove.SP;
		
		//XEventManager.SP.AddHandler(OnReceiveMission,EEvent.Mission_ReceiveMission );
	}
	
	public bool AutoTo(Vector3 toPos, OnAutoMoveDone cb)
	{
		Vector3 endPos = Vector3.zero;
		return AutoTo(toPos, cb, out endPos);
	}
	
	public bool AutoTo(Vector3 toPos, OnAutoMoveDone cb, out Vector3 endPos)
	{
		onAutoMoveDone = null;
		posList = XLogicWorld.SP.SceneManager.FindPath(mainPlayer.Position, toPos);
		if(null != posList && posList.Count >= 1)
		{
			endPos = (Vector3)(posList[posList.Count-1]);
			XLogicWorld.SP.SceneManager.FixTerrainHeight(ref endPos);
		}
		else
		{
			endPos = Vector3.zero;
		}
		if(nextLineMove())
		{
			onAutoMoveDone = cb;
			isMoving = true;
		}
		else if(isMoving)
		{
			isMoving = false;
			lineMove.Stop(true);
		}
		return isMoving;
	}
	
	private bool nextLineMove()
	{
		if(null == posList || posList.Count < 1)
			return false;
		
		Vector3 pos = (Vector3)(posList[0]);
		posList.RemoveAt(0);
		return lineMove.SegmentMoveTo(pos, mainPlayer.Speed, onLineMoveDone,EAnimName.Run);
	}
	
	private void onLineMoveDone()
	{
		if(isMoving && !nextLineMove())
		{
			isMoving = false;
			lineMove.Stop(true);
			if(null != onAutoMoveDone)
			{
				OnAutoMoveDone tmpCB = onAutoMoveDone;
				onAutoMoveDone = null;
				tmpCB(); 	// 有可能会开启下一个自动寻路
			}
		}
	}
	
	// 停止客户端自动寻路
	public void Stop(bool bTellServer)
	{
		if(!isMoving) return;
		isMoving = false;
		onAutoMoveDone = null;
		lineMove.Stop(true);
	}
}

// 主角默认操作状态: 无操作
public class XMainPlayerStateNone : XState<XMainPlayer>
{
	public XMainPlayerStateNone(XMainPlayer owner)
		: base((int)EStateId.esMainPlayerNone, owner)
	{
		
	}
	
	public override void Enter (params object[] args)
	{
		int a =0;
		a = 9;
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esAutoMoveToObj):
			Machine.TranslateToState((int)EStateId.esAutoMoveToObj, args);
			return true;
			
		case(EStateEvent.esAutoMoveToPos):
			Machine.TranslateToState((int)EStateId.esAutoMoveToPos, args);
			return true;
			
		case(EStateEvent.esKeyMoveDir):
			Machine.TranslateToState((int)EStateId.esKeyMove, args);
			return true;
			
		case(EStateEvent.esNavigateTo):
			Machine.TranslateToState((int)EStateId.esNavigate, args);
			return true;
			
		case(EStateEvent.esSit):
			Machine.TranslateToState((int)EStateId.esMainSit);
			return true;
		
		case(EStateEvent.esNavigateKill):
			Machine.TranslateToState((int)EStateId.esNavKill,args);
			return true;
			
		}	
		return false;
	}
}

// 自动寻路到某点
public class XMainPlayerStateAutoMoveToPos : XState<XMainPlayer>
{
	private XU3dEffect effect = null;
	
	public XMainPlayerStateAutoMoveToPos(XMainPlayer owner)
		: base((int)EStateId.esAutoMoveToPos, owner)
	{
	}
	
	private void destroyEffect()
	{
		if(null != effect)
		{
			effect.Destroy();
			effect = null;
		}
	}
	
	private void autoMoveTo(Vector3 vec)
	{
		destroyEffect();
		Vector3 endPos = Vector3.zero;
		if(XMainPlayerAutoMove.SP.AutoTo(vec, onAutoMoveDone, out endPos))
		{
			effect = new XU3dEffect(900017);
			effect.Position = endPos;
		}
		else
		{
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
	}
	
	private void onAutoMoveDone()
	{
		Machine.TranslateToState((int)EStateId.esMainPlayerNone);
	}
	
	public override void Enter (params object[] args)
	{
		autoMoveTo((Vector3)(args[0]));
	}
	
	public override bool OnEvent(int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esAutoMoveToPos):
			autoMoveTo((Vector3)(args[0]));
			return true;
			
		case(EStateEvent.esAutoMoveToObj):
			Machine.TranslateToState((int)EStateId.esAutoMoveToObj, args);
			return true;
			
		case(EStateEvent.esKeyMoveDir):
			Machine.TranslateToState((int)EStateId.esKeyMove, args);
			return true;
			
		case(EStateEvent.esNavigateTo):
			Machine.TranslateToState((int)EStateId.esNavigate, args);
			return true;
			
		case(EStateEvent.esSit):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainSit);
			return true;
			
		case(EStateEvent.esForceSetPosition):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			return true;
			
		case(EStateEvent.esBeginLoadLevel):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			return true;
		}
		return false;
	}
	
	public override void Exit()
	{
		destroyEffect();
	}
}

// 自动寻路到XGameObject
public class XMainPlayerStateAutoMoveToObj : XState<XMainPlayer>
{
	private XGameObject toObj = null;

	public XMainPlayerStateAutoMoveToObj(XMainPlayer owner)
		: base((int)EStateId.esAutoMoveToObj, owner)
	{
	}
	
	private void autoMoveTo(XGameObject xobj)
	{
		toObj = null;
		if(XMainPlayerAutoMove.SP.AutoTo(xobj.Position, onAutoMoveDone))
		{
			toObj = xobj;
		}
		else
		{
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
	}
	
	private void onAutoMoveDone()
	{
		toObj.OnMouseUpAsButton(0);
		Machine.TranslateToState((int)EStateId.esMainPlayerNone);
	}
	
	public override void Enter (params object[] args)
	{
		autoMoveTo((XGameObject)(args[0]));
	}
	
	public override void Breathe (float deltaTime)
	{
		if(null == toObj)
		{
			XMainPlayerAutoMove.SP.Stop(true);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			return;
		}
		
		float dis = XUtil.CalcDistanceXZ(m_Owner.Position, toObj.Position);
		if(dis <= toObj.GetClickDistance())
		{
			XMainPlayerAutoMove.SP.Stop(true);
			toObj.OnMouseUpAsButton(0);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esKeyMoveDir):
			Machine.TranslateToState((int)EStateId.esKeyMove, args);
			return true;
			
		case(EStateEvent.esAutoMoveToPos):
			Machine.TranslateToState((int)EStateId.esAutoMoveToPos, args);
			return true;
			
		case(EStateEvent.esAutoMoveToObj):
			autoMoveTo((XGameObject)(args[0]));
			return true;
			
		case(EStateEvent.esNavigateTo):
			Machine.TranslateToState((int)EStateId.esNavigate, args);
			return true;
			
		case(EStateEvent.esSit):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainSit);
			return true;
			
		case(EStateEvent.esForceSetPosition):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			return true;
			
		case(EStateEvent.esBeginLoadLevel):
			XMainPlayerAutoMove.SP.Stop(false);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			return true;
		}
		return false;
	}
	
	public override void Exit ()
	{
		toObj = null;
	}
}


// 键盘移动
public class XMainPlayerStateKeyMove : XState<XMainPlayer>
{
	// 同方向一次移动请求的最大距离
    private static readonly float MAX_LINE_MOVE_DIST = 10.0f;
	private Vector2 moveDir = Vector2.zero;
	private Vector3 cameraVec = Vector3.one;	// 根据moveDir和摄像机旋转信息计算出来的期望运动方向
	private bool bIsMoving = false;
	
	public XMainPlayerStateKeyMove(XMainPlayer owner)
		: base((int)EStateId.esKeyMove, owner)
	{
		
	}
	
	private bool keyMove()		// 朝着当前运动方向运动
	{
		bool bSucess = true;
		if(Vector2.zero == moveDir)
		{
			bSucess = false;
		}
		else
		{
			Vector3 expectTgtPos = m_Owner.Position + cameraVec * MAX_LINE_MOVE_DIST;
			Vector3 tmpPos = expectTgtPos; tmpPos.y = m_Owner.Position.y;
			Vector3 tgtRot = Quaternion.LookRotation(tmpPos - m_Owner.Position).eulerAngles;			
			Vector3 tgtPos = XLogicWorld.SP.SceneManager.GetLineReachablePoint(m_Owner.Position, expectTgtPos);
			if(XUtil.CalcDistanceXZ(m_Owner.Position, tgtPos) == 0f)
			{
				bSucess = XMainPlayerLineMove.SP.StepMove(m_Owner.Speed, tgtRot.y);
			}
			else
			{
				Vector3 expectDir = expectTgtPos * 10000f - m_Owner.Position * 10000f;	// 期望方向
				Vector3 tgtDir = tgtPos * 10000f - m_Owner.Position * 10000f;	// 实际方向
				expectDir.y = tgtDir.y = 0f;
				if(Vector3.Angle(expectDir, tgtDir) <= 0.2f)	// 线段运动
				{
					bSucess = XMainPlayerLineMove.SP.SegmentMoveTo(tgtPos, m_Owner.Speed, onLineMoveDone,EAnimName.Run);
				}
				else // 滑行
				{
					bSucess = XMainPlayerLineMove.SP.SlideMoveTo(tgtPos, tgtRot.y, m_Owner.Speed, onLineMoveDone);
				}
			}
		}
		if(!bSucess)
		{
			if(bIsMoving)
			{
				XMainPlayerLineMove.SP.Stop(true);
			}
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
		bIsMoving = bSucess;
		return bIsMoving;
	}
	
	private void keyMove(Vector2 dir)
	{
		moveDir = dir;
		if(Vector2.zero == moveDir)
		{
		}
		else
		{
			cameraVec = XCameraLogic.SP.mainCamera.transform.TransformDirection(moveDir);
			cameraVec.y = 0f;
			cameraVec.Normalize();
		}
		bIsMoving = keyMove();
	}
	
	private void onLineMoveDone()
	{
		keyMove();
	}
	
	public override void Enter(params object[] args)
	{
		keyMove((Vector2)(args[0]));
	}
	
	public override bool OnEvent(int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esKeyMoveDir):
			keyMove((Vector2)(args[0]));
			return true;
			
		case(EStateEvent.esForceSetPosition):
			bIsMoving = false;
			keyMove();
			return true;
		
		case(EStateEvent.esBeginLoadLevel):
			bIsMoving = false;
			XMainPlayerLineMove.SP.Stop(false);
			return true;
			
		case(EStateEvent.esLevelLoaded):
			bIsMoving = false;
			keyMove(XKeyEventGate.SP.getMoveDir());
			return true;			
		}
		return false;
	}
	
	public override void Exit ()
	{
		base.Exit ();
		bIsMoving = false;
		moveDir = Vector2.zero;
	}
}

// 导航状态
public class XMainPlayerStateNavigate : XState<XMainPlayer>
{
	public class NavigateInfo
	{
		public uint expectSceneId = 0;				// 期望到达的目标场景
		
		public EObjectType expectObjectType;	// 期望到达的对象类型
		public int expectObjectId;				// 期望到达的对象ID
		public Vector3 expectPos;				// 期望到达的目标坐标
		public bool IsPos;						// true: 导航到坐标点, false: 导航到对象
		public uint expectDuplicateID;				// 期望到达的副本场景ID
		
		public NavigateInfo(uint sceneId, EObjectType objectType, int objectId, Vector3 pos)
		{
			expectSceneId = sceneId;
			expectObjectType = objectType;
			expectObjectId = objectId;
			expectPos = pos;
			IsPos = false;
		}
		
		public NavigateInfo(uint sceneId, EObjectType objectType, int objectId, Vector3 pos , uint DuplicateID)
		{
			expectSceneId = sceneId;
			expectObjectType = objectType;
			expectObjectId = objectId;
			expectPos = pos;
			IsPos = false;
			expectDuplicateID = DuplicateID;
		}
		
		public NavigateInfo(uint sceneId, Vector3 pos)
		{
			expectSceneId = sceneId;
			expectPos = pos;
			IsPos = true;
		}
	}
	
	public NavigateInfo navInfo = null;
	private XGameObject	navGo = null;
	
	public XMainPlayerStateNavigate(XMainPlayer owner)
		: base((int)EStateId.esNavigate, owner)
	{
	}

	private void onAutoMoveDone()
	{
		if(null != navGo)
		{
			navGo.OnMouseUpAsButton(0);
		}
		Machine.TranslateToState((int)EStateId.esMainPlayerNone);
	}
	
	private void navToObj(XGameObject go)
	{
		navGo = go;
		XEventManager.SP.SendEvent(EEvent.Nav_TargetChange,navGo.Position);
		if(!XMainPlayerAutoMove.SP.AutoTo(navGo.Position, onAutoMoveDone) )
		{
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}		
	}
	
	private bool navToExpectObj()
	{
		switch(navInfo.expectObjectType)
		{
		case(EObjectType.Npc):
		{
			XNpc npc = XNpc.GetByKindIndex(navInfo.expectObjectId);
			if(null == npc) return false;
			navToObj(npc); return true;
		}
		case(EObjectType.TransPoint):
		{
			XTransPoint transPoint = XTransPoint.GetByIndex((uint)navInfo.expectObjectId);
			if(null == transPoint) return false;
			navToObj(transPoint); return true;
		}
		case(EObjectType.GatherObject):
		{
			XGatherObject gatherObject = XGatherObject.GetByStaticId(navInfo.expectObjectId);
			if(null == gatherObject) return false;
			navToObj(gatherObject); return true;
		}
		}
		return false;
	}
	
	private void navOnCurScene()
	{
		if(!navInfo.IsPos && navToExpectObj())
		{
			return;
		}
		XEventManager.SP.SendEvent(EEvent.Nav_TargetChange,navInfo.expectPos);
		
		if(!XMainPlayerAutoMove.SP.AutoTo(navInfo.expectPos, onAutoMoveDone))
		{
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
		
		
	}
	
	private void NavigateTo(NavigateInfo info)
	{
		navGo = null;
		navInfo = info;
		if(null == navInfo)
		{
			XMainPlayerAutoMove.SP.Stop(true);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
		else
		{
			if(navInfo.expectSceneId == XLogicWorld.SP.SceneManager.LoadedSceneId)
			{
				navOnCurScene();
			}else
			{
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eWorldMap);
				XEventManager.SP.SendEvent(EEvent.WorldMap_RequireLoadScene, navInfo.expectSceneId);
				//XLogicWorld.SP.SceneManager.RequireEnterScene(navInfo.expectSceneId);
			}
		}
		
		XNewPlayerGuideManager.SP.handleDirectionDelay((int)XNewPlayerGuideManager.GuideType.Guide_Mouse);
	}
	
	public override void Enter (params object[] args)
	{
		NavigateTo((NavigateInfo)(args[0]));
	}
	
	public override void Breathe (float deltaTime)
	{
		if(null == navGo)
			return;
		
		float dis = XUtil.CalcDistanceXZ(m_Owner.Position, navGo.Position);
		if(dis <= navGo.GetClickDistance())
		{
			XMainPlayerAutoMove.SP.Stop(true);
			navGo.OnMouseUpAsButton(0);
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			XEventManager.SP.SendEvent(EEvent.Nav_MoveEnd);
		}
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esAutoMoveToObj):
			Machine.TranslateToState((int)EStateId.esAutoMoveToObj, args);
			return true;
			
		case(EStateEvent.esAutoMoveToPos):
			Machine.TranslateToState((int)EStateId.esAutoMoveToPos, args);
			return true;
		
		case(EStateEvent.esKeyMoveDir):
			Machine.TranslateToState((int)EStateId.esKeyMove, args);
			return true;
			
		case(EStateEvent.esNavigateTo):
			NavigateTo((NavigateInfo)(args[0]));
			return true;
			
		case(EStateEvent.esNavigateKill):
			Machine.TranslateToState((int)EStateId.esNavKill,args);
			return true;
			
		case(EStateEvent.esBeginLoadLevel):
			XMainPlayerAutoMove.SP.Stop(false);
			
			if(null == navInfo || navInfo.expectSceneId != (int)(args[0]))
			{
				Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			}
			return true;
			
		case(EStateEvent.esLevelLoaded):
			
			if(null == navInfo || navInfo.expectSceneId != (int)(args[0]))
			{
				XMainPlayerAutoMove.SP.Stop(true);
				Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			}
			else
			{
				navOnCurScene();
			}
			return true;
			
		case(EStateEvent.esObjectAppear):
			{
				if(null == navGo && !navInfo.IsPos)
				{
						EObjectType objectType = (EObjectType)(args[0]);
						if(objectType != navInfo.expectObjectType)
						{
							return true;
						}
						switch(objectType)
						{
							case(EObjectType.Npc):
							{
								XNpc npc = (XNpc)(args[1]);
								if(npc.KindIndex == navInfo.expectObjectId)
									navToObj(npc);
								return true;
							}
							case(EObjectType.TransPoint):
							{
								XTransPoint transPoint = (XTransPoint)(args[1]);
								if(transPoint.EntryInfo.Index == (uint)(navInfo.expectObjectId))
									navToObj(transPoint);
								return true;
							}	
							case(EObjectType.GatherObject):
							{
								XGatherObject gatherObject = (XGatherObject)(args[1]);
								if(gatherObject.m_cfgGatherObject.ID == navInfo.expectObjectId)
									navToObj(gatherObject);
								return true;
							}
						}
				}
			}
			return true;
		}
		return false;
	}
	
	public override void Exit ()
	{
		navInfo = null;
	}
}

// 主角导航自动寻敌: 
public class XMainPlayerStateNavKill : XState<XMainPlayer>
{
	public uint m_expectDuplicateId = 0;
	
	public XMainPlayerStateNavKill(XMainPlayer owner)
		: base((int)EStateId.esNavKill, owner)
	{
		
	}
	
	private void doneFind()
	{
		
	}
	
	private bool findEnemy()
	{
		XMonster monster = XLogicWorld.SP.SubSceneManager.GetCurMonster();
		if( null == monster )
		{
			return false;
		}
		
		XMainPlayerAutoMove.SP.AutoTo(monster.Position,doneFind);
		return true;
	}
	
	private bool needFindEnemy()
	{
		if( !m_Owner.IsInSubScene )
			return false;
		
		if(m_expectDuplicateId == XLogicWorld.SP.SubSceneManager.SubSceneID)
		{
			return true;
		}else
		{
			return false;
		}
		
	}
	
	private void processLoadedEvent( ESceneType type )
	{
			XMainPlayerAutoMove.SP.Stop(false);
			if(needFindEnemy() )
			{
				if( ESceneType.ClientScene == type )
				{
					if( true == findEnemy() )
					{
						return;
					}else{
						Machine.TranslateToState((int)EStateId.esMainPlayerNone);
					}
				}else if(ESceneType.FightScenePVE == type || ESceneType.FightScenePVP == type)
				{
					if( XLogicWorld.SP.SubSceneManager.IsWin )
					{
						return;
					}else
					{
						Machine.TranslateToState((int)EStateId.esMainPlayerNone );
					}
					return;
				}
				else
				{
					Machine.TranslateToState((int)EStateId.esMainPlayerNone);
				}
			}else
			{
				Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			}
	}
	
	public override void Enter (params object[] args)
	{
		m_expectDuplicateId = (uint)(args[0]);
		
		if(needFindEnemy() )
		{
			findEnemy();
		}else
		{
			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
		}
		
	}
	
	public override bool OnEvent (int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		switch(ESE)
		{
		case(EStateEvent.esAutoMoveToObj):
			Machine.TranslateToState((int)EStateId.esAutoMoveToObj, args);
			return true;
			
		case(EStateEvent.esAutoMoveToPos):
			Machine.TranslateToState((int)EStateId.esAutoMoveToPos, args);
			return true;
			
		case(EStateEvent.esKeyMoveDir):
			Machine.TranslateToState((int)EStateId.esKeyMove, args);
			return true;
			
		case(EStateEvent.esNavigateTo):
			Machine.TranslateToState((int)EStateId.esNavigate, args);
			return true;
			
		case(EStateEvent.esSit):
			Machine.TranslateToState((int)EStateId.esMainSit);
			return true;
			
		case(EStateEvent.esBeginLoadLevel):
			XMainPlayerAutoMove.SP.Stop(false);
			return true;
			
		case(EStateEvent.esLevelLoaded):
			processLoadedEvent((ESceneType)args[1] );
			
			return true;
		case (EStateEvent.esNavigateKill):
			if(needFindEnemy() )
			{
				findEnemy();
			}else
			{
				Machine.TranslateToState((int)EStateId.esMainPlayerNone);
			}
			return true;
		}
		return false;
	}
	
	public override void Exit()
	{
		XMainPlayerAutoMove.SP.Stop(false);
	}
}


// 打坐
//public class XMainPlayerStateSit : XState<XMainPlayer>
//{
//	private XU3dEffect effect = null;
//	private XU3dEffect effectMeditationLive = null;
//
//	public XMainPlayerStateSit(XMainPlayer owner)
//		: base((int)EStateId.esSit, owner)
//	{
//	}
//	
//	public override void Enter (params object[] args)
//	{
//		effect = new XU3dEffect(900420);
//		effectMeditationLive = new XU3dEffect(900061);
//		if(!m_Owner.OnEnterSit())
//		{
//			Machine.TranslateToState((int)EStateId.esMainPlayerNone);
//			return;
//		}
//
//		XLogicWorld.SP.MainPlayer.AttachGo(ESkeleton.eMainObject,effect.m_gameObject);
//		XLogicWorld.SP.MainPlayer.AttachGo(ESkeleton.eCapsuleTop,effectMeditationLive.m_gameObject);
//		m_Owner.OnEnterSit();
//	}
//	
//	public override bool OnEvent (int evt, params object[] args)
//	{
//		EStateEvent ESE =(EStateEvent)evt;
//		switch(ESE)
//		{
//		case(EStateEvent.esKeyMoveDir):
//			Machine.TranslateToState((int)EStateId.esKeyMove, args);
//			return true;
//			
//		case(EStateEvent.esAutoMoveToPos):
//			Machine.TranslateToState((int)EStateId.esAutoMoveToPos, args);
//			return true;
//			
//		case(EStateEvent.esAutoMoveToObj):
//			Machine.TranslateToState((int)EStateId.esAutoMoveToObj, args);
//			return true;
//
//		case(EStateEvent.esCancelSit):
//			m_Owner.ChangeState((int)EStateId.esIdle);
//			destroyEffect();
//			return true;
//		case(EStateEvent.esLevelLoaded):		
//			m_Owner.PushOnTerrain();
//			return true;		
//		}
//		return false;
//	}
//	
//	public override void Exit ()
//	{
//		destroyEffect();
//		m_Owner.OnExitSit();
//		//XMeditationManager.SP.MeditationEnd();
//	}
//
//	private void destroyEffect()
//	{
//		if(null != effect)
//		{
//			effect.Destroy();
//			effect = null;
//		}
//		if(null !=  effectMeditationLive)
//		{
//			effectMeditationLive.Destroy();
//			effectMeditationLive = null;
//
//		}
//	}
//}

public class XMainPlayerStatePreEnterScene : XState<XMainPlayer>
{
	private int mIndex;
	private int mHardLevel;
	public static uint TransEffect = 900015;
	
	public XMainPlayerStatePreEnterScene(XMainPlayer owner)
		: base((int)EStateId.esPreEnterScene, owner)
	{
	}
	
	public override void Enter (params object[] args)
	{
		mIndex		= (int)args[0];
		mHardLevel	= (int)args[1];
		
		CoroutineManager.StartCoroutine(ReadyTrans());
	}
	
	public override void Exit ()
	{
		
	}
	
	public IEnumerator ReadyTrans()
	{
		XU3dEffect effect = new XU3dEffect(TransEffect);
		effect.Position	= XLogicWorld.SP.MainPlayer.Position;
		yield return new WaitForSeconds(2);
		
		//m_Owner.SendModelEvent(EModelEvent.evtFadeOut);	
		//yield return new WaitForSeconds(2);
		
		XEventManager.SP.SendEvent(EEvent.SelectScene_ChooseScene, mIndex,mHardLevel);
		m_Owner.ChangeState((int)EStateId.esMainPlayerNone);
	}
}