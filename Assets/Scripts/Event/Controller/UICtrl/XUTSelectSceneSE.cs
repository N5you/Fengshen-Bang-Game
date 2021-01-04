using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using XGame.Client.Packets;
class XUTSelectSceneSE : XUICtrlTemplate<XSelectSceneSE>
{
	private class SceneInfo
	{
		public SceneInfo (int nIndex, uint levelID, string strName, bool bLock, int sceneLevel, int starLevel, string bossSpriteID)
		{
			Index = nIndex;
			LevelID = levelID;
			Name = strName;
			Lock = bLock;
			SceneLevel = sceneLevel;
			StarLevel = starLevel;
			BossSpriteID = bossSpriteID;
		}
		
		public int Index;
		public uint LevelID;
		public string Name;
		public bool Lock;
		public int  SceneLevel;
		public int  StarLevel;
		public string BossSpriteID;
	}
	public override void OnHide()
	{
		base.OnHide ();
		m_HoldScenInfo.Clear ();
		LogicUI.CancelAutoEnterScene();
	}
	
	public override void OnShow()
	{
		base.OnShow ();
		UpdateScene (XSelectSceneSE.CurSelectSceneType);
		updateStar (XSelectSceneSE.CurSelectSceneType);
		CheckAutoEnterScene();
	}
		
	public override void OnCreated(object arg)
	{
		base.OnCreated (arg);
		UpdateScene (XSelectSceneSE.CurSelectSceneType);
	}
	
	private List<SceneInfo> m_HoldScenInfo = new List<SceneInfo> ();
	public static XTransPoint TransPoint = null;
	
	public XUTSelectSceneSE ()
	{
		XEventManager.SP.AddHandler (AddScene, EEvent.SelectScene_AddScene);
		//XEventManager.SP.AddHandler (SetName, EEvent.SelectScene_SetName);
		XEventManager.SP.AddHandler (CheckLevel, EEvent.Update_Level);
		XEventManager.SP.AddHandler (CheckMission, EEvent.Mission_ActionState);
		XEventManager.SP.AddHandler (OnClickNav, EEvent.SelectScene_SelectSceneType);
		XEventManager.SP.AddHandler(ChooseScene, EEvent.SelectScene_ChooseScene);
	}
	
	public void UpdateScene(int sceneType)
	{
		for (int i=0; i<m_HoldScenInfo.Count; i++) {
			SceneInfo info = m_HoldScenInfo [i];
			LogicUI.AddScene (info.Index, info.LevelID, info.Name, info.Lock, info.SceneLevel, info.StarLevel, info.BossSpriteID, sceneType);
		}
	}
	
	private void AddScene(EEvent evt, params object[] args)
	{
		XCfgLevelEntry entryInfo = (XCfgLevelEntry)args [0];
		if (m_HoldScenInfo.Count != 0) {
			m_HoldScenInfo.Clear ();
		}
		
		for (int cnt = 0; cnt != entryInfo.Level.Length; ++cnt) {
			uint levelID = entryInfo.Level [cnt];
			int Index = cnt;
			XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig (levelID);
			if (cfg == null)
				continue;
		
			int lastSceneLevel = (int)XLogicWorld.SP.SubSceneManager.GetCurSceneLevel (cfg.Index);
			int starLevel = (int)XLogicWorld.SP.SubSceneManager.GetSceneStar (cfg.Index, (ECopySceneLevel)XSelectSceneSE.CurSelectSceneType);
			//以后更改服务器星星规则后删除
			if (starLevel >= 3)
				starLevel = 3;
		
			bool Lock = XLogicWorld.SP.SubSceneManager.IsUnLock (cfg.Index);
			string Name = cfg.Name;
			string bossSpriteName = cfg.BossSpriteName;
		
			if (null == LogicUI) {
				m_HoldScenInfo.Add (new SceneInfo (Index, levelID, Name, Lock, lastSceneLevel, starLevel, bossSpriteName));
				continue;
			}
			LogicUI.AddScene (Index, levelID, Name, Lock, lastSceneLevel, starLevel, bossSpriteName, XSelectSceneSE.CurSelectSceneType);
			m_HoldScenInfo.Add (new SceneInfo (Index, levelID, Name, Lock, lastSceneLevel, starLevel, bossSpriteName));
			updateStar (XSelectSceneSE.CurSelectSceneType);
		}
	}
	
