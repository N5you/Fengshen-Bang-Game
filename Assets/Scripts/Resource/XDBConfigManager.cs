using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using resource;

public enum EDBConfg_Item
{
	EDBConfg_Item_None			    = 0,
	EDBConfg_Item_BuffBase		    = 1,
	EDBConfg_Item_BuffLevel		    = 2,
	EDBConfg_Item_FeatureUnLock	    = 3,
	EDBConfg_Item_ArmourGroup	    = 4,
	EDBConfg_Item_AttrID		    = 5,
	EDBConfg_Item_BagSpace		    = 6,
	EDBConfg_Item_BankSpace		    = 7,
	EDBConfg_Item_ColorValue	    = 8,
	EDBConfg_Item_Decompose		    = 9,
	EDBConfg_Item_EquipPosRate	    = 10,
	EDBConfg_Item_Item			    = 11,
	EDBConfg_Item_Strengthen	    = 12,
	EDBConfg_Item_Suit			    = 13,
	EDBConfg_Item_Missions		    = 14,
	EDBConfg_Item_MissionsDynamic   = 15,
	EDBConfg_Item_MissionsType	    = 16,
	EDBConfg_Item_MoneyTree		    = 17,
	EDBConfg_Item_LevelAdjust	    = 18,
	EDBConfg_Item_MonsterBase	    = 19,
	EDBConfg_Item_MonsterGroup	    = 20,
	EDBConfg_Item_Mount			    = 21,
	EDBConfg_Item_NpcBase		    = 22,
	EDBConfg_Item_SceneNpcList	    = 23,
	EDBConfg_Item_OnlineReward	    = 24,
	EDBConfg_Item_FemaleFirstName   = 25,
	EDBConfg_Item_FilterWord	    = 26,
	EDBConfg_Item_LastName		    = 27,
	EDBConfg_Item_MaleFirstName	    = 28,
	EDBConfg_Item_String		    = 29,
	EDBConfg_Item_PetBase		    = 30,
	EDBConfg_Item_AttrMark		    = 31,
	EDBConfg_Item_ClassLvUp		    = 32,
	EDBConfg_Item_PlayerBase	    = 33,
	EDBConfg_Item_PlayerLvUp	    = 34,
	EDBConfg_Item_Train			    = 35,
	EDBConfg_Item_Formula		    = 36,
	EDBConfg_Item_GatherObject	    = 37,
	EDBConfg_Item_GatherObjectBorn	= 38,
	EDBConfg_Item_ProductCareer	    = 39,
	EDBConfg_Item_AllSceneList	    = 40,
	EDBConfg_Item_ClientScene	    = 41,
	EDBConfg_Item_GameParam		    = 42,
	EDBConfg_Item_LevelEntry	    = 43,
	EDBConfg_Item_ShopItem		    = 44,
	EDBConfg_Item_SkillBase		    = 45,
	EDBConfg_Item_SkillLevel	    = 46,
	EDBConfg_Item_SkillOper		    = 47,
	EDBConfg_Item_BuffStr		    = 48,
	EDBConfg_Item_HintStr		    = 49,
	EDBConfg_Item_ItemStr		    = 50,
	EDBConfg_Item_WorldMap		    = 51,
	EDBConfg_Item_PKChallenge	    = 52,
	EDBConfg_Item_Title			    = 53,
	EDBConfg_Item_XPKAward		    = 54,
	EDBConfg_Item_Growth_Condition	= 55,
	EDBConfg_Item_Growth_Target		= 56,
	EDBConfg_Item_NewPlayerGuide	= 57,
	EDBConfg_Item_ShanHTBase        = 58,
	EDBConfg_Item_ShanHTLevel       = 59,
	EDBConfg_Item_OperTip	        = 60,
	EDBConfg_Item_AuctionConfig	    = 61,
	EDBConfg_Item_BuyChanllege      = 62,
	EDBConfg_Item_BoxTip            = 63,
	EDBConfg_Item_AddAward          = 64,
	EDBConfg_Item_NickName          = 65,
	EDBConfg_Item_Phiz              = 66,
	EDBConfg_Item_BuyHealth         = 67,
	EDBConfg_Item_MountAttr         = 68,
	EDBConfg_Item_FiendlyLevel      = 69,
	EDBConfg_Item_ShengWangItem     = 70,
	EDBConfg_Item_ShengWangLvl      = 71,
	EDBConfg_Item_Formation         = 72,
	EDBConfg_Item_SevenTarget       = 73,
	EDBConfg_Item_ZhanYaoLu         = 74,
	EDBConfg_Item_LianZhan          = 75,
	EDBConfg_Item_HillSeaBook       = 76,
	EDBConfg_Item_DailySign         = 77,
	EDBConfg_Item_UIRelationShip    = 78,
	EDBConfg_Item_PlayHint          = 79,
	EDBConfg_Item_VipBase           = 80,
	EDBConfg_Item_VipAward	        = 81,
	EDBConfg_Item_VipShow           = 82,
	EDBConfg_Item_DayActivity       = 83,
    EDBConfg_Item_DayAward          = 84,
	EDBConfg_Item_BattleFailGuide    = 85,
	EDBConfg_Item_DailyPlaySign	    = 86
}

