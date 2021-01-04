
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSuitMgr : CCfg1KeyMgrTemplate<XCfgSuitMgr, uint, XCfgSuit> { };

partial class XCfgSuit : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_SuitID = "SuitID";
	public static readonly string _KEY_SuitName = "SuitName";
	public static readonly string _KEY_AttrID_3_0 = "AttrID_3_0";
	public static readonly string _KEY_AttrValue_3_0 = "AttrValue_3_0";
	public static readonly string _KEY_SuitNeedNum_3_0 = "SuitNeedNum_3_0";
	public static readonly string _KEY_AttrID_3_1 = "AttrID_3_1";
	public static readonly string _KEY_AttrValue_3_1 = "AttrValue_3_1";
	public static readonly string _KEY_SuitNeedNum_3_1 = "SuitNeedNum_3_1";
	public static readonly string _KEY_AttrID_3_2 = "AttrID_3_2";
	public static readonly string _KEY_AttrValue_3_2 = "AttrValue_3_2";
	public static readonly string _KEY_SuitNeedNum_3_2 = "SuitNeedNum_3_2";

	public uint SuitID { get; private set; }				// 套装ID
	public string SuitName { get; private set; }				// 套装名称
	public ushort[] AttrID { get; private set; }				// 属性ID0
	public uint[] AttrValue { get; private set; }				// 属性值0
	public byte[] SuitNeedNum { get; private set; }				// 属性1激活需要件数

	public XCfgSuit()
	{
		AttrID = new ushort[3];
		AttrValue = new uint[3];
		SuitNeedNum = new byte[3];
	}

	public uint GetKey1() { return SuitID; }

	public bool ReadItem(TabFile tf)
	{
		SuitID = tf.Get<uint>(_KEY_SuitID);
		SuitName = tf.Get<string>(_KEY_SuitName);
		AttrID[0] = tf.Get<ushort>(_KEY_AttrID_3_0);
		AttrValue[0] = tf.Get<uint>(_KEY_AttrValue_3_0);
		SuitNeedNum[0] = tf.Get<byte>(_KEY_SuitNeedNum_3_0);
		AttrID[1] = tf.Get<ushort>(_KEY_AttrID_3_1);
		AttrValue[1] = tf.Get<uint>(_KEY_AttrValue_3_1);
		SuitNeedNum[1] = tf.Get<byte>(_KEY_SuitNeedNum_3_1);
		AttrID[2] = tf.Get<ushort>(_KEY_AttrID_3_2);
		AttrValue[2] = tf.Get<uint>(_KEY_AttrValue_3_2);
		SuitNeedNum[2] = tf.Get<byte>(_KEY_SuitNeedNum_3_2);
		return true;
	}
}

