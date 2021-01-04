
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgHillSeaBookMgr : CCfg1KeyMgrTemplate<XCfgHillSeaBookMgr, uint, XCfgHillSeaBook> { };

partial class XCfgHillSeaBook : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_BossID = "BossID";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_BattleLevel = "BattleLevel";
	public static readonly string _KEY_ModelID = "ModelID";
	public static readonly string _KEY_Reputation = "Reputation";
	public static readonly string _KEY_Money = "Money";
	public static readonly string _KEY_Exp = "Exp";
	public static readonly string _KEY_TCID = "TCID";
	public static readonly string _KEY_MonID_9_0 = "MonID_9_0";
	public static readonly string _KEY_MonID_9_1 = "MonID_9_1";
	public static readonly string _KEY_MonID_9_2 = "MonID_9_2";
	public static readonly string _KEY_MonID_9_3 = "MonID_9_3";
	public static readonly string _KEY_MonID_9_4 = "MonID_9_4";
	public static readonly string _KEY_MonID_9_5 = "MonID_9_5";
	public static readonly string _KEY_MonID_9_6 = "MonID_9_6";
	public static readonly string _KEY_MonID_9_7 = "MonID_9_7";
	public static readonly string _KEY_MonID_9_8 = "MonID_9_8";
	public static readonly string _KEY_FixValue = "FixValue";
	public static readonly string _KEY_SceneID = "SceneID";

	public uint BossID { get; private set; }				// Boss ID(唯一标识符)从0开始
	public uint Level { get; private set; }				// 需要的等级
	public string Name { get; private set; }				// Boss的名字
	public uint BattleLevel { get; private set; }				// 关卡战斗等级影响敌人属性
	public uint ModelID { get; private set; }				// 显示用模型ID
	public uint Reputation { get; private set; }				// 声望
	public uint Money { get; private set; }				// 金钱
	public uint Exp { get; private set; }				// 经验
	public uint TCID { get; private set; }				// TC掉落
	public uint[] MonID { get; private set; }				// 位置1怪物ID
	public float FixValue { get; private set; }				// 体力扣除系数
	public uint SceneID { get; private set; }				// 显示战斗背景场景ID

	public XCfgHillSeaBook()
	{
		MonID = new uint[9];
	}

	public uint GetKey1() { return BossID; }

	public bool ReadItem(TabFile tf)
	{
		BossID = tf.Get<uint>(_KEY_BossID);
		Level = tf.Get<uint>(_KEY_Level);
		Name = tf.Get<string>(_KEY_Name);
		BattleLevel = tf.Get<uint>(_KEY_BattleLevel);
		ModelID = tf.Get<uint>(_KEY_ModelID);
		Reputation = tf.Get<uint>(_KEY_Reputation);
		Money = tf.Get<uint>(_KEY_Money);
		Exp = tf.Get<uint>(_KEY_Exp);
		TCID = tf.Get<uint>(_KEY_TCID);
		MonID[0] = tf.Get<uint>(_KEY_MonID_9_0);
		MonID[1] = tf.Get<uint>(_KEY_MonID_9_1);
		MonID[2] = tf.Get<uint>(_KEY_MonID_9_2);
		MonID[3] = tf.Get<uint>(_KEY_MonID_9_3);
		MonID[4] = tf.Get<uint>(_KEY_MonID_9_4);
		MonID[5] = tf.Get<uint>(_KEY_MonID_9_5);
		MonID[6] = tf.Get<uint>(_KEY_MonID_9_6);
		MonID[7] = tf.Get<uint>(_KEY_MonID_9_7);
		MonID[8] = tf.Get<uint>(_KEY_MonID_9_8);
		FixValue = tf.Get<float>(_KEY_FixValue);
		SceneID = tf.Get<uint>(_KEY_SceneID);
		return true;
	}
}

