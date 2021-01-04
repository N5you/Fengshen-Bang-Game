
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMountAttrMgr : CCfg1KeyMgrTemplate<XCfgMountAttrMgr, ushort, XCfgMountAttr> { };

partial class XCfgMountAttr : ITabItemWith1Key<ushort>
{
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_NeedExp = "NeedExp";
	public static readonly string _KEY_WuLi = "WuLi";
	public static readonly string _KEY_LingQiao = "LingQiao";
	public static readonly string _KEY_TiZhi = "TiZhi";
	public static readonly string _KEY_ShuFa = "ShuFa";
	public static readonly string _KEY_TongLing = "TongLing";
	public static readonly string _KEY_Speed = "Speed";
	public static readonly string _KEY_CostMoney = "CostMoney";
	public static readonly string _KEY_CostIngot = "CostIngot";

	public ushort Level { get; private set; }				// 等级
	public uint NeedExp { get; private set; }				// 所需经验值
	public ushort WuLi { get; private set; }				// 武力加成
	public ushort LingQiao { get; private set; }				// 灵巧加成
	public short TiZhi { get; private set; }				// 体质加成
	public ushort ShuFa { get; private set; }				// 术法加成
	public ushort TongLing { get; private set; }				// 统领加成
	public float Speed { get; private set; }				// 速度加成
	public uint CostMoney { get; private set; }				// 普通培养消耗（单位铜币）
	public ushort CostIngot { get; private set; }				// 超级培养消耗（单位元宝）

	public XCfgMountAttr()
	{
	}

	public ushort GetKey1() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Level = tf.Get<ushort>(_KEY_Level);
		NeedExp = tf.Get<uint>(_KEY_NeedExp);
		WuLi = tf.Get<ushort>(_KEY_WuLi);
		LingQiao = tf.Get<ushort>(_KEY_LingQiao);
		TiZhi = tf.Get<short>(_KEY_TiZhi);
		ShuFa = tf.Get<ushort>(_KEY_ShuFa);
		TongLing = tf.Get<ushort>(_KEY_TongLing);
		Speed = tf.Get<float>(_KEY_Speed);
		CostMoney = tf.Get<uint>(_KEY_CostMoney);
		CostIngot = tf.Get<ushort>(_KEY_CostIngot);
		return true;
	}
}

