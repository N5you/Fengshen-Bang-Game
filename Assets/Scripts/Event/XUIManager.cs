using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using UnityEngine;
using resource;
// 界面的枚举需要和UIPanel.txt匹配的上
public enum EUIPanel
{
    ePlayerHead = 1,
    eNpcHead = 2,
    eMonsterHead = 3,
    eLoginUI = 4,
    eServerListUI = 5,
    eCharOper = 6,
    eChatWindow = 7,
    eNpcDialog = 8,
    eMainPlayerInfo = 9,
    eTargetInfo = 10,
    eScenePassed = 11,
    eSelectScene = 12,
    eSceneDifficulty = 13,
    eCenterTip = 14,
    eSkillOpertation = 15,
    eToolTipA = 16,
    eSmallMap = 17,
    eFightWin = 18,
    eFightFail = 19,
    eFunctionButton = 20,
    eRoleInformation = 21,
    eBagWindow = 22,
    eWorldMap = 23,
    ePopMenu = 24,
    eCursor = 25,
    eMessageBox = 26,
    eFunctionBottomTR = 27,
    eStrengthenWindow = 28,
    eSelectScene_Pop = 29,
    eLoadSceneUI = 30,
    eFightHeadShow = 31,
    ePassInfo = 32,
    eToolTipB = 33,
    eMissionTip = 34,
    eFormation = 35,
    eMissionDialog = 36,
    eObjectHalf = 37,
    eInputMessageBox = 38,
    eShopDialog = 39,
    eProduct = 40,
    eReadTip = 41,
    eAuction = 42,
    eFuncUnLock = 43,
    eFriend = 44,
    eMail = 45,
    eMailBox = 46,
    eStorage = 47,
    eGMWindow = 48,
    eXianDH = 49,
    ePrivateChat = 50,
    eMoneyTree = 51,
    eShanHT = 52,
    eCutSceneDialog = 53,
    eOnlineReawrd = 54,
    ePVPResult = 55,
    eLoginFirst = 56,
    eFriendCharmRank = 57,
    eFriendFlowerHouse = 58,
    ePractice = 59,
    eNewPlayerGuide = 60,
    eOperTip = 61,
    eGetAward = 62,
    eAwardInfo = 63,
    eSelectSceneSE = 64,
    eMessageBoxWithNoCancel = 65,
    eShengWang = 66,
    eFlyItem = 67,
    eFaBao = 68,
    eToolTipC = 69,
    eZhanYaoLu = 70,
    eSevenTarget = 71,
    eMeditation = 72,
    eHillSeaBook = 73,
    eDailySign = 74,
    eGuildMain = 75,
    eGuildCreate = 76,
    eGuildMaintain = 77,
    eGuildInfo = 78,
    eGuildList = 79,
    ePlayHint = 80,
    eZhuangBeiTiShi = 81,
    eForceGuide = 82,
    eVip = 83,
    eDayActivity = 84,
	eBattleFailGuide = 85,
	eDailyPlaySign = 86,
	eSaoDang = 87,
	eSingleTalk = 88,
	eTouZi	= 89,
    eCount
}

public class XUIManager : XSingleton<XUIManager>
{
    private uint CountID = 1000;

    /// <summary>
    /// UI初始位置关联
    /// </summary>
    private SortedList<EUIPanel, EUIAnchor> m_Layout = new SortedList<EUIPanel, EUIAnchor>();

    /// <summary>
    /// //注册UI资源对象
    /// </summary>
    private SortedList<EUIPanel, uint> m_PanelToResourceID = new SortedList<EUIPanel, uint>();

    /// <summary>
    /// 注册UI逻辑对象	
    /// </summary>
    private SortedList<EUIPanel, XUICtrlBase> m_UICtrls;

    /// <summary>
    /// 多UI对象列表
    /// </summary>
    private SortedList<uint, XUICtrlBase> mMuliUIArray = new SortedList<uint, XUICtrlBase>();

    /// <summary>
    /// 当前运行时管理的UI对象
    /// </summary>
    public Hashtable m_UI2Ctrl;

    /// <summary>
    /// UI关系管理
    /// </summary>
    public XUIRelationShipManager mUIRelationManager = new XUIRelationShipManager();

