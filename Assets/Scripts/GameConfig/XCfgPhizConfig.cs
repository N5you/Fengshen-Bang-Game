
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPhizConfigMgr : CCfg1KeyMgrTemplate<XCfgPhizConfigMgr, int, XCfgPhizConfig> { };

partial class XCfgPhizConfig : ITabItemWith1Key<int>
{
	public static readonly string _KEY_Id = "Id";
	public static readonly string _KEY_Sprite = "Sprite";
	public static readonly string _KEY_Tip = "Tip";
	public static readonly string _KEY_Shortening = "Shortening";

	public int Id { get; private set; }				// 表情id
	public string Sprite { get; private set; }				// 表情图片
	public string Tip { get; private set; }				// 表情提示
	public string Shortening { get; private set; }				// 表情简写

	public XCfgPhizConfig()
	{
	}

	public int GetKey1() { return Id; }

	public bool ReadItem(TabFile tf)
	{
		Id = tf.Get<int>(_KEY_Id);
		Sprite = tf.Get<string>(_KEY_Sprite);
		Tip = tf.Get<string>(_KEY_Tip);
		Shortening = tf.Get<string>(_KEY_Shortening);
		return true;
	}
}

