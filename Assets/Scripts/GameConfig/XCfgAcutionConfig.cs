
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgAcutionConfigMgr : CCfg2KeyMgrTemplate<XCfgAcutionConfigMgr, ushort, ushort, XCfgAcutionConfig> { };

partial class XCfgAcutionConfig : ITabItemWith2Key<ushort, ushort>
{
	public static readonly string _KEY_ItemType = "ItemType";
	public static readonly string _KEY_ItemSubType = "ItemSubType";
	public static readonly string _KEY_ItemTypeName = "ItemTypeName";
	public static readonly string _KEY_ItemsSubName = "ItemsSubName";

	public ushort ItemType { get; private set; }				// 大类
	public ushort ItemSubType { get; private set; }				// 子类
	public string ItemTypeName { get; private set; }				// 大类名称
	public string ItemsSubName { get; private set; }				// 子类名称

	public XCfgAcutionConfig()
	{
	}

	public ushort GetKey1() { return ItemType; }

	public ushort GetKey2() { return ItemSubType; }

	public bool ReadItem(TabFile tf)
	{
		ItemType = tf.Get<ushort>(_KEY_ItemType);
		ItemSubType = tf.Get<ushort>(_KEY_ItemSubType);
		ItemTypeName = tf.Get<string>(_KEY_ItemTypeName);
		ItemsSubName = tf.Get<string>(_KEY_ItemsSubName);
		return true;
	}
}

