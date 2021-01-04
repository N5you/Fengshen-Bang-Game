using System;
using XGame.Client.Packets;
using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

public enum EBattleState
{
    Invalid,
    Loading,
    Fighting,
    Leaving,
}

//该对象本身，只做接收消息，设置数据，具体表现都交由BattleDisplayerMgr来处理
//将一场战斗的表现完全与逻辑独立开来，方便录像，以及引擎调用

public class XBattleManager : XSingleton<XBattleManager>
{	
	public enum EBattleType
	{
		EBattleType_NONE,
		EBattleType_PVE,
		EBattleType_PVP,
		EBattleType_PVE_SHANHT,
		EBattleType_Num
	}
	
    public EBattleState BattleState { get; private set; }
	private SC_BattleData		mCurBattleMsg;
	public bool					IsLoadedOver = false;
	private PB_ObjectDataList	mCurObjectData;
	private SC_BattleResult		mCurBattleResult;
	private BATTLE_TYPE			mBattleType = BATTLE_TYPE.BATTLE_TYPE_NONE;
	public bool					IsReadyLeaveFight = false;
	
	public BATTLE_TYPE BattleType{
		get{return mBattleType; }
		set{mBattleType =  value; }
	}
	
	public uint battleSceneID{
		get;
		set;
	}
	
	public XBattleManager()
	{
		XEventManager.SP.AddHandler(ShowFightResult,EEvent.Show_Fight_Result);
		XEventManager.SP.AddHandler(ShowFightResultDirect,EEvent.Show_Fight_Result_Direct);
		XEventManager.SP.AddHandler(FightReplay,EEvent.Show_Fight_Replay);
	}
	
