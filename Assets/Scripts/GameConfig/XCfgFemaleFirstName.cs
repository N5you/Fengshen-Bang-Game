
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgFemaleFirstNameMgr : CCfgListMgrTemplate<XCfgFemaleFirstNameMgr, XCfgFemaleFirstName> { };

partial class XCfgFemaleFirstName : ITabItem
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_FirstName = "FirstName";

	public uint Index { get; private set; }				// 序号ID
	public string FirstName { get; private set; }				// 姓

	public XCfgFemaleFirstName()
	{
	}

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		FirstName = tf.Get<string>(_KEY_FirstName);
		return true;
	}
}

