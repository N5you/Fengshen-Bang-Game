
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBuffLevelMgr : CCfg2KeyMgrTemplate<XCfgBuffLevelMgr, uint, byte, XCfgBuffLevel> { };

partial class XCfgBuffLevel : ITabItemWith2Key<uint, byte>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_BuffDiscription = "BuffDiscription";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_UColor = "UColor";
	public static readonly string _KEY_Usize = "Usize";
	public static readonly string _KEY_MagicAttrType_6_0 = "MagicAttrType_6_0";
	public static readonly string _KEY_MagicAttrValue_6_0 = "MagicAttrValue_6_0";
	public static readonly string _KEY_MagicAttrType_6_1 = "MagicAttrType_6_1";
	public static readonly string _KEY_MagicAttrValue_6_1 = "MagicAttrValue_6_1";
	public static readonly string _KEY_MagicAttrType_6_2 = "MagicAttrType_6_2";
	public static readonly string _KEY_MagicAttrValue_6_2 = "MagicAttrValue_6_2";
	public static readonly string _KEY_MagicAttrType_6_3 = "MagicAttrType_6_3";
	public static readonly string _KEY_MagicAttrValue_6_3 = "MagicAttrValue_6_3";
	public static readonly string _KEY_MagicAttrType_6_4 = "MagicAttrType_6_4";
	public static readonly string _KEY_MagicAttrValue_6_4 = "MagicAttrValue_6_4";
	public static readonly string _KEY_MagicAttrType_6_5 = "MagicAttrType_6_5";
	public static readonly string _KEY_MagicAttrValue_6_5 = "MagicAttrValue_6_5";

	public uint Index { get; private set; }				// buff_ID
	public byte Level { get; private set; }				// buff等级
	public string BuffDiscription { get; private set; }				// buff效果描述
	public uint ModelId { get; private set; }				// 模型变更
	public string UColor { get; private set; }				// 模型染色值变更
	public float Usize { get; private set; }				// 模型大小变化
	public ushort[] MagicAttrType { get; private set; }				// 魔法属性类型
	public int[] MagicAttrValue { get; private set; }				// 魔法属性值

	public XCfgBuffLevel()
	{
		MagicAttrType = new ushort[6];
		MagicAttrValue = new int[6];
	}

	public uint GetKey1() { return Index; }

	public byte GetKey2() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Level = tf.Get<byte>(_KEY_Level);
		BuffDiscription = tf.Get<string>(_KEY_BuffDiscription);
		ModelId = tf.Get<uint>(_KEY_ModelId);
		UColor = tf.Get<string>(_KEY_UColor);
		Usize = tf.Get<float>(_KEY_Usize);
		MagicAttrType[0] = tf.Get<ushort>(_KEY_MagicAttrType_6_0);
		MagicAttrValue[0] = tf.Get<int>(_KEY_MagicAttrValue_6_0);
		MagicAttrType[1] = tf.Get<ushort>(_KEY_MagicAttrType_6_1);
		MagicAttrValue[1] = tf.Get<int>(_KEY_MagicAttrValue_6_1);
		MagicAttrType[2] = tf.Get<ushort>(_KEY_MagicAttrType_6_2);
		MagicAttrValue[2] = tf.Get<int>(_KEY_MagicAttrValue_6_2);
		MagicAttrType[3] = tf.Get<ushort>(_KEY_MagicAttrType_6_3);
		MagicAttrValue[3] = tf.Get<int>(_KEY_MagicAttrValue_6_3);
		MagicAttrType[4] = tf.Get<ushort>(_KEY_MagicAttrType_6_4);
		MagicAttrValue[4] = tf.Get<int>(_KEY_MagicAttrValue_6_4);
		MagicAttrType[5] = tf.Get<ushort>(_KEY_MagicAttrType_6_5);
		MagicAttrValue[5] = tf.Get<int>(_KEY_MagicAttrValue_6_5);
		return true;
	}
}

