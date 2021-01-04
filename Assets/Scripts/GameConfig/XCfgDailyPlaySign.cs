
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgDailyPlaySignMgr : CCfg1KeyMgrTemplate<XCfgDailyPlaySignMgr, uint, XCfgDailyPlaySign> { };

partial class XCfgDailyPlaySign : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Key = "Key";
	public static readonly string _KEY_SortKey = "SortKey";
	public static readonly string _KEY_Text = "Text";

	public uint Key { get; private set; }				// 模块定义的id
	public uint SortKey { get; private set; }				// 每日玩法提示key，排序用,越小排在越前
	public string Text { get; private set; }				// 显示文本字符串，格式化

	public XCfgDailyPlaySign()
	{
	}

	public uint GetKey1() { return Key; }

	public bool ReadItem(TabFile tf)
	{
		Key = tf.Get<uint>(_KEY_Key);
		SortKey = tf.Get<uint>(_KEY_SortKey);
		Text = tf.Get<string>(_KEY_Text);
		return true;
	}
}

