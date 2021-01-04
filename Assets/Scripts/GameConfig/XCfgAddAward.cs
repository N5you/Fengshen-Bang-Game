
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgAddAwardMgr : CCfg1KeyMgrTemplate<XCfgAddAwardMgr, uint, XCfgAddAward> { };

partial class XCfgAddAward : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Content = "Content";
	public static readonly string _KEY_AtlasID = "AtlasID";
	public static readonly string _KEY_BtnCom = "BtnCom";
	public static readonly string _KEY_BtnHover = "BtnHover";
	public static readonly string _KEY_BtnPress = "BtnPress";
	public static readonly string _KEY_Tip = "Tip";

	public uint ID { get; private set; }				// 索引
	public string Content { get; private set; }				// 字符串内容
	public uint AtlasID { get; private set; }				// 图集
	public string BtnCom { get; private set; }				// 按钮普通态
	public string BtnHover { get; private set; }				// 按钮高亮态
	public string BtnPress { get; private set; }				// 按钮按下态
	public string Tip { get; private set; }				// 点击后弹出的提示信息

	public XCfgAddAward()
	{
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		Content = tf.Get<string>(_KEY_Content);
		AtlasID = tf.Get<uint>(_KEY_AtlasID);
		BtnCom = tf.Get<string>(_KEY_BtnCom);
		BtnHover = tf.Get<string>(_KEY_BtnHover);
		BtnPress = tf.Get<string>(_KEY_BtnPress);
		Tip = tf.Get<string>(_KEY_Tip);
		return true;
	}
}

