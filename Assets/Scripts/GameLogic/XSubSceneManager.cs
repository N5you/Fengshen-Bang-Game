using UnityEngine;
using XGame.Client.Packets;
using System.Collections.Generic;
using System;

public class XSubSceneManager
{
	public uint ClientSceenId
	{
		get{
			return SubSceneID;
		}
		set{
			if(value > 0)
			{
				SubSceneID	= value;
			}
		}
	}
	
	public static int MaxMonsterRoundNum = 6;
	
	
	private SC_CopySceneResult	sceneResultMsg;
	
	private Dictionary<uint,ushort>	mAllSubSceneData	= new Dictionary<uint, ushort>();
	private List<XGameObject>	mRoadFlagList = new List<XGameObject>();
	
	public uint GetSceneID()
	{
		if(clientSceneCfg == null)
			return 0;
		
		return clientSceneCfg.SceneID;
	}
	
	public bool  IsLastOne()
	{
		if(clientSceneCfg == null)
			return false;
		
		if (mCurRound >= 0 && (mCurRound == clientSceneCfg.RoundNum - 1))
			return true;
		
		return false;
	}
	
	public Vector3 GetRebornPos()
	{
		if(clientSceneCfg == null)
			return Vector3.zero;
		
		return clientSceneCfg.RebornPos[mCurRound];
	}
	
	public float GetRebornDir()
	{
		if(clientSceneCfg == null)
			return 0f;
		
		return clientSceneCfg.RebornDir[mCurRound];
	}
	
	public string GetSubSceneName()
	{
		if(clientSceneCfg == null)
			return "";
		
		return clientSceneCfg.Name;
	}
	
	public uint	GetLeftMonster()
	{
		if(clientSceneCfg == null)
			return 0;
		
		return (uint)(clientSceneCfg.RoundNum - mCurRound);
	}
	
	public Vector3 EnterCameraPos
	{
		get
		{
			if(clientSceneCfg == null)
				return Vector3.zero;
			
			return clientSceneCfg.EnterCameraPos;
		}
	}
	
	public Vector3 EnterCameraRot
	{
		get
		{
			if(clientSceneCfg == null)
				return Vector3.zero;
			
			return clientSceneCfg.EnterCameraRot;
		}
	}
	
	public int mCurRound;
	public  uint SubSceneID;
	//当前关卡是否已经通关过了
	public  bool mHasPassThrough;
	public  ECopySceneLevel CurSelLevel	{get;set;}
	XCfgClientScene clientSceneCfg;
	public XMonster	mMonster;
	
	public XMonster GetCurMonster()
	{
		return mMonster;
	}

	public XSubSceneManager()
	{
		Reset();
	}

	public void Reset()
	{
		mCurRound		= 0;
		SubSceneID		= 0;
		clientSceneCfg	= null;
	}  
	
	public void Breathe()
	{		
		XBattleManager.SP.Breathe();
		
		XMonster monster = GetCurMonster();
		if(monster != null && monster.ObjectModel != null && monster.ObjectModel.mainModel != null)
		{
			GameObject go = monster.ObjectModel.mainModel.m_gameObject;
			if(go != null && go != XLogicWorld.SP.MainPlayer.ObjectModel.GetCurMoveTarget())
			{
				XLogicWorld.SP.MainPlayer.ObjectModel.SetMoveTarget(EMoveTarget_Type.EMoveTarget_Object,monster.ObjectModel.mainModel.m_gameObject,Vector3.zero);
			}
			
		}
	}
	
	public void LeaveClientScene()
	{
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoadSceneUI);
		CS_LeaveClientScene.Builder builder = CS_LeaveClientScene.CreateBuilder();
		builder.SetLevelIndex(0);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_LeaveClientScene, builder.Build());
		
		XLogicWorld.SP.MainPlayer.IsInSubScene	= false;
		
		SubSceneID = 0;
	}

	public bool EnterFightScene()
	{		
		//CreateLoadingEffect();
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoadSceneUI);
		XHardWareGate.SP.Lock = true;
		CS_EnterFight.Builder builder = CS_EnterFight.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_EnterFight, builder.Build());		
		BattleDisplayerMgr.SP.CacheMP();
		return true;
	}
	
