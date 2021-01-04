
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgShopItemMgr : CCfg2KeyMgrTemplate<XCfgShopItemMgr, uint, uint, XCfgShopItem> { };

partial class XCfgShopItem : ITabItemWith2Key<uint, uint>
{
	public static readonly string _KEY_NPCID = "NPCID";
	public static readonly string _KEY_itemID = "itemID";
	public static readonly string _KEY_maxNum = "maxNum";
	public static readonly string _KEY_needVIPLevel = "needVIPLevel";
	public static readonly string _KEY_sellType = "sellType";
	public static readonly string _KEY_specialCoinType = "specialCoinType";
	public static readonly string _KEY_specialCoinHowMuch = "specialCoinHowMuch";
	public static readonly string _KEY_convertItemID = "convertItemID";
	public static readonly string _KEY_convertItemCount = "convertItemCount";
	public static readonly string _KEY_Sequencs = "Sequencs";

	public uint NPCID { get; private set; }				// //出售物品的NPC
	public uint itemID { get; private set; }				// //物品的基本ID
	public uint maxNum { get; private set; }				// //如果有最大物品数量就开启限制功能
	public uint needVIPLevel { get; private set; }				// //购买所需要的VIP等级
	public uint sellType { get; private set; }				// //出售类型。0表示按照物品的模板方式出售，1表示按照表中配置的特殊货币价格出售，2表示按照表中配置的兑换物兑换。
	public uint specialCoinType { get; private set; }				// //具体指定的特定货币类型0铜币，1元宝，2声望
	public uint specialCoinHowMuch { get; private set; }				// //具体指定的货币数量
	public uint convertItemID { get; private set; }				// //兑换物品的ID
	public uint convertItemCount { get; private set; }				// //兑换物品的数量
	public uint Sequencs { get; private set; }				// //商品在商店中的排序顺序

	public XCfgShopItem()
	{
	}

	public uint GetKey1() { return NPCID; }

	public uint GetKey2() { return itemID; }

	public bool ReadItem(TabFile tf)
	{
		NPCID = tf.Get<uint>(_KEY_NPCID);
		itemID = tf.Get<uint>(_KEY_itemID);
		maxNum = tf.Get<uint>(_KEY_maxNum);
		needVIPLevel = tf.Get<uint>(_KEY_needVIPLevel);
		sellType = tf.Get<uint>(_KEY_sellType);
		specialCoinType = tf.Get<uint>(_KEY_specialCoinType);
		specialCoinHowMuch = tf.Get<uint>(_KEY_specialCoinHowMuch);
		convertItemID = tf.Get<uint>(_KEY_convertItemID);
		convertItemCount = tf.Get<uint>(_KEY_convertItemCount);
		Sequencs = tf.Get<uint>(_KEY_Sequencs);
		return true;
	}
}

