
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPlayHintMgr : CCfgListMgrTemplate<XCfgPlayHintMgr, XCfgPlayHint> { };

partial class XCfgPlayHint : ITabItem
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_OpenLevel = "OpenLevel";
	public static readonly string _KEY_OpenQuestID = "OpenQuestID";
	public static readonly string _KEY_OpenQuestName = "OpenQuestName";
	public static readonly string _KEY_OpenPlayName = "OpenPlayName";
	public static readonly string _KEY_AtlasID = "AtlasID";
	public static readonly string _KEY_SpriteName = "SpriteName";

	public uint ID { get; private set; }				// 开放顺序
	public uint OpenLevel { get; private set; }				// 开放等级
	public uint OpenQuestID { get; private set; }				// 开放需要任务ID
	public string OpenQuestName { get; private set; }				// 开放任务名
	public string OpenPlayName { get; private set; }				// 开放玩法名
	public uint AtlasID { get; private set; }				// 图集ID
	public string SpriteName { get; private set; }				// 图片名字

	public XCfgPlayHint()
	{
	}

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		OpenLevel = tf.Get<uint>(_KEY_OpenLevel);
		OpenQuestID = tf.Get<uint>(_KEY_OpenQuestID);
		OpenQuestName = tf.Get<string>(_KEY_OpenQuestName);
		OpenPlayName = tf.Get<string>(_KEY_OpenPlayName);
		AtlasID = tf.Get<uint>(_KEY_AtlasID);
		SpriteName = tf.Get<string>(_KEY_SpriteName);
		return true;
	}
}

