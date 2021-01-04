
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgVipShowMgr : CCfg2KeyMgrTemplate<XCfgVipShowMgr, uint, uint, XCfgVipShow> { };

partial class XCfgVipShow : ITabItemWith2Key<uint, uint>
{
	public static readonly string _KEY_VipLvl = "VipLvl";
	public static readonly string _KEY_OrderId = "OrderId";
	public static readonly string _KEY_ItemDesc = "ItemDesc";
	public static readonly string _KEY_CurDesc = "CurDesc";
	public static readonly string _KEY_NextDesc = "NextDesc";

	public uint VipLvl { get; private set; }				// //Vip等级
	public uint OrderId { get; private set; }				// //显示顺序id
	public string ItemDesc { get; private set; }				// //项目描述
	public string CurDesc { get; private set; }				// //当前级状态
	public string NextDesc { get; private set; }				// //下级状态

	public XCfgVipShow()
	{
	}

	public uint GetKey1() { return VipLvl; }

	public uint GetKey2() { return OrderId; }

	public bool ReadItem(TabFile tf)
	{
		VipLvl = tf.Get<uint>(_KEY_VipLvl);
		OrderId = tf.Get<uint>(_KEY_OrderId);
		ItemDesc = tf.Get<string>(_KEY_ItemDesc);
		CurDesc = tf.Get<string>(_KEY_CurDesc);
		NextDesc = tf.Get<string>(_KEY_NextDesc);
		return true;
	}
}

