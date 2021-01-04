using System;
using UnityEngine;

public class XSceneDataBase
{
	public XSceneDataBase()
	{
		SceneMgr	= XLogicWorld.SP.SceneManager;		
	}
	
	protected XSceneManager SceneMgr;
	public virtual void OnLoaded(int levelId, string levelName)
	{
		if(SceneMgr != null)
			SceneMgr.SetTerrainRoad();
		
		XHardWareGate.SP.InitHardWareState();
	}
	
	public virtual void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public virtual void OnLeaveScene()
	{
		
	}
}

public class XSceneDataCamera : XSceneDataBase
{
	private Vector3 m_camPosition = Vector3.zero;
	private Quaternion m_camRotation = Quaternion.identity;
	private float m_fScrollValue = 0.0f;
	
	protected void StoreCamera()
	{	
		m_camPosition = XCameraLogic.SP.mainCamera.transform.localPosition;
		m_camRotation = XCameraLogic.SP.mainCamera.transform.localRotation;
		m_fScrollValue= XCameraLogic.SP.WheelDelta;
	}
	
	protected void RestoreCamera()
	{
		XCameraLogic.SP.mainCamera.transform.localPosition = m_camPosition;
		XCameraLogic.SP.mainCamera.transform.localRotation = m_camRotation;
		XCameraLogic.SP.WheelDelta 						   = m_fScrollValue;
	}
	
}

public class XSceneData_CharScene : XSceneDataBase
{
	public override void OnLoaded(int levelId, string levelName)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eCharOper);
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_CHARACTER);
		XCameraLogic.SP.SetCamera(XCharacterOperationUI.SelCharCameraPos,new Vector3(0,0,0));
		XCameraLogic.SP.ChangeFOV(XCharacterOperationUI.SelCharCameraFOV);
		
		Cursor.visible	= true;
	}
}


public class XSceneData_Normal : XSceneDataCamera
{
	
	
	public XSceneData_Normal()
	{
		
	}
	
	public override void OnLoaded(int levelId, string levelName)
	{
		base.OnLoaded(levelId,levelName);
		if(SceneMgr == null)
			return ;
		
		SceneMgr.ShowTransPoint();
		SceneMgr.ShowGatherObject();
		XTransPoint.NotTrigger();
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_NORMAL);
		Cursor.visible	= true;
		
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_PVE_SHANHT == XBattleManager.SP.BattleType )
		{
			XUTShanHT ctrlSH = XUIManager.SP.GetUIControl(EUIPanel.eShanHT) as XUTShanHT;
			ctrlSH.AddLastBattleInfo();
		}
		else if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_PVP == XBattleManager.SP.BattleType)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eXianDH);
		}
		else if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_PVE_ZHANYL == XBattleManager.SP.BattleType)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eZhanYaoLu);
		}
		XBattleManager.SP.BattleType  = XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE;
		
		switch(SceneMgr.PreSceneType )
		{
		case ESceneType.ClientScene:
			
			RestoreCamera();
			
			break;
		}
		
		XPlayHintMgr.SP.ShowUI();
		XDailyPlaySignMgr.SP.ShowUI();
		XEquipGetMgr.SP.ShowAllHintEquip();
		XMissionManager.SP.CheckNavigateToRefer();
		
		XNewPlayerGuideManager.SP.StartFirstForceGuide();
		
		if ( !DailySignManager.SP.GetCanFinish() )
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eDailySign);
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{
		XUIManager.SP.HideAllUI();
		StoreCamera();
	}
	
}

public class XSceneData_SubScene : XSceneDataCamera
{
	private XSubSceneManager SubSceneMgr;
	
	Vector3 m_camPosition = Vector3.zero;
	Quaternion m_camRotation = Quaternion.identity;
	float m_fScrollValue = 0.0f;
	
	public XSceneData_SubScene()
	{
		SubSceneMgr	= XLogicWorld.SP.SubSceneManager;
	}	
	
	public override void OnLoaded(int levelId, string levelName)
	{
		if(SubSceneMgr == null)
			return ;
		
		base.OnLoaded(levelId,levelName);
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_SUB_SCENE);
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(false);
		SubSceneMgr.showSceneMonster();
		SubSceneMgr.showSceneRoadFlag();
		BattleDisplayerMgr.SP.DestoryGrid();
		Cursor.visible	= true;	
		
