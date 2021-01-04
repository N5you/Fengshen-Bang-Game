
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMissionMgr : CCfg1KeyMgrTemplate<XCfgMissionMgr, uint, XCfgMission> { };

partial class XCfgMission : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_typeID = "typeID";
	public static readonly string _KEY_property = "property";
	public static readonly string _KEY_title = "title";
	public static readonly string _KEY_targetMessage = "targetMessage";
	public static readonly string _KEY_missionMessage = "missionMessage";
	public static readonly string _KEY_NPCUncompleted = "NPCUncompleted";
	public static readonly string _KEY_NPCCompleted = "NPCCompleted";
	public static readonly string _KEY_NPCRecive = "NPCRecive";
	public static readonly string _KEY_NPCDisclaim = "NPCDisclaim";
	public static readonly string _KEY_NPCUncompletedAsk = "NPCUncompletedAsk";
	public static readonly string _KEY_NPCCompletedAsk = "NPCCompletedAsk";
	public static readonly string _KEY_killTargetTitle1 = "killTargetTitle1";
	public static readonly string _KEY_killTargetTitle2 = "killTargetTitle2";
	public static readonly string _KEY_killTargetTitle3 = "killTargetTitle3";
	public static readonly string _KEY_itemTargetTitle1 = "itemTargetTitle1";
	public static readonly string _KEY_itemTargetTitle2 = "itemTargetTitle2";
	public static readonly string _KEY_itemTargetTitle3 = "itemTargetTitle3";
	public static readonly string _KEY_isDynamic = "isDynamic";
	public static readonly string _KEY_nextID = "nextID";
	public static readonly string _KEY_reciveNPCID = "reciveNPCID";
	public static readonly string _KEY_receiveNPCSceneID = "receiveNPCSceneID";
	public static readonly string _KEY_referNPCID = "referNPCID";
	public static readonly string _KEY_referNPCSceneID = "referNPCSceneID";
	public static readonly string _KEY_needJob = "needJob";
	public static readonly string _KEY_needLevel = "needLevel";
	public static readonly string _KEY_needMissionID = "needMissionID";
	public static readonly string _KEY_needSpeciality = "needSpeciality";
	public static readonly string _KEY_needSpecialityValue = "needSpecialityValue";
	public static readonly string _KEY_needTongLevel = "needTongLevel";
	public static readonly string _KEY_referType = "referType";
	public static readonly string _KEY_referItemID1 = "referItemID1";
	public static readonly string _KEY_referItemCount1 = "referItemCount1";
	public static readonly string _KEY_referItemID2 = "referItemID2";
	public static readonly string _KEY_referItemCount2 = "referItemCount2";
	public static readonly string _KEY_referItemID3 = "referItemID3";
	public static readonly string _KEY_referItemCount3 = "referItemCount3";
	public static readonly string _KEY_receiveItemID = "receiveItemID";
	public static readonly string _KEY_receiveItemCount = "receiveItemCount";
	public static readonly string _KEY_referKillID1 = "referKillID1";
	public static readonly string _KEY_referKillCount1 = "referKillCount1";
	public static readonly string _KEY_referKillID2 = "referKillID2";
	public static readonly string _KEY_referKillCount2 = "referKillCount2";
	public static readonly string _KEY_referKillID3 = "referKillID3";
	public static readonly string _KEY_referKillCount3 = "referKillCount3";
	public static readonly string _KEY_referDuplicateID = "referDuplicateID";
	public static readonly string _KEY_awardChooseItemID1 = "awardChooseItemID1";
	public static readonly string _KEY_awardChooseItemID2 = "awardChooseItemID2";
	public static readonly string _KEY_awardChooseItemID3 = "awardChooseItemID3";
	public static readonly string _KEY_awardChooseItemJob1 = "awardChooseItemJob1";
	public static readonly string _KEY_awardChooseItemJob2 = "awardChooseItemJob2";
	public static readonly string _KEY_awardChooseItemJob3 = "awardChooseItemJob3";
	public static readonly string _KEY_awardItemID1 = "awardItemID1";
	public static readonly string _KEY_awardItemID2 = "awardItemID2";
	public static readonly string _KEY_awardItemCount1 = "awardItemCount1";
	public static readonly string _KEY_awardItemCount2 = "awardItemCount2";
	public static readonly string _KEY_awardPetID = "awardPetID";
	public static readonly string _KEY_awardMoney = "awardMoney";
	public static readonly string _KEY_awardIngot = "awardIngot";
	public static readonly string _KEY_awardExp = "awardExp";
	public static readonly string _KEY_awardBuff = "awardBuff";
	public static readonly string _KEY_awardTitle = "awardTitle";
	public static readonly string _KEY_awardTongExp = "awardTongExp";
	public static readonly string _KEY_missionSceneID = "missionSceneID";
	public static readonly string _KEY_missionConveyDoorID = "missionConveyDoorID";
	public static readonly string _KEY_missionDuplicateID = "missionDuplicateID";

	public uint ID { get; private set; }				// //全局唯一标识
	public uint typeID { get; private set; }				// //任务的分类
	public uint property { get; private set; }				// //任务的属性 0为支线1为主线
	public string title { get; private set; }				// //任务的标题名
	public string targetMessage { get; private set; }				// //任务的目标文本
	public string missionMessage { get; private set; }				// //任务的剧情介绍文本
	public string NPCUncompleted { get; private set; }				// //任务目标没有达成时，与NPC交谈时的NPC对话文本
	public string NPCCompleted { get; private set; }				// //任务目标成功达成时，与NPC交谈时的NPC对话文本
	public string NPCRecive { get; private set; }				// //接任务时，接受任务按钮的回答文本
	public string NPCDisclaim { get; private set; }				// //接任务时，拒绝任务按钮的回答文本
	public string NPCUncompletedAsk { get; private set; }				// //任务目标未达成时，与NPC交谈时的按钮的回答文本
	public string NPCCompletedAsk { get; private set; }				// //任务目标已达成时，与NPC交谈时的的按钮的回答文本
	public string killTargetTitle1 { get; private set; }				// //单个任务目标的信息说明文本，1号的任务目标
	public string killTargetTitle2 { get; private set; }				// //单个任务目标的信息说明文本，2号的任务目标
	public string killTargetTitle3 { get; private set; }				// //单个任务目标的信息说明文本，3号的任务目标
	public string itemTargetTitle1 { get; private set; }				// //单个任务目标的信息说明文本，1号的任务目标
	public string itemTargetTitle2 { get; private set; }				// //单个任务目标的信息说明文本，2号的任务目标
	public string itemTargetTitle3 { get; private set; }				// //单个任务目标的信息说明文本，3号的任务目标
	public short isDynamic { get; private set; }				// //是否为动态的奖励任务1为是0为否
	public uint nextID { get; private set; }				// //紧接着下一个推荐的任务
	public uint reciveNPCID { get; private set; }				// //接受任务的NPC ID
	public uint receiveNPCSceneID { get; private set; }				// //接受任务NPC所在的场景ID
	public uint referNPCID { get; private set; }				// //交任务的NPC ID
	public uint referNPCSceneID { get; private set; }				// //提交任务NPC所在的场景ID
	public uint needJob { get; private set; }				// //需要的职业
	public uint needLevel { get; private set; }				// //需要的级别
	public uint needMissionID { get; private set; }				// //需要之前已经完成的任务ID
	public uint needSpeciality { get; private set; }				// //需要技能特长ID
	public uint needSpecialityValue { get; private set; }				// //需要的技能特长值
	public uint needTongLevel { get; private set; }				// //需要帮会级别
	public uint referType { get; private set; }				// //任务提交类型
	public uint referItemID1 { get; private set; }				// //任务提交所需的物品1 ID
	public uint referItemCount1 { get; private set; }				// //任务提交所需的物品数量1
	public uint referItemID2 { get; private set; }				// //任务提交所需的物品2 ID
	public uint referItemCount2 { get; private set; }				// //任务提交所需的物品数量2
	public uint referItemID3 { get; private set; }				// //任务提交所需的物品3 ID
	public uint referItemCount3 { get; private set; }				// //任务提交所需的物品数量3
	public uint receiveItemID { get; private set; }				// //任务接受的时候给的 物品 ID
	public uint receiveItemCount { get; private set; }				// //任务接受的时候给的物品 数量
	public uint referKillID1 { get; private set; }				// //任务提交所需要杀的怪物ID 1
	public uint referKillCount1 { get; private set; }				// //任务提交所需要杀的怪物数 1
	public uint referKillID2 { get; private set; }				// //任务提交所需要杀的怪物ID 2
	public uint referKillCount2 { get; private set; }				// //任务提交所需要杀的怪物数 2
	public uint referKillID3 { get; private set; }				// //任务提交所需要杀的怪物ID 3
	public uint referKillCount3 { get; private set; }				// //任务提交所需要杀的怪物数 3
	public uint referDuplicateID { get; private set; }				// //完成任务条件所需的副本ID
	public uint awardChooseItemID1 { get; private set; }				// //任务接受的时候给的物品选择之一
	public uint awardChooseItemID2 { get; private set; }				// //任务接受的时候给的物品选择之一
	public uint awardChooseItemID3 { get; private set; }				// //任务接受的时候给的物品选择之一
	public uint awardChooseItemJob1 { get; private set; }				// //战士的奖励物品3选1
	public uint awardChooseItemJob2 { get; private set; }				// //法师的奖励物品3选1
	public uint awardChooseItemJob3 { get; private set; }				// //弓手的奖励物品3选1
	public uint awardItemID1 { get; private set; }				// //任务奖励物品1
	public uint awardItemID2 { get; private set; }				// //任务奖励物品2
	public uint awardItemCount1 { get; private set; }				// //任务奖励物品1数量
	public uint awardItemCount2 { get; private set; }				// //任务奖励物品2数量
	public uint awardPetID { get; private set; }				// //任务奖励宠物
	public uint awardMoney { get; private set; }				// //奖励铜钱数量
	public uint awardIngot { get; private set; }				// //奖励元宝数量
	public uint awardExp { get; private set; }				// //奖励经验数
	public uint awardBuff { get; private set; }				// //奖励Buff ID
	public uint awardTitle { get; private set; }				// //奖励称号
	public uint awardTongExp { get; private set; }				// //奖励公会经验
	public uint missionSceneID { get; private set; }				// //任务副本所在营地ID，帮助自动寻路
	public uint missionConveyDoorID { get; private set; }				// //任务传送门ID，帮助自动寻路
	public uint missionDuplicateID { get; private set; }				// //任务主要活动副本ID，帮助自动寻路

	public XCfgMission()
	{
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		typeID = tf.Get<uint>(_KEY_typeID);
		property = tf.Get<uint>(_KEY_property);
		title = tf.Get<string>(_KEY_title);
		targetMessage = tf.Get<string>(_KEY_targetMessage);
		missionMessage = tf.Get<string>(_KEY_missionMessage);
		NPCUncompleted = tf.Get<string>(_KEY_NPCUncompleted);
		NPCCompleted = tf.Get<string>(_KEY_NPCCompleted);
		NPCRecive = tf.Get<string>(_KEY_NPCRecive);
		NPCDisclaim = tf.Get<string>(_KEY_NPCDisclaim);
		NPCUncompletedAsk = tf.Get<string>(_KEY_NPCUncompletedAsk);
		NPCCompletedAsk = tf.Get<string>(_KEY_NPCCompletedAsk);
		killTargetTitle1 = tf.Get<string>(_KEY_killTargetTitle1);
		killTargetTitle2 = tf.Get<string>(_KEY_killTargetTitle2);
		killTargetTitle3 = tf.Get<string>(_KEY_killTargetTitle3);
		itemTargetTitle1 = tf.Get<string>(_KEY_itemTargetTitle1);
		itemTargetTitle2 = tf.Get<string>(_KEY_itemTargetTitle2);
		itemTargetTitle3 = tf.Get<string>(_KEY_itemTargetTitle3);
		isDynamic = tf.Get<short>(_KEY_isDynamic);
		nextID = tf.Get<uint>(_KEY_nextID);
		reciveNPCID = tf.Get<uint>(_KEY_reciveNPCID);
		receiveNPCSceneID = tf.Get<uint>(_KEY_receiveNPCSceneID);
		referNPCID = tf.Get<uint>(_KEY_referNPCID);
		referNPCSceneID = tf.Get<uint>(_KEY_referNPCSceneID);
		needJob = tf.Get<uint>(_KEY_needJob);
		needLevel = tf.Get<uint>(_KEY_needLevel);
		needMissionID = tf.Get<uint>(_KEY_needMissionID);
		needSpeciality = tf.Get<uint>(_KEY_needSpeciality);
		needSpecialityValue = tf.Get<uint>(_KEY_needSpecialityValue);
		needTongLevel = tf.Get<uint>(_KEY_needTongLevel);
		referType = tf.Get<uint>(_KEY_referType);
		referItemID1 = tf.Get<uint>(_KEY_referItemID1);
		referItemCount1 = tf.Get<uint>(_KEY_referItemCount1);
		referItemID2 = tf.Get<uint>(_KEY_referItemID2);
		referItemCount2 = tf.Get<uint>(_KEY_referItemCount2);
		referItemID3 = tf.Get<uint>(_KEY_referItemID3);
		referItemCount3 = tf.Get<uint>(_KEY_referItemCount3);
		receiveItemID = tf.Get<uint>(_KEY_receiveItemID);
		receiveItemCount = tf.Get<uint>(_KEY_receiveItemCount);
		referKillID1 = tf.Get<uint>(_KEY_referKillID1);
		referKillCount1 = tf.Get<uint>(_KEY_referKillCount1);
		referKillID2 = tf.Get<uint>(_KEY_referKillID2);
		referKillCount2 = tf.Get<uint>(_KEY_referKillCount2);
		referKillID3 = tf.Get<uint>(_KEY_referKillID3);
		referKillCount3 = tf.Get<uint>(_KEY_referKillCount3);
		referDuplicateID = tf.Get<uint>(_KEY_referDuplicateID);
		awardChooseItemID1 = tf.Get<uint>(_KEY_awardChooseItemID1);
		awardChooseItemID2 = tf.Get<uint>(_KEY_awardChooseItemID2);
		awardChooseItemID3 = tf.Get<uint>(_KEY_awardChooseItemID3);
		awardChooseItemJob1 = tf.Get<uint>(_KEY_awardChooseItemJob1);
		awardChooseItemJob2 = tf.Get<uint>(_KEY_awardChooseItemJob2);
		awardChooseItemJob3 = tf.Get<uint>(_KEY_awardChooseItemJob3);
		awardItemID1 = tf.Get<uint>(_KEY_awardItemID1);
		awardItemID2 = tf.Get<uint>(_KEY_awardItemID2);
		awardItemCount1 = tf.Get<uint>(_KEY_awardItemCount1);
		awardItemCount2 = tf.Get<uint>(_KEY_awardItemCount2);
		awardPetID = tf.Get<uint>(_KEY_awardPetID);
		awardMoney = tf.Get<uint>(_KEY_awardMoney);
		awardIngot = tf.Get<uint>(_KEY_awardIngot);
		awardExp = tf.Get<uint>(_KEY_awardExp);
		awardBuff = tf.Get<uint>(_KEY_awardBuff);
		awardTitle = tf.Get<uint>(_KEY_awardTitle);
		awardTongExp = tf.Get<uint>(_KEY_awardTongExp);
		missionSceneID = tf.Get<uint>(_KEY_missionSceneID);
		missionConveyDoorID = tf.Get<uint>(_KEY_missionConveyDoorID);
		missionDuplicateID = tf.Get<uint>(_KEY_missionDuplicateID);
		return true;
	}
}