//	public void OnLevelLoaded(int sceneId, string sceneName, ESceneType sceneType)
//	{	
//		if(ESceneType.ClientScene == sceneType)
//		{			
//			XUICtrlManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_SUB_SCENE);
//			showSceneMonster();
//			showSceneRoadFlag();
//			BattleDisplayerMgr.SP.DestoryGrid();
//			
//			XLogicWorld.SP.MainPlayer.Position	= PrePlayerPos;
//			XLogicWorld.SP.MainPlayer.Direction	= PrePlayerDir;
//			
//			if(GetLeftMonster() == 0)
//			{
//				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eScenePassed);
//			}
//		}
//		else if(ESceneType.FightScene == sceneType)
//		{
//			XUICtrlManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_BATTLE);
//			BattleManager.FighterInit();
//			BattleManager.Start();
//			DestroyRoadFlag();
//			DestoryMonster();
//		}
//		else if(ESceneType.NormalScene == sceneType)
//		{
//			XUICtrlManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_NORMAL);
//			DestroyRoadFlag();
//			DestoryMonster();
//			BattleDisplayerMgr.SP.DestoryGrid();			
//			XTransPoint.NotTrigger();
//			
//		}else if(ESceneType.CutScene == sceneType)
//		{
//			XUICtrlManager.SP.ShowByMode(UI_MODE.UI_MODE_CUTSCENE );
//			DestoryMonster();
//			DestroyRoadFlag();
//			
//		}
//	}
	
	public ECopySceneLevel GetCurSceneLevel(uint sceneID)
	{
		if(mAllSubSceneData.ContainsKey(sceneID))
		{
			ushort curValue = mAllSubSceneData[sceneID];
			int ss = (mAllSubSceneData[sceneID] & (0xf));
			return (ECopySceneLevel)ss;
		}
		
		return ECopySceneLevel.eCpSceneLv_Normal;
	}
	
	public ECopyScenePassStar GetSceneStar(uint sceneID,ECopySceneLevel sceneLevel)
	{
		if(mAllSubSceneData.ContainsKey(sceneID))
		{
			ushort curValue = mAllSubSceneData[sceneID];
			int bitPos = 4 + (int)sceneLevel * 3;
			int ss = ((mAllSubSceneData[sceneID] >> bitPos) & (0x7));
			return (ECopyScenePassStar)ss;
		}
		
		return ECopyScenePassStar.eCpScenePsStar_0;
	}
	
	public ECopyScenePassStar GetCurSceneStar(uint sceneID)
	{
		ECopySceneLevel level = GetCurSceneLevel(sceneID);
		if(mAllSubSceneData.ContainsKey(sceneID))
		{
			ushort curValue = mAllSubSceneData[sceneID];
			int bitPos = 4 + (int)level * 3;
			return (ECopyScenePassStar)((mAllSubSceneData[sceneID] >> bitPos) & (0x7));
		}
		
		return ECopyScenePassStar.eCpScenePsStar_0;
	}
	
	public void SetSceneStar(uint sceneID,ECopySceneLevel sceneLevel,ECopyScenePassStar star)
	{
		if(!mAllSubSceneData.ContainsKey(sceneID))
			return ;
		
		if(GetCurSceneLevel(sceneID) < sceneLevel)
			SetSceneLevel(sceneID,sceneLevel);
		
		int tempSceneLevel = (int)sceneLevel;
		
		int bitPos = 4 +  tempSceneLevel * 3;
		ushort curValue  = (ushort)((mAllSubSceneData[sceneID] & (~(0x7 << bitPos))) | ((int)star << bitPos));
		mAllSubSceneData[sceneID]	= curValue;
		
		if(mAllSubSceneData.ContainsKey(clientSceneCfg.PostIndex))
		{			
			if(sceneLevel >= ECopySceneLevel.eCpSceneLv_Normal)
			{
				if(GetCurSceneLevel(clientSceneCfg.PostIndex) == ECopySceneLevel.eCpSceneLv_Easy)
					SetSceneLevel(clientSceneCfg.PostIndex,ECopySceneLevel.eCpSceneLv_Normal);
			}
		}
	}
	
	public void SetSceneLevel(uint sceneID,ECopySceneLevel sceneLevel)
	{
		mAllSubSceneData[sceneID] = (ushort)(((mAllSubSceneData[sceneID] >> 4) << 4) | (int)sceneLevel);
	}
	
	public bool IsLockPreScene(uint sceneID)
	{
		bool isLock = true;
		
		return isLock;
	}

	public bool IsUnLock(uint sceneID)
	{	
		ECopySceneLevel temp = GetCurSceneLevel(sceneID);
		int tempint = (int)temp;
		bool IsUnLock = temp < ECopySceneLevel.eCpSceneLv_Normal;
		return IsUnLock;
	}
	
	private bool GetHasPassThrough(uint sceneID,ECopySceneLevel csLevel)
	{
		if(GetSceneStar (sceneID, csLevel) == ECopyScenePassStar.eCpScenePsStar_0) 
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	#region Packet Processer
	public void EnterClientScene(SC_EnterClientScene msg)
	{
		this.Reset();
		SubSceneID	= msg.SubSceneID;
		clientSceneCfg = XCfgClientSceneMgr.SP.GetConfig(SubSceneID);
		if(clientSceneCfg == null)
			return ;
		
		XLogicWorld.SP.MainPlayer.IsInSubScene	= true;
		
		mHasPassThrough = GetHasPassThrough(SubSceneID,CurSelLevel);
		
		if(true == msg.HasCutSceneID)
		{
			XCutSceneMgr.SP.playCutScene((uint)msg.CutSceneID , actEnterClientScene);
		}else
		{
			actEnterClientScene();
		}
	}
	
	private void actEnterClientScene()
	{
		XLogicWorld.SP.LoadScene((int)clientSceneCfg.SceneID, ESceneType.ClientScene);
		XLogicWorld.SP.MainPlayer.CacheSceneInitPos	= clientSceneCfg.EnterPos;
		XLogicWorld.SP.MainPlayer.CacheSceneInitDir	= Quaternion.Euler(new Vector3(0,clientSceneCfg.EnterDir,0)).eulerAngles;
		
		//XCameraLogic.SP.AdjustCamera(clientSceneCfg.EnterCameraPos,clientSceneCfg.EnterCameraRot);
	}
	
	public void On_SC_ProcessSceneData(SC_UIntArr msg)
	{
		for(uint i = 0, j = 1; i < msg.DataCount; i++,j+=2)
		{
			uint data = msg.GetData((int)i);
			ushort scene1 = (ushort)(data & (0x0000ffff));
			//ushort scene2 = (ushort)((data & (0xffff000000)) >> 16);
			ushort scene2 = (ushort)(data >> 16);
			mAllSubSceneData.Add(j,scene1);
			mAllSubSceneData.Add(j+1,scene2);
		}
	}
	
	public void On_SC_SetBattleObject(PB_ObjectDataList msg)
	{		
		XBattleManager.SP.SetBattleObject(msg);		
		XBattleManager.SP.battleSceneID = msg.BattleSceneID;
		if(msg.HasPreCutSceneID )
		{
			XCutSceneMgr.SP.playCutScene(msg.PreCutSceneID,actLoadPVE );
		}else
		{
			actLoadPVE();
		}
		
	}
	
	private void actLoadPVE()
	{
		XBattleManager.SP.LoadPVE(XBattleManager.SP.battleSceneID );
	}
	
	public void On_SC_BattleData(SC_BattleData msg)
	{		
		XBattleManager.SP.SetBattleData(msg);
	}

	public void On_SC_CopySceneResult(SC_CopySceneResult msg)
	{
		Log.Write("战斗得分: {0}", msg.StarPoint);
		sceneResultMsg	= msg;	
		
		SetSceneStar(clientSceneCfg.Index,(ECopySceneLevel)msg.SceneLevel,(ECopyScenePassStar)msg.StarPoint);
		XEventManager.SP.SendEvent(EEvent.SubScene_SetResult,clientSceneCfg.Name,sceneResultMsg.StarPoint,
			sceneResultMsg.Exp, sceneResultMsg.ExtExp, sceneResultMsg.SceneLevel);
	}
	
	public void On_SC_SubSceneAwardSel(SC_AwardSel msg)
	{
		PB_ItemInfo pbInfo = msg.AwardItem; 
		XItem temp = new XItem();
		temp.InitFromPB(pbInfo);
		EItemBoxType type;
		ushort index;
		XItemManager.GetContainerType((ushort)pbInfo.Position,out type,out index);
		XLogicWorld.SP.MainPlayer.ItemManager.SetItem(type,index,temp);	
		
		XItem TargetItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((ushort)msg.AwardItem.Position);
		if(!TargetItem.IsEmpty())
		{
			XItemManager.FlyItemAnimation(TargetItem);
			XEquipGetMgr.SP.GetNewEquip(TargetItem);
		}
		
		XEventManager.SP.SendEvent(EEvent.Bag_Update,EUIPanel.eBagWindow);
		
		XEventManager.SP.SendEvent(EEvent.SubScene_AwardSel,pbInfo.Position);
	}
	
	#endregion

	public bool showSceneMonster()
	{
		if (mCurRound >= 0 && mCurRound < clientSceneCfg.RoundNum)
		{
			DestoryMonster();

			Vector3 pos = clientSceneCfg.GroupPos[mCurRound];
			Vector2 dir = new Vector3(0, clientSceneCfg.GroupDir[mCurRound],0);
			XLogicWorld.SP.SceneManager.FixTerrainHeight(ref pos);
		  
			XMonsterAppearInfo info = new XMonsterAppearInfo(pos, dir);
			info.SetMonsterGroupID(clientSceneCfg.GroupID[mCurRound]);
			mMonster = XLogicWorld.SP.ObjectManager.AppearMonster(info);
			mMonster.SetHudVisible(false);
			return true;
		}
		return false;
	}
	
	public void DestoryMonster()
	{
		if(mMonster == null)
			return ;
		
		mMonster.DisAppear();
		mMonster = null;
	}
	
	public void showSceneRoadFlag()
	{
		if(clientSceneCfg == null)
			return ;
		
		DestroyRoadFlag();
		
		for(int i = 0; i < 3;i++)
		{
			if(clientSceneCfg.RoadFlagID[i] == 0)
				continue;
			XGameObject obj = new XGameObject(0);
			//obj.Name		= "RoadFlag" + Convert.ToString(i);
			obj.Position	= clientSceneCfg.RoadFlagPos[i];
			obj.Direction	= new Vector3(0,clientSceneCfg.RoadFlagDir[i],0);
			obj.ModelId		= clientSceneCfg.RoadFlagID[i];
			obj.Appear();
			obj.SetHudVisible(false);
			mRoadFlagList.Add(obj);
		}
	}
	
	public void DestroyRoadFlag()
	{
		if(mRoadFlagList.Count > 0)
		{
			for(int j = 0;j < mRoadFlagList.Count; j++)
			{
				mRoadFlagList[j].DisAppear();
			}
			mRoadFlagList.Clear();
		}
		
		//DestoryMonster();
	}
	
	public bool IsWin{
		get
		{
			return XBattleManager.SP.IsWin;
		}
	}
	
	
	
}
