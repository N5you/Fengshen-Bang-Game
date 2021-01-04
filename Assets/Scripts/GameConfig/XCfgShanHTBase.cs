
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgShanHTBaseMgr : CCfg1KeyMgrTemplate<XCfgShanHTBaseMgr, uint, XCfgShanHTBase> { };

partial class XCfgShanHTBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_StartIndex = "StartIndex";
	public static readonly string _KEY_EndIndex = "EndIndex";
	public static readonly string _KEY_TotemLight = "TotemLight";
	public static readonly string _KEY_TotemDark = "TotemDark";
	public static readonly string _KEY_NameSprite = "NameSprite";

	public uint Index { get; private set; }				// 山脉ID(唯一)
	public string Name { get; private set; }				// 山脉名称
	public uint Level { get; private set; }				// 山脉等级
	public uint StartIndex { get; private set; }				// 开始关卡
	public uint EndIndex { get; private set; }				// 结束关卡
	public uint TotemLight { get; private set; }				// 背景图案亮
	public uint TotemDark { get; private set; }				// 背景图案暗
	public uint NameSprite { get; private set; }				// 文字图案

	public XCfgShanHTBase()
	{
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		Level = tf.Get<uint>(_KEY_Level);
		StartIndex = tf.Get<uint>(_KEY_StartIndex);
		EndIndex = tf.Get<uint>(_KEY_EndIndex);
		TotemLight = tf.Get<uint>(_KEY_TotemLight);
		TotemDark = tf.Get<uint>(_KEY_TotemDark);
		NameSprite = tf.Get<uint>(_KEY_NameSprite);
		return true;
	}
}

