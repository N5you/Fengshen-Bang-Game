
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XPKAwardMgr : CCfg1KeyMgrTemplate<XPKAwardMgr, uint, XPKAward> { };

partial class XPKAward : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_RankID = "RankID";
	public static readonly string _KEY_RankIDLimit = "RankIDLimit";
	public static readonly string _KEY_GameMoney = "GameMoney";
	public static readonly string _KEY_Honour = "Honour";
	public static readonly string _KEY_ItemID = "ItemID";
	public static readonly string _KEY_ItemCount = "ItemCount";
	public static readonly string _KEY_TitleID = "TitleID";

	public uint RankID { get; private set; }				// 挑战者名次上限
	public uint RankIDLimit { get; private set; }				// 挑战者名次下限
	public uint GameMoney { get; private set; }				// 奖励铜钱
	public uint Honour { get; private set; }				// 奖励荣誉
	public uint ItemID { get; private set; }				// 奖励物品ID
	public uint ItemCount { get; private set; }				// 物品个数
	public uint TitleID { get; private set; }				// 称号ID

	public XPKAward()
	{
	}

	public uint GetKey1() { return RankID; }

	public bool ReadItem(TabFile tf)
	{
		RankID = tf.Get<uint>(_KEY_RankID);
		RankIDLimit = tf.Get<uint>(_KEY_RankIDLimit);
		GameMoney = tf.Get<uint>(_KEY_GameMoney);
		Honour = tf.Get<uint>(_KEY_Honour);
		ItemID = tf.Get<uint>(_KEY_ItemID);
		ItemCount = tf.Get<uint>(_KEY_ItemCount);
		TitleID = tf.Get<uint>(_KEY_TitleID);
		return true;
	}
}

