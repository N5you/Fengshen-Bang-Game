
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgLastNameMgr : CCfgListMgrTemplate<XCfgLastNameMgr, XCfgLastName> { };

partial class XCfgLastName : ITabItem
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_LastName = "LastName";

	public uint Index { get; private set; }				// 序号ID
	public string LastName { get; private set; }				// 姓

	public XCfgLastName()
	{
	}

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		LastName = tf.Get<string>(_KEY_LastName);
		return true;
	}
}

