
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgDayActivityAwardMgr : CCfg1KeyMgrTemplate<XCfgDayActivityAwardMgr, ushort, XCfgDayActivityAward> { };

partial class XCfgDayActivityAward : ITabItemWith1Key<ushort>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_PresentPos = "PresentPos";
	public static readonly string _KEY_VitalityValue = "VitalityValue";
	public static readonly string _KEY_LevelInAward = "LevelInAward";
	public static readonly string _KEY_awardExp = "awardExp";
	public static readonly string _KEY_awardGameMoney = "awardGameMoney";
	public static readonly string _KEY_awardPrestige = "awardPrestige";
	public static readonly string _KEY_awardRealMoney = "awardRealMoney";
	public static readonly string _KEY_awardItemID_4_0 = "awardItemID_4_0";
	public static readonly string _KEY_awardItemID_4_1 = "awardItemID_4_1";
	public static readonly string _KEY_awardItemID_4_2 = "awardItemID_4_2";
	public static readonly string _KEY_awardItemID_4_3 = "awardItemID_4_3";
	public static readonly string _KEY_awardItemCount_4_0 = "awardItemCount_4_0";
	public static readonly string _KEY_awardItemCount_4_1 = "awardItemCount_4_1";
	public static readonly string _KEY_awardItemCount_4_2 = "awardItemCount_4_2";
	public static readonly string _KEY_awardItemCount_4_3 = "awardItemCount_4_3";

	public ushort Index { get; private set; }				// 奖励序号
	public uint PresentPos { get; private set; }				// 奖励位置
	public int VitalityValue { get; private set; }				// 需要达到活跃度
	public uint LevelInAward { get; private set; }				// 奖励数值是否对应玩家等级
	public int awardExp { get; private set; }				// 奖励经验
	public int awardGameMoney { get; private set; }				// 奖励游戏币
	public int awardPrestige { get; private set; }				// 奖励声望
	public int awardRealMoney { get; private set; }				// 奖励元宝
	public uint[] awardItemID { get; private set; }				// 奖励1ID
	public uint[] awardItemCount { get; private set; }				// 奖励1数量

	public XCfgDayActivityAward()
	{
		awardItemID = new uint[4];
		awardItemCount = new uint[4];
	}

	public ushort GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<ushort>(_KEY_Index);
		PresentPos = tf.Get<uint>(_KEY_PresentPos);
		VitalityValue = tf.Get<int>(_KEY_VitalityValue);
		LevelInAward = tf.Get<uint>(_KEY_LevelInAward);
		awardExp = tf.Get<int>(_KEY_awardExp);
		awardGameMoney = tf.Get<int>(_KEY_awardGameMoney);
		awardPrestige = tf.Get<int>(_KEY_awardPrestige);
		awardRealMoney = tf.Get<int>(_KEY_awardRealMoney);
		awardItemID[0] = tf.Get<uint>(_KEY_awardItemID_4_0);
		awardItemID[1] = tf.Get<uint>(_KEY_awardItemID_4_1);
		awardItemID[2] = tf.Get<uint>(_KEY_awardItemID_4_2);
		awardItemID[3] = tf.Get<uint>(_KEY_awardItemID_4_3);
		awardItemCount[0] = tf.Get<uint>(_KEY_awardItemCount_4_0);
		awardItemCount[1] = tf.Get<uint>(_KEY_awardItemCount_4_1);
		awardItemCount[2] = tf.Get<uint>(_KEY_awardItemCount_4_2);
		awardItemCount[3] = tf.Get<uint>(_KEY_awardItemCount_4_3);
		return true;
	}
}

