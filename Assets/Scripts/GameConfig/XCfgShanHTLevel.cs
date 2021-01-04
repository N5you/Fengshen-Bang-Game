
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgShanHTLevelMgr : CCfg1KeyMgrTemplate<XCfgShanHTLevelMgr, uint, XCfgShanHTLevel> { };

partial class XCfgShanHTLevel : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Type = "Type";
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_Reputation = "Reputation";
	public static readonly string _KEY_Money = "Money";
	public static readonly string _KEY_Exp = "Exp";
	public static readonly string _KEY_TCID = "TCID";
	public static readonly string _KEY_TipID = "TipID";
	public static readonly string _KEY_MonID_9_0 = "MonID_9_0";
	public static readonly string _KEY_MonID_9_1 = "MonID_9_1";
	public static readonly string _KEY_MonID_9_2 = "MonID_9_2";
	public static readonly string _KEY_MonID_9_3 = "MonID_9_3";
	public static readonly string _KEY_MonID_9_4 = "MonID_9_4";
	public static readonly string _KEY_MonID_9_5 = "MonID_9_5";
	public static readonly string _KEY_MonID_9_6 = "MonID_9_6";
	public static readonly string _KEY_MonID_9_7 = "MonID_9_7";
	public static readonly string _KEY_MonID_9_8 = "MonID_9_8";
	public static readonly string _KEY_TargetLevel = "TargetLevel";
	public static readonly string _KEY_FixValue = "FixValue";
	public static readonly string _KEY_FightValue = "FightValue";

	public uint Index { get; private set; }				// 关卡ID(唯一标识符)
	public string Name { get; private set; }				// 名字
	public uint Type { get; private set; }				// 关卡类型(0普通1精英2Boss)
	public uint SceneID { get; private set; }				// 场景ID
	public uint Reputation { get; private set; }				// 声望
	public uint Money { get; private set; }				// 金钱
	public uint Exp { get; private set; }				// 经验
	public uint TCID { get; private set; }				// TC掉落
	public uint TipID { get; private set; }				// 关卡掉落TipID
	public uint[] MonID { get; private set; }				// 位置1怪物ID
	public uint TargetLevel { get; private set; }				// 怪物目标等级
	public float FixValue { get; private set; }				// 怪物能力调整系数
	public uint FightValue { get; private set; }				// 显示关卡怪物的战斗力

	public XCfgShanHTLevel()
	{
		MonID = new uint[9];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		Type = tf.Get<uint>(_KEY_Type);
		SceneID = tf.Get<uint>(_KEY_SceneID);
		Reputation = tf.Get<uint>(_KEY_Reputation);
		Money = tf.Get<uint>(_KEY_Money);
		Exp = tf.Get<uint>(_KEY_Exp);
		TCID = tf.Get<uint>(_KEY_TCID);
		TipID = tf.Get<uint>(_KEY_TipID);
		MonID[0] = tf.Get<uint>(_KEY_MonID_9_0);
		MonID[1] = tf.Get<uint>(_KEY_MonID_9_1);
		MonID[2] = tf.Get<uint>(_KEY_MonID_9_2);
		MonID[3] = tf.Get<uint>(_KEY_MonID_9_3);
		MonID[4] = tf.Get<uint>(_KEY_MonID_9_4);
		MonID[5] = tf.Get<uint>(_KEY_MonID_9_5);
		MonID[6] = tf.Get<uint>(_KEY_MonID_9_6);
		MonID[7] = tf.Get<uint>(_KEY_MonID_9_7);
		MonID[8] = tf.Get<uint>(_KEY_MonID_9_8);
		TargetLevel = tf.Get<uint>(_KEY_TargetLevel);
		FixValue = tf.Get<float>(_KEY_FixValue);
		FightValue = tf.Get<uint>(_KEY_FightValue);
		return true;
	}
}

