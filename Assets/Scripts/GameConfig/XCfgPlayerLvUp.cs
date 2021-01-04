
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPlayerLvUpMgr : CCfg1KeyMgrTemplate<XCfgPlayerLvUpMgr, uint, XCfgPlayerLvUp> { };

partial class XCfgPlayerLvUp : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_MaxExp = "MaxExp";
	public static readonly string _KEY_MaxPetNum = "MaxPetNum";
	public static readonly string _KEY_MaxBattleUnit = "MaxBattleUnit";
	public static readonly string _KEY_MeditationValue = "MeditationValue";

	public uint Level { get; private set; }				// 等级
	public uint MaxExp { get; private set; }				// 升到下一级所需经验
	public byte MaxPetNum { get; private set; }				// 可携带宠物数量上限
	public byte MaxBattleUnit { get; private set; }				// 上阵人数上限
	public uint MeditationValue { get; private set; }				// 打坐经验值

	public XCfgPlayerLvUp()
	{
	}

	public uint GetKey1() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Level = tf.Get<uint>(_KEY_Level);
		MaxExp = tf.Get<uint>(_KEY_MaxExp);
		MaxPetNum = tf.Get<byte>(_KEY_MaxPetNum);
		MaxBattleUnit = tf.Get<byte>(_KEY_MaxBattleUnit);
		MeditationValue = tf.Get<uint>(_KEY_MeditationValue);
		return true;
	}
}

