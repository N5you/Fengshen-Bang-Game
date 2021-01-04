using XGame.Client.Packets;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using UnityEngine;

//代表一场战斗的表现
public class BattleDisplayerMgr: XSingleton<BattleDisplayerMgr>
{
	public static readonly float BATTLE_UNIT_SIZE = 3.5f;      // 战斗单位间距
	public static readonly float BATTLE_ROUND_INTERVAL = 1.0f;	// 回合之间相距2s
	public static readonly float BATTLE_GROUP_OFFSET = 4.0f;   // 左右战斗群体距离中心点的距离
	public static readonly uint GRID_EFFECT_ID = 900026;

	// 战斗摄像机相对于中心点的偏移
	public static readonly Vector3 BATTLE_CAMERA_OFFSET = new Vector3(26.06129f,24.24784f,-30.4854f);
	public static readonly float BATTLE_CAMERA_FOV = 20.0f;
	
	public float 	PlaySpeed {get;set;}
	public bool		IsDirectOver = false;
	
	private XBattlePosition m_MainPlayerPos = null;
	
	public XBattlePosition MainPlayerPos{
		get{ return m_MainPlayerPos; }
		set{ m_MainPlayerPos = value;}
	}
	
	public BattleDisplayerMgr()
	{
		XEventManager.SP.AddHandler(FightSubBlood,EEvent.FightHead_Show_Sub_Blood);
		XEventManager.SP.AddHandler(ResetMaxFightBlood,EEvent.FightHead_Show_ReSet_Max_Blood);
		
		XEventManager.SP.AddHandler(FightAnimStart,EEvent.Fight_Anim_Start );
		XEventManager.SP.AddHandler(FightAnimEnd,EEvent.Fight_Anim_End );
	}
	
	public int LeftBloodValue {get;set;}
	public int RightBloodValue {get;set;}
	public int LeftBattleValue {get;set;}
	public int RightBattleValue {get;set;}
	
	private int CurLeftBloodValue;
	private int CurRightBloodValue;
	
	private XU3dEffect[] mEffectList = new XU3dEffect[XBattleDefine.BATTLE_POS_COUNT * (int)EBattleGroupType.eBattleGroup_End];
	
	public void SetBloodValue(uint left,uint right)
	{
		LeftBloodValue	= (int)left;
		RightBloodValue	= (int)right;
		
		CurLeftBloodValue	= LeftBloodValue;
		CurRightBloodValue	= RightBloodValue;		
	}
	
