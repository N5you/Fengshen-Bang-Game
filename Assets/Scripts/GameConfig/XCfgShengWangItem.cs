
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgShengWangItemMgr : CCfg1KeyMgrTemplate<XCfgShengWangItemMgr, uint, XCfgShengWangItem> { };

partial class XCfgShengWangItem : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ItemID = "ItemID";
	public static readonly string _KEY_PrestigeLvShow = "PrestigeLvShow";
	public static readonly string _KEY_PrestigeLvBuy = "PrestigeLvBuy";
	public static readonly string _KEY_MoneyType = "MoneyType";
	public static readonly string _KEY_MoneyValue = "MoneyValue";
	public static readonly string _KEY_Sequencs = "Sequencs";

	public uint ItemID { get; private set; }				// //物品ID
	public uint PrestigeLvShow { get; private set; }				// //在商店中显示该物品需要的声望等级
	public uint PrestigeLvBuy { get; private set; }				// //购买该物品所需要的声望等级
	public uint MoneyType { get; private set; }				// //货币类型，1铜币，2元宝，3声望
	public uint MoneyValue { get; private set; }				// //花费的货币数量
	public uint Sequencs { get; private set; }				// //在商店中的排序，1为最右

	public XCfgShengWangItem()
	{
	}

	public uint GetKey1() { return ItemID; }

	public bool ReadItem(TabFile tf)
	{
		ItemID = tf.Get<uint>(_KEY_ItemID);
		PrestigeLvShow = tf.Get<uint>(_KEY_PrestigeLvShow);
		PrestigeLvBuy = tf.Get<uint>(_KEY_PrestigeLvBuy);
		MoneyType = tf.Get<uint>(_KEY_MoneyType);
		MoneyValue = tf.Get<uint>(_KEY_MoneyValue);
		Sequencs = tf.Get<uint>(_KEY_Sequencs);
		return true;
	}
}

