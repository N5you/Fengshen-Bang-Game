
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgClassLvUpMgr : CCfg1KeyMgrTemplate<XCfgClassLvUpMgr, uint, XCfgClassLvUp> { };

partial class XCfgClassLvUp : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ClassLevel = "ClassLevel";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_LvRequire = "LvRequire";
	public static readonly string _KEY_GrowthRequire = "GrowthRequire";
	public static readonly string _KEY_MaxGrowth = "MaxGrowth";
	public static readonly string _KEY_GrowthAdd = "GrowthAdd";
	public static readonly string _KEY_CostMoney = "CostMoney";
	public static readonly string _KEY_ModelIndex = "ModelIndex";
	public static readonly string _KEY_PanaceaCount = "PanaceaCount";
	public static readonly string _KEY_AddAttrValue = "AddAttrValue";
	public static readonly string _KEY_ItemID_4_0 = "ItemID_4_0";
	public static readonly string _KEY_ItemID_4_1 = "ItemID_4_1";
	public static readonly string _KEY_ItemID_4_2 = "ItemID_4_2";
	public static readonly string _KEY_ItemID_4_3 = "ItemID_4_3";
	public static readonly string _KEY_CostRealMoney = "CostRealMoney";

	public uint ClassLevel { get; private set; }				// 境界等级
	public string Name { get; private set; }				// 境界名
	public uint LvRequire { get; private set; }				// 角色等级要求
	public float GrowthRequire { get; private set; }				// 升级所需成长值要求
	public float MaxGrowth { get; private set; }				// 本境界等级的成长值上限
	public float GrowthAdd { get; private set; }				// 升级至下一境界后的成长值加成
	public uint CostMoney { get; private set; }				// 铜币数目
	public uint ModelIndex { get; private set; }				// 境界对应的模型显示（只对宠物有效）
	public uint PanaceaCount { get; private set; }				// 境界对应属性上限
	public uint AddAttrValue { get; private set; }				// 单项属性增加值
	public uint[] ItemID { get; private set; }				// 武力丹药ID
	public uint CostRealMoney { get; private set; }				// 消耗的元宝

	public XCfgClassLvUp()
	{
		ItemID = new uint[4];
	}

	public uint GetKey1() { return ClassLevel; }

	public bool ReadItem(TabFile tf)
	{
		ClassLevel = tf.Get<uint>(_KEY_ClassLevel);
		Name = tf.Get<string>(_KEY_Name);
		LvRequire = tf.Get<uint>(_KEY_LvRequire);
		GrowthRequire = tf.Get<float>(_KEY_GrowthRequire);
		MaxGrowth = tf.Get<float>(_KEY_MaxGrowth);
		GrowthAdd = tf.Get<float>(_KEY_GrowthAdd);
		CostMoney = tf.Get<uint>(_KEY_CostMoney);
		ModelIndex = tf.Get<uint>(_KEY_ModelIndex);
		PanaceaCount = tf.Get<uint>(_KEY_PanaceaCount);
		AddAttrValue = tf.Get<uint>(_KEY_AddAttrValue);
		ItemID[0] = tf.Get<uint>(_KEY_ItemID_4_0);
		ItemID[1] = tf.Get<uint>(_KEY_ItemID_4_1);
		ItemID[2] = tf.Get<uint>(_KEY_ItemID_4_2);
		ItemID[3] = tf.Get<uint>(_KEY_ItemID_4_3);
		CostRealMoney = tf.Get<uint>(_KEY_CostRealMoney);
		return true;
	}
}

