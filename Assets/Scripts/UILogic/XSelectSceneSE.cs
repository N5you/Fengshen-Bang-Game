using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
class XSelectSceneSE : XDefaultFrame
{
	[System.Serializable]
	public class XSelectSceneItem
	{
		private static readonly string[] ExclamationMarkStatus = {"11000786","11000787"};
		private static readonly string[] StarStatus = {"11000788", "11000789"};
		private static readonly string[] LockSpriteStatus = {"11000782", "11000785"};
		private static readonly string[] SelectFBSpriteStatus = {"11000780","11000781","11000781","11000783","11000784","11000784"};
		private static readonly string[] RushFBSpriteStatus = {"11000790","11000791","11000792","11000793","11000794","11000795"};
		private static readonly uint TipsEffectID = 900039;
		public GameObject 		    RootGameObject;
		private UIImageButton 		    RushFB;
		private UIImageButton 		    SelectFB;
		private UILabel 		    FBNameText;
		private UILabel		    FBNameTextSE;
		private UISprite		    FBNameTextSEBackGround;
		private UISprite 		    LockSprite;
		private UISprite 		    BossSprite;
		private UISprite		    ExclamationMark;
		private UISprite[] 		    StarSrpiteArrry = new UISprite[MAX_STARS];
		private int 			    m_nIndex;
		private uint		   	    m_LevelID;
		private bool 			    m_IsLock;
		private bool			    m_IsBossScene;
		private bool			    m_IsMission;
		private int			    m_StarLevel;
		private int			    m_SceneType;  // 1 = General, 2 = Hero;
		private XU3dEffect 		    m_Effect;
		
		public void initData(int nIndex)
		{
			m_IsLock = false;
			m_IsBossScene = false;
			m_IsMission = false;
			m_StarLevel = 0;
			m_SceneType = 1;
			this.m_nIndex = nIndex;
			
			if (RootGameObject == null) {
				Log.Write (LogLevel.ERROR, "XSelectSceneSE, the RootGameObject is Null!");
				return;
			}
			
			FBNameText = RootGameObject.transform.Find ("Lable_FB_Name").GetComponent<UILabel> ();
			FBNameText.text = "";
			
			FBNameTextSE = RootGameObject.transform.Find ("Label_FB_Name2").GetComponent<UILabel> ();
			FBNameTextSEBackGround = RootGameObject.transform.Find ("Sprite_Button_Bg").GetComponent<UISprite> ();
				
			SelectFB = RootGameObject.transform.Find ("IButton_FB_Select").GetComponent<UIImageButton> ();
			RushFB = RootGameObject.transform.Find ("IButton_Enter").GetComponent<UIImageButton> ();
			BossSprite = RootGameObject.transform.Find ("Boss").GetComponent<UISprite> ();
			LockSprite = RootGameObject.transform.Find ("Sprite_Lock").GetComponent<UISprite> ();
			ExclamationMark = RootGameObject.transform.transform.Find ("Sprite_GanTanHao").GetComponent<UISprite> ();
			
			if (StarSrpiteArrry != null) {
				try {
					StarSrpiteArrry [0] = RootGameObject.transform.Find ("Sprite_Xing1").GetComponent<UISprite> ();
					StarSrpiteArrry [1] = RootGameObject.transform.Find ("Sprite_Xing2").GetComponent<UISprite> ();
					StarSrpiteArrry [2] = RootGameObject.transform.Find ("Sprite_Xing3").GetComponent<UISprite> ();
				}
				catch {
					Log.Write (LogLevel.ERROR, "XSelectSceneSE, the index is out of Arrry Range");
				}
			}

			
			if (RushFB != null) {
				UIEventListener listenerRushFBOnClick = UIEventListener.Get (RushFB.gameObject);
				listenerRushFBOnClick.onClick += OnClickRushFB;
			}
			
			if (SelectFB != null) {
				UIEventListener listenerSelectFBOnClick = UIEventListener.Get (SelectFB.gameObject);
				listenerSelectFBOnClick.onClick += OnClickSelectFB;
				listenerSelectFBOnClick.onHover += OnHoverSelectFB;
			}
			
			if (LockSprite != null) {
				UIEventListener listenerLockSpriteOnHover = UIEventListener.Get (LockSprite.gameObject);
				listenerLockSpriteOnHover.onHover += OnHoverLockSprite;
			}
		}

