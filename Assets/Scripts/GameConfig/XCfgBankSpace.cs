
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBankSpaceMgr : CCfg1KeyMgrTemplate<XCfgBankSpaceMgr, uint, XCfgBankSpace> { };

partial class XCfgBankSpace : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_SlotID = "SlotID";
	public static readonly string _KEY_State = "State";
	public static readonly string _KEY_Price = "Price";
	public static readonly string _KEY_PageID = "PageID";
	public static readonly string _KEY_PagePrice = "PagePrice";

	public uint SlotID { get; private set; }				// 仓库槽ID
	public byte State { get; private set; }				// 槽初始状态(0未开启，1是开启)
	public uint Price { get; private set; }				// 购买价格
	public uint PageID { get; private set; }				// 页ID
	public uint PagePrice { get; private set; }				// 页价格

	public XCfgBankSpace()
	{
	}

	public uint GetKey1() { return SlotID; }

	public bool ReadItem(TabFile tf)
	{
		SlotID = tf.Get<uint>(_KEY_SlotID);
		State = tf.Get<byte>(_KEY_State);
		Price = tf.Get<uint>(_KEY_Price);
		PageID = tf.Get<uint>(_KEY_PageID);
		PagePrice = tf.Get<uint>(_KEY_PagePrice);
		return true;
	}
}

