
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMissionsDynamicMgr : CCfg1KeyMgrTemplate<XCfgMissionsDynamicMgr, uint, XCfgMissionsDynamic> { };

partial class XCfgMissionsDynamic : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_level = "level";
	public static readonly string _KEY_expParam = "expParam";
	public static readonly string _KEY_moneyParam = "moneyParam";
	public static readonly string _KEY_gangExpParam = "gangExpParam";

	public uint level { get; private set; }				// //全局唯一标识
	public uint expParam { get; private set; }				// //经验系数
	public uint moneyParam { get; private set; }				// //铜币系数
	public uint gangExpParam { get; private set; }				// //帮派经验参数

	public XCfgMissionsDynamic()
	{
	}

	public uint GetKey1() { return level; }

	public bool ReadItem(TabFile tf)
	{
		level = tf.Get<uint>(_KEY_level);
		expParam = tf.Get<uint>(_KEY_expParam);
		moneyParam = tf.Get<uint>(_KEY_moneyParam);
		gangExpParam = tf.Get<uint>(_KEY_gangExpParam);
		return true;
	}
}

