
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XPKChallengeMgr : CCfg1KeyMgrTemplate<XPKChallengeMgr, uint, XPKChallenge> { };

partial class XPKChallenge : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_RankID = "RankID";
	public static readonly string _KEY_RankIDLimit = "RankIDLimit";
	public static readonly string _KEY_Param = "Param";
	public static readonly string _KEY_GameMoneySucess = "GameMoneySucess";
	public static readonly string _KEY_HonourSucess = "HonourSucess";
	public static readonly string _KEY_GameMoneyFailed = "GameMoneyFailed";
	public static readonly string _KEY_HonourFailed = "HonourFailed";
	public static readonly string _KEY_BattleSceneID = "BattleSceneID";

	public uint RankID { get; private set; }				// 挑战者名次上限
	public uint RankIDLimit { get; private set; }				// 挑战者名次下限
	public uint Param { get; private set; }				// 挑战配置系数
	public uint GameMoneySucess { get; private set; }				// 成功奖励铜钱
	public uint HonourSucess { get; private set; }				// 成功奖励荣誉
	public uint GameMoneyFailed { get; private set; }				// 失败奖励铜钱
	public uint HonourFailed { get; private set; }				// 失败奖励荣誉
	public uint BattleSceneID { get; private set; }				// 战斗场景ID

	public XPKChallenge()
	{
	}

	public uint GetKey1() { return RankID; }

	public bool ReadItem(TabFile tf)
	{
		RankID = tf.Get<uint>(_KEY_RankID);
		RankIDLimit = tf.Get<uint>(_KEY_RankIDLimit);
		Param = tf.Get<uint>(_KEY_Param);
		GameMoneySucess = tf.Get<uint>(_KEY_GameMoneySucess);
		HonourSucess = tf.Get<uint>(_KEY_HonourSucess);
		GameMoneyFailed = tf.Get<uint>(_KEY_GameMoneyFailed);
		HonourFailed = tf.Get<uint>(_KEY_HonourFailed);
		BattleSceneID = tf.Get<uint>(_KEY_BattleSceneID);
		return true;
	}
}

