
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgShengWangLvlMgr : CCfg1KeyMgrTemplate<XCfgShengWangLvlMgr, uint, XCfgShengWangLvl> { };

partial class XCfgShengWangLvl : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_PrestigeLv = "PrestigeLv";
	public static readonly string _KEY_PrestigeValue = "PrestigeValue";

	public uint PrestigeLv { get; private set; }				// //对应声望等级
	public uint PrestigeValue { get; private set; }				// //声望值

	public XCfgShengWangLvl()
	{
	}

	public uint GetKey1() { return PrestigeLv; }

	public bool ReadItem(TabFile tf)
	{
		PrestigeLv = tf.Get<uint>(_KEY_PrestigeLv);
		PrestigeValue = tf.Get<uint>(_KEY_PrestigeValue);
		return true;
	}
}

