
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgLevelEntryMgr : CCfg1KeyMgrTemplate<XCfgLevelEntryMgr, uint, XCfgLevelEntry> { };

partial class XCfgLevelEntry : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_Position = "Position";
	public static readonly string _KEY_ModeID = "ModeID";
	public static readonly string _KEY_Level_16_0 = "Level_16_0";
	public static readonly string _KEY_Level_16_1 = "Level_16_1";
	public static readonly string _KEY_Level_16_2 = "Level_16_2";
	public static readonly string _KEY_Level_16_3 = "Level_16_3";
	public static readonly string _KEY_Level_16_4 = "Level_16_4";
	public static readonly string _KEY_Level_16_5 = "Level_16_5";
	public static readonly string _KEY_Level_16_6 = "Level_16_6";
	public static readonly string _KEY_Level_16_7 = "Level_16_7";
	public static readonly string _KEY_Level_16_8 = "Level_16_8";
	public static readonly string _KEY_Level_16_9 = "Level_16_9";
	public static readonly string _KEY_Level_16_10 = "Level_16_10";
	public static readonly string _KEY_Level_16_11 = "Level_16_11";
	public static readonly string _KEY_Level_16_12 = "Level_16_12";
	public static readonly string _KEY_Level_16_13 = "Level_16_13";
	public static readonly string _KEY_Level_16_14 = "Level_16_14";
	public static readonly string _KEY_Level_16_15 = "Level_16_15";

	public uint Index { get; private set; }				// 关卡跳转点ID
	public string Name { get; private set; }				// 跳转点说明
	public uint SceneID { get; private set; }				// 场景ID
	public Vector3 Position { get; private set; }				// 跳转点位置
	public uint ModeID { get; private set; }				// 显示模型ID
	public uint[] Level { get; private set; }				// 关卡ID_0

	public XCfgLevelEntry()
	{
		Level = new uint[16];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		SceneID = tf.Get<uint>(_KEY_SceneID);
		Position = XUtil.String2Vector3(tf.Get<string>(_KEY_Position));
		ModeID = tf.Get<uint>(_KEY_ModeID);
		Level[0] = tf.Get<uint>(_KEY_Level_16_0);
		Level[1] = tf.Get<uint>(_KEY_Level_16_1);
		Level[2] = tf.Get<uint>(_KEY_Level_16_2);
		Level[3] = tf.Get<uint>(_KEY_Level_16_3);
		Level[4] = tf.Get<uint>(_KEY_Level_16_4);
		Level[5] = tf.Get<uint>(_KEY_Level_16_5);
		Level[6] = tf.Get<uint>(_KEY_Level_16_6);
		Level[7] = tf.Get<uint>(_KEY_Level_16_7);
		Level[8] = tf.Get<uint>(_KEY_Level_16_8);
		Level[9] = tf.Get<uint>(_KEY_Level_16_9);
		Level[10] = tf.Get<uint>(_KEY_Level_16_10);
		Level[11] = tf.Get<uint>(_KEY_Level_16_11);
		Level[12] = tf.Get<uint>(_KEY_Level_16_12);
		Level[13] = tf.Get<uint>(_KEY_Level_16_13);
		Level[14] = tf.Get<uint>(_KEY_Level_16_14);
		Level[15] = tf.Get<uint>(_KEY_Level_16_15);
		return true;
	}
}

