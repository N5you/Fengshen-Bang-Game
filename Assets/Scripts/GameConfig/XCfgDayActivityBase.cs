
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgDayActivityBaseMgr : CCfg1KeyMgrTemplate<XCfgDayActivityBaseMgr, uint, XCfgDayActivityBase> { };

partial class XCfgDayActivityBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_FUID = "FUID";
	public static readonly string _KEY_SortLevel = "SortLevel";
	public static readonly string _KEY_ConditionEnum = "ConditionEnum";
	public static readonly string _KEY_Completions = "Completions";
	public static readonly string _KEY_VitalityValue = "VitalityValue";
	public static readonly string _KEY_ButtonName = "ButtonName";
	public static readonly string _KEY_LinkUI = "LinkUI";
	public static readonly string _KEY_IconID_4_0 = "IconID_4_0";
	public static readonly string _KEY_IconID_4_1 = "IconID_4_1";
	public static readonly string _KEY_IconID_4_2 = "IconID_4_2";
	public static readonly string _KEY_IconID_4_3 = "IconID_4_3";
	public static readonly string _KEY_awardTips_4_0 = "awardTips_4_0";
	public static readonly string _KEY_awardTips_4_1 = "awardTips_4_1";
	public static readonly string _KEY_awardTips_4_2 = "awardTips_4_2";
	public static readonly string _KEY_awardTips_4_3 = "awardTips_4_3";

	public uint Index { get; private set; }				// 活跃度项ID
	public string Name { get; private set; }				// 项目名称
	public uint FUID { get; private set; }				// 解锁ID
	public uint SortLevel { get; private set; }				// 排序ID
	public uint ConditionEnum { get; private set; }				// 完成条件类型
	public ushort Completions { get; private set; }				// 完成次数
	public int VitalityValue { get; private set; }				// 项目获得活跃度
	public string ButtonName { get; private set; }				// 按钮名称
	public ushort LinkUI { get; private set; }				// 开启UI
	public string[] IconID { get; private set; }				// 奖励图标1
	public string[] awardTips { get; private set; }				// 奖励描述1

	public XCfgDayActivityBase()
	{
		IconID = new string[4];
		awardTips = new string[4];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		FUID = tf.Get<uint>(_KEY_FUID);
		SortLevel = tf.Get<uint>(_KEY_SortLevel);
		ConditionEnum = tf.Get<uint>(_KEY_ConditionEnum);
		Completions = tf.Get<ushort>(_KEY_Completions);
		VitalityValue = tf.Get<int>(_KEY_VitalityValue);
		ButtonName = tf.Get<string>(_KEY_ButtonName);
		LinkUI = tf.Get<ushort>(_KEY_LinkUI);
		IconID[0] = tf.Get<string>(_KEY_IconID_4_0);
		IconID[1] = tf.Get<string>(_KEY_IconID_4_1);
		IconID[2] = tf.Get<string>(_KEY_IconID_4_2);
		IconID[3] = tf.Get<string>(_KEY_IconID_4_3);
		awardTips[0] = tf.Get<string>(_KEY_awardTips_4_0);
		awardTips[1] = tf.Get<string>(_KEY_awardTips_4_1);
		awardTips[2] = tf.Get<string>(_KEY_awardTips_4_2);
		awardTips[3] = tf.Get<string>(_KEY_awardTips_4_3);
		return true;
	}
}

