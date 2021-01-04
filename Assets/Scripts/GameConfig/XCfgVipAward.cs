
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgVipAwardMgr : CCfg1KeyMgrTemplate<XCfgVipAwardMgr, uint, XCfgVipAward> { };

partial class XCfgVipAward : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_VipLvl = "VipLvl";
	public static readonly string _KEY_Recharge = "Recharge";
	public static readonly string _KEY_Money = "Money";
	public static readonly string _KEY_RealMoney = "RealMoney";
	public static readonly string _KEY_Item1Id = "Item1Id";
	public static readonly string _KEY_Item1Count = "Item1Count";
	public static readonly string _KEY_Item2Id = "Item2Id";
	public static readonly string _KEY_Item2Count = "Item2Count";
	public static readonly string _KEY_Item3Id = "Item3Id";
	public static readonly string _KEY_Item3Count = "Item3Count";
	public static readonly string _KEY_Item4Id = "Item4Id";
	public static readonly string _KEY_Item4Count = "Item4Count";
	public static readonly string _KEY_Item5Id = "Item5Id";
	public static readonly string _KEY_Item5Count = "Item5Count";
	public static readonly string _KEY_Item6Id = "Item6Id";
	public static readonly string _KEY_Item6Count = "Item6Count";

	public uint VipLvl { get; private set; }				// //vip等级
	public uint Recharge { get; private set; }				// //充值数量
	public uint Money { get; private set; }				// //奖励金钱
	public uint RealMoney { get; private set; }				// //奖励元宝
	public uint Item1Id { get; private set; }				// //奖励物品
	public uint Item1Count { get; private set; }				// //奖励物品数量
	public uint Item2Id { get; private set; }				// //奖励物品2id
	public uint Item2Count { get; private set; }				// //奖励物品数量
	public uint Item3Id { get; private set; }				// //奖励物品
	public uint Item3Count { get; private set; }				// //奖励物品数量
	public uint Item4Id { get; private set; }				// //奖励物品
	public uint Item4Count { get; private set; }				// //奖励物品数量
	public uint Item5Id { get; private set; }				// //奖励物品
	public uint Item5Count { get; private set; }				// //奖励物品数量
	public uint Item6Id { get; private set; }				// //奖励物品
	public uint Item6Count { get; private set; }				// //奖励物品数量

	public XCfgVipAward()
	{
	}

	public uint GetKey1() { return VipLvl; }

	public bool ReadItem(TabFile tf)
	{
		VipLvl = tf.Get<uint>(_KEY_VipLvl);
		Recharge = tf.Get<uint>(_KEY_Recharge);
		Money = tf.Get<uint>(_KEY_Money);
		RealMoney = tf.Get<uint>(_KEY_RealMoney);
		Item1Id = tf.Get<uint>(_KEY_Item1Id);
		Item1Count = tf.Get<uint>(_KEY_Item1Count);
		Item2Id = tf.Get<uint>(_KEY_Item2Id);
		Item2Count = tf.Get<uint>(_KEY_Item2Count);
		Item3Id = tf.Get<uint>(_KEY_Item3Id);
		Item3Count = tf.Get<uint>(_KEY_Item3Count);
		Item4Id = tf.Get<uint>(_KEY_Item4Id);
		Item4Count = tf.Get<uint>(_KEY_Item4Count);
		Item5Id = tf.Get<uint>(_KEY_Item5Id);
		Item5Count = tf.Get<uint>(_KEY_Item5Count);
		Item6Id = tf.Get<uint>(_KEY_Item6Id);
		Item6Count = tf.Get<uint>(_KEY_Item6Count);
		return true;
	}
}

