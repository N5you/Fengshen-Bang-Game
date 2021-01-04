
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMeditationMgr : CCfg1KeyMgrTemplate<XCfgMeditationMgr, uint, XCfgMeditation> { };

partial class XCfgMeditation : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_VIPLevel = "VIPLevel";
	public static readonly string _KEY_MaxTime = "MaxTime";
	public static readonly string _KEY_ExpRate = "ExpRate";

	public uint VIPLevel { get; private set; }				// VIP等级
	public int MaxTime { get; private set; }				// 打坐最长时间
	public int ExpRate { get; private set; }				// 打坐经验倍率

	public XCfgMeditation()
	{
	}

	public uint GetKey1() { return VIPLevel; }

	public bool ReadItem(TabFile tf)
	{
		VIPLevel = tf.Get<uint>(_KEY_VIPLevel);
		MaxTime = tf.Get<int>(_KEY_MaxTime);
		ExpRate = tf.Get<int>(_KEY_ExpRate);
		return true;
	}
}

