
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgLevelAdjustMgr : CCfg1KeyMgrTemplate<XCfgLevelAdjustMgr, uint, XCfgLevelAdjust> { };

partial class XCfgLevelAdjust : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_Attack_3_0 = "Attack_3_0";
	public static readonly string _KEY_Attack_3_1 = "Attack_3_1";
	public static readonly string _KEY_Attack_3_2 = "Attack_3_2";
	public static readonly string _KEY_Defense_3_0 = "Defense_3_0";
	public static readonly string _KEY_Defense_3_1 = "Defense_3_1";
	public static readonly string _KEY_Defense_3_2 = "Defense_3_2";
	public static readonly string _KEY_FSDefense_3_0 = "FSDefense_3_0";
	public static readonly string _KEY_FSDefense_3_1 = "FSDefense_3_1";
	public static readonly string _KEY_FSDefense_3_2 = "FSDefense_3_2";
	public static readonly string _KEY_Life_3_0 = "Life_3_0";
	public static readonly string _KEY_Life_3_1 = "Life_3_1";
	public static readonly string _KEY_Life_3_2 = "Life_3_2";
	public static readonly string _KEY_Exp_3_0 = "Exp_3_0";
	public static readonly string _KEY_Exp_3_1 = "Exp_3_1";
	public static readonly string _KEY_Exp_3_2 = "Exp_3_2";

	public uint Level { get; private set; }				// 等级(唯一标识符)
	public uint[] Attack { get; private set; }				// 普通难度攻击力
	public uint[] Defense { get; private set; }				// 普通难度防御力
	public uint[] FSDefense { get; private set; }				// 普通难度法术防御力
	public uint[] Life { get; private set; }				// 普通难度生命值
	public uint[] Exp { get; private set; }				// 普通难度经验值

	public XCfgLevelAdjust()
	{
		Attack = new uint[3];
		Defense = new uint[3];
		FSDefense = new uint[3];
		Life = new uint[3];
		Exp = new uint[3];
	}

	public uint GetKey1() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Level = tf.Get<uint>(_KEY_Level);
		Attack[0] = tf.Get<uint>(_KEY_Attack_3_0);
		Attack[1] = tf.Get<uint>(_KEY_Attack_3_1);
		Attack[2] = tf.Get<uint>(_KEY_Attack_3_2);
		Defense[0] = tf.Get<uint>(_KEY_Defense_3_0);
		Defense[1] = tf.Get<uint>(_KEY_Defense_3_1);
		Defense[2] = tf.Get<uint>(_KEY_Defense_3_2);
		FSDefense[0] = tf.Get<uint>(_KEY_FSDefense_3_0);
		FSDefense[1] = tf.Get<uint>(_KEY_FSDefense_3_1);
		FSDefense[2] = tf.Get<uint>(_KEY_FSDefense_3_2);
		Life[0] = tf.Get<uint>(_KEY_Life_3_0);
		Life[1] = tf.Get<uint>(_KEY_Life_3_1);
		Life[2] = tf.Get<uint>(_KEY_Life_3_2);
		Exp[0] = tf.Get<uint>(_KEY_Exp_3_0);
		Exp[1] = tf.Get<uint>(_KEY_Exp_3_1);
		Exp[2] = tf.Get<uint>(_KEY_Exp_3_2);
		return true;
	}
}

