using System;
using UnityEngine;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
public class PacketGate
{
    private enum Command
    {
        Begin = SC_Protocol.eSC_PacketBegin,
        End = SC_Protocol.eSC_PacketEnd
    }

    private delegate void PacketProcessFuc(NetPacket packet);

    Hashtable m_ProcessMethod;

    public PacketGate()
    {
        m_ProcessMethod = new Hashtable();

        // 注册消息处理函数
        // Login - Client
        Register((int)LC_Protocol.eLC_CheckAccountResult, Process_CheckAccountResult);
        Register((int)LC_Protocol.eLC_CharServerInfo, Process_CharServerInfo);
        Register((int)LC_Protocol.eLC_SelectServerRet, Process_SelectServerRet);

        // CharServer - Client
        Register((int)ChC_Protocol.eChC_Return, Process_ChC_Return);
        Register((int)ChC_Protocol.eChC_LoadCharList, Process_LoadCharList);
        Register((int)ChC_Protocol.eChC_CreatePlayer, Process_CreatePlayer);
        Register((int)ChC_Protocol.eChC_GameServerInfo, Process_GameServerInfo);

        // GameServer - Client
        Register((int)SC_Protocol.eSC_EnterGame, Process_EnterGame);
        Register((int)SC_Protocol.eSC_PlayerAppear, Process_PlayerAppear);
        Register((int)SC_Protocol.eSC_PlayerAppearData, Process_PlayerAppearData);
        Register((int)SC_Protocol.eSC_MissionReceiveResult, Process_MissionReceiveResult);
        Register((int)SC_Protocol.eSC_MissionReferResult, Process_MissionReferResult);
        Register((int)SC_Protocol.eSC_MissionGiveUpResult, Process_MissionGiveUpResult);
        Register((int)SC_Protocol.eSC_ReferExistMission, Process_ReferExistMission);
        Register((int)SC_Protocol.eSC_ActiveMission, Process_ActiveMissionState);
        Register((int)SC_Protocol.eSC_HillSeaBookMsg, Process_HillSeaBookUpdate);
        Register((int)SC_Protocol.eSC_PlayerDisappear, Process_PlayerDisappear);
        Register((int)SC_Protocol.eSC_StartMove, Process_StartMove);
        Register((int)SC_Protocol.eSC_StopMove, Process_StopMove);
        Register((int)SC_Protocol.eSC_ForceSetPosition, Process_ForceSetPosition);
        Register((int)SC_Protocol.eSC_NpcAppear, Process_NpcAppear);
        Register((int)SC_Protocol.eSC_NpcAppearData, Process_NpcAppearData);
        Register((int)SC_Protocol.eSC_NpcDisappear, Process_NpcDisappear);
        Register((int)SC_Protocol.eSC_Jump, Process_Jump);
        Register((int)SC_Protocol.eSC_JumpOver, Process_JumpOver);
        Register((int)SC_Protocol.eSC_Chat, Process_Chat);
        Register((int)SC_Protocol.eSC_ChangeScene, Process_ChangeScene);
        Register((int)SC_Protocol.eSC_PlayerBaseInfo, Process_PlayerBaseInfo);
        Register((int)SC_Protocol.eSC_PlayerDetailInfo, Process_PlayerDetailInfo);
        Register((int)SC_Protocol.eSC_ShowNpcDialog, Process_ShowNpcDialog);
        Register((int)SC_Protocol.eSC_BeginShowNpcDialog, Process_BeginShowNpcDialog);
        Register((int)SC_Protocol.eSC_Sit, Process_Sit);
        Register((int)SC_Protocol.eSC_StopSit, Process_StopSit);

        // 副本, 战斗
        Register((int)SC_Protocol.eSC_EnterClientScene, Process_EnterClientScene);
        Register((int)SC_Protocol.eSC_BattleData, Process_BattleData);
        Register((int)SC_Protocol.eSC_BattleResult, Process_BattleResultPVE);
        Register((int)SC_Protocol.eSC_CopySceneState, Process_CopySceneState);
        Register((int)SC_Protocol.eSC_PlayerBattlePos, Process_PlayerBattlePos);
        Register((int)SC_Protocol.eSC_CopySceneResult, Process_CopySceneResult);
        Register((int)SC_Protocol.eSC_SubSceneAwardSel, Process_CopySceneAwardSel);
        Register((int)SC_Protocol.eSC_ReadyPVP, Process_ReadyBattleObject);
        Register((int)SC_Protocol.eSC_ReadyPVE, Process_ReadyBattleObject);
        Register((int)SC_Protocol.eSC_ResultPVP, Process_BattleResultPVP);

        Register((int)SC_Protocol.eSC_ShanHTBattleResult, Process_BattleResultShanHT);
        Register((int)SC_Protocol.eSC_ShanHTBaseData, Process_ShanHT_Info);
        Register((int)SC_Protocol.eSC_ShanHTRank, Process_ShanHT_Rank);
        Register((int)SC_Protocol.eSC_ShanHTNotice, Process_ShanHT_Notice);
        Register((int)SC_Protocol.eSC_ShanHTNoticeItem, Process_ShanHT_Notice_Item);


        // 任务
        //Register((int)SC_Protocol.eSC_ActiveQuestList,      Process_ActiveQuestList);
        //Register((int)SC_Protocol.eSC_QuestMagic,           Process_QuestMagic);
        //Register((int)SC_Protocol.eSC_AllQuestBit,          Process_AllQuestBit);
        //Register((int)SC_Protocol.eSC_SetQuestState,        Process_SetQuestState);
        //Register((int)SC_Protocol.eSC_SetQuestMagic,        Process_SetQuestMagic);
        //Register((int)SC_Protocol.eSC_AcceptQuest,          Process_AcceptQuest);
        //Register((int)SC_Protocol.eSC_FinishQuest,          Process_FinishQuest);
        //Register((int)SC_Protocol.eSC_DelQuest,             Process_DelQuest);
        //Register((int)SC_Protocol.eSC_UpdateQuestFlag,      Process_UpdateQuestFlag);

        // 物品

        Register((int)SC_Protocol.eSC_ItemList, Process_ItemList);
        //Register((int)SC_Protocol.eSC_ItemOper,             Process_ItemOper);
        //Register((int)SC_Protocol.eSC_SwapItem,             Process_SwapItem);
        Register((int)SC_Protocol.eSC_ItemSpace, Process_ItemSpace);
        Register((int)SC_Protocol.eSC_ItemCombine, Process_ItemCombine);
        Register((int)SC_Protocol.eSC_ItemDrop, Process_ItemDrop);
        Register((int)SC_Protocol.eSC_ItemComStrengthen, Process_ItemComStrengthen);
        Register((int)SC_Protocol.eSC_ItemMoneyStrengthen, Process_ItemMoneyStrengthen);
        Register((int)SC_Protocol.eSC_ItemDecompose, Process_ItemDecompose);
        Register((int)SC_Protocol.eSC_ItemGemInlay, Process_ItemGemInlay);
        Register((int)SC_Protocol.eSC_ItemGemRemove, Process_ItemGemRemove);
        Register((int)SC_Protocol.eSC_ItemGemRemoveAll, Process_ItemGemRemoveAll);
        Register((int)SC_Protocol.eSC_ItemUpdate, Process_ItemUpdate);
        Register((int)SC_Protocol.eSC_ItemSeal, Process_ItemSeal);
        Register((int)SC_Protocol.eSC_ItemSplit, Process_ItemSplit);
        Register((int)SC_Protocol.eSC_ItemArrange, Process_ItemArrange);
        Register((int)SC_Protocol.eSC_ItemShow, Process_ItemShow);
        Register((int)SC_Protocol.eSC_ItemUse, Process_ItemUse);


        Register((int)SC_Protocol.eSC_ItemBuyBackList, Process_BuyBackList);
        Register((int)SC_Protocol.eSC_ItemBuyFail, Process_BuyItemFail);

        Register((int)SC_Protocol.eSC_ItemNPCBuyLimitList, Process_BuyItemLimiteList);
        // 宠物
        Register((int)SC_Protocol.eSC_AllPetData, Process_AllPetData);
        Register((int)SC_Protocol.eSC_AddPet, Process_AddPet);
        Register((int)SC_Protocol.eSC_DelPet, Process_DelPet);
        Register((int)SC_Protocol.eSC_SyncPet_BattlePos, Process_SyncPet_Attr);
        Register((int)SC_Protocol.eSC_SyncPet_Loyal, Process_SyncPet_Attr);
        Register((int)SC_Protocol.eSC_SyncPet_Exp, Process_SyncPet_Attr);
        Register((int)SC_Protocol.eSC_SyncPet_Level, Process_SyncPet_Attr);
        Register((int)SC_Protocol.eSC_Pet_ChangeName, Process_Pet_ChangeName);

        // 技能
        Register((int)SC_Protocol.eSC_ActiveSkill, Process_ActiveSkill);
        Register((int)SC_Protocol.eSC_LearnSkill, Process_LearnSkill);
        Register((int)SC_Protocol.eSC_UpgradeSkill, Process_UpgradeSkill);
        Register((int)SC_Protocol.eSC_ForgetSkill, Process_ForgetSkill);
        Register((int)SC_Protocol.eSC_SkillPoint, Process_SkillPoint);
        Register((int)SC_Protocol.eSC_EquipedSkill, Process_EquipedSkill);

        // 坐骑
        Register((int)SC_Protocol.eSC_ActiveMount, Process_ActiveMount);
        Register((int)SC_Protocol.eSC_AddMount, Process_AddMount);
        Register((int)SC_Protocol.eSC_DelMount, Process_DelMount);
        Register((int)SC_Protocol.eSC_MountPeiYangCount, Process_MountPeiYangCount);
        Register((int)SC_Protocol.eSC_MountPeiYangResult, Process_MountPeiYangResult);

        // Buff
        Register((int)SC_Protocol.eSC_ActiveBuff, Process_ActiveBuff);
        Register((int)SC_Protocol.eSC_SetBuffLayer, Process_SetBuffLayer);
        Register((int)SC_Protocol.eSC_SetBuffCount, Process_SetBuffCount);


        // 生产系统
        Register((int)SC_Protocol.eSC_ActiveCareer, Process_ActiveCareer);
        Register((int)SC_Protocol.eSC_SelectCareer, Process_LearnCareer);
        Register((int)SC_Protocol.eSC_ForgetCareer, Process_ForgetCareer);
        Register((int)SC_Protocol.eSC_UpgradeCareer, Process_UpgradeCareer);
        Register((int)SC_Protocol.eSC_ProductStrength, Process_ProductStrength);
        Register((int)SC_Protocol.eSC_ProductExp, Process_ProductExp);
        Register((int)SC_Protocol.eSC_AddGatherRec, Process_AddGatherRec);
        Register((int)SC_Protocol.eSC_AddFormula, Process_AddFormula);
        Register((int)SC_Protocol.eSC_GatherOnceOver, Process_GathterOnceOver);
        Register((int)SC_Protocol.eSC_ProduceOnceOver, Process_ProduceOnceOver);
        Register((int)SC_Protocol.eSC_ApplyProductResult, Process_ApplyProductError);

        // 市集系统
        Register((int)SC_Protocol.eSC_AllAuction, Process_AllAuction);
        Register((int)SC_Protocol.eSC_AllPlayerAuction, Process_MyAuction);
        Register((int)SC_Protocol.eSC_AuctionHistryInfo, Process_UpdateAuctionHistroyInfo);
        Register((int)SC_Protocol.eSC_UpdateAuctionItem, Process_UpdateAuctionItem);
        Register((int)SC_Protocol.eSC_UpdateOneAuction, Process_UpdateMyAuction);
        Register((int)SC_Protocol.eSC_AuctionGetPrice, Process_GetAuctionPrice);
        Register((int)SC_Protocol.eSC_AuctionResult, Process_AuctionResult);

        // Player 同步属性		
        Register((int)SC_Protocol.eSC_SyncPlayer_Title, Process_Title);
        Register((int)SC_Protocol.eSC_SyncPlayer_Model, Process_Model);
        Register((int)SC_Protocol.eSC_SyncPlayer_Level, Process_Level);
        Register((int)SC_Protocol.eSC_SyncPlayer_Exp, Process_Exp);
        Register((int)SC_Protocol.eSC_SyncPlayer_Color, Process_Color);
        Register((int)SC_Protocol.eSC_SyncPlayer_ClassLevel, Process_ClassLevel);
        Register((int)SC_Protocol.eSC_SyncPlayer_GameMoney, Process_GameMoney);
        Register((int)SC_Protocol.eSC_SyncPlayer_RealMoney, Process_RealMoney);
        Register((int)SC_Protocol.eSC_SyncPlayer_FightRemain, Process_FightRemain);
        Register((int)SC_Protocol.eSC_SyncPlayer_Power, Process_Power);
        Register((int)SC_Protocol.eSC_SyncPlayer_Grow, Process_Grow);
        Register((int)SC_Protocol.eSC_SyncPlayer_RunSpeed, Process_RunSpeed);
        Register((int)SC_Protocol.eSC_SyncPlayer_MountIndex, Process_MountIndex);
        Register((int)SC_Protocol.eSC_SyncPlayer_ArmourItemID, Process_ArmourItemID);
        Register((int)SC_Protocol.eSC_SyncPlayer_WeaponItemID, Process_WeaponItemID);
        Register((int)SC_Protocol.eSC_SyncPlayer_DynAttr, Process_DynAttr);
        Register((int)SC_Protocol.eSC_SyncPlayer_AddBuff, Process_AddBuff);
        Register((int)SC_Protocol.eSC_SyncPlayer_RemoveBuff, Process_RemoveBuff);
        Register((int)SC_Protocol.eSC_SyncPlayer_EatFoodCount, Process_EatFoodCount);
        Register((int)SC_Protocol.eSC_SyncPlayer_ShengWangValue, Process_ShengWangValue);

        Register((int)SC_Protocol.eSC_SyncPlayer_TotalShengWangValue, Process_TotalShengWangValue);
        Register((int)SC_Protocol.eSC_SyncPlayer_DayPeiYangCount, Process_DayPeiYangCount);

        Register((int)SC_Protocol.eSC_Player_AddLingDan, Process_PlayerAddLingDan);
        Register((int)SC_Protocol.eSC_Pet_AddLingDan, Process_PetAddLingDan);
        Register((int)SC_Protocol.eSC_SyncPlayer_ProductOper, Process_ProductOper);

        Register((int)SC_Protocol.eSC_SyncPet_Attr, Process_Pet_Attr);
        Register((int)SC_Protocol.eSC_SyncPlayer_ShowFashion, Process_ShowPlayerFashion);

        Register((int)SC_Protocol.eSC_SyncPlayer_VipAward, Process_VipAward);
        Register((int)SC_Protocol.eSC_SyncPlayer_VIPLevel, Process_VipLevel);

        Register((int)SC_Protocol.eSC_SyncPlayer_GuildId, Process_GuildId);
        Register((int)SC_Protocol.eSC_SyncPlayer_Recharge, Process_Recharge);

        Register((int)SC_Protocol.eSC_Player_PeiYang, Process_Player_PeiYang);
        Register((int)SC_Protocol.eSC_Pet_PeiYang, Process_Pet_PeiYang);

        Register((int)SC_Protocol.eSC_Player_HJ, Process_Player_HL);
        Register((int)SC_Protocol.eSC_Pet_HJ, Process_Pet_HL);


        Register((int)SC_Protocol.eSC_GetPKObjectList, Process_PKObjectList);
        Register((int)SC_Protocol.eSC_PK_Notice, Process_PK_Notice);
        Register((int)SC_Protocol.eSC_PK_Buy_Challenge, Process_PK_BuyChallenge);
        Register((int)SC_Protocol.eSC_PK_Add_Speed, Process_PK_AddSpeed);
        Register((int)SC_Protocol.eSC_PK_Failed, Process_PK_Failed);
        Register((int)SC_Protocol.eSC_AddAward, Process_AddAward);


        // 邮件系统
        Register((int)SC_Protocol.eSC_AllMail, Process_Mail_AllMail);
        Register((int)SC_Protocol.eSC_SendMail, Process_Mail_GetMail);
        Register((int)SC_Protocol.eSC_MailOperResult, Process_Mail_Oper);

        //摇钱
        Register((int)SC_Protocol.eSC_ShakeTree_Resualt, Process_MoneyTree_Resualt);
        Register((int)SC_Protocol.eSC_ShakeTree_GetShake, Process_MoneyTree_GetShake);

        // 查看玩家信息
        Register((int)SC_Protocol.eSC_GetPlayerInfo, Process_ShowPlayerInfo);

        //在线奖励
        Register((int)SC_Protocol.eSC_OnlineRewardNewEvent, Process_OnlineReward_NewGet);
        Register((int)SC_Protocol.eSC_OnlineRewardTimeToGet, Process_OnlineReward_TimeToGet);

        // 成长目标
        Register((int)SC_Protocol.eSC_GrowthTarget, Process_GrowthTarget);

        // 新手引导
        Register((int)SC_Protocol.eSC_NewPlayerGuide, Process_NewPlayerGuide);

        // Npc 同步数据
        Register((int)SC_Protocol.eSC_SyncNpc_Model, Process_UInt);
        Register((int)SC_Protocol.eSC_SyncNpc_Level, Process_UInt);
        Register((int)SC_Protocol.eSC_SyncNpc_MaxHp, Process_UInt);

        //好友
        Register((int)SC_Protocol.eSC_Friend_ErrorCode, Process_Friend_ErrorCode);
        Register((int)SC_Protocol.eSC_Friend_AllData, Process_Friend_AllData);
        Register((int)SC_Protocol.eSC_Friend_AddFriendTips, Process_Friend_AddFriendTips);
        Register((int)SC_Protocol.eSC_Friend_AddResualt, Process_Friend_AddFriendResualt);
        Register((int)SC_Protocol.eSC_Friend_DelResualt, Process_Friend_DelFriendResualt);
        Register((int)SC_Protocol.eSC_Friend_PresentFlowerResualt, Process_Friend_PresentFlowerResualt);
        Register((int)SC_Protocol.eSC_Friend_MoveToBlackList, Process_Friend_AddToBlackList);
        Register((int)SC_Protocol.eSC_Friend_RemoveInBlackList, Process_Friend_ReciveDelBlack);
        Register((int)SC_Protocol.eSC_Friend_GetMarryRequestToPlayer, Process_Friend_ReciveGetMarryAgree);
        Register((int)SC_Protocol.eSC_Friend_GetMarryReault, Process_Friend_ReciveGetMarryResualt);
        Register((int)SC_Protocol.eSC_Friend_DivorceRequestToReciver, Process_Friend_DivorceRequestToReciver);
        Register((int)SC_Protocol.eSC_Friend_DivorceResualt, Process_Friend_DivorceResualt);
        Register((int)SC_Protocol.eSC_Friend_UpdateLevel, Process_Friend_UpdateLevel);
        Register((int)SC_Protocol.eSC_Friend_UpdateOnlineStatus, Process_Friend_UpdateOnlineStatus);
        Register((int)SC_Protocol.eSC_Friend_UpdateSignature, Process_Friend_UpdateSignature);
        Register((int)SC_Protocol.eSC_Friend_UpdateHouseInfo, Process_Friend_UpdateHouseInfo);
        Register((int)SC_Protocol.eSC_Friend_UpdatePresentCount, Process_Friend_UpdatePresentCount);
        Register((int)SC_Protocol.eSC_Friend_UpdateRefreshPresentList, Process_Friend_UpdateRefreshPresentObject);
        Register((int)SC_Protocol.eSC_Friend_AddPresentObject, Process_Friend_AddPresentObject);
        Register((int)SC_Protocol.eSC_Friend_Flower_RankInfo, Process_Friend_RankInfo_Update);

        Register((int)SC_Protocol.eSC_PlayerLoginEnd, Process_PlayerLoginEnd);

        //称号
        Register((int)SC_Protocol.eSC_NickName_AllData, Process_NickName_AllData);
        Register((int)SC_Protocol.eSC_NickName_AddNickName, Process_NickName_AddNickName);
        Register((int)SC_Protocol.eSC_NickName_DelNickName, Process_NickName_DelNickName);
        Register((int)SC_Protocol.eSC_NickName_SetCurName, Process_NickName_SetCurName);
        Register((int)SC_Protocol.eSC_SyncPlayer_NickName, Process_NickName_SyncNickName);

        //防沉迷
        Register((int)SC_Protocol.eSC_WallowNotice, Process_WallowNotice);
        Register((int)SC_Protocol.eSC_WallowOneHour, Process_WallowOneHour);
        Register((int)SC_Protocol.eSC_WallowTwoHour, Process_WallowTwoHour);
        Register((int)SC_Protocol.eSC_WallowThreeHour, Process_WallowThreeHour);
        Register((int)SC_Protocol.eSC_WallowShut, Process_WallowShut);

        Register((int)SC_Protocol.eSC_Player_Random_AddLD, Process_Player_RandomAddLD);
        Register((int)SC_Protocol.eSC_Pet_Random_AddLD, Process_Pet_RandomAddLD);

        Register((int)SC_Protocol.eSC_SevenTargetAllStatus, Process_AllSevenTarget);
        Register((int)SC_Protocol.eSC_SevenTargetItemStatus, Process_ItemSevenTarget);

        Register((int)SC_Protocol.eSC_ZYL_MonsterInfo, Process_ZYL_MonsterInfo);
        Register((int)SC_Protocol.eSC_ZYL_BaseData, Process_ZYL_BaseData);
        Register((int)SC_Protocol.eSC_ZYL_UltraKill, Process_ZYL_Notice_UltraKil);

        //打坐
        Register((int)SC_Protocol.eSC_Meditation_Start, Process_MeditationStart);
        Register((int)SC_Protocol.eSC_Meditation_End, Process_MeditationEnd);
        Register((int)SC_Protocol.eSC_Meditation_UpdateRewardExp, Process_MeditationUpdateRewardExp);

        //扫荡
        Register((int)SC_Protocol.eSC_SaoDang_Result, Process_SaoDang_Result);

        // 签到
        Register((int)SC_Protocol.eSC_DailySignStatus, Process_Daily_Status);
        Register((int)SC_Protocol.eSC_DailySignResult, Process_Daily_Result);

        // guild
        Register((int)SC_Protocol.eSC_Guild_RespondAllSynInfo, Process_Guild_RecvAllSynInfo);
        Register((int)SC_Protocol.eSC_Guild_BroadAddSynInfo, Process_Guild_RecvBroadAddSynInfo);
        Register((int)SC_Protocol.eSC_Guild_BroadRemoveSynInfo, Process_Guild_RecvBroadRemoveSynInfo);
        Register((int)SC_Protocol.eSC_Guild_UpdateSynInfoList, Process_Guild_RecvUpdateSynInfoList);
        Register((int)SC_Protocol.eSC_Guild_RespondBaseInfo, Process_Guild_RecvBaseInfo);
        Register((int)SC_Protocol.eSC_Guild_RespondAllMemInfo, Process_Guild_RecvAllMemInfo);
        Register((int)SC_Protocol.eSC_Guild_RespondAllApplyInfo, Process_Guild_RecvAllApplyInfo);
        Register((int)SC_Protocol.eSC_Guild_BroadAddMem, Process_Guild_RecvBroadAddMem);
        Register((int)SC_Protocol.eSC_Guild_BroadRemoveMem, Process_Guild_RecvBroadRemoveMem);
        Register((int)SC_Protocol.eSC_Guild_UpdateMemInfoList, Process_Guild_RecvUpdateMemInfoList);
        Register((int)SC_Protocol.eSC_Guild_SendSelfApplyList, Process_Guild_RecvSelfApplyList);
        Register((int)SC_Protocol.eSC_Guild_BroadAddApply, Process_Guild_RecvBroadAddApply);
        Register((int)SC_Protocol.eSC_Guild_BroadRemoveApply, Process_Guild_RecvBroadRemoveApply);
        Register((int)SC_Protocol.eSC_Guild_UpdateApplyInfoList, Process_Guild_RecvUpdateApplyInfoList);
        Register((int)SC_Protocol.eSC_Guild_BroadAnno, Process_Guild_BroadAnno);
        Register((int)SC_Protocol.eSC_Guild_BroadTran, Process_Guild_BroadTran);
        Register((int)SC_Protocol.eSC_Guild_Feedback, Process_Guild_RecvFeedback);

        //活跃度奖励
        Register((int)SC_Protocol.eSC_DayActivity_AllData, Process_DayActivity_InitData);
        Register((int)SC_Protocol.eSC_DayActivity_AwardInfo, Process_DayActivity_AwardInfo);
        Register((int)SC_Protocol.eSC_DayActivity_ItemInfo, Process_DayActivity_ItemInfo);
        Register((int)SC_Protocol.eSC_DayActivity_ResetInfo, Process_DayActivity_ResetInfo);
		Register((int)SC_Protocol.eSC_DayActivity_ActivityValue, Process_DayActivity_ReciveActivityValue);
    }

