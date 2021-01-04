using System;
using XGame.Client.Packets;
using Google.ProtocolBuffers;
using UnityEngine;
using resource;

class XLogicWorld
{
    #region Propertys
    public Login LoginProc { get; private set; }
    public XNetworkManager NetManager { get; private set; }
    public XObjectManager ObjectManager { get; private set; }
    public XSceneManager SceneManager { get; private set; }
    public XSubSceneManager SubSceneManager { get; private set; }
    public SkillManager SkillManager { get; private set; }
    public XQuestManager QuestManager { get; private set; }
    public XPetManager PetManager { get; private set; }
    public XAuctionManager AuctionManager { get; private set; }
    public XMainPlayer MainPlayer { get; private set; }
    public XMailManager MailManager { get; private set; }
    public XShanHTManager ShanHT { get; private set; }
    #endregion

    private static XLogicWorld m_this = new XLogicWorld();
    public static XLogicWorld SP { get { return m_this; } }	
	
	private int TempSceneId;
	private ESceneType TempSceneType;

    private XLogicWorld()
    {
        m_this = this;

        LoginProc           = new Login();
        NetManager          = new XNetworkManager();
        ObjectManager       = new XObjectManager();
        SceneManager        = new XSceneManager();
        SubSceneManager     = new XSubSceneManager();
        SkillManager        = new SkillManager();
        QuestManager        = new XQuestManager();
        PetManager          = new XPetManager();
        AuctionManager      = new XAuctionManager();
        MainPlayer          = new XMainPlayer(0);
        MailManager         = new XMailManager();
        
		ShanHT = new XShanHTManager();

        //--4>TODO: 此处加载所有配置文𐀤»¥后考虑是否放到进入游戏过程中或者需要用到的时候加
        //this.initAllConfigMgr();
		
		SceneManager.Init();
    }

//    private bool initAllConfigMgr()
//    {
//        return (XCfgMonsterBaseMgr.SP.Init(StaticResourceManager.SP.TextMonsterBaseConfig)
//                && XCfgMonsterGroupMgr.SP.Init(StaticResourceManager.SP.TextMonsterGroupConfig)
//                && XStringManager.SP.Init()
//                && XCfgSceneInfoMgr.SP.Init(StaticResourceManager.SP.TextAllSceneListConfig)
//                && XCfgClientSceneMgr.SP.Init(StaticResourceManager.SP.TextClientSceneConfig)
//                && XCfgLevelEntryMgr.SP.Init(StaticResourceManager.SP.TextLevelEntryConfig)
//                && XCfgPetBaseMgr.SP.Init(StaticResourceManager.SP.TextPetBaseConfig)
//                && XCfgItemMgr.SP.Init(StaticResourceManager.SP.TextItemBaseConfig)
//                && XCfgMountMgr.SP.Init(StaticResourceManager.SP.TextMountConfig)
//                && XCfgWorldMapMgr.SP.Init(StaticResourceManager.SP.TextWorldMapConfig)
//                && XCfgStringMgr.SP.Init(StaticResourceManager.SP.TxtCfgString)
//                && XCfgLastNameMgr.SP.Init(StaticResourceManager.SP.TxtCfgLastName)
//                && XCfgMaleFirstNameMgr.SP.Init(StaticResourceManager.SP.TxtCfgMaleFirstName)
//                && XCfgFemaleFirstNameMgr.SP.Init(StaticResourceManager.SP.TxtCfgFemaleFirstName)
//                && XCfgPlayerBaseMgr.SP.Init(StaticResourceManager.SP.TextPlayerBaseConfig)
//                && XCfgBagSpaceMgr.SP.Init(StaticResourceManager.SP.TextBagSpaceConfig)
//                && XCfgBankSpaceMgr.SP.Init(StaticResourceManager.SP.TextBankSpaceConfig)
//                && XCfgArmourGroupMgr.SP.Init(StaticResourceManager.SP.TextArmourGroupConfig)
//                && XCfgAttrIDMgr.SP.Init(StaticResourceManager.SP.TextAttrIDConfig)
//                && XCfgStrengthenMgr.SP.Init(StaticResourceManager.SP.TextStrengthenConfig)
//                && XCfgDecomposeMgr.SP.Init(StaticResourceManager.SP.TextItemDecomposeConfig)
//                && XCfgGameParamMgr.SP.Init(StaticResourceManager.SP.TextGameParamConfig)
//                && XCfgBuffBaseMgr.SP.Init(StaticResourceManager.SP.TxtBuffBase)
//                && XCfgBuffLevelMgr.SP.Init(StaticResourceManager.SP.TxtBuffLevel)
//                && XCfgNpcBaseMgr.SP.Init(StaticResourceManager.SP.TextNpcConfig)
//                && XCfgSceneNpcInfoMgr.SP.Init(StaticResourceManager.SP.TextNpcSceneConfig)
//                && XCfgMissionMgr.SP.Init(StaticResourceManager.SP.TextMissionConfig)
//                && XCfgMissionsDynamicMgr.SP.Init(StaticResourceManager.SP.TextMissionDynamicConfig)
//                && XCfgMissionsTypeMgr.SP.Init(StaticResourceManager.SP.TextMissionTypeConfig)
//                && XCfgClassLvUpMgr.SP.Init(StaticResourceManager.SP.TxtClassLvUp)
//                && XCfgAttrMarkMgr.SP.Init(StaticResourceManager.SP.TxtAttrMark)
//                && XCfgProductCareerMgr.SP.Init(StaticResourceManager.SP.TxtProductCareer)
//                && XCfgGatherObjectMgr.SP.Init(StaticResourceManager.SP.TxtGatherObject)
//                && XCfgGatherObjectBornMgr.SP.Init(StaticResourceManager.SP.TxtGatherObjectBorn)
//                && XCfgFormulaMgr.SP.Init(StaticResourceManager.SP.TxtFormula)
//                && XCfgTrainMgr.SP.Init(StaticResourceManager.SP.TxtTrain)
//                && XCfgPlayerLvUpMgr.SP.Init(StaticResourceManager.SP.TxtPlayerLvUp)
//                && XCfgLevelAdjustMgr.SP.Init(StaticResourceManager.SP.TextLevelAdjustConfig)
//                && XCfgShopItemMgr.SP.Init(StaticResourceManager.SP.TxtShopItemConfig)
//                && FeatureUnLockMgr.SP.Init(StaticResourceManager.SP.TxtFeatureIDConfig)
//                && XPKChallengeMgr.SP.Init(StaticResourceManager.SP.TxtPKChallengeConfig)
//                && XPKAwardMgr.SP.Init(StaticResourceManager.SP.TxtPKAwardConfig)
//                && XCfgMoneyTreeMgr.SP.Init(StaticResourceManager.SP.TxtMoneyTree)
//                && XTitleMgr.SP.Init(StaticResourceManager.SP.TxtTitleConfig)
//                && XCfgOnlineRewardMgr.SP.Init(StaticResourceManager.SP.TxtOnlineRewardConfig)
//				&& XCfgGrowthTargetMgr.SP.Init(StaticResourceManager.SP.TxtPractiseConfig)
//                );
//    }

