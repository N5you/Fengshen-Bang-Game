
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgColorValueMgr : CCfg1KeyMgrTemplate<XCfgColorValueMgr, uint, XCfgColorValue> { };

partial class XCfgColorValue : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ColorID = "ColorID";
	public static readonly string _KEY_MagicRate = "MagicRate";
	public static readonly string _KEY_BaseRate = "BaseRate";
	public static readonly string _KEY_PriceRate = "PriceRate";

	public uint ColorID { get; private set; }				// 成色ID
	public float MagicRate { get; private set; }				// 成色对魔法属性的调整比率
	public float BaseRate { get; private set; }				// 成色对基础属性的调整比率
	public float PriceRate { get; private set; }				// 随机成色物品的商店售价修正系数

	public XCfgColorValue()
	{
	}

	public uint GetKey1() { return ColorID; }

	public bool ReadItem(TabFile tf)
	{
		ColorID = tf.Get<uint>(_KEY_ColorID);
		MagicRate = tf.Get<float>(_KEY_MagicRate);
		BaseRate = tf.Get<float>(_KEY_BaseRate);
		PriceRate = tf.Get<float>(_KEY_PriceRate);
		return true;
	}
}