    public XUIManager()
    {
        m_UICtrls = new SortedList<EUIPanel, XUICtrlBase>();
        m_UI2Ctrl = new Hashtable();

        XEventManager.SP.AddHandler(Show, EEvent.UI_Show);
        XEventManager.SP.AddHandler(MuliUIShow, EEvent.UI_MuliShow);
        XEventManager.SP.AddHandler(MuliUIHide, EEvent.UI_MuliHide);

        XEventManager.SP.AddHandler(Hide, EEvent.UI_Hide);
        XEventManager.SP.AddHandler(Toggle, EEvent.UI_Toggle);
        XEventManager.SP.AddHandler(ReqOriginal, EEvent.UI_ReqOriginal);
        XEventManager.SP.AddHandler(ShowMainUI, EEvent.UI_ShowMainUI);
        XEventManager.SP.AddHandler(HideAllUI, EEvent.UI_HideAllUI);

        XEventManager.SP.AddHandler(OnOriginal, EEvent.UI_OnOriginal);
        XEventManager.SP.AddHandler(OnCreated, EEvent.UI_OnCreated);
        XEventManager.SP.AddHandler(OnShow, EEvent.UI_OnShow);
        XEventManager.SP.AddHandler(OnHide, EEvent.UI_OnHide);

        XEventManager.SP.AddHandler(ResourceReady, EEvent.ResourceFileReady);
    }

    public void InitRes()
    {
        SortedList<uint, XResourceBase> mgr = XResourceManager.GetAllResourceByType(XResourcePanel.ResTypeName);
        if (mgr == null)
            return;

        foreach (KeyValuePair<uint, XResourceBase> temp in mgr)
        {
            AddUIResource((EUIPanel)temp.Key, temp.Key);
        }
    }

    public void ResourceReady(EEvent evt, params object[] args)
    {
        InitRes();
    }

    public void Init()
    {
        InitUIPostion();
        InitUILogic();
        InitUIMode();

        UICamera.UnPressDelegate += UnPress;
    }

    public void AddUIResource(EUIPanel panel, uint resID)
    {
        if (m_PanelToResourceID.ContainsKey(panel))
            return;

        m_PanelToResourceID.Add(panel, resID);
    }