public class XDBConfigManager : XSingleton<XDBConfigManager>
{
	SortedList<EDBConfg_Item,IConfigManager>	mConfigList = new SortedList<EDBConfg_Item, IConfigManager>();
	
	public XDBConfigManager()
	{
		
	}
	
	
	
	public void Init()
	{
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BuffBase,XCfgBuffBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BuffLevel,XCfgBuffLevelMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_FeatureUnLock,FeatureUnLockMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ArmourGroup,XCfgArmourGroupMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_AttrID,XCfgAttrIDMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BagSpace,XCfgBagSpaceMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BankSpace,XCfgBankSpaceMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ColorValue,XCfgColorValueMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Decompose,XCfgDecomposeMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_EquipPosRate,XCfgEquipPosRateMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Item,XCfgItemMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Strengthen,XCfgStrengthenMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Suit,XCfgSuitMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Missions,XCfgMissionMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MissionsDynamic,XCfgMissionsDynamicMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MissionsType,XCfgMissionsTypeMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MoneyTree,XCfgMoneyTreeMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_LevelAdjust,XCfgLevelAdjustMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MonsterBase,XCfgMonsterBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MonsterGroup,XCfgMonsterGroupMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Mount,XCfgMountMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_NpcBase,XCfgNpcBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_SceneNpcList,XCfgSceneNpcInfoMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_OnlineReward,XCfgOnlineRewardMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_FemaleFirstName,XCfgFemaleFirstNameMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_FilterWord,XCfgFilterWordMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_LastName,XCfgLastNameMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MaleFirstName,XCfgMaleFirstNameMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_String,XCfgStringMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_PetBase,XCfgPetBaseMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_AttrMark,XCfgAttrMarkMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ClassLvUp,XCfgClassLvUpMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_PlayerBase,XCfgPlayerBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_PlayerLvUp,XCfgPlayerLvUpMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Train,XCfgTrainMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Formula,XCfgFormulaMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_GatherObject,XCfgGatherObjectMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_GatherObjectBorn,XCfgGatherObjectBornMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ProductCareer,XCfgProductCareerMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_AllSceneList,XCfgSceneInfoMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ClientScene,XCfgClientSceneMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_GameParam,XCfgGameParamMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_LevelEntry,XCfgLevelEntryMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ShopItem,XCfgShopItemMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_SkillBase,XCfgSkillBaseMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_SkillLevel,XCfgSkillLevelMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_SkillOper,XCfgSkillOperMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BuffStr,XStringManager.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_HintStr,XStringManager.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ItemStr,XStringManager.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_WorldMap,XCfgWorldMapMgr.SP);
		
		//mConfigList.Add(EDBConfg_Item.EDBConfg_Item_PKChallenge,XPKChallengeMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Title,XTitleMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_XPKAward,XPKAwardMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BuyChanllege, XCfgBuyChallengeMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Growth_Condition,XCfgConditionMgr.SP);	
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Growth_Target,XCfgGrowthTargetMgr.SP);	
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_NewPlayerGuide, XCfgNewPlayerGuideMgr.SP);
	
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ShanHTBase, XCfgShanHTBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ShanHTLevel, XCfgShanHTLevelMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_OperTip, XCfgOperTipMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_AuctionConfig, XCfgAcutionConfigMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BoxTip, XCfgBoxTipMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_AddAward,XCfgAddAwardMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_NickName,XCfgNickNameMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Phiz, XCfgPhizConfigMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BuyHealth, XCfgHealthBuyMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_MountAttr, XCfgMountAttrMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_FiendlyLevel, XCfgFriendlyLevelMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ShengWangItem, XCfgShengWangItemMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ShengWangLvl, XCfgShengWangLvlMgr.SP);

		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_Formation, XCfgFormationMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_SevenTarget, XCfgSevenTargetMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_ZhanYaoLu, XCfgZhanYaoLuMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_LianZhan, XCfgLianZhanMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_HillSeaBook, XCfgHillSeaBookMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_DailySign, XCfgDailySignMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_UIRelationShip, XCfgUIRelationShipMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_PlayHint, XCfgPlayHintMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_VipBase, XCfgVipBaseMgr.SP);
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_VipAward, XCfgVipAwardMgr.SP);
        mConfigList.Add(EDBConfg_Item.EDBConfg_Item_VipShow, XCfgVipShowMgr.SP);

	   	mConfigList.Add(EDBConfg_Item.EDBConfg_Item_DayActivity, XCfgDayActivityBaseMgr.SP);
        mConfigList.Add(EDBConfg_Item.EDBConfg_Item_DayAward, XCfgDayActivityAwardMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_BattleFailGuide, XCfgBattleFailGuideMgr.SP);
		
		mConfigList.Add(EDBConfg_Item.EDBConfg_Item_DailyPlaySign, XCfgDailyPlaySignMgr.SP);
		
		XPlayHintMgr.SP.Init();
		
	}
	
	public IConfigManager GetConfigManager(EDBConfg_Item item)
	{
		if(!mConfigList.ContainsKey(item))
			return null;
		
		return mConfigList[item];
	}
}
