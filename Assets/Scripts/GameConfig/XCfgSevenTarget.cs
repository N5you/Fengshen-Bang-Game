
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSevenTargetMgr : CCfg1KeyMgrTemplate<XCfgSevenTargetMgr, byte, XCfgSevenTarget> { };

partial class XCfgSevenTarget : ITabItemWith1Key<byte>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_conditionID1 = "conditionID1";
	public static readonly string _KEY_linkUI1 = "linkUI1";
	public static readonly string _KEY_conditionID2 = "conditionID2";
	public static readonly string _KEY_linkUI2 = "linkUI2";
	public static readonly string _KEY_conditionID3 = "conditionID3";
	public static readonly string _KEY_linkUI3 = "linkUI3";
	public static readonly string _KEY_awardWarrior1 = "awardWarrior1";
	public static readonly string _KEY_awardMage1 = "awardMage1";
	public static readonly string _KEY_awardArcher1 = "awardArcher1";
	public static readonly string _KEY_awardNum1 = "awardNum1";
	public static readonly string _KEY_awardWarrior2 = "awardWarrior2";
	public static readonly string _KEY_awardMage2 = "awardMage2";
	public static readonly string _KEY_awardArcher2 = "awardArcher2";
	public static readonly string _KEY_awardNum2 = "awardNum2";

	public byte Index { get; private set; }				// 天数
	public uint conditionID1 { get; private set; }				// 目标条件ID1
	public uint linkUI1 { get; private set; }				// 关联UI1ID
	public uint conditionID2 { get; private set; }				// 目标条件ID2
	public uint linkUI2 { get; private set; }				// 关联UI2ID
	public uint conditionID3 { get; private set; }				// 目标条件ID3
	public uint linkUI3 { get; private set; }				// 关联UI3ID
	public uint awardWarrior1 { get; private set; }				// 战士奖励物品1
	public uint awardMage1 { get; private set; }				// 法师奖励物品1
	public uint awardArcher1 { get; private set; }				// 弓手奖励物品1
	public uint awardNum1 { get; private set; }				// 奖励物品数量1
	public uint awardWarrior2 { get; private set; }				// 战士奖励物品2
	public uint awardMage2 { get; private set; }				// 法师奖励物品2
	public uint awardArcher2 { get; private set; }				// 弓手奖励物品2
	public uint awardNum2 { get; private set; }				// 奖励物品数量2

	public XCfgSevenTarget()
	{
	}

	public byte GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<byte>(_KEY_Index);
		conditionID1 = tf.Get<uint>(_KEY_conditionID1);
		linkUI1 = tf.Get<uint>(_KEY_linkUI1);
		conditionID2 = tf.Get<uint>(_KEY_conditionID2);
		linkUI2 = tf.Get<uint>(_KEY_linkUI2);
		conditionID3 = tf.Get<uint>(_KEY_conditionID3);
		linkUI3 = tf.Get<uint>(_KEY_linkUI3);
		awardWarrior1 = tf.Get<uint>(_KEY_awardWarrior1);
		awardMage1 = tf.Get<uint>(_KEY_awardMage1);
		awardArcher1 = tf.Get<uint>(_KEY_awardArcher1);
		awardNum1 = tf.Get<uint>(_KEY_awardNum1);
		awardWarrior2 = tf.Get<uint>(_KEY_awardWarrior2);
		awardMage2 = tf.Get<uint>(_KEY_awardMage2);
		awardArcher2 = tf.Get<uint>(_KEY_awardArcher2);
		awardNum2 = tf.Get<uint>(_KEY_awardNum2);
		return true;
	}
}