    private void Register(int cmd, PacketProcessFuc method)
    {
        if (m_ProcessMethod.Contains(cmd))
        {
            Log.Write(LogLevel.ERROR, "[WARN] PacketGate.Register failed, cmd: " + cmd);
            return;
        }
        m_ProcessMethod.Add(cmd, method);
    }

    public void ProcessPacket(NetPacket packet)
    {
        if (m_ProcessMethod.Contains(packet.Head.Command) == false)
        {
            //--4>: 不用 LogLeve.ERROR, 客户端没处理的一些包先不用管
            Log.Write(LogLevel.WARN, "[ERROR] PacketGate.ProcessPacket: Attempt to packet with commad: " + packet.Head.Command);
            return;
        }

        //--4>TEST:
        //Log.Write("[TEST] cmd:{0}, size:{1}", packet.Head.Command, packet.Message.Length);

        PacketProcessFuc fun = m_ProcessMethod[packet.Head.Command] as PacketProcessFuc;
        if (fun != null)
        {
            fun(packet);
        }
    }

    #region login packet processer

    public void Process_CheckAccountResult(NetPacket packet)
    {
        LC_CheckAccountResult msg = LC_CheckAccountResult.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnCheckAccountResult(msg);
    }

    public void Process_CharServerInfo(NetPacket packet)
    {
        LC_CharServerInfo msg = LC_CharServerInfo.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnCharServerInfo(msg);
    }