	public void FightSubBlood(EEvent evt, params object[] args)
	{
		EBattleGroupType group = (EBattleGroupType)(args[0]);
		int DamageValue = (int)(args[1]);
		
		if(group == EBattleGroupType.eBattleGroup_Left)
		{
			CurLeftBloodValue = (int)(CurLeftBloodValue + DamageValue);
			if(CurLeftBloodValue < 0)
				CurLeftBloodValue = 0;
			if(CurLeftBloodValue > LeftBloodValue)
				CurLeftBloodValue = LeftBloodValue;
			
			float final = CurLeftBloodValue *1.0f/LeftBloodValue;
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_LBlood_Value,final);
		}
		else if(group == EBattleGroupType.eBattleGroup_Right)
		{
			CurRightBloodValue= (int)(CurRightBloodValue + DamageValue);
			if(CurRightBloodValue < 0)
				CurRightBloodValue = 0;
			if(CurRightBloodValue > RightBloodValue)
				CurRightBloodValue = RightBloodValue;
			
			float final = CurRightBloodValue * 1.0f/RightBloodValue;
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_RBlood_Value,final);
		}
	}
	
	public void FireSoulValue(XBattlePosition playerPos )
	{
		//is main player
		XBattleObject obj = m_BattleObjects[(int)playerPos.Group,(int)playerPos.Position];
			
		if( EObjectType.OtherPlayer == obj.ObjectType )
		{
			XBattlePlayer player = ((XBattlePlayer)obj);
			
			if(12<player.SoulValue )
			{
				Log.Write(LogLevel.WARN,"player soul value > 12 is a error");
				return;
			}else if( 12 == player.SoulValue)	
			{
				((XBattlePlayer)obj).SoulValue = 0;
				
				//UI Soul show empty effect
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Soul_Empty);
			}
		}
	}
	
	public void ResetMaxFightBlood(EEvent evt, params object[] args)
	{
		EBattleGroupType group = (EBattleGroupType)(args[0]);
		int DamageValue = (int)(args[1]);
		
		if(group == EBattleGroupType.eBattleGroup_Left)
		{
			CurLeftBloodValue = (int)(CurLeftBloodValue + DamageValue);
			if(CurLeftBloodValue < 0)
				CurLeftBloodValue = 0;
			if(CurLeftBloodValue > LeftBloodValue)
				LeftBloodValue = CurLeftBloodValue;
			
			float final = CurLeftBloodValue *1.0f/LeftBloodValue;
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_LBlood_Value,final);
		}
		else if(group == EBattleGroupType.eBattleGroup_Right)
		{
			CurRightBloodValue= (int)(CurRightBloodValue + DamageValue);
			if(CurRightBloodValue < 0)
				CurRightBloodValue = 0;
			if(CurRightBloodValue > RightBloodValue)
				RightBloodValue = CurRightBloodValue;
			
			float final = CurRightBloodValue * 1.0f/RightBloodValue;
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_RBlood_Value,final);
		}
	}
	
	private void FightAnimStart(EEvent evt, params object[] args )
	{
		foreach(XBattleObject obj in m_BattleObjects )
		{
			if(obj == null)
				continue;
			
			//hide head name board
			obj.SendModelEvent(EModelEvent.evtHudVisible,false );
		}
	}
	
	public void FightAnimEnd(EEvent evt, params object[] args)
	{
		//HP only care the current cut scene target list
		foreach(XBattlePosition pos in XCutSceneMgr.SP.m_BatTargetPosList)
		{
			XBattleObject obj = GetBattleObject(pos);
			if(obj == null)
				continue;
			
			if(0 >=obj.Hp )
			{
				obj._playAnimation(EAnimName.Death,1.0f,false);
			}
		}
		
		foreach(XBattleObject obj in m_BattleObjects )
		{
			if(obj == null)
				continue;
			
			
			//hide head name board
			obj.SendModelEvent(EModelEvent.evtHudVisible,true );
		}
	}
	
	private void CheckCameraEffect()
	{
		Camera mainCamera = Camera.main;
		if(null == mainCamera) return;
		MaskEffect effect = mainCamera.gameObject.GetComponent<MaskEffect>() as MaskEffect;
		if(effect == null)
			return ;
		effect.enabled	= false;
	}
	
	public void DirectBattleOver()
	{
		mCurRoundID	= mCurBattleRoundList.Count;
		CheckCameraEffect();
	}
	
	#region Battle Display Init
	// 缓存MainPlayer下面的所有buff的静态数据, 带入到战斗中.
	public void CacheMP()
	{
		m_MPModelIdCache = XLogicWorld.SP.MainPlayer.ModelId;
		m_MpWeaponModelIdCache = XLogicWorld.SP.MainPlayer.WeaponModelId;
		m_MPBuffCache = new List<XBuff>();
		foreach(XBuff buff in XLogicWorld.SP.MainPlayer.BuffOper.m_ExistBuff.Values)
		{
			XBuff buffCache = new XBuff(buff.BuffId, buff.BuffLevel, buff.BuffLayer, null);
			buffCache.RemainLife = buff.RemainLife;
			m_MPBuffCache.Add(buffCache);
		}
	}
	
	public void ShowAllFighter()
	{
		
	}
	
	public void SetCamera()
	{	
		XCameraLogic.SP.AttachTo(XLogicWorld.SP.SceneManager.GetCenterPos(), BATTLE_CAMERA_OFFSET);
		
		float CameraFov = BATTLE_CAMERA_FOV;
		XCfgGameParam param1 = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_FOV);
		if(param1 != null)
			CameraFov	= param1.Value;
		XCameraLogic.SP.ChangeFOV(CameraFov);
	}
	
	public void CreatePlayer(PB_SingleObjectData info)
	{
		XBattlePosition pos = XBattlePosition.Create(info.Pos);
		EBattleGroupType	e 	= pos.Group;
		uint	BattlePos		= pos.Position;
		
		Vector3 dir = GetFighterDir(e);

		// 玩家站位
		m_BattleObjects[(int)e, BattlePos]	= new XBattlePlayer();
		XBattleObject playerObject 			= m_BattleObjects[(int)e, BattlePos];
		playerObject.curBattlePos			= pos;
		playerObject.Position				= GetFighterPos(e, (int)BattlePos);
		playerObject.Direction				= dir; 
		playerObject.Name					= info.Name;
		playerObject.MaxHp					= (int)info.MaxHP;
		playerObject.Hp 					= (int)info.MaxHP;
		playerObject.Class					= (int)info.ClassType;
		playerObject.Sex					= (byte)info.Sex;
		((XBattlePlayer)playerObject).SoulValue	= info.Soul;
		
		//main player
		if(XLogicWorld.SP.MainPlayer.BattlePos == pos.Position )
		{
			XEventManager.SP.SendEvent(EEvent.FightHead_Show_Soul_Value,info.Soul );
		}
		
		//playerObject.Level					= mainPlayer.Level;

        uint ClothesModelID = XItemManager.GetClothesModelID(info.PlayerDuty, info.Sex, info.ClothesID, info.ClothesStrengthenLevel, info.ClothesColor);
        uint WeaponModelID = XItemManager.GetWeaponModelID(info.PlayerDuty, info.Sex, info.WeaponID, info.WeaponStrengthenLevel, info.WeaponColor);
		
		playerObject.ModelId 				= ClothesModelID;
		playerObject.SetModel(EModelCtrlType.eModelCtrl_Original, ClothesModelID);
		playerObject.WeaponModelId			= WeaponModelID;
		
		// copy buff
//		foreach(XBuff buffCache in m_MPBuffCache)
//		{
//			playerObject.BuffOper.AddBuff(buffCache.BuffId, buffCache.BuffLevel, buffCache.BuffLayer, true, buffCache.RemainLife);
//		}
		
		playerObject.Appear();
		playerObject.StartFight();
		
	}
	
	public void CreatePet(PB_SingleObjectData info)
	{
		XBattlePosition pos = XBattlePosition.Create(info.Pos);
		EBattleGroupType	e 	= pos.Group;
		uint	BattlePos		= pos.Position;
		
		Vector3 dir = GetFighterDir(e);
		
		m_BattleObjects[(int)e, BattlePos] = new XBattlePet();
		XBattleObject battlePet = m_BattleObjects[(int)e, BattlePos];
		battlePet.curBattlePos				= pos;
        battlePet.Direction					= dir;
        battlePet.Position					= GetFighterPos(e, (int)BattlePos);
		battlePet.Name						= info.Name;		
		battlePet.MaxHp						= (int)info.MaxHP;
		battlePet.Hp						= (int)info.MaxHP;
		
		XCfgPetBase petCfg = XCfgPetBaseMgr.SP.GetConfig(info.ClothesID);
		if(petCfg == null)
			return ;
		battlePet.ModelId					= petCfg.ModelId[info.ClassLevel];
		battlePet.SetModel(EModelCtrlType.eModelCtrl_Original, battlePet.ModelId);
        battlePet.Appear();
		battlePet.StartFight();
	}
	
	public void CreateMonster(PB_SingleObjectData info)
	{
		XBattlePosition pos = XBattlePosition.Create(info.Pos);
		EBattleGroupType	e 	= pos.Group;
		uint	BattlePos		= pos.Position;
		
		Vector3 dir = GetFighterDir(e);
		
		XCfgMonsterBase MonsterCfg = XCfgMonsterBaseMgr.SP.GetConfig(info.ClothesID);
		if(MonsterCfg == null)
			return ;
		
		
		XBattleMonster battleMonster = new XBattleMonster();
		m_BattleObjects[(int)e, BattlePos] 	= battleMonster;
		battleMonster.curBattlePos			= pos;
		battleMonster.ModelId 	= MonsterCfg.ModelId;
		battleMonster.SetModel(EModelCtrlType.eModelCtrl_Original,battleMonster.ModelId);
		battleMonster.Direction	= dir;
		battleMonster.Position	= GetFighterPos(e,(int)BattlePos);
		battleMonster.Scale		= MonsterCfg.Zoom;
		battleMonster.Name		= MonsterCfg.Name;
		//battleMonster.Level		= (int)AdjustLevel(cfgMon.Level);
		
		//HP调整
		//uint realHP = AdjustHP(cfgMon.Level,(uint)cfgMon.MaxHp);
		battleMonster.MaxHp		= (int)info.MaxHP;
		battleMonster.Hp		= (int)info.MaxHP;
		battleMonster.Appear();
		battleMonster.StartFight();
		
	}
	
	public void showMainPlayer(EBattleGroupType e,uint BattlePos, XMainPlayer mainPlayer)
	{
		if (e < EBattleGroupType.eBattleGroup_Begin || e >= EBattleGroupType.eBattleGroup_End)
			return;

		Vector3 dir = GetFighterDir(e);

		// 玩家站位
		m_BattleObjects[(int)e, BattlePos]	= new XBattlePlayer();
		XBattleObject playerObject 			= m_BattleObjects[(int)e, BattlePos];
		playerObject.ModelId 				= m_MPModelIdCache;
		playerObject.WeaponModelId			= m_MpWeaponModelIdCache;
		playerObject.Position				= GetFighterPos(e, (int)BattlePos);
		playerObject.Direction				= dir;  
		playerObject.Name					= mainPlayer.Name;
		playerObject.MaxHp					= mainPlayer.MaxHp;
		playerObject.Hp 					= mainPlayer.MaxHp;
		playerObject.Level					= mainPlayer.Level;
		
		// copy buff
		foreach(XBuff buffCache in m_MPBuffCache)
		{
			playerObject.BuffOper.AddBuff(buffCache.BuffId, buffCache.BuffLevel, buffCache.BuffLayer, true, buffCache.RemainLife);
		}
		
		playerObject.Appear();
		playerObject.StartFight();

		// 宠物站位
		foreach (XPet pet in XLogicWorld.SP.PetManager.AllPet)
		{
            if (null == pet)
                continue;

            int batPos = (int)pet.BattlePos;
            if (XUtil.NotInRange<int>(batPos, 1, XBattleDefine.BATTLE_POS_COUNT))
                continue;
			
			m_BattleObjects[(int)e, batPos] = new XBattlePet();
			XBattleObject battlePet = m_BattleObjects[(int)e, batPos];
            battlePet.Direction					= dir;
            battlePet.Position					= GetFighterPos(e, batPos);
			battlePet.Name						= pet.Name;
			battlePet.ModelId					= pet.ModelId;
			battlePet.MaxHp						= pet.MaxHp;
			battlePet.Hp						= pet.Hp;
            battlePet.Appear();
			battlePet.StartFight();
		}

		//--4>TODO: 设置法宝
	}
	
	public void ShowGrid()
	{
		DestoryGrid();
		
		int num = 0;
		for(EBattleGroupType type = EBattleGroupType.eBattleGroup_Begin; type < EBattleGroupType.eBattleGroup_End; type++)
		{
			for(int i = 0; i < XBattleDefine.BATTLE_POS_COUNT; i++)
			{
				if(i == 0)
					continue;
				
				mEffectList[num] = new XU3dEffect(GRID_EFFECT_ID);
				mEffectList[num].Position 	= GetFighterPos(type,i);
				mEffectList[num].Direction	= GetFighterDir(type);
				//mEffectList[num].Scale		= new Vector3(0.666f,0.666f,0.6666f);
				num++;
			}
		}
	}
	
	public void DestoryGrid()
	{
		for(int n = 0; n < (int)EBattleGroupType.eBattleGroup_End * XBattleDefine.BATTLE_POS_COUNT; n++)
		{
			if(mEffectList[n] != null)
				mEffectList[n].Destroy();
		}
	}
	
	private uint AdjustLevel(uint monsterLevel)
	{
		XCfgClientScene cfgClient = XCfgClientSceneMgr.SP.GetConfig(XLogicWorld.SP.SubSceneManager.SubSceneID);
		if(cfgClient == null)
			return 0;
		
		int sceneLevel = (int)XLogicWorld.SP.SubSceneManager.CurSelLevel;
		if(sceneLevel == 0)
			sceneLevel	= 1;
		
		uint realLevel = cfgClient.LevelNeed[sceneLevel - 1];
		return realLevel;
	}
	
	private uint AdjustHP(uint monsterLevel,uint maxHP)
	{
		XCfgClientScene cfgClient = XCfgClientSceneMgr.SP.GetConfig(XLogicWorld.SP.SubSceneManager.SubSceneID);
		if(cfgClient == null)
			return 0;
		
		int sceneLevel = (int)XLogicWorld.SP.SubSceneManager.CurSelLevel;
		if(sceneLevel == 0)
			sceneLevel	= 1;
		
		uint realLevel = cfgClient.LevelNeed[sceneLevel - 1];
		
		XCfgLevelAdjust oldLevelAdjust = XCfgLevelAdjustMgr.SP.GetConfig(monsterLevel);
		XCfgLevelAdjust NewLevelAdjust = XCfgLevelAdjustMgr.SP.GetConfig(realLevel);
		
		if(oldLevelAdjust == null || NewLevelAdjust == null)
			return maxHP;
		
		float HPRate = (float)NewLevelAdjust.Life[sceneLevel - 1] / oldLevelAdjust.Life[sceneLevel - 1];
		return (uint)(HPRate * maxHP);
	}
	
	private uint AdjustHPByOwnerLevel(uint monsterLevel,uint maxHP,uint ownerLevel)
	{		
		XCfgLevelAdjust oldLevelAdjust = XCfgLevelAdjustMgr.SP.GetConfig(monsterLevel);
		XCfgLevelAdjust NewLevelAdjust = XCfgLevelAdjustMgr.SP.GetConfig(ownerLevel);
		
		if(oldLevelAdjust == null || NewLevelAdjust == null)
			return maxHP;
		
		float HPRate = (float)NewLevelAdjust.Life[0] / oldLevelAdjust.Life[0];
		return (uint)(HPRate * maxHP);
	}
	
	public void showMonsterGroup(EBattleGroupType e, uint monsterGroupID)
	{
		if (e < EBattleGroupType.eBattleGroup_Begin || e >= EBattleGroupType.eBattleGroup_End)
		{
			return;
		}

		XCfgMonsterGroup cfgGroup = XCfgMonsterGroupMgr.SP.GetConfig(monsterGroupID);
		if (cfgGroup == null)
		{
			Log.Write(LogLevel.ERROR, "[ERROR] showMonsterGroup failed, no group config find for id:{0}", monsterGroupID);
			return;
		}

		Vector3 dir = GetFighterDir(e);
		for (int batPos = 1; batPos < (int)XBattleDefine.BATTLE_POS_COUNT; ++batPos)
		{
			XCfgMonsterBase cfgMon = XCfgMonsterBaseMgr.SP.GetConfig(cfgGroup.MonID[batPos]);
			if (cfgMon != null)
			{
				XBattleMonster battleMonster = new XBattleMonster();
				m_BattleObjects[(int)e, batPos] 	= battleMonster;			   
				battleMonster.ModelId 	= cfgMon.ModelId;
				battleMonster.Direction	= dir;
				battleMonster.Position	= GetFighterPos(e,batPos);
				battleMonster.Scale		= cfgMon.Zoom;
				battleMonster.Name		= cfgMon.Name;
				battleMonster.Level		= (int)AdjustLevel(cfgMon.Level);
				
				//HP调整
				uint realHP = AdjustHP(cfgMon.Level,(uint)cfgMon.MaxHp);
				battleMonster.MaxHp		= (int)realHP;
				battleMonster.Hp		= (int)realHP;
				battleMonster.Appear();
				battleMonster.StartFight();
			}
			else
			{
				m_BattleObjects[(int)e, batPos]	= null;
			}
		}

		//--4>TODO: 设置法宝

	}
	
	public XBattleMonster AddMonster(XBattlePosition pos,uint monsterID,uint ownerLevel)
	{
		EBattleGroupType	e 	= pos.Group;
		uint batPos				= pos.Position;
		
		Vector3 dir = GetFighterDir(e);
		XCfgMonsterBase cfgMon = XCfgMonsterBaseMgr.SP.GetConfig(monsterID);
		if (cfgMon != null)
		{
			XBattleMonster battleMonster = new XBattleMonster();
			m_BattleObjects[(int)e, batPos] 	= battleMonster;
			battleMonster.ModelId 	= cfgMon.ModelId;
			battleMonster.Direction	= dir;
			battleMonster.Position	= GetFighterPos(e,(int)batPos);
			battleMonster.Name		= cfgMon.Name;
			battleMonster.Level		= (int)ownerLevel;
			
			//HP调整
			uint realHP = AdjustHPByOwnerLevel(cfgMon.Level,(uint)cfgMon.MaxHp,ownerLevel);
			battleMonster.MaxHp		= (int)realHP;
			battleMonster.Hp		= (int)realHP;
			battleMonster.Appear();
			battleMonster.StartFight();
			
			return battleMonster;
		}
		else
		{
			m_BattleObjects[(int)e, batPos]	= null;
		}
		
		return null;
	}
	
	public static Vector3 GetFighterDir(EBattleGroupType t)
	{
		Vector3 dir = Vector3.zero;
		switch (t)
		{
			case EBattleGroupType.eBattleGroup_Left:
				dir = new Vector3(0f, 90f, 0f);
				break;
			case EBattleGroupType.eBattleGroup_Right:
				dir = new Vector3(0f, -90f, 0f);
				break;
			default:
				break;
		}
		return dir;
	}
	
	public static Vector3 GetFighterPos(EBattleGroupType e, int standIndex)
	{
		if (standIndex < 0 || standIndex >= XBattleDefine.BATTLE_POS_COUNT)
		{
			return Vector3.zero;
		}
	
		Vector3 pos = XLogicWorld.SP.SceneManager.GetCenterPos();
		int xdiff = e == EBattleGroupType.eBattleGroup_Left ? -1 : 1;
	
		if (standIndex == XBattleDefine.BATTLE_MAGIC_POS)
		{
			//--4>TODO: 法宝站位特殊
		}
		else
		{
			float defaultUnitSize = BATTLE_UNIT_SIZE;
			XCfgGameParam param = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Unit_Dist);
			if(param != null)
				defaultUnitSize	= param.Value;
			
			float GroupOffset = BATTLE_GROUP_OFFSET;
			XCfgGameParam param1 = XCfgGameParamMgr.SP.GetConfig((uint)GameParam_Define.Battle_Dist_Offset);
			if(param1 != null)
				GroupOffset	= param1.Value;
			
			pos.x += xdiff * (GroupOffset + ((standIndex-1) / 3) * defaultUnitSize);
			pos.z += -1 * defaultUnitSize * (((standIndex - 1) % 3) - 1);
		}        
		XLogicWorld.SP.SceneManager.FixTerrainHeight(ref pos);
		return pos;
	}
	#endregion
	
	public void Reset()
	{
		mCurRoundID	= 0;
		mCurBattleRoundList.Clear();
		mActive		= false;
		Log.Write(LogLevel.INFO,"mCurBattleRoundList Clear");
	}
	
	public void DestroyAllModel()
	{
		for(int i = (int)EBattleGroupType.eBattleGroup_Begin; i < (int)EBattleGroupType.eBattleGroup_End; i++)
		{
			for(int j = 0; j < XBattleDefine.BATTLE_POS_COUNT; j++)
			{
				if(m_BattleObjects[i,j] != null)
					m_BattleObjects[i,j].DisAppear();
				
				m_BattleObjects[i,j] = null;
			}
		}
	}
	
	public void Start()
	{
		mCurRoundID	= 0;
		mActive		= true;	
		
		Display();
	}
	
	public void End()
	{
		
	}
	
	public XBattleObject GetBattleObject(XBattlePosition pos)
	{
		if(pos == null)
		{
			Log.Write(LogLevel.ERROR,"1111");
			return null;
		}			
		
		return GetBattleObject(pos.Group,pos.Position);
	}
	
	public XBattleObject GetBattleObject(EBattleGroupType e, uint pos)
	{
		if (e < EBattleGroupType.eBattleGroup_Begin || e >= EBattleGroupType.eBattleGroup_End)
		{
			return null;
		}
		if (pos < 0 || pos >= XBattleDefine.BATTLE_POS_COUNT)
		{
			return null;
		}
		return m_BattleObjects[(int)e, pos];
	}
	
	public BattleRound AddBattleRound()
	{
		BattleRound round = new BattleRound(this);
		mCurBattleRoundList.Add(round);
		return round;
	}
	
	public void Display()
	{
		if(!mActive)
			return ;
		
		if(mCurBattleRoundList.Count > mCurRoundID)
		{
			mCurBattleRoundList[mCurRoundID].Display();
		}
		
		for(EBattleGroupType e = EBattleGroupType.eBattleGroup_Begin; e< EBattleGroupType.eBattleGroup_End; e++)
		{
			for(uint pos = 0; pos < XBattleDefine.BATTLE_POS_COUNT; pos++)
			{
				XBattleObject bo = GetBattleObject(e, pos);
				if(null == bo) continue;
				bo.Breathe();
			}
		}
	}
	
	public void AddBattleRoundID()
	{
		mCurRoundID++;
	}
	
	public bool IsEnd()
	{
		return mCurRoundID >= mCurBattleRoundList.Count;
	}	
	
	public void DeadSoul( XBattleObject deadObj )
	{
		int deadGroup = (int)deadObj.curBattlePos.Group;
		
		if(deadGroup == (int)m_MainPlayerPos.Group )// is my self group dead?
			return;
		
		//m_BattleObjects
		for( int i=1;i<XBattleDefine.BATTLE_POS_COUNT;i++ )
		{
		  	XBattleObject obj  =	m_BattleObjects[1 - deadGroup,i];
			
			if(null == obj)
				continue;
			
			if(EObjectType.OtherPlayer != obj.ObjectType )
				continue;
			
			((XBattlePlayer)obj ).SoulValue += 1; //add soul
			
			//main player add soul value
			if(m_MainPlayerPos.Group == obj.curBattlePos.Group &&
				m_MainPlayerPos.Position == obj.curBattlePos.Position
				)
			{
				Vector3 soulPos = deadObj.ObjectModel._getSkeleton(ESkeleton.eHeadCenter).position;
				
				XEventManager.SP.SendEvent(EEvent.FightHead_Show_Soul_Fire,soulPos ,((XBattlePlayer)obj ).SoulValue );
			}
			
		}
	}
	
	private bool mActive	= false;
	private int  mCurRoundID;
	public XBattleObject[,] m_BattleObjects 		= new XBattleObject[(int)EBattleGroupType.eBattleGroup_End, XBattleDefine.BATTLE_POS_COUNT];
	private List<BattleRound>	mCurBattleRoundList	= new List<BattleRound>();
	private List<XBuff> m_MPBuffCache = null;
	private uint m_MPModelIdCache = 0;
	private uint m_MpWeaponModelIdCache = 0;
}