    ~XLogicWorld()
    {
        OnDestroy();
    }

    #region Functions For App
    public void Init()
    {
        SkillManager.Init();
        XActionIcon.InitColor();

        XMissionManager.SP.init();
		XCutSceneMgr.SP.init();
		XAudioManager.SP.init();
    	XHillSeaBookManager.SP.init();
	}

    public void Breathe()
    {
        MainPlayer.Breathe();
        ObjectManager.Breathe();
        NetManager.Breathe();
        SceneManager.Breathe();
        SubSceneManager.Breathe();
        XProductManager.SP.Breathe();
		XZhanYaoLuManager.SP.Breathe();
    }

    public void LoadScene(int nSceneId)
    {
        LoadScene(nSceneId, ESceneType.NormalScene);
    }

    public void LoadScene(int nSceneId, ESceneType t)
    {	
		TempSceneId		= nSceneId;
		TempSceneType	= t;		
		PreLoadLoadingUI();
       
    }

    public void OnLevelLoaded(int id, string name, ESceneType st)
    {
		CoroutineManager.StartCoroutine(SceneManager.OnLevelLoaded(id, name, st));
    }
	
	public void PreLoadLoadingUI()
	{
		XResourcePanel uiCutsceneResource = XResourceManager.GetResource(XResourcePanel.ResTypeName,(uint)(EUIPanel.eLoadSceneUI)) as XResourcePanel;
		if(uiCutsceneResource.IsLoadDone() )
		{
			actLoadScene(null);
		}
		else
		{
			uiCutsceneResource.CurPanel 	= EUIPanel.eLoadSceneUI;
			uiCutsceneResource.ParentTF 	= LogicApp.SP.UIRoot.UIAnchors [(int)EUIAnchor.eCenter];
			XResourceManager.StartLoadResource(XResourcePanel.ResTypeName,(uint)(EUIPanel.eLoadSceneUI));
			uiCutsceneResource.ResLoadEvent +=new XResourceBase.LoadCompletedDelegate(actLoadScene);
		}
	}
	
	public void actLoadScene(DownloadItem item)
	{
		MainPlayer.OnBeginLoadLevel(TempSceneId, TempSceneType);
		ObjectManager.RemoveAllObjectNotLogic();
		CoroutineManager.StartCoroutine(SceneManager._loadScene(TempSceneId, TempSceneType));
	}	

    // 只在游戏退出的时候执行这个操    
    public void OnDestroy()
    {
        NetManager.DisconnectServer();
        ObjectManager.RemoveAllObject();
        ObjectManager = null;
        SceneManager = null;
        SkillManager = null;
        MainPlayer = null;
    }
	
	public XMainPlayer GetMainPlayer()
	{
		return MainPlayer;
	}
    #endregion
}

