
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgVipBaseMgr : CCfg1KeyMgrTemplate<XCfgVipBaseMgr, uint, XCfgVipBase> { };

partial class XCfgVipBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_VipLvl = "VipLvl";
	public static readonly string _KEY_BuyTiliCount = "BuyTiliCount";
	public static readonly string _KEY_BuyGivefowCount = "BuyGivefowCount";
	public static readonly string _KEY_BuyYaoqCount = "BuyYaoqCount";
	public static readonly string _KEY_BuyShhjKillCount = "BuyShhjKillCount";
	public static readonly string _KEY_BuyZhylCount = "BuyZhylCount";
	public static readonly string _KEY_BuyXdhCombatCount = "BuyXdhCombatCount";
	public static readonly string _KEY_FreeXdhCombatCount = "FreeXdhCombatCount";
	public static readonly string _KEY_BuyAucPutawayCount = "BuyAucPutawayCount";
	public static readonly string _KEY_BuyShhCombatCount = "BuyShhCombatCount";
	public static readonly string _KEY_BuyJlCount = "BuyJlCount";
	public static readonly string _KEY_FreeShhCombatCount = "FreeShhCombatCount";
	public static readonly string _KEY_BeSkipCombatAni = "BeSkipCombatAni";
	public static readonly string _KEY_SitExpMul = "SitExpMul";
	public static readonly string _KEY_SignMoneyMul = "SignMoneyMul";
	public static readonly string _KEY_BeUseLd = "BeUseLd";
	public static readonly string _KEY_FriMax = "FriMax";
	public static readonly string _KEY_BlaMax = "BlaMax";

	public uint VipLvl { get; private set; }				// //vip等级
	public uint BuyTiliCount { get; private set; }				// //购买体力次数
	public uint BuyGivefowCount { get; private set; }				// //购买赠送鲜花次数
	public uint BuyYaoqCount { get; private set; }				// //购买摇钱树次数
	public uint BuyShhjKillCount { get; private set; }				// //购买山海经斩杀次数
	public uint BuyZhylCount { get; private set; }				// //购买斩妖录斩杀次数
	public uint BuyXdhCombatCount { get; private set; }				// 购买仙道会挑战次数
	public uint FreeXdhCombatCount { get; private set; }				// //仙道会免费挑战次数
	public uint BuyAucPutawayCount { get; private set; }				// //购买拍卖行上架次数
	public uint BuyShhCombatCount { get; private set; }				// //购买山河社稷图挑战次数
	public uint BuyJlCount { get; private set; }				// //购买精力次数
	public uint FreeShhCombatCount { get; private set; }				// //山河社稷图免费进入次数
	public uint BeSkipCombatAni { get; private set; }				// //开启跳过战斗功能
	public float SitExpMul { get; private set; }				// //打坐经验获取倍率
	public uint SignMoneyMul { get; private set; }				// //7天签到铜钱奖励倍数
	public uint BeUseLd { get; private set; }				// //开启元宝直接服用灵丹
	public uint FriMax { get; private set; }				// //好友上限
	public uint BlaMax { get; private set; }				// //黑名单上限

	public XCfgVipBase()
	{
	}

	public uint GetKey1() { return VipLvl; }

	public bool ReadItem(TabFile tf)
	{
		VipLvl = tf.Get<uint>(_KEY_VipLvl);
		BuyTiliCount = tf.Get<uint>(_KEY_BuyTiliCount);
		BuyGivefowCount = tf.Get<uint>(_KEY_BuyGivefowCount);
		BuyYaoqCount = tf.Get<uint>(_KEY_BuyYaoqCount);
		BuyShhjKillCount = tf.Get<uint>(_KEY_BuyShhjKillCount);
		BuyZhylCount = tf.Get<uint>(_KEY_BuyZhylCount);
		BuyXdhCombatCount = tf.Get<uint>(_KEY_BuyXdhCombatCount);
		FreeXdhCombatCount = tf.Get<uint>(_KEY_FreeXdhCombatCount);
		BuyAucPutawayCount = tf.Get<uint>(_KEY_BuyAucPutawayCount);
		BuyShhCombatCount = tf.Get<uint>(_KEY_BuyShhCombatCount);
		BuyJlCount = tf.Get<uint>(_KEY_BuyJlCount);
		FreeShhCombatCount = tf.Get<uint>(_KEY_FreeShhCombatCount);
		BeSkipCombatAni = tf.Get<uint>(_KEY_BeSkipCombatAni);
		SitExpMul = tf.Get<float>(_KEY_SitExpMul);
		SignMoneyMul = tf.Get<uint>(_KEY_SignMoneyMul);
		BeUseLd = tf.Get<uint>(_KEY_BeUseLd);
		FriMax = tf.Get<uint>(_KEY_FriMax);
		BlaMax = tf.Get<uint>(_KEY_BlaMax);
		return true;
	}
}

