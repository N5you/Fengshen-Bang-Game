
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgDailySignMgr : CCfg1KeyMgrTemplate<XCfgDailySignMgr, byte, XCfgDailySign> { };

partial class XCfgDailySign : ITabItemWith1Key<byte>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_AwardType = "AwardType";
	public static readonly string _KEY_ColorLevel = "ColorLevel";
	public static readonly string _KEY_IconID = "IconID";
	public static readonly string _KEY_Tips1 = "Tips1";
	public static readonly string _KEY_Tips2 = "Tips2";
	public static readonly string _KEY_Tips3 = "Tips3";
	public static readonly string _KEY_Exp = "Exp";
	public static readonly string _KEY_GameMoney = "GameMoney";
	public static readonly string _KEY_RealMoney = "RealMoney";
	public static readonly string _KEY_Prestige = "Prestige";
	public static readonly string _KEY_awardWarrior1 = "awardWarrior1";
	public static readonly string _KEY_awardMage1 = "awardMage1";
	public static readonly string _KEY_awardArcher1 = "awardArcher1";
	public static readonly string _KEY_awardNum1 = "awardNum1";

	public byte Index { get; private set; }				// 每日id
	public uint AwardType { get; private set; }				// 奖励类型
	public uint ColorLevel { get; private set; }				// 成色
	public uint IconID { get; private set; }				// 图集ID
	public string Tips1 { get; private set; }				// 战士奖励提示
	public string Tips2 { get; private set; }				// 法师奖励提示
	public string Tips3 { get; private set; }				// 弓手奖励提示
	public uint Exp { get; private set; }				// 经验
	public uint GameMoney { get; private set; }				// 游戏币
	public uint RealMoney { get; private set; }				// 元宝
	public uint Prestige { get; private set; }				// 声望
	public uint awardWarrior1 { get; private set; }				// 战士奖励物品
	public uint awardMage1 { get; private set; }				// 法师奖励物品
	public uint awardArcher1 { get; private set; }				// 弓手奖励物品
	public uint awardNum1 { get; private set; }				// 奖励物品数量

	public XCfgDailySign()
	{
	}

	public byte GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<byte>(_KEY_Index);
		AwardType = tf.Get<uint>(_KEY_AwardType);
		ColorLevel = tf.Get<uint>(_KEY_ColorLevel);
		IconID = tf.Get<uint>(_KEY_IconID);
		Tips1 = tf.Get<string>(_KEY_Tips1);
		Tips2 = tf.Get<string>(_KEY_Tips2);
		Tips3 = tf.Get<string>(_KEY_Tips3);
		Exp = tf.Get<uint>(_KEY_Exp);
		GameMoney = tf.Get<uint>(_KEY_GameMoney);
		RealMoney = tf.Get<uint>(_KEY_RealMoney);
		Prestige = tf.Get<uint>(_KEY_Prestige);
		awardWarrior1 = tf.Get<uint>(_KEY_awardWarrior1);
		awardMage1 = tf.Get<uint>(_KEY_awardMage1);
		awardArcher1 = tf.Get<uint>(_KEY_awardArcher1);
		awardNum1 = tf.Get<uint>(_KEY_awardNum1);
		return true;
	}
}