    private void InitUILogic()
    {
        m_UICtrls.Add(EUIPanel.eLoginUI, new XUTLoginUI());
        m_UICtrls.Add(EUIPanel.eServerListUI, new XUTServerListUI());
        m_UICtrls.Add(EUIPanel.eCharOper, new XUTCharacterOperation());
        m_UICtrls.Add(EUIPanel.ePlayerHead, new XUIPlayerHead());
        m_UICtrls.Add(EUIPanel.eNpcHead, new XUINpcHead());
        m_UICtrls.Add(EUIPanel.eMonsterHead, new XUIMonsterHead());
        m_UICtrls.Add(EUIPanel.eObjectHalf, new XUTObjectHalf());
        m_UICtrls.Add(EUIPanel.eChatWindow, new XUTChatWindow());
        m_UICtrls.Add(EUIPanel.eMainPlayerInfo, new XUTMainPlayerInfo());
        m_UICtrls.Add(EUIPanel.eFunctionButton, new XUTFunctionButton());
        m_UICtrls.Add(EUIPanel.eNpcDialog, new XUTNpcDialog());
        m_UICtrls.Add(EUIPanel.eScenePassed, new XUTScenePassed());
        m_UICtrls.Add(EUIPanel.eMissionTip, new XUTMissionTip());
        m_UICtrls.Add(EUIPanel.eSelectScene, new XUTSelectScene());
        m_UICtrls.Add(EUIPanel.eSkillOpertation, new XUTSkillOperation());
        m_UICtrls.Add(EUIPanel.eToolTipA, new XUTToolTipA());
        m_UICtrls.Add(EUIPanel.eCenterTip, new XUTCenterTip());
        m_UICtrls.Add(EUIPanel.eFightWin, new XUTFightWin());
        m_UICtrls.Add(EUIPanel.eFightFail, new XUTFightFail());
        m_UICtrls.Add(EUIPanel.eRoleInformation, new XUIRoleInformation());
        m_UICtrls.Add(EUIPanel.eBagWindow, new XUIBagWindow());
        m_UICtrls.Add(EUIPanel.eWorldMap, new XUTWorldMap());
        m_UICtrls.Add(EUIPanel.ePopMenu, new XUIPopMenu());
        m_UICtrls.Add(EUIPanel.eCursor, new XUICursor());
        m_UICtrls.Add(EUIPanel.eMessageBox, new XUTMessageBox());
        m_UICtrls.Add(EUIPanel.eFunctionBottomTR, new XUIFunctionBottomTR());
        m_UICtrls.Add(EUIPanel.eSmallMap, new XUISmallMap());
        m_UICtrls.Add(EUIPanel.eStrengthenWindow, new XUIStrengthenWindow());
        m_UICtrls.Add(EUIPanel.eSelectScene_Pop, new XUTSelectScene_Pop());
        m_UICtrls.Add(EUIPanel.eLoadSceneUI, new XUTLoadScene());
        m_UICtrls.Add(EUIPanel.eFightHeadShow, new XUTFightHeadShow());
        m_UICtrls.Add(EUIPanel.ePassInfo, new XUTPassInfo());
        m_UICtrls.Add(EUIPanel.eToolTipB, new XUTToolTipB());
        m_UICtrls.Add(EUIPanel.eFormation, new XUTFormation());
        m_UICtrls.Add(EUIPanel.eMissionDialog, new XUTMissionDialog());
        m_UICtrls.Add(EUIPanel.eInputMessageBox, new XUTInputMessageBox());
        m_UICtrls.Add(EUIPanel.eProduct, new XUTProduct());
        m_UICtrls.Add(EUIPanel.eReadTip, new XUTReadTip());
        m_UICtrls.Add(EUIPanel.eShopDialog, new XUTShopDialog());
        m_UICtrls.Add(EUIPanel.eAuction, new XUTAuction());
        m_UICtrls.Add(EUIPanel.eFuncUnLock, new XUTFuncUnLock());
        m_UICtrls.Add(EUIPanel.eFriend, new XUTFriend());
        m_UICtrls.Add(EUIPanel.eMail, new XUTMail());
        m_UICtrls.Add(EUIPanel.eMailBox, new XUTMailBox());
        m_UICtrls.Add(EUIPanel.eStorage, new XUTStorage());
        m_UICtrls.Add(EUIPanel.eGMWindow, new XUTGMWindow());
        m_UICtrls.Add(EUIPanel.eTargetInfo, new XUTTargetInfo());
        m_UICtrls.Add(EUIPanel.eXianDH, new XUTXianDH());
        m_UICtrls.Add(EUIPanel.ePrivateChat, new XUTPrivateChat());
        m_UICtrls.Add(EUIPanel.eMoneyTree, new XUTMoneyTreeUI());
        m_UICtrls.Add(EUIPanel.eShanHT, new XUTShanHT());
        m_UICtrls.Add(EUIPanel.eCutSceneDialog, new XUICutScenePanel());
        m_UICtrls.Add(EUIPanel.eOnlineReawrd, new XUTOnlineReward());
        m_UICtrls.Add(EUIPanel.ePVPResult, new XUTPVPResult());
        m_UICtrls.Add(EUIPanel.eFriendCharmRank, new XUTFriendCharmRank());
        m_UICtrls.Add(EUIPanel.eFriendFlowerHouse, new XUTFriendFlowerHouse());
        m_UICtrls.Add(EUIPanel.ePractice, new XUTPractise());
        m_UICtrls.Add(EUIPanel.eNewPlayerGuide, new XUTNewPlayerGuide());
        m_UICtrls.Add(EUIPanel.eLoginFirst, new XUTFirstLogin());
        m_UICtrls.Add(EUIPanel.eOperTip, new XUTOperTip());
        m_UICtrls.Add(EUIPanel.eGetAward, new XUTGetAward());
        m_UICtrls.Add(EUIPanel.eAwardInfo, new XUTAwardInfo());
        m_UICtrls.Add(EUIPanel.eSelectSceneSE, new XUTSelectSceneSE());
        m_UICtrls.Add(EUIPanel.eMessageBoxWithNoCancel, new XUTMessageBoxWithNoCancel());
        m_UICtrls.Add(EUIPanel.eShengWang, new XUTShengWang());
        m_UICtrls.Add(EUIPanel.eFlyItem, new XUTFlyItem());
        m_UICtrls.Add(EUIPanel.eFaBao, new XUTFaBao());
        m_UICtrls.Add(EUIPanel.eSevenTarget, new XUTSevenTarget());
        m_UICtrls.Add(EUIPanel.eToolTipC, new XUTToolTipC());
        m_UICtrls.Add(EUIPanel.eZhanYaoLu, new XUTZhanYaoLu());
        m_UICtrls.Add(EUIPanel.eMeditation, new XUTMeditation());
        m_UICtrls.Add(EUIPanel.eHillSeaBook, new XUIHillSeaBookDialog());
        m_UICtrls.Add(EUIPanel.eDailySign, new XUTDailySign());
        m_UICtrls.Add(EUIPanel.eGuildMain, new XUTGuildMain());
        m_UICtrls.Add(EUIPanel.eGuildCreate, new XUTGuildCreate());
        m_UICtrls.Add(EUIPanel.eGuildMaintain, new XUTGuildMaintain());
        m_UICtrls.Add(EUIPanel.eGuildInfo, new XUTGuildInfo());
        m_UICtrls.Add(EUIPanel.eGuildList, new XUTGuildList());
        m_UICtrls.Add(EUIPanel.ePlayHint, new XUTPlayHint());
        m_UICtrls.Add(EUIPanel.eZhuangBeiTiShi, new XUTZhuangBeiTiShi());
        m_UICtrls.Add(EUIPanel.eForceGuide, new XUTForceGuide());
        m_UICtrls.Add(EUIPanel.eVip, new XUTVip());
		m_UICtrls.Add(EUIPanel.eDayActivity, new XUTDayActivity());
		m_UICtrls.Add(EUIPanel.eBattleFailGuide, new XUTBattleFailGuide());
		m_UICtrls.Add(EUIPanel.eDailyPlaySign, new XUTDailyPlaySign());
		m_UICtrls.Add(EUIPanel.eSaoDang, new XUTSaoDang());
		m_UICtrls.Add(EUIPanel.eSingleTalk,new XUTSingleTalk());		
    }

