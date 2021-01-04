
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgFilterWordMgr : CCfg1KeyMgrTemplate<XCfgFilterWordMgr, uint, XCfgFilterWord> { };

partial class XCfgFilterWord : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_FilterWord = "FilterWord";

	public uint Index { get; private set; }				// 序号ID
	public string FilterWord { get; private set; }				// 姓

	public XCfgFilterWord()
	{
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		FilterWord = tf.Get<string>(_KEY_FilterWord);
		return true;
	}
}

