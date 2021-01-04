
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgDecomposeMgr : CCfgListMgrTemplate<XCfgDecomposeMgr, XCfgDecompose> { };

partial class XCfgDecompose : ITabItem
{
	public static readonly string _KEY_EquipLevel = "EquipLevel";
	public static readonly string _KEY_EquipColorLevel = "EquipColorLevel";
	public static readonly string _KEY_StrengthenLevel = "StrengthenLevel";
	public static readonly string _KEY_IsCanDecompose = "IsCanDecompose";
	public static readonly string _KEY_Material_2_0 = "Material_2_0";
	public static readonly string _KEY_Min_2_0 = "Min_2_0";
	public static readonly string _KEY_Max_2_0 = "Max_2_0";
	public static readonly string _KEY_Material_2_1 = "Material_2_1";
	public static readonly string _KEY_Min_2_1 = "Min_2_1";
	public static readonly string _KEY_Max_2_1 = "Max_2_1";

	public uint EquipLevel { get; private set; }				// 装备等级
	public byte EquipColorLevel { get; private set; }				// 成色
	public byte StrengthenLevel { get; private set; }				// 强化等级
	public byte IsCanDecompose { get; private set; }				// 是否可分解
	public uint[] Material { get; private set; }				// 分解后材料1ID
	public uint[] Min { get; private set; }				// 最小数量
	public uint[] Max { get; private set; }				// 最大数量

	public XCfgDecompose()
	{
		Material = new uint[2];
		Min = new uint[2];
		Max = new uint[2];
	}

	public bool ReadItem(TabFile tf)
	{
		EquipLevel = tf.Get<uint>(_KEY_EquipLevel);
		EquipColorLevel = tf.Get<byte>(_KEY_EquipColorLevel);
		StrengthenLevel = tf.Get<byte>(_KEY_StrengthenLevel);
		IsCanDecompose = tf.Get<byte>(_KEY_IsCanDecompose);
		Material[0] = tf.Get<uint>(_KEY_Material_2_0);
		Min[0] = tf.Get<uint>(_KEY_Min_2_0);
		Max[0] = tf.Get<uint>(_KEY_Max_2_0);
		Material[1] = tf.Get<uint>(_KEY_Material_2_1);
		Min[1] = tf.Get<uint>(_KEY_Min_2_1);
		Max[1] = tf.Get<uint>(_KEY_Max_2_1);
		return true;
	}
}

