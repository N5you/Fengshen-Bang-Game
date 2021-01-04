
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSkillOperMgr : CCfgListMgrTemplate<XCfgSkillOperMgr, XCfgSkillOper> { };

partial class XCfgSkillOper : ITabItem
{
	public static readonly string _KEY_Class = "Class";
	public static readonly string _KEY_SkillID = "SkillID";
	public static readonly string _KEY_SkillLevel = "SkillLevel";
	public static readonly string _KEY_PreID = "PreID";
	public static readonly string _KEY_PreLevel = "PreLevel";
	public static readonly string _KEY_PosX = "PosX";
	public static readonly string _KEY_PosY = "PosY";
	public static readonly string _KEY_ClassLevel = "ClassLevel";
	public static readonly string _KEY_SkillPoint = "SkillPoint";
	public static readonly string _KEY_FieldID = "FieldID";
	public static readonly string _KEY_StarSprite = "StarSprite";
	public static readonly string _KEY_NotActiveSprite = "NotActiveSprite";

	public byte Class { get; private set; }				// 职业
	public ushort SkillID { get; private set; }				// 技能ID
	public byte SkillLevel { get; private set; }				// 技能等级
	public ushort PreID { get; private set; }				// 前置技能ID
	public byte PreLevel { get; private set; }				// 前置技能等级
	public float PosX { get; private set; }				// 位置X
	public float PosY { get; private set; }				// 位置Y
	public uint ClassLevel { get; private set; }				// 职业阶级
	public uint SkillPoint { get; private set; }				// 技能点消耗
	public byte FieldID { get; private set; }				// 技能栏ID
	public string StarSprite { get; private set; }				// 星图
	public string NotActiveSprite { get; private set; }				// 

	public XCfgSkillOper()
	{
	}

	public bool ReadItem(TabFile tf)
	{
		Class = tf.Get<byte>(_KEY_Class);
		SkillID = tf.Get<ushort>(_KEY_SkillID);
		SkillLevel = tf.Get<byte>(_KEY_SkillLevel);
		PreID = tf.Get<ushort>(_KEY_PreID);
		PreLevel = tf.Get<byte>(_KEY_PreLevel);
		PosX = tf.Get<float>(_KEY_PosX);
		PosY = tf.Get<float>(_KEY_PosY);
		ClassLevel = tf.Get<uint>(_KEY_ClassLevel);
		SkillPoint = tf.Get<uint>(_KEY_SkillPoint);
		FieldID = tf.Get<byte>(_KEY_FieldID);
		StarSprite = tf.Get<string>(_KEY_StarSprite);
		NotActiveSprite = tf.Get<string>(_KEY_NotActiveSprite);
		return true;
	}
}

