
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgGameParamMgr : CCfg1KeyMgrTemplate<XCfgGameParamMgr, uint, XCfgGameParam> { };

partial class XCfgGameParam : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Value = "Value";

	public uint ID { get; private set; }				// 变量枚举
	public float Value { get; private set; }				// 参数值

	public XCfgGameParam()
	{
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		Value = tf.Get<float>(_KEY_Value);
		return true;
	}
}

