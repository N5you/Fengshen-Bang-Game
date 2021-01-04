
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgFormulaMgr : CCfg1KeyMgrTemplate<XCfgFormulaMgr, uint, XCfgFormula> { };

partial class XCfgFormula : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_ItemType = "ItemType";
	public static readonly string _KEY_LearnExp = "LearnExp";
	public static readonly string _KEY_CostTime = "CostTime";
	public static readonly string _KEY_CostStrength = "CostStrength";
	public static readonly string _KEY_Exp_3_0 = "Exp_3_0";
	public static readonly string _KEY_Exp_3_1 = "Exp_3_1";
	public static readonly string _KEY_Exp_3_2 = "Exp_3_2";
	public static readonly string _KEY_NeedItemId_6_0 = "NeedItemId_6_0";
	public static readonly string _KEY_NeedItemNum_6_0 = "NeedItemNum_6_0";
	public static readonly string _KEY_NeedItemId_6_1 = "NeedItemId_6_1";
	public static readonly string _KEY_NeedItemNum_6_1 = "NeedItemNum_6_1";
	public static readonly string _KEY_NeedItemId_6_2 = "NeedItemId_6_2";
	public static readonly string _KEY_NeedItemNum_6_2 = "NeedItemNum_6_2";
	public static readonly string _KEY_NeedItemId_6_3 = "NeedItemId_6_3";
	public static readonly string _KEY_NeedItemNum_6_3 = "NeedItemNum_6_3";
	public static readonly string _KEY_NeedItemId_6_4 = "NeedItemId_6_4";
	public static readonly string _KEY_NeedItemNum_6_4 = "NeedItemNum_6_4";
	public static readonly string _KEY_NeedItemId_6_5 = "NeedItemId_6_5";
	public static readonly string _KEY_NeedItemNum_6_5 = "NeedItemNum_6_5";
	public static readonly string _KEY_OutputItemId = "OutputItemId";
	public static readonly string _KEY_OutputItemNum = "OutputItemNum";

	public uint ID { get; private set; }				// 配方ID
	public string Name { get; private set; }				// 配方名称
	public string ItemType { get; private set; }				// 产品类别
	public ushort LearnExp { get; private set; }				// 领悟所需最低熟练值
	public uint CostTime { get; private set; }				// 制造时长 S
	public uint CostStrength { get; private set; }				// 消耗的体力值
	public ushort[] Exp { get; private set; }				// plus1熟练度上限值

	public uint[] NeedItemId { get; private set; }				// 需求1物品ID
	public ushort[] NeedItemNum { get; private set; }				// 需求1物品数量
	public uint OutputItemId { get; private set; }				// 产品ID
	public ushort OutputItemNum { get; private set; }				// 产品数量

	public XCfgFormula()
	{
		Exp = new ushort[3];
		NeedItemId = new uint[6];
		NeedItemNum = new ushort[6];
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		Name = tf.Get<string>(_KEY_Name);
		ItemType = tf.Get<string>(_KEY_ItemType);
		LearnExp = tf.Get<ushort>(_KEY_LearnExp);
		CostTime = tf.Get<uint>(_KEY_CostTime);
		CostStrength = tf.Get<uint>(_KEY_CostStrength);
		Exp[0] = tf.Get<ushort>(_KEY_Exp_3_0);
		Exp[1] = tf.Get<ushort>(_KEY_Exp_3_1);
		Exp[2] = tf.Get<ushort>(_KEY_Exp_3_2);
		NeedItemId[0] = tf.Get<uint>(_KEY_NeedItemId_6_0);
		NeedItemNum[0] = tf.Get<ushort>(_KEY_NeedItemNum_6_0);
		NeedItemId[1] = tf.Get<uint>(_KEY_NeedItemId_6_1);
		NeedItemNum[1] = tf.Get<ushort>(_KEY_NeedItemNum_6_1);
		NeedItemId[2] = tf.Get<uint>(_KEY_NeedItemId_6_2);
		NeedItemNum[2] = tf.Get<ushort>(_KEY_NeedItemNum_6_2);
		NeedItemId[3] = tf.Get<uint>(_KEY_NeedItemId_6_3);
		NeedItemNum[3] = tf.Get<ushort>(_KEY_NeedItemNum_6_3);
		NeedItemId[4] = tf.Get<uint>(_KEY_NeedItemId_6_4);
		NeedItemNum[4] = tf.Get<ushort>(_KEY_NeedItemNum_6_4);
		NeedItemId[5] = tf.Get<uint>(_KEY_NeedItemId_6_5);
		NeedItemNum[5] = tf.Get<ushort>(_KEY_NeedItemNum_6_5);
		OutputItemId = tf.Get<uint>(_KEY_OutputItemId);
		OutputItemNum = tf.Get<ushort>(_KEY_OutputItemNum);
		return true;
	}
}

