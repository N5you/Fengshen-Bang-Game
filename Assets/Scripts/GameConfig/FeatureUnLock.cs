
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class FeatureUnLockMgr : CCfg1KeyMgrTemplate<FeatureUnLockMgr, uint, FeatureUnLock> { };

partial class FeatureUnLock : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Des = "Des";
	public static readonly string _KEY_AtlasID = "AtlasID";
	public static readonly string _KEY_IconID_Com = "IconID_Com";
	public static readonly string _KEY_IconID_Hover = "IconID_Hover";
	public static readonly string _KEY_IconID_Pressed = "IconID_Pressed";
	public static readonly string _KEY_Group = "Group";
	public static readonly string _KEY_SortID = "SortID";
	public static readonly string _KEY_RequireLevel = "RequireLevel";
	public static readonly string _KEY_RequireQuest = "RequireQuest";
	public static readonly string _KEY_RequireDupID = "RequireDupID";
	public static readonly string _KEY_AnimationType = "AnimationType";
	public static readonly string _KEY_MixID = "MixID";

	public uint Index { get; private set; }				// 控件ID
	public string Des { get; private set; }				// 控件文字
	public uint AtlasID { get; private set; }				// 图标图集ID
	public string IconID_Com { get; private set; }				// 图标ID
	public string IconID_Hover { get; private set; }				// 图标ID
	public string IconID_Pressed { get; private set; }				// 图标ID
	public uint Group { get; private set; }				// 控件组
	public uint SortID { get; private set; }				// 排序值
	public uint RequireLevel { get; private set; }				// 等级要求
	public uint RequireQuest { get; private set; }				// 任务要求
	public uint RequireDupID { get; private set; }				// 副本要求
	public uint AnimationType { get; private set; }				// 1插入2融入
	public uint MixID { get; private set; }				// 融入控件ID

	public FeatureUnLock()
	{
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Des = tf.Get<string>(_KEY_Des);
		AtlasID = tf.Get<uint>(_KEY_AtlasID);
		IconID_Com = tf.Get<string>(_KEY_IconID_Com);
		IconID_Hover = tf.Get<string>(_KEY_IconID_Hover);
		IconID_Pressed = tf.Get<string>(_KEY_IconID_Pressed);
		Group = tf.Get<uint>(_KEY_Group);
		SortID = tf.Get<uint>(_KEY_SortID);
		RequireLevel = tf.Get<uint>(_KEY_RequireLevel);
		RequireQuest = tf.Get<uint>(_KEY_RequireQuest);
		RequireDupID = tf.Get<uint>(_KEY_RequireDupID);
		AnimationType = tf.Get<uint>(_KEY_AnimationType);
		MixID = tf.Get<uint>(_KEY_MixID);
		return true;
	}
}

