
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgTouZiMgr : CCfg1KeyMgrTemplate<XCfgTouZiMgr, uint, XCfgTouZi> { };

partial class XCfgTouZi : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_DayIndex = "DayIndex";
	public static readonly string _KEY_Rate = "Rate";

	public uint DayIndex { get; private set; }				// //第几天领取
	public float Rate { get; private set; }				// //返还比例

	public XCfgTouZi()
	{
	}

	public uint GetKey1() { return DayIndex; }

	public bool ReadItem(TabFile tf)
	{
		DayIndex = tf.Get<uint>(_KEY_DayIndex);
		Rate = tf.Get<float>(_KEY_Rate);
		return true;
	}
}

