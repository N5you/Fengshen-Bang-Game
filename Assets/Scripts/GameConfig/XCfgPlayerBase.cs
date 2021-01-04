
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPlayerBaseMgr : CCfg1KeyMgrTemplate<XCfgPlayerBaseMgr, byte, XCfgPlayerBase> { };

partial class XCfgPlayerBase : ITabItemWith1Key<byte>
{
	public static readonly string _KEY_Class = "Class";
	public static readonly string _KEY_FemaleModel = "FemaleModel";
	public static readonly string _KEY_MaleModel = "MaleModel";
	public static readonly string _KEY_SelFemaleModel = "SelFemaleModel";
	public static readonly string _KEY_SelMaleModel = "SelMaleModel";
	public static readonly string _KEY_DefaultWeapon = "DefaultWeapon";
	public static readonly string _KEY_SuperSkill = "SuperSkill";

	public byte Class { get; private set; }				// 职业(战士-1,法师-2,弓手-3)
	public uint FemaleModel { get; private set; }				// 女角默认模型ID
	public uint MaleModel { get; private set; }				// 男角默认模型ID
	public uint SelFemaleModel { get; private set; }				// 创建角色女角默认模型ID
	public uint SelMaleModel { get; private set; }				// 创建角色男角默认模型ID
	public uint DefaultWeapon { get; private set; }				// 默认武器模型ID
	public ushort SuperSkill { get; private set; }				// 超级技能

	public XCfgPlayerBase()
	{
	}

	public byte GetKey1() { return Class; }

	public bool ReadItem(TabFile tf)
	{
		Class = tf.Get<byte>(_KEY_Class);
		FemaleModel = tf.Get<uint>(_KEY_FemaleModel);
		MaleModel = tf.Get<uint>(_KEY_MaleModel);
		SelFemaleModel = tf.Get<uint>(_KEY_SelFemaleModel);
		SelMaleModel = tf.Get<uint>(_KEY_SelMaleModel);
		DefaultWeapon = tf.Get<uint>(_KEY_DefaultWeapon);
		SuperSkill = tf.Get<ushort>(_KEY_SuperSkill);
		return true;
	}
}

