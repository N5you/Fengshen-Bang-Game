
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgHealthBuyMgr : CCfg1KeyMgrTemplate<XCfgHealthBuyMgr, uint, XCfgHealthBuy> { };

partial class XCfgHealthBuy : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_VIPLevel = "VIPLevel";
	public static readonly string _KEY_BuyCount = "BuyCount";
	public static readonly string _KEY_FirstCost = "FirstCost";
	public static readonly string _KEY_CostDelta = "CostDelta";
	public static readonly string _KEY_HealthValue = "HealthValue";

	public uint VIPLevel { get; private set; }				// 职业ID
	public uint BuyCount { get; private set; }				// 体力购买次数，该VIP等级的每日体力购买最大次数
	public uint FirstCost { get; private set; }				// 初次购买花费元宝数目
	public uint CostDelta { get; private set; }				// 次数递增元宝花费
	public uint HealthValue { get; private set; }				// 体质

	public XCfgHealthBuy()
	{
	}

	public uint GetKey1() { return VIPLevel; }

	public bool ReadItem(TabFile tf)
	{
		VIPLevel = tf.Get<uint>(_KEY_VIPLevel);
		BuyCount = tf.Get<uint>(_KEY_BuyCount);
		FirstCost = tf.Get<uint>(_KEY_FirstCost);
		CostDelta = tf.Get<uint>(_KEY_CostDelta);
		HealthValue = tf.Get<uint>(_KEY_HealthValue);
		return true;
	}
}

