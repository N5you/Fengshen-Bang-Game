
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMoneyTreeMgr : CCfg1KeyMgrTemplate<XCfgMoneyTreeMgr, uint, XCfgMoneyTree> { };

partial class XCfgMoneyTree : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_MaxShakeEveryDay = "MaxShakeEveryDay";
	public static readonly string _KEY_FirstCostRealMoney = "FirstCostRealMoney";
	public static readonly string _KEY_CostRealMoney = "CostRealMoney";

	public uint Level { get; private set; }				// 等级
	public uint MaxShakeEveryDay { get; private set; }				// 每天摇钱次数
	public int FirstCostRealMoney { get; private set; }				// 初次元宝消耗
	public int CostRealMoney { get; private set; }				// 每次元宝增加

	public XCfgMoneyTree()
	{
	}

	public uint GetKey1() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Level = tf.Get<uint>(_KEY_Level);
		MaxShakeEveryDay = tf.Get<uint>(_KEY_MaxShakeEveryDay);
		FirstCostRealMoney = tf.Get<int>(_KEY_FirstCostRealMoney);
		CostRealMoney = tf.Get<int>(_KEY_CostRealMoney);
		return true;
	}
}