	private void CheckLevel(EEvent evt, params object[] args)
	{ 
		//TODO 根据等级检测当前关卡开启情况
	}
	
	private void CheckMission(EEvent evt, params object[] args)
	{
		//TODO 根据任务情况更新面板
		if (LogicUI != null) {
			SortedList<uint,MissionMessage> m_actionMissionList = (SortedList<uint,MissionMessage>)args [0];
			foreach (KeyValuePair<uint,MissionMessage> s in m_actionMissionList) {
				if (s.Value.m_bMissionCompleted) {
					//LogicUI.CheckHasMission();
				}
			}
		}
	}
	
	private void updateStar(int sceneyType)
	{
		int rewardStarNum = 0;
		int MaxStarNum = 0;
		int countSceneyType = 0;
		if (sceneyType == 1) {
			for (int cnt = 0; cnt != m_HoldScenInfo.Count; ++cnt) {
				if (m_HoldScenInfo [cnt].SceneLevel == sceneyType) {
					int tempstar = (int)XLogicWorld.SP.SubSceneManager.GetSceneStar (m_HoldScenInfo [cnt].LevelID, (ECopySceneLevel)sceneyType);
					if (tempstar >= 3)
						tempstar = 3;
					rewardStarNum += tempstar;
					++countSceneyType;
				}
			}
		}
		else if (sceneyType == 2) {
				for (int cnt = 0; cnt != m_HoldScenInfo.Count; ++cnt) {
					if (m_HoldScenInfo [cnt].SceneLevel == sceneyType) {
						int tempstar = (int)XLogicWorld.SP.SubSceneManager.GetSceneStar (m_HoldScenInfo [cnt].LevelID, (ECopySceneLevel)sceneyType);
						if (tempstar >= 3)
							tempstar = 3;
						rewardStarNum += tempstar;
						++countSceneyType;
					}
				}
			}
		MaxStarNum = countSceneyType * 3;
		LogicUI.SetRewardStar ((uint)rewardStarNum, (uint)MaxStarNum);
	}
	
	private void OnClickNav(EEvent evt, params object[] args)
	{
		int sceneType = (int)args [0];
		LogicUI.IsVisibleAll (false);
		for (int cnt = 0; cnt != m_HoldScenInfo.Count; ++cnt) {
			LogicUI.AddScene (cnt, m_HoldScenInfo [cnt].LevelID, m_HoldScenInfo [cnt].Name, m_HoldScenInfo [cnt].Lock, m_HoldScenInfo [cnt].SceneLevel, m_HoldScenInfo [cnt].StarLevel, m_HoldScenInfo [cnt].BossSpriteID, sceneType);
		}
		updateStar (sceneType);
	}

	private void ChooseScene(EEvent evt, params object[] args)
	{
		if(null != TransPoint)
		{
			TransPoint.OnChooseScene((int)(args[0]),(int)(args[1]));
		}
	}
	
	public void CheckAutoEnterScene()
	{
		//是否自动寻路过来
		XMainPlayerStateNavigate navigateState = XLogicWorld.SP.MainPlayer.GetCurrentState() as XMainPlayerStateNavigate;
		if(navigateState == null || navigateState.navInfo == null)
			return;
		else
		{
			if(XNewPlayerGuideManager.SP.getGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_FightScense) == false)
				return;
			if(TransPoint == null)
				return;
			XCfgLevelEntry entryInfo = TransPoint.EntryInfo;
			for (int cnt = 0; cnt != entryInfo.Level.Length; ++cnt) 
			{
				uint levelID = entryInfo.Level [cnt];
				XCfgClientScene cfgClientScene = XCfgClientSceneMgr.SP.GetConfig (levelID);
				if (cfgClientScene == null)
					continue;
				if(levelID == navigateState.navInfo.expectDuplicateID)
				{
					LogicUI.StartAutoEnterScene((uint)cnt);
				}
			}
		}
	}
}