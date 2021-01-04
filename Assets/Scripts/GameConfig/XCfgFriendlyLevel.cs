
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgFriendlyLevelMgr : CCfg1KeyMgrTemplate<XCfgFriendlyLevelMgr, uint, XCfgFriendlyLevel> { };

partial class XCfgFriendlyLevel : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_UID = "UID";
	public static readonly string _KEY_FriendlyName = "FriendlyName";
	public static readonly string _KEY_FriendlyValue = "FriendlyValue";

	public uint UID { get; private set; }				// ID
	public string FriendlyName { get; private set; }				// 名称
	public uint FriendlyValue { get; private set; }				// 友好度

	public XCfgFriendlyLevel()
	{
	}

	public uint GetKey1() { return UID; }

	public bool ReadItem(TabFile tf)
	{
		UID = tf.Get<uint>(_KEY_UID);
		FriendlyName = tf.Get<string>(_KEY_FriendlyName);
		FriendlyValue = tf.Get<uint>(_KEY_FriendlyValue);
		return true;
	}
}