    public void Process_SelectServerRet(NetPacket packet)
    {
        LC_SelectServerRet msg = LC_SelectServerRet.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnSelectServerRet(msg);
    }

    #endregion

    #region charserver packet processer

    public void Process_ChC_Return(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.Msg_Server_Return, Msg_Return.ParseFrom(packet.Message));
    }

    public void Process_LoadCharList(NetPacket packet)
    {
        ChC_LoadCharList msg = ChC_LoadCharList.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnLoadCharList(msg);
    }

    public void Process_CreatePlayer(NetPacket packet)
    {
        ChC_CreatePlayer msg = ChC_CreatePlayer.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnCreatePlayer(msg);
    }

    public void Process_GameServerInfo(NetPacket packet)
    {
        ChC_GameServerInfo msg = ChC_GameServerInfo.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnGameServerInfo(msg);
    }

    #endregion


    #region gameserver packet processer

    public void Process_EnterGame(NetPacket packet)
    {
        SC_EnterGame msg = SC_EnterGame.ParseFrom(packet.Message);
        XLogicWorld.SP.LoginProc.OnEnterGame(msg);
    }

    public void Process_PlayerAppear(NetPacket packet)
    {
        SC_PlayerAppear msg = SC_PlayerAppear.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.On_SC_PlayerAppear(msg);
    }

    public void Process_PlayerAppearData(NetPacket packet)
    {
        SC_PlayerAppearData msg = SC_PlayerAppearData.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.On_SC_ObjectAppearData(EObjectType.OtherPlayer, msg.ObjectId, msg);
    }

    public void Process_MissionReceiveResult(NetPacket packet)
    {
        SC_MissionReceiveResult msg = SC_MissionReceiveResult.ParseFrom(packet.Message);
        XMissionManager.SP.missionReceiveResult(msg.MissionID, (EMissionMessage)(msg.ReceiveResult));
    }

    public void Process_MissionReferResult(NetPacket packet)
    {
        SC_MissionReferResult msg = SC_MissionReferResult.ParseFrom(packet.Message);
        EMissionMessage result = (EMissionMessage)(msg.ReferResult);
        XMissionManager.SP.missionReferResult(msg.MissionID, result);
    }

    public void Process_MissionGiveUpResult(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XMissionManager.SP.missionGiveUpResult(msg.Data);
    }

    public void Process_ReferExistMission(NetPacket packet)
    {
        SC_ReferExistMission msg = SC_ReferExistMission.ParseFrom(packet.Message);
        XMissionManager.SP.addReferMission(msg.MissionID);
    }

    public void Process_ActiveMissionState(NetPacket packet)
    {
        SC_ActiveMission msg = SC_ActiveMission.ParseFrom(packet.Message);

        MissionMessage mission = new MissionMessage();
        mission.m_uiMissionID = msg.MissionID;
        mission.m_uiMissionItemCount1 = msg.ItemCount1;
        mission.m_uiMissionItemCount2 = msg.ItemCount2;
        mission.m_uiMissionItemCount3 = msg.ItemCount3;

        mission.m_uiMissionKillCount1 = msg.KillCount1;
        mission.m_uiMissionKillCount2 = msg.KillCount2;
        mission.m_uiMissionKillCount3 = msg.KillCount3;

        mission.m_bMissionCompleted = msg.IsCompleteled;

        XMissionManager.SP.actionMission(mission);
    }

    public void Process_HillSeaBookUpdate(NetPacket packet)
    {
        SC_HillSeaBookMsg msg = SC_HillSeaBookMsg.ParseFrom(packet.Message);
        XHillSeaBookManager.SP.updateHillSeaBook(msg);
    }

    public void Process_PlayerDisappear(NetPacket packet)
    {
        SC_PlayerDisappear msg = SC_PlayerDisappear.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.DisappearObject(EObjectType.OtherPlayer, msg.PlayerId);
    }

    public void Process_StartMove(NetPacket packet)
    {
        SC_StartMove msg = SC_StartMove.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.OtherPlayer, msg.ObjectId) as XPlayer;
        if (player != null)
        {
            player.On_SC_StartMove(msg);
        }
    }

    public void Process_StopMove(NetPacket packet)
    {
        SC_StopMove msg = SC_StopMove.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.OtherPlayer, msg.ObjectId) as XPlayer;
        if (player != null)
        {
            player.On_SC_StopMove(msg);
        }
    }

    public void Process_ForceSetPosition(NetPacket packet)
    {
        SC_ForceSetPosition msg = SC_ForceSetPosition.ParseFrom(packet.Message);
        XPlayer player = null;
        if (XLogicWorld.SP.MainPlayer.ID == msg.ObjectId)
        {
            player = XLogicWorld.SP.MainPlayer;
        }
        else
        {
            player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.MainPlayer, msg.ObjectId) as XPlayer;
        }
        if (player != null)
        {
            float posY = XLogicWorld.SP.SceneManager.GetExactHeightAt(msg.PosX, msg.PosZ);
            player.ForceSetPosition(new Vector3(msg.PosX, posY, msg.PosZ), new Vector3(0f, msg.Dir, 0f));
        }
    }

    public void Process_NpcAppear(NetPacket packet)
    {
        SC_NpcAppear msg = SC_NpcAppear.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.On_SC_NpcAppear(msg);
    }

    public void Process_NpcAppearData(NetPacket packet)
    {
        SC_NpcAppearData msg = SC_NpcAppearData.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.On_SC_ObjectAppearData(EObjectType.Npc, msg.ObjectId, msg);
    }

    public void Process_NpcDisappear(NetPacket packet)
    {
        SC_NpcDisappear msg = SC_NpcDisappear.ParseFrom(packet.Message);
        XLogicWorld.SP.ObjectManager.DisappearObject(EObjectType.Npc, msg.NpcId);
    }

    public void Process_Jump(NetPacket packet)
    {
        SC_Jump msg = SC_Jump.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.OtherPlayer, msg.ObjectId) as XPlayer;
        if (player != null)
        {
            player.On_SC_Jump(msg);
        }
    }

    public void Process_JumpOver(NetPacket packet)
    {
        SC_JumpOver msg = SC_JumpOver.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.ObjectId);
        if (null != player)
        {
            player.On_SC_JumpOver(msg);
        }
    }

    public void Process_Sit(NetPacket packet)
    {
        SC_Sit msg = SC_Sit.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.OtherPlayer, msg.ObjectId) as XPlayer;
        if (player != null)
        {
            player.On_SC_Sit(msg);
        }
    }

    public void Process_StopSit(NetPacket packet)
    {
        SC_StopSit msg = SC_StopSit.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.OtherPlayer, msg.ObjectId) as XPlayer;
        if (player != null)
        {
            player.On_SC_StopSit(msg);
        }
    }

    public void Process_Chat(NetPacket packet)
    {
        SC_Chat msg = SC_Chat.ParseFrom(packet.Message);
        XEventManager.SP.SendEvent(EEvent.Chat_OnChatMsg, msg);
    }

    public void Process_ChangeScene(NetPacket packet)
    {
        SC_ChangeScene msg = SC_ChangeScene.ParseFrom(packet.Message);
        Vector3 pos = new Vector3(msg.PosX, msg.PosY, msg.PosZ);

        //		ESceneType sceneType	= XLogicWorld.SP.SceneManager.LoadedSceneType;
        //		if(sceneType == ESceneType.NormalScene || sceneType == ESceneType.NormalCutScene ||
        //				sceneType == ESceneType.CutScene)
        //		{
        //			XLogicWorld.SP.MainPlayer.Position	= pos;
        //		}
        //		else
        //		{
        //			XLogicWorld.SP.MainPlayer.CacheSceneInitPos	= pos;
        //		}
        //		XLogicWorld.SP.MainPlayer.CacheSceneInitPos	= pos;

        XLogicWorld.SP.MainPlayer.Position = pos;
        XLogicWorld.SP.LoadScene((int)(msg.SceneId));
    }

    public void Process_PlayerBaseInfo(NetPacket packet)
    {
        SC_PlayerBaseInfo msg = SC_PlayerBaseInfo.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.On_SC_PlayerBaseInfo(msg);
    }

    public void Process_PlayerDetailInfo(NetPacket packet)
    {
        SC_PlayerDetailInfo msg = SC_PlayerDetailInfo.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.On_SC_PlayerDetailInfo(msg);
    }

    public void Process_EnterClientScene(NetPacket packet)
    {
        SC_EnterClientScene msg = SC_EnterClientScene.ParseFrom(packet.Message);
        XLogicWorld.SP.SubSceneManager.EnterClientScene(msg);
    }

    //    public void Process_BattleRoundInfo(NetPacket packet)
    //    {
    //        //XLogicWorld.SP.SubSceneManager.On_SC_BattleRoundInfo(SC_BattleRoundInfo.ParseFrom(packet.Message));
    //    }

    public void Process_BattleData(NetPacket packet)
    {
        XLogicWorld.SP.SubSceneManager.On_SC_BattleData(SC_BattleData.ParseFrom(packet.Message));
    }

    public void Process_BattleResultPVP(NetPacket packet)
    {
        XBattleManager.SP.On_SC_BattleResultPVP(SC_BattleResult.ParseFrom(packet.Message));
    }

    public void Process_BattleResultPVE(NetPacket packet)
    {
        XBattleManager.SP.On_SC_BattleResultPVE(SC_BattleResult.ParseFrom(packet.Message));
    }

    public void Process_ShanHT_Info(NetPacket packet)
    {
        XShanHTManager.SP.ON_SC_ShanHT_Info(SC_ShanHT_Info.ParseFrom(packet.Message));
    }

    public void Process_ShanHT_Rank(NetPacket packet)
    {
        SC_ShanHT_Rank msg = SC_ShanHT_Rank.ParseFrom(packet.Message);
        XShanHTManager.SP.ON_SC_ShanHT_Rank(msg);
    }

    public void Process_ShanHT_Notice(NetPacket packet)
    {
        SC_ShanHT_Notice msg = SC_ShanHT_Notice.ParseFrom(packet.Message);

        XCfgShanHTBase curConfig = XCfgShanHTBaseMgr.SP.GetConfig(msg.ShanHeBaseID);
        if (curConfig == null)
            return;
        XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 189, msg.Name, curConfig.Name);
    }

    public void Process_ShanHT_Notice_Item(NetPacket packet)
    {
        SC_ShanHT_Notice_Item msg = SC_ShanHT_Notice_Item.ParseFrom(packet.Message);

        XCfgShanHTLevel curConfig = XCfgShanHTLevelMgr.SP.GetConfig(msg.ShanHeLevelID);
        if (curConfig == null)
            return;
        XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(msg.ItemID);
        if (cfgItem == null)
            return;
        XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 190, msg.Name, curConfig.Name, cfgItem.Name);
    }

    public void Process_BattleResultShanHT(NetPacket packet)
    {
        XBattleManager.SP.On_SC_BattleResultShanHT(SC_BattleResult.ParseFrom(packet.Message));
    }

    public void Process_CopySceneState(NetPacket packet)
    {
        SC_UIntArr msg = SC_UIntArr.ParseFrom(packet.Message);
        Log.Write("[TEST] CopySceneState: {0}", msg.SerializedSize);
        XLogicWorld.SP.SubSceneManager.On_SC_ProcessSceneData(msg);

    }

    public void Process_PlayerBattlePos(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.BattlePos = msg.Data;
    }

    public void Process_CopySceneResult(NetPacket packet)
    {
        SC_CopySceneResult msg = SC_CopySceneResult.ParseFrom(packet.Message);
        XLogicWorld.SP.SubSceneManager.On_SC_CopySceneResult(msg);
    }

    public void Process_CopySceneAwardSel(NetPacket packet)
    {
        SC_AwardSel msg = SC_AwardSel.ParseFrom(packet.Message);
        XLogicWorld.SP.SubSceneManager.On_SC_SubSceneAwardSel(msg);
    }

    public void Process_ReadyBattleObject(NetPacket packet)
    {
        PB_ObjectDataList msg = PB_ObjectDataList.ParseFrom(packet.Message);
        XLogicWorld.SP.SubSceneManager.On_SC_SetBattleObject(msg);
    }

    public void Process_ShowNpcDialog(NetPacket packet)
    {
        SC_ShowNpcDialog msg = SC_ShowNpcDialog.ParseFrom(packet.Message);
        //XNpc npc = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.Npc, msg.NpcId) as XNpc;

    }

    public void Process_BeginShowNpcDialog(NetPacket packet)
    {

    }

    public void Process_AddDialogEvent(NetPacket packet)
    {
        SC_AddDialogEvent msg = SC_AddDialogEvent.ParseFrom(packet.Message);

    }

    public void Process_ActiveQuestList(NetPacket packet)
    {
        SC_ActiveQuestList msg = SC_ActiveQuestList.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_ActiveQuestList(msg);
    }

    public void Process_QuestMagic(NetPacket packet)
    {
        SC_QuestMagic msg = SC_QuestMagic.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_QuestMagic(msg);
    }

    public void Process_AllQuestBit(NetPacket packet)
    {
        SC_AllQuestBit msg = SC_AllQuestBit.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_AllQuestBit(msg);
    }

    public void Process_SetQuestState(NetPacket packet)
    {
        SC_SetQuestState msg = SC_SetQuestState.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_SetQuestState(msg);
    }

    public void Process_SetQuestMagic(NetPacket packet)
    {
        SC_SetQuestMagic msg = SC_SetQuestMagic.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_SetQuestMagic(msg);
    }

    public void Process_AcceptQuest(NetPacket packet)
    {
        SC_AcceptQuest msg = SC_AcceptQuest.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_AcceptQuest(msg);
    }

    public void Process_FinishQuest(NetPacket packet)
    {
        SC_FinishQuest msg = SC_FinishQuest.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_FinishQuest(msg);
    }

    public void Process_DelQuest(NetPacket packet)
    {
        SC_DelQuest msg = SC_DelQuest.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_DelQuest(msg);
    }

    public void Process_UpdateQuestFlag(NetPacket packet)
    {
        SC_UpdateQuestFlag msg = SC_UpdateQuestFlag.ParseFrom(packet.Message);
        XLogicWorld.SP.QuestManager.On_SC_UpdateQuestFlag(msg);
    }

    public void Process_ItemList(NetPacket packet)
    {
        SC_ItemList msg = SC_ItemList.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemList(msg);
    }

    public void Process_ItemComStrengthen(NetPacket packet)
    {
        SC_ItemComStrengthen msg = SC_ItemComStrengthen.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemComStrengthen(msg);
    }

    public void Process_ItemMoneyStrengthen(NetPacket packet)
    {
        SC_ItemMoneyStrengthen msg = SC_ItemMoneyStrengthen.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemMoneyStrengthen(msg);
    }

    public void Process_ItemDecompose(NetPacket packet)
    {
        SC_ItemDecompose msg = SC_ItemDecompose.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemDecompose(msg);
    }

    public void Process_ItemGemInlay(NetPacket packet)
    {
        SC_ItemGemInlay msg = SC_ItemGemInlay.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemGemInlay(msg);
    }

    public void Process_ItemGemRemove(NetPacket packet)
    {
        SC_ItemGemRemove msg = SC_ItemGemRemove.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemGemRemove(msg);
    }

    public void Process_ItemGemRemoveAll(NetPacket packet)
    {
        SC_ItemGemRemoveAll msg = SC_ItemGemRemoveAll.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemGemRemoveAll(msg);
    }

    public void Process_ItemUpdate(NetPacket packet)
    {
        SC_ItemUpdate msg = SC_ItemUpdate.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemUpdate(msg);
    }

    public void Process_ItemSeal(NetPacket packet)
    {
        SC_ItemList msg = SC_ItemList.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemSeal(msg);
    }

    public void Process_ItemSplit(NetPacket packet)
    {
        SC_ItemSplit msg = SC_ItemSplit.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemSplit(msg);
    }

    public void Process_ItemArrange(NetPacket packet)
    {
        SC_ItemList msg = SC_ItemList.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemArrange(msg);
    }

    public void Process_ItemShow(NetPacket packet)
    {
        PB_ItemInfo msg = PB_ItemInfo.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemShow(msg);
    }

    public void Process_BuyBackList(NetPacket packet)
    {
        SC_ItemList msg = SC_ItemList.ParseFrom(packet.Message);

        XShopItemMgr.SP.updateBuyBackList(msg);
    }

    public void Process_BuyItemFail(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);

        XShopItemMgr.SP.BuyItemFail(msg);
    }

    public void Process_BuyItemLimiteList(NetPacket packet)
    {
        SC_NPCLimitShopItemList msg = SC_NPCLimitShopItemList.ParseFrom(packet.Message);

        XEventManager.SP.SendEvent(EEvent.ShopDialog_NpcLimiteList, msg);
    }

    public void Process_ItemSpace(NetPacket packet)
    {
        SC_ItemSpace msg = SC_ItemSpace.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemSpace(msg);
    }

    public void Process_ItemCombine(NetPacket packet)
    {
        SC_ItemCombine msg = SC_ItemCombine.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemCombine(msg);
    }

    public void Process_ItemDrop(NetPacket packet)
    {
        SC_ItemDrop msg = SC_ItemDrop.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemDrop(msg);
    }

    public void Process_ItemUse(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XItemPacketProcess.SP.On_SC_ItemUse(msg);
    }

    public void Process_AllPetData(NetPacket packet)
    {
        SC_AllPetData msg = SC_AllPetData.ParseFrom(packet.Message);
        XLogicWorld.SP.PetManager.On_SC_AllPetData(msg);
    }

    public void Process_AddPet(NetPacket packet)
    {
        SC_OnePet msg = SC_OnePet.ParseFrom(packet.Message);
        XLogicWorld.SP.PetManager.SetPet(msg);
        XEquipGetMgr.SP.FlushEquipHint();
    }

    public void Process_DelPet(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.PetManager.DelPet(msg.Data);
    }

    void Process_SyncPet_Attr(NetPacket packet)
    {
        switch (packet.Head.Command)
        {
            case (int)SC_Protocol.eSC_SyncPet_BattlePos:
                XLogicWorld.SP.PetManager.On_Sync_BattlePos(packet);
                break;
            case (int)SC_Protocol.eSC_SyncPet_Loyal:
                XLogicWorld.SP.PetManager.On_Sync_Loyal(packet);
                break;
            case (int)SC_Protocol.eSC_SyncPet_Level:
                XEventManager.SP.SendEvent(EEvent.Update_Level);
                break;
            case (int)SC_Protocol.eSC_SyncPet_Exp:
                XLogicWorld.SP.PetManager.On_Sync_Exp(packet);
                break;
            default:
                break;
        }
    }

    void Process_Pet_ChangeName(NetPacket packet)
    {
        SC_PetChangeName msg = SC_PetChangeName.ParseFrom(packet.Message);
        XLogicWorld.SP.PetManager.SetName(msg.PetIndex, msg.Name);
    }

    public void Process_ActiveSkill(NetPacket packet)
    {
        SC_UIntArr msg = SC_UIntArr.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_ActiveSkill(msg);
    }

    public void Process_LearnSkill(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_LearnSkill(msg);
    }

    public void Process_UpgradeSkill(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_UpgradeSkill(msg);

    }

    public void Process_ForgetSkill(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_ForgetSkill(msg);
    }

    public void Process_SkillPoint(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_SkillPoint(msg);
    }

    public void Process_EquipedSkill(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        SkillManager.SP.On_SC_EquipedSkill(msg);
    }

    // 坐骑 start
    public void Process_ActiveMount(NetPacket packet)
    {
        SC_ActiveMount msg = SC_ActiveMount.ParseFrom(packet.Message);
        XMountManager.SP.On_SC_ActiveMount(msg);
    }

    public void Process_AddMount(NetPacket packet)
    {
        SC_OneMount msg = SC_OneMount.ParseFrom(packet.Message);
        XMountManager.SP.On_SC_AddMount(msg);
    }

    public void Process_DelMount(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XMountManager.SP.On_SC_DelMount(msg);
    }

    public void Process_MountPeiYangCount(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XMountManager.SP.On_SC_PeiYangCount(msg);
    }

    public void Process_MountPeiYangResult(NetPacket packet)
    {
        SC_PeiYangResult msg = SC_PeiYangResult.ParseFrom(packet.Message);
        XMountManager.SP.On_SC_PeiYangResult(msg);
    }
    // 坐骑 end

    public void Process_Title(NetPacket packet)
    {
        SC_String msg = SC_String.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            //--4>TODO:
            //player.SetTitle(msg.Data);
        }
    }

    public void Process_Model(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            player.ModelId = msg.Data;
        }
    }

    public void Process_Level(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            int nLev = (int)msg.Data;
            //if (nLev > player.Level && player is XMainPlayer)
            if (player is XMainPlayer)
            {
                XU3dEffect levEffect = new XU3dEffect(900001);
                player.AttachGo(ESkeleton.eCapsuleBottom, levEffect.m_gameObject, Vector3.zero, Vector3.zero);
            }
            player.Level = nLev;

            XGrowthTargetManger.SP.ON_SC_Player_LevelUp();
        }
    }

    public void Process_Exp(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.Exp = msg.Data;
    }

    public void Process_Color(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            //--4>TODO:
            //player.SetColor(msg.Data);
        }
    }

    public void Process_ClassLevel(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            //--4>TODO:
            //player.SetClassLevel(msg.Data);
        }
    }

    public void Process_GameMoney(NetPacket packet)
    {
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        //XMainPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.MainPlayer, msg.Uid) as XMainPlayer;
        XMainPlayer player = XLogicWorld.SP.MainPlayer;
        if (player != null)
        {
            player.GameMoney = msg.Data;
        }
    }

    public void Process_RealMoney(NetPacket packet)
    {
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        //XMainPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.MainPlayer, msg.Uid) as XMainPlayer;
        XMainPlayer player = XLogicWorld.SP.MainPlayer;
        if (player != null)
        {
            player.RealMoney = msg.Data;
        }
    }

    public void Process_FightRemain(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XMainPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.MainPlayer, msg.Uid) as XMainPlayer;
        if (player != null)
        {
            //--4>TODO:
            //player.SetFightRemain(msg.Data);
        }
    }

    public void Process_Power(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XMainPlayer player = XLogicWorld.SP.MainPlayer;
        if (player != null)
        {
            player.Power = msg.Data;
        }
    }

    public void Process_Grow(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XMainPlayer player = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.MainPlayer, msg.Uid) as XMainPlayer;
        if (player != null)
        {
            //--4>TODO:
            //player.SetGrow(msg.Data);
        }
    }

    void Process_RunSpeed(NetPacket packet)
    {
        SC_Float msg = SC_Float.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null != player)
        {
            player.Speed = msg.Data;
        }
    }

    void Process_MountIndex(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null != player)
        {
            player.MountIndex = (ushort)msg.Data;
        }
    }

    void Process_ArmourItemID(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null != player)
        {
            player.ArmourItemID = msg.Data;
        }
    }

    void Process_WeaponItemID(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null != player)
        {
            player.WeaponItemID = msg.Data;
        }
    }

    void Process_DynAttr(NetPacket packet)
    {
        int OldBattleValue = XLogicWorld.SP.MainPlayer.BattleValue;
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null != player)
        {
            int key = (int)(msg.Data >> 32);
            int val = (int)(msg.Data);
            player.DynSet((EShareAttr)key, val);
        }
        int deltaBattleValue = XLogicWorld.SP.MainPlayer.BattleValue - OldBattleValue;
        if (deltaBattleValue > 0)
            XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip, 522, deltaBattleValue);
    }

    void Process_Pet_Attr(NetPacket packet)
    {
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        int PetIndex = (int)msg.Uid;
        if (PetIndex > 20)
            return;
        XPet pet = XLogicWorld.SP.PetManager.AllPet[(int)PetIndex];
        if (null != pet)
        {
            int key = (int)(msg.Data >> 32);
            int val = (int)(msg.Data);
            pet.DynSet((EShareAttr)key, val);
        }
    }

    void Process_AddBuff(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;
        UInt64 buff64 = msg.Data;
        uint nBuffId = (uint)(buff64 >> 32);
        byte btBuffLevel = (byte)((int)(buff64) >> 16);
        byte btBuffLayer = (byte)(buff64);
        player.BuffOper.AddBuff(nBuffId, btBuffLevel, btBuffLayer, false, 0);
    }

    void Process_RemoveBuff(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;
        player.BuffOper.RemoveBuff(msg.Data);
    }

    void Process_EatFoodCount(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.EatFoodCount = msg.Data;
    }

    void Process_ShengWangValue(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.ShengWangValue = msg.Data;
    }

    void Process_TotalShengWangValue(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.TotalShengWangValue = msg.Data;
    }

    void Process_DayPeiYangCount(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.DayPeiYangCount = msg.Data;
    }

    void Process_ActiveBuff(NetPacket packet)
    {
        SC_UInt64Arr msg = SC_UInt64Arr.ParseFrom(packet.Message);
        for (int i = 0; i < msg.DataCount; i += 2)
        {
            UInt64 buff64 = msg.GetData(i);
            UInt64 buffRL = msg.GetData(i + 1);
            uint nBuffId = (uint)(buff64 >> 32);
            byte btBuffLevel = (byte)((int)(buff64) >> 16);
            byte btBuffLayer = (byte)(buff64);
            XLogicWorld.SP.MainPlayer.BuffOper.AddBuff(nBuffId, btBuffLevel, btBuffLayer, true, (int)buffRL);
        }
    }

    void Process_SetBuffLayer(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.BuffOper.SetBuffLayer((uint)msg.Uid, (byte)msg.Data);
    }

    void Process_SetBuffCount(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.BuffOper.SetBuffCount((uint)msg.Uid, (byte)msg.Data);
    }

    void Process_ActiveCareer(NetPacket packet)
    {
        SC_ActiveCareer msg = SC_ActiveCareer.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_ActiveCareer(msg);
    }

    void Process_LearnCareer(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_LearnCareer((byte)msg.Data, 1, 0);
    }

    void Process_UpgradeCareer(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_UpgradeCareer((byte)(msg.Data >> 16), (byte)(msg.Data));
    }

    void Process_ForgetCareer(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_ForgetCareer((byte)msg.Data);
    }

    void Process_ProductStrength(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_SetStrength(msg.Data);
    }

    void Process_ProductExp(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        int data = msg.Data;
        XProductManager.SP.On_SC_SetExp((byte)(data >> 16), (ushort)data);
    }

    void Process_AddGatherRec(NetPacket packet)
    {
        SC_UIntArr msg = SC_UIntArr.ParseFrom(packet.Message);
        foreach (uint data in msg.DataList)
        {
            XProductManager.SP.On_SC_AddGatherRec(data);
        }
    }

    void Process_AddFormula(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_AddFormula(msg.Data);
    }

    void Process_GathterOnceOver(NetPacket packet)
    {
        XProductManager.SP.On_SC_GatherOnceOver();
    }

    void Process_ProduceOnceOver(NetPacket packet)
    {
        SC_ProduceOnce msg = SC_ProduceOnce.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_ProduceOnceOver(msg);
    }

    void Process_ApplyProductError(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XProductManager.SP.On_SC_ProductError(msg.Data);
    }

    void Process_AllAuction(NetPacket packet)
    {
        PB_AllAuction msg = PB_AllAuction.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.On_SC_AllAuction(msg);
    }

    void Process_MyAuction(NetPacket packet)
    {
        PB_AllMyAuction msg = PB_AllMyAuction.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.On_SC_AllMyAuction(msg);
    }

    void Process_UpdateAuctionItem(NetPacket packet)
    {
        PB_AuctionItem msg = PB_AuctionItem.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.On_SC_UpdateAuctionItem(msg);
    }

    void Process_UpdateMyAuction(NetPacket packet)
    {
        PB_OneAuction msg = PB_OneAuction.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.On_SC_UpdateMyAuction(msg);
    }

    void Process_GetAuctionPrice(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.ON_SC_GetAuctionPrice(msg);
    }

    void Process_AuctionResult(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.ON_SC_AuctionResult(msg.Data);
    }

    void Process_UpdateAuctionHistroyInfo(NetPacket packet)
    {
        PB_AuctionHistroyInfo msg = PB_AuctionHistroyInfo.ParseFrom(packet.Message);
        XLogicWorld.SP.AuctionManager.On_SC_AuctionHistroyInfo(msg);
    }

    void Process_ProductOper(NetPacket packet)
    {
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;

        Int64 data = msg.Data;
        if (0 == data)
        {
            if (player == XLogicWorld.SP.MainPlayer)
            {
                XProductManager.SP.On_SC_EndProduct();
            }
            else
            {
                player.QuitProduct();
            }
        }
        else
        {
            if (player == XLogicWorld.SP.MainPlayer)
            {
                XProductManager.SP.On_SC_StartProduct(data);
            }
            else
            {
                player.StartProduct(data);
            }
        }
    }

    void Process_ShowPlayerFashion(NetPacket packet)
    {
        SC_Int64 msg = SC_Int64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;
        uint state = (uint)(msg.Data >> 32);
        uint fashionid = (uint)(msg.Data & 0x00000000FFFFFFFF);
        player.ShowFashion = state;
        player.FashionId = fashionid;
    }

    void Process_VipAward(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;

        (player as XMainPlayer).VipAward = msg.Data;
        XEventManager.SP.SendEvent(EEvent.Vip_UpdateInfo);
    }

    void Process_VipLevel(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;

        (player as XMainPlayer).VIPLevel = msg.Data;
        XEventManager.SP.SendEvent(EEvent.Vip_UpdateInfo);
    }

    void Process_GuildId(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;

        (player as XMainPlayer).GuildId = msg.Data;
    }

    void Process_Recharge(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (null == player)
            return;

        (player as XMainPlayer).Recharge = msg.Data;
    }

    public void Process_UInt(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XNpc npc = XLogicWorld.SP.ObjectManager.GetObject(EObjectType.Npc, msg.Uid) as XNpc;
        if (npc == null)
        {
            return;
        }
        switch (packet.Head.Command)
        {
            case (int)SC_Protocol.eSC_SyncNpc_Model:
                npc.ModelId = msg.Data;
                break;
            case (int)SC_Protocol.eSC_SyncNpc_Level:
                npc.Level = (int)msg.Data;
                break;
            case (int)SC_Protocol.eSC_SyncNpc_MaxHp:
                npc.MaxHp = (int)msg.Data;
                break;
            default:
                break;
        }
    }

    public void Process_PlayerAddLingDan(NetPacket packet)
    {
        SC_Player_AddLingDan msg = SC_Player_AddLingDan.ParseFrom(packet.Message);
        uint index = msg.Index;
        uint itemIndex = msg.DelItemIndex;
        uint addValue = msg.AddValue;

        XMainPlayer player = XLogicWorld.SP.MainPlayer;
        player.ItemManager.DelItem((ushort)itemIndex, 1);

        if (index == 0)
            player.WuLi += addValue;
        else if (index == 1)
            player.LingQiao += addValue;
        else if (index == 2)
            player.TiZhi += addValue;
        else if (index == 3)
            player.ShuFa += addValue;


    }

    public void Process_PetAddLingDan(NetPacket packet)
    {
        SC_Pet_AddLingDan msg = SC_Pet_AddLingDan.ParseFrom(packet.Message);
        uint index = msg.Index;
        uint PetIndex = msg.PetIndex;
        uint itemIndex = msg.DelItemIndex;
        uint addValue = msg.AddValue;

        XPet pet = XLogicWorld.SP.PetManager.AllPet[index];
        if (pet == null)
            return;

        XLogicWorld.SP.MainPlayer.ItemManager.DelItem((ushort)itemIndex, 1);

        if (index == 0)
            pet.WuLi += addValue;
        else if (index == 1)
            pet.LingQiao += addValue;
        else if (index == 2)
            pet.TiZhi += addValue;
        else if (index == 3)
            pet.ShuFa += addValue;


    }

    public void Process_Player_PeiYang(NetPacket packet)
    {
        SC_Player_PeiYang msg = SC_Player_PeiYang.ParseFrom(packet.Message);
        XLogicWorld.SP.MainPlayer.Grow += msg.Value;
        if (msg.Type == 0)
            XLogicWorld.SP.MainPlayer.GameMoney -= msg.Money;
        else
            XLogicWorld.SP.MainPlayer.RealMoney -= msg.Money;

        XLogicWorld.SP.MainPlayer.DayPeiYangCount++;

        XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_PeiYang, msg.Value, msg.Type);

    }

    public void Process_Pet_PeiYang(NetPacket packet)
    {
        SC_Pet_PeiYang msg = SC_Pet_PeiYang.ParseFrom(packet.Message);
        float curGrow = XLogicWorld.SP.PetManager.AllPet[msg.PetIndex].DynGet(EShareAttr.esa_Grow);
        XLogicWorld.SP.PetManager.AllPet[msg.PetIndex].DynSet(EShareAttr.esa_Grow, (int)(curGrow + msg.Value * Define.CONFIG_RATE_BASE));
        if (msg.Type == 0)
            XLogicWorld.SP.MainPlayer.GameMoney += msg.Money;
        else
            XLogicWorld.SP.MainPlayer.RealMoney += msg.Money;

        XLogicWorld.SP.MainPlayer.DayPeiYangCount++;

        XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_PeiYang, msg.Value, msg.Type);
    }

    public void Process_Player_HL(NetPacket packet)
    {
        SC_Player_HJ msg = SC_Player_HJ.ParseFrom(packet.Message);
        if (XLogicWorld.SP.MainPlayer.ClassLevel < Define.MAX_CLASS_LEVEL)
        {
            XLogicWorld.SP.MainPlayer.ClassLevel++;
            XLogicWorld.SP.MainPlayer.Grow += msg.AddValue;
            XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_JJ);
        }
    }

    public void Process_Mail_AllMail(NetPacket packet)
    {
        SC_AllMail msg = SC_AllMail.ParseFrom(packet.Message);
        for (int i = 0; i < msg.MailsCount; i++)
        {
            SC_SendMail info = msg.GetMails(i);
            XMailManager.XMailInfo mail = new XMailManager.XMailInfo(info.Id, info.Title, info.Sender, info.Content,
                info.Time, info.Read, info.MailType, info.DeleteType, info.Money);
            for (int t = 0; t < info.Items.ItemListCount; t++)
            {
                if (info.Items.GetItemList(t).ItemId > 0u && info.Items.GetItemList(t).ItemCount > 0u)
                {
                    XItem item = new XItem();
                    item.InitFromPB(info.Items.GetItemList(t));
                    mail.AddItem(item);
                }
            }
            XMailManager.listMail[mail] = mail;
        }
        XMailManager.HandleMailComeIn();
    }

    public void Process_Mail_GetMail(NetPacket packet)
    {
        SC_SendMail msg = SC_SendMail.ParseFrom(packet.Message);

        XMailManager.XMailInfo mail = new XMailManager.XMailInfo(msg.Id, msg.Title, msg.Sender, msg.Content, msg.Time,
            msg.Read, msg.MailType, msg.DeleteType, msg.Money);
        for (int t = 0; t < msg.Items.ItemListCount; t++)
        {
            if (msg.Items.GetItemList(t).ItemId > 0u && msg.Items.GetItemList(t).ItemCount > 0u)
            {
                XItem item = new XItem();
                item.InitFromPB(msg.Items.GetItemList(t));
                mail.AddItem(item);
            }
        }
        XMailManager.listMail[mail] = mail;
        XMailManager.HandleMailComeIn();
    }

    public void Process_Mail_Oper(NetPacket packet)
    {
        SC_MailOperResult msg = SC_MailOperResult.ParseFrom(packet.Message);

        XEventManager.SP.SendEvent(EEvent.Mail_Oper, msg.Id, msg.Result);
    }

    public void Process_Pet_HL(NetPacket packet)
    {
        SC_Pet_HJ msg = SC_Pet_HJ.ParseFrom(packet.Message);
        XPet pet = XLogicWorld.SP.PetManager.AllPet[msg.PetIndex];
        if (pet == null)
            return;

        int ClassLevel = (int)pet.ClassLevel;
        if (ClassLevel < Define.MAX_CLASS_LEVEL)
        {
            pet.ClassLevel = msg.ClassLevel;
            int curGrow = XLogicWorld.SP.PetManager.AllPet[msg.PetIndex].DynGet(EShareAttr.esa_Grow);
            pet.DynSet(EShareAttr.esa_Grow, (int)(curGrow + msg.AddValue * Define.CONFIG_RATE_BASE));
            //æŽæ°æš¡å
            XCfgClassLvUp cfgLvUp = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
            if (cfgLvUp != null)
            {
                XCfgPetBase CfgPet = XCfgPetBaseMgr.SP.GetConfig(pet.PetID);
                if (CfgPet != null)
                {
                    pet.ModelId = CfgPet.ModelId[cfgLvUp.ModelIndex];
                }
            }

            XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_JJ);
        }

    }

    public void Process_PKObjectList(NetPacket packet)
    {
        SC_PKObjectList msg = SC_PKObjectList.ParseFrom(packet.Message);

        uint CurRank = msg.CurRank;
        uint ContinueCount = msg.ContinueCount;
        uint LeftChallengeCount = msg.LeftChallengeCount;
        uint BestRank = msg.BestRank;
        uint MaxContinueWin = msg.MaxContinueWinCount;
        UInt64 LeftTime = msg.LeftTime;
        uint CDTime = msg.PKCD;

        XEventManager.SP.SendEvent(EEvent.PKData_Other, CurRank, ContinueCount, LeftChallengeCount, BestRank, MaxContinueWin, LeftTime, CDTime, msg.ChallengeCount);

        SortedList<UInt64, PB_PKRecord> RecordList = new SortedList<UInt64, PB_PKRecord>();
        for (int m = 0; m < msg.PKRecordListCount; m++)
        {
            PB_PKRecord tempRecord = msg.PKRecordListList[m];
            if (RecordList.ContainsKey(tempRecord.Time))
            {
                Log.Write(LogLevel.WARN, "Repeate Key ");
                RecordList[tempRecord.Time + 1] = tempRecord;
            }
            else
            {
                RecordList.Add(tempRecord.Time, tempRecord);
            }
        }

        int count = 0;
        foreach (PB_PKRecord record in RecordList.Values)
        {
            XEventManager.SP.SendEvent(EEvent.PKData_Record, count, record.Name, record.Guid, record.RecordType, record.Rank);
            count++;
        }

        //SortRecord
        XEventManager.SP.SendEvent(EEvent.PKData_ClearSortRecord);
        for (int n = 0; n < msg.PKSortRecordListCount; n++)
        {
            PB_SortRecord sortRecord = msg.PKSortRecordListList[n];
            XEventManager.SP.SendEvent(EEvent.PKData_SortRecord, n, sortRecord.Rank, sortRecord.Name, sortRecord.Level, sortRecord.BattleValue, sortRecord.ChangeFlag);
        }

        for (int i = 0; i < msg.PKObjectListCount; i++)
        {
            PB_SinglePKObject obj = msg.GetPKObjectList(i);
            string Name = obj.Name;
            uint Level = obj.Level;
            uint Duty = obj.Duty;
            uint Sex = obj.Sex;
            uint ObjCurRank = obj.CurRank;

            uint ClothesModelID = XItemManager.GetClothesModelID(Duty, Sex, obj.ClothesID, obj.ClothesStrengthenLevel, obj.ClothesColor);
            uint WeaponModelID = XItemManager.GetWeaponModelID(Duty, Sex, obj.WeaponID, obj.WeaponStrengthenLevel, obj.WeaponColor);

            XEventManager.SP.SendEvent(EEvent.PKData_SingleObject, i, Name, Level, ObjCurRank, ClothesModelID, WeaponModelID);
        }
        XEventManager.SP.SendEvent(EEvent.PKData_PKDataGetFinish);
    }

    public void Process_PK_Notice(NetPacket packet)
    {
        SC_PK_Notice msg = SC_PK_Notice.ParseFrom(packet.Message);
        PKNoticeType type = msg.PKNoticeType;
        string name = msg.AttackerName;
        UInt64 guid = msg.AttackerGUID;
        string beAttackName = msg.BeAttackerName;
        UInt64 beAttackGUID = msg.BeAttackerGUID;
        XEventManager.SP.SendEvent(EEvent.PKData_Notice, type, name, guid, beAttackName, beAttackGUID);
    }

    public void Process_PK_BuyChallenge(NetPacket packet)
    {
        SC_PK_Buy_ChallengeCount msg = SC_PK_Buy_ChallengeCount.ParseFrom(packet.Message);
        UInt32 ChallengeCount = msg.ChallengeCount;
        UInt32 LeftChallengeCount = msg.LeftChallengeCount;
        XEventManager.SP.SendEvent(EEvent.PKData_BuyChallenge, ChallengeCount, LeftChallengeCount);

    }

    public void Process_PK_AddSpeed(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.PKData_AddSpeed);

    }

    public void Process_PK_Failed(NetPacket packet)
    {
        SC_PKFailed msg = SC_PKFailed.ParseFrom(packet.Message);
        XEventManager.SP.SendEvent(EEvent.PKData_Failed, msg.LeftChallengeCount, msg.PKCD);

        SortedList<UInt64, PB_PKRecord> RecordList = new SortedList<UInt64, PB_PKRecord>();
        for (int m = 0; m < msg.PKRecordListCount; m++)
        {
            PB_PKRecord tempRecord = msg.PKRecordListList[m];
            if (RecordList.ContainsKey(tempRecord.Time))
            {
                RecordList.Add(tempRecord.Time + 1, tempRecord);
            }
            else
                RecordList.Add(tempRecord.Time, tempRecord);
        }

        int count = 0;
        foreach (PB_PKRecord record in RecordList.Values)
        {
            XEventManager.SP.SendEvent(EEvent.PKData_Record, count, record.Name, record.Guid, record.RecordType, record.Rank);
            count++;
        }
    }

    public void Process_AddAward(NetPacket packet)
    {
        SC_AddAward msg = SC_AddAward.ParseFrom(packet.Message);

        EAwardType type = msg.AwardType;
        switch (type)
        {
            case EAwardType.EAwardType_XianDH:
                {
                    XEventManager.SP.SendEvent(EEvent.Obj_AddAward, msg.AwardType, msg.Rank, msg.GameMoney, msg.Honour, msg.ItemID, msg.ItemCount);
                }
                break;
            case EAwardType.EAwardType_ZhanYaoLu:
                {
                    XEventManager.SP.SendEvent(EEvent.Obj_AddAward, msg.AwardType, msg.Honour);
                }
                break;
        }

    }

    public void Process_MoneyTree_Resualt(NetPacket packet)
    {
        SC_MoneyTree_Resualt msg = SC_MoneyTree_Resualt.ParseFrom(packet.Message);
        int getGameMoney = msg.GetGameMoney;
        int isCrit = msg.IsCrit;
        int curShakeTimes = msg.CurShakeTimes;
        XMoneyTreeManager.SP.ON_SC_Resualt(getGameMoney, isCrit, curShakeTimes);
    }

    public void Process_MoneyTree_GetShake(NetPacket packet)
    {
        SC_MoneyTree_UpdateShakeTimes msg = SC_MoneyTree_UpdateShakeTimes.ParseFrom(packet.Message);
        int curShakeTimes = msg.CurShakeTimes;
        XMoneyTreeManager.SP.ON_SC_SendShake(curShakeTimes);
    }

    public void Process_ShowPlayerInfo(NetPacket packet)
    {
        SC_PlayerInfo msg = SC_PlayerInfo.ParseFrom(packet.Message);
        XEventManager.SP.SendEvent(EEvent.Chat_ShowPlayerInfo, msg);
    }

    public void Process_OnlineReward_NewGet(NetPacket packet)
    {
        SC_OnlineReward_NewEvent msg = SC_OnlineReward_NewEvent.ParseFrom(packet.Message);
        uint curID = msg.CurID;

        XOnlineRewardManager.SP.ON_SC_NewEvent(curID);
    }

    public void Process_OnlineReward_TimeToGet(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        uint curid = msg.Data;
        XOnlineRewardManager.SP.ON_SC_TimeToGet(curid);
    }

    public void Process_GrowthTarget(NetPacket packet)
    {
        SC_GrowthTarget msg = SC_GrowthTarget.ParseFrom(packet.Message);
        XGrowthTargetManger.SP.ON_SC_GrowthData_Coming(msg);
    }

    public void Process_NewPlayerGuide(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XNewPlayerGuideManager.SP.ON_SC_NewPlayerGuideData_Coming(msg);
    }

    public void Process_Friend_AllData(NetPacket packet)
    {
        SC_Friend_AllFriendData msg = SC_Friend_AllFriendData.ParseFrom(packet.Message);
        XFriendManager.SP.ON_SC_Clinet_ReciveInitAllData(msg);
    }

    public void Process_Friend_AddFriendResualt(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);
        XFriendManager.SP.ON_SC_Client_ReciveAddFriend(msg);
    }

    public void Process_Friend_DelFriendResualt(NetPacket packet)
    {
        SC_Friend_PlayerID msg = SC_Friend_PlayerID.ParseFrom(packet.Message);
        ulong tid = msg.Userid;
        XFriendManager.SP.ON_SC_Client_ReciveDelFriend(tid);
    }

    public void Process_Friend_PresentFlowerResualt(NetPacket packet)
    {
        SC_Friend_ErrorCode msg = SC_Friend_ErrorCode.ParseFrom(packet.Message);
        XFriendManager.SP.ON_SC_Clinet_RecivePresentFlowerResualt(msg);
    }

    public void Process_Friend_ErrorCode(NetPacket packet)
    {
        SC_Friend_ErrorCode msg = SC_Friend_ErrorCode.ParseFrom(packet.Message);
        uint code = msg.Code;
        XFriendManager.SP.ON_SC_Client_ReciveErrorCode(code);
    }

    public void Process_Friend_AddToBlackList(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);

        XFriendPlayer fPlayer = new XFriendPlayer();
        fPlayer.UID = msg.UID;
        fPlayer.Level = msg.Level;
        fPlayer.Name = msg.Name;
        fPlayer.Sex = (ushort)msg.Sex;
        fPlayer.Relation = (byte)msg.Relation;
        fPlayer.Signature = msg.Singnature;
        fPlayer.Status = msg.Status;
        fPlayer.FriendlyValue = msg.FriendlyValue;
        XFriendManager.SP.ON_SC_Client_ReciveAddToBlackList(fPlayer);
    }

    public void Process_Friend_ReciveDelBlack(NetPacket packet)
    {
        SC_Friend_PlayerID msg = SC_Friend_PlayerID.ParseFrom(packet.Message);
        ulong tid = msg.Userid;
        XFriendManager.SP.ON_SC_Client_ReciveDelBlack(tid);
    }

    public void Process_Friend_ReciveGetMarryAgree(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        ulong tid = msg.Data;
        XFriendManager.SP.ON_SC_Client_ReciveGetMarryAgree(tid);
    }

    public void Process_Friend_ReciveGetMarryResualt(NetPacket packet)
    {
        SC_Friend_MarryResualt msg = SC_Friend_MarryResualt.ParseFrom(packet.Message);

        if (msg.IsSender != 0)
        {
            XFriendManager.SP.ON_SC_Client_ReciveGetMarryResualt(true, msg.SenderId, msg.TargetId);
        }
        else
        {
            XFriendManager.SP.ON_SC_Client_ReciveGetMarryResualt(false, msg.SenderId, msg.TargetId);
        }
    }

    public void Process_Friend_DivorceRequestToReciver(NetPacket packet)
    {
        SC_Friend_PlayerID msg = SC_Friend_PlayerID.ParseFrom(packet.Message);
        ulong tid = msg.Userid;
        XFriendManager.SP.ON_SC_Client_ReciveDivorceToReciver(tid);
    }

    public void Process_Friend_DivorceResualt(NetPacket packet)
    {
        SC_Friend_PlayerID msg = SC_Friend_PlayerID.ParseFrom(packet.Message);
        ulong tid = msg.Userid;
        XFriendManager.SP.ON_SC_Client_ReciveDivorceResualt(tid);
    }

    public void Process_Friend_AddFriendTips(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);
        if (msg.HasUID && msg.HasName)
        {
            XFriendManager.SP.ON_SC_Client_ReciveAddFriendTip(msg.UID, msg.Name);
        }
    }

    public void Process_Friend_UpdateLevel(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);
        if (msg.HasLevel && msg.HasUID)
        {
            XFriendManager.SP.ON_SC_Client_ReciveUpdateLevel(msg.UID, msg.Level);
        }
    }

    public void Process_Friend_UpdateOnlineStatus(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);
        if (msg.HasStatus && msg.HasUID)
        {
            XFriendManager.SP.ON_SC_Client_ReciveUpdateOnlineStatus(msg.UID, msg.Status);
        }
    }

    public void Process_Friend_UpdateSignature(NetPacket packet)
    {
        SC_Friend_UserInfo msg = SC_Friend_UserInfo.ParseFrom(packet.Message);
        if (msg.HasSingnature && msg.HasUID)
        {
            XFriendManager.SP.ON_SC_Client_ReciveUpdateSignature(msg.UID, msg.Singnature);
        }
    }


    public void Process_Friend_UpdatePresentCount(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        int data = msg.Data;
        XFriendManager.SP.ON_SC_Client_ReciveUpdatePresentCount((uint)data);
    }

    public void Process_Friend_UpdateRefreshPresentObject(NetPacket packet)
    {
        SC_Empty msg = SC_Empty.ParseFrom(packet.Message);
        XFriendManager.SP.ON_SC_Client_ReciveRefreshPresentObject();
    }

    public void Process_Friend_AddPresentObject(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XFriendManager.SP.ON_SC_Client_AddPresentObject(msg.Data);
    }


    public void Process_Friend_UpdateHouseInfo(NetPacket packet)
    {
        SC_Friend_FlowerHouseInfo houseDataMsg = SC_Friend_FlowerHouseInfo.ParseFrom(packet.Message);
        if (houseDataMsg.HasUID)
        {
            XFriendManager.SP.ON_SC_Client_ReciveUpdateHouseInfo(houseDataMsg.UID, houseDataMsg.Name, houseDataMsg.Flowers, houseDataMsg.ReciveTime);
        }
    }

    public void Process_Friend_RankInfo_Update(NetPacket packet)
    {
        SC_Friend_Flower_Rank msg = SC_Friend_Flower_Rank.ParseFrom(packet.Message);
        XCharmRankManager.SP.On_SC_ReciveData(msg);
    }

    public void Process_NickName_AllData(NetPacket packet)
    {
        SC_NickName_AllData msg = SC_NickName_AllData.ParseFrom(packet.Message);
        XNickNameManager.SP.On_SC_ReciveAllData(msg);
    }

    public void Process_NickName_AddNickName(NetPacket packet)
    {
        SC_NickName_Info msg = SC_NickName_Info.ParseFrom(packet.Message);
        XNickNameManager.SP.On_SC_ReciveAddNickName(msg.NID);
    }

    public void Process_NickName_DelNickName(NetPacket packet)
    {
        SC_NickName_Info msg = SC_NickName_Info.ParseFrom(packet.Message);
        XNickNameManager.SP.On_SC_ReciveDelNickName(msg.NID);
    }

    public void Process_NickName_SetCurName(NetPacket packet)
    {
        SC_NickName_Info msg = SC_NickName_Info.ParseFrom(packet.Message);
        XNickNameManager.SP.ON_SC_ReciveSetCurNickName(msg.NID);
    }

    public void Process_NickName_SyncNickName(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XPlayer player = XLogicWorld.SP.ObjectManager.GetPlayer(msg.Uid);
        if (player != null)
        {
            if (msg.Data != 0)
            {
                uint nNickNameID = (uint)msg.Data;
                XNickNameInfo infoData = XNickNameManager.SP.GetNickNameInfoFromCfg(nNickNameID);
                if (infoData != null)
                {
                    try
                    {
                        string colorname = XGameColorDefine.NickName_Color[infoData.colorID];
                        player.NickName = string.Format("{0}{1}", colorname, infoData.name);
                    }
                    catch
                    {
                        Log.Write(LogLevel.ERROR, "Xplayer, the index is out of Quality_Color num");
                    }
                }
            }
            else
            {
                player.NickName = "";
            }
        }
    }

    public void Process_PlayerLoginEnd(NetPacket packet)
    {
        FeatureDataUnLockMgr.SP.DataLoadEnd();
        XEventManager.SP.SendEvent(EEvent.UI_UpdateBattleValue);
    }

    public void Process_WallowShut(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel, null, null, XStringManager.SP.GetString(262));
    }

    public void Process_Player_RandomAddLD(NetPacket packet)
    {
        //XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel,null,null,XStringManager.SP.GetString(262));
        SC_Player_Random_AddLD msg = SC_Player_Random_AddLD.ParseFrom(packet.Message);

        uint index = msg.SelIndex;
        uint addValue = msg.FinalValue;

        XMainPlayer player = XLogicWorld.SP.MainPlayer;

        if (index == 0)
            player.WuLi += addValue;
        else if (index == 1)
            player.LingQiao += addValue;
        else if (index == 2)
            player.TiZhi += addValue;
        else if (index == 3)
            player.ShuFa += addValue;

    }

    public void Process_Pet_RandomAddLD(NetPacket packet)
    {
        SC_Pet_Random_AddLD msg = SC_Pet_Random_AddLD.ParseFrom(packet.Message);
        uint index = msg.SelIndex;
        uint PetIndex = msg.PetIndex;
        uint addValue = msg.FinalValue;

        XPet pet = XLogicWorld.SP.PetManager.AllPet[PetIndex];
        if (pet == null)
            return;

        if (index == 0)
            pet.WuLi += addValue;
        else if (index == 1)
            pet.LingQiao += addValue;
        else if (index == 2)
            pet.TiZhi += addValue;
        else if (index == 3)
            pet.ShuFa += addValue;
    }

    public void Process_WallowOneHour(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel, null, null, XStringManager.SP.GetString(259));
    }

    public void Process_WallowTwoHour(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel, null, null, XStringManager.SP.GetString(260));
    }

    public void Process_WallowThreeHour(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel, null, null, XStringManager.SP.GetString(261));
    }

    public void Process_WallowNotice(NetPacket packet)
    {
        XEventManager.SP.SendEvent(EEvent.MessageBoxWithNoCancel, null, null, XStringManager.SP.GetString(258));
    }

    public void Process_AllSevenTarget(NetPacket packet)
    {
        SC_SevenListData msg = SC_SevenListData.ParseFrom(packet.Message);
        XSevenTargetManager.SP.ON_SC_AllSevenTargetStatus(msg);
    }
    public void Process_ItemSevenTarget(NetPacket packet)
    {
        SC_SevenItemData msg = SC_SevenItemData.ParseFrom(packet.Message);
        XSevenTargetManager.SP.ON_SC_ItemTargetStatus(msg);
    }

    public void Process_ZYL_MonsterInfo(NetPacket packet)
    {
        SC_Int msg = SC_Int.ParseFrom(packet.Message);
        XZhanYaoLuManager.SP.ON_SC_MonsterInfo(msg.Data);
    }

    public void Process_ZYL_BaseData(NetPacket packet)
    {
        SC_ZhanYaoLu_Data msg = SC_ZhanYaoLu_Data.ParseFrom(packet.Message);
        XZhanYaoLuManager.SP.ON_SC_BaseData(msg);
    }

    public void Process_ZYL_Notice_UltraKil(NetPacket packet)
    {
        SC_String msg = SC_String.ParseFrom(packet.Message);
        XZhanYaoLuManager.SP.ON_SC_Notice_UltraKil(msg);
    }

    public void Process_MeditationStart(NetPacket packet)
    {
        SC_Meditation_StartData msg = SC_Meditation_StartData.ParseFrom(packet.Message);
        XMeditationManager.SP.ON_SC_MeditationStart(msg);
    }
    public void Process_MeditationEnd(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XMeditationManager.SP.ON_SC_MeditationEnd(msg);
    }
    public void Process_MeditationUpdateRewardExp(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XMeditationManager.SP.ON_SC_MeditaitonUpdateExp(msg);
    }

    public void Process_SaoDang_Result(NetPacket packet)
    {
        XSaoDangManager.SP.ON_SC_SaoDang_Result(SC_BattleResult.ParseFrom(packet.Message));
    }

    public void Process_Daily_Status(NetPacket packet)
    {
        SC_DailySignStatus msg = SC_DailySignStatus.ParseFrom(packet.Message);
        DailySignManager.SP.ON_SC_DailyStatus(msg);
    }

    public void Process_Daily_Result(NetPacket packet)
    {
        SC_DailySignStatus msg = SC_DailySignStatus.ParseFrom(packet.Message);
        DailySignManager.SP.ON_SC_DailyResult(msg);
    }

    // Guild begin====================================================================================================================

    public void Process_Guild_RecvAllSynInfo(NetPacket packet)
    {
        SC_Guild_RespondAllSynInfo msg = SC_Guild_RespondAllSynInfo.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvAllSynInfo(msg);
    }

    public void Process_Guild_RecvBroadAddSynInfo(NetPacket packet)
    {
        SC_Guild_BroadAddSynInfo msg = SC_Guild_BroadAddSynInfo.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadAddSynInfo(msg);
    }

    public void Process_Guild_RecvBroadRemoveSynInfo(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadRemoveSynInfo(msg);
    }

    public void Process_Guild_RecvUpdateSynInfoList(NetPacket packet)
    {

    }

    public void Process_Guild_RecvBaseInfo(NetPacket packet)
    {
        SC_Guild_RespondBaseInfo msg = SC_Guild_RespondBaseInfo.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBaseInfo(msg);
    }

    public void Process_Guild_RecvAllMemInfo(NetPacket packet)
    {
        SC_Guild_RespondAllMemInfo msg = SC_Guild_RespondAllMemInfo.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvAllMemInfo(msg);
    }


    public void Process_Guild_RecvAllApplyInfo(NetPacket packet)
    {
        SC_Guild_RespondAllApplyInfo msg = SC_Guild_RespondAllApplyInfo.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvAllApplyInfo(msg);
    }

    public void Process_Guild_RecvBroadAddMem(NetPacket packet)
    {
        SC_Guild_BroadAddMem msg = SC_Guild_BroadAddMem.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadAddMem(msg);
    }

    public void Process_Guild_RecvBroadRemoveMem(NetPacket packet)
    {
        SC_Guild_BroadRemoveMem msg = SC_Guild_BroadRemoveMem.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadRemoveMem(msg);
    }

    public void Process_Guild_RecvUpdateMemInfoList(NetPacket packet)
    {

    }

    public void Process_Guild_RecvSelfApplyList(NetPacket packet)
    {
        SC_Guild_SendSelfApplyList msg = SC_Guild_SendSelfApplyList.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvSelfApplyList(msg);
    }

    public void Process_Guild_RecvBroadAddApply(NetPacket packet)
    {
        SC_Guild_BroadAddApply msg = SC_Guild_BroadAddApply.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadAddApply(msg);
    }

    public void Process_Guild_RecvBroadRemoveApply(NetPacket packet)
    {
        SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvBroadRemoveApply(msg);
    }

    public void Process_Guild_RecvUpdateApplyInfoList(NetPacket packet)
    {

    }

    public void Process_Guild_BroadAnno(NetPacket packet)
    {
        SC_Guild_BroadAnno msg = SC_Guild_BroadAnno.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_Guild_BroadAnno(msg);
    }

    public void Process_Guild_BroadTran(NetPacket packet)
    {
        SC_Guild_BroadTran msg = SC_Guild_BroadTran.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_Guild_BroadTran(msg);
    }

    public void Process_Guild_RecvFeedback(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        XGuildManager.SP.ON_SC_RecvFeedback(msg);
    }

    // Guild end=====================================================================================================================

    //活跃度奖励 Start
    public void Process_DayActivity_InitData(NetPacket packet)
    {
		SC_DayActivity_AllData msg = SC_DayActivity_AllData.ParseFrom(packet.Message);
		XDayActivityManager.SP.ON_SC_ReciveAllData(msg);
    }
	
    public void Process_DayActivity_AwardInfo(NetPacket packet)
    {
		SC_DayActivity_AwardInfo msg = SC_DayActivity_AwardInfo.ParseFrom(packet.Message);
		XDayActivityManager.SP.ON_SC_ReciveAwardInfo(msg);
    }
	
    public void Process_DayActivity_ItemInfo(NetPacket packet)
    {
		SC_DayActivity_ItemInfo msg = SC_DayActivity_ItemInfo.ParseFrom(packet.Message);
		XDayActivityManager.SP.ON_SC_ReciveItemInfo(msg);
    }
	
    public void Process_DayActivity_ResetInfo(NetPacket packet)
    {
		SC_UInt64 msg = SC_UInt64.ParseFrom(packet.Message);
		if(msg.Data == XLogicWorld.SP.MainPlayer.ID)
		{
			XDayActivityManager.SP.ON_SC_ReciveResetInfo();
		}
    }
	
	public void Process_DayActivity_ReciveActivityValue(NetPacket packet)
	{
		SC_DayActivity_ActivityValue msg = SC_DayActivity_ActivityValue.ParseFrom(packet.Message);
		XDayActivityManager.SP.ON_SC_ReciveValueActivity(msg);
	}
    //活跃度奖励 End

    #endregion
}
