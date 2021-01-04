
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgLianZhanMgr : CCfg1KeyMgrTemplate<XCfgLianZhanMgr, uint, XCfgLianZhan> { };

partial class XCfgLianZhan : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ComboCnt = "ComboCnt";
	public static readonly string _KEY_MonsterLevelFixed = "MonsterLevelFixed";
	public static readonly string _KEY_MoneyRate = "MoneyRate";
	public static readonly string _KEY_ShengWangRate = "ShengWangRate";
	public static readonly string _KEY_ShengWang = "ShengWang";

	public uint ComboCnt { get; private set; }				// 连斩次数
	public uint MonsterLevelFixed { get; private set; }				// 怪物等级修正值
	public float MoneyRate { get; private set; }				// 铜钱系数
	public float ShengWangRate { get; private set; }				// 声望系数
	public uint ShengWang { get; private set; }				// 次日声望奖励

	public XCfgLianZhan()
	{
	}

	public uint GetKey1() { return ComboCnt; }

	public bool ReadItem(TabFile tf)
	{
		ComboCnt = tf.Get<uint>(_KEY_ComboCnt);
		MonsterLevelFixed = tf.Get<uint>(_KEY_MonsterLevelFixed);
		MoneyRate = tf.Get<float>(_KEY_MoneyRate);
		ShengWangRate = tf.Get<float>(_KEY_ShengWangRate);
		ShengWang = tf.Get<uint>(_KEY_ShengWang);
		return true;
	}
}

