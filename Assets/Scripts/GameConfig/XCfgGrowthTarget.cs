
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgGrowthTargetMgr : CCfg1KeyMgrTemplate<XCfgGrowthTargetMgr, int, XCfgGrowthTarget> { };

partial class XCfgGrowthTarget : ITabItemWith1Key<int>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Type = "Type";
	public static readonly string _KEY_Status = "Status";
	public static readonly string _KEY_Range = "Range";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_Description = "Description";
	public static readonly string _KEY_AwardItemID1 = "AwardItemID1";
	public static readonly string _KEY_AwardItemCount1 = "AwardItemCount1";
	public static readonly string _KEY_AwardItemID2 = "AwardItemID2";
	public static readonly string _KEY_AwardItemCount2 = "AwardItemCount2";
	public static readonly string _KEY_AwardItemID3 = "AwardItemID3";
	public static readonly string _KEY_AwardItemCount3 = "AwardItemCount3";
	public static readonly string _KEY_AwardItemID4 = "AwardItemID4";
	public static readonly string _KEY_AwardItemCount4 = "AwardItemCount4";
	public static readonly string _KEY_AwardMonty = "AwardMonty";
	public static readonly string _KEY_AwardIngot = "AwardIngot";
	public static readonly string _KEY_AwardExp = "AwardExp";

	public int ID { get; private set; }				// 成长目标
	public int Type { get; private set; }				// 成长目标类型
	public int Status { get; private set; }				// 当前目标状态
	public string Range { get; private set; }				// 二级目录的范围
	public string Name { get; private set; }				// 成长目标名
	public uint Level { get; private set; }				// 等级限制
	public string Description { get; private set; }				// 目标描述
	public uint AwardItemID1 { get; private set; }				// 奖励物品1
	public ushort AwardItemCount1 { get; private set; }				// 奖励物品1数量
	public uint AwardItemID2 { get; private set; }				// 奖励物品2
	public ushort AwardItemCount2 { get; private set; }				// 奖励物品2数量
	public uint AwardItemID3 { get; private set; }				// 奖励物品3
	public ushort AwardItemCount3 { get; private set; }				// 奖励物品3数量
	public uint AwardItemID4 { get; private set; }				// 奖励物品4
	public ushort AwardItemCount4 { get; private set; }				// 奖励物品4数量
	public uint AwardMonty { get; private set; }				// 奖励铜钱数量
	public uint AwardIngot { get; private set; }				// 奖励元宝数量
	public uint AwardExp { get; private set; }				// 奖励经验数

	public XCfgGrowthTarget()
	{
	}

	public int GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<int>(_KEY_ID);
		Type = tf.Get<int>(_KEY_Type);
		Status = tf.Get<int>(_KEY_Status);
		Range = tf.Get<string>(_KEY_Range);
		Name = tf.Get<string>(_KEY_Name);
		Level = tf.Get<uint>(_KEY_Level);
		Description = tf.Get<string>(_KEY_Description);
		AwardItemID1 = tf.Get<uint>(_KEY_AwardItemID1);
		AwardItemCount1 = tf.Get<ushort>(_KEY_AwardItemCount1);
		AwardItemID2 = tf.Get<uint>(_KEY_AwardItemID2);
		AwardItemCount2 = tf.Get<ushort>(_KEY_AwardItemCount2);
		AwardItemID3 = tf.Get<uint>(_KEY_AwardItemID3);
		AwardItemCount3 = tf.Get<ushort>(_KEY_AwardItemCount3);
		AwardItemID4 = tf.Get<uint>(_KEY_AwardItemID4);
		AwardItemCount4 = tf.Get<ushort>(_KEY_AwardItemCount4);
		AwardMonty = tf.Get<uint>(_KEY_AwardMonty);
		AwardIngot = tf.Get<uint>(_KEY_AwardIngot);
		AwardExp = tf.Get<uint>(_KEY_AwardExp);
		return true;
	}
}