		public void SetItemData(bool isLock, int sceneType, int starLevel, int sceneLevel, bool isBossScene, string strName, uint LevelID, string bossSpriteID)
		{
			this.m_IsLock = isLock;
			this.m_SceneType = sceneType;
			this.m_StarLevel = starLevel;
			this.m_IsBossScene = isBossScene;
			this.FBNameText.text = strName;
			this.FBNameTextSE.text = strName;
			this.m_LevelID = LevelID;
			
			XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig (m_LevelID);
			if (cfg == null) {
				Log.Write (LogLevel.ERROR, "XSelectSceneSE, ClientScene CFG is null!");
				return;
			}
			if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Normal) {
				MissionMessage actionms = XMissionManager.SP.getActionMission (cfg.RequireMissionID);
				if (actionms != null) {
					m_IsMission = true;
				}
				else {
					m_IsMission = false;
				}
			
				//检测是否已通关
				if (XLogicWorld.SP.SubSceneManager.GetSceneStar (LevelID, ECopySceneLevel.eCpSceneLv_Normal) > ECopyScenePassStar.eCpScenePsStar_0 && XLogicWorld.SP.SubSceneManager.GetCurSceneLevel (LevelID) >= ECopySceneLevel.eCpSceneLv_Normal) {
					if (m_IsBossScene)
						this.SetBossVisible (true, bossSpriteID);
					if (actionms != null) {
						if (!actionms.m_bMissionCompleted) {
							createEffect ();
						}
					}
					this.SetFBNameVisible (true);
					this.SetSelectVisible (true, sceneType);
					this.SetEnterFBVisible (true, sceneType);
					this.setStarSprite (this.m_StarLevel);
				}
				else {
					//检测是否满足等级条件，不满足不显示
					if (XLogicWorld.SP.MainPlayer.Level < cfg.RequireLevel)
						return;
				
					//是否存在前置关卡
					if (cfg.PreIndex != 0) {
					
						//判断前置关卡是否已通关
						if (XLogicWorld.SP.SubSceneManager.GetSceneStar (cfg.PreIndex, ECopySceneLevel.eCpSceneLv_Normal) == ECopyScenePassStar.eCpScenePsStar_0) {
							if (XLogicWorld.SP.SubSceneManager.GetCurSceneLevel (cfg.PreIndex) > ECopySceneLevel.eCpSceneLv_Easy) {
								this.SetLockVisible (true, sceneType);
								return;
							}
							return;
						}
					}
				
					//是否需要与任务关联绑定
					if (cfg.RequireMissionID != 0) 
					{
						if (actionms != null) 
						{
							if (!actionms.m_bMissionCompleted) 
							{
								if (m_Effect == null) 
								{
									createEffect ();
								}
								
								GameObject obj = LockSprite.gameObject;
								Vector3 showPos = new Vector3(0, -128, 0);
								XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_FightScense, 
									1, showPos, obj.transform.parent.gameObject);
							}
						}
						else 
						{
							this.SetLockVisible (true, sceneType);
							return;
						}
					}
					if (isBossScene) {
						FBNameTextSE.gameObject.SetActive (true);
						FBNameTextSEBackGround.gameObject.SetActive (true);
						this.SetBossVisible (true, cfg.BossSpriteName);
						this.SetSelectVisible (true, sceneType);
					}
					else {
						FBNameTextSE.gameObject.SetActive (true);
						FBNameTextSEBackGround.gameObject.SetActive (true);
						this.SetExclamationMarkVisible (true);
						this.SetSelectVisible (true, sceneType);
					}
				}
			}
			else if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Hard) {
				
					if (XLogicWorld.SP.SubSceneManager.GetSceneStar (m_LevelID, ECopySceneLevel.eCpSceneLv_Normal) == ECopyScenePassStar.eCpScenePsStar_0) {
						return;
					}
					if (cfg.PreIndex != 0) {
						//前置英雄副本没有通关
						if (ECopyScenePassStar.eCpScenePsStar_0 == XLogicWorld.SP.SubSceneManager.GetSceneStar (cfg.PreIndex, ECopySceneLevel.eCpSceneLv_Hard)) {
							
							if (ECopySceneLevel.eCpSceneLv_Easy < XLogicWorld.SP.SubSceneManager.GetCurSceneLevel (cfg.PreIndex)) {
								this.SetLockVisible (true, sceneType);
								return;
							}
							return;
						}
					}
				
					if (XLogicWorld.SP.SubSceneManager.GetSceneStar (m_LevelID, ECopySceneLevel.eCpSceneLv_Hard) > ECopyScenePassStar.eCpScenePsStar_0) {
						if (m_IsBossScene)
							this.SetBossVisible (true, bossSpriteID);
						this.SetFBNameVisible (true);
						this.SetSelectVisible (true, sceneType);
						this.SetEnterFBVisible (true, sceneType);
						this.setStarSprite (this.m_StarLevel);
					}
					else {
						if (m_IsBossScene)
							this.SetBossVisible (true, bossSpriteID);
						FBNameTextSE.gameObject.SetActive (true);
						FBNameTextSEBackGround.gameObject.SetActive (true);
						this.SetExclamationMarkVisible (true);
						this.SetSelectVisible (true, sceneType);
					}
				
				}
				else {
					UnityEngine.Debug.LogError ("XSelectSceneSE, ECopySceneLevel is: " + (ECopySceneLevel)m_SceneType);
				}
		}
		
		private void SetLockVisible(bool isVisible, int sceneType)
		{
			if (BossSprite != null) {
				NGUITools.SetActive (LockSprite.gameObject, isVisible);
				if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Normal) {
					LockSprite.spriteName = LockSpriteStatus [0];
				}
				else if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Hard) {
						LockSprite.spriteName = LockSpriteStatus [1];
					}
					else {
						Log.Write (LogLevel.ERROR, "XSelectSceneSe, the sceneType is error:" + sceneType.ToString ()); 
					}
			}
		}
		
		private void SetBossVisible(bool isVisible, string strSpriteName)
		{
			if (BossSprite != null) {
				NGUITools.SetActive (BossSprite.gameObject, isVisible);
				BossSprite.spriteName = strSpriteName;
				BossSprite.ResetSize();
			}
		}
		
		private void SetFBNameVisible(bool isVisible)
		{
			if (FBNameText != null) {
				NGUITools.SetActive (FBNameText.gameObject, isVisible);	
			}
		}
		
		private void SetEnterFBVisible(bool isVisible, int sceneType)
		{
			if (RushFB != null) {
				NGUITools.SetActive (RushFB.gameObject, isVisible);
				if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Normal) {
					RushFB.normalSprite = RushFBSpriteStatus [0];
					RushFB.hoverSprite = RushFBSpriteStatus [1];
					RushFB.pressedSprite = RushFBSpriteStatus [2];
					RushFB.target.spriteName = RushFBSpriteStatus [0];
				}
				else if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Hard) {
						RushFB.normalSprite = RushFBSpriteStatus [3];
						RushFB.hoverSprite = RushFBSpriteStatus [4];
						RushFB.pressedSprite = RushFBSpriteStatus [5];
						RushFB.target.spriteName = RushFBSpriteStatus [3];
					}
					else {
						Log.Write (LogLevel.ERROR, "XSelectSceneSe, the sceneType is error:" + sceneType.ToString ()); 
					}
			}
		}

		private void SetSelectVisible(bool isVisible, int sceneType)
		{
			if (SelectFB != null) {
				NGUITools.SetActive (SelectFB.gameObject, isVisible);
				if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Normal) {
					SelectFB.normalSprite = SelectFBSpriteStatus [0];
					SelectFB.hoverSprite = SelectFBSpriteStatus [1];
					SelectFB.pressedSprite = SelectFBSpriteStatus [2];
					SelectFB.target.spriteName = SelectFBSpriteStatus [0];
				}
				else if (sceneType == (int)ECopySceneLevel.eCpSceneLv_Hard) {
						SelectFB.normalSprite = SelectFBSpriteStatus [3];
						SelectFB.hoverSprite = SelectFBSpriteStatus [4];
						SelectFB.pressedSprite = SelectFBSpriteStatus [5];
						SelectFB.target.spriteName = SelectFBSpriteStatus [3];
					}
					else {
						Log.Write (LogLevel.ERROR, "XSelectSceneSe, the sceneType is error:" + sceneType.ToString ()); 
					}
			}
		}
		
		private void SetExclamationMarkVisible(bool isVisible)
		{
			if (ExclamationMark != null) {
				NGUITools.SetActive (ExclamationMark.gameObject, isVisible);
			}
		}

		private void setStarSprite(int starlevel)
		{
			for (int cnt = 0; cnt != MAX_STARS; ++cnt) {
				if (cnt < starlevel)
					this.StarSrpiteArrry [cnt].spriteName = StarStatus [0];
				else
					this.StarSrpiteArrry [cnt].spriteName = StarStatus [1];
				StarSrpiteArrry [cnt].gameObject.SetActive (true);
			}
		}
		
		private void OnClickRushFB(GameObject go)
		{
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectSceneSE);
			XSaoDangManager.SP.ClientSceneID = (int)this.m_LevelID;
			XSaoDangManager.SP.ClientSceneLevel = this.m_SceneType;
			XSaoDangManager.SP.LeftCnt = 1;
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eSaoDang);
			//XEventManager.SP.SendEvent (EEvent.MessageBox, funcOK, null, XStringManager.SP.GetString (139));
		}

		private void OnClickSelectFB(GameObject go)
		{
			Debug.Log (RootGameObject.name.ToString () + ", OK! going to the FB:" + this.FBNameText.text);
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectSceneSE);
			int mHardLevel = this.m_SceneType;
			int mIndex = this.m_nIndex;
			XLogicWorld.SP.MainPlayer.ChangeState ((int)EStateId.esPreEnterScene, mIndex, mHardLevel);
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_FightScense);
		}
		
		private void OnHoverLockSprite(GameObject go, bool isHover)
		{
			if (isHover) {
				string strName = "";
				string strContent = "";
				getTextTips (ref strName, ref strContent, m_SceneType);
				XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipB);
				XEventManager.SP.SendEvent (EEvent.ToolTip_B, strName, "", strContent);	
			}
			else {
				XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
			}
		}
		
		private void OnHoverSelectFB(GameObject go, bool isHover)
		{
			if (isHover) {
				ExclamationMark.spriteName = ExclamationMarkStatus [1];
			}
			else {
				ExclamationMark.spriteName = ExclamationMarkStatus [0];
			}
		}
		
		private void createEffect()
		{
			m_Effect = new XU3dEffect (TipsEffectID, EffectLoadedHandle);
			m_Effect = null;
		}

		private void EffectLoadedHandle(XU3dEffect effect)
		{
			effect.Layer = GlobalU3dDefine.Layer_UI_2D;
			effect.Parent = this.RootGameObject.transform;
			effect.LocalPosition = new Vector3 (0, -16, -50);
			effect.Scale = new Vector3 (1000, 1000, 1);
		}
		
		public void DesEffect()
		{
			if (m_Effect != null) {
				m_Effect.Destroy ();
			}
		}
		
		public void CheckIsMissionComplate()
		{
			XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig (m_LevelID);
			if (cfg != null) {
				MissionMessage actionms = XMissionManager.SP.getActionMission (cfg.RequireMissionID);
				if (actionms != null) {
					createEffect ();
					m_IsMission = true;
				}
			}
		}
		
		private void getTextTips(ref string strName, ref string strTips, int sceneType)
		{
			XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig (m_LevelID);
			if (cfg == null) {
				Log.Write (LogLevel.ERROR, "XSelectSceneSE, getTextTips() ClientScene CFG is null!");
				return;
			}
			strName = string.Format ("[color=00ff00]{0}", cfg.Name);
			
			if ((ECopySceneLevel)sceneType == ECopySceneLevel.eCpSceneLv_Normal) {
				strName += string.Format ("{0}", XStringManager.SP.GetString (284));
			}
			else if ((ECopySceneLevel)sceneType == ECopySceneLevel.eCpSceneLv_Hard) {
					strName += string.Format ("{0}", XStringManager.SP.GetString (285));
				}
			
			strTips = string.Format (XStringManager.SP.GetString (280), cfg.LevelNeed [sceneType - 1]);
			
			if (XLogicWorld.SP.SubSceneManager.GetSceneStar (cfg.PreIndex, (ECopySceneLevel)sceneType) == ECopyScenePassStar.eCpScenePsStar_0) {
				strTips += string.Format ("\n[color=F8924B]{0}", XStringManager.SP.GetString (281));
			}
			if ((ECopySceneLevel)sceneType == ECopySceneLevel.eCpSceneLv_Normal) {
				if (cfg.RequireMissionID != 0) {
					if (!XMissionManager.SP.isActionMission (cfg.RequireMissionID)) {
						strTips += string.Format ("\n[color=F8924B]{0}", XStringManager.SP.GetString (282));
					}
				}
			}
		}
	}
	
	private static readonly int MAX_SEL_SCENE_NUM = 8;
	private static readonly int MAX_STARS = 3;
	private static readonly int DEFUALT_SCENETYPE = 1;
	public static int CurSelectSceneType = DEFUALT_SCENETYPE;
	public UICheckbox[] NavCheckBox = new UICheckbox[2];
	public XSelectSceneItem[] ListItem = new XSelectSceneItem[MAX_SEL_SCENE_NUM];
	public UILabel RewardStarText;
	public UILabel NoItemTips;
	
	public uint AutoEnterIndex { get; set; }
	
	public override bool Init()
	{
		base.Init ();
		NavCheckBox [0].onStateChange += OnActiveGeneral;
		NavCheckBox [1].onStateChange += OnActiveHero;
		
		for (int cnt = 0; cnt != ListItem.Length; ++cnt) {
			ListItem [cnt].initData (cnt);
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		SetRewardStar (0, 0);
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSelectSceneSE);
	}
	
	
	public override void Show()
	{		
		base.Show ();
		IsVisibleAll (false);
	}
	
	public override void Hide()
	{
		base.Hide ();
		for (int cnt = 0; cnt != MAX_SEL_SCENE_NUM; ++cnt) {
			ListItem [cnt].DesEffect ();
		}
		NavCheckBox [0].isChecked = true;
		NavCheckBox [1].isChecked = false;
	}
	
	public void SetRewardStar(uint reawardNum, uint MaxNum)
	{
		if (RewardStarText == null) {
			Log.Write (LogLevel.ERROR, "XSelectSceneItem, RewardStarText is null");
			return;
		}
		RewardStarText.text = string.Format ("{0}/{1}", reawardNum, MaxNum);
	}
	
	

	public void AddScene(int nIndex, uint LevelID, string strName, bool isLock, int sceneLevel, int starLevel, string bossID, int sceneType)
	{
		bool isBoss = true;
		if (bossID == "" || bossID == "0") {
			isBoss = false;
		}
		
		ListItem [nIndex].SetItemData (isLock, sceneType, starLevel, sceneLevel, isBoss, strName, LevelID, bossID);
	}
	
	public void IsVisibleAll(bool isVisible)
	{
		for (int i = 0; i < MAX_SEL_SCENE_NUM; i++) {
			ListItem [i].DesEffect ();
			NGUITools.SetActiveChildren (ListItem [i].RootGameObject, isVisible);
		}
	}
	
	private void OnActiveGeneral(bool state)
	{
		if (state) {
			XEventManager.SP.SendEvent (EEvent.SelectScene_SelectSceneType, ECopySceneLevel.eCpSceneLv_Normal);
		}
	}
	
	private void OnActiveHero(bool state)
	{
		if (state) {
			XEventManager.SP.SendEvent (EEvent.SelectScene_SelectSceneType, ECopySceneLevel.eCpSceneLv_Hard);
			for (int cnt = 0; cnt != MAX_SEL_SCENE_NUM; ++cnt) {
				ListItem [cnt].DesEffect ();
			}
		}
	}
	
	public void NoItemTipsVisible(bool isVisible)
	{
		if (NoItemTips != null) {
			if (NoItemTips.gameObject.activeInHierarchy) {
				NoItemTips.gameObject.SetActive (isVisible);
			}
		}
	}
	
	
	public void AutoEnterScene()
	{
		if(AutoEnterIndex >= 0  && AutoEnterIndex <  MAX_SEL_SCENE_NUM && XLogicWorld.SP.MainPlayer.GetCurrentStateID() == (int)EStateId.esMainPlayerNone)
		{
			XLogicWorld.SP.MainPlayer.ChangeState ((int)EStateId.esPreEnterScene, (int)AutoEnterIndex, 1);
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eSelectSceneSE);
		}
		AutoEnterIndex = 0;
	}
	
	public void  StartAutoEnterScene(uint index)
	{
		AutoEnterIndex = index;
		Invoke("AutoEnterScene",1);
	}
	
	public void  CancelAutoEnterScene()
	{
		CancelInvoke("AutoEnterScene");
		AutoEnterIndex = 0;
	}
}