		if(SubSceneMgr.GetLeftMonster() == 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eScenePassed);
		}
		else
		{
			XLogicWorld.SP.MainPlayer.NavigateKill(SubSceneMgr.SubSceneID );
		}
		switch(SceneMgr.PreSceneType )
		{
		case ESceneType.FightScenePVE:
		case ESceneType.FightScenePVP:
			
			RestoreCamera();
			break;
		}
		
		XEquipGetMgr.SP.ShowAllHintEquip();
		
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{
		if(SubSceneMgr == null)
			return ;
		SubSceneMgr.DestroyRoadFlag();
		SubSceneMgr.DestoryMonster();
		BattleDisplayerMgr.SP.DestoryGrid();
		XLogicWorld.SP.MainPlayer.ObjectModel.CancelMoveTarget();
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(true);
		XUIManager.SP.HideAllUI();
		
		StoreCamera();
	}
}

public class XSceneData_FightScenePVE : XSceneDataBase
{
	private XSubSceneManager SubSceneMgr;
	
	public XSceneData_FightScenePVE()
	{
		SubSceneMgr	= XLogicWorld.SP.SubSceneManager;		
	}
	
	public override void OnLoaded(int levelId, string levelName)
	{
		base.OnLoaded(levelId,levelName);
		XCameraLogic.SP.IsEnableCameraCollide	= false;
		XHardWareGate.SP.LockMouse				= true;		
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_STATIC_BATTLE);
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(false);
		XBattleManager.SP.FighterInit();
		Log.Write(LogLevel.DEBUG,"XSceneData_FightScenePVE");
		XBattleManager.SP.Start();
		Cursor.visible	= true;
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{		
		BattleDisplayerMgr.SP.DestoryGrid();		
		XCameraLogic.SP.IsEnableCameraCollide	= true;
		XHardWareGate.SP.LockMouse				= false;
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(true);
	}
}

public class XSceneData_FightScenePVP : XSceneDataBase
{	
	private XSubSceneManager SubSceneMgr;
	
	public XSceneData_FightScenePVP()
	{
		SubSceneMgr	= XLogicWorld.SP.SubSceneManager;		
	}
	
	public override void OnLoaded(int levelId, string levelName)
	{
		base.OnLoaded(levelId,levelName);
		XCameraLogic.SP.IsEnableCameraCollide	= false;
		XHardWareGate.SP.LockMouse				= true;
		Cursor.visible	= true;
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(false);
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{
		XCameraLogic.SP.IsEnableCameraCollide	= true;
		XHardWareGate.SP.LockMouse				= false;
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(true);
	}
}

public class XSceneData_CutScene : XSceneDataBase
{
	private XSubSceneManager SubSceneMgr;
	
	public XSceneData_CutScene()
	{
		SubSceneMgr	= XLogicWorld.SP.SubSceneManager;
	}
	
	public override void OnLoaded(int levelId, string levelName)
	{
		base.OnLoaded(levelId,levelName);
		
		XCutScenePanel.m_activeTalk = true;
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_CUTSCENE );
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(false);
		XLogicWorld.SP.MainPlayer.Visible = false;
		SubSceneMgr.DestoryMonster();
		SubSceneMgr.DestroyRoadFlag();
		Cursor.visible	=  true;
		
		XCutSceneMgr.SP.start();
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{
		XLogicWorld.SP.MainPlayer.Visible 	= true;	
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(true);
	}
}

public class XSceneData_CutSceneNormal : XSceneDataBase
{
	private XSubSceneManager SubSceneMgr;
	
	public XSceneData_CutSceneNormal()
	{
		SubSceneMgr	= XLogicWorld.SP.SubSceneManager;
	}
	
	public override void OnLoaded(int levelId, string levelName )
	{
		base.OnLoaded(levelId,levelName);
		XCutScenePanel.m_activeTalk = false;
		XUIManager.SP.ShowByMode(UI_MODE.UI_MODE_NORMAL_CUTSCENE );
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(false);
		Cursor.visible	= false;
		XCutSceneMgr.SP.start();
	}
	
	public override void OnLoadedError(int levelId, string levelName)
	{
		
	}
	
	public override void OnLeaveScene()
	{
		XLogicWorld.SP.ObjectManager.SetAllObjectActive(true);
	}
}