	public void ShowFightResult(EEvent evt, params object[] args)
	{	
		Time.timeScale	= 1.0f;
		if(mCurBattleResult == null)
		{
			Log.Write(LogLevel.WARN,"no battle Result !!");
			return ;
		}
		
		if(mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE || mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE_SHANHT || mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE_ZHANYL
			|| mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE_HILLSEABOOK)
		{
			if(EBattleGroupType.eBattleGroup_Right == mCurBattleResult.Winner)
	        {
	            XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eFightWin);
	        }
	        else
	        {
	           // 失败界面
				XEventManager.SP.SendEvent(EEvent.CopySceneResult_FailLevel,mCurBattleResult.Result);
	           	XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eFightFail);
	        }
		}
		else if(mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVP)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.ePVPResult);
		}
		//mBattleType = EBattleType.EBattleType_NONE;
		
		//Get Item Hint
		if(EBattleGroupType.eBattleGroup_Right == mCurBattleResult.Winner)
		{
			for(int i = 0; i < mCurBattleResult.Bonus.ItemListCount; i++)
			{
				XItem curGetItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByGUID(mCurBattleResult.Bonus.GetItemList(i).ItemGUID);
				XEquipGetMgr.SP.GetNewEquip(curGetItem);
				
			}
		}
	}
	
	public void ShowFightResultDirect(EEvent evt, params object[] args)
	{
		Time.timeScale	= 1.0f;
		BattleDisplayerMgr.SP.DirectBattleOver();
		ShowFightResult(EEvent.Show_Fight_Result,null);
	}
	
	public void FightReplay(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFightWin);
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFightFail);
		BattleDisplayerMgr.SP.DestroyAllModel();
		BattleDisplayerMgr.SP.ShowGrid();
		Start();
	}
	
	public void Start()
	{
		BattleState = EBattleState.Fighting;
		_SetObjectData();
		if(_SetBattleData())
			BattleDisplayerMgr.SP.Start();
	}	

    public void Breathe()
    {
        if (BattleState != EBattleState.Fighting)
            return;
		
		BattleDisplayerMgr.SP.Display();
	}

    public void Reset()
    {
        BattleState = EBattleState.Invalid;
		IsLoadedOver	= false;
		BattleDisplayerMgr.SP.Reset();
    }
	
	public void SetBattleObject(PB_ObjectDataList msg)
	{
		mCurObjectData	= msg;
	}

    public void SetBattleData(SC_BattleData msg)
    {		
		mCurBattleMsg	= msg;
		if(IsLoadedOver)
		{
			if(_SetBattleData())
				BattleDisplayerMgr.SP.Start();
		}	
    }
	
	private void _SetObjectData()
	{
		if(mCurObjectData == null)
			return ;
		
		for(int i = 0; i < mCurObjectData.ObjectDataListCount; i++)
		{
			PB_SingleObjectData data = mCurObjectData.GetObjectDataList(i);
			if(data == null)
				continue;
			
			EBattleData_ObjectType objectType = data.ObjectType;
			switch(objectType)
			{
			case EBattleData_ObjectType.eBD_Monster:
				BattleDisplayerMgr.SP.CreateMonster(data);
				break;
			case EBattleData_ObjectType.eBD_Pet:
				BattleDisplayerMgr.SP.CreatePet(data);
				break;
			case EBattleData_ObjectType.eBD_Player:
				BattleDisplayerMgr.SP.CreatePlayer(data);
				break;
			}
		}
	}
	
	private bool _SetBattleData()
	{
		Log.Write(LogLevel.INFO,"_SetBattleData");
		if(mCurBattleMsg == null)
		{
			IsLoadedOver	= true;
			Log.Write(LogLevel.INFO,"_SetBattleData mCurBattleMsg == null");
			return false;
		}
		
		BattleDisplayerMgr.SP.Reset();
		BattleDisplayerMgr.SP.SetBloodValue(mCurBattleMsg.LeftBloodValue,mCurBattleMsg.RightBloodValue);
		BattleDisplayerMgr.SP.LeftBattleValue	= (int)mCurBattleMsg.LeftBattleValue;
		BattleDisplayerMgr.SP.RightBattleValue	= (int)mCurBattleMsg.RightBattleValue;

		XEventManager.SP.SendEvent(EEvent.FightHead_Show_LBlood_Value,1.0f);
		XEventManager.SP.SendEvent(EEvent.FightHead_Show_RBlood_Value,1.0f);
		XEventManager.SP.SendEvent(EEvent.FightHead_Show_Battle_Value,BattleDisplayerMgr.SP.LeftBattleValue,BattleDisplayerMgr.SP.RightBattleValue);
		
		
		for(int i = 0; i < mCurBattleMsg.BattleRoundCount - 1; i++)
		{
			PB_BattleRound round = mCurBattleMsg.GetBattleRound(i);
			BattleRound logicRound = BattleDisplayerMgr.SP.AddBattleRound();
			// Round的buff表现设置
			logicRound.InitBuffDisplayMulti(round.BuffEffectArrList);
			
			//每回合的士气设置
			for(int j = 0; j < round.MoralesCount; j++)
			{
				logicRound.AddMoraleData(round.GetMorales(j).Pos,round.GetMorales(j).IsFull);
			}
			//每回合单人行为
			for(int m = 0; m < round.ActionsCount; m++)
			{
				PB_OneBattleAction action = round.GetActions(m);
				BattleAction LogicAction = logicRound.AddBattleAction();
				
				// Action的buff表现设置
				LogicAction.InitBuffDisplayMulti(action.BuffEffectArrList);
				
				//单次属性
				LogicAction.AttackBattlePos	= XBattlePosition.Create(action.Attacker);
				LogicAction.ActionRound		= action.ActionRound;
				if(action.HasAutoHP)
					LogicAction.AutoHP			= action.AutoHP;
				if(action.HasIsBeStriked)
				{
					BattleAction beforceAction = logicRound.GetBattleAcion(logicRound.GetActionNum()-2) as BattleAction;
					BattleAction beforceAction1 = logicRound.GetBattleAcion(logicRound.GetActionNum()-1) as BattleAction;
					beforceAction1.IsStriker		= true;					
					beforceAction.IsWillBeStriked	= true;
					
					LogicAction.IsBeStriked		= action.IsBeStriked;
					
				}
					
				if(action.HasIsMoraleAttack)
				{
					LogicAction.IsMoraleAttack	= action.IsMoraleAttack;
				}
				
				//技能数据设置
				PB_SkillResult skillResult 	= action.SkillResult;
				XSkill DisplaySkill 		= LogicAction.mCurSkill;
				XBattlePosition targetPos 	= XBattlePosition.Create(skillResult.MainTarget);
				LogicAction.MainTargetPos	= targetPos;				
				DisplaySkill.IsLucky		= skillResult.IsLucky;
				DisplaySkill.Attacker		= BattleDisplayerMgr.SP.GetBattleObject(LogicAction.AttackBattlePos);
				DisplaySkill.MainTarget		= BattleDisplayerMgr.SP.GetBattleObject(LogicAction.MainTargetPos);
				DisplaySkill.mTargetList.Add(DisplaySkill.MainTarget);
				DisplaySkill.TargetBattlePos= LogicAction.MainTargetPos;
				DisplaySkill.SkillID		= skillResult.IdLevel;
				DisplaySkill.AddDamageEvent(1,LogicAction.Action_DamageHandle);
				DisplaySkill.InitBuffDisplayMulti(skillResult.BuffEffectArrList);
				
				
				for(int b = 0; b < skillResult.EffectDataCount; b++)
				{
					PB_One_EffectData effectData = skillResult.GetEffectData(b);
					SkillEffectData_Base skillEffectBase = LogicAction.AddSkillEffectData(effectData.EffectType);
					skillEffectBase.CurSkill	= DisplaySkill;
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_COM_DAMAGE)
					{
						SkillEffectData_ComDamage comDamage = skillEffectBase as SkillEffectData_ComDamage;
						if(comDamage == null)
							continue;
						
						if(effectData.HasDamageResult)
						{
							PB_OneTargetDamage	oneDamage = effectData.DamageResult.DirectTarge;
							comDamage.SetMainTargetDamage(oneDamage.TargetPos,oneDamage.Flag,oneDamage.DamageValue);
							for(int v = 0; v < effectData.DamageResult.OtherTargetCount; v++)
							{
								PB_OneTargetDamage otherDamage = effectData.DamageResult.GetOtherTarget(v);
								comDamage.AddOtherTargetDamage(otherDamage.TargetPos,otherDamage.Flag,otherDamage.DamageValue);
								
								XCharacter other = BattleDisplayerMgr.SP.GetBattleObject(XBattlePosition.Create(otherDamage.TargetPos));
								if(other != null)
									DisplaySkill.mTargetList.Add(other);
							}
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_ADD_STATE)
					{
						SkillEffectData_AddState stateEffect = skillEffectBase as SkillEffectData_AddState;
						if(stateEffect == null)
							continue;
						
						if(effectData.HasAddStateResult)
						{							
							for(int x = 0; x < effectData.AddStateResult.StateIDCount; x++)
							{
								stateEffect.AddState(effectData.AddStateResult.StateIDList[x],effectData.AddStateResult.StateLevelList[x],effectData.AddStateResult.TargetPosList[x]);
							}
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_DEL_STATE)
					{
						SkillEffectData_DisState stateEffect = skillEffectBase as SkillEffectData_DisState;
						if(stateEffect == null)
							continue;
						if(effectData.HasDisStateResult)
						{
							for(int x = 0; x < effectData.DisStateResult.StateIDCount; x++)
							{
								stateEffect.AddState(effectData.DisStateResult.StateIDList[x],effectData.DisStateResult.TargetPosList[x]);
							}
						}						
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_REVIVE)
					{
						SkillEffectData_Revive reviveData = skillEffectBase as SkillEffectData_Revive;
						if(reviveData == null)
							continue;
						if(effectData.HasReviveResult)
						{
							for(int t = 0; t < effectData.ReviveResult.CurHPCount; t++)
							{								
								reviveData.AddReviveData(effectData.ReviveResult.CurHPList[t],effectData.ReviveResult.CurAngerList[t],
									effectData.ReviveResult.TargetPosList[t]);
							}
							
						}						
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_SuckBlood)
					{
						SkillEffectData_SuckBlood SuckBloodData = skillEffectBase as SkillEffectData_SuckBlood;
						if(SuckBloodData == null)
							continue;
						if(effectData.HasSuckBloodResult)
						{
							SuckBloodData.SetAttackPos(LogicAction.AttackBattlePos);
							PB_OneSuckBloodData	oneDamage = effectData.SuckBloodResult.DirectTarget;
							SuckBloodData.SetMainTargetSuckBlood(oneDamage.TargetPos,oneDamage.Flag,oneDamage.DamageValue,oneDamage.SuckBloodValue);
							for(int v = 0; v < effectData.SuckBloodResult.OtherTargetCount; v++)
							{
								PB_OneSuckBloodData otherDamage = effectData.SuckBloodResult.GetOtherTarget(v);
								SuckBloodData.AddOtherTargetSuckBlood(otherDamage.TargetPos,otherDamage.Flag,otherDamage.DamageValue,otherDamage.SuckBloodValue);
							}
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_SUMMON)
					{
						SkillEffectData_Summon SummonData = skillEffectBase as SkillEffectData_Summon;
						if(SummonData == null)
							continue;
						
						if(effectData.HasSummonResult)
						{
							SummonData.MonsterPos	= XBattlePosition.Create(effectData.SummonResult.Pos);
							SummonData.MonsterID	= effectData.SummonResult.MonsterID;
							SummonData.OwnerLevel	= (uint)DisplaySkill.Attacker.Level;
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_MORALE)
					{
						SkillEffectData_Morale MoraleData = skillEffectBase as SkillEffectData_Morale;
						if(MoraleData == null)
							continue;
						
						if(effectData.HasMoraleResult)
						{
							
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_CRI)
					{
						SkillEffectData_CRI criData = skillEffectBase as SkillEffectData_CRI;
						if(criData == null)
							continue;
						
						if(effectData.HasCRIData)
						{
							criData.SetAttackPos(LogicAction.AttackBattlePos);
							PB_OneCRIData	oneDamage = effectData.CRIData.DirectTarget;
							criData.SetMainTargetCRI(oneDamage.TargetPos,oneDamage.Flag,oneDamage.DamageValue,oneDamage.IsCRI > 0);
							for(int v = 0; v < effectData.CRIData.OtherTargetCount; v++)
							{
								PB_OneCRIData otherDamage = effectData.CRIData.GetOtherTarget(v);
								criData.AddOtherTargetCRI(otherDamage.TargetPos,otherDamage.Flag,otherDamage.DamageValue,otherDamage.IsCRI > 0);
							}
						}
					}
					if(effectData.EffectType == ECOM_EFFECT_TYPE.COM_EFFECT_MultiAttack)
					{
						SkillEffectData_MultiAttack MultiAttackData = skillEffectBase as SkillEffectData_MultiAttack;
						if(MultiAttackData == null)
							continue;
						
						if(effectData.HasMultiAttackData)
						{
							PB_OneMultiAttack	oneDamage = effectData.MultiAttackData.DirectTarget;
							MultiAttackData.SetTargetDamage(oneDamage.TargetPos,oneDamage.Flag,oneDamage.DamageValueList);
							
							//oneDamage.DamageValueList
							
							//MultiAttackData.targetPos	= XBattlePosition.Create(effectData.MultiAttackData.TargetPos);
							//MultiAttackData.Flag		= effectData.MultiAttackData.Flag;
							//DisplaySkill.ClearDamageList();
							DisplaySkill.IsMultiAttack	= true;
							for(int v = 0; v < effectData.MultiAttackData.OtherTargetCount; v++)
							{
								PB_OneMultiAttack otherDamage = effectData.MultiAttackData.GetOtherTarget(v);
								MultiAttackData.SetTargetDamage(otherDamage.TargetPos,otherDamage.Flag,otherDamage.DamageValueList);
								XCharacter other = BattleDisplayerMgr.SP.GetBattleObject(XBattlePosition.Create(otherDamage.TargetPos));
								if(other != null)
									DisplaySkill.mTargetList.Add(other);
							}
							int damageCnt = oneDamage.DamageValueCount;
							for(int j = 0; j < damageCnt; j++)
							{
								DisplaySkill.AddDamageEvent(j+2,MultiAttackData.HandleMultiDamage);
							}
						}
					}
				}
				
				DisplaySkill.InitSkillDisplay();
				
				//如果是反击的话，在此增加一个行为，被反击者返回
				if(action.HasIsBeStriked)
				{
					BattleAction_RunBack NextAction = logicRound.AddBattleRunBack();
					NextAction.AttackBattlePos		= XBattlePosition.Create(skillResult.MainTarget);
					
				}
			}
		}
		
		return true;
	}
	
	public void On_SC_BattleResultPVP(SC_BattleResult msg)
	{
		mCurBattleResult	= msg;
		mBattleType			= BATTLE_TYPE.BATTLE_TYPE_PVP;
        Log.Write("Result:{0}, WinGroup:{1}, HasBonus:{2}", msg.Result.ToString(), msg.Winner.ToString(), msg.HasBonus.ToString());
		
		if(mCurBattleResult.Winner == EBattleGroupType.eBattleGroup_Right)
			IsWin	= true;
		else
			IsWin	= false;
		
		if(mCurBattleResult.HasBonus)
		{
			XLogicWorld.SP.MainPlayer.GameMoney	+= mCurBattleResult.Bonus.GameMoney;
			XEventManager.SP.SendEvent(EEvent.PVPResult_Reset);
			XEventManager.SP.SendEvent(EEvent.PVPResult_Set,mCurBattleResult.Result,mCurBattleResult.Bonus.GameMoney,mCurBattleResult.Bonus.HonourValue,IsWin);
		}
	}
	

    public void On_SC_BattleResultPVE(SC_BattleResult msg)
    {       
		mCurBattleResult	= msg;
		mBattleType			= msg.BattleType;
		
        Log.Write("Result:{0}, WinGroup:{1}, HasBonus:{2}", msg.Result.ToString(), msg.Winner.ToString(), msg.HasBonus.ToString());
		
		if(mCurBattleResult.Winner == EBattleGroupType.eBattleGroup_Right)
			IsWin	= true;
		else
			IsWin	= false;
		
		if(mCurBattleResult.HasBonus && IsWin)
		{
			for(int i = 0; i < mCurBattleResult.Bonus.ItemListCount; i++)
			{
				PB_ItemInfo pbInfo = mCurBattleResult.Bonus.GetItemList(i); 
				XItem temp = new XItem();
				temp.InitFromPB(pbInfo);
				EItemBoxType type;
				ushort index;
				XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
				XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);
			}
			
			if(mCurBattleResult.Bonus.ItemListCount > 0)
				XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
			
			 // 胜利界面	
			XEventManager.SP.SendEvent(EEvent.CopySceneResult_Win_Reset);
			XEventManager.SP.SendEvent(EEvent.CopySceneResult_WinLevel,mCurBattleResult.Bonus.BonusExp,(uint)mCurBattleResult.Result,mCurBattleResult.Bonus.GameMoney);
			for(int i = 0; i < mCurBattleResult.Bonus.ItemListCount; i++)
			{
				PB_ItemInfo info = mCurBattleResult.Bonus.GetItemList(i);
				XEventManager.SP.SendEvent(EEvent.CopySceneResult_Win_SetItem,i,info.Position);
			}
		}
		
		if(mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE_SHANHT)
		{				 // 山河图战斗信息界面
			XEventManager.SP.SendEvent(EEvent.ShanHe_Update_BattleInfo,mCurBattleResult);
		}
		else if(mBattleType == BATTLE_TYPE.BATTLE_TYPE_PVE_ZHANYL)
		{
			XEventManager.SP.SendEvent(EEvent.ZYL_BattleEnd,IsWin);
		}

	}
	
	public void On_SC_BattleResultShanHT(SC_BattleResult msg)
	{
		mCurBattleResult	= msg;
		mBattleType			= msg.BattleType;
		
        Log.Write("Result:{0}, WinGroup:{1}, HasBonus:{2}", msg.Result.ToString(), msg.Winner.ToString(), msg.HasBonus.ToString());
		
		if(mCurBattleResult.Winner == EBattleGroupType.eBattleGroup_Right)
			IsWin	= true;
		else
			IsWin	= false;
		
	
		if(IsWin && mCurBattleResult.HasBonus)
		{			
			for(int i = 0; i < mCurBattleResult.Bonus.ItemListCount; i++)
			{
				PB_ItemInfo pbInfo = mCurBattleResult.Bonus.GetItemList(i); 
				XItem temp = new XItem();
				temp.InitFromPB(pbInfo);
				EItemBoxType type;
				ushort index;
				XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
				XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);
			}
			
			if(mCurBattleResult.Bonus.ItemListCount > 0)
				XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
			
			 // 战斗胜利界面	
			XEventManager.SP.SendEvent(EEvent.CopySceneResult_Win_Reset);
			XEventManager.SP.SendEvent(EEvent.CopySceneResult_WinLevel,mCurBattleResult.Bonus.BonusExp,(uint)mCurBattleResult.Result,mCurBattleResult.Bonus.GameMoney);
			for(int i = 0; i < mCurBattleResult.Bonus.ItemListCount; i++)
			{
				PB_ItemInfo info = mCurBattleResult.Bonus.GetItemList(i);
				XEventManager.SP.SendEvent(EEvent.CopySceneResult_Win_SetItem,i,info.Position);
			}
		}
		 // 山河图战斗信息界面
		XEventManager.SP.SendEvent(EEvent.ShanHe_Update_BattleInfo,mCurBattleResult);
	}
	
	public void LeaveFightScenePVP()
	{
		if(ESceneType.NormalScene == XLogicWorld.SP.SceneManager.PreSceneType)
			XLogicWorld.SP.NetManager.SendEmptyPacket((int)CS_Protocol.eCS_ForceUpdateSceneData);
		
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoadSceneUI);
		LeaveFight();
		XLogicWorld.SP.LoadScene(XLogicWorld.SP.SceneManager.PreSceneId,XLogicWorld.SP.SceneManager.PreSceneType);
		XHardWareGate.SP.Lock = false;
		return ;
	}
	
	public bool LeaveFightScenePVE(bool bWin)
	{
		if(mCurBattleResult == null)
		{
			Log.Write(LogLevel.WARN,"LeaveFightScenePVE no battle Result !!");
			return false;
		}
		
		if (ESceneType.FightScenePVE != XLogicWorld.SP.SceneManager.LoadedSceneType)
		{
			return false;
		}
		//在副本中 离开战斗场景
		if(ESceneType.ClientScene == XLogicWorld.SP.SceneManager.PreSceneType || 
			ESceneType.CutScene == XLogicWorld.SP.SceneManager.PreSceneType ||
			ESceneType.FightCutScene == XLogicWorld.SP.SceneManager.PreSceneType)
		{
			IsReadyLeaveFight	= true;		
			if (bWin)
			{
				bool bHasCutScene = mCurBattleResult.HasSufCutScene;
				uint uiCutSceneID = (uint)mCurBattleResult.SufCutScene;		
				//LeaveFight();
				if(bHasCutScene )
				{
					XCutSceneMgr.SP.playCutScene(uiCutSceneID,actLeaveFightScenePVE);
				}else
				{
					XLogicWorld.SP.MainPlayer.CacheSceneInitPos	= XLogicWorld.SP.MainPlayer.Position;
					XLogicWorld.SP.MainPlayer.CacheSceneInitDir	= XLogicWorld.SP.MainPlayer.Direction;
					
					XLogicWorld.SP.LoadScene((int)XLogicWorld.SP.SubSceneManager.GetSceneID(),ESceneType.ClientScene);
				}	
				XLogicWorld.SP.SubSceneManager.mCurRound++;
			}
			else
			{
				//LeaveFight();			
				XLogicWorld.SP.LoadScene((int)XLogicWorld.SP.SubSceneManager.GetSceneID(),ESceneType.ClientScene);
				
				XLogicWorld.SP.MainPlayer.CacheSceneInitPos	= XLogicWorld.SP.SubSceneManager.GetRebornPos();
				XLogicWorld.SP.MainPlayer.CacheSceneInitDir	= new Vector3(0,XLogicWorld.SP.SubSceneManager.GetRebornDir(),0);
				
				//XLogicWorld.SP.MainPlayer.Position	= XLogicWorld.SP.SubSceneManager.GetRebornPos();
				//XLogicWorld.SP.MainPlayer.Direction = new Vector3(0,XLogicWorld.SP.SubSceneManager.GetRebornDir(),0);			
			}
		}//在普通场景中 离开战斗场景(山河图 挑战用到)
		else if(ESceneType.NormalScene == XLogicWorld.SP.SceneManager.PreSceneType)
		{
			IsReadyLeaveFight	= true;
			
			XLogicWorld.SP.NetManager.SendEmptyPacket((int)CS_Protocol.eCS_ForceUpdateSceneData);
				
			//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoadSceneUI);
			//LeaveFight();
			XLogicWorld.SP.LoadScene(XLogicWorld.SP.SceneManager.PreSceneId,XLogicWorld.SP.SceneManager.PreSceneType);
		}
		XHardWareGate.SP.Lock = false;
		return true;
	}
	
	private void actLeaveFightScenePVE()
	{
		XLogicWorld.SP.LoadScene((int)XLogicWorld.SP.SubSceneManager.GetSceneID(),ESceneType.ClientScene);
	}
	
    public void LoadPVE(uint sceneID)
    {
        this.Reset();
		BattleState = EBattleState.Loading;		
		
		XLogicWorld.SP.LoadScene((int)sceneID, ESceneType.FightScenePVE);
    }

    public void FighterInit()
    {		
		XLogicWorld.SP.MainPlayer.Visible = false;
		BattleDisplayerMgr.SP.SetCamera();	
		BattleDisplayerMgr.SP.ShowGrid();
    }
	
	public void LeaveFight()
	{
		XLogicWorld.SP.MainPlayer.Visible = true;
		mCurBattleMsg	= null;
		mCurBattleResult= null;
		XCameraLogic.SP.Detach();
		XCameraLogic.SP.ChangeFOV(35);
		XLogicWorld.SP.MainPlayer.AttachMainCamera();
		XCameraLogic.SP.Scroll(0.3f, true);
		BattleDisplayerMgr.SP.Reset();
		BattleDisplayerMgr.SP.DestroyAllModel();
	}
	
	public bool IsWin {get;set;}

}
