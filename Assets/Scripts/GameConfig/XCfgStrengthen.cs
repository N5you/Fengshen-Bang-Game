
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgStrengthenMgr : CCfgListMgrTemplate<XCfgStrengthenMgr, XCfgStrengthen> { };

public partial class XCfgStrengthen : ITabItem
{
	public static readonly string _KEY_EquipColorLevel = "EquipColorLevel";
	public static readonly string _KEY_StrengthenLevel = "StrengthenLevel";
	public static readonly string _KEY_EquipLevel = "EquipLevel";
	public static readonly string _KEY_IsCanStrengthen = "IsCanStrengthen";
	public static readonly string _KEY_MaterialID_2_0 = "MaterialID_2_0";
	public static readonly string _KEY_MaterialNum_2_0 = "MaterialNum_2_0";
	public static readonly string _KEY_MaterialID_2_1 = "MaterialID_2_1";
	public static readonly string _KEY_MaterialNum_2_1 = "MaterialNum_2_1";
	public static readonly string _KEY_Rate = "Rate";
	public static readonly string _KEY_AfterLevel = "AfterLevel";
	public static readonly string _KEY_FailLevel = "FailLevel";
	public static readonly string _KEY_GameMoney = "GameMoney";
	public static readonly string _KEY_RealMoney = "RealMoney";
	public static readonly string _KEY_AttrRate = "AttrRate";
	public static readonly string _KEY_AppearLevel = "AppearLevel";
	public static readonly string _KEY_SlotNum = "SlotNum";

	public uint EquipColorLevel { get; private set; }				// 装备成色ID
	public byte StrengthenLevel { get; private set; }				// 强化等级
	public uint EquipLevel { get; private set; }				// 装备等级
	public byte IsCanStrengthen { get; private set; }				// 是否能强化(0为不可强化，1为可强化）
	public uint[] MaterialID { get; private set; }				// 材料1ID
	public uint[] MaterialNum { get; private set; }				// 材料1数量
	public float Rate { get; private set; }				// 成功率
	public byte AfterLevel { get; private set; }				// 强化后强化等级
	public byte FailLevel { get; private set; }				// 强化失败后强化等级
	public uint GameMoney { get; private set; }				// 铜币消耗
	public uint RealMoney { get; private set; }				// 元宝消耗
	public float AttrRate { get; private set; }				// 属性比例
	public byte AppearLevel { get; private set; }				// 外观等级(分为1-5级，1为无流光，2-5为依次增强的4种流光效果)
	public byte SlotNum { get; private set; }				// 凹槽数量

	public XCfgStrengthen()
	{
		MaterialID = new uint[2];
		MaterialNum = new uint[2];
	}

	public bool ReadItem(TabFile tf)
	{
		EquipColorLevel = tf.Get<uint>(_KEY_EquipColorLevel);
		StrengthenLevel = tf.Get<byte>(_KEY_StrengthenLevel);
		EquipLevel = tf.Get<uint>(_KEY_EquipLevel);
		IsCanStrengthen = tf.Get<byte>(_KEY_IsCanStrengthen);
		MaterialID[0] = tf.Get<uint>(_KEY_MaterialID_2_0);
		MaterialNum[0] = tf.Get<uint>(_KEY_MaterialNum_2_0);
		MaterialID[1] = tf.Get<uint>(_KEY_MaterialID_2_1);
		MaterialNum[1] = tf.Get<uint>(_KEY_MaterialNum_2_1);
		Rate = tf.Get<float>(_KEY_Rate);
		AfterLevel = tf.Get<byte>(_KEY_AfterLevel);
		FailLevel = tf.Get<byte>(_KEY_FailLevel);
		GameMoney = tf.Get<uint>(_KEY_GameMoney);
		RealMoney = tf.Get<uint>(_KEY_RealMoney);
		AttrRate = tf.Get<float>(_KEY_AttrRate);
		AppearLevel = tf.Get<byte>(_KEY_AppearLevel);
		SlotNum = tf.Get<byte>(_KEY_SlotNum);
		return true;
	}
}

