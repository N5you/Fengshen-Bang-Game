
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgArmourGroupMgr : CCfgListMgrTemplate<XCfgArmourGroupMgr, XCfgArmourGroup> { };

partial class XCfgArmourGroup : ITabItem
{
	public static readonly string _KEY_EquipGroupID = "EquipGroupID";
	public static readonly string _KEY_Sex = "Sex";
	public static readonly string _KEY_JobID = "JobID";
	public static readonly string _KEY_LevelID_5_0 = "LevelID_5_0";
	public static readonly string _KEY_LevelID_5_1 = "LevelID_5_1";
	public static readonly string _KEY_LevelID_5_2 = "LevelID_5_2";
	public static readonly string _KEY_LevelID_5_3 = "LevelID_5_3";
	public static readonly string _KEY_LevelID_5_4 = "LevelID_5_4";

	public uint EquipGroupID { get; private set; }				// 装备组ID（武器和铠甲）
	public byte Sex { get; private set; }				// 性别(1男2女)
	public uint JobID { get; private set; }				// 1战士2法师3弓箭手
	public uint[] LevelID { get; private set; }				// 模型ID

	public XCfgArmourGroup()
	{
		LevelID = new uint[5];
	}

	public bool ReadItem(TabFile tf)
	{
		EquipGroupID = tf.Get<uint>(_KEY_EquipGroupID);
		Sex = tf.Get<byte>(_KEY_Sex);
		JobID = tf.Get<uint>(_KEY_JobID);
		LevelID[0] = tf.Get<uint>(_KEY_LevelID_5_0);
		LevelID[1] = tf.Get<uint>(_KEY_LevelID_5_1);
		LevelID[2] = tf.Get<uint>(_KEY_LevelID_5_2);
		LevelID[3] = tf.Get<uint>(_KEY_LevelID_5_3);
		LevelID[4] = tf.Get<uint>(_KEY_LevelID_5_4);
		return true;
	}
}