    private void InitUIPostion()
    {
        m_Layout.Add(EUIPanel.ePlayerHead, EUIAnchor.eUnknow);
        m_Layout.Add(EUIPanel.eNpcHead, EUIAnchor.eUnknow);
        m_Layout.Add(EUIPanel.eMonsterHead, EUIAnchor.eUnknow);
        m_Layout.Add(EUIPanel.eLoginUI, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eCharOper, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eServerListUI, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eChatWindow, EUIAnchor.eBottom_Left);
        m_Layout.Add(EUIPanel.eNpcDialog, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eScenePassed, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSelectScene, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSceneDifficulty, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eCenterTip, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSkillOpertation, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eToolTipA, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSmallMap, EUIAnchor.eTop_Right);
        m_Layout.Add(EUIPanel.eMainPlayerInfo, EUIAnchor.eTop_Left);
        m_Layout.Add(EUIPanel.eFunctionButton, EUIAnchor.eBottom_Right);
        m_Layout.Add(EUIPanel.eFightWin, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFightFail, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eRoleInformation, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eBagWindow, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eWorldMap, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.ePopMenu, EUIAnchor.eWindows);
        m_Layout.Add(EUIPanel.eCursor, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMessageBox, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFunctionBottomTR, EUIAnchor.eTop_Right);
        m_Layout.Add(EUIPanel.eStrengthenWindow, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSelectScene_Pop, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eLoadSceneUI, EUIAnchor.eTop_Right);
        m_Layout.Add(EUIPanel.eFightHeadShow, EUIAnchor.eTop);
        m_Layout.Add(EUIPanel.ePassInfo, EUIAnchor.eTop_Right);
        m_Layout.Add(EUIPanel.eToolTipB, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMissionTip, EUIAnchor.eRight);
        m_Layout.Add(EUIPanel.eFormation, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMissionDialog, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eObjectHalf, EUIAnchor.eUnknow);
        m_Layout.Add(EUIPanel.eInputMessageBox, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eShopDialog, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eProduct, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eReadTip, EUIAnchor.eBottom);
        m_Layout.Add(EUIPanel.eAuction, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFuncUnLock, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFriend, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMail, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMailBox, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eStorage, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGMWindow, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eTargetInfo, EUIAnchor.eTop_Left);
        m_Layout.Add(EUIPanel.eXianDH, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.ePrivateChat, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMoneyTree, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eShanHT, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eCutSceneDialog, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eOnlineReawrd, EUIAnchor.eTop_Right);
        m_Layout.Add(EUIPanel.ePVPResult, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.ePractice, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eNewPlayerGuide, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFriendCharmRank, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFriendFlowerHouse, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eLoginFirst, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eOperTip, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGetAward, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eAwardInfo, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSelectSceneSE, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMessageBoxWithNoCancel, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eShengWang, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFlyItem, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eFaBao, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eToolTipC, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eZhanYaoLu, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eSevenTarget, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eMeditation, EUIAnchor.eBottom_Right);
        m_Layout.Add(EUIPanel.eHillSeaBook, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eDailySign, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGuildMain, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGuildCreate, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGuildMaintain, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGuildInfo, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eGuildList, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.ePlayHint, EUIAnchor.eRight);
        m_Layout.Add(EUIPanel.eZhuangBeiTiShi, EUIAnchor.eRight);
        m_Layout.Add(EUIPanel.eForceGuide, EUIAnchor.eCenter);
        m_Layout.Add(EUIPanel.eVip, EUIAnchor.eCenter);
		m_Layout.Add(EUIPanel.eDayActivity, EUIAnchor.eCenter);
		m_Layout.Add(EUIPanel.eBattleFailGuide, EUIAnchor.eCenter);
		m_Layout.Add(EUIPanel.eDailyPlaySign, EUIAnchor.eTop_Left);
		m_Layout.Add(EUIPanel.eSaoDang, EUIAnchor.eCenter);
		m_Layout.Add(EUIPanel.eSingleTalk, EUIAnchor.eBottom);
    }

