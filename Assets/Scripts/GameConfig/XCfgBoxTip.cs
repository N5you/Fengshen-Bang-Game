
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBoxTipMgr : CCfg1KeyMgrTemplate<XCfgBoxTipMgr, uint, XCfgBoxTip> { };

partial class XCfgBoxTip : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Content = "Content";

	public uint ID { get; private set; }				// 索引
	public string Content { get; private set; }				// 字符串内容

	public XCfgBoxTip()
	{
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		Content = tf.Get<string>(_KEY_Content);
		return true;
	}
}

