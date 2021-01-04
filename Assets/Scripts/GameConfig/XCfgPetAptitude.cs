
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPetAptitudeMgr : CCfg1KeyMgrTemplate<XCfgPetAptitudeMgr, uint, XCfgPetAptitude> { };

partial class XCfgPetAptitude : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_ChangeRate = "ChangeRate";

	public uint Index { get; private set; }				// ID(唯一标识符, 不能为0)
	public string Name { get; private set; }				// 资质名称
	public float ChangeRate { get; private set; }				// 资质能力转化系数

	public XCfgPetAptitude()
	{
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		ChangeRate = tf.Get<float>(_KEY_ChangeRate);
		return true;
	}
}

