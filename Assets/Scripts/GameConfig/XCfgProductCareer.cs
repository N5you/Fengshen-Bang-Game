
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgProductCareerMgr : CCfg2KeyMgrTemplate<XCfgProductCareerMgr, byte, byte, XCfgProductCareer> { };

partial class XCfgProductCareer : ITabItemWith2Key<byte, byte>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_Kind = "Kind";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_NeedPlayerLevel = "NeedPlayerLevel";
	public static readonly string _KEY_NeedExp = "NeedExp";
	public static readonly string _KEY_MaxExpLimit = "MaxExpLimit";
	public static readonly string _KEY_AnimName = "AnimName";
	public static readonly string _KEY_DefaultIcon = "DefaultIcon";
	public static readonly string _KEY_SelectIcon = "SelectIcon";
	public static readonly string _KEY_RelativeID1 = "RelativeID1";
	public static readonly string _KEY_RelativeID2 = "RelativeID2";
	public static readonly string _KEY_Discription = "Discription";

	public byte ID { get; private set; }				// 专业ID
	public byte Level { get; private set; }				// 专业等级
	public byte Kind { get; private set; }				// 专业类别 0:采集, 1:制造
	public string Name { get; private set; }				// 专业名
	public uint NeedPlayerLevel { get; private set; }				// 需要角色等级
	public int NeedExp { get; private set; }				// 需要熟练度
	public int MaxExpLimit { get; private set; }				// 熟练度最大值限制
	public int AnimName { get; private set; }				// 采集动画
	public string DefaultIcon { get; private set; }				// 默认显示Icon
	public string SelectIcon { get; private set; }				// 选中时Icon
	public byte RelativeID1 { get; private set; }				// 关联ID1
	public byte RelativeID2 { get; private set; }				// 关联ID2
	public string Discription { get; private set; }				// 描述

	public XCfgProductCareer()
	{
	}

	public byte GetKey1() { return ID; }

	public byte GetKey2() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<byte>(_KEY_ID);
		Level = tf.Get<byte>(_KEY_Level);
		Kind = tf.Get<byte>(_KEY_Kind);
		Name = tf.Get<string>(_KEY_Name);
		NeedPlayerLevel = tf.Get<uint>(_KEY_NeedPlayerLevel);
		NeedExp = tf.Get<int>(_KEY_NeedExp);
		MaxExpLimit = tf.Get<int>(_KEY_MaxExpLimit);
		AnimName = tf.Get<int>(_KEY_AnimName);
		DefaultIcon = tf.Get<string>(_KEY_DefaultIcon);
		SelectIcon = tf.Get<string>(_KEY_SelectIcon);
		RelativeID1 = tf.Get<byte>(_KEY_RelativeID1);
		RelativeID2 = tf.Get<byte>(_KEY_RelativeID2);
		Discription = tf.Get<string>(_KEY_Discription);
		return true;
	}
}

