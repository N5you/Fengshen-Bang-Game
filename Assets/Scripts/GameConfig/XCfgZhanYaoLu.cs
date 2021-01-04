
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgZhanYaoLuMgr : CCfg1KeyMgrTemplate<XCfgZhanYaoLuMgr, uint, XCfgZhanYaoLu> { };

partial class XCfgZhanYaoLu : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_NameColor = "NameColor";
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_MonsterModel = "MonsterModel";
	public static readonly string _KEY_Reputation = "Reputation";
	public static readonly string _KEY_Money = "Money";
	public static readonly string _KEY_MinLevel = "MinLevel";
	public static readonly string _KEY_MaxLevel = "MaxLevel";
	public static readonly string _KEY_AtlasID = "AtlasID";
	public static readonly string _KEY_SpriteID = "SpriteID";
	public static readonly string _KEY_MonID_9_0 = "MonID_9_0";
	public static readonly string _KEY_MonID_9_1 = "MonID_9_1";
	public static readonly string _KEY_MonID_9_2 = "MonID_9_2";
	public static readonly string _KEY_MonID_9_3 = "MonID_9_3";
	public static readonly string _KEY_MonID_9_4 = "MonID_9_4";
	public static readonly string _KEY_MonID_9_5 = "MonID_9_5";
	public static readonly string _KEY_MonID_9_6 = "MonID_9_6";
	public static readonly string _KEY_MonID_9_7 = "MonID_9_7";
	public static readonly string _KEY_MonID_9_8 = "MonID_9_8";

	public uint Index { get; private set; }				// 关卡ID(唯一标识符)
	public string Name { get; private set; }				// 怪物名称
	public string NameColor { get; private set; }				// 名称成色值
	public uint SceneID { get; private set; }				// 战斗场景ID
	public uint MonsterModel { get; private set; }				// 怪物模型
	public uint Reputation { get; private set; }				// 奖励声望
	public uint Money { get; private set; }				// 奖励铜钱
	public uint MinLevel { get; private set; }				// T开启本关卡所需要的最低角色等级
	public uint MaxLevel { get; private set; }				// 开启本关卡所需要的最高角色等级
	public uint AtlasID { get; private set; }				// 底图图集ID
	public uint SpriteID { get; private set; }				// 底图图片ID
	public uint[] MonID { get; private set; }				// 位置1怪物ID

	public XCfgZhanYaoLu()
	{
		MonID = new uint[9];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		NameColor = tf.Get<string>(_KEY_NameColor);
		SceneID = tf.Get<uint>(_KEY_SceneID);
		MonsterModel = tf.Get<uint>(_KEY_MonsterModel);
		Reputation = tf.Get<uint>(_KEY_Reputation);
		Money = tf.Get<uint>(_KEY_Money);
		MinLevel = tf.Get<uint>(_KEY_MinLevel);
		MaxLevel = tf.Get<uint>(_KEY_MaxLevel);
		AtlasID = tf.Get<uint>(_KEY_AtlasID);
		SpriteID = tf.Get<uint>(_KEY_SpriteID);
		MonID[0] = tf.Get<uint>(_KEY_MonID_9_0);
		MonID[1] = tf.Get<uint>(_KEY_MonID_9_1);
		MonID[2] = tf.Get<uint>(_KEY_MonID_9_2);
		MonID[3] = tf.Get<uint>(_KEY_MonID_9_3);
		MonID[4] = tf.Get<uint>(_KEY_MonID_9_4);
		MonID[5] = tf.Get<uint>(_KEY_MonID_9_5);
		MonID[6] = tf.Get<uint>(_KEY_MonID_9_6);
		MonID[7] = tf.Get<uint>(_KEY_MonID_9_7);
		MonID[8] = tf.Get<uint>(_KEY_MonID_9_8);
		return true;
	}
}

