
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgEquipPosRateMgr : CCfg1KeyMgrTemplate<XCfgEquipPosRateMgr, uint, XCfgEquipPosRate> { };

partial class XCfgEquipPosRate : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_EquipPos = "EquipPos";
	public static readonly string _KEY_Value = "Value";

	public uint EquipPos { get; private set; }				// 装备部位
	public float Value { get; private set; }				// 部位对属性的影响

	public XCfgEquipPosRate()
	{
	}

	public uint GetKey1() { return EquipPos; }

	public bool ReadItem(TabFile tf)
	{
		EquipPos = tf.Get<uint>(_KEY_EquipPos);
		Value = tf.Get<float>(_KEY_Value);
		return true;
	}
}

