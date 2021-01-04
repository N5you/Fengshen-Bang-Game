using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Packets;

/* 类名: XMainPlayer
 * 描述: 主角对象
 * 
 */
public partial class XMainPlayer : XPlayer
{
	private bool m_bEntered = false;
	
	
	public XMainPlayer(ulong id)
		: base(id)
	{
		ObjectType = EObjectType.MainPlayer;
		IsEnableHover	= false;
		initOperationSM();
		ItemManager.SetSpaceMgr(XItemSpaceMgr.SP);
		
		XEventManager.SP.AddHandler(NavStartHandler,EEvent.Nav_TargetChange);
		XEventManager.SP.AddHandler(NavEndHandler,EEvent.Nav_MoveEnd);
	}
	
	public  void NavStartHandler(EEvent evt, params object[] args)
	{
		Vector3 targetPos = (Vector3)args[0];
		ObjectModel.SetMoveTarget(EMoveTarget_Type.EMoveTarget_Pos,null,targetPos);
	}	
	
	public  void NavEndHandler(EEvent evt, params object[] args)
	{
		ObjectModel.CancelMoveTarget();
	}
	
	
	
	public override void Breathe()
	{
		base.Breathe();
		m_OperationSM.Breathe();
		XMainPlayerLineMove.SP.Breathe();
	}
	
	public void AttachMainCamera()
	{
		SendModelEvent(EModelEvent.evtAttachMainCamera);
	}
	
	public void On_SC_PlayerBaseInfo(SC_PlayerBaseInfo info)
    {
		ID = info.ObjectId;
        Name = info.Name;
        Title = info.Title;
        Sex = (byte)info.Sex;
        ClassLevel = info.ClassLevel;
        Power = info.Power;
        FightRemain = info.FightRemain;
        UColor = info.Color;
		SetModel(EModelCtrlType.eModelCtrl_Original, info.Model);
        Exp = info.Exp;
        GameMoney = info.GameMoney;
        RealMoney = info.RealMoney;
        BagSize = info.BagSize;
        BankSize = info.BankSize;		
		//CacheSceneInitPos	= new Vector3(info.PosX, info.PosY, info.PosZ);
		Position	= new Vector3(info.PosX, info.PosY, info.PosZ);
		
		Level = info.Level;
		Class = info.Class;
		Grow = info.Grow / Define.CONFIG_RATE_BASE;
		WuLi		= info.WuLi;
		LingQiao	= info.LingQiao;
		TiZhi		= info.TiZhi;
		ShuFa		= info.ShuFa;
		VIPLevel	= info.VIPLevel;
		HealthBuyCount = info.HealthBuyCount;
		EatFoodCount	= info.EatFoodCount;
		ShengWangValue	= info.ShengWangValue;
		TotalShengWangValue	= info.TotalShengWangValue;
		DayPeiYangCount	= info.DayPeiYangCount;
		ShowFashion = info.ShowFashion;
		GuildId			= info.GuildId;
		Recharge		= info.Recharge;
		VipAward		= info.VipAward;
    }
	
	public void On_SC_PlayerDetailInfo(SC_PlayerDetailInfo msg)
    {		
		UpdateDynAttrs(msg.AttrsList);
		if(!m_bEntered)
		{
			SetDefaultWeapon();
			Appear();
			//m_ObjectModel.LoadModelEvent	+= new XObjectModel.LoadModelComplete(LoadModelComplete);
		}
    }
	
//	public void LoadModelComplete(XObjectModel model)
//	{
//		SendModelEvent(EModelEvent.evtShowBlood, false);
//		AttachMainCamera();
//		m_ObjectModel.XRayType			= EXRayMatType.EXRayMatType_MainPlayer;
//		m_ObjectModel.WeaponXRayType	= EXRayMatType.EXRayMatType_MainPlayerWeapon;
//		XCameraLogic.SP.Scroll(0.3f, true);
//		XEventManager.SP.SendEvent(EEvent.MainPlayer_EnterGame);
//		m_bEntered = true;
//	}
	
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		SendModelEvent(EModelEvent.evtShowBlood, false);
		