    private void InitUIMode()
    {
        //在这里讲每个界面的显示模式设置上，以后UI需要配置的属性多起来的话，可以放到配置文件中
        //现在先放置到这里，不设置的话，切换模式的话，对该UI不做任何处理
        m_UICtrls[EUIPanel.eChatWindow].UIMode                 = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL | 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE | 1 << (int)UI_MODE.UI_MODE_STATIC_BATTLE;
        m_UICtrls[EUIPanel.eMainPlayerInfo].UIMode             = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL | 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE;
        m_UICtrls[EUIPanel.eFunctionButton].UIMode             = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL | 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE;
        m_UICtrls[EUIPanel.eNpcDialog].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eScenePassed].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eSelectScene].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eSkillOpertation].UIMode            = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eToolTipA].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eCenterTip].UIMode                  = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL | 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE | 1 << (int)UI_MODE.UI_MODE_STATIC_BATTLE;
        m_UICtrls[EUIPanel.eFightWin].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFightFail].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eRoleInformation].UIMode            = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eBagWindow].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eWorldMap].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.ePopMenu].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFunctionBottomTR].UIMode           = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL;
        m_UICtrls[EUIPanel.eSmallMap].UIMode                   = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL;
        m_UICtrls[EUIPanel.eStrengthenWindow].UIMode           = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eSelectScene_Pop].UIMode            = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFightHeadShow].UIMode              = 1 << (int)UI_MODE.UI_MODE_STATIC_BATTLE;
        m_UICtrls[EUIPanel.ePassInfo].UIMode                   = 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE;
        m_UICtrls[EUIPanel.eMissionTip].UIMode                 = 1 << (int)UI_MODE.UI_MODE_STATIC_NORMAL | 1 << (int)UI_MODE.UI_MODE_STATIC_SUB_SCENE;
        m_UICtrls[EUIPanel.eFormation].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMissionDialog].UIMode              = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eShopDialog].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFuncUnLock].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFriend].UIMode                     = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMail].UIMode                       = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMailBox].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eStorage].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGMWindow].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eTargetInfo].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eXianDH].UIMode                     = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.ePrivateChat].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMoneyTree].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eShanHT].UIMode                     = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eCutSceneDialog].UIMode             = 1 << (int)UI_MODE.UI_MODE_CUTSCENE | 1 << (int)UI_MODE.UI_MODE_NORMAL_CUTSCENE;
        m_UICtrls[EUIPanel.eOnlineReawrd].UIMode               = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.ePVPResult].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFriendCharmRank].UIMode            = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFriendFlowerHouse].UIMode          = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.ePractice].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eNewPlayerGuide].UIMode             = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eLoginFirst].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eOperTip].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGetAward].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eAwardInfo].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eSelectSceneSE].UIMode              = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMessageBoxWithNoCancel].UIMode     = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eShengWang].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFlyItem].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eFaBao].UIMode                      = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eToolTipC].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eCharOper].UIMode                   = 1 << (int)UI_MODE.UI_MODE_CHARACTER;
        m_UICtrls[EUIPanel.eZhanYaoLu].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eSevenTarget].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eMeditation].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eHillSeaBook].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eDailySign].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGuildMain].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGuildCreate].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGuildMaintain].UIMode              = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGuildInfo].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eGuildList].UIMode                  = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.ePlayHint].UIMode                   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eZhuangBeiTiShi].UIMode             = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
        m_UICtrls[EUIPanel.eForceGuide].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
	    m_UICtrls[EUIPanel.eDayActivity].UIMode                = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
		m_UICtrls[EUIPanel.eBattleFailGuide].UIMode            = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
		m_UICtrls[EUIPanel.eDailyPlaySign].UIMode			   = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
		m_UICtrls[EUIPanel.eSaoDang].UIMode                    = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
		m_UICtrls[EUIPanel.eSingleTalk].UIMode                 = 1 << (int)UI_MODE.UI_MODE_DYNAMIC;
    }

    public void Breathe()
    {
        foreach (XUICtrlBase uiCtrl in m_UICtrls.Values)
        {
            uiCtrl.Breathe();
        }
    }

    void ShowMainUI(EEvent evt, params object[] args)
    {
        Show(EEvent.UI_Show, EUIPanel.eChatWindow, null);
        Show(EEvent.UI_Show, EUIPanel.eMainPlayerInfo, null);
        Show(EEvent.UI_Show, EUIPanel.eFunctionButton, null);
        Show(EEvent.UI_Show, EUIPanel.eFunctionBottomTR, null);
        Show(EEvent.UI_Show, EUIPanel.eSmallMap, null);
        Show(EEvent.UI_Show, EUIPanel.eMissionTip, null);
    }

    public void HideAllUI(EEvent evt, params object[] args)
    {
        HideAllUI();
    }

    public void HideAllUI()
    {
        return;
        foreach (KeyValuePair<EUIPanel, XUICtrlBase> index in m_UICtrls)
        {
            index.Value.Hide();
        }
    }

    public XUICtrlBase GetUIControl(EUIPanel panel)
    {
        if (m_UICtrls.ContainsKey(panel))
            return m_UICtrls[panel];

        return null;
    }

    public void ShowByMode(UI_MODE mode)
    {
        //隐藏当前dymaic标记的UI
        foreach (KeyValuePair<EUIPanel, XUICtrlBase> index in m_UICtrls)
        {
            if ((index.Value.UIMode & (1 << (int)UI_MODE.UI_MODE_DYNAMIC)) > 0)
            {
                index.Value.Hide();
            }

        }

        foreach (XUICtrlBase MuliCtrl in mMuliUIArray.Values)
        {
            if ((MuliCtrl.UIMode & (1 << (int)UI_MODE.UI_MODE_DYNAMIC)) > 0)
            {
                MuliCtrl.Hide();
            }
        }

        //弹出所有相应标记的对象,关闭非NO_OP标记的UI
        foreach (KeyValuePair<EUIPanel, XUICtrlBase> index in m_UICtrls)
        {
            if ((index.Value.UIMode & (1 << (int)mode)) > 0)
            {
                Show(EEvent.UI_Show, index.Key);
            }
            else
            {
                if ((index.Value.UIMode & (1 << (int)UI_MODE.UI_MODE_NO_OP)) == 0)
                {
                    index.Value.Hide();
                }
            }
        }
    }

    void Show(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[0];
        int debugPanelType = (int)ePanel;
        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        if (!m_UICtrls[ePanel].IsCanShow())
            return;

        if (!m_UICtrls[ePanel].Show())
        {
            CreateUI(ePanel, 0);
        }
        else
        {
            if (EUIAnchor.eCenter == GetUIAnchor(ePanel))
            {
                //mUIRelationManager.CheckPanel(ePanel);
             	mUIRelationManager.IsHasOpenPanel(ePanel);
            }
        }
    }
    void Hide(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[0];
        int debugPanelType = (int)ePanel;
        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        m_UICtrls[ePanel].Hide();
        mUIRelationManager.HideSubPanel(ePanel);
    }

    void Toggle(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[0];
        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        if (XResourceManager.ResourceIsLoaded(XResourcePanel.ResTypeName, (uint)ePanel))
        {
            m_UICtrls[ePanel].Toggle();
            if (m_UICtrls[ePanel].IsLogicShow)
            {
                if (EUIAnchor.eCenter == GetUIAnchor(ePanel))
                {
                    if (!mUIRelationManager.IsHasOpenPanel(ePanel))
                    {
                        m_UICtrls[ePanel].SetPos(new Vector3(0f, 0f, m_UICtrls[ePanel].GetPos().z));
                    }
                }
            }
        }
        else
        {
            m_UICtrls[ePanel].IsLogicShow = true;
            CreateUI(ePanel, 0);
            if (EUIAnchor.eCenter == GetUIAnchor(ePanel))
            {
                //mUIRelationManager.CheckPanel(ePanel);
                if (!mUIRelationManager.IsHasOpenPanel(ePanel))
                {
                    m_UICtrls[ePanel].SetPos(new Vector3(0f, 0f, m_UICtrls[ePanel].GetPos().z));
                }
            }
        }
    }

    void ReqOriginal(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[0];
        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        if (m_UICtrls[ePanel].ReqOriginal(args[1]))
            return;

        OriginalUI(ePanel);
    }

    void OnOriginal(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[1];
        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        m_UICtrls[ePanel].OnOriginal(args[0]);
    }

    void OnCreated(EEvent evt, params object[] args)
    {
        EUIPanel ePanel = (EUIPanel)args[1];
        uint PanelID = (uint)args[2];

        if (!m_UICtrls.ContainsKey(ePanel))
            return;

        if (PanelID == 0)
        {
            m_UI2Ctrl[args[0]] = m_UICtrls[ePanel];
            m_UICtrls[ePanel].OnCreated(args[0]);

            //设置坐标
            if (GetUIAnchor(ePanel) == EUIAnchor.eCenter)
                m_UICtrls[ePanel].SetPos(new Vector3(0f, 0f, m_UICtrls[ePanel].GetPos().z));
            mUIRelationManager.PanelCreated(ePanel);
        }
        else
        {
            XUICtrlBase ctrl = GetMuliUIObject(PanelID);
            if (ctrl != null)
            {
                m_UI2Ctrl[args[0]] = ctrl;
                ctrl.OnCreated(args[0]);
            }
        }
    }

    void OnShow(EEvent evt, params object[] args)
    {
        if (!m_UI2Ctrl.ContainsKey(args[0]))
            return;

        (m_UI2Ctrl[args[0]] as XUICtrlBase).OnShow();
    }

    void OnHide(EEvent evt, params object[] args)
    {
        if (!m_UI2Ctrl.ContainsKey(args[0]))
            return;

        (m_UI2Ctrl[args[0]] as XUICtrlBase).OnHide();
    }

    public void CreateUI(EUIPanel ePanel, uint panelID)
    {
        if (!m_PanelToResourceID.ContainsKey(ePanel))
        {
            Log.Write(LogLevel.WARN, "not find PanelID is {0} panel", panelID);
            return;
        }

        uint resID = m_PanelToResourceID[ePanel];
        XResourcePanel panel = (XResourcePanel)XResourceManager.GetResource(XResourcePanel.ResTypeName, resID);
        if (panel == null)
        {
            Log.Write(LogLevel.ERROR, "cant find UI ID {0}", resID);
            return;
        }

        if (panelID != 0)
            panel.AddPanelKey(panelID);

        if (panel.IsLoadDone())
        {
            panel.CreateUI(panel.MainAsset.DownLoad);
        }
        else
        {
            panel.CurPanel = ePanel;
            panel.ParentTF = LogicApp.SP.UIRoot.UIAnchors[(int)m_Layout[ePanel]];
            XResourceManager.StartLoadResource(XResourcePanel.ResTypeName, resID);
            if (!panel.IsAddEvent)
            {
                panel.ResLoadEvent += panel.CreateUI;
                panel.IsAddEvent = true;
            }
        }
    }

    public void OriginalUI(EUIPanel ePanel)
    {
        if (!m_PanelToResourceID.ContainsKey(ePanel))
            return;

        uint resID = m_PanelToResourceID[ePanel];
        XResourcePanel panel = (XResourcePanel)XResourceManager.GetResource(XResourcePanel.ResTypeName, resID);
        if (panel == null)
        {
            Log.Write(LogLevel.ERROR, "cant find UI ID {0}", resID);
            return;
        }
        if (panel.IsLoadDone())
        {
            XEventManager.SP.SendEvent(EEvent.UI_OnOriginal, panel.PanelObject.GetComponent<XUIBaseLogic>(), ePanel);
        }
        else
        {
            panel.CurPanel = ePanel;
            XResourceManager.StartLoadResource(XResourcePanel.ResTypeName, resID);
            panel.ParentTF = LogicApp.SP.UIRoot.UIAnchors[(int)m_Layout[ePanel]];
            panel.ResLoadEvent += new XResourceBase.LoadCompletedDelegate(panel.OriginalUI);
        }
    }

    private void UnPress()
    {
        if (!XUIPopMenu.isOper)
        {
            XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.ePopMenu);
            XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipA);
        }
        if (!XChatWindow.isBiaoQingOper)
        {
            XEventManager.SP.SendEvent(EEvent.Chat_HideBiaoQingSelUI);
        }

        XUIPopMenu.isOper = false;
        XChatWindow.isBiaoQingOper = false;
    }

    public uint GetMuliUIObjectKey(EUIPanel uiType)
    {
        foreach (KeyValuePair<uint, XUICtrlBase> ctrl in mMuliUIArray)
        {
            if (uiType == ctrl.Value.GetPanelType() && !ctrl.Value.IsLogicShow)
            {
                return ctrl.Key;
            }
        }

        //Not Find
        if (!m_UICtrls.ContainsKey(uiType))
            return 0;

        object newObj = Activator.CreateInstance(m_UICtrls[uiType].GetType());
        CountID++;
        XUICtrlBase go = newObj as XUICtrlBase;
        go.PanelKey = CountID;
        go.UIMode = m_UICtrls[uiType].UIMode;
        go.IsLogicShow = true;
        mMuliUIArray.Add(CountID, go);
        return CountID;
    }

    public XUICtrlBase GetMuliUIObject(uint key)
    {
        if (mMuliUIArray.ContainsKey(key))
            return mMuliUIArray[key];

        return null;
    }

    public void HideAllByPanelType(EUIPanel uipanel)
    {
        foreach (XUICtrlBase ctrl in mMuliUIArray.Values)
        {
            if (ctrl.GetPanelType() == uipanel)
                ctrl.Hide();
        }
    }

    void MuliUIShow(EEvent evt, params object[] args)
    {
        uint key = (uint)args[0];
        XUICtrlBase ctrl = GetMuliUIObject(key);
        if (ctrl == null)
            return;

        if (!ctrl.IsCanShow())
            return;

        if (!ctrl.Show())
            CreateUI(ctrl.GetPanelType(), key);
    }

    void MuliUIHide(EEvent evt, params object[] args)
    {
        uint key = (uint)args[0];
        XUICtrlBase ctrl = GetMuliUIObject(key);
        if (ctrl == null)
            return;

        ctrl.Hide();
    }

    public ushort GetUISortValue(EUIPanel ePanel)
    {
        XCfgUIRelationShip cfg = XCfgUIRelationShipMgr.SP.GetConfig((ushort)ePanel);
        if (cfg == null)
            return 0;
        return cfg.SortByDESC;
    }

    public EUIAnchor GetUIAnchor(EUIPanel _panel)
    {
        if (m_Layout.ContainsKey(_panel))
            return m_Layout[_panel];
        return EUIAnchor.eUnknow;
    }
}