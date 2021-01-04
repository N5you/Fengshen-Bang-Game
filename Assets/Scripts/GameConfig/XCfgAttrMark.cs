
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgAttrMarkMgr : CCfg1KeyMgrTemplate<XCfgAttrMarkMgr, uint, XCfgAttrMark> { };

partial class XCfgAttrMark : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_JobID = "JobID";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_AttrMark_5_0 = "AttrMark_5_0";
	public static readonly string _KEY_AttrMark_5_1 = "AttrMark_5_1";
	public static readonly string _KEY_AttrMark_5_2 = "AttrMark_5_2";
	public static readonly string _KEY_AttrMark_5_3 = "AttrMark_5_3";
	public static readonly string _KEY_AttrMark_5_4 = "AttrMark_5_4";

	public uint JobID { get; private set; }				// 职业ID
	public string Name { get; private set; }				// 职业名描述
	public byte[] AttrMark { get; private set; }				// 武力

	public XCfgAttrMark()
	{
		AttrMark = new byte[5];
	}

	public uint GetKey1() { return JobID; }

	public bool ReadItem(TabFile tf)
	{
		JobID = tf.Get<uint>(_KEY_JobID);
		Name = tf.Get<string>(_KEY_Name);
		AttrMark[0] = tf.Get<byte>(_KEY_AttrMark_5_0);
		AttrMark[1] = tf.Get<byte>(_KEY_AttrMark_5_1);
		AttrMark[2] = tf.Get<byte>(_KEY_AttrMark_5_2);
		AttrMark[3] = tf.Get<byte>(_KEY_AttrMark_5_3);
		AttrMark[4] = tf.Get<byte>(_KEY_AttrMark_5_4);
		return true;
	}
}