		m_ObjectModel.XRayType			= EXRayMatType.EXRayMatType_MainPlayer;	
		m_ObjectModel.ReAttachWeapon();
		m_ObjectModel.ReAttachMount();
		if(!m_bEntered)
		{
			XCameraLogic.SP.Scroll(0.3f, true);
			XEventManager.SP.SendEvent(EEvent.MainPlayer_EnterGame);			
			m_bEntered = true;
			AttachMainCamera();
		}
	}
	
	public override void OnWeaponLoaded()
	{
		if(m_ObjectModel != null)
			m_ObjectModel.WeaponXRayType	= EXRayMatType.EXRayMatType_MainPlayerWeapon;
	}
	
	private void SetDefaultWeapon()
	{
		int JobClass = DynGet(EShareAttr.esa_Class);
		XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)JobClass);
		if(playerBase != null)
			WeaponModelId	= playerBase.DefaultWeapon;
	}
	
	public override void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{
		//XEventManager.SP.SendEvent(EEvent.ObjSel_SetData,Name);
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eTargetInfo);
	}
	
	#region 角色操作状态机
	private XStateMachince m_OperationSM = null;
	private void initOperationSM()
	{
		m_OperationSM = new XStateMachince(new XMainPlayerStateNone(this));
		m_OperationSM.RegState(new XMainPlayerStateAutoMoveToPos(this));
		m_OperationSM.RegState(new XMainPlayerStateAutoMoveToObj(this));
		m_OperationSM.RegState(new XMainPlayerStateKeyMove(this));
		m_OperationSM.RegState(new XMainPlayerStateNavigate(this));
		m_OperationSM.RegState(new XMainPlayerStateNavKill(this));
		//m_OperationSM.RegState(new XMainPlayerStateSit(this));
		m_OperationSM.RegState(new XMainPlayerStatePreEnterScene(this));
	}
	
	public int GetCurrentStateID()
	{
		if(m_OperationSM == null)
			return 0;
		return m_OperationSM.CurrentState.ID;
	}
	
	public XStateBase GetCurrentState()
	{
		if(m_OperationSM == null)
			return null;
		return m_OperationSM.CurrentState;
	}

	public void ChangeState(int evt, params object[] args)
	{
		m_OperationSM.TranslateToState(evt, args);
	}
	
	
	public bool AutoMoveTo(XGameObject xobj)
	{
		return m_OperationSM.OnEvent((int)EStateEvent.esAutoMoveToObj, xobj);
	}
	
	public bool AutoMoveTo(Vector3 toPos)
	{
		return m_OperationSM.OnEvent((int)EStateEvent.esAutoMoveToPos, toPos);
	}
	
	public bool KeyMove(Vector2 dir)
	{
		XNewPlayerGuideManager.SP.handleKeybordFinish((int)XNewPlayerGuideManager.GuideType.Guide_KeyBord);
		
		return m_OperationSM.OnEvent((int)EStateEvent.esKeyMoveDir, dir);
	}
	
	public void StartJump()
	{
		if(!base.Jump(OnJumpOver))
			return;
		
		Vector3 posNow = Position;
		CS_Jump.Builder builder = CS_Jump.CreateBuilder();
        	builder.PosX = posNow.x;
        	builder.PosZ = posNow.z;
        	XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Jump, builder.Build());
	}
	
	public void OnJumpOver()
	{
		JumpOver();
		CS_JumpOver.Builder builder = CS_JumpOver.CreateBuilder();
		builder.SetPosX(Position.x);
		builder.SetPosZ(Position.z);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_JumpOver, builder.Build());
	}
		
	public void StartSit()
	{
		m_BehaviourSM.OnEvent((int)EStateEvent.esSit);
	}

	public void EndSit()
	{
		m_BehaviourSM.OnEvent((int)EStateEvent.esCancelSit);
	}

	public override void OnEnterSit()
	{
		Vector3 posNow = this.Position;
		CS_Sit.Builder builder = CS_Sit.CreateBuilder();
			        builder.PosX = posNow.x;
			        builder.PosY = posNow.y;
			        builder.PosZ = posNow.z;
			        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Sit, builder.Build());
	}

	public override void OnExitSit()
	{
		XMeditationManager.SP.MeditationEndRequest();
	}
	
	public override void ForceSetPosition(Vector3 pos, Vector3 dir)
	{
		Log.Write(LogLevel.INFO, "Force Set Position: " + pos + " " + dir);
		base.ForceSetPosition(pos, dir);
		m_OperationSM.OnEvent((int)EStateEvent.esForceSetPosition);
	}
	
	public void NavigateTo(XMainPlayerStateNavigate.NavigateInfo navInfo)
	{
		m_OperationSM.OnEvent((int)EStateEvent.esNavigateTo, navInfo);
	}
	
	public void NavigateKill( uint expectDuplicateId )
	{
		m_OperationSM.OnEvent( (int)EStateEvent.esNavigateKill,expectDuplicateId );
	}
	
	public override void OnBeginLoadLevel(int nLevelId, ESceneType sceneType)
	{
		m_OperationSM.OnEvent((int)EStateEvent.esBeginLoadLevel, nLevelId, sceneType);
	}
	
	public override void OnLevelLoaded(int nLevelId, ESceneType sceneType)
	{
		base.OnLevelLoaded (nLevelId, sceneType);
		m_OperationSM.OnEvent((int)EStateEvent.esLevelLoaded, nLevelId, sceneType);
	}
	
	public void OnObjectAppear(EObjectType objectType, XGameObject gameObject)
	{
		m_OperationSM.OnEvent((int)EStateEvent.esObjectAppear, objectType, gameObject);
	}
	#endregion
	
	#region attr
	public uint WuLi
	{
		get
		{
			return m_AttrMainPlayer.WuLiValue;
		}
		set
		{
			if(m_AttrMainPlayer.WuLiValue != value)
			{
				m_AttrMainPlayer.WuLiValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint LingQiao
	{
		get
		{
			return m_AttrMainPlayer.LingQiaoValue;
		}
		set
		{
			if(m_AttrMainPlayer.LingQiaoValue != value)
			{
				m_AttrMainPlayer.LingQiaoValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint TiZhi
	{
		get
		{
			return m_AttrMainPlayer.TiZhiValue;
		}
		set
		{
			if(m_AttrMainPlayer.TiZhiValue != value)
			{
				m_AttrMainPlayer.TiZhiValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint ShuFa
	{
		get
		{
			return m_AttrMainPlayer.ShuFaValue;
		}
		set
		{
			if(m_AttrMainPlayer.ShuFaValue != value)
			{
				m_AttrMainPlayer.ShuFaValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public bool	IsEnter
	{
		get
		{
			return m_bEntered;
		}
	}
	
	public override int Level
	{
		get { return DynGet(EShareAttr.esa_Level); }
		set
		{
			DynSet(EShareAttr.esa_Level, value);
			XEventManager.SP.SendEvent(EEvent.Update_Level, this, value);
		}
	}
	
	public uint HealthBuyCount
	{
		get { return m_AttrMainPlayer.HealthBuyCount; }
		set
		{
			if(m_AttrMainPlayer.HealthBuyCount != value)
			{
				m_AttrMainPlayer.HealthBuyCount	= value;
				XEventManager.SP.SendEvent(EEvent.UI_UpdateBuyHealthCount);
			}
		}
	}
	
	public uint VIPLevel
	{
		get { return m_AttrMainPlayer.VIPLevel; }
		set
		{
			if(m_AttrMainPlayer.VIPLevel != value)
			{
				m_AttrMainPlayer.VIPLevel	= value;
				XEventManager.SP.SendEvent(EEvent.UI_UpdateVIP);
			}
		}
	}
	
	public int BattleValue
	{
		get
		{
			return XUIRoleInformation.CalcBattleValue(this);
		}
	}
	
	public uint EatFoodCount
	{
		get { return m_AttrMainPlayer.EatFoodCount; }
		set
		{
			if(m_AttrMainPlayer.EatFoodCount != value)
			{
				m_AttrMainPlayer.EatFoodCount	= value;
				XEventManager.SP.SendEvent(EEvent.UI_EatFoodCount);
			}
			
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_Food, (int)(3 - m_AttrMainPlayer.EatFoodCount), 3);
		}
	}
	
	public uint ShengWangValue
	{
		get { return m_AttrMainPlayer.ShengWangValue; }
		set
		{
			if(m_AttrMainPlayer.ShengWangValue != value)
			{
				m_AttrMainPlayer.ShengWangValue	= value;
				XEventManager.SP.SendEvent(EEvent.UI_ShengWangValue);
			}
		}
	}
	
	public uint TotalShengWangValue
	{
		get { return m_AttrMainPlayer.TotalShengWangValue;}
		set
		{
			if(m_AttrMainPlayer.TotalShengWangValue != value)
			{
				m_AttrMainPlayer.TotalShengWangValue = value;
				XEventManager.SP.SendEvent(EEvent.UI_TotalShengWangValue);
				
			}
		}
	}
	
	public ulong GuildId
	{
		get { return m_AttrMainPlayer.GuildId;}
		set
		{
			if(m_AttrMainPlayer.GuildId != value)
			{
				m_AttrMainPlayer.GuildId = value;
				//XEventManager.SP.SendEvent(EEvent.);
				
			}
		}
	}
	
	public uint VipAward
	{
		get { return m_AttrMainPlayer.VipAward;}
		set
		{
			if(m_AttrMainPlayer.VipAward != value)
			{
				m_AttrMainPlayer.VipAward = value;
				//XEventManager.SP.SendEvent(EEvent.);
				
			}
		}
	}
	
	public ulong Recharge
	{
		get { return m_AttrMainPlayer.Recharge;}
		set
		{
			if(m_AttrMainPlayer.Recharge != value)
			{
				m_AttrMainPlayer.Recharge = value;
				//XEventManager.SP.SendEvent(EEvent.);
				
			}
		}
	}
	
	public uint DayPeiYangCount
	{
		get { return m_AttrMainPlayer.DayPeiYangCount;}
		set
		{
			if(m_AttrMainPlayer.DayPeiYangCount != value)
			{
				m_AttrMainPlayer.DayPeiYangCount = value;
				XEventManager.SP.SendEvent(EEvent.UI_DayPeiYangCount);
			}
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_HuaLing, 
					(int)(XUIRoleInformation.MAX_DAY_PEIYANG_COUNT - DayPeiYangCount), (int)XUIRoleInformation.MAX_DAY_PEIYANG_COUNT);
		}
	}
	
	public bool 		IsInSubScene	{get;set;}
	public uint 		SrcSubSceneID	{get;set;}
	public ESceneType 	SrcSceneType	{get;set;}
	
	public Vector3		CacheSceneInitPos = Vector3.zero;
	public Vector3		CacheSceneInitDir = Vector3.zero;
	
	public void InitRoleInfo(SC_PlayerInfo playerInfo)
	{
		SC_PlayerBaseInfo info = playerInfo.BaseInfo;
		ID = info.ObjectId;
        Name = info.Name;
        Title = info.Title;
        Sex = (byte)info.Sex;
        ClassLevel = info.ClassLevel;
        Power = info.Power;
        FightRemain = info.FightRemain;
        UColor = info.Color;
		SetModel(EModelCtrlType.eModelCtrl_Original, info.Model);
        Exp = info.Exp;
        GameMoney = info.GameMoney;
        RealMoney = info.RealMoney;
        BagSize = info.BagSize;
        BankSize = info.BankSize;
		Position = new Vector3(info.PosX, info.PosY, info.PosZ);
		PushOnTerrain();
		Level = info.Level;
		Class = info.Class;
		Grow = info.Grow / Define.CONFIG_RATE_BASE;
		WuLi		= info.WuLi;
		LingQiao	= info.LingQiao;
		TiZhi		= info.TiZhi;
		ShuFa		= info.ShuFa;
		GuildId		= info.GuildId;
		Recharge  = info.Recharge;
		VipAward  = info.VipAward;
		
		SC_PlayerDetailInfo msg = playerInfo.DetailInfo;
		UpdateDynAttrs(msg.AttrsList);

		SC_ItemList items = playerInfo.Items;
		for (int i = 0; i < items.ItemListCount; ++i)
        {
			PB_ItemInfo pbInfo = items.GetItemList(i); 
			XItem temp = new XItem();
			temp.InitFromPB(pbInfo);
			EItemBoxType type;
			ushort index;
			XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
			ItemManager.SetItem(type,index,temp);
        }
	}
		
	#endregion
	
	//  
	public uint GetShengWangLvl()
	{
		uint iLvl = 1;
		XCfgShengWangLvl  cfgShengWangLvl = null;
		//XCfgShengWangLvlMgr.SP.GetConfig();
		SortedList<uint, XCfgShengWangLvl> ItemTable = XCfgShengWangLvlMgr.SP.ItemTable;
		if(ItemTable == null) return 1;
		foreach(KeyValuePair<uint, XCfgShengWangLvl> kvpItem in ItemTable)
		{
			cfgShengWangLvl = kvpItem.Value;
			if(TotalShengWangValue >= cfgShengWangLvl.PrestigeValue)
			{
				iLvl = cfgShengWangLvl.PrestigeLv;
				continue;
			}
			return iLvl;
		}
		return 1;
	}


	
}